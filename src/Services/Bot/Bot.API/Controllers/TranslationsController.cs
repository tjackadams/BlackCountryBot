using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using BlackCountryBot.BuildingBlocks.EventBus.Extensions;
using Bot.API.Application.Commands;
using Bot.API.Application.Queries;
using Bot.Domain.AggregatesModel.TranslationAggregate;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Bot.API.Controllers
{  
    [Route("api/v1/[controller]")]
    [ApiController]
    public class TranslationsController : ControllerBase
    {
        private readonly ILogger<TranslationsController> _logger;
        private readonly IMediator _mediator;
        private readonly ITranslationQueries _translationQueries;
        public TranslationsController(
            IMediator mediator,
            ITranslationQueries translationQueries,
            ILogger<TranslationsController> logger
            )
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _translationQueries = translationQueries ?? throw new ArgumentNullException(nameof(translationQueries));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [Route("delete")]
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteTranslationByIdAsync([FromBody] DeleteTranslationCommand command, [FromHeader(Name = "x-requestid")] string requestId, CancellationToken cancellationToken)
        {
            bool commandResult = false;

            if(Guid.TryParse(requestId, out Guid guid ) && guid != Guid.Empty)
            {
                var requestDeleteTranslation = new IdentifiedCommand<DeleteTranslationCommand, bool>(command, guid);

                _logger.LogInformation(
                    "----- Sending command: {CommandName} - {IdProperty}: {CommandId} ({@Command})",
                    requestDeleteTranslation.GetGenericTypeName(),
                    nameof(requestDeleteTranslation.Command.TranslationId),
                    requestDeleteTranslation.Command.TranslationId,
                    requestDeleteTranslation);

                commandResult = await _mediator.Send(command, cancellationToken);
            }

            if (!commandResult)
            {
                NoContent();
            }

            return Ok();
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TranslationSummaryViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllTranslationsAsync([FromQuery] int? top, CancellationToken cancellationToken)
        {
            List<TranslationSummaryViewModel> translations = await _translationQueries.GetAllTranslationsAsync(top, cancellationToken);
            return Ok(translations);
        }

        [Route("{translationId:int}")]
        [HttpGet]
        [ProducesResponseType(typeof(TranslationSummaryViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetTranslationByIdAsync([Required]int translationId, CancellationToken cancellationToken)
        {
            TranslationSummaryViewModel translation = await _translationQueries.GetTranslationByIdAsync(translationId, cancellationToken);
            if (translation == null)
            {
                return NotFound();
            }

            return Ok(translation);
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateTranslationAsync(
            [FromBody]SetTranslationContentCommand command,
            [FromHeader(Name = "x-requestid")] string requestId,
            CancellationToken cancellationToken)
        {
            bool commandResult = false;

            if (Guid.TryParse(requestId, out Guid guid) && guid != Guid.Empty)
            {
                var requestUpdateTranslation = new IdentifiedCommand<SetTranslationContentCommand, bool>(command, guid);

                _logger.LogInformation(
                    "----- Sending command: {CommandName} - {IdProperty}: {CommandId} ({@Command})",
                    requestUpdateTranslation.GetGenericTypeName(),
                    nameof(requestUpdateTranslation.Command.TranslationId),
                    requestUpdateTranslation.Command.TranslationId,
                    requestUpdateTranslation);

                commandResult = await _mediator.Send(requestUpdateTranslation, cancellationToken);
            }

            if (!commandResult)
            {
                return BadRequest();
            }

            return Ok();
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateTranslationAsync(
            [FromBody] CreateTranslationCommand createTranslationCommand,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "----- Sending command: {CommandName} - {IdProperty}: {CommandId} ({@Command})",
                createTranslationCommand.GetGenericTypeName(),
                nameof(createTranslationCommand.OriginalPhrase),
                createTranslationCommand.OriginalPhrase.GetHashCode(StringComparison.OrdinalIgnoreCase),
                createTranslationCommand);

            bool commandResult = await _mediator.Send(createTranslationCommand, cancellationToken);
            if (!commandResult)
            {
                return BadRequest();
            }

            TranslationSummaryViewModel translation = await _translationQueries.GetTranslationByPhraseAsync(createTranslationCommand.OriginalPhrase);
            if (translation == null)
            {
                return BadRequest();
            }

            return CreatedAtAction(nameof(GetTranslationByIdAsync), new { translationId = translation.TranslationId });
        }

        [Route("{translationId:int}/tweet")]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> TweetTranslationAsync([Required]int translationId, CancellationToken cancellationToken)
        {
            var tweetTranslationCommand = new TweetTranslationCommand(translationId);

            _logger.LogInformation(
                "----- Sending command: {CommandName} - {IdProperty}: {CommandId} ({@Command})",
                tweetTranslationCommand.GetGenericTypeName(),
                nameof(tweetTranslationCommand.TranslationId),
                tweetTranslationCommand.TranslationId,
                tweetTranslationCommand);

            bool commandResult = await _mediator.Send(tweetTranslationCommand, cancellationToken);
            if (!commandResult)
            {
                return BadRequest();
            }

            return NoContent();

        }
    }
}
