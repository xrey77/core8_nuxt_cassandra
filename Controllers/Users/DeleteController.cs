using AutoMapper;
using Cassandra;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using core8_nuxt_cassandra.Services;
using core8_nuxt_cassandra.Helpers;

namespace core8_nuxt_cassandra.Controllers.Users
{
    [ApiExplorerSettings(GroupName = "Delete User")]
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class DeleteController : ControllerBase {

    private IUserService _userService;

    private IMapper _mapper;
    private readonly IConfiguration _configuration;  

    private readonly IWebHostEnvironment _env;

    private readonly ILogger<DeleteController> _logger;

    public DeleteController(
        IConfiguration configuration,
        IWebHostEnvironment env,
        IUserService userService,
        IMapper mapper,
        ILogger<DeleteController> logger
        )
    {
        _configuration = configuration;  
        _userService = userService;
        _mapper = mapper;
        _logger = logger;
        _env = env;        
    }  

        [HttpDelete("/api/deleteuser/{id}")]
        public async Task<IActionResult> deleteUser(Guid id) {
            try
            {
                var user = await _userService.Delete(id);
                Console.WriteLine("May Error : ", user);
                if (user != false){
                    return Ok(new { message = "Successfully Deleted."});                
                } else {
                    return NotFound(new { message = "User Id not found" });
                }
           }
            catch (Cassandra.NoHostAvailableException)
            {               
                return BadRequest(new { message = "Server down.." });
            }
        }
    }
}