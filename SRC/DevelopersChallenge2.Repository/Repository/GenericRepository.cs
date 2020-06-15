using DevelopersChallenge2.DAL.Context;
using DevelopersChallenge2.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DevelopersChallenge2.Repository.Repository
{
    public class GenericoRepository : IRepository
    {
        public IRepositoryFactory RepositoryFactory { get; private set; }

        protected const string PROPRIEDADE_ID = "Id";

        public SqlDbContext DbContext { get { return RepositoryFactory.DbContext; } }
        public long IdUsuario { get { return RepositoryFactory.IdUsuario; } }

        public GenericoRepository(IRepositoryFactory repositoryFactory)
        {
            RepositoryFactory = repositoryFactory;
        }

        public virtual DbContextTransaction BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            return DbContext.Database.BeginTransaction(isolationLevel);
        }

        protected TEntity AnexarCompleto<TEntity>(TEntity entidade, bool verificarPropriedadesGenericas = true, string chavePrimaria = PROPRIEDADE_ID)
             where TEntity : class, new()
        {
            if (DbContext.Entry(entidade).State == EntityState.Detached)
            {
                var entidadeAnexada = this.SearchForId<TEntity>((int)entidade.GetType().GetProperty(chavePrimaria).GetValue(entidade));

                DbContext.Entry(entidadeAnexada).CurrentValues.SetValues(entidade);
                return entidadeAnexada;
            }
            else
            {
                return entidade;
            }
        }

        public virtual void Insert<TEntity>(TEntity entidade, bool salvarContexto = true)
             where TEntity : class, new()
        {
            DbContext.Set<TEntity>().Add(entidade);

            if (salvarContexto)
            {
                this.Save();
            }
        }

        public virtual void InsertBatch<TEntity>(IEnumerable<TEntity> entidades)
             where TEntity : class, new()
        {
            DbContext.Set<TEntity>().AddRange(entidades);

            this.Save();
        }

        public virtual void Edit<TEntity>(TEntity entidade, string chavePrimaria, bool salvarContexto = true)
             where TEntity : class, new()
        {
            var entidadeAnexada = this.AnexarCompleto(entidade, true, chavePrimaria);
            DbContext.Entry(entidadeAnexada).State = EntityState.Modified;

            if (salvarContexto)
            {
                this.Save();
            }
        }

        public virtual void Edit<TEntity>(TEntity entidade, bool salvarContexto = true)
             where TEntity : class, new()
        {
            Edit(entidade, PROPRIEDADE_ID, salvarContexto);
        }

        public virtual void EditBatch<TEntity>(IEnumerable<TEntity> entidades)
             where TEntity : class, new()
        {
            this.Save();
        }

        public virtual void Delete<TEntity>(TEntity entidade, bool salvarContexto = true)
             where TEntity : class, new()
        {
            var entidadeAnexada = this.AnexarCompleto(entidade);
            DbContext.Set<TEntity>().Remove(entidadeAnexada);

            if (salvarContexto)
            {
                this.Save();
            }
        }

        public virtual void DeleteBatch<TEntity>(IEnumerable<TEntity> entidades)
             where TEntity : class, new()
        {
            DbContext.Set<TEntity>().RemoveRange(entidades);

            this.Save();
        }

        public virtual IQueryable<TEntity> SearchFor<TEntity>(Expression<Func<TEntity, bool>> predicado)
             where TEntity : class, new()
        {
            return DbContext.Set<TEntity>()
                .Where(predicado);
        }

        public virtual IQueryable<TEntity> GetAll<TEntity>()
             where TEntity : class, new()
        {
            return DbContext.Set<TEntity>();
        }

        public virtual IQueryable<TEntity> GetAllActives<TEntity>()
             where TEntity : class, new()
        {
            var entities = this.GetAll<TEntity>();

            if (typeof(TEntity).GetProperty("EstReg") == null)
            {
                return entities;
            }

            var pe = Expression.Parameter(typeof(TEntity), "e");
            var esquerdo = Expression.Property(pe, typeof(TEntity).GetProperty("EstReg"));
            var direito = Expression.Constant("A");
            var predicadoWhere = Expression.Equal(esquerdo, direito);

            var chamadaExpressaoWhere = Expression.Call(
                typeof(Queryable),
                "Where",
                new Type[] { entities.ElementType },
                entities.Expression,
                Expression.Lambda<Func<TEntity, bool>>(predicadoWhere, new ParameterExpression[] { pe }));

            return entities.Provider.CreateQuery<TEntity>(chamadaExpressaoWhere);
        }

        public virtual TEntity SearchForId<TEntity>(params object[] ids)
             where TEntity : class, new()
        {
            return DbContext.Set<TEntity>().Find(ids);
        }

        public virtual void Save()
        {
            RepositoryFactory.SalvarContexto();
        }

        public virtual object UnProxy<TEntity>(TEntity proxyObject)
             where TEntity : class, new()
        {
            var proxyCreationEnabled = DbContext.Configuration.ProxyCreationEnabled;

            try
            {
                DbContext.Configuration.ProxyCreationEnabled = false;
                var poco = DbContext.Entry(proxyObject).CurrentValues.ToObject();
                return poco;
            }
            finally
            {
                DbContext.Configuration.ProxyCreationEnabled = proxyCreationEnabled;
            }
        }

        public virtual void Attach<TEntity>(TEntity entidade, EntityState state = EntityState.Unchanged)
             where TEntity : class, new()
        {
            DbContext.Entry(entidade).State = EntityState.Unchanged;
        }
    }

    public class GenericoRepository<TEntity> : GenericoRepository, IRepository<TEntity>
        where TEntity : class, new()
    {
        public GenericoRepository(IRepositoryFactory repositoryFactory)
            : base(repositoryFactory)
        { }

        public virtual void Insert(TEntity entidade, bool salvarContexto = true)
        {
            base.Insert(entidade, salvarContexto);
        }

        public virtual void Edit(TEntity entidade, bool salvarContexto = true)
        {
            base.Edit(entidade, salvarContexto);
        }

        public virtual void Delete(TEntity entidade, bool salvarContexto = true)
        {
            base.Delete(entidade, salvarContexto);
        }

        public virtual IQueryable<TEntity> SearchFor(Expression<Func<TEntity, bool>> predicado)
        {
            return base.SearchFor(predicado);
        }

        public virtual TEntity GetById(params object[] ids)
        {
            return base.SearchForId<TEntity>(ids);
        }

        public virtual IQueryable<TEntity> GetAll()
        {
            return base.GetAll<TEntity>();
        }

        public virtual IQueryable<TEntity> GetAllActives()
        {
            return base.GetAllActives<TEntity>();
        }

        public virtual object UnProxy(TEntity proxyObject)
        {
            return base.UnProxy(proxyObject);
        }

        public virtual void Attach(TEntity entity, EntityState state = EntityState.Unchanged)
        {
            base.Attach(entity, state);
        }
    }
}
