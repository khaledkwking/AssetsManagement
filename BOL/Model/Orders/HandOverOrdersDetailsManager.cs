using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DAL;
namespace BOL
{
    public class HandOverOrdersDetailsManager : Repository<HandOverOrdersDetails>
    {

        public HandOverOrdersDetailsManager(GPFAssetsEntities ctx) : base(ctx)
        {

        }
        public override bool Delete(HandOverOrdersDetails entity)
        {
            return base.Delete(entity);
        }
        //public List <Car> GetStudentByGender(int genderId)
        //{
        //    return GetAll().Where(c => c.genderId == genderId).ToList();
        //}
        public bool update(int id)
        {
            HandOverOrdersDetails st = GetById(id);

            return Update(st);
        }
        public List<HandOverOrdersDetails> GetCastByName(string UnitSearch)
        {
            //stu => stu.CarNo.ToUpper().Contains(Search_Data.ToUpper()) || stu.CarType.ToUpper().Contains(Search_Data.ToUpper()));

            return GetNotDelAll().Where(c => c.Item_RFID.ToUpper().Contains(UnitSearch.ToUpper())).ToList();
        }
        public List<HandOverOrdersDetails> GetByOrderId(int OrderId)
        {
            //stu => stu.CarNo.ToUpper().Contains(Search_Data.ToUpper()) || stu.CarType.ToUpper().Contains(Search_Data.ToUpper()));
            List<HandOverOrdersDetails> List;
            List = GetAll().Where(c => c.OverOrderId == OrderId && (c.IsDeleted == false || c.IsDeleted == null)).ToList();
            List = GetAllDetails(List);
            return List;
        }
        public List<HandOverOrdersDetails> GetNotDelAll()
        {

            List<HandOverOrdersDetails> List;
            List = GetAll().Where(c => c.IsDeleted == false || c.IsDeleted == null).ToList();
            List = GetAllDetails(List);


            return List;
        }
        public List<HandOverOrdersDetails> GetAllDetails(List<HandOverOrdersDetails> List)
        {
            UnitOfWork UWork = new UnitOfWork();
            List<vwEmployees> empList = UWork.EmployeesManager.GetNotDelAll().ToList();
            List<vwDepartments> DeptList = UWork.DepartmentManager.GetNotDelAll().ToList();
            List<Room_tbl> Room_tblList = UWork.RoomsManager.GetNotDelAll().ToList();
            foreach (var Item in List)
            {
                if (Item.EmpId != null)
                {
                    int EmpId = int.Parse(Item.EmpId.ToString());
                    vwEmployees CurList = new vwEmployees();
                    CurList = empList.Where(c => c.Id == EmpId).FirstOrDefault();
                    Item.VmEmployees = CurList;
                }
                if (Item.DeptId != null)
                {
                    int DeptId = int.Parse(Item.DeptId.ToString());
                    vwDepartments CurDeptList = new vwDepartments();
                    CurDeptList = DeptList.Where(c => c.Id == DeptId).FirstOrDefault();

                    Item.VmDepartments = CurDeptList;
                }
                if (Item.StoreId != null)
                {
                    int RoomId = int.Parse(Item.StoreId.ToString());
                    Room_tbl CurRoomList = new Room_tbl();
                    CurRoomList = Room_tblList.Where(c => c.Room_Id == RoomId).FirstOrDefault();
                    Item.Room_tbl = CurRoomList;
                }
            }
            return List;
        }
    }
}