using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using BlackCountryBot.Web.Features.Phrases;
using MediatR;
using Microsoft.AspNetCore.Mvc;


namespace BlackCountryBot.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PhrasesController : ControllerBase
    {
        private readonly IMediator _mediator;
        public PhrasesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return new JsonResult(await _mediator.Send(new GetAll.Query()));
        }

        [HttpPost]
        public async Task<IActionResult> Create([Required][FromBody]Create.Command command)
        {
            await _mediator.Send(command);
            return NoContent();
        }
    }
}