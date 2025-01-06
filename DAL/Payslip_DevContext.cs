using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace MyPaySlipLive.DAL
{
    public partial class Payslip_DevContext : DbContext
    {
        public Payslip_DevContext()
        {
        }

        public Payslip_DevContext(DbContextOptions<Payslip_DevContext> options)
            : base(options)
        {
        }

        public virtual DbSet<TblCompany> TblCompanies { get; set; } = null!;
        public virtual DbSet<TblRole> TblRoles { get; set; } = null!;
        public virtual DbSet<TblUser> TblUsers { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=198.12.253.79;Initial Catalog=Payslip_Dev;Persist Security Info=False;User ID=SA;Password=Atlantic@1ndia;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=true;Connection Timeout=30;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TblCompany>(entity =>
            {
                entity.ToTable("tbl_Company");

                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CompanyCode).HasMaxLength(100);

                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(500);
            });

            modelBuilder.Entity<TblRole>(entity =>
            {
                entity.ToTable("tbl_Role");

                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.Property(e => e.Role).HasMaxLength(500);
            });

            modelBuilder.Entity<TblUser>(entity =>
            {
                entity.ToTable("tbl_User");

                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CompanyCode).HasMaxLength(100);

                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.Property(e => e.Email).HasMaxLength(1000);

                entity.Property(e => e.Name).HasMaxLength(500);

                entity.Property(e => e.Password).HasMaxLength(500);

             
                entity.HasOne(d => d.RoleNavigation)
                    .WithMany(p => p.TblUsers)
                    .HasForeignKey(d => d.Role)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tbl_User_tbl_Role");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
