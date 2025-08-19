using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace core8_nuxt_cassandra.Models
{
    public class UploadProductpicModel {
        public Guid Id { get; set; }
        public IFormFile ProductPicture { get; set; }
    }    
}