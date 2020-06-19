using System;
using System.Text;
using System.Security.Claims;
using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.DTOs;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using DatingApp.API.IOptionsFolder;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using System.Linq;

namespace DatingApp.API.Controllers
{
    [AllowAnonymous]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    //remember to inherit from controller base, it messes up requests types and intellisense otherwise

    //first we create claims
    //then sort out the signingkey so that the server can accurately validate the user
    // could use IOptions Pattern here
    //Jwt Security Handler takes Token Descriptor as its argument
    //handler always creates the thing
    //essentially your always creating something, creating the handler then passing that thing to the 
    //thing the handler creates
    {
        private readonly IAuthRepository repo;
        private readonly IConfiguration _config;

        public IOptions<TokenConfiguration> options { get; }

        private readonly IMapper _mapper;

        public AuthController(IAuthRepository _repo, IConfiguration configuration, IOptions<TokenConfiguration> options, IMapper mapper)
        {
            this._mapper = mapper;
            repo = _repo;
            this._config = configuration;
            this.options = options;

        }
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserForLoginDTO userLoginDTO)
        {

            var userFromRepo = await repo.Login(userLoginDTO.Username.ToLower(), userLoginDTO.Password);

            if (userFromRepo == null)
            {
                return Unauthorized();
            }

            //need to bring in some claims here
            //string newKey = optionsService.Token;

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userFromRepo.Id.ToString()),
                new Claim(ClaimTypes.Name,userFromRepo.UserName)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            // in order for server to secure token it needs to be signed.
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            //this line here thats the problem
            var token = tokenHandler.CreateToken(tokenDescriptor);

            var user = _mapper.Map<UserForListDTO>(userFromRepo);

            return Ok(new
            {
                token = tokenHandler.WriteToken(token), 
                user = user
            });


        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserForRegisterDTO userRegister)
        //take into consideration from body should be used
        //when making api call, change it to json open it up so that the parameters of the dto match exactly
        //so {
        //"Username:"Jacob",
        //"Password":"Walker"
        //remember that it should match whether it is post or get
        //}
        {
            if (!ModelState.IsValid)
            {
            
            var errors = ModelState.Select(x => x.Value.Errors)
                           .Where(y=>y.Count>0)
                           .ToList();

                return BadRequest(ModelState);
            }

            userRegister.Username = userRegister.Username.ToLower();

            if (await repo.UserExists(userRegister.Username))
            {
                return BadRequest();
            }

            var userToCreate = _mapper.Map<User>(userRegister);

            var createdUser = await repo.Register(userToCreate, userRegister.Password);

            var userToReturn = _mapper.Map<UserForDetailedDTO>(createdUser);

            return CreatedAtRoute("GetUser", new {Controller= "User", id= createdUser.Id},userToReturn);
        }

    }
}