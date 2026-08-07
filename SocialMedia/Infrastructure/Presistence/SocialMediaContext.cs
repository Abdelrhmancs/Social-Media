using API.Domain.Entites;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Presistence
{
    public class SocialMediaContext : IdentityDbContext<User>
    {
        public SocialMediaContext(DbContextOptions<SocialMediaContext> options)
            : base(options)
        {
        }

        // Other tables in the system
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Follow> Follows { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Share> Shares { get; set; }
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<ConversationMember> ConversationMembers { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<MessageReadReceipt> MessageReadReceipts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // IMPORTANT: required so Identity builds its default schema
            base.OnModelCreating(modelBuilder);

            // ================= COMMENTS =================
            modelBuilder.Entity<Comment>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.ToTable("comments");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Text).HasColumnName("text");
                entity.Property(e => e.ParentCommentId).HasColumnName("parent_comment_id");
                entity.Property(e => e.PostId).HasColumnName("post_id");
                entity.Property(e => e.UserId).HasColumnName("user_id");
                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Post)
                    .WithMany(p => p.Comments)
                    .HasForeignKey(d => d.PostId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Comments)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.ParentComment)
                    .WithMany(p => p.InverseParentComment)
                    .HasForeignKey(d => d.ParentCommentId);
            });

            // ================= FOLLOWS =================
            modelBuilder.Entity<Follow>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.ToTable("follows");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.FollowerId).HasColumnName("follower_id");
                entity.Property(e => e.FollowedId).HasColumnName("followed_id");
                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Follower)
                    .WithMany(p => p.FollowFollowers)
                    .HasForeignKey(d => d.FollowerId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Followed)
                    .WithMany(p => p.FollowFolloweds)
                    .HasForeignKey(d => d.FollowedId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasIndex(e => new { e.FollowerId, e.FollowedId })
                    .IsUnique();
            });

            // ================= LIKES =================
            modelBuilder.Entity<Like>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.ToTable("likes");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.PostId).HasColumnName("post_id");
                entity.Property(e => e.UserId).HasColumnName("user_id");
                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Post)
                    .WithMany(p => p.Likes)
                    .HasForeignKey(d => d.PostId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Likes)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasIndex(e => new { e.UserId, e.PostId })
                    .IsUnique();
            });

            // ================= NOTIFICATIONS =================
            modelBuilder.Entity<Notification>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.ToTable("notifications");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.ReferenceId).HasColumnName("reference_id");
                entity.Property(e => e.UserId).HasColumnName("user_id");
                entity.Property(e => e.SenderId).HasColumnName("sender_id");
                entity.Property(e => e.Type)
                    .HasColumnName("type")
                    .HasMaxLength(20);
                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.NotificationUsers)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Sender)
                    .WithMany(p => p.NotificationSenders)
                    .HasForeignKey(d => d.SenderId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            // ================= POSTS =================
            modelBuilder.Entity<Post>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.ToTable("posts");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.UserId).HasColumnName("user_id");
                entity.Property(e => e.Caption).HasColumnName("caption");
                entity.Property(e => e.ImageUrl).HasColumnName("image_url").HasMaxLength(255);
                entity.Property(e => e.VideoUrl).HasColumnName("video_url").HasMaxLength(255);
                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Posts)
                    .HasForeignKey(d => d.UserId);
            });

            // ================= SHARES =================
            modelBuilder.Entity<Share>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.ToTable("shares");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.UserId).HasColumnName("user_id");
                entity.Property(e => e.PostId).HasColumnName("post_id");
                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Post)
                    .WithMany(p => p.Shares)
                    .HasForeignKey(d => d.PostId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Shares)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasIndex(e => new { e.UserId, e.PostId })
                    .IsUnique();
            });

            // ================= CONVERSATIONS =================
            modelBuilder.Entity<Conversation>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.ToTable("conversations");
                
                entity.Property(e => e.Type).HasColumnName("type");
                entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(100);
                entity.Property(e => e.GroupPicture).HasColumnName("group_picture").HasMaxLength(255);
                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");
            });

            // ================= CONVERSATION MEMBERS =================
            modelBuilder.Entity<ConversationMember>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.ToTable("conversation_members");

                entity.Property(e => e.ConversationId).HasColumnName("conversation_id");
                entity.Property(e => e.UserId).HasColumnName("user_id");
                entity.Property(e => e.Role).HasColumnName("role");
                entity.Property(e => e.JoinedAt)
                    .HasColumnName("joined_at")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Conversation)
                    .WithMany(p => p.Members)
                    .HasForeignKey(d => d.ConversationId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.ConversationMembers)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasIndex(e => new { e.ConversationId, e.UserId }).IsUnique();
            });

            // ================= MESSAGES =================
            modelBuilder.Entity<Message>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.ToTable("messages");

                entity.Property(e => e.ConversationId).HasColumnName("conversation_id");
                entity.Property(e => e.SenderId).HasColumnName("sender_id");
                entity.Property(e => e.Content).HasColumnName("content").HasMaxLength(2000);
                entity.Property(e => e.Type).HasColumnName("type");
                entity.Property(e => e.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false);
                entity.Property(e => e.SentAt)
                    .HasColumnName("sent_at")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Conversation)
                    .WithMany(p => p.Messages)
                    .HasForeignKey(d => d.ConversationId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.Sender)
                    .WithMany(p => p.SentMessages)
                    .HasForeignKey(d => d.SenderId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            // ================= MESSAGE READ RECEIPTS =================
            modelBuilder.Entity<MessageReadReceipt>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.ToTable("message_read_receipts");

                entity.Property(e => e.MessageId).HasColumnName("message_id");
                entity.Property(e => e.UserId).HasColumnName("user_id");
                entity.Property(e => e.ReadAt)
                    .HasColumnName("read_at")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Message)
                    .WithMany(p => p.ReadReceipts)
                    .HasForeignKey(d => d.MessageId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.MessageReadReceipts)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasIndex(e => new { e.MessageId, e.UserId }).IsUnique();
            });
        }
    }
}
