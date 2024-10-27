using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using DAL;
namespace BOL
{
    public class TransferAssetsDetailsManager : Repository<TransferAssetsDetails>
    {

        public const int DamagedItem = 3;
        public TransferAssetsDetailsManager(GPFAssetsEntities ctx) : base(ctx)
        {

        }

        public override bool Delete(TransferAssetsDetails entity)
        {
            return base.Delete(entity);
        }
       
        public bool update(int id)
        {
            TransferAssetsDetails st = GetById(id);
            //st.StudentName = Name;
            return Update(st);
        }
        public bool updateDept_Room_Employee(long stockid, int DeptId, long RoomId, int? EmpId)
        {

            TransferAssetsDetails st = GetById(stockid);
            //st.Depart_Id = DeptId;
            //st.Room_Id = RoomId;
            //st.Emp_Id = EmpId;
            //st.Item_AssDate = DateTime.Today;
            //st.StudentName = Name;
            return Update(st);

        }
        //public List<TransferAssets> GetCastByUnitName(string UnitSearch, long? RoomId)
        //{
        //    //stu => stu.CarNo.ToUpper().Contains(Search_Data.ToUpper()) || stu.CarType.ToUpper().Contains(Search_Data.ToUpper()));
        //    List<tbl_ItemsStock> List;
        //    List= GetAll().Where(c => (c.Item_BarCode.ToUpper().Contains(UnitSearch.ToUpper()) || c.Item_RFID.ToUpper().Contains(UnitSearch.ToUpper())
        //    || c.Item_tbl.Item_Name.ToUpper().Contains(UnitSearch.ToUpper())
        //    || c.Item_Serial.ToUpper().Contains(UnitSearch.ToUpper())) 
        //    && (c.IsDeleted == false || c.IsDeleted == null)
        //    && (c.Room_Id == RoomId || RoomId == null)

        //    ).ToList();
        //    return GetDetials(List);
        //}

         
      
        public List<TransferAssetsDetails> GetNotDelAll()
        {
            UnitOfWork UWork = new UnitOfWork();
            List<vwEmployees> empList = UWork.EmployeesManager.GetNotDelAll().ToList();
            List<vwDepartments> DeptList = UWork.DepartmentManager.GetNotDelAll().ToList();
            List<TransferAssetsDetails> List;

            List = GetAll().Where(c => c.IsDeleted == false || c.IsDeleted == null).ToList();

            return List;
        }
   
     
       
    }
}