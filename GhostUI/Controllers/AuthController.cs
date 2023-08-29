using System;
using GhostUI.Hubs;
using GhostUI.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

using System.Linq;
using GhostUI.UserDB;
using static GhostUI.Abstraction.Interfaces;
using GhostUI.Abstraction.Tools;

namespace GhostUI.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AuthController : ControllerBase
    {
        private readonly IHubContext<UsersHub> _hubContext;

        private readonly ILogger _logger;
        private readonly JwtDB _context;
        private readonly IJwtManager mgr;

        public AuthController(IHubContext<UsersHub> usersHub,ILogger<AuthController> logger, JwtDB db,IJwtManager itmgr)
        {
            _hubContext = usersHub;
            _logger = logger;
            _context = db;
            mgr = itmgr;
        }

        [HttpPost]
        [ProducesResponseType(typeof(AuthUser), StatusCodes.Status200OK)]
        public async Task<IActionResult> Login([FromBody]Credentials request)
        {
            _logger.LogInformation("Login api is called.");
            var haserror =await mgr.Authenticate(request.UserName,request.Password);
           
            if(haserror.HasError())
            {
                return Ok(new AuthUser("fail", "", request.UserName, haserror)); 
            }


            await _hubContext.Clients.All.SendAsync("UserLogin");

            var token = Guid.NewGuid().ToString();
            var authUser = new AuthUser("success", token, request?.UserName ?? "","");

            return Ok(authUser);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Logout()
        {
            _logger.LogInformation("Logout api is called.");

            await _hubContext.Clients.All.SendAsync("UserLogout");
            return Ok();
        }
    }
}