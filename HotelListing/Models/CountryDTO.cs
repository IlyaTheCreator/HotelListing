﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HotelListing.Models
{
    public class CreateCountryDTO
    {
        [Required]
        [StringLength(maximumLength: 50, ErrorMessage = "Name is too long")]
        public string Name { get; set; }

        [Required]
        [StringLength(maximumLength: 2, ErrorMessage = "Short name is too long")]
        public string ShortName { get; set; }
    }

    public class CountryDTO : CreateCountryDTO
    {
        public int Id { get; init; }
        public IList<HotelDTO> Hotels { get; set; }
    }
}
