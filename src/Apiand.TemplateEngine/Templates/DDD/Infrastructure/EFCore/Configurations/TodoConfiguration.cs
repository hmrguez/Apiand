using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XXXnameXXX.Domain.Entities.Todo;

namespace XXXnameXXX.Infrastructure.Configurations
{
    public class TodoConfiguration : IEntityTypeConfiguration<Todo>
    {
        public void Configure(EntityTypeBuilder<Todo> builder)
        {
            builder.ToTable("Todo");
            builder.HasKey(u => u.Id);
            builder.Property(u => u.Title).IsRequired().HasMaxLength(100);
        }
    }
}