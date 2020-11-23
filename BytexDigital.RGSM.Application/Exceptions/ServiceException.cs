using System;
using System.Collections.Generic;
using System.Linq;

using BytexDigital.RGSM.Application.ErrorHandling;
using BytexDigital.RGSM.Shared.Interfaces;

namespace BytexDigital.RGSM.Application.Exceptions
{
    public class ServiceException : Exception, IErrorCollection
    {
        public List<Error> Errors { get; set; }

        IReadOnlyCollection<IError> IErrorCollection.Errors => Errors.Cast<IError>().ToList();

        public ServiceException()
        {

        }
        public ServiceException(List<Error> errors)
        {
            Errors = errors;
        }

        public ServiceException(string field, string message) : this(new List<Error> { new Error { Field = field, Message = message } })
        {

        }

        public static ErrorBuilder AddError()
        {
            return new ServiceExceptionBuilder().AddError();
        }
    }

    public class ServiceExceptionBuilder
    {
        public ServiceException ServiceException { get; }

        public ServiceExceptionBuilder()
        {
            ServiceException = new ServiceException
            {
                Errors = new List<Error>()
            };
        }

        public ErrorBuilder AddError()
        {
            return new ErrorBuilder(this);
        }
    }

    public class ErrorBuilder
    {
        public ServiceExceptionBuilder ServiceExceptionBuilder { get; }
        public Error Error { get; }

        public ErrorBuilder(ServiceExceptionBuilder serviceExceptionBuilder)
        {
            ServiceExceptionBuilder = serviceExceptionBuilder;
            Error = new Error();
        }

        public ErrorBuilder WithCode(string code)
        {
            Error.Code = code;

            return this;
        }

        public ErrorBuilder WithMessage(string message)
        {
            Error.Message = message;

            return this;
        }

        public ErrorBuilder WithField(string field)
        {
            Error.Field = field;

            return this;
        }

        public ErrorBuilder AddError()
        {
            ServiceExceptionBuilder.ServiceException.Errors.Add(Error);

            return new ErrorBuilder(ServiceExceptionBuilder);

        }

        public ServiceException Build()
        {
            ServiceExceptionBuilder.ServiceException.Errors.Add(Error);

            return ServiceExceptionBuilder.ServiceException;
        }
    }
}
