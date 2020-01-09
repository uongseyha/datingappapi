using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingAppAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DatingAppAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly EFContext _dbContext;
        public ProductController(EFContext eFContext)
        {
            _dbContext = eFContext;
        }
        
        [HttpGet]
        public IActionResult ProductList()
        {
            return Ok(_dbContext.Products.ToList());
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public IActionResult Product(int id)
        {
            return Ok(_dbContext.Products.FirstOrDefault(x => x.Id==id));
        }

    }
}