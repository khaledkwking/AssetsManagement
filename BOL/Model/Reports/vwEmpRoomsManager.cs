using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using DAL;
namespace BOL
{
    public class vwEmpRoomsManager : Repository<vwEmpRooms>
    {

        public vwEmpRoomsManager(GPFAssetsEntities ctx) : base(ctx)
        {

        }

        
        public List<vwEmpRooms> GetNotDelAllByParam(int? BulidingId, int? FloorId, int [] RoomTypesId)
        {
            //stu => stu.CarNo.ToUpper().Contains(Search_Data.ToUpper()) || stu.CarType.ToUpper().Contains(Search_Data.ToUpper()));
            List<vwEmpRooms> List;
            //DbFunctions.TruncateTime(c.OutOrders.OutOrderDate) >= DbFunctions.TruncateTime(FromDate) && DbFunctions.TruncateTime(c.OutOrders.OutOrderDate) <= DbFunctions.TruncateTime(Todate)
            if (RoomTypesId != null)
            {
                List = GetAll().Where(c => (c.Building_Id == BulidingId || BulidingId == null)
                     && (c.Floor_Id == FloorId || FloorId == null)
                      && (RoomTypesId.Contains(c.RoomTypeId.Value))
                    
                     && (c.StoreFlag == false)
                      && (c.EmpId == null)).ToList();
            }
            else
            {
                List = GetAll().Where(c => (c.Building_Id == BulidingId || BulidingId == null)
                     && (c.Floor_Id == FloorId || FloorId == null)
                     && (c.StoreFlag == false)
                      && (c.EmpId == null)).ToList();
            }
            //return List;
            return GetAllDetails(List);
        }
       
       
        public List<vwEmpRooms> GetAllDetails(List<vwEmpRooms> List)
        {
            UnitOfWork UWork = new UnitOfWork();
            List<vwEmployees> empList = UWork.EmployeesManager.GetNotDelAll().ToList();
            List<vwDepartments> DeptList = UWork.DepartmentManager.GetNotDelAll().ToList();

            foreach (var Item in List)
            {
                //if (Item.EmpId != null)
                //{
                //    int EmpId = int.Parse(Item.EmpId.ToString());
                //    vwEmployees CurList = new vwEmployees();
                //    CurList = empList.Where(c => c.Id == EmpId).FirstOrDefault();
                //    if (CurList != null)
                //    {
                //        Item.EmpName = CurList.FULL_NAME_AR;
                //    }
                //    //Item.VmEmployees = CurList;
                //}
                if (Item.DeptId != null)
                {
                    int DeptId = int.Parse(Item.DeptId.ToString());
                    vwDepartments CurDeptList = new vwDepartments();
                    CurDeptList = DeptList.Where(c => c.Id == DeptId).FirstOrDefault();
                    if (CurDeptList != null)
                    {
                        Item.DeptName = CurDeptList.Name;
                    }
                    //Item.VmDepartments = CurDeptList;
                }

            }
            return List;
        }

    }
}