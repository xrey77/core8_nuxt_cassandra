using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using System;
using core8_nuxt_cassandra.Services;
using core8_nuxt_cassandra.Models.dto;
using core8_nuxt_cassandra.Helpers;
using core8_nuxt_cassandra.Models;

namespace core8_nuxt_cassandra.Controllers.Products
{
    [ApiExplorerSettings(GroupName = "Search Product Description")]
    [AllowAnonymous] 
    [ApiController]
    [Route("[controller]")]
    public class SearchController : ControllerBase {

        private IProductService _productService;
        private IMapper _mapper;
        private readonly IConfiguration _configuration;  
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<SearchController> _logger;

        public SearchController(
            IConfiguration configuration,
            IWebHostEnvironment env,
            IProductService productService,
            IMapper mapper,
            ILogger<SearchController> logger
            )
        {
            _configuration = configuration;  
            _productService = productService;
            _mapper = mapper;
            _logger = logger;
            _env = env;        
        }  

        [HttpGet("/api/searchproducts/{page}/{key}")]
        public async Task<IActionResult> SearchProducts(int page, string key) {
            try {                
                var totalpage = await _productService.TotPageSearch(page, key);
                var products = await _productService.SearchAll(page, key);
                var model = _mapper.Map<IList<ProductModel>>(products);
                return Ok(new {totpage = totalpage, page = page, products=model});
            } catch(AppException ex) {
               return BadRequest(new {message = ex.Message});
            }
        }
    }    
}