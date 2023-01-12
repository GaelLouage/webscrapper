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
        public async Task<ResultDto> DownloadImagesAsync(WebsitToScrap website)
        {
            var result = new ResultDto();
            
            website.TagName = "img";
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

        public async Task<ResultDto> GetDataByTagsAsync(WebsitToScrap website)
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

        public async Task<(ResultDto resultD, byte[] byteArray)> GetImageById(int id, WebsitToScrap website)
        {
            byte[] imageBytes = new byte[] { };
            var result = new ResultDto();
            website.TagName = "img";
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
    }
}
