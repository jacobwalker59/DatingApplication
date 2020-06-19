using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("[controller]")]
    public class ValuesController: ControllerBase
    {
        private readonly DataBaseContext db;

        public ValuesController(DataBaseContext db)
        {
            this.db = db;
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetValues()
        {
            var values = await db.values.ToListAsync();
            return Ok(values);
        }
        [AllowAnonymous]
        [HttpGet("{id}")]
        //value/id why is a mystery for now
        public async Task<IActionResult> GetValue(int id)
        {
            var values = await db.values.FirstOrDefaultAsync(x => x.Id == id);
            return Ok(values);
        }
    }
}