using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using lol_check_scheduler.src.infrastructure.database.model;
using Microsoft.EntityFrameworkCore;

namespace lol_check_scheduler.src.infrastructure.database
{
    public class DatabaseContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<Summoner>? Summoner { get; set; }
        public DbSet<Device>? Device { get; set; }
        public DbSet<Subscriber>? Subscriber { get; set; }

        public object? T { get; internal set; }
    }
}