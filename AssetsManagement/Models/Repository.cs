using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;


    public class Repository<TEntity> : IRepository<TEntity> where TEntity:class
    {
      
        private CARACCOUNTWebEntities _ctx;
        private DbSet <TEntity> _set;
        public Repository(CARACCOUNTWebEntities ctx)
        {
            _ctx = ctx;
            _set = _ctx.Set<TEntity>();

        }
        public Repository()
        {
            _ctx = new CARACCOUNTWebEntities();
            _set = _ctx.Set<TEntity>();

        }
        public IQueryable<TEntity> GetAll()
        {
            return _set;
        }

        public List<TEntity> GetAllBind()
        {
            return GetAll().ToList();
        }
        public TEntity GetById(params object[] Id)
        {
            return _set.Find(Id);
        }
        public TEntity Add(TEntity entity)
        {
            _set.Add(entity);
            return _ctx.SaveChanges() > 0 ? entity : null;
        }

        public virtual bool Delete(TEntity entity)
        {
        
        _set.Attach(entity);
        _set.Remove(entity);
            return _ctx.SaveChanges() > 0;
        }

             

        public bool Update(TEntity entity)
        {
            _set.Attach(entity);
            _ctx.Entry(entity).State = EntityState.Modified;
            return _ctx.SaveChanges() > 0;
         }
    }

