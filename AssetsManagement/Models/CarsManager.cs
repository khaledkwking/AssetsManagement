using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DAL;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class CarsManager: Repository <Cars> 
    {

    //[Required(ErrorMessage = "Enter Car No")]
    //[Display(Name = "CarNo")]
    //public string CarNo { get; set; }
    
    //[Column("CarNo")]
    //[Required]
    //public string CarNo { get; set; }

  

    public CarsManager(CARACCOUNTWebEntities ctx):base (ctx)
        {
      
        
        }
    
        public override bool Delete(Cars entity)
        {
            return base.Delete(entity);
        }
        public List<Cars> GetCastByCarNo(string CarNo)
        {
        //stu => stu.CarNo.ToUpper().Contains(Search_Data.ToUpper()) || stu.CarType.ToUpper().Contains(Search_Data.ToUpper()));

        return GetAll().Where(c => c.CarNo.ToUpper().Contains (CarNo.ToUpper()) || c.CarType.ToUpper().Contains (CarNo.ToUpper()) ).ToList();
        }
        public bool update(int id, string Name, string address, int genderId)
            {
                Cars st = GetById(id);
                //st.StudentName = Name;
                return Update(st);
            }
        
}
