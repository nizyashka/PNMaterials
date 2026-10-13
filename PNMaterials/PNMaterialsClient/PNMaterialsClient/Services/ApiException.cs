using System.Net;

namespace PNMaterialsClient.Services;

public class ApiException : Exception
{
    public ApiErrorKind Kind { get; }
    public HttpStatusCode? StatusCode { get; }

    public ApiException(
        ApiErrorKind kind,
        string message,
        HttpStatusCode? statusCode = null,
        Exception? innerException = null)
        : base(message, innerException)
    {
        Kind = kind;
        StatusCode = statusCode;
    }
}
