using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DAL;
namespace BOL
{
    public class OutOrdersManager : Repository<OutOrders>
    {

        public OutOrdersManager(GPFAssetsEntities ctx) : base(ctx)
        {

        }
        public override bool Delete(OutOrders entity)
        {
            return base.Delete(entity);
        }

        public OutOrders GetByOrderId(long OrderId)
        {
            UnitOfWork UWork = new UnitOfWork();
            List<vwEmployees> empList = UWork.EmployeesManager.GetNotDelAll().ToList();
            List<vwDepartments> DeptList = UWork.DepartmentManager.GetNotDelAll().ToList();
            OutOrders Item = GetById(OrderId);
            Item.OutOrdersDetails=Item.OutOrdersDetails.Where(c => c.IsDeleted == false || c.IsDeleted == null).ToList();
            Item.ReturnOutOrders = Item.ReturnOutOrders.Where(c => c.IsDeleted == false || c.IsDeleted == null).ToList();

            if (Item != null)
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
            }
            return Item;
        }

        public bool update(int id)
        {
            OutOrders st = GetById(id);

            return Update(st);
        }
        public List<OutOrders> GetCastByName(string UnitSearch)
        {
            //stu => stu.CarNo.ToUpper().Contains(Search_Data.ToUpper()) || stu.CarType.ToUpper().Contains(Search_Data.ToUpper()));

            return GetAll().Where(c => c.Remarks.ToUpper().Contains(UnitSearch.ToUpper())||
            c.OutOrderId.ToString().ToUpper().Contains(UnitSearch.ToUpper())).ToList();
        }
        public List<OutOrders> GetNotDelAll()
        {
            UnitOfWork UWork = new UnitOfWork();
            List<vwEmployees> empList = UWork.EmployeesManager.GetNotDelAll().ToList();
            List<vwDepartments> DeptList = UWork.DepartmentManager.GetNotDelAll().ToList();
            List<OutOrders> List;
            
            List = GetAll().Where(c => c.IsDeleted == false || c.IsDeleted == null).ToList();
            

            foreach (var Item in List)
            {
                if (Item.ReturnOutOrders != null)
                {
                    Item.ReturnOutOrders  = Item.ReturnOutOrders.Where(c => c.IsDeleted == false || c.IsDeleted == null).ToList();
                }
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
            }
            return List;
            //return GetAll().Where(c => c.IsDeleted == false || c.IsDeleted == null).ToList();
        }
        public List<OutOrders> GetNotDelAllByUserId(int userId)
        {
            UnitOfWork UWork = new UnitOfWork();
            List<vwEmployees> empList = UWork.EmployeesManager.GetNotDelAll().ToList();
            List<vwDepartments> DeptList = UWork.DepartmentManager.GetNotDelAll().ToList();
            List<OutOrders> List;
            List<Room_tbl> Inventories = UWork.RoomsManager.GetUserInventories(userId).ToList();
            var listOfRoomId = Inventories.Select(r => r.Room_Id).ToList();

            List = GetAll().Where(c => c.IsDeleted == false || c.IsDeleted == null ).ToList();

            List<OutOrders> NewList = List.Where(c => listOfRoomId.Contains(c.StoreId.GetValueOrDefault())).ToList();
                      

            foreach (var Item in NewList)
            {
                if (Item.ReturnOutOrders != null)
                {
                    Item.ReturnOutOrders = Item.ReturnOutOrders.Where(c => c.IsDeleted == false || c.IsDeleted == null).ToList();
                }
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
            }
            return NewList;
            //return GetAll().Where(c => c.IsDeleted == false || c.IsDeleted == null).ToList();
        }
    }
}