using Microsoft.AspNetCore.Identity;

namespace DatingApp.API.Models
{
    public class Role: IdentityRole<int>
    {
        public System.Collections.Generic.ICollection<UserRole> UserRoles {get;set;}
    }
}