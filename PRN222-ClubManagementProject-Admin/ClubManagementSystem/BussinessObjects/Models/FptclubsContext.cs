using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace BussinessObjects.Models;

public partial class FptclubsContext : DbContext
{
    public FptclubsContext()
    {
    }

    public FptclubsContext(DbContextOptions<FptclubsContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Connection> Connections { get; set; }

    public virtual DbSet<Club> Clubs { get; set; }

    public virtual DbSet<ClubMember> ClubMembers { get; set; }

    public virtual DbSet<ClubRequest> ClubRequests { get; set; }

    public virtual DbSet<Event> Events { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Post> Posts { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<ClubTask> Tasks { get; set; }

    public virtual DbSet<TaskAssignment> TaskAssignments { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<PostReaction> Reactions { get; set; }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<JoinRequest> JoinRequests { get; set; }
    public virtual DbSet<Fee> Fees { get; set; }
    public virtual DbSet<MembershipFee> MembershipFees { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json")
            .Build();
        optionsBuilder.UseSqlServer(config.GetConnectionString("DefaultConnection"));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Club>(entity =>
        {
            entity.HasKey(e => e.ClubId).HasName("PK__Clubs__BCAD3DD9E76837C6");

            entity.Property(e => e.ClubId).HasColumnName("club_id");
            entity.Property(e => e.ClubName)
                .HasMaxLength(100)
                .IsRequired()
                .HasColumnName("club_name");
            entity.Property(e => e.Logo)
                .HasColumnType("varbinary(max)")
                .HasColumnName("logo");
            entity.Property(e => e.Cover)
                .HasColumnType("varbinary(max)")
                .HasColumnName("cover");
            entity.Property(e => e.Description)
                .HasMaxLength(500)
                .HasColumnName("description");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Status)
                .HasDefaultValue(true)
                .HasColumnType("bit")
                .HasColumnName("status");
        });

        modelBuilder.Entity<ClubMember>(entity =>
        {
            entity.HasKey(e => e.MembershipId).HasName("PK__ClubMemb__CAE49DDD6EBE5379");

            entity.Property(e => e.MembershipId).HasColumnName("membership_id");
            entity.Property(e => e.ClubId).HasColumnName("club_id");
            entity.Property(e => e.JoinedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("joined_at");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Status)
                .HasDefaultValue(true)
                .HasColumnType("bit")
                .HasColumnName("status");

            entity.HasOne(d => d.Club).WithMany(p => p.ClubMembers)
                .HasForeignKey(d => d.ClubId)
                .OnDelete(DeleteBehavior.Restrict) // Tắt cascade để tránh cycles
                .HasConstraintName("FK__ClubMembe__club___4316F928");

            entity.HasOne(d => d.Role).WithMany(p => p.ClubMembers)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.Restrict) // Tắt cascade
                .HasConstraintName("FK__ClubMembe__role___44FF419A");

            entity.HasOne(d => d.User).WithMany(p => p.ClubMembers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Restrict) // Tắt cascade
                .HasConstraintName("FK__ClubMembe__user___440B1D61");

            entity.HasMany(cm => cm.MembershipFees)
                .WithOne(mf => mf.ClubMember)
                .HasForeignKey(mf => mf.MemberId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ClubRequest>(entity =>
        {
            entity.HasKey(e => e.RequestId).HasName("PK__ClubRequ__18D3B90F642177DA");

            entity.Property(e => e.RequestId).HasColumnName("request_id");
            entity.Property(e => e.ClubName)
                .HasMaxLength(100)
                .HasColumnName("club_name");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Description)
                .HasColumnType("nvarchar(MAX)")
                .HasColumnName("description");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Pending")
                .HasColumnName("status");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Logo)
                .HasColumnType("varbinary(max)")
                .HasColumnName("logo");
            entity.Property(e => e.Cover)
                .HasColumnType("varbinary(max)")
                .HasColumnName("cover");

            entity.HasOne(d => d.User).WithMany(p => p.ClubRequests)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Restrict) // Tắt cascade
                .HasConstraintName("FK__ClubReque__user___656C112C");
        });

        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasKey(e => e.EventId).HasName("PK__Events__2370F727D6C1A214");

            entity.Property(e => e.EventId).HasColumnName("event_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.EventDate)
                .HasColumnType("datetime")
                .HasColumnName("event_date");
            entity.Property(e => e.EventDescription)
                .HasColumnType("text")
                .HasColumnName("event_description");
            entity.Property(e => e.EventTitle)
                .HasMaxLength(255)
                .HasColumnName("event_title");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Events)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict) // Tắt cascade
                .HasConstraintName("FK__Events__created___52593CB8");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__Notifica__E059842FE2370968");

            entity.Property(e => e.NotificationId).HasColumnName("notification_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.IsRead)
                .HasDefaultValue(false)
                .HasColumnName("is_read");
            entity.Property(e => e.Message)
                .HasColumnType("text")
                .HasColumnName("message");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Restrict) // Tắt cascade
                .HasConstraintName("FK__Notificat__user___60A75C0F");
        });

        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasKey(e => e.PostId).HasName("PK__Posts__3ED78766A395B13D");

            entity.Property(e => e.PostId).HasColumnName("post_id");
            entity.Property(e => e.CreatedBy)
                .IsRequired()
                .HasColumnName("created_by");
            entity.Property(e => e.Content)
                .IsRequired()
                .HasMaxLength(2000)
                .HasColumnName("content");
            entity.Property(e => e.Image)
                .HasColumnType("varbinary(max)")
                .HasColumnName("image");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Pending")
                .HasColumnName("status");

            entity.HasOne(d => d.ClubMember)
                .WithMany(p => p.Posts)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict) // Tắt cascade
                .HasConstraintName("FK__Posts__created_b__49C3F6B7");
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.CommentId);

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Post).WithMany(p => p.Comments)
                .HasForeignKey(d => d.PostId)
                .OnDelete(DeleteBehavior.Restrict); // Tắt cascade

            entity.HasOne(d => d.User).WithMany(p => p.Comments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Restrict); // Tắt cascade
        });

        modelBuilder.Entity<PostReaction>(entity =>
        {
            entity.HasKey(e => e.ReactionId).HasName("PK__Comments");

            entity.HasOne(d => d.Post).WithMany(p => p.Reactions)
                .HasForeignKey(d => d.PostId)
                .OnDelete(DeleteBehavior.Restrict); // Tắt cascade

            entity.HasOne(d => d.User).WithMany(p => p.Reactions)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Restrict); // Tắt cascade
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__760965CC319BD705");

            entity.HasIndex(e => e.RoleName, "UQ__Roles__783254B1D499B73A").IsUnique();

            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.RoleName)
                .HasMaxLength(50)
                .HasColumnName("role_name");
        });

        modelBuilder.Entity<ClubTask>(entity =>
        {
            entity.HasKey(e => e.TaskId).HasName("PK__Tasks__0492148DB46F1696");

            entity.Property(e => e.TaskId).HasColumnName("task_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.DueDate)
                .HasColumnType("datetime")
                .HasColumnName("due_date");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Pending")
                .HasColumnName("status");
            entity.Property(e => e.TaskDescription)
                .HasColumnType("text")
                .HasColumnName("task_description");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Tasks)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict) // Tắt cascade
                .HasConstraintName("FK__Tasks__created_b__571DF1D5");
        });

        modelBuilder.Entity<TaskAssignment>(entity =>
        {
            entity.HasKey(e => e.AssignmentId).HasName("PK__TaskAssi__DA8918149E9E4CAB");

            entity.Property(e => e.AssignmentId).HasColumnName("assignment_id");
            entity.Property(e => e.AssignedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("assigned_at");
            entity.Property(e => e.MembershipId).HasColumnName("membership_id");
            entity.Property(e => e.TaskId).HasColumnName("task_id");

            entity.HasOne(d => d.Membership).WithMany(p => p.TaskAssignments)
                .HasForeignKey(d => d.MembershipId)
                .OnDelete(DeleteBehavior.Restrict) // Tắt cascade
                .HasConstraintName("FK__TaskAssig__membe__5BE2A6F2");

            entity.HasOne(d => d.Task).WithMany(p => p.TaskAssignments)
                .HasForeignKey(d => d.TaskId)
                .OnDelete(DeleteBehavior.Restrict) // Tắt cascade
                .HasConstraintName("FK__TaskAssig__task___5AEE82B9");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__B9BE370F995A7F0D");

            entity.HasIndex(e => e.Email, "UQ__Users__AB6E6164897924D5").IsUnique();
            entity.HasIndex(e => e.Username, "UQ__Users__F3DBC57250E7F37C").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Username)
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnName("username");
            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .HasColumnName("password");
            entity.Property(e => e.ProfilePicture)
                .HasColumnType("varbinary(max)")
                .HasColumnName("profile_picture");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
        });

        modelBuilder.Entity<Fee>(entity =>
        {
            entity.HasKey(e => e.FeeId);

            entity.Property(e => e.FeeId).HasColumnName("fee_id");
            entity.Property(e => e.ClubId).HasColumnName("club_id");
            entity.Property(e => e.FeeDescription).HasColumnName("fee_description");
            entity.Property(e => e.FeeType).HasColumnName("fee_type");
            entity.Property(e => e.PaymentMethod).HasColumnName("payment_method");
            entity.Property(e => e.Amount).HasColumnType("decimal(10,2)").HasColumnName("amount");
            entity.Property(e => e.DueDate).HasColumnType("datetime").HasColumnName("due_date");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())").HasColumnType("datetime").HasColumnName("created_at");

            entity.HasOne(d => d.Club)
                .WithMany(p => p.Fees)
                .HasForeignKey(d => d.ClubId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(d => d.MembershipFees)
                .WithOne(mf => mf.Fee)
                .HasForeignKey(mf => mf.FeeId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<MembershipFee>(entity =>
        {
            entity.HasKey(e => e.MembershipFeeId);

            entity.Property(e => e.MembershipFeeId).HasColumnName("membership_fee_id");
            entity.Property(e => e.FeeId).HasColumnName("fee_id");
            entity.Property(e => e.MemberId).HasColumnName("member_id");
            entity.Property(e => e.Status).HasMaxLength(20).HasColumnName("status").HasDefaultValue("Pending");
            entity.Property(e => e.PaidAt).HasColumnType("datetime").HasColumnName("paid_at");
            entity.Property(e => e.TransactionId).HasMaxLength(100).HasColumnName("transaction_id");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())").HasColumnType("datetime").HasColumnName("created_at");
            entity.Property(e => e.NotificationId).HasColumnName("notification_id");

            entity.HasOne(d => d.Fee)
                .WithMany(p => p.MembershipFees)
                .HasForeignKey(d => d.FeeId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.ClubMember)
                .WithMany(cm => cm.MembershipFees) // Cần thêm collection MembershipFees trong ClubMember
                .HasForeignKey(d => d.MemberId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.Notification)
                .WithOne(n => n.MembershipFee)
                .HasForeignKey<MembershipFee>(d => d.NotificationId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        OnModelCreatingPartial(modelBuilder);
    }


    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
