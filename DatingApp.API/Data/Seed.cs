using System;
using System.Collections.Generic;
using System.Linq;
using DatingApp.API.Models;
using Newtonsoft.Json;

namespace DatingApp.API.Data
{
    public class Seed
    {
        public static void SeedUsers(DataBaseContext db){

            if(!db.users.Any())
            {
                var userData = System.IO.File.ReadAllText("Data/newDataSeedFile.json");
                var users = JsonConvert.DeserializeObject<List<User>>(userData);
                foreach(var user in users)
                {
                    byte[]passwordHash,passwordSalt;
                    CreatePasswordHash("password",out passwordHash, out passwordSalt);
                    user.PasswordHash = passwordHash;
                    user.PasswordSalt = passwordSalt;
                    user.UserName = user.UserName.ToLower();
                    db.users.Add(user);
                }
                db.SaveChanges();
            }
            

        }
        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            //explicitly stating with out that out must be defined as well as declared to return some value
            
            //using keyword disposes of the variable at the end of the method
            using(var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
    }
}