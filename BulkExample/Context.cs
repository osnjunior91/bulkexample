using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace BulkExample
{
    public class Context : DbContext
    {
        private DbSet<Partida> Partidas { get; set; }

        public double SalvaPartidasEntity(List<Partida> partidas)
        {
            Stopwatch sw = new Stopwatch();
            sw = Stopwatch.StartNew();
            Partidas.AddRange(partidas);
            this.SaveChanges();
            sw.Stop();
            return TimeSpan.FromMilliseconds(sw.ElapsedMilliseconds).TotalSeconds;
        }

        public List<Partida> BuscarPartidas()
        {
            return Partidas.ToList();
        }

        public double RemoverTodosEntity(List<Partida> partidas)
        {
            Stopwatch sw = new Stopwatch();
            sw = Stopwatch.StartNew();
            Partidas.RemoveRange(partidas);
            this.SaveChanges();
            sw.Stop();
            return TimeSpan.FromMilliseconds(sw.ElapsedMilliseconds).TotalSeconds;
        }

        public double SalvaPartidasBulk(List<Partida> partidasBulk)
        {
            Stopwatch sw = new Stopwatch();
            sw = Stopwatch.StartNew();
            this.BulkInsert(partidasBulk);
            sw.Stop();
            return TimeSpan.FromMilliseconds(sw.ElapsedMilliseconds).TotalSeconds;
        }

        public double RemoverTodosBulk(List<Partida> partidasBulk)
        {
            Stopwatch sw = new Stopwatch();
            sw = Stopwatch.StartNew();
            this.BulkDelete(partidasBulk);
            return TimeSpan.FromMilliseconds(sw.ElapsedMilliseconds).TotalSeconds;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Data Source=DESKTOP-NIIU474\SQLEXPRESS;Initial Catalog=BULKEF;Persist Security Info=True;User ID=sa;Password=sa");
            base.OnConfiguring(optionsBuilder);
        }
    }
}
