using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TvMazeScrapper.Models
{
    public class ShowModel
    {
        public long ShowId { get; set; }
        public long Id { get; set; }
        public string Name { get; set; }
        public List<CastModel> Cast { get; set; }
    }
}
