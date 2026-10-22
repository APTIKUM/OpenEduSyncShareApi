using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using OpenEduSyncShare.Application.Interfaces;

namespace OpenEduSyncShare.Api.Controllers;

[ApiController]
[Route("api/answers")]
public sealed class AnswersController(IAnswersService answersService) : ControllerBase
{
    [HttpPost("query")]
    public async Task<ActionResult<Dictionary<string, JsonElement?>>> GetAnswers(
        [FromBody] IEnumerable<string> queries,
        CancellationToken cancellationToken)
    {
        var answers = await answersService.GetAnswersAsync(
            queries, cancellationToken);

        return Ok(answers);
    }

    [HttpPost("set")]
    public async Task<ActionResult> SetAnswers(
        [FromBody] Dictionary<string, JsonElement> answers,
        CancellationToken cancellationToken)
    {
        await answersService.SaveAnswersAsync(
            answers, cancellationToken);

        return Ok(answers);
    }
}