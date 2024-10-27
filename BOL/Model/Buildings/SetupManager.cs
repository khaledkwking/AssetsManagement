using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DAL;
namespace BOL
{
    public class SetupManager : Repository<tbl_Setup >
    {

        public SetupManager(GPFAssetsEntities ctx) : base(ctx)
        {

        }

        //public override bool SaveLog(string ActionCode, int Id, string ActionType, int ActionUserId)
        //{
        //    return base.SaveLog(ActionCode, Id, ActionType, ActionUserId);
        //}

        public override bool Delete(tbl_Setup entity)
        {
            return base.Delete(entity);
        }
        //public List <Car> GetStudentByGender(int genderId)
        //{
        //    return GetAll().Where(c => c.genderId == genderId).ToList();
        //}
        public bool update(int id)
        {
            tbl_Setup st = GetById(id);

            return Update(st);
        }
        //public List<Room_tbl> GetCastByUnitName(string UnitSearch)
        //{
        //    //stu => stu.CarNo.ToUpper().Contains(Search_Data.ToUpper()) || stu.CarType.ToUpper().Contains(Search_Data.ToUpper()));

        //    return GetAll().Where(c => c.Room_Name.ToUpper().Contains(UnitSearch.ToUpper()) || c.Room_NameEn.ToUpper().Contains(UnitSearch.ToUpper())).ToList();
        //}

        public List<tbl_Setup> GetNotDelAll()
        {
            List<tbl_Setup> List  = GetAll().Where(c => c.IsDeleted == false || c.IsDeleted == null).ToList();
           
         
            return List;
        }
     
       
      
        
    }
}