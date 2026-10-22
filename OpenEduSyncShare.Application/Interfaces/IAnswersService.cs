using System.Text.Json;

namespace OpenEduSyncShare.Application.Interfaces;

public interface IAnswersService
{
    Task<Dictionary<string, JsonElement?>> GetAnswersAsync(
        IEnumerable<string> queries,
        CancellationToken cancellationToken = default);

    Task SaveAnswersAsync(
        Dictionary<string, JsonElement> answers,
        CancellationToken cancellationToken = default);
}