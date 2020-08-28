using MediatR;
using Microsoft.AspNetCore.Mvc;
using SampleDomainService.Application.Commands;
using System.Threading;
using System.Threading.Tasks;

namespace SampleDomainService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SampleController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SampleController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> CreateSampleAsync([FromBody] CreateSampleCommand createSampleCommand, CancellationToken cancellationToken = default)
        {
            await _mediator.Publish(createSampleCommand, cancellationToken);
            return Ok();
        }
    }
}