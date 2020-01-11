using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TvMazeScrapper.Models
{
    public class CastModel
    {
        public long CastId { get; set; }
        public long Id { get; set; }
        public string Name { get; set; }
        public string Birthday { get; set; }
        public int ShowModelId { get; set; }
    }
}
