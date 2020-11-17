using BytexDigital.RGSM.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BytexDigital.RGSM.Persistence.Configurations
{
    public class ApplicationUserGroupConfiguration : IEntityTypeConfiguration<ApplicationUserGroup>
    {
        public void Configure(EntityTypeBuilder<ApplicationUserGroup> builder)
        {
            builder
                .HasOne(x => x.Group)
                .WithMany(x => x.Users)
                .HasPrincipalKey(x => x.Id)
                .HasForeignKey(x => x.GroupId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasOne(x => x.ApplicationUser)
                .WithMany(x => x.Groups)
                .HasPrincipalKey(x => x.Id)
                .HasForeignKey(x => x.ApplicationUserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
