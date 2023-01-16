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

        [HttpPost("GetDataFromTagName")]
        public async Task<ActionResult> GetDataFromTagName(WebSiteToScrap website)
        {
            if (!ModelState.IsValid) return BadRequest("Invalid request.");
           
            return Ok(await _webScrapperService.GetDataByTagsAsync(website));
        }
        [HttpGet("ImageById/{id}")]
        public async Task<ActionResult> GetImageById(int id, string web)
        {
            WebSiteToScrap website = new WebSiteToScrap();
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
            var websiteToScrap = new WebSiteToScrap();
            websiteToScrap.WebsiteUrl = web;
            websiteToScrap.Tag.TagName = "img";
            var imagesData = await _webScrapperService.DownloadImagesAsync(websiteToScrap);
            if (imagesData is null) return NotFound("No images found!");
            return Ok(imagesData);
        }
        [HttpPost("PostImagesInBase64Format")]
        public async Task<ActionResult> PostImagesInBase64Format([FromBody] Website web)
        {
            var websiteToScrap = new WebSiteToScrap();
            websiteToScrap.WebsiteUrl = web.WebsiteUrl;
            websiteToScrap.Tag.TagName = "img";
            var imagesDictionary = await _webScrapperService.GetImagesInBase64FormatAsync(websiteToScrap);
            if (imagesDictionary.dataDictionary is null) return NotFound();
            return Ok(new Tuple<ResultDto,Dictionary<int,string>>(imagesDictionary.restultDto,imagesDictionary.dataDictionary));
        }

        [HttpPost("PostAllData")]
        public async Task<IActionResult> PostAllData([FromBody] Website web)
        {
            var websiteToScrap = new WebSiteToScrap();
            websiteToScrap.WebsiteUrl = web.WebsiteUrl;
            var allData = await _webScrapperService.GetDataFromUrlAsync(websiteToScrap);
            if(allData is null) return NotFound();
            return Ok(allData);
        }
        [HttpPost("postAllDataFromTagNames")]
        public async Task<IActionResult> PostAllDataFromTagNames([FromBody]WebsiteWithMultipleTags web)
        {

            var allData = await _webScrapperService.GetDataByAllTagsAsync(web);
            if (allData is null) return NotFound();
            return Ok(allData);
        }
    }
}
