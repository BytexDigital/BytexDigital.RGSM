using System.Collections.Generic;

namespace BytexDigital.RGSM.Node.TransferObjects.Entities
{
    public class PermissionDto : EntityDto
    {
        public string ServerId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<GroupReferenceDto> GroupReferences { get; set; }
    }
}
