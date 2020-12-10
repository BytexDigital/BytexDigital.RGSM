using System.Collections.Generic;

namespace BytexDigital.RGSM.Node.TransferObjects.Entities.Scheduling
{
    public class SchedulerPlanDto : EntityDto
    {
        public string ServerId { get; set; }
        public bool IsEnabled { get; set; }

        public List<ScheduleGroupDto> ScheduleGroups { get; set; }
    }
}
