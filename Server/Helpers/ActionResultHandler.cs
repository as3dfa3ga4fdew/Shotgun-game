using Microsoft.AspNetCore.Mvc;

namespace Server.Helpers
{
    public class ActionResultHandler
    {
        public static CreatedResult Created(string location, object? value)
        {
            return new(location, value);
        }

        public static ConflictResult Conflict()
        {
            return new();
        }

        public static ConflictObjectResult Conflict(object? errorObject)
        {
            return new(errorObject);
        }

        public static BadRequestResult BadRequest()
        {
            return new();
        }

        public static BadRequestObjectResult BadRequest(object? errorObject)
        {
            return new(errorObject);
        }

        public static UnauthorizedResult Unauthorized()
        {
            return new();
        }

        public static UnauthorizedObjectResult Unauthorized(object? errorObject)
        {
            return new(errorObject);
        }

        public static OkResult Ok()
        {
            return new();
        }

        public static OkObjectResult Ok(object? value)
        {
            return new(value);
        }

        public static NotFoundResult NotFound()
        {
            return new();
        }

        public static NotFoundObjectResult NotFound(object? errorObject)
        {
            return new(errorObject);
        }

        public static NoContentResult NoContent() => new();

        public static StatusCodeResult StatusCode(int statusCode) => new(statusCode);

        public static ObjectResult StatusCode(int statusCode, object? value)
        {
            return new ObjectResult(value) { StatusCode = statusCode };
        }

        public static ForbidResult Forbid() => new();

        public static RedirectResult Redirect(string url) => new(url);
    }
}
