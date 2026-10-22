namespace OpenEduSyncShare.Application.Exceptions;

public sealed class DatabaseUnavailableException(
    string message,
    Exception innerException) : Exception(message, innerException);