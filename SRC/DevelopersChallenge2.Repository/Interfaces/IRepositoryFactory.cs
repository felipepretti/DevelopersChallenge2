using DevelopersChallenge2.DAL.Context;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core;
using System.Data.Entity.Validation;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DevelopersChallenge2.Repository.Interfaces
{
    public interface IRepositoryFactory : IDisposable
    {
        SqlDbContext DbContext { get; }
        Type TipoDbContext { get; }
        long IdUsuario { get; }
        bool PermitirSalvarContexto { get; set; }
        TRepository CriarRepositorio<TRepository>() where TRepository : IRepository;
        void SalvarContexto();
    }

    public class RepositoryFactory : IRepositoryFactory
    {
        public SqlDbContext DbContext { get; private set; }
        public Type TipoDbContext { get; private set; }
        public long IdUsuario { get; private set; }

        private bool _PermitirSalvarContexto = true;
        public bool PermitirSalvarContexto
        {
            get { return _PermitirSalvarContexto; }
            set { _PermitirSalvarContexto = value; }
        }

        public RepositoryFactory(long idUsuario, Type tipoDbContext = null)
            : this(tipoDbContext)
        {
            IdUsuario = idUsuario;
        }

        internal RepositoryFactory(Type dbContextType = null)
        {
            TipoDbContext = dbContextType;

            if (TipoDbContext == null)
            {
                TipoDbContext = typeof(SqlDbContext);
            }

            DbContext = (SqlDbContext)Activator.CreateInstance(TipoDbContext);
        }

        public TRepository CriarRepositorio<TRepository>()
            where TRepository : IRepository
        {
            if (typeof(TRepository).IsInterface)
            {
                return (TRepository)CriarRepositorioPorInterface(typeof(TRepository));
            }

            return (TRepository)CriarRepositorio(typeof(TRepository));
        }

        private IRepository CriarRepositorioPorInterface(Type interfaceRepositorio)
        {
            var repositorios = Assembly.GetCallingAssembly().GetTypes();
            var implementacoesRepositorio = repositorios.GetTypesImplementsInterface(interfaceRepositorio);

            return CriarRepositorio(implementacoesRepositorio.FirstOrDefault());
        }

        private IRepository CriarRepositorio(Type tipoRepositorio)
        {
            if (tipoRepositorio.GetInterface(typeof(IRepository).Name) != null)
            {
                return (IRepository)Activator.CreateInstance(tipoRepositorio, this);
            }

            if (tipoRepositorio.GetConstructors().Any(o => o.GetParameters().Length == 2))
            {
                return (IRepository)Activator.CreateInstance(tipoRepositorio, IdUsuario);
            }

            return (IRepository)Activator.CreateInstance(tipoRepositorio);
        }

        public void Dispose()
        {
            if (DbContext != null)
            {
                if (DbContext.Database.Connection.State != ConnectionState.Closed)
                {
                    DbContext.Database.Connection.Close();
                }

                DbContext.Database.Connection.Dispose();
                DbContext.Dispose();
            }

            DbContext = null;
        }

        public void SalvarContexto()
        {
            if (!PermitirSalvarContexto)
            {
                return;
            }

            try
            {
                DbContext.SaveChanges();
            }
#if (DEBUG)
            catch (DbEntityValidationException dbEntityValidationEx)
            {
                string error = string.Empty;

                foreach (var validationErrors in dbEntityValidationEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        error += string.Format("ERROR SAVING CONTEXT");
                    }
                }

                System.Diagnostics.Debug.WriteLine(error);
                throw new DbEntityValidationException(error, dbEntityValidationEx);
            }
#endif
            catch (OptimisticConcurrencyException optimisticConcurrencyException)
            {
                throw new OptimisticConcurrencyException("Concurrency error when saving to the database", optimisticConcurrencyException.InnerException);
            }
        }
    }
}
