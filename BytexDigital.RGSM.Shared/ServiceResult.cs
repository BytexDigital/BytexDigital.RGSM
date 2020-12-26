using BytexDigital.ErrorHandling.Shared;

namespace BytexDigital.RGSM.Shared
{
    public class ServiceResult
    {
        public bool Succeeded { get; protected set; }
        public FailureDetails FailureDetails { get; protected set; }

        public static implicit operator ServiceResult(FailureDetails failureDetails)
            => new ServiceResult { Succeeded = false, FailureDetails = failureDetails };
    }

    public class ServiceResult<T> : ServiceResult
    {
        public T Result { get; protected set; }

        public static implicit operator ServiceResult<T>(FailureDetails failureDetails)
            => new ServiceResult<T> { Succeeded = false, FailureDetails = failureDetails };

        public static implicit operator ServiceResult<T>(T result)
            => new ServiceResult<T> { Succeeded = true, Result = result };
    }
}
