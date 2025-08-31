namespace KwikNesta.Workers.Svc.Application.Models
{
    public class PageMeta
    {
        public int Page { get; set; }
        public int Size { get; set; }
        public int Pages { get; set; }
        public bool HasNext { get; set; }
        public bool HasPrevious { get; set; }
        public int Count { get; set; }
    }
}
