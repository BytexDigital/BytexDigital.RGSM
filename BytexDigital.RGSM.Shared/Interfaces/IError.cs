namespace BytexDigital.RGSM.Shared.Interfaces
{
    public interface IError
    {
        public string Message { get; }
        public string Code { get; }
        public string Field { get; }

    }
}
