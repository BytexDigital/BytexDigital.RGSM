using System;
using System.Linq;

using BytexDigital.RGSM.Shared.Interfaces;

namespace BytexDigital.RGSM.Shared.Extensions
{
    public static class IErrorCollectionExtensions
    {
        public static IErrorCollection ForField(this IErrorCollection errorCollection, string field, Action<IError> action)
        {
            foreach (var error in errorCollection.Errors)
            {
                if (error.Field.ToLowerInvariant() == field.ToLowerInvariant())
                {
                    action.Invoke(error);
                }
            }

            return errorCollection;
        }

        public static bool FieldHasErrors(this IErrorCollection errorCollection, string field)
        {
            return errorCollection.Errors.Any(x => x.Field.ToLowerInvariant() == field.ToLowerInvariant());
        }

        public static bool FieldHasIdentifier(this IErrorCollection errorCollection, string field, string identifier)
        {
            return errorCollection.Errors.Any(x => x.Field.ToLowerInvariant() == field.ToLowerInvariant() && x.Code?.ToLowerInvariant() == identifier.ToLowerInvariant());
        }
    }
}
