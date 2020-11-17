using BytexDigital.RGSM.Shared.Interfaces;

namespace BytexDigital.RGSM.Application.ErrorHandling
{
    public class Error : IError
    {
        public string Identifier { get; set; }
        public string Field { get; set; }
        public string Message { get; set; }
    }
}
