namespace PNMaterialsClient.Services;

public enum ApiErrorKind
{
    Network,
    Timeout,
    Validation,
    NotFound,
    Conflict,
    Server,
    InvalidResponse,
    Unexpected
}
