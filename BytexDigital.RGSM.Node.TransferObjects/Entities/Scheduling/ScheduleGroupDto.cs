using System.Collections.Generic;

namespace BytexDigital.RGSM.Node.TransferObjects.Entities.Scheduling
{
    public class ScheduleGroupDto : EntityDto
    {
        public string SchedulerId { get; set; }
        public string DisplayName { get; set; }
        public string CronExpression { get; set; }
        public int Priority { get; set; }
        public List<ScheduleActionDto> ScheduleActions { get; set; }
    }
}
