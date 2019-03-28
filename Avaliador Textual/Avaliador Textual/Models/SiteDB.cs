using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;

namespace Avaliador_Textual.Models
{
    public class SiteDB : DbContext
    {
        public DbSet<Arquivo> Arquivos { get; set; }

        // Criado override para não criar as tabelas no plural automaticamente
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            base.OnModelCreating(modelBuilder);
        }

        public override int SaveChanges()
        {
            var entities = ChangeTracker.Entries().Where(x => x.Entity is Arquivo && (x.State == EntityState.Added || x.State == EntityState.Modified));
            foreach (var entity in entities)
            {
                ((Arquivo)entity.Entity).Data = DateTime.Now;
                ((Arquivo)entity.Entity).InicializaArquivo();
            }

            return base.SaveChanges();
        }

        public SiteDB() : base("DefaultConnection")
        {

        }
    }   
}