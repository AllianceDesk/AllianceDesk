using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using ASI.Basecode.Data.Models;

namespace ASI.Basecode.Data
{
    public partial class AsiBasecodeDBContext : DbContext
    {
        public AsiBasecodeDBContext()
        {
        }

        public AsiBasecodeDBContext(DbContextOptions<AsiBasecodeDBContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<UserRole> Roles { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<TicketPriority> Priorities { get; set; }
        public DbSet<TicketStatus> Statuses { get; set; }
        public DbSet<TicketHistory> TicketHistories { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<UserPreference> UserPreferences { get; set; }
        public DbSet<Article> Articles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Add values to tables

            modelBuilder.Entity<UserRole>().HasData(
                new UserRole { RoleId = 1, Name = "Admin" },
                new UserRole { RoleId = 2, Name = "User" },
                new UserRole { RoleId = 3, Name = "Agent" }
            );

            modelBuilder.Entity<TicketStatus>().HasData(
                new TicketStatus { StatusId = 1, Name = "Open" },
                new TicketStatus { StatusId = 2, Name = "Assigned" },
                new TicketStatus { StatusId = 3, Name = "In Progress" },
                new TicketStatus { StatusId = 4, Name = "Resolved" },
                new TicketStatus { StatusId = 5, Name = "Closed" }
            );

            modelBuilder.Entity<TicketPriority>().HasData(
                new TicketPriority { PriorityId = 1, Name = "High" },
                new TicketPriority { PriorityId = 2, Name = "Mid" },
                new TicketPriority { PriorityId = 3, Name = "Low" }
            );

            modelBuilder.Entity<TicketOperation>().HasData(
                new TicketOperation { OperationId = 1, Name = "Created" },
                new TicketOperation { OperationId = 2, Name = "Updated" },
                new TicketOperation { OperationId = 3, Name = "Assigned" },
                new TicketOperation { OperationId = 4, Name = "Reassigned" },
                new TicketOperation { OperationId = 5, Name = "Closed"}
            );

            modelBuilder.Entity<ArticleCategory>().HasData(
                new ArticleCategory { CategoryId = 1, Name = "Technical" },
                new ArticleCategory { CategoryId = 2, Name = "Billing" },
                new ArticleCategory { CategoryId = 3, Name = "General Questions" },
                new ArticleCategory { CategoryId = 4, Name = "Account Management" },
                new ArticleCategory { CategoryId = 5, Name = "Customer Support" }
            );

            // User Relationships


            // User To Role
            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasMany(u => u.CreatedTickets)
                .WithOne(t => t.Creator)
                .HasForeignKey(t => t.CreatedBy)
                .OnDelete(DeleteBehavior.Cascade);
            
            modelBuilder.Entity<User>()
                .HasMany(u => u.AssignedTickets)
                .WithOne(t => t.Assignee)
                .HasForeignKey(t => t.AssignedTo)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(u => u.TicketHistories)
                .WithOne(th => th.User)
                .HasForeignKey(th => th.PerformedBy)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
               .HasMany(u => u.Feedbacks)
               .WithOne(f => f.User)
               .HasForeignKey(f => f.UserId)
               .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(u => u.CreatedArticles)
                .WithOne(a => a.Author)
                .HasForeignKey(a => a.AuthorId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(u => u.UserPreferences)
                .WithOne(up => up.User)
                .HasForeignKey(up => up.UserId)
                .OnDelete(DeleteBehavior.Cascade);



            // Ticket Relationships

            modelBuilder.Entity<Ticket>()
                .HasMany(t => t.TicketHistories)
                .WithOne(th => th.Ticket)
                .HasForeignKey(th => th.TicketId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Ticket>()
               .HasOne(t => t.Feedback)
               .WithOne(f => f.Ticket)
               .HasForeignKey<Feedback>(f => f.TicketId)
               .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
