using DevelopersChallenge2.Model;
using DevelopersChallenge2.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevelopersChallenge2.DAL.Context
{
    public class SqlDbContext : DbContext
    {
        #region _____ PROPERTIES _____
        public DbSet<Bank> Bank { get; set; }
        public DbSet<Transaction> Transaction { get; set; }
        #endregion

        static SqlDbContext()
        {
            Database.SetInitializer<SqlDbContext>(null);
        }

        public SqlDbContext()
            : base(ConfigurationManager.AppSettings["connectionString"].ToString())
        {

        }

        public override Task<int> SaveChangesAsync()
        {
            var changed = ChangeTracker.Entries<ISoftDeletable>();

            if (changed != null)
            {
                foreach (var entry in changed.Where(e => e.State == EntityState.Deleted))
                {
                    entry.State = EntityState.Unchanged;
                    entry.Entity.EstReg = "H";
                }
            }

            var baseChanged = ChangeTracker.Entries<IBaseModel>();

            if (baseChanged != null)
            {
                foreach (var entry in baseChanged.Where(e => e.State == EntityState.Added))
                {
                    entry.Entity.DataReg = System.DateTime.Now;
                    entry.Entity.EstReg = "A";
                }
            }

            return base.SaveChangesAsync();
        }

        public override int SaveChanges()
        {
            var changed = ChangeTracker.Entries<ISoftDeletable>();

            if (changed != null)
            {
                foreach (var entry in changed.Where(e => e.State == EntityState.Deleted))
                {
                    entry.State = EntityState.Unchanged;
                    entry.Entity.EstReg = "H";
                }
            }

            var baseChanged = ChangeTracker.Entries<IBaseModel>();

            if (baseChanged != null)
            {
                foreach (var entry in baseChanged.Where(e => e.State == EntityState.Added))
                {
                    entry.Entity.DataReg = System.DateTime.Now;
                    entry.Entity.EstReg = "A";
                }
            }

            return base.SaveChanges();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Conventions.Remove<PluralizingEntitySetNameConvention>();
            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

            modelBuilder.Entity<Transaction>()
              .Property(x => x.Value)
              .HasPrecision(10, 2);
        }
    }
}
