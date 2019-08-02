using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace WebSPA.Controllers
{
    [Route("api/[controller]")]
    public class ConfigurationsController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly IOptionsSnapshot<AppSettings> _settings;

        public ConfigurationsController(IWebHostEnvironment env, IOptionsSnapshot<AppSettings> settings)
        {
            _env = env;
            _settings = settings;
        }

        [HttpGet]
        public IActionResult Configuration()
        {
            return Json(_settings.Value);
        }
    }
}
