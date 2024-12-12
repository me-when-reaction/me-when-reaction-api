using System.Net.Mime;
using FluentValidation;
using MeWhenAPI.Domain.DTO;
using MeWhenAPI.Domain.Exception;

namespace MeWhenAPI.Service.Pipe
{
    public class ExceptionMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception e)
            {
                context.Response.ContentType = MediaTypeNames.Application.Json;
                if (e is BadRequestException)
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    await context.Response.WriteAsJsonAsync(new BaseMessageDTO() {
                        Data = e.Message,
                        Message = "Is there something wrong with your request? Check again 🤔",
                        StatusCode = StatusCodes.Status400BadRequest
                    });
                }
                else if (e is ValidationException valException)
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    await context.Response.WriteAsJsonAsync(new BaseMessageDTO() {
                        Data = valException.Errors.Select(x => x.ErrorMessage).ToList(),
                        Message = "Is there something wrong with your request? Check again 🤔",
                        StatusCode = StatusCodes.Status400BadRequest
                    });
                }
                else
                {
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    await context.Response.WriteAsJsonAsync(new BaseMessageDTO() {
                        Data = e.Message,
                        Message = "Oops, something went wrong with the server 😔",
                        StatusCode = StatusCodes.Status500InternalServerError
                    });
                }
            }
        }
    }
}