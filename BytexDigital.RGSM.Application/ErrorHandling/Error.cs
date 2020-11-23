using BytexDigital.RGSM.Shared.Interfaces;

namespace BytexDigital.RGSM.Application.ErrorHandling
{
    public class Error : IError
    {
        public const string CODE_GENERIC_VALIDATION_FAILURE = "GenericValidationFailure";

        public string Message { get; set; }
        public string Code { get; set; }
        public string Field { get; set; }
    }
}
