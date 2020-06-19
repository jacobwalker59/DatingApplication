using System;
using System.Threading.Tasks;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data {
    public class AuthRepository : IAuthRepository {
        private readonly DataBaseContext db;
        public AuthRepository (DataBaseContext db) {
            
            this.db = db;
        }

        //so user registers
        //user has a pasword hash and a password salt created for them in the createpassword hash method
        //this method then returns the values of the variables using the out method


        public async Task<User> Register (User user, string password) 
        {
            byte [] passwordHash, passwordSalt;
            //out keyword creates a reference to the variable
            //take into consideration the word reference as opposed to a copy
            //the method taking these keywords in need to use the out keywords too
            // out is basically taking something which is undefinted and setting it to somevalue all the time
            //within a method
            //user is added to the db
            //then when user logs in the password hash and the input password are compared
            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            user.PasswordSalt = passwordSalt;
            user.PasswordHash = passwordHash;

            await db.users.AddAsync(user);
            await db.SaveChangesAsync();

            return user;
        }
        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            //explicitly stating with out that out must be defined as well as declared to return some value
            
            //using keyword disposes of the variable at the end of the method
            using(var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        public async Task<bool> UserExists (string userName) {
            
            if(await db.users.AnyAsync(x => x.UserName== userName))
            {
                return true;
            }
            return false;
        }

        public async Task<User> Login(string userName, string password)
        {
            var user = await db.users.Include(p=> p.Photos).FirstOrDefaultAsync(x => x.UserName == userName);

            if(user == null)
            {
                return null;
            }

            if(!VerifyPasswordHash(password,user.PasswordHash, user.PasswordSalt))
            {
                return null;
            }
            return user;

        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using(var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for(int i =0; i< computedHash.Length;i++)
                {
                    if(computedHash[i] != passwordHash[i])
                    {
                        return false;
                    }
                   
                }
                 return true;
            }
        }
    }
}