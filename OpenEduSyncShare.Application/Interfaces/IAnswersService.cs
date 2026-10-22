namespace OpenEduSyncShare.Application.Interfaces;

public interface IAnswersService
{
    Dictionary<string, string?> GetAnswers(IEnumerable<string> queries);
}