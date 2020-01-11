using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TvMazeScrapper.Services;
using TvMazeScrapper.Models;

namespace TvMazeScrapper.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ScrapController : ControllerBase
    {
        private readonly IThrottledHttpClient _throttledHttpClient;

        public ScrapController(ThrottledHttpClient throttledHttpClient)
        {
            _throttledHttpClient = throttledHttpClient;
        }

        /// <summary>
        /// Url: https://localhost:44314/api/Scrap/StartScrap/PageNumber
        /// Scraps TV Maze information and store into SQLite database with Throttling API calling rate as per TVMaze specifications.
        /// </summary>
        /// <param name="PageNumber">Page number where you want to start scrapping. Default 0.</param>
        /// <returns>
        /// HttpResponseModel with a Message and IsError.
        /// </returns>
        [HttpGet("{PageNumber?}")]
        public async Task<IActionResult> StartScrap(int PageNumber = 0)
        {
            try
            {
                //Calls Custom HttpClient with Throttling. 
                //No await operator as Scrapping can take more than one hour due to rate timie of 20 Requests/10 Sec
                _throttledHttpClient.StartScrappingAsync(PageNumber);
                return new OkObjectResult(new HttpResponseModel { Message = "Successfully added Scrapping task." , IsError=false});
            }catch(Exception e)
            {
                return new BadRequestObjectResult(new HttpResponseModel { Message = e.Message, IsError = true });
            }
        }
    }
}