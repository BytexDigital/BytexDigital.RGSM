using System;

namespace BytexDigital.RGSM.Node.Application.Exceptions
{
    public class NoMasterConnectionException : Exception
    {
        public NoMasterConnectionException() : base("The node cannot connect to the masterserver. Please check that the given base uri and api key are valid.")
        {

        }
    }
}
