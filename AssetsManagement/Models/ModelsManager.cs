using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DAL;

    public class ModelsManager : Repository <Models>
    {
        public ModelsManager (CARACCOUNTWebEntities ctx) : base(ctx)
        {
       
        }

        public override bool Delete(Models entity)
        {
            return base.Delete(entity);
        }
        //public List <Car> GetStudentByGender(int genderId)
        //{
        //    return GetAll().Where(c => c.genderId == genderId).ToList();
        //}
        public bool update(int id, string Name, string address, int genderId)
        {
                Models st = GetById(id);
            //st.StudentName = Name;
            return Update(st);
        }
    public List<Models> GetCastByModel(string CarNo)
    {
        //stu => stu.CarNo.ToUpper().Contains(Search_Data.ToUpper()) || stu.CarType.ToUpper().Contains(Search_Data.ToUpper()));

        return GetAll().Where(c => c.ModelDesc.ToUpper().Contains(CarNo.ToUpper()) || c.ModelId.ToString ().ToUpper().Contains(CarNo.ToUpper())).ToList();
    }
}
