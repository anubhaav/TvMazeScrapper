using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TvMazeScrapper.Models
{
    public class ShowCastResponseModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public List<CastResponseListModel> Cast { get; set; }
    }
    public class CastResponseListModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Birthday { get; set; }
    }
}
