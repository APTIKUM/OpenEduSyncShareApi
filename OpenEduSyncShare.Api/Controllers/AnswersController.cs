using Microsoft.AspNetCore.Mvc;
using OpenEduSyncShare.Application.Interfaces;

namespace OpenEduSyncShare.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AnswersController(IAnswersService answersService) : Controller
{
    private readonly IAnswersService _answersService = answersService;

    [HttpPost]
    public ActionResult<Dictionary<string, string?>> GetAnswers([FromBody] IEnumerable<string> queries)
    {
        var answers = _answersService.GetAnswers(queries);
        return Ok(answers);
    }
}