using System.IO;
using System;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp;
using core8_nuxt_cassandra.Models;
using core8_nuxt_cassandra.Services;

namespace core8_nuxt_cassandra.Controllers.Users
{
    [ApiExplorerSettings(GroupName = "Upload User Image")]
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    public class UploadpictureController : ControllerBase {

    private IUserService _userService;

    private IMapper _mapper;
    private readonly IConfiguration _configuration;  

    private readonly IWebHostEnvironment _env;

    private readonly ILogger<UploadpictureController> _logger;

    public UploadpictureController(
        IConfiguration configuration,
        IWebHostEnvironment env,
        IUserService userService,
        IMapper mapper,
        ILogger<UploadpictureController> logger
        )
    {
        _configuration = configuration;  
        _userService = userService;
        _mapper = mapper;
        _logger = logger;
        _env = env;        
    }  
        [HttpPost]
        public async Task<IActionResult> uploadPicture([FromForm]UploadfileModel model) {
                if (model.Profilepic.FileName != null)
                {
                    try
                    {
                        string ext= Path.GetExtension(model.Profilepic.FileName);

                        var folderName = Path.Combine("wwwroot", "users/");
                        var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

                        var newFilename =pathToSave + "00" + model.Id + ".jpg";

                        using var image = SixLabors.ImageSharp.Image.Load(model.Profilepic.OpenReadStream());
                        image.Mutate(x => x.Resize(100, 100));
                        image.Save(newFilename);
                        string file = "https://localhost:7100/users/00"+model.Id.ToString()+".jpg";
                        if (model.Profilepic != null)
                        {
                           bool retVal = await _userService.UpdatePicture(model.Id, file);                            
                        }
                        return Ok(new { 
                            statuscode = 200, 
                            message = "Profile Picture has been updated.",
                            profilepic = file});                        
                    }
                    catch (Exception ex)
                    {
                        return BadRequest(new {statuscode = 400, message =ex.Message});
                    }
                }
                return NotFound(new { statuscode = 404, message = "Profile Picture not found."});

        }
    }
    
}