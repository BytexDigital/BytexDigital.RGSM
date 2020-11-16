using System.Collections.Generic;

namespace BytexDigital.RGSM.Shared.TransferObjects.Models.Errors
{
    public class FailureDto
    {
        public int Status { get; set; }
        public string Title { get; set; }
        public string TraceIdentifier { get; set; }
        public List<ErrorDto> Errors { get; set; }
    }
}
