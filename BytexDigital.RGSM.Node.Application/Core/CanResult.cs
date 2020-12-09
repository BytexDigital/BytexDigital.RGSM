namespace BytexDigital.RGSM.Node.Application.Core
{
    public class CanResult
    {
        public bool IsPossible { get; set; }
        public string FailureReason { get; set; }

        public static CanResult CannotBecause(string reason)
        {
            return new CanResult
            {
                IsPossible = false,
                FailureReason = reason
            };
        }

        public static CanResult Can()
        {
            return new CanResult { IsPossible = true };
        }

        public static implicit operator bool(CanResult canResult) => canResult.IsPossible;

        public override string ToString()
        {
            return $"{(IsPossible ? "Success" : $"(Failure) {FailureReason}")}";
        }
    }
}
