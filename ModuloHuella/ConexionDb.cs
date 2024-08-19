using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using ModuloHuella;

namespace ModuloHuella
{
    public class ConexionDb : DbContext
    {
        public ConexionDb(DbContextOptions<ConexionDb> options)
            : base(options)
        {
        }

        public DbSet<Cliente> Cliente { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
       
        {
            optionsBuilder.UseSqlite("Data Source=HuellaDb.db");
        }

    }
}

public class Cliente
{
    public int IdCliente { get; set; }
    public string Nombre { get; set; }
    public string ApellidoP { get; set; }
    public string AppellidoM { get; set; }
    public int Celular { get; set; }
    public string Correo { get; set; }
    public byte[] Huella { get; set; }
    public byte[] Foto { get; set; }

}

public class Datos
{
    private readonly ConexionDb _dbContext;

    public Datos()
    {
        var options = new DbContextOptionsBuilder<ConexionDb>().
            UseSqlite("Data Source=HuellaDb.db").
            Options;

        _dbContext = new ConexionDb(options);

    }
}
