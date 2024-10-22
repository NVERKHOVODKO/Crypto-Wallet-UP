namespace Up.Core.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (EntityNotFoundException ex)
        {
            await HandleExceptionAsync(httpContext,
                HttpStatusCode.NotFound,
                ex.Message);
        }
        catch (IncorrectDataException ex)
        {
            await HandleExceptionAsync(httpContext,
                HttpStatusCode.BadRequest,
                ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            await HandleExceptionAsync(httpContext,
                HttpStatusCode.NotFound,
                ex.Message);
        }
        catch (AuthenticationException ex)
        {
            await HandleExceptionAsync(httpContext,
                HttpStatusCode.NotFound,
                ex.Message);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(httpContext,
                HttpStatusCode.InternalServerError,
                "Internal server error");
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, HttpStatusCode httpStatusCode, string message)
    {
        var response = context.Response;

        response.ContentType = "application/json";
        response.StatusCode = (int)httpStatusCode;

        ErrorDto errorDto = new()
        {
            Message = message,
            StatusCode = (int)httpStatusCode
        };

        //await response.WriteAsJsonAsync(errorDto);
    }
}