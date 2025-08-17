using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using core8_nuxt_cassandra.Services;
using core8_nuxt_cassandra.Entities;
using core8_nuxt_cassandra.Models.dto;
using core8_nuxt_cassandra.Helpers;

namespace core8_nuxt_cassandra.Controllers.Users {
    [ApiExplorerSettings(GroupName = "Retrieve User ID")]
    [Authorize]    
    [ApiController]
    [Route("api/[controller]/{id}")]
    public class GetbyidController : ControllerBase {
        private IUserService _userService;
        private IMapper _mapper;
        private readonly IConfiguration _configuration;  
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<GetbyidController> _logger;

        public GetbyidController(IConfiguration configuration,IWebHostEnvironment env,IUserService userService,IMapper mapper,ILogger<GetbyidController> logger)
        {
            _configuration = configuration;  
            _userService = userService;
            _mapper = mapper;
            _logger = logger;
            _env = env;        
        }  

        [HttpGet]
        public IActionResult getByuserid(Guid id) {
            try {
                Console.WriteLine("USER ID :", id);
                var user = _userService.GetById(id);
                var model = _mapper.Map<UserModel>(user);
                return Ok(new {statuscode = 200,message = "User found, please wait.",user = model});
            } catch(AppException ex) {
                return BadRequest(new {statuscode = 400, message = ex.Message});
            }
        }
    }
}