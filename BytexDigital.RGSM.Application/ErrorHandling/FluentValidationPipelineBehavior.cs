//using System.Collections.Generic;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;

//using BytexDigital.RGSM.Application.Exceptions;

//using FluentValidation;

//using MediatR;

//namespace BytexDigital.RGSM.Application.ErrorHandling
//{
//    public class FluentValidationPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
//    {
//        private readonly IEnumerable<IValidator<TRequest>> _validators;

//        public FluentValidationPipelineBehavior(IEnumerable<IValidator<TRequest>> validators)
//        {
//            _validators = validators;
//        }

//        public Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
//        {
//            var validationFailures = _validators
//                .Select(validator => validator.Validate(request))
//                .SelectMany(validationResult => validationResult.Errors)
//                .Where(validationFailure => validationFailure != null)
//                .ToList();

//            if (validationFailures.Any())
//            {
//                var errors = new List<Error>();

//                foreach (var failure in validationFailures)
//                {
//                    errors.Add(new Error
//                    {
//                        Code = Error.CODE_GENERIC_VALIDATION_FAILURE,
//                        Field = failure.PropertyName,
//                        Message = failure.ErrorMessage
//                    });
//                }

//                throw new ServiceException(errors);
//            }

//            return next();
//        }
//    }
//}
