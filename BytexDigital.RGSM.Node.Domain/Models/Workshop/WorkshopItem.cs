namespace BytexDigital.RGSM.Node.Domain.Models.Workshop
{
    public class WorkshopItem
    {
        public ulong Id { get; set; }
        public bool IsInstalled { get; set; }

        public bool RequiresUpdate { get; set; }

        public bool IsUpdating { get; set; }
        public double UpdateProgress { get; set; }
        public string UpdateFailureReason { get; set; }
    }
}
