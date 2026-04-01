using Microsoft.AspNetCore.Http.HttpResults;

namespace AzureShoppingCart.Common
{
    public static class ApiResults
    {
        public static Ok<SuccessResponse> Ok() =>
            TypedResults.Ok(new SuccessResponse(StatusCodes.Status200OK));
        public static Ok<SuccessResponse<T>> Ok<T>(T value) =>
            TypedResults.Ok(new SuccessResponse<T>(value, StatusCodes.Status200OK));
        public static Created<SuccessResponse> Created(string uri) =>
            TypedResults.Created(uri,
                new SuccessResponse(StatusCodes.Status201Created));
        public static Created<SuccessResponse<T>> Created<T>(string uri, T value) =>
            TypedResults.Created(uri,
                new SuccessResponse<T>(value, StatusCodes.Status201Created));
        public static NoContent NoContent() => TypedResults.NoContent();

        public static ProblemHttpResult Problem(Error error)
        {
            return TypedResults.Problem(
                title: GetTitle(error),
                detail: GetDetail(error),
                type: GetType(error.Type),
                statusCode: GetStatusCode(error.Type));

            static string GetTitle(Error error) =>
                error.Type switch
                {
                    ErrorType.Validation => error.Code,
                    ErrorType.Problem => error.Code,
                    ErrorType.NotFound => error.Code,
                    ErrorType.Conflict => error.Code,
                    _ => "Server failure"
                };

            static string GetDetail(Error error) =>
                error.Type switch
                {
                    ErrorType.Validation => error.Description,
                    ErrorType.Problem => error.Description,
                    ErrorType.NotFound => error.Description,
                    ErrorType.Conflict => error.Description,
                    _ => "An unexpected error occurred"
                };

            static string GetType(ErrorType errorType) =>
                errorType switch
                {
                    ErrorType.Validation => "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                    ErrorType.Problem => "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                    ErrorType.NotFound => "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                    ErrorType.Conflict => "https://tools.ietf.org/html/rfc7231#section-6.5.8",
                    _ => "https://tools.ietf.org/html/rfc7231#section-6.6.1"
                };

            static int GetStatusCode(ErrorType errorType) =>
                errorType switch
                {
                    ErrorType.Validation => StatusCodes.Status400BadRequest,
                    ErrorType.Problem => StatusCodes.Status400BadRequest,
                    ErrorType.NotFound => StatusCodes.Status404NotFound,
                    ErrorType.Conflict => StatusCodes.Status409Conflict,
                    _ => StatusCodes.Status500InternalServerError
                };
        }
    }

    public class SuccessResponse : Response
    {
        public SuccessResponse(int statusCode)
        {
            Message = HttpStatusHelper.GetMessageByStatusCode(statusCode);
            Status = statusCode;
        }
    }

    public class SuccessResponse<T> : Response
    {
        public T Data { get; set; }

        public SuccessResponse(T value, int statusCode)
        {
            Message = HttpStatusHelper.GetMessageByStatusCode(statusCode);
            Status = statusCode;
            Data = value;
        }
    }

    public abstract class Response
    {
        public string Message { get; set; } = null!;
        public int Status { get; set; }
        public bool Succeeded { get; set; } = true;
    }
}
