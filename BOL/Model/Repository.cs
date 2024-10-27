using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DAL;


public class Repository<TEntity> :  IRepository<TEntity> where TEntity : class
{

    private GPFAssetsEntities _ctx;
    private DbSet<TEntity> _set;
    public int UserId;

    public Repository(GPFAssetsEntities ctx)
        {

         
        //ConnectionStringSettings settings =  ConfigurationManager.ConnectionStrings["GPFAssetsEntities"];
          ctx.Database.Connection.ConnectionString =  DAL.secureData.ConString;
            _ctx = ctx;
            _set = _ctx.Set<TEntity>();

        }
        public Repository()
        {
            _ctx = new GPFAssetsEntities();
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
        //public virtual TEntity Add(TEntity entity)
        //{
        //    if (SesssionUser.GetCurrentUserId() <= 0)
        //    {
        //        return null;
        //    }
        //     _set.Add(entity);

        //    if (checkProperty(entity, "CreatedOn"))
        //    {
        //        _ctx.Entry(entity).CurrentValues["CreatedOn"] = DateTime.Now;
        //    }
        //    if (checkProperty(entity, "CreatedBy"))
        //    {
        //        _ctx.Entry(entity).CurrentValues["CreatedBy"] = SesssionUser.GetCurrentUserId();
        //    }
        //    if (checkProperty(entity, "UpdateOn"))
        //    {
        //        _ctx.Entry(entity).CurrentValues["UpdateOn"] = DateTime.Now;
        //    }
        //    if (checkProperty(entity, "UpdateBy"))
        //    {
        //        _ctx.Entry(entity).CurrentValues["UpdateBy"] = SesssionUser.GetCurrentUserId();
        //    }

        //    return _ctx.SaveChanges() > 0 ? entity : null;
           
        //}

    public virtual TEntity Add(TEntity entity)
    {
        if (SesssionUser.GetCurrentUserId() <= 0)
        {
            return null;
        }
        _set.Add(entity);
        int userId = SesssionUser.GetCurrentUserId();
        if (checkProperty(entity, "CreatedOn"))
        {
            _ctx.Entry(entity).CurrentValues["CreatedOn"] = DateTime.Now;
        }
        if (checkProperty(entity, "CreatedBy"))
        {
            _ctx.Entry(entity).CurrentValues["CreatedBy"] = userId;
        }
        if (checkProperty(entity, "UpdateOn"))
        {
            _ctx.Entry(entity).CurrentValues["UpdateOn"] = DateTime.Now;
        }
        if (checkProperty(entity, "UpdateBy"))
        {
            _ctx.Entry(entity).CurrentValues["UpdateBy"] = userId;
        }

        TEntity Addentity = _ctx.SaveChanges() > 0 ? entity : null;
        if (Addentity != null)
        {
                 //int Id = int.Parse(GetKey(Addentity).ToString());
                SaveLog(Addentity, "Insert", userId);
        }
        return Addentity;
    }
    public virtual int GetKey<T>(T entity, ref string ActionCode)
    {
        
        var metadata = ((IObjectContextAdapter)_ctx).ObjectContext.MetadataWorkspace;
        // Get the mapping between CLR types and metadata OSpace
        var objectItemCollection = ((ObjectItemCollection)metadata.GetItemCollection(DataSpace.OSpace));

        // Get metadata for given CLR type
        var entityMetadata = metadata
                .GetItems<EntityType>(DataSpace.OSpace)
                .Single(e => objectItemCollection.GetClrType(e) == typeof(TEntity));
        string[] key = entityMetadata.KeyProperties.Select(p => p.Name).ToArray();
        int Id = 0;
        ActionCode = entityMetadata.Name;
        if (key != null)
        {
            var CId = _ctx.Entry(entity).Property(key[0]).CurrentValue;
            Id = int.Parse(CId.ToString());
        }
        return Id;
    }
    public virtual bool Delete(TEntity entity)
        {
        int UserId = SesssionUser.GetCurrentUserId();
        if (UserId <= 0)
            {
                return false;
            }
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
                    _ctx.Entry(entity).CurrentValues["DeletedBy"] = UserId;
                }
        
              
              
                bool ret =_ctx.SaveChanges() > 0;
                SaveLog(entity, "Delete", UserId);
                return ret;
        }
            else
            {
                return false;
            }
           
        }

    public virtual bool DeleteOld(TEntity entity)
    {

        _set.Attach(entity);
        _set.Remove(entity);
       
        return _ctx.SaveChanges() > 0;
    }

    public bool Update(TEntity entity)
    {
        int UserId = SesssionUser.GetCurrentUserId();
        if (UserId <= 0)
        {
            return false;
        }
        _set.Attach(entity);
       

        if (checkProperty(entity, "UpdateOn"))
        {
            _ctx.Entry(entity).CurrentValues["UpdateOn"] = DateTime.Now; 
        }
        if (checkProperty(entity, "UpdateBy"))
        {
            _ctx.Entry(entity).CurrentValues["UpdateBy"] = UserId;
        }


        _ctx.Entry(entity).State = EntityState.Modified;
        if (checkProperty(entity, "CreatedOn"))
        {
            _ctx.Entry(entity).Property("CreatedOn").IsModified = false;
        }

        if (checkProperty(entity, "CreatedBy"))
        {
            _ctx.Entry(entity).Property("CreatedBy").IsModified = false;
        }

       
        bool ret= _ctx.SaveChanges() > 0;
        SaveLog(entity, "Update", UserId);
        return ret;
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
    public virtual bool SaveLog(TEntity entity, string ActionType, int ActionUserId)
    {
        bool ret = false;
        try
        {

            string ActionCode=""; //= entity.GetType().BaseType.Name ;
            int Id = GetKey(entity, ref ActionCode);
            if (Id > 0)
            {
                int result = _ctx.SpLog_DataDML(ActionCode, Id, ActionType, ActionUserId);
                ret = true;

            }
        }
        catch (Exception ex)
        {
            {
                return ret;
            }

        }
        return ret;

    }
}

