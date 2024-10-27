using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DAL;

    public class CarStatusManager : Repository <CarStatus>
    {
        public CarStatusManager(CARACCOUNTWebEntities ctx) : base(ctx)
        {

        }

        public override bool Delete(CarStatus entity)
        {
            return base.Delete(entity);
        }
        //public List <Car> GetStudentByGender(int genderId)
        //{
        //    return GetAll().Where(c => c.genderId == genderId).ToList();
        //}
        public bool update(int id, string Name, string address, int genderId)
        {
            CarStatus st = GetById(id);
            //st.StudentName = Name;
            return Update(st);
        }
    }
