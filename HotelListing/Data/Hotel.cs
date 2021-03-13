using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace HotelListing.Data
{
    public class Hotel
    {
        public int Id { get; init; }
        public string Name { get; set; }
        public string Address { get; set; }
        public double Rating { get; set; }

        [ForeignKey(nameof(Country))] // This data annotation belongs to the 2 fields below
        public int CountryId { get; set; }

        public Country Country { get; set; }
    }
}
