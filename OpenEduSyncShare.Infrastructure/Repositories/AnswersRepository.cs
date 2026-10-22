using System.Net.Sockets;
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

    public async Task<Dictionary<string, string?>> GetAnswersAsync(
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
                answer => answer.AnswerText);

            return questionIds.ToDictionary(
                questionId => questionId,
                answersById.GetValueOrDefault);
        }
        catch (Exception ex) when (IsConnectionFailure(ex))
        {
            throw new DatabaseUnavailableException("Database connection failure occurred.", ex);
        }
    }

    public async Task SaveAnswersAsync(
        Dictionary<string, string> answers,
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

            foreach (var (questionId, answerText) in answers)
            {
                if (existingAnswers.TryGetValue(questionId, out var existing))
                {
                    existing.AnswerText = answerText;
                }
                else
                {
                    dbContext.Answers.Add(new Answer
                    {
                        QuestionId = questionId,
                        AnswerText = answerText
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