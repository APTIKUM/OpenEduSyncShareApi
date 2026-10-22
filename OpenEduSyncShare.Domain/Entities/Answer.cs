using System.Text.Json;

namespace OpenEduSyncShare.Domain.Entities;

public class Answer
{
    public string QuestionId { get; set; } = string.Empty;
    public JsonElement AnswerData { get; set; }
}