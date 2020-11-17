using System.Linq;
using System.Net;
using System.Threading.Tasks;

using BytexDigital.RGSM.Application.Exceptions;
using BytexDigital.RGSM.Shared.TransferObjects.Models.Errors;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BytexDigital.RGSM.Panel.Server.Common.Filters
{
    public class ServiceExceptionFilter : IAsyncExceptionFilter
    {
        public Task OnExceptionAsync(ExceptionContext context)
        {
            if (context.Exception is ServiceValidationException serviceValidationException)
            {
                var errorDto = new FailureDto
                {
                    Errors = serviceValidationException.Errors.Select(x => new ErrorDto { Field = x.Field, Message = x.Message, Identifier = x.Identifier }).ToList()
                };

                context.Result = new ObjectResult(errorDto)
                {
                    StatusCode = (int)HttpStatusCode.Conflict
                };

                context.ExceptionHandled = true;
            }

            return Task.CompletedTask;
        }
    }
}
