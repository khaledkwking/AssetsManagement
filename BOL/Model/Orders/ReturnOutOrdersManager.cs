using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DAL;
namespace BOL
{
    public class ReturnOutOrdersManager : Repository<ReturnOutOrders>
    {

        public ReturnOutOrdersManager(GPFAssetsEntities ctx) : base(ctx)
        {

        }
        public override bool Delete(ReturnOutOrders entity)
        {
            return base.Delete(entity);
        }
        //public List <Car> GetStudentByGender(int genderId)
        //{
        //    return GetAll().Where(c => c.genderId == genderId).ToList();
        //}
        public bool update(int id)
        {
            ReturnOutOrders st = GetById(id);

            return Update(st);
        }
        public List<ReturnOutOrders> GetCastByName(string UnitSearch)
        {
            //stu => stu.CarNo.ToUpper().Contains(Search_Data.ToUpper()) || stu.CarType.ToUpper().Contains(Search_Data.ToUpper()));

            return GetAll().Where(c => c.Remarks.ToUpper().Contains(UnitSearch.ToUpper())).ToList();
        }
        public List<ReturnOutOrders> GetNotDelAll()
        {
            UnitOfWork UWork = new UnitOfWork();
            List<Room_tbl> InventoriesList = UWork.RoomsManager.GetInventoriesAll().ToList();

            List<ReturnOutOrders> List;

            List = GetAll().Where(c => c.IsDeleted == false || c.IsDeleted == null).ToList();


            //foreach (var Item in List)
            //{
            //    if (Item.StoreId_From != null)
            //    {
            //        long StoreId_From = Item.StoreId_From.GetValueOrDefault();
            //        Room_tbl CurList = new Room_tbl();
            //        CurList = InventoriesList.Where(c => c.Room_Id == StoreId_From).FirstOrDefault();
            //        Item.FromStores = CurList;
            //    }
            //    //if (Item.StoreId_To != null)
            //    //{
            //    //    long StoreId_To = Item.StoreId_To.GetValueOrDefault();
            //    //    Room_tbl CurRoomList = new Room_tbl();
            //    //    CurRoomList = InventoriesList.Where(c => c.Room_Id == StoreId_To).FirstOrDefault();
            //    //    Item.ToStore = CurRoomList;
            //    //}
            //}
            return List;
            //return GetAll().Where(c => c.IsDeleted == false || c.IsDeleted == null).ToList();
        }
        public List<ReturnOutOrders> GetNotDelAllByOrderId(long OrderId)
        {
            UnitOfWork UWork = new UnitOfWork();
            List<vwEmployees> empList = UWork.EmployeesManager.GetNotDelAll().ToList();
            List<vwDepartments> DeptList = UWork.DepartmentManager.GetNotDelAll().ToList();

            List<Room_tbl> InventoriesList = UWork.RoomsManager.GetInventoriesAll().ToList();


            List<ReturnOutOrders> List;

            List = GetAll().Where(c => (c.IsDeleted == false || c.IsDeleted == null) && c.OutOrderId == OrderId).ToList();


            foreach (var Item in List)
            {
                if (Item.OutOrderId != null)
                {
                    long OutOrderId = int.Parse(Item.OutOrderId.ToString());
                    OutOrders OutOrdersList = UWork.OutOrdersManager.GetByOrderId(OutOrderId);
                    Item.OutOrders = OutOrdersList;

                }
                //if (Item.OutOrders.DeptId != null)
                //{
                //    int DeptId = int.Parse(Item.OutOrders.DeptId.ToString());
                //    vwDepartments CurDeptList = new vwDepartments();
                //    CurDeptList = DeptList.Where(c => c.Id == DeptId).FirstOrDefault();

                //    Item.OutOrders.VmDepartments = CurDeptList;
                //}
            }
            return List;
            //return GetAll().Where(c => c.IsDeleted == false || c.IsDeleted == null).ToList();
        }
    }
}