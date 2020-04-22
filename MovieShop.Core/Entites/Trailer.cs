using System;
using System.Collections.Generic;
using System.Text;

namespace MovieShop.Core.Entites
{
    public class Trailer
    {
        public int Id { get; set; }
        public String Name { get; set; }
        public String TrailerUrl { get; set; }
        public int MovieId { get; set; }
        public Movie Movie { get; set; }
    }
}
