using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DAL;
namespace BOL
{
    public class CatMainManager : Repository<CatMain_tbl>
    {

        public CatMainManager(GPFAssetsEntities ctx) : base(ctx)
        {

        }

        public override bool Delete(CatMain_tbl entity)
        {
            return base.Delete(entity);
        }
        //public List <Car> GetStudentByGender(int genderId)
        //{
        //    return GetAll().Where(c => c.genderId == genderId).ToList();
        //}
        public bool update(int id, string Name, string address, int genderId)
        {
            CatMain_tbl st = GetById(id);
            //st.StudentName = Name;
            return Update(st);
        }
        public List<CatMain_tbl> GetCastByUnitName(string UnitSearch)
        {
            //stu => stu.CarNo.ToUpper().Contains(Search_Data.ToUpper()) || stu.CarType.ToUpper().Contains(Search_Data.ToUpper()));

            return GetAll().Where(c => c.CatMain_Name.ToUpper().Contains(UnitSearch.ToUpper()) || c.CatMain_NameEn.ToUpper().Contains(UnitSearch.ToUpper())).ToList();
        }
        public List<CatMain_tbl> GetNotDelAll()
        {
            return GetAll().Where(c => c.IsDeleted == false || c.IsDeleted == null).ToList();
        }
        public List<CatMain_tbl> GetUserMainCats(int CuruserId)
        {
            UnitOfWork unitWork = new UnitOfWork();
            List<CatMain_tbl> allMainCate = GetNotDelAll().Where(c=> c.IsDeleted == false || c.IsDeleted == null).ToList();
            //int userId = SesssionUser.GetCurrentUserId();

            List<tbUsersMainCats> UsersMainCats = unitWork.UsersMainCatsManager.GetAll().Where(m => m.UserID == CuruserId && m.Accessing == true).ToList();

            var UserMainCats = (from p in allMainCate // get Rooms table as p
                                  join e in UsersMainCats // implement join as e in Emp_rooms table
                                    on p.CatMain_Id equals e.CatMain_Id //implement join on rows where p.RoomId == e.RoomId
                                  select p).ToList();
            return UserMainCats;
        }

    }
}