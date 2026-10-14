using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace PNMaterialsAPI.ErrorHandling;

public sealed class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger,
    IProblemDetailsService problemDetails) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken ct)
    {
        var (status, detail) = Classify(exception);

        if (status >= 500)
            logger.LogError(exception, "Ошибка при выполнении {Method} {Path}", context.Request.Method, context.Request.Path);
        else
            logger.LogWarning(exception, "Конфликт данных при выполнении {Method} {Path}", context.Request.Method, context.Request.Path);

        context.Response.StatusCode = status;

        return await problemDetails.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = context,
            Exception = exception,
            ProblemDetails =
            {
                Status = status,
                Title = status >= 500 ? "Внутренняя ошибка сервера" : "Конфликт данных",
                Detail = detail
            }
        });
    }

    private static (int Status, string Detail) Classify(Exception exception) => exception switch
    {
        DbUpdateConcurrencyException =>
            (StatusCodes.Status409Conflict, "Данные были изменены другим пользователем. Обновите карточку и повторите."),
        DbUpdateException { InnerException: SqlException { Number: 547 } } =>
            (StatusCodes.Status409Conflict, "Связанная запись была изменена или удалена. Обновите данные и повторите."),
        DbUpdateException { InnerException: SqlException { Number: 2601 or 2627 } } =>
            (StatusCodes.Status409Conflict, "Запись с такими данными уже существует."),
        _ => (StatusCodes.Status500InternalServerError, "Внутренняя ошибка сервера.")
    };
}
