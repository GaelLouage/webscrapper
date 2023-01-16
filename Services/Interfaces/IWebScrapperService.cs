using Infrastructuur.Dtos;
using Infrastructuur.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebScrapper.Services.Interfaces
{
    public interface IWebScrapperService
    {
        Task<ResultDto> GetDataByTagsAsync(WebSiteToScrap website);
        Task<ResultDto> DownloadImagesAsync(WebSiteToScrap website);
        Task<(ResultDto restultDto, Dictionary<int, string> dataDictionary)> GetImagesInBase64FormatAsync(WebSiteToScrap website);
        Task<(ResultDto resultD, byte[] byteArray)> GetImageById(int id, WebSiteToScrap website);

        Task<ResultDto> GetDataFromUrlAsync(WebSiteToScrap website);
        Task<ResultDto> GetDataByAllTagsAsync(WebsiteWithMultipleTags website);
    }
}
