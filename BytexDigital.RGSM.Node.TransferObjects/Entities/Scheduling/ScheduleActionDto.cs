using System.Collections.Generic;

using BytexDigital.RGSM.Node.TransferObjects.Enumerations;

namespace BytexDigital.RGSM.Node.TransferObjects.Entities.Scheduling
{
    public class ScheduleActionDto
    {
        public string ScheduleEventId { get; set; }
        public ScheduleActionTypeDto ActionType { get; set; }
        public int Order { get; set; }
        public bool ContinueOnError { get; set; }
        public List<KeyValueDto> KeyValues { get; set; }
    }
}
