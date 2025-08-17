namespace core8_nuxt_cassandra.Models.dto
{
    public class PagedResponse<T>
    {
        public IEnumerable<T> Items { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
        public int TotalPages { get; set; }
        // public string NextPageToken { get; set; }
    }    

}