using BytexDigital.RGSM.Panel.Server.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BytexDigital.RGSM.Panel.Server.Persistence.Configurations
{
    public class GroupPermissionConfiguration : IEntityTypeConfiguration<GroupPermission>
    {
        public void Configure(EntityTypeBuilder<GroupPermission> builder)
        {
            builder
                .HasOne(x => x.Group)
                .WithMany(x => x.Permissions)
                .HasPrincipalKey(x => x.Id)
                .HasForeignKey(x => x.GroupId);

            builder
                .HasOne(x => x.Permission)
                .WithMany(x => x.Groups)
                .HasPrincipalKey(x => x.Id)
                .HasForeignKey(x => x.PermissionId);
        }
    }
}
