using Bot.API.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlackCountryBot.Web.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class VersionsController : ControllerBase
    {
        private readonly IAppVersionService _appVersionService;
        public VersionsController(IAppVersionService appVersionService)
        {
            _appVersionService = appVersionService;
        }

        [HttpGet(Name = nameof(GetVersion))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetVersion()
        {
            string version = _appVersionService.Version;

            return Ok(new
            {
                version
            });
        }
    }
}
