using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using TvMazeScrapper.Data;
using TvMazeScrapper.Models;

namespace TvMazeScrapper.Services
{
    public class ThrottledHttpClient : IThrottledHttpClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = @"http://api.tvmaze.com";

        public ThrottledHttpClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// Scraps TV Maze information and store into SQLite database with Throttling API calling rate as per TVMaze specifications.
        /// </summary>
        /// <param name="PageNumber">Page number where you want to start scrapping. Default 0.</param>
        /// <returns>VOID</returns>
        public async Task StartScrappingAsync(int pageNumber = 0)
        {
            //Throttling API call to 0.5f.
            float limitingPeriodInSeconds = 0.5f;

            //Limiting the request to only 1.
            //As scrapping is compute intensive.
            int requestLimit = 1;


            //Using SemaphoreSlim for throttling.
            var throttler = new SemaphoreSlim(requestLimit);

            //For While Loop, till API doesn't send 404.
            //As per TvMaze 404 response indicate end of Show List.
            var isTrue = true;

            /**
             * Starting Scrapping from last page number
             * Skipping already added Pages or Show Page.
             * See: http://api.tvmaze.com/shows?page={pageNumber}
             * If User has not provided the page number
             * then check the database for last page number
             * in the database.
             **/
            if (pageNumber == 0)
            {
                var pageDb = new ShowCastDbContext();
                var pageNo = pageDb.PageNumberDb.Where(p => p.PageNumberModelId == 1).FirstOrDefault();
                if (pageNo != null)
                {
                    pageNumber = pageNo.PageNumber + 1;
                }
            }

            //Getting and operating on API.
            while (isTrue)
            {
                await throttler.WaitAsync();

                //Delay for calling the api if earlier request is earlier than 0.5 Seconds.
                //Just for Saftey of Rate Limit error.
                await Task.Delay(TimeSpan.FromSeconds(limitingPeriodInSeconds));

                //See: http://api.tvmaze.com/shows?page={pageNumber}
                var task = _httpClient.GetAsync($"{_baseUrl}/shows?page={pageNumber}");

                try
                {
                    var response = await task;

                    //Only if Status code is 200 then operate else, release throttling 
                    //and call the api again.
                    if (response.StatusCode != System.Net.HttpStatusCode.NotFound)
                    {
                        try
                        {
                            using (var db = new ShowCastDbContext())
                            {
                                db.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

                                //Deserialising from Response
                                var shows = await response.Content.ReadAsAsync<List<ShowResponseModel>>();

                                var showModelList = new List<ShowModel>();

                                foreach (var show in shows)
                                {
                                    //If Id already present then skip the show entry.
                                    //Safeguard from any duplication
                                    if (!db.ShowDb.Any(s => s.Id == show.Id))
                                    {
                                        //Get Cast Data
                                        //See: http://api.tvmaze.com/shows/{Show-Id}/cast
                                        var castData = await _httpClient.GetAsync($"http://api.tvmaze.com/shows/{show.Id}/cast");

                                        var casts = await castData.Content.ReadAsAsync<List<CastResponseModel>>();

                                        List<CastModel> _castList = new List<CastModel>();

                                        foreach (var cast in casts)
                                        {
                                            /** 
                                              * Check for any duplicate entry of Casts in the Request.
                                              * There are many Shows where the request contains Duplicate Cast ID
                                              **/
                                            if (!_castList.Contains(
                                                new CastModel
                                                {
                                                    Id = cast.Person.Id,
                                                    Birthday = cast.Person.Birthday,
                                                    Name = cast.Person.Name
                                                }))
                                            {
                                                _castList.Add(new CastModel
                                                {
                                                    Id = cast.Person.Id,
                                                    Birthday = cast.Person.Birthday,
                                                    Name = cast.Person.Name
                                                });
                                            }
                                        }
                                        showModelList.Add(new ShowModel
                                        {
                                            Id = show.Id,
                                            Name = show.Name,
                                            Cast = _castList
                                        });
                                    }

                                    //Deplay for calling CAST api, Rate Limit Safeguard.
                                    await Task.Delay(TimeSpan.FromSeconds(limitingPeriodInSeconds));
                                   
                                }

                                //Saving Show to the Databas through optimized method.
                                await db.ShowDb.AddRangeAsync(showModelList);
                                await db.SaveChangesAsync();

                                pageNumber += 1;

                                //Saving Page Number of Scrapped Data to the DB.
                                var pageNo = db.PageNumberDb.Where(p => p.PageNumberModelId == 1).FirstOrDefault();

                                //If this is the first page number add page number = 1.
                                //Else increase the page number.
                                if (pageNo == null)
                                {
                                    db.PageNumberDb.Add(new PageNumberModel { PageNumberModelId = 1, PageNumber = 1 });
                                    await db.SaveChangesAsync();
                                }
                                else
                                {
                                    pageNo.PageNumber += 1;
                                    db.PageNumberDb.Update(pageNo);
                                    await db.SaveChangesAsync();
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine($"\t\t\t {pageNumber} Error in saving the Show and Cast with Page Number.");
                        }
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        //If Show list ends then Stop While loop.
                        isTrue = false;

                        //And Relase Throttle.
                        throttler.Release();
                    }

                    //When the Current Show Data ends
                    //release Throttle for a new call.
                    throttler.Release();
                }
                catch (HttpRequestException)
                {
                    Console.WriteLine($"\t\t\t {pageNumber} error out");
                    throttler.Release();
                }
            };
        }
    }

    #region HelperClasses
    public class ShowResponseModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class CastResponseModel
    {
        public Person Person { get; set; }

    }

    public class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Birthday { get; set; }
    }
    #endregion
}
