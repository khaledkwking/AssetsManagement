using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DAL;
namespace BOL
{
    public class HandoverOrdersManager : Repository<HandoverOrders>
    {

        public HandoverOrdersManager(GPFAssetsEntities ctx) : base(ctx)
        {

        }
        public override bool Delete(HandoverOrders entity)
        {
            return base.Delete(entity);
        }
        //public List <Car> GetStudentByGender(int genderId)
        //{
        //    return GetAll().Where(c => c.genderId == genderId).ToList();
        //}
        public bool update(int id)
        {
            HandoverOrders st = GetById(id);

            return Update(st);
        }
        public List<HandoverOrders> GetCastByName(string UnitSearch)
        {
            //stu => stu.CarNo.ToUpper().Contains(Search_Data.ToUpper()) || stu.CarType.ToUpper().Contains(Search_Data.ToUpper()));

            return GetAll().Where(c => c.Remarks.ToUpper().Contains(UnitSearch.ToUpper())).ToList();
        }
        public List<HandoverOrders> GetNotDelAll()
        {
            UnitOfWork UWork = new UnitOfWork();
            //List<vwEmployees> empList = UWork.EmployeesManager.GetNotDelAll().ToList();
            //List<vwDepartments> DeptList = UWork.DepartmentManager.GetNotDelAll().ToList();
            List<Room_tbl> Room_tblList = UWork.RoomsManager.GetNotDelAll().ToList();
            List<HandoverOrders> List;

            List = GetAll().Where(c => c.IsDeleted == false || c.IsDeleted == null).ToList();


            foreach (var Item in List)
            {
                //long? RoomId=0;
                //int? EmpId=0;
                //int? DeptId=0;
                //if (Item.HandOverOrdersDetails != null)
                //{
                //    if (Item.HandOverOrdersDetails.Count > 0)
                //    {
                //        RoomId = Item.HandOverOrdersDetails.FirstOrDefault().StoreId;
                //        EmpId = Item.HandOverOrdersDetails.FirstOrDefault().EmpId;
                //        DeptId = Item.HandOverOrdersDetails.FirstOrDefault().DeptId;
                //    }
                //}
                //if (EmpId != null && EmpId != 0)
                //{
                //    vwEmployees CurList = new vwEmployees();
                //    CurList = empList.Where(c => c.Id == EmpId).FirstOrDefault();
                //    Item.VmEmployees = CurList;
                //}
                //if (DeptId != null && DeptId != 0)
                //{
                //    vwDepartments CurDeptList = new vwDepartments();
                //    CurDeptList = DeptList.Where(c => c.Id == DeptId).FirstOrDefault();

                //    Item.VmDepartments = CurDeptList;
                //}
                //if (RoomId != null && RoomId != 0)
                //{
                //    Room_tbl CurRoomList = new Room_tbl();
                //    CurRoomList = Room_tblList.Where(c => c.Room_Id == RoomId).FirstOrDefault();
                //    Item.Room_tbl1 = CurRoomList;
                //}

                if (Item.StoreId != null)
                {
                    int StoreId = int.Parse(Item.StoreId.ToString());
                    Room_tbl CurStoreList = new Room_tbl();
                    CurStoreList = Room_tblList.Where(c => c.Room_Id == StoreId).FirstOrDefault();

                    Item.Room_tbl = CurStoreList;
                }

            }
            return List;
            //return GetAll().Where(c => c.IsDeleted == false || c.IsDeleted == null).ToList();
        }

        
        public List<HandoverOrders> GetNotDelAllByUserId(int userId)
        {
            UnitOfWork UWork = new UnitOfWork();

            List<Room_tbl> Inventories = UWork.RoomsManager.GetUserInventories(userId).ToList();
            var listOfRoomId = Inventories.Select(r => r.Room_Id).ToList();

            
            List<Room_tbl> Room_tblList = UWork.RoomsManager.GetNotDelAll().ToList();
            List<HandoverOrders> List;

            List = GetAll().Where(c => c.IsDeleted == false || c.IsDeleted == null).ToList();
            List<HandoverOrders> NewList = List.Where(c => listOfRoomId.Contains(c.StoreId.GetValueOrDefault())).ToList();


            foreach (var Item in NewList)
            {
               
                if (Item.StoreId != null)
                {
                    int StoreId = int.Parse(Item.StoreId.ToString());
                    Room_tbl CurStoreList = new Room_tbl();
                    CurStoreList = Room_tblList.Where(c => c.Room_Id == StoreId).FirstOrDefault();

                    Item.Room_tbl = CurStoreList;
                }

            }
            return NewList;
            //return GetAll().Where(c => c.IsDeleted == false || c.IsDeleted == null).ToList();
        }
       
    }
}