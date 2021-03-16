using HotelListing.Configurations.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelListing.Data
{

    // This class defines the bridge between our defined classes (Country & Hotel)
    // and the actual database

    // If you are fine with the default user you don't need to provide the <ApiUser>
    public class DatabaseContext : IdentityDbContext<ApiUser> // just DbContext without identity package
    {
        public DatabaseContext(DbContextOptions options) : base(options)
        {}

        public DbSet<Country> Countries { get; set; }
        public DbSet<Hotel> Hotels { get; set; }

        // seeding in action
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder); // needed for the identity package

            // (Configurations/Entities)
            builder.ApplyConfiguration(new CountryConfiguration());
            builder.ApplyConfiguration(new HotelConfiguration());
            builder.ApplyConfiguration(new RoleConfiguration()); 
        }
    }
}
