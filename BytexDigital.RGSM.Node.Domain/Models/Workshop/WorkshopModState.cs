namespace BytexDigital.RGSM.Node.Domain.Models.Workshop
{
    public class WorkshopModState
    {
        public bool IsInstalled { get; set; }
        public bool IsUpdating { get; set; }
        public bool RequiresUpdate { get; set; }
        public double UpdateProgress { get; set; }
        public string UpdateFailureReason { get; set; }

        public WorkshopMod WorkshopMod { get; set; }
    }
}
