using DatingApp.API.Controllers.Models;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class DataBaseContext: DbContext
    {
         public DataBaseContext(DbContextOptions<DataBaseContext> options) : base(options)
        {
            
        }

        public DbSet<ValueEntity> values {get;set;}
        public DbSet<User> users {get;set;}
        public DbSet<Photo> photos {get;set;}
        public DbSet<Like> likes {get;set;}
        public DbSet<Message> messages {get;set;}

        protected override void OnModelCreating(ModelBuilder builder){
            
            builder.Entity<Like>().HasKey(k => new{
                k.LikerId, k.LikeeId
            });

            builder.Entity<Like>().HasOne(u => u.Likee)
            .WithMany(u => u.Likers).HasForeignKey(u => u.LikeeId).OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Like>().HasOne(u => u.Liker)
            .WithMany(u => u.Likees).HasForeignKey(u => u.LikerId).OnDelete(DeleteBehavior.Restrict);
        
            builder.Entity<Message>().HasOne(u => u.Sender)
            .WithMany(m => m.MessagesSent).HasForeignKey(u => u.SenderId).OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Message>().HasOne(u => u.Recipient)
            .WithMany(m => m.MessagesReceived).HasForeignKey(u => u.RecipientId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}