using API.Common.Response.Model.Responses;
using DiagnosKit.Core.Logging.Contracts;
using EFCore.CrudKit.Library.Data.Interfaces;
using KwikNesta.Contracts.Models.Location;
using KwikNesta.Workers.Svc.Application.Extensions;
using KwikNesta.Workers.Svc.Application.Interfaces;
using KwikNesta.Workers.Svc.Application.Models;

namespace KwikNesta.Workers.Svc.Application.Implementations
{
    public class LocationServiceClient : ILocationServiceClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IEFCoreMongoCrudKit _mongoCrudKit;
        private readonly ILoggerManager _logger;

        public LocationServiceClient(IHttpClientFactory httpClientFactory,
                                 IEFCoreMongoCrudKit mongoCrudKit,
                                 ILoggerManager logger)
        {
            _httpClientFactory = httpClientFactory;
            _mongoCrudKit = mongoCrudKit;
            _logger = logger;
        }

        public async Task<ApiBaseResponse> GetCountriesV1Async()
        {
            var client = _httpClientFactory.CreateClient("LocationService");
            var response = await client.GetAsync("/api/v1/location/countries");
            if(response == null || !response.IsSuccessStatusCode)
            {
                return new BadRequestResponse($"Response with status code: {response?.StatusCode ?? 0}");
            }

            var responseData = await response.ReadFromJsonAsync<LocationCountryModel>();
            if(responseData == null)
            {
                return new NotFoundResponse("No data found");
            }

            return new OkResponse<List<KwikNestaCountry>>(responseData.Data);
        }

        public async Task<ApiBaseResponse> GetStatesForCountryV1Async(Guid countryId)
        {
            var client = _httpClientFactory.CreateClient("LocationService");
            var response = await client.GetAsync($"/api/v1/location/country/{countryId}/states");
            if (response == null || !response.IsSuccessStatusCode)
            {
                return new BadRequestResponse($"Response with status code: {response?.StatusCode ?? 0}");
            }

            var states = await response.ReadFromJsonAsync<List<KwikNestaState>>();
            if (states == null)
            {
                return new NotFoundResponse("No state data found");
            }

            return new OkResponse<List<KwikNestaState>>(states);
        }

        public async Task<ApiBaseResponse> GetCitiesForStateV1Async(Guid countryId, Guid stateId)
        {
            var client = _httpClientFactory.CreateClient("LocationService");
            var response = await client.GetAsync($"/api/v1/location/country/{countryId}/state/{stateId}/cities");
            if (response == null || !response.IsSuccessStatusCode)
            {
                return new BadRequestResponse($"Response with status code: {response?.StatusCode ?? 0}");
            }

            var cities = await response.ReadFromJsonAsync<List<KwikNestaCity>>();
            if (cities == null)
            {
                return new NotFoundResponse("No data found");
            }

            return new OkResponse<List<KwikNestaCity>>(cities);
        }
    }
}