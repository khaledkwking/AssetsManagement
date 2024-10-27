using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DAL;


public class Repository_HR<TEntity> :  IRepository<TEntity> where TEntity : class
{

    private GPFEmployeesEntities _ctx;
    private DbSet<TEntity> _set;
    public int UserId;

    public Repository_HR(GPFEmployeesEntities ctx)
        {
         
             ctx.Database.Connection.ConnectionString = DAL.secureData.HRconString;
            _ctx = ctx;
            _set = _ctx.Set<TEntity>();

        }
        public Repository_HR()
        {
            _ctx = new GPFEmployeesEntities();
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

            if (checkProperty(entity, "CreatedOn"))
            {
                _ctx.Entry(entity).CurrentValues["CreatedOn"] = DateTime.Now;
            }
            if (checkProperty(entity, "CreatedBy"))
            {
                _ctx.Entry(entity).CurrentValues["CreatedBy"] = SesssionUser.GetCurrentUserId();
            }

             return _ctx.SaveChanges() > 0 ? entity : null;
        }

        public virtual bool Delete(TEntity entity)
        {

        //_set.Remove(entity);
        _set.Attach(entity);
        if (checkProperty(entity, "IsDeleted"))
            {
            _ctx.Entry(entity).CurrentValues["IsDeleted"] = true;
                if (checkProperty(entity, "DeletedOn"))
                {
                    _ctx.Entry(entity).CurrentValues["DeletedOn"] = DateTime.Now;

                 }
                if (checkProperty(entity, "DeletedBy"))
                {
                    _ctx.Entry(entity).CurrentValues["DeletedBy"] = SesssionUser.GetCurrentUserId();
                }
        
              
                return _ctx.SaveChanges() > 0;

             }
            else
            {
                return false;
            }
           
        }

    
    public bool Update(TEntity entity)
    {

                if (checkProperty(entity, "UpdateOn"))
                {
                    _ctx.Entry(entity).CurrentValues["UpdateOn"] = DateTime.Now; 
                }
                if (checkProperty(entity, "UpdateBy"))
                {
                    _ctx.Entry(entity).CurrentValues["UpdateBy"] = SesssionUser.GetCurrentUserId();
                }
                 _set.Attach(entity);
                _ctx.Entry(entity).State = EntityState.Modified;
            
                return _ctx.SaveChanges() > 0;
     }


    public bool checkProperty(TEntity entity, string PropertyName)
    {
        PropertyInfo info = entity.GetType().GetProperty(PropertyName);

        if (info != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

}

