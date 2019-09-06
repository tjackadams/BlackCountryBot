using System.Net;
using System.Threading.Tasks;
using BlackCountryBot.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace BlackCountryBot.Web.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class VersionsController : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(typeof(DefaultResponse<string>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetVersion()
        {
            return await Task.FromResult(Ok(new DefaultResponse<string>
            {
                Value = System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString()
            }));
        }
    }
}
