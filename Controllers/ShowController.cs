using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TvMazeScrapper.Data;
using TvMazeScrapper.Models;
using TvMazeScrapper.Helpers;

namespace TvMazeScrapper.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ShowController : ControllerBase
    {
        private readonly ShowCastDbContext _context;

        public ShowController(ShowCastDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Url: https://localhost:44314/api/Show/GetShowCast/Id/PageNumber/PerPage
        /// Send JSON response of a Single Show and List of Cast Information where cast list is ordered by birthday descending 
        /// with null Birthdays to the last. It also support pagination.
        /// </summary>
        /// <param name="Id">Id of the Show. Default 1.</param>
        /// <param name="PageNumber">Page number of the list. Default 1.</param>
        /// <param name="PerPage">Per page result which is sent. Default 10.</param>
        /// <returns>Show with Cast List.</returns>
        [HttpGet("{Id?}/{PageNumber?}/{PerPage?}")]
        public async Task<IActionResult> GetShowCast(long Id = 1, int? PageNumber = 1, int? PerPage = 10)
        {
            try
            {
                var showCast = await PaginationHelper<ShowModel>.CreateAsync(
                    _context.ShowDb
                    .AsQueryable()
                    .AsNoTracking()
                    .Where(s => s.Id == (Id))
                    .Include(s => s.Cast),
                    PageNumber ?? 1,
                    PerPage ?? 10
                    );

                /** 
                 * If ShowCast's Length is Zero or Null then
                 * Send Message that the shows are not available 
                 * as they are not scrapped or not available on TvMaze.
                 **/
                if (showCast == null || showCast.Count == 0)
                {
                    return new OkObjectResult(new HttpResponseModel { Message = $"Show with Id: '{Id}' is not available.", IsError = false });
                }

                /** 
                 * Show & Cast List Response as described int the PDF.
                 * There can also be JOIN or other methods but for simplicity of this task 
                 * I Created simple Model class.
                 **/
                List<ShowCastResponseModel> showCastResponseObject = new List<ShowCastResponseModel>();
                List<CastResponseListModel> castList = new List<CastResponseListModel>();

                foreach (var castResponse in showCast[0].Cast)
                {
                    castList.Add(new CastResponseListModel { Id = castResponse.Id, Name = castResponse.Name, Birthday = castResponse.Birthday });
                }

                // Creating casts with Descending Birthda, and null birthday's at the last.
                showCastResponseObject.Add(
                    new ShowCastResponseModel
                    {
                        Id = showCast[0].Id,
                        Name = showCast[0].Name,
                        Cast = castList.OrderByDescending(c => ((c.Birthday != null) ? DateTime.Parse(c.Birthday) : DateTime.MinValue)).ToList()
                    });

                return new OkObjectResult(showCastResponseObject);
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult(new HttpResponseModel { Message = e.Message, IsError = true });
            }
        }


        /// <summary>
        /// Url: https://localhost:44314/api/Show/GetShowCastList/FromId/ToId/PageNumber/PerPage
        /// Send JSON response of a Multiple Shows and List of Cast Information where cast list is ordered by birthday descending 
        /// with null Birthdays to the last. It also supports pagination.
        /// </summary>
        /// <param name="FromId">Start Id of the Show. Default 1.</param>
        /// <param name="ToId">End Id of the Show. Default 250.</param>
        /// <param name="PageNumber">Page number of the list. Default 1.</param>
        /// <param name="PerPage">Per page result which is sent. Default 10.</param>
        /// <returns>List of Shows with Cast</returns>
        [HttpGet("{FromId?}/{ToId?}/{PageNumber?}/{PerPage?}")]
        public async Task<IActionResult> GetShowCastList(long FromId = 1, long ToId = 250, int? PageNumber = 1, int? PerPage = 10)
        {
            try
            {
                //Get Paginated Show List from Pagination Helper
                var showCast = await PaginationHelper<ShowModel>.CreateAsync(
                    _context.ShowDb
                    .AsQueryable()
                    .AsNoTracking()
                    .Where(s => s.Id >= FromId && s.Id <= ToId)
                    .Include(s => s.Cast),
                    PageNumber ?? 1,
                    PerPage ?? 10
                    );

                /** 
                 * If ShowCast's Length is Zero or Null then
                 * Send Message that the shows are not available 
                 * as they are not scrapped or not available on TvMaze.
                 **/
                if (showCast == null || showCast.Count == 0)
                {
                    return new OkObjectResult(new HttpResponseModel { Message = $"Shows from Id: '{FromId}' to '{ToId}' are not available.", IsError = false });
                }

                /** 
                 * List for Show & Cast Response as described int the PDF.
                 * There can also be JOIN or other methods but for simplicity of this task 
                 * I Created simple Model class.
                 **/
                List<ShowCastResponseModel> showCastResponseObject = new List<ShowCastResponseModel>();
                foreach (var response in showCast)
                {
                    List<CastResponseListModel> castList = new List<CastResponseListModel>();

                    foreach (var castResponse in response.Cast)
                    {
                        castList.Add(new CastResponseListModel { Id = castResponse.Id, Name = castResponse.Name, Birthday = castResponse.Birthday });
                    }

                    // Creating casts with Descending Birthda, and null birthday's at the last.
                    showCastResponseObject.Add(
                        new ShowCastResponseModel
                        {
                            Id = response.Id,
                            Name = response.Name,
                            Cast = castList.OrderByDescending(c => ((c.Birthday != null) ? DateTime.Parse(c.Birthday) : DateTime.MinValue)).ToList()
                        });
                }
                return new OkObjectResult(showCastResponseObject);
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult(new HttpResponseModel { Message = e.Message, IsError = true });
            }
        }
    }
}