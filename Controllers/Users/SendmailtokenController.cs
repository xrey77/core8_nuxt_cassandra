using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Google.Authenticator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using core8_nuxt_cassandra.Services;

namespace core8_nuxt_cassandra.Controllers.Users
{

    [ApiExplorerSettings(GroupName = "Send Mail Token to User")]
    [AllowAnonymous]
    [ApiController]
    [Route("[controller]")]
    public class SendmailtokenController : ControllerBase {

    private IUserService _userService;
    private IEmailService _emailService;
    private IMapper _mapper;
    private readonly IConfiguration _configuration;  
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<SendmailtokenController> _logger;

    public SendmailtokenController (
        IConfiguration configuration,
        IWebHostEnvironment env,
        IUserService userService,
        IEmailService emailService,
        IMapper mapper,
        ILogger<SendmailtokenController> logger
        )
    {
        _configuration = configuration;  
        _userService = userService;
        _emailService = emailService;
        _mapper = mapper;
        _logger = logger;
        _env = env;        
    }  

     [HttpPatch("/sendmailtoken/{email}")]
    public async Task<IActionResult> sendmailToken(string email) {
        try {
            var retVal = await _userService.SendEmailToken(email);
            string subj = "Mail Token";
            // string fullname = await _userService.FullName(email);
            string msg = "Copy paste this mail token " + retVal.ToString();
            await _emailService.sendMail(email, subj, msg);
            if (retVal != 0) {
               return Ok(new {message = "Mail Token has been sent to " + email + " , please check Email Inbox."});
            } else {
                return BadRequest(new {message = "Unable to send Mail token."});
            }
        } catch(Exception ex) {
            return BadRequest(new {message = ex.Message});
        }        
    }   
    }
}