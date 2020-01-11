using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TvMazeScrapper.Services
{
    public interface IThrottledHttpClient
    {
        Task StartScrappingAsync(int pageNumber = 1);
    }
}
