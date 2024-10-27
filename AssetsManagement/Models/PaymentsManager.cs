using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DAL;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class PaymentsManager : Repository <Payments>
    {
        public PaymentsManager(CARACCOUNTWebEntities ctx) : base(ctx)
        {
        
        }

        public override bool Delete(Payments entity)
        {
            return base.Delete(entity);
        }
        //public List <Car> GetStudentByGender(int genderId)
        //{
        //    return GetAll().Where(c => c.genderId == genderId).ToList();
        //}
        public bool update(int id, string Name, string address, int genderId)
        {
            Payments st = GetById(id);
            //st.StudentName = Name;
            return Update(st);
        }
    public List<Payments> GetCastByModel(string CarNo)
    {
        //stu => stu.CarNo.ToUpper().Contains(Search_Data.ToUpper()) || stu.CarType.ToUpper().Contains(Search_Data.ToUpper()));

        return GetAll().Where(c => c.ItemType.ToUpper().Contains(CarNo.ToUpper()) || c.PaymentId.ToString ().ToUpper().Contains(CarNo.ToUpper())).ToList();
    }
}
