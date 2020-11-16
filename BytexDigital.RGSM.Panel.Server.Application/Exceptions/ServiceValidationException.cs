using System;
using System.Collections.Generic;

using BytexDigital.RGSM.Panel.Server.Application.ErrorHandling;

namespace BytexDigital.RGSM.Panel.Server.Application.Exceptions
{
    public class ServiceValidationException : Exception
    {
        public List<Error> Errors { get; set; }

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
