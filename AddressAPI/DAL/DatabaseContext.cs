using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AddressAPI.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AddressAPI.DAL
{
    public class DatabaseContext:DbContext
    {
        public DbSet<Address> Addresses { get; set; }
        public DatabaseContext(DbContextOptions<DatabaseContext> configuration):base(configuration)
        {

        }
    }
}
