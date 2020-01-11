using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TvMazeScrapper.Models
{
    public class PageNumberModel
    {
        [Key]
        public int PageNumberModelId { get; set; }
        public int PageNumber { get; set; }
    }
}
