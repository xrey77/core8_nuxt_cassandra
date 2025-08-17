namespace core8_nuxt_cassandra.Models
{
    public class PagedRequest
    {
        public int PageSize { get; set; } = 10; // Default page size
        public string PagingState { get; set; } // The continuation token
    }    
}