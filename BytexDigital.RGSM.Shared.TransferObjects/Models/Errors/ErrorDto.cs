using BytexDigital.RGSM.Shared.Interfaces;

namespace BytexDigital.RGSM.Shared.TransferObjects.Models.Errors
{
    public class ErrorDto : IError
    {
        public string Identifier { get; set; }
        public string Field { get; set; }
        public string Message { get; set; }
    }
}
