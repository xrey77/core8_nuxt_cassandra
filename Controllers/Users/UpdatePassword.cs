using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using core8_nuxt_cassandra.Entities;
using core8_nuxt_cassandra.Helpers;
using core8_nuxt_cassandra.Models.dto;
using core8_nuxt_cassandra.Services;

namespace core8_nuxt_cassandra.Controllers.Users
{
    [ApiExplorerSettings(GroupName = "Update User")]
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UpdatePasswordController : ControllerBase {
        
    private IUserService _userService;

    private IMapper _mapper;
    private readonly IConfiguration _configuration;  

    private readonly IWebHostEnvironment _env;

    private readonly ILogger<UpdatePasswordController> _logger;

    public UpdatePasswordController(
        IConfiguration configuration,
        IWebHostEnvironment env,
        IUserService userService,
        IMapper mapper,
        ILogger<UpdatePasswordController> logger
        )
    {
        _configuration = configuration;  
        _userService = userService;
        _mapper = mapper;
        _logger = logger;
        _env = env;        
    }  

        [HttpPatch("/api/updatepassword/{id}")]        
        public async Task<IActionResult> updateUserPassword(Guid id, UserPasswordUpdate model) {
            try
            {
                var user = _mapper.Map<User>(model);
                user.Id = id;
                user.Password = model.Password;
              var ret = await _userService.UpdatePassword(user);
                return Ok(new {statuscode=200, message="Your profile password has been updated."});
            }
            catch (AppException ex)
            {
                return BadRequest(new { statuscode = 404, message = ex.Message });
            }
        }


    }
}