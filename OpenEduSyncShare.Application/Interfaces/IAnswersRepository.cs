namespace OpenEduSyncShare.Application.Interfaces;

public interface IAnswersRepository
{
    Task<Dictionary<string, string?>> GetAnswersAsync(
        IEnumerable<string> queries,
        CancellationToken cancellationToken = default);

    Task SaveAnswersAsync(
        Dictionary<string, string> answers,
        CancellationToken cancellationToken = default);
}