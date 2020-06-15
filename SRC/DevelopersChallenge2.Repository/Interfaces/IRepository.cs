using DevelopersChallenge2.DAL.Context;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DevelopersChallenge2.Repository.Interfaces
{
    public interface IRepository
    {
        SqlDbContext DbContext { get; }
        long IdUsuario { get; }
        IRepositoryFactory RepositoryFactory { get; }
        void Insert<TEntity>(TEntity entidade, bool salvarContexto = true) where TEntity : class, new();
        void Edit<TEntity>(TEntity entidade, bool salvarContexto = true) where TEntity : class, new();
        void Delete<TEntity>(TEntity entidade, bool salvarContexto = true) where TEntity : class, new();
        IQueryable<TEntity> SearchFor<TEntity>(Expression<Func<TEntity, bool>> predicado) where TEntity : class, new();
        IQueryable<TEntity> GetAll<TEntity>() where TEntity : class, new();
        IQueryable<TEntity> GetAllActives<TEntity>() where TEntity : class, new();
        TEntity SearchForId<TEntity>(params object[] ids) where TEntity : class, new();
        void Save();
        object UnProxy<TEntity>(TEntity proxyObject) where TEntity : class, new();
        void Attach<TEntity>(TEntity entity, EntityState state = EntityState.Unchanged) where TEntity : class, new();
        DbContextTransaction BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);
    }

    public interface IRepository<TEntity> : IRepository
        where TEntity : class, new()
    {
        void Insert(TEntity entidade, bool salvarContexto = true);
        void Edit(TEntity entidade, bool salvarContexto = true);
        void Delete(TEntity entidade, bool salvarContexto = true);
        IQueryable<TEntity> SearchFor(Expression<Func<TEntity, bool>> predicado);
        IQueryable<TEntity> GetAll();
        IQueryable<TEntity> GetAllActives();
        TEntity GetById(params object[] ids);
        object UnProxy(TEntity proxyObject);
        void Attach(TEntity entity, EntityState state = EntityState.Unchanged);
    }
}
