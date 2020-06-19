using System.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DatingApp.API.Helpers;

namespace DatingApp.API.Data
{
    public class DatingRepository : IDatingRepository
    {
        private readonly DataBaseContext db;
        public DatingRepository(DataBaseContext db)
        {
            this.db = db;
        }
        public void Add<T>(T entity) where T : class
        {
           db.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            db.Remove(entity);
        }

        public async Task<Like> GetLikes(int userId, int recipientId)
        {
            return await db.likes.FirstOrDefaultAsync(u => u.LikerId == userId && u.LikeeId == recipientId);
        }

        public async Task<Photo> GetMainPhotoForUser(int userId)
        {
            return await db.photos.Where(u => u.UserId==userId).FirstOrDefaultAsync(p=> p.IsMain);
        }

        public async Task<Photo> GetPhoto(int id)
        {
            var photo = await db.photos.FirstOrDefaultAsync(p => p.Id == id);
            return photo;
        }

        public async Task<User> GetUser(int id)
        {
           var user = await db.users.Include(p => p.Photos).FirstOrDefaultAsync(u => u.Id == id);
            return user;
        }
        
        public async Task<PagedList<User>> GetUsers(UserParams userParams)
        {
            
            var users =  db.users.Include(p => p.Photos).Include(x => x.Likees).Include(x => x.Likers).OrderByDescending(u => u.LastOnline).AsQueryable();
            
            users = users.Where(u => u.Id != userParams.UserId);
            users = users.Where(u => u.Gender == userParams.Gender);
            if(userParams.Likers){
               
                var userLikers = await GetUserLikes(userParams.UserId,userParams);
                users = users.Where(u => userLikers.Contains(u.Id));

            }
            if(userParams.Likees){
                var userLikees = await GetUserLikes(userParams.UserId,userParams);
                users = users.Where(u => userLikees.Contains(u.Id));
            }
            if(userParams.MinAge !=18 || userParams.MaxAge !=99){

                var minDob = DateTime.Today.AddYears(-userParams.MaxAge -1);
                var maxDob = DateTime.Today.AddYears(-userParams.MinAge -1);

                users = users.Where(u => u.DateOfBirth >=minDob && u.DateOfBirth <= maxDob);
            }

            if(!string.IsNullOrEmpty(userParams.OrderBy)){
                
                switch(userParams.OrderBy)
                {
                    case "created": users = users.OrderByDescending(u => u.CreatedProfile);
                    break;
                    default: users = users.OrderByDescending(u => u.CreatedProfile);
                    break;
                }
            }

            return await PagedList<User>.CreateAsync(users, userParams.PageNumber, userParams.PageSize);
            
        }

        private async Task<IEnumerable<int>> GetUserLikes(int id, UserParams par){
            
            var user = await db.users.Include(x => x.Likers).Include(x => x.Likees).FirstOrDefaultAsync(u => u.Id == id);
            //problem here with the if statements, drag them out from here and see what is wrong with them tomorrow.
            // var users1 =  user.Likers.Where(u => u.LikeeId == id).Select(i => i.LikerId);
            // var users2 = user.Likees.Where(u => u.LikerId == id).Select(i => i.LikeeId);
            
            if(par.Likers){
               return user.Likers.Where(u => u.LikeeId == id).Select(i => i.LikerId);
               
            }
            else{
                return user.Likees.Where(u => u.LikerId == id).Select(i => i.LikeeId);
            }

        }

        public async Task<bool> SaveAll()
        { 
             return await db.SaveChangesAsync()>0;
        }

        public async Task<Message> GetMessage(int id)
        {
            return await db.messages.FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<PagedList<Message>> GetMessagesForUser(MessageParams mParams)
        {
            var messages = db.messages.Include(u => u.Sender).ThenInclude(p => p.Photos)
            .Include(r => r.Recipient).ThenInclude(p => p.Photos).AsQueryable();

            //when you add where messages on

            switch (mParams.MessageContainer){

                case  "Inbox":
                messages = messages.Where(u => u.RecipientId == mParams.UserId && u.RecipientDeleted == false);
                break;
                case "Outbox":
                messages = messages.Where(u => u.SenderId == mParams.UserId && u.RecipientDeleted == false);
                break;
                default: 
                messages = messages.Where(u => u.RecipientId == mParams.UserId && u.RecipientDeleted == false && u.IsRead == false);
                break;
            }

                messages = messages.OrderByDescending(d => d.MessageSent);
                return await PagedList<Message>.CreateAsync(messages,mParams.PageNumber, mParams.PageSize);



        }

        public async Task<IEnumerable<Message>> GetMessageThread(int userId, int recipientId)
        {
            var messages = await db.messages.Include(u => u.Sender).ThenInclude(p => p.Photos)
            .Include(r => r.Recipient).ThenInclude(p => p.Photos).Where(m => m.RecipientId == userId && m.RecipientDeleted == false &&
            m.SenderId == recipientId || m.RecipientId == recipientId && m.SenderId == userId && m.SenderDeleted == false).OrderByDescending(m => m.MessageSent).ToListAsync();

            return messages;
        }
    }
}