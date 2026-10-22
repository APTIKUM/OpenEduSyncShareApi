using OpenEduSyncShare.Application.Interfaces;

namespace OpenEduSyncShare.Application.Services;

public sealed class AnswersService(
    IAnswersRepository answersRepository) : IAnswersService
{
    public Task<Dictionary<string, string?>> GetAnswersAsync(
        IEnumerable<string> queries,
        CancellationToken cancellationToken = default)
    {
        return answersRepository.GetAnswersAsync(
            queries, cancellationToken);
    }

    public Task SaveAnswersAsync(
        Dictionary<string, string> answers,
        CancellationToken cancellationToken = default)
    {
        return answersRepository.SaveAnswersAsync(
            answers, cancellationToken);
    }
}