using InmobiliariaAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace InmobiliariaAPI
{

    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<Propietario> Propietarios { get; set; }
        public DbSet<Inmueble> Inmuebles { get; set; }
    }
}
