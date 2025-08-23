using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Mvc;

namespace XPE.ArquiteturaSoftware.DesafioFinal.Api.Extensions;

public static class ResultHttpExtensions
{
    public static IActionResult ToActionResult<T>(this Result<T> result, ControllerBase c)
    {
        if (result.IsSuccess) return c.Ok(result.Value);

        var message = result.Error ?? "Failure";
        if (string.Equals(message, "Not found", StringComparison.OrdinalIgnoreCase))
            return c.NotFound(new { message });

        if (message.StartsWith("Validation:", StringComparison.OrdinalIgnoreCase))
            return c.BadRequest(new { message });

        return c.StatusCode(StatusCodes.Status503ServiceUnavailable, new { message });
    }

    public static IActionResult ToActionResult(this Result result, ControllerBase c)
    {
        if (result.IsSuccess) return c.Ok();

        var message = result.Error ?? "Failure";
        if (string.Equals(message, "Not found", StringComparison.OrdinalIgnoreCase))
            return c.NotFound(new { message });

        if (message.StartsWith("Validation:", StringComparison.OrdinalIgnoreCase))
            return c.BadRequest(new { message });

        return c.StatusCode(StatusCodes.Status503ServiceUnavailable, new { message });
    }
}