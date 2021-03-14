using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelListing.Data
{
    public class Country
    {
        public int Id { get; init; }
        public string Name { get; set; }
        public string ShortName { get; set; }

        // this won't go to the database. This is a relation between
        // the models themselves
        public virtual IList<Hotel> Hotels { get; set; }
    }
}
