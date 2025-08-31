using KwikNesta.Contracts.Models.Location;

namespace KwikNesta.Workers.Svc.Application.Models
{
    public class LocationCountryModel
    {
        public PageMeta MetaData { get; set; } = new();
        public List<KwikNestaCountry> Data { get; set; } = [];
    }
}