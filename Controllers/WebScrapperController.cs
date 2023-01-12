using Microsoft.AspNetCore.Mvc;
using HtmlAgilityPack;
using Infrastructuur.Models;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using WebScrapper.Services.Interfaces;
using System.Net;
using System;
using System.Text;
using Infrastructuur.Dtos;

namespace WebScrapper.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WebScrapperController : ControllerBase
    {
        public IWebScrapperService _webScrapperService;

        public WebScrapperController(IWebScrapperService webScrapperService)
        {
            _webScrapperService = webScrapperService;
        }

        [HttpPost("GetAnkerTags")]
        public async Task<ActionResult> GetDataFromTagName(WebsitToScrap website)
        {
            if (!ModelState.IsValid) return BadRequest("Invalid request.");
           
            return Ok(await _webScrapperService.GetDataByTagsAsync(website));
        }
        [HttpGet("ImageById/{id}")]
        public async Task<ActionResult> GetImageById(int id, string web)
        {
            WebsitToScrap website = new WebsitToScrap();
      
            website.WebsiteUrl = web;
        
            var result = (await _webScrapperService.GetImageById(id, website)).resultD;
            var imageBytes = (await _webScrapperService.GetImageById(id, website)).byteArray;

             if (result is null ||imageBytes is null) return NotFound("No image found!");
            var d = Tuple.Create(result, Convert.ToBase64String(imageBytes));
            return Ok(d);
        }
        [HttpGet("GetAllTheImages")]
        public async Task<ActionResult> GetAllTheImages(string web)
        {
            var websiteToScrap = new WebsitToScrap();
            websiteToScrap.WebsiteUrl = web;
            websiteToScrap.TagName = "img";
            var imagesData = await _webScrapperService.DownloadImagesAsync(websiteToScrap);
            if (imagesData is null) return NotFound("No images found!");
            return Ok(imagesData);
        }
    }
}
