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
        Task<ResultDto> GetDataByTagsAsync(WebsitToScrap website);
        Task<ResultDto> DownloadImagesAsync(WebsitToScrap website);
        Task<(ResultDto resultD, byte[] byteArray)> GetImageById(int id, WebsitToScrap website);
    }
}
