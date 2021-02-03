namespace BytexDigital.RGSM.Node.TransferObjects.Models.Workshop
{
    public class WorkshopModStateDto
    {
        public bool IsInstalled { get; set; }
        public bool IsUpdating { get; set; }
        public bool RequiresUpdate { get; set; }
        public double UpdateProgress { get; set; }
        public string UpdateFailureReason { get; set; }

        public WorkshopModDto WorkshopMod { get; set; }
    }
}
