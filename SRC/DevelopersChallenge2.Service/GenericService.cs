using DevelopersChallenge2.DAL.Context;
using DevelopersChallenge2.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevelopersChallenge2.Service
{
    public class GenericService : BaseService
    {
        public GenericService()
        { }
    }

    public class GenericoService<TRepository> : GenericService, IGenericoService<TRepository>
        where TRepository : IRepository
    {
        private Type DbContextType;

        public long IdUsuario { get; private set; }

        private IRepositoryFactory _RepositoryFactory;
        public IRepositoryFactory RepositoryFactory
        {
            get
            {
                if (_RepositoryFactory == null)
                {
                    _RepositoryFactory = new RepositoryFactory(IdUsuario, DbContextType);
                }
                return _RepositoryFactory;
            }
            set { _RepositoryFactory = value; }
        }

        private TRepository _Repository;
        public TRepository Repository
        {
            get
            {
                if (_Repository == null)
                {
                    _Repository = RepositoryFactory.CriarRepositorio<TRepository>();
                }
                return _Repository;
            }
            set { _Repository = value; }
        }

        public GenericoService(IRepositoryFactory repositoryFactory = null, Type dbContextType = null)
        {
            if (repositoryFactory != null && repositoryFactory.IdUsuario != 0)
            {
                IdUsuario = repositoryFactory.IdUsuario;
            }

            RepositoryFactory = repositoryFactory;
            DbContextType = dbContextType;
        }

        public virtual DbContextTransaction BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            return Repository.BeginTransaction(isolationLevel);
        }

        public void Dispose()
        {
            RepositoryFactory.Dispose();
        }       
    }

    public class GenericoService<TRepository, TDBContext> : GenericoService<TRepository>
        where TDBContext : SqlDbContext
        where TRepository : IRepository
    {
        public GenericoService()
            : this(null)
        { }

        public GenericoService(IRepositoryFactory repositoryFactory)
            : base(repositoryFactory, typeof(TDBContext))
        { }
    }

    public interface IGenericoService<TRepository> : IDisposable
        where TRepository : IRepository
    {
        TRepository Repository { get; }
        long IdUsuario { get; }

        DbContextTransaction BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);
    }
}
