using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DAL;
namespace BOL
{
    public class DestroyOrdersDetailsManager : Repository<DestroyOrdersDetails>
    {
        public const int DamagedItem = 3;
        public DestroyOrdersDetailsManager(GPFAssetsEntities ctx) : base(ctx)
        {

        }
        public override bool Delete(DestroyOrdersDetails entity)
        {
            return base.Delete(entity);
        }
        //public List <Car> GetStudentByGender(int genderId)
        //{
        //    return GetAll().Where(c => c.genderId == genderId).ToList();
        //}
        public bool update(int id)
        {
            DestroyOrdersDetails st = GetById(id);

            return Update(st);
        }
        public List<DestroyOrdersDetails> GetCastByName(string UnitSearch)
        {
            //stu => stu.CarNo.ToUpper().Contains(Search_Data.ToUpper()) || stu.CarType.ToUpper().Contains(Search_Data.ToUpper()));

            return GetNotDelAll().Where(c => c.Item_RFID.ToUpper().Contains(UnitSearch.ToUpper())).ToList();
        }
        public List<DestroyOrdersDetails> GetByOrderId(int DestroyOrderId)
        {
            //stu => stu.CarNo.ToUpper().Contains(Search_Data.ToUpper()) || stu.CarType.ToUpper().Contains(Search_Data.ToUpper()));

            return GetNotDelAll().Where(c => c.DestroyOrderId == DestroyOrderId).ToList();

        }
        public List<DestroyOrdersDetails> GetNotDelAll()
        {
            return GetAll().Where(c => c.IsDeleted == false || c.IsDeleted == null).ToList();
        }
        public List<DestroyOrdersDetails> GetNotDelAllAndDamaged(string UnitSearch)
        {
            UnitOfWork UWork = new UnitOfWork();
            List<vwEmployees> empList = UWork.EmployeesManager.GetNotDelAll().ToList();
            List<vwDepartments> DeptList = UWork.DepartmentManager.GetNotDelAll().ToList();
            List<DestroyOrdersDetails> List;
            if (!String.IsNullOrEmpty(UnitSearch))
            {
                List = GetAll().Where(c => (c.Item_BarCode.ToUpper().Contains(UnitSearch.ToUpper()) || c.Item_RFID.ToUpper().Contains(UnitSearch.ToUpper())

            || c.tbl_ItemsStock.Item_tbl.Item_Name.ToUpper().Contains(UnitSearch.ToUpper())) && ( c.IsDeleted == false || c.IsDeleted == null)
                 ).ToList();
            }
            else
            {
                List = GetAll().Where(c => c.IsDeleted == false || c.IsDeleted == null).ToList();
            }

            //List = GetAll().Where(c => c.IsDeleted == true && c.Item_StateId == DamagedItem).ToList();



            return List;
        }
    }
}