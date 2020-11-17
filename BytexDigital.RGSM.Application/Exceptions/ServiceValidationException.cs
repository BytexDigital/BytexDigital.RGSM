using System;
using System.Collections.Generic;
using System.Linq;

using BytexDigital.RGSM.Application.ErrorHandling;
using BytexDigital.RGSM.Shared.Interfaces;

namespace BytexDigital.RGSM.Application.Exceptions
{
    public class ServiceValidationException : Exception, IErrorCollection
    {
        public List<Error> Errors { get; set; }

        IReadOnlyCollection<IError> IErrorCollection.Errors => Errors.Cast<IError>().ToList();

        public ServiceValidationException(string field) : this(new List<Error> { new Error { Field = field } })
        {

        }

        public ServiceValidationException(string field, string message) : this(new List<Error> { new Error { Field = field, Message = message } })
        {

        }

        public ServiceValidationException(string field, string message, string identifier)
            : this(new List<Error> { new Error { Field = field, Message = message, Identifier = identifier } })
        {

        }

        public ServiceValidationException(List<Error> errors)
        {
            Errors = errors;
        }
    }
}
