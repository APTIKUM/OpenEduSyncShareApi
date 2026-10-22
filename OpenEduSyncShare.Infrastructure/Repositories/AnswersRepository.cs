using System.Net.Sockets;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using OpenEduSyncShare.Application.Exceptions;
using OpenEduSyncShare.Application.Interfaces;
using OpenEduSyncShare.Domain.Entities;
using OpenEduSyncShare.Infrastructure.Persistence;

namespace OpenEduSyncShare.Infrastructure.Repositories;

public sealed class AnswersRepository(AppDbContext dbContext) : IAnswersRepository
{

    private static bool IsConnectionFailure(Exception exception)
    {
        return exception switch
        {
            NpgsqlException { IsTransient: true } or SocketException or TimeoutException or IOException => true,
            _ => exception.InnerException is not null && IsConnectionFailure(exception.InnerException)
        };
    }

    public async Task<Dictionary<string, JsonElement?>> GetAnswersAsync(
        IEnumerable<string> queries,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var questionIds = queries.Distinct().ToArray();

            var storedAnswers = await dbContext.Answers
                .AsNoTracking()
                .Where(answer => questionIds.Contains(answer.QuestionId))
                .ToListAsync(cancellationToken);

            var answersById = storedAnswers.ToDictionary(
                answer => answer.QuestionId,
                answer => answer.AnswerData);

            return questionIds.ToDictionary(
                questionId => questionId,
                questionId => answersById.TryGetValue(
                    questionId, out var answer)
                    ? (JsonElement?)answer
                    : null);
        }
        catch (Exception ex) when (IsConnectionFailure(ex))
        {
            throw new DatabaseUnavailableException("Database connection failure occurred.", ex);
        }
    }

    public async Task SaveAnswersAsync(
        Dictionary<string, JsonElement> answers,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var questionIds = answers.Keys.ToArray();

            var existingAnswers = await dbContext.Answers
                .Where(answer => questionIds.Contains(answer.QuestionId))
                .ToDictionaryAsync(
                    answer => answer.QuestionId,
                    cancellationToken);

            foreach (var (questionId, answerData) in answers)
            {
                if (answerData.ValueKind == JsonValueKind.Undefined)
                {
                    throw new ArgumentException(
                        $"'{answerData}' is not a valid JSON value for question ID {questionId}.");
                }

                if (existingAnswers.TryGetValue(questionId, out var existing))
                {
                    existing.AnswerData = answerData;
                }
                else
                {
                    dbContext.Answers.Add(new Answer
                    {
                        QuestionId = questionId,
                        AnswerData = answerData
                    });
                }
            }

            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex) when (IsConnectionFailure(ex))
        {
            throw new DatabaseUnavailableException("Database connection failure occurred.", ex);
        }
    }
}