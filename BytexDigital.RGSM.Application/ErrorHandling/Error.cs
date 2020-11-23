using BytexDigital.RGSM.Shared.Interfaces;

namespace BytexDigital.RGSM.Application.ErrorHandling
{
    public class Error : IError
    {
        public string Message { get; set; }
        public string Code { get; set; }
        public string Field { get; set; }
    }
}
