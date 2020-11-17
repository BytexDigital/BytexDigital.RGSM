namespace BytexDigital.RGSM.Shared.Interfaces
{
    public interface IError
    {
        public string Identifier { get; }
        public string Field { get; }
        public string Message { get; }
    }
}
