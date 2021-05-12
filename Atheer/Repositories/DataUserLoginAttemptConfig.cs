using Atheer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atheer.Repositories
{
    public class DataUserLoginAttemptConfig : IEntityTypeConfiguration<UserLoginAttempt>
    {
        public void Configure(EntityTypeBuilder<UserLoginAttempt> builder)
        {
            builder.HasKey(x => new
            {
                x.UserId,
                x.AttemptAt
            });

            builder.HasIndex(x => x.UserId);
        }
    }
}