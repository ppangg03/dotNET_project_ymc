using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using backend_yourmycelebrity.Models;

namespace backend_yourmycelebrity.Data;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Activity> Activities { get; set; }

    public virtual DbSet<ArtistProfile> ArtistProfiles { get; set; }

    public virtual DbSet<GroupMember> GroupMembers { get; set; }

    public virtual DbSet<IdolGroup> IdolGroups { get; set; }

    public virtual DbSet<MakeupTeam> MakeupTeams { get; set; }

    public virtual DbSet<ManagerTeam> ManagerTeams { get; set; }

    public virtual DbSet<ManagerTeamMember> ManagerTeamMembers { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Schedule> Schedules { get; set; }

    public virtual DbSet<StaffProfile> StaffProfiles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql("Name=DefaultConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Activity>(entity =>
        {
            entity.HasKey(e => e.ActivityId).HasName("activities_pkey");

            entity.Property(e => e.ActivityId).UseIdentityAlwaysColumn();
        });

        modelBuilder.Entity<ArtistProfile>(entity =>
        {
            entity.HasKey(e => e.ArtistId).HasName("artist_pkey");

            entity.Property(e => e.ArtistId).UseIdentityAlwaysColumn();

            entity.HasOne(d => d.User).WithMany(p => p.ArtistProfiles)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("artist_profile_users_fk");
        });

        modelBuilder.Entity<GroupMember>(entity =>
        {
            entity.HasKey(e => e.GroupId).HasName("group_members_pk");

            entity.Property(e => e.GroupId).UseIdentityAlwaysColumn();

            entity.HasOne(d => d.Artist).WithMany(p => p.GroupMembers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("group_members_artist_profile_fk");

            entity.HasOne(d => d.Idol).WithMany(p => p.GroupMembers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("group_members_idol_group_fk");
        });

        modelBuilder.Entity<IdolGroup>(entity =>
        {
            entity.HasKey(e => e.IdolGroupId).HasName("artist_group_pk");

            entity.Property(e => e.IdolGroupId).UseIdentityAlwaysColumn();
        });

        modelBuilder.Entity<MakeupTeam>(entity =>
        {
            entity.HasKey(e => e.MakeupteamId).HasName("makeup_team_pk");

            entity.Property(e => e.MakeupteamId).UseIdentityAlwaysColumn();

            entity.HasOne(d => d.Lead).WithMany(p => p.MakeupTeamLeads)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("makeup_team_staff_profile_fk_1");

            entity.HasOne(d => d.Staff).WithMany(p => p.MakeupTeamStaffs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("makeup_team_staff_profile_fk");
        });

        modelBuilder.Entity<ManagerTeam>(entity =>
        {
            entity.HasKey(e => e.ManagerTeamId).HasName("manager_team_pk");

            entity.Property(e => e.ManagerTeamId).UseIdentityAlwaysColumn();
        });

        modelBuilder.Entity<ManagerTeamMember>(entity =>
        {
            entity.HasOne(d => d.ManagerTeam).WithMany()
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("manager_team_members_manager_team_fk");

            entity.HasOne(d => d.Staff).WithMany()
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("manager_team_members_staff_profile_fk");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("order_pk");

            entity.Property(e => e.OrderId).UseIdentityAlwaysColumn();

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("order_users_fk");
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.HasKey(e => e.OrderdetailId).HasName("order_detail_pk");

            entity.Property(e => e.OrderdetailId).UseIdentityAlwaysColumn();
            entity.Property(e => e.ProductId)
                .ValueGeneratedOnAdd()
                .UseIdentityAlwaysColumn();

            entity.HasOne(d => d.Order).WithMany(p => p.OrderDetails).HasConstraintName("order_detail_order_fk");

            entity.HasOne(d => d.Product).WithMany(p => p.OrderDetails).HasConstraintName("order_detail_product_fk");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("product_pk");

            entity.Property(e => e.ProductId).UseIdentityAlwaysColumn();

            entity.HasOne(d => d.ProductArtistNavigation).WithMany(p => p.Products)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("product_artist_profile_fk");
        });

        modelBuilder.Entity<Schedule>(entity =>
        {
            entity.HasKey(e => e.ScheduleId).HasName("schedule_pkey");

            entity.HasOne(d => d.Activity).WithMany(p => p.Schedules)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("schedule_activities_fk");

            entity.HasOne(d => d.Artist).WithMany(p => p.Schedules)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("schedule_artist_profile_fk");

            entity.HasOne(d => d.MakeupTeam).WithMany(p => p.Schedules)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("schedule_makeup_team_fk");
        });

        modelBuilder.Entity<StaffProfile>(entity =>
        {
            entity.HasKey(e => e.StaffId).HasName("staff_pk");

            entity.Property(e => e.StaffId).UseIdentityAlwaysColumn();
            entity.Property(e => e.Responsibilities).HasComment("ช่างผม หรือช่างแต่งหน้า หรือ สไตลิส");

            entity.HasOne(d => d.User).WithMany(p => p.StaffProfiles)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("staff_profile_users_fk");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("users_pkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
