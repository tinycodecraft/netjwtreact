using System;
using GhostUI.Hubs;
using GhostUI.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using GhostUI.DB.Models;
using System.Linq;

namespace GhostUI.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AuthController : ControllerBase
    {
        private readonly IHubContext<UsersHub> _hubContext;

        private readonly ILogger _logger;
        private readonly IdenContext _context;

        public AuthController(IHubContext<UsersHub> usersHub,ILogger<AuthController> logger,IdenContext db)
        {
            _hubContext = usersHub;
            _logger = logger;
            _context = db;
        }

        [HttpPost]
        [ProducesResponseType(typeof(AuthUser), StatusCodes.Status200OK)]
        public async Task<IActionResult> Login([FromBody]Credentials request)
        {
            _logger.LogInformation("Login api is called.");
            var founduser = _context.AspNetUsers.FirstOrDefault();
            if (founduser != null)
                _logger.LogInformation($"One user found is: {founduser.UserName}");

            await _hubContext.Clients.All.SendAsync("UserLogin");

            var token = Guid.NewGuid().ToString();
            var authUser = new AuthUser("success", token, request?.UserName ?? "");

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