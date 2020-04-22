using System;
using System.Collections.Generic;
using System.Text;

namespace MovieShop.Core.Entites
{
    public class MovieGenre
    {
        public int MovieId { get; set; }
        public int GenreId { get; set; }
        public Movie Movie { get; set; }
        public Genre Genre { get; set; }
    }
}
