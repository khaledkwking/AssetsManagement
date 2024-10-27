using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DAL;

public class OfficesManager : Repository<Offices> 

    {
      
        public OfficesManager(GPFAssetsEntities ctx) : base(ctx)
        {


        }

        
        public override bool Delete(Offices entity)
        {
            return base.Delete(entity);
        }
        //public List<Car> GetCastByCarNo(string CarNo)
        //{
        //    //stu => stu.CarNo.ToUpper().Contains(Search_Data.ToUpper()) || stu.CarType.ToUpper().Contains(Search_Data.ToUpper()));

        //    return GetAll().Where(c => c.CarNo.ToUpper().Contains(CarNo.ToUpper()) || c.CarType.ToUpper().Contains(CarNo.ToUpper())).ToList();
        //}
        //public bool update(int id, string Name, string address, int genderId)
        //{
        //    Car st = GetById(id);
        //    //st.StudentName = Name;
        //    return Update(st);
        //}

    }
