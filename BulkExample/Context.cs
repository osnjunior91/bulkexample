using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BulkExample
{
    public class Context : DbContext
    {
        public DbSet<Partida> Partidas { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Data Source=DESKTOP-NIIU474\SQLEXPRESS;Initial Catalog=BULKEF;Persist Security Info=True;User ID=sa;Password=sa");
            base.OnConfiguring(optionsBuilder);
        }
    }
}
