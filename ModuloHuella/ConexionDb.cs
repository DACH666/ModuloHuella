using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace ModuloHuella
{
    public class ConexionDb : DbContext
    {
        public ConexionDb(DbContextOptions<ConexionDb> options) 
            : base(options)
        {
        }
        
        public DbSet<Cliente> Clientes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=ModuloHuella.db");
        }

    }
}
