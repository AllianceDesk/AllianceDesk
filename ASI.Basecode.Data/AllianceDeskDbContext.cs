using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using ASI.Basecode.Data.Models;

namespace ASI.Basecode.Data
{
    public partial class AllianceDeskDbContext : DbContext
    {
        public AllianceDeskDbContext()
        {
        }

        public AllianceDeskDbContext(DbContextOptions<AllianceDeskDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Article> Articles { get; set; }
        public virtual DbSet<Attachment> Attachments { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Favorite> Favorites { get; set; }
        public virtual DbSet<Feedback> Feedbacks { get; set; }
        public virtual DbSet<Team> Teams { get; set; }
        public virtual DbSet<Ticket> Tickets { get; set; }
        public virtual DbSet<TicketActivity> TicketActivities { get; set; }
        public virtual DbSet<TicketActivityOperation> TicketActivityOperations { get; set; }
        public virtual DbSet<TicketMessage> TicketMessages { get; set; }
        public virtual DbSet<TicketPriority> TicketPriorities { get; set; }
        public virtual DbSet<TicketStatus> TicketStatuses { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserPreference> UserPreferences { get; set; }
        public virtual DbSet<UserRole> UserRoles { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Addr=localhost;database=AllianceDeskDb;Integrated Security=False;Trusted_Connection=True");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Article>(entity =>
            {
                entity.Property(e => e.ArticleId)
                    .ValueGeneratedNever()
                    .HasColumnName("article_id");

                entity.Property(e => e.Body).HasColumnName("body");

                entity.Property(e => e.CategoryId).HasColumnName("category_id");

                entity.Property(e => e.CreatedBy).HasColumnName("created_by");

                entity.Property(e => e.DateCreated)
                    .HasColumnType("datetime")
                    .HasColumnName("date_created");

                entity.Property(e => e.DateUpdated)
                    .HasColumnType("datetime")
                    .HasColumnName("date_updated");

                entity.Property(e => e.Title).HasColumnName("title");

                entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");

                entity.HasOne(d => d.CategoryNavigation)
                    .WithMany(p => p.Articles)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("FK_Article_Category");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.ArticleCreatedByNavigations)
                    .HasForeignKey(d => d.CreatedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Article_Creator");

                entity.HasOne(d => d.UpdatedByNavigation)
                    .WithMany(p => p.ArticleUpdatedByNavigations)
                    .HasForeignKey(d => d.UpdatedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Article_Updater");
            });

            modelBuilder.Entity<Attachment>(entity =>
            {
                entity.Property(e => e.AttachmentId)
                    .ValueGeneratedNever()
                    .HasColumnName("attachment_id");

                entity.Property(e => e.FilePath)
                    .IsRequired()
                    .HasColumnName("file_path");

                entity.Property(e => e.TicketId).HasColumnName("ticket_id");

                entity.Property(e => e.UploadedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("uploaded_at");

                entity.Property(e => e.UploadedBy).HasColumnName("uploaded_by");

                entity.HasOne(d => d.Ticket)
                    .WithMany(p => p.Attachments)
                    .HasForeignKey(d => d.TicketId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Attachment_Ticket");

                entity.HasOne(d => d.UploadedByNavigation)
                    .WithMany(p => p.Attachments)
                    .HasForeignKey(d => d.UploadedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Attachment_User");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.Property(e => e.CategoryId).HasColumnName("category_id");

                entity.Property(e => e.CategoryName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("category_name");
            });

            modelBuilder.Entity<Favorite>(entity =>
            {
                entity.HasIndex(e => new { e.UserId, e.ArticleId }, "UQ__Favorite__A57D5868F1EF9D25")
                    .IsUnique();

                entity.Property(e => e.FavoriteId)
                    .ValueGeneratedNever()
                    .HasColumnName("favorite_id");

                entity.Property(e => e.ArticleId).HasColumnName("article_id");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.Article)
                    .WithMany(p => p.Favorites)
                    .HasForeignKey(d => d.ArticleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Favorite_Article");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Favorites)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Favorite_User");
            });

            modelBuilder.Entity<Feedback>(entity =>
            {
                entity.Property(e => e.FeedbackId)
                    .ValueGeneratedNever()
                    .HasColumnName("feedback_id");

                entity.Property(e => e.Comments).HasColumnName("comments");

                entity.Property(e => e.DateCreated)
                    .HasColumnType("datetime")
                    .HasColumnName("date_created");

                entity.Property(e => e.Rating).HasColumnName("rating");

                entity.Property(e => e.TicketId).HasColumnName("ticket_id");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.Ticket)
                    .WithMany(p => p.Feedbacks)
                    .HasForeignKey(d => d.TicketId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Feedback_Ticket");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Feedbacks)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Feedback_User");
            });

            modelBuilder.Entity<Team>(entity =>
            {
                entity.Property(e => e.TeamId)
                    .ValueGeneratedNever()
                    .HasColumnName("team_id");

                entity.Property(e => e.TeamName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("team_name");
            });

            modelBuilder.Entity<Ticket>(entity =>
            {
                entity.Property(e => e.TicketId)
                    .ValueGeneratedNever()
                    .HasColumnName("ticket_id");

                entity.Property(e => e.AssignedAgent).HasColumnName("assigned_agent");

                entity.Property(e => e.AssignedTeam).HasColumnName("assigned_team");

                entity.Property(e => e.CategoryId).HasColumnName("category_id");

                entity.Property(e => e.CreatedBy).HasColumnName("created_by");

                entity.Property(e => e.DateClosed)
                    .HasColumnType("datetime")
                    .HasColumnName("date_closed");

                entity.Property(e => e.DateCreated)
                    .HasColumnType("datetime")
                    .HasColumnName("date_created");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnName("description");

                entity.Property(e => e.PriorityId).HasColumnName("priority_id");

                entity.Property(e => e.StatusId).HasColumnName("status_id");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("title");

                entity.HasOne(d => d.AssignedAgentNavigation)
                    .WithMany(p => p.TicketAssignedAgentNavigations)
                    .HasForeignKey(d => d.AssignedAgent)
                    .HasConstraintName("FK_Ticket_AgentAssigned");

                entity.HasOne(d => d.AssignedTeamNavigation)
                    .WithMany(p => p.Tickets)
                    .HasForeignKey(d => d.AssignedTeam)
                    .HasConstraintName("FK_Ticket_TeamAssigned");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Tickets)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Ticket_Category");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.TicketCreatedByNavigations)
                    .HasForeignKey(d => d.CreatedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Ticket_Creator");

                entity.HasOne(d => d.Priority)
                    .WithMany(p => p.Tickets)
                    .HasForeignKey(d => d.PriorityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Ticket_Priority");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.Tickets)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Ticket_Status");
            });

            modelBuilder.Entity<TicketActivity>(entity =>
            {
                entity.HasKey(e => e.HistoryId)
                    .HasName("PK__TicketAc__096AA2E9C72212C0");

                entity.Property(e => e.HistoryId)
                    .ValueGeneratedNever()
                    .HasColumnName("history_id");

                entity.Property(e => e.ModifiedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("modified_at");

                entity.Property(e => e.ModifiedBy).HasColumnName("modified_by");

                entity.Property(e => e.NewValue).HasColumnName("new_value");

                entity.Property(e => e.OldValue).HasColumnName("old_value");

                entity.Property(e => e.OperationId).HasColumnName("operation_id");

                entity.Property(e => e.TicketId).HasColumnName("ticket_id");

                entity.HasOne(d => d.ModifiedByNavigation)
                    .WithMany(p => p.TicketActivities)
                    .HasForeignKey(d => d.ModifiedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TicketActivity_User");

                entity.HasOne(d => d.Operation)
                    .WithMany(p => p.TicketActivities)
                    .HasForeignKey(d => d.OperationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TicketActivity_Operation");

                entity.HasOne(d => d.Ticket)
                    .WithMany(p => p.TicketActivities)
                    .HasForeignKey(d => d.TicketId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TicketActivity_Ticket");
            });

            modelBuilder.Entity<TicketActivityOperation>(entity =>
            {
                entity.HasKey(e => e.OperationId)
                    .HasName("PK__TicketAc__9DE17123B6D037DA");

                entity.Property(e => e.OperationId).HasColumnName("operation_id");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<TicketMessage>(entity =>
            {
                entity.HasKey(e => e.MessageId)
                    .HasName("PK__TicketMe__0BBF6EE6BA3A0BC2");

                entity.Property(e => e.MessageId)
                    .ValueGeneratedNever()
                    .HasColumnName("message_id");

                entity.Property(e => e.MessageBody)
                    .IsRequired()
                    .HasColumnName("message_body");

                entity.Property(e => e.PostedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("posted_at");

                entity.Property(e => e.TicketId).HasColumnName("ticket_id");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.Ticket)
                    .WithMany(p => p.TicketMessages)
                    .HasForeignKey(d => d.TicketId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Message_Ticket");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.TicketMessages)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Message_User");
            });

            modelBuilder.Entity<TicketPriority>(entity =>
            {
                entity.HasKey(e => e.PriorityId)
                    .HasName("PK__TicketPr__EE325785D7A71898");

                entity.Property(e => e.PriorityId).HasColumnName("priority_id");

                entity.Property(e => e.PriorityName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("priority_name");
            });

            modelBuilder.Entity<TicketStatus>(entity =>
            {
                entity.HasKey(e => e.StatusId)
                    .HasName("PK__TicketSt__3683B531370E8C7E");

                entity.Property(e => e.StatusId).HasColumnName("status_id");

                entity.Property(e => e.StatusName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("status_name");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.UserId)
                    .ValueGeneratedNever()
                    .HasColumnName("user_id");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("email");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("password");

                entity.Property(e => e.RoleId).HasColumnName("role_id");

                entity.Property(e => e.TeamId).HasColumnName("team_id");

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("username");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_User_Role");

                entity.HasOne(d => d.Team)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.TeamId)
                    .HasConstraintName("FK_User_Team");
            });

            modelBuilder.Entity<UserPreference>(entity =>
            {
                entity.HasKey(e => e.PreferenceId)
                    .HasName("PK__UserPref__FB41DBCFC435C54A");

                entity.Property(e => e.PreferenceId)
                    .ValueGeneratedNever()
                    .HasColumnName("preference_id");

                entity.Property(e => e.DefaultTicketView).HasColumnName("default_ticket_view");

                entity.Property(e => e.EmailNotifications).HasColumnName("email_notifications");

                entity.Property(e => e.InAppNotifications).HasColumnName("in_app_notifications");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserPreferences)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserPreferences_User");
            });

            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.HasKey(e => e.RoleId)
                    .HasName("PK__UserRole__760965CCD79E505F");

                entity.Property(e => e.RoleId).HasColumnName("role_id");

                entity.Property(e => e.RoleName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("role_name");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
