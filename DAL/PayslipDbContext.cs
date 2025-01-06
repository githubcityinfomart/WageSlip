using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace MyPaySlipLive.DAL
{
    public partial class PayslipDbContext : DbContext
    {
        public PayslipDbContext()
        {
        }

        public PayslipDbContext(DbContextOptions<PayslipDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<TblBlog> TblBlogs { get; set; } = null!;
        public virtual DbSet<TblRole> TblRoles { get; set; } = null!;
        public virtual DbSet<TblUser> TblUsers { get; set; } = null!;
        public virtual DbSet<TblUserDetail> TblUserDetails { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=198.12.253.79;Initial Catalog=Mywageslip;Persist Security Info=False;User ID=SA;Password=Atlantic@1ndia;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=true;Connection Timeout=30;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TblBlog>(entity =>
            {
                entity.ToTable("tbl_Blog");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Date).HasColumnType("datetime");
            });

            modelBuilder.Entity<TblRole>(entity =>
            {
                entity.ToTable("tbl_Role");

                entity.Property(e => e.Id).ValueGeneratedNever();

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

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Name).HasMaxLength(500);

                entity.Property(e => e.Password).HasMaxLength(500);

                entity.Property(e => e.PhoneNumber).HasMaxLength(20);

                entity.Property(e => e.UserCode).HasMaxLength(100);

                entity.HasOne(d => d.RoleNavigation)
                    .WithMany(p => p.TblUsers)
                    .HasForeignKey(d => d.Role)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tbl_User_tbl_Role");
            });

            modelBuilder.Entity<TblUserDetail>(entity =>
            {
                entity.ToTable("tbl_UserDetail");

                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.Account).HasMaxLength(1000);

                entity.Property(e => e.Advance).HasMaxLength(1000);

                entity.Property(e => e.Allow).HasMaxLength(1000);

                entity.Property(e => e.Bank).HasMaxLength(1000);

                entity.Property(e => e.Basic).HasMaxLength(1000);

                entity.Property(e => e.Ca)
                    .HasMaxLength(1000)
                    .HasColumnName("CA");

                entity.Property(e => e.Category).HasMaxLength(1000);

                entity.Property(e => e.Company).HasMaxLength(1000);

                entity.Property(e => e.Conv).HasMaxLength(1000);

                entity.Property(e => e.Eallow)
                    .HasMaxLength(1000)
                    .HasColumnName("EAllow");

                entity.Property(e => e.Ebasic)
                    .HasMaxLength(1000)
                    .HasColumnName("EBasic");

                entity.Property(e => e.Eca)
                    .HasMaxLength(1000)
                    .HasColumnName("ECA");

                entity.Property(e => e.Ecode)
                    .HasMaxLength(100)
                    .HasColumnName("ECode");

                entity.Property(e => e.Ehra)
                    .HasMaxLength(1000)
                    .HasColumnName("EHRA");

                entity.Property(e => e.Epf)
                    .HasMaxLength(1000)
                    .HasColumnName("EPF");

                entity.Property(e => e.Esi)
                    .HasMaxLength(1000)
                    .HasColumnName("ESI");

                entity.Property(e => e.EsiNumber)
                    .HasMaxLength(1000)
                    .HasColumnName("ESI_Number");

                entity.Property(e => e.Esie)
                    .HasMaxLength(1000)
                    .HasColumnName("ESIE");

                entity.Property(e => e.Etotal)
                    .HasMaxLength(1000)
                    .HasColumnName("ETotal");

                entity.Property(e => e.Fpf)
                    .HasMaxLength(1000)
                    .HasColumnName("FPF");

                entity.Property(e => e.Gross).HasMaxLength(1000);

                entity.Property(e => e.Hra)
                    .HasMaxLength(1000)
                    .HasColumnName("HRA");

                entity.Property(e => e.Leaves).HasMaxLength(1000);

                entity.Property(e => e.MobileNumber).HasMaxLength(1000);

                entity.Property(e => e.Month).HasMaxLength(1000);

                entity.Property(e => e.Name).HasMaxLength(1000);

                entity.Property(e => e.Net).HasMaxLength(1000);

                entity.Property(e => e.Nhlv)
                    .HasMaxLength(1000)
                    .HasColumnName("NHLV");

                entity.Property(e => e.Others).HasMaxLength(1000);

                entity.Property(e => e.Pf)
                    .HasMaxLength(1000)
                    .HasColumnName("PF");

                entity.Property(e => e.PfEmp)
                    .HasMaxLength(1000)
                    .HasColumnName("PF_EMP");

                entity.Property(e => e.PfNumber)
                    .HasMaxLength(1000)
                    .HasColumnName("PF_Number");

                entity.Property(e => e.ReImb).HasMaxLength(1000);

                entity.Property(e => e.SalBasis)
                    .HasMaxLength(1000)
                    .HasColumnName("Sal_Basis");

                entity.Property(e => e.SapCode).HasMaxLength(1000);

                entity.Property(e => e.Sex).HasMaxLength(50);

                entity.Property(e => e.Tax).HasMaxLength(1000);

                entity.Property(e => e.Tded)
                    .HasMaxLength(1000)
                    .HasColumnName("TDed");

                entity.Property(e => e.Tds)
                    .HasMaxLength(1000)
                    .HasColumnName("TDS");

                entity.Property(e => e.Total).HasMaxLength(1000);

                entity.Property(e => e.TotalDeduction).HasMaxLength(1000);

                entity.Property(e => e.TransferredToAc).HasMaxLength(1000);

                entity.Property(e => e.Ttl)
                    .HasMaxLength(1000)
                    .HasColumnName("TTL");

                entity.Property(e => e.Washing).HasMaxLength(1000);

                entity.Property(e => e.Wdays)
                    .HasMaxLength(1000)
                    .HasColumnName("WDays");

                entity.Property(e => e.Year).HasMaxLength(1000);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.TblUserDetails)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tbl_UserDetail_tbl_User");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
