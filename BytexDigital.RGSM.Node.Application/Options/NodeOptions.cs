namespace BytexDigital.RGSM.Node.Application.Options
{
    public class NodeOptions
    {
        public string BaseUri { get; set; }

        public Master MasterOptions { get; set; }

        public class Master
        {
            public string BaseUri { get; set; }
            public string ApiKey { get; set; }
        }
    }
}
