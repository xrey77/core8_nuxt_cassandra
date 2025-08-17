namespace core8_nuxt_cassandra.Models.dto
{
    
    public class ActivationModel {
        public Guid id { get; set; }
        public int isActivated { get; set; }
        public int isBlocked { get; set; }
    }
}