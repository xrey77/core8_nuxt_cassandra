using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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
    public class UpdateController : ControllerBase {
        
    private IUserService _userService;

    private IMapper _mapper;
    private readonly IConfiguration _configuration;  

    private readonly IWebHostEnvironment _env;

    private readonly ILogger<UpdateController> _logger;

    public UpdateController(
        IConfiguration configuration,
        IWebHostEnvironment env,
        IUserService userService,
        IMapper mapper,
        ILogger<UpdateController> logger
        )
    {
        _configuration = configuration;  
        _userService = userService;
        _mapper = mapper;
        _logger = logger;
        _env = env;        
    }  


        [HttpPatch("/api/updateprofile/{id}")]        
        public async Task<IActionResult> updateUserprofile(Guid id, UserUpdate model) {
            try
            {
                var user = _mapper.Map<User>(model);
                user.Id = id;
                user.FirstName = model.Firstname;
                user.LastName = model.Lastname;
                user.Mobile = model.Mobile;
                var ret = await _userService.ProfileUpdate(user);
                return Ok(new {statuscode=200, message="Your profile has been updated."});
            }
            catch (AppException ex)
            {
                return BadRequest(new { statuscode = 400, message = ex.Message });
            }
        }

    }
}