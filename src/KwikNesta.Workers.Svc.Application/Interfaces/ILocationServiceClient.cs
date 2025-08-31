using API.Common.Response.Model.Responses;

namespace KwikNesta.Workers.Svc.Application.Interfaces
{
    public interface ILocationServiceClient
    {
        Task<ApiBaseResponse> GetCitiesForStateV1Async(Guid countryId, Guid stateId);
        Task<ApiBaseResponse> GetCountriesV1Async();
        Task<ApiBaseResponse> GetStatesForCountryV1Async(Guid countryId);
    }
}
