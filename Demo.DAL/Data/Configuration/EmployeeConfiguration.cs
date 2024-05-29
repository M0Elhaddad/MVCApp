using Demo.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.DAL.Data.Configuration
{
    public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            builder.Property(E => E.Salary).HasColumnType("decimal(18,2");
            builder.Property(E => E.Address).HasColumnType("varchar(33)");
            builder.Property(E => E.Email).HasColumnType("varchar(50)");

            builder.HasOne(E => E.Department)
                .WithMany(D => D.Employees)
                .HasForeignKey(E => E.DeptId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Property(E => E.Name)
                .IsRequired(true)
                .HasMaxLength(50);

        }
    }
}
