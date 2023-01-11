﻿using Microsoft.AspNetCore.Mvc;

namespace WebScrapper.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WebScrapper : ControllerBase
    {
        [HttpGet]
        public IActionResult GetData()
        {

            return Ok("test");
        }
    }
}
