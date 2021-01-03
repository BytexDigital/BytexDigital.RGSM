namespace BytexDigital.RGSM.Node.Domain.Enumerations
{
    public enum ScheduleActionType
    {
        ExecutionDelay = 1,
        RunExecutable = 2,

        StartServer = 10,
        StopServer = 11,

        UpdateWorkshopModifications = 12,
        UpdateGameserver = 13,

        BeRcon_SendMessage = 100
    }
}
