using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Google.Authenticator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using core8_nuxt_cassandra.Services;
using core8_nuxt_cassandra.Entities;
using core8_nuxt_cassandra.Models.dto;
using core8_nuxt_cassandra.Helpers;

namespace core8_nuxt_cassandra.Controllers.Products
{
    [ApiExplorerSettings(GroupName = "Update Product")]
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UpdateProduct : ControllerBase {
        private IProductService _productService;
        private IMapper _mapper;
        private readonly IConfiguration _configuration;  
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<UpdateProduct> _logger;

        public UpdateProduct(
            IConfiguration configuration,
            IWebHostEnvironment env,
            IProductService productService,
            IMapper mapper,
            ILogger<UpdateProduct> logger
            )
        {
            _configuration = configuration;  
            _productService = productService;
            _mapper = mapper;
            _logger = logger;
            _env = env;        
        }  

        [HttpPatch("/api/updateproduct/{id}")]
        public IActionResult UpdateProducts(Guid id, ProductModel model) {
            try {                
                DateTime now = DateTime.Now;
                var findId = _productService.GetProductById(id);
                var prods = _mapper.Map<Product>(findId);
                prods.Category = model.Category;
                prods.Descriptions = model.Descriptions;
                prods.Qty = model.Qty;
                prods.Unit = model.Unit;
                prods.CostPrice = model.CostPrice;
                prods.SellPrice = model.SellPrice;
                prods.SalePrice = model.SalePrice;
                prods.AlertStocks = model.AlertStocks;
                prods.CriticalStocks = model.CriticalStocks;
                prods.UpdatedAt = now;
                _productService.ProductUpdate(prods);
                return Ok(new {message = "Product has been updated."});
            } catch(AppException ex) {
               return BadRequest(new {message = ex.Message});
            }
        }
    }    
}