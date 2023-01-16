using Infrastructuur.Dtos;
using Infrastructuur.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WebScrapper.Services.Interfaces;
using static System.Net.Mime.MediaTypeNames;

namespace WebScrapper.Services.Classes
{
    public class WebScrapperService : IWebScrapperService
    {
        public async Task<ResultDto> DownloadImagesAsync(WebSiteToScrap website)
        {
            var result = new ResultDto();
            
            website.Tag.TagName = "img";
            var images = await GetDataByTagsAsync(website);
            int imageCounter = 1;
         
            foreach (var image in images.Data)
            {
                string imageUrl = string.Empty; ;
                if (!image.Value.Contains("https://"))
                {
                    imageUrl = "https://";
                }
                 imageUrl +=  image.Value.Replace("src://", "").Replace("src:","");
                try
                {
                    using (WebClient client = new WebClient())
                    {
                        string imageName = imageUrl.Split('/')[imageUrl.Split('/').Length - 1];
                        string destinationOfImage = Path.Combine(@""+Environment.CurrentDirectory + @"\Images\", imageName);
                        client.DownloadFileAsync(new Uri(imageUrl), destinationOfImage);
                       
                    }
                } catch { }
             
                result.Data.Add(imageCounter.ToString(),imageUrl);
                imageCounter++;
            }
            if(result.Data.Count() == 0)
            {
                result.Errors.Add("No images found.");
            }
            return result;
        }

        public async Task<ResultDto> GetDataByTagsAsync(WebSiteToScrap website)
        {
            var result = new ResultDto();
            if (string.IsNullOrEmpty(website.WebsiteUrl)) 
            {
                result.Errors.Add($"No url with name: {website.WebsiteUrl} found.");
                return result;
            }
            result.Data =  await Infrastructuur.WebScrapper.WebScrapper.GetByTagNameAsync(website);
            if(result.Data is null)
            {
                result.Errors.Add("No data found.");
                return result;
            }
            return result;
        }

        public async Task<ResultDto> GetDataFromUrlAsync(WebSiteToScrap website)
        {
            var result = new ResultDto();
            if (string.IsNullOrEmpty(website.WebsiteUrl))
            {
                result.Errors.Add($"No url with name: {website.WebsiteUrl} found.");
                return result;
            }
            var tagNames = new List<string>()
            {
                "p","div","a","img","h1","form","input","button","ul","ol","li","tr","td"
            };
            //get all the data from the url
            var dataList = new List<Dictionary<string, string>>();
            dataList = await GetAllDataFromUrl(website, result, tagNames, dataList);

            if (result.Data is null)
            {
                result.Errors.Add("No data found.");
                return result;
            }
            return result;
        }

        private static async Task<List<Dictionary<string, string>>> GetAllDataFromUrl(WebSiteToScrap website, ResultDto result, List<string> tagNames, List<Dictionary<string, string>> dataList)
        {
            // Use async/await and LINQ to make the code more efficient 
            var tasks = tagNames.Select(async tag =>
            {
                website.Tag.TagName = tag;
                return await Infrastructuur.WebScrapper.WebScrapper.GetByTagNameAsync(website);
            });
            dataList = (await Task.WhenAll(tasks)).ToList();
            // Use Linq to flatten the dataList and add it to the result.Data
            result.Data = dataList.SelectMany(d => d)
                                .GroupBy(d => d.Key)
                                .ToDictionary(g => g.Key, g => g.First().Value);
            return dataList;
        }
        public async Task<(ResultDto resultD, byte[] byteArray)> GetImageById(int id, WebSiteToScrap website)
        {
            byte[] imageBytes = new byte[] { };
            var result = new ResultDto();
            website.Tag.TagName = "img";
            website.WebsiteUrl = website.WebsiteUrl;
           
            var img = (await GetDataByTagsAsync(website)).Data.FirstOrDefault(x => x.Key == id.ToString());
            if (img.Key is null)
            {
                result.Errors.Add("No image found!");
                return (resultD: result, byteArray: imageBytes);
            }
            string imageUrl = "";
            if (!img.Value.Contains("https://"))
            {
                imageUrl = "https://";
            }
            // make a dto for this
            imageUrl += img.Value.Replace("src://", "")
                                     .Replace("src:", "");

            result.Data.Add("1",imageUrl);
            using (WebClient client = new WebClient())
            {
                string destinationOfImage = Path.Combine(@"" + Environment.CurrentDirectory + @"\Images\", imageUrl);
                client.DownloadFileAsync(new Uri(imageUrl), @"C:\Users\louagga\source\repos\WebScrapper\Images\img.jpg");
            }
            using (HttpClient client = new HttpClient())
            {

                imageBytes = await client.GetByteArrayAsync(imageUrl);
            }
            return (resultD: result, byteArray: imageBytes);
        }
        public async Task<(ResultDto restultDto, Dictionary<int, string>  dataDictionary)> GetImagesInBase64FormatAsync(WebSiteToScrap website)
        {
            var result = new ResultDto();
            var data = new Dictionary<int, string>();
            website.Tag.TagName = "img";
            var images = await GetDataByTagsAsync(website);
            int imageCounter = 1;

            foreach (var image in images.Data)
            {
                string imageUrl = string.Empty; ;
                if (!image.Value.Contains("https://"))
                {
                    imageUrl = "https://";
                }
                imageUrl += image.Value.Replace("src://", "").Replace("src:", "");
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        data.Add(imageCounter, Convert.ToBase64String(await client.GetByteArrayAsync(imageUrl)));
                    }
                }
                catch { }

                result.Data.Add(imageCounter.ToString(), imageUrl);
                imageCounter++;
            }
            if (result.Data.Count() == 0)
            {
                result.Errors.Add("No images found.");
            }
            return (restultDto: result, dataDictionary: data);
        }

        public async Task<ResultDto> GetDataByAllTagsAsync(WebsiteWithMultipleTags website)
        {
            var result = new ResultDto();
            var websiteToScrap = new WebSiteToScrap();
            websiteToScrap.WebsiteUrl = website.WebsiteUrl;
            
            if (string.IsNullOrEmpty(website.WebsiteUrl))
            {
                result.Errors.Add($"No url with name: {website.WebsiteUrl} found.");
                return result;
            }
     
            var data = new List<Dictionary<string, string>>();    
            foreach(var tag in website.Tags)
            {
                websiteToScrap.Tag.TagName = tag;
                data.Add(await Infrastructuur.WebScrapper.WebScrapper.GetByTagNameAsync(websiteToScrap));
            }
            result.Data = data.SelectMany(d => d)
                .GroupBy(x => x.Key)
                .ToDictionary(x => x.Key, x => x.First().Value);
       
            if (result.Data is null)
            {
                result.Errors.Add("No data found.");
                return result;
            }
            return result;
        }
    }
}
