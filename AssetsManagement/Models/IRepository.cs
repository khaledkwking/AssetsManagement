using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


    public interface IRepository <TEntity>
    {
        List <TEntity> GetAllBind();
        IQueryable <TEntity> GetAll();
        TEntity  GetById(params object[] Id);
        TEntity Add(TEntity entity);
        bool Update(TEntity entity);
        bool Delete(TEntity entity);

    }
