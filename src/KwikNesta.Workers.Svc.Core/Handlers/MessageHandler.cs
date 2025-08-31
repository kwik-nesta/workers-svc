using API.Common.Response.Model.Extensions;
using API.Common.Response.Model.Responses;
using CSharpTypes.Extensions.Enumeration;
using CSharpTypes.Extensions.List;
using DiagnosKit.Core.Logging.Contracts;
using EFCore.CrudKit.Library.Data.Interfaces;
using KwikNesta.Contracts.Enums;
using KwikNesta.Contracts.Models;
using KwikNesta.Contracts.Models.Location;
using KwikNesta.Workers.Svc.Application.Extensions;
using KwikNesta.Workers.Svc.Application.Interfaces;
using KwikNesta.Workers.Svc.Core.Handlers.Interfaces;

namespace KwikNesta.Workers.Svc.Core.Handlers
{
    public class MessageHandler : IMessageHandler
    {
        private readonly ILoggerManager _logger;
        private readonly IServiceManager _service;
        private readonly IEFCoreMongoCrudKit _mongoCrudKit;

        public MessageHandler(ILoggerManager logger,
                              IServiceManager service,
                              IEFCoreMongoCrudKit mongoCrudKit)
        {
            _logger = logger;
            _service = service;
            _mongoCrudKit = mongoCrudKit;
        }

        public async Task HandleAsync(NotificationMessage message)
        {
            if (message != null)
            {
                switch (message.Type)
                {
                    case EmailType.AccountActivation:
                        await _service.Notification.SendAccountActivationEmail(message);
                        break;
                    case EmailType.PasswordReset:
                        await _service.Notification.SendPasswordResetEmail(message);
                        break;
                    case EmailType.PasswordResetNotification:
                        await _service.Notification.SendPasswordResetNotificationEmail(message);
                        break;
                    case EmailType.AccountDeactivation:
                        await _service.Notification.SendAccountDeactivationEmail(message);
                        break;
                    case EmailType.AccountReactivation:
                        await _service.Notification.SendAccountReactivationEmail(message);
                        break;
                    case EmailType.AccountSuspension:
                        await _service.Notification.SendAccountSuspensionEmail(message);
                        break;
                    case EmailType.AccountReactivationNotification:
                        await _service.Notification.SendAccountReactivationNotificationEmail(message);
                        break;
                    case EmailType.AdminAccountReactivation:
                        await _service.Notification.SendAdminRectivationNotificationEmail(message);
                        break;
                }
            }
            else
            {
                _logger.LogWarn($"Message content came null");
            }
        }

        public async Task HandleAsync(AuditLog message)
        {
            try
            {
                if (message != null)
                {
                    var isValid = message.ValidatePayload();
                    if (!isValid)
                    {
                        _logger.LogWarn("Invalid audit payload");
                        return;
                    }

                    await _mongoCrudKit.InsertAsync(message);
                    _logger.LogInfo("Audit trail successfully added. Action Performed: {Action}", message.Action.GetDescription());
                    return;
                }
                else
                {
                    _logger.LogWarn($"Message content came null");
                    return;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred: {Message}", ex.Message);
            }
        }

        public async Task HandleAsync(DataLoadRequest message)
        {
            if(message != null)
            {
                switch (message.Type) 
                {
                    case DataLoadType.Location:
                        await RunLocationDataLoad();
                        break;
                    default:
                        _logger.LogWarn($"Data load request type: {message.Type.GetDescription()}, not implemented.");
                        break;
                }
            }
            else
            {
                _logger.LogWarn($"The {nameof(DataLoadRequest)} message came in null");
            }
        }

        #region Private methods
        private async Task RunLocationDataLoad()
        {
            try
            {
                var countriesResponseData = await  _service.Location.GetCountriesV1Async();
                if (!TryStatusCheck<KwikNestaCountry>(countriesResponseData, out var countries))
                {
                    return;
                }

                foreach (var country in countries)
                {
                    _logger.LogInfo("Running data-load for country {Name}.", country.Name);

                    var countryExists = await _mongoCrudKit.ExistsAsync<KwikNestaCountry>(c => c.Id == country.Id);
                    if (!countryExists)
                    {
                        // Get the states for the country
                        var statesResponseData = await _service.Location.GetStatesForCountryV1Async(country.Id);
                        if(!TryStatusCheck<KwikNestaState>(statesResponseData, out var states))
                        {
                            continue;
                        }

                        // Itereate over each state
                        var citiesToInsert = new List<KwikNestaCity>();
                        var statesToInsert = new List<KwikNestaState>();
                        foreach (var state in states)
                        {
                            _logger.LogInfo("Running data-load for state {Name}.", state.Name);

                            var stateExists = await _mongoCrudKit.ExistsAsync<KwikNestaState>(c => c.Id == country.Id);
                            if (!stateExists)
                            {
                                // Get the cities for the state
                                var citiesResponseDate = await _service.Location.GetCitiesForStateV1Async(country.Id, state.Id);
                                if (!TryStatusCheck<KwikNestaCity>(citiesResponseDate, out var cities))
                                {
                                    continue;
                                }

                                statesToInsert.Add(state);
                                citiesToInsert.AddRange(cities);
                            }
                            else
                            {
                                _logger.LogInfo("State: {Name} in {CountryName} already exists in the database", state.Name, country.Name);
                            }
                        }

                        await _mongoCrudKit.InsertAsync(country);
                        _logger.LogInfo("Data-load for country {Name} is now done successfully.", country.Name);
                        if (statesToInsert.IsNotNullOrEmpty())
                        {
                            await _mongoCrudKit.InsertRangeAsync(statesToInsert);
                        }
                        if (citiesToInsert.IsNotNullOrEmpty())
                        {
                            await _mongoCrudKit.InsertRangeAsync(citiesToInsert);
                        }

                        _logger.LogInfo("Data-load for country {0} is now done successfully.\nNo. of states {1}\nNo. of cities: {2}", country.Name, statesToInsert.Count, citiesToInsert.Count);
                    }
                    else
                    {
                        _logger.LogInfo("Country: {Name} already stateExists in the database", country.Name);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }

        private bool TryStatusCheck<T>(ApiBaseResponse response, out List<T> values) where T : class
        {
            values = new List<T>();
            if (!response.Success)
            {
                _logger.LogError($"Request to get countries failed. Reason: {response.Message}");
                return false;
            }

            values = response.GetResult<List<T>>();
            if (!values.IsNotNullOrEmpty())
            {
                _logger.LogInfo($"No {nameof(T)} records returned");
                return false;
            }

            return true;
        }
        #endregion
    }
}
