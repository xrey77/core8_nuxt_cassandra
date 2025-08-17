namespace core8_nuxt_cassandra
{    
    public class PagedResult<T>
    {
        public IEnumerable<T> Items { get; set; }
        public string NextPagingState { get; set; }
    }    
}