//using System.Linq;
//using System.Net;
//using System.Threading.Tasks;

//using BytexDigital.RGSM.Application.Exceptions;
//using BytexDigital.RGSM.Shared.TransferObjects.Models.Errors;

//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Filters;

//namespace BytexDigital.RGSM.Application.ErrorHandling
//{
//    public class ServiceExceptionFilter : IAsyncExceptionFilter
//    {
//        public Task OnExceptionAsync(ExceptionContext context)
//        {
//            if (context.Exception is ServiceException serviceValidationException)
//            {
//                var errorDto = new FailureDto
//                {
//                    Title = "An exception occurred.",
//                    TraceIdentifier = context.HttpContext.TraceIdentifier,
//                    Status = (int)HttpStatusCode.Conflict,
//                    Errors = serviceValidationException.Errors.Select(x => new ErrorDto { Field = x.Field, Message = x.Message, Code = x.Code }).ToList()
//                };

//                context.Result = new ObjectResult(errorDto)
//                {
//                    StatusCode = (int)HttpStatusCode.Conflict
//                };

//                context.ExceptionHandled = true;
//            }

//            return Task.CompletedTask;
//        }
//    }
//}
