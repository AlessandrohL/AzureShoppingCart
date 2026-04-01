namespace AzureShoppingCart.Common;

public static class HttpStatusHelper
{
    public static string GetMessageByStatusCode(int statusCode)
    {
        return statusCode switch
        {
            StatusCodes.Status200OK => HttpMessages.Ok,
            StatusCodes.Status201Created => HttpMessages.Created,
            StatusCodes.Status204NoContent => HttpMessages.Ok,
            StatusCodes.Status400BadRequest => HttpMessages.BadRequest,
            StatusCodes.Status404NotFound => HttpMessages.NotFound,
            StatusCodes.Status403Forbidden => HttpMessages.Forbidden,
            StatusCodes.Status401Unauthorized => HttpMessages.Unauthorized,
            StatusCodes.Status422UnprocessableEntity => HttpMessages.UnprocessableEntity,
            StatusCodes.Status500InternalServerError => HttpMessages.InternalServerError,
            _ => "Unknown"
        };
    }

    public static class HttpMessages
    {
        public const string Ok = "Operation completed successfully.";
        public const string Created = "Resource created successfully.";
        public const string BadRequest = "The request is incorrect or malformed.";
        public const string NotFound = "The requested resource was not found.";
        public const string Forbidden = "You do not have permission to access the resource.";
        public const string Unauthorized = "Authentication is required to access the resource.";
        public const string InternalServerError = "An internal server error occurred.";
        public const string UnprocessableEntity = "The entity could not be processed.";
    }
}