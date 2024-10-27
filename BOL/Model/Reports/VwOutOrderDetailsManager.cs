using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using DAL;
namespace BOL
{
    public class VwOutOrderDetailsManager : Repository<vwOutOrderDetails>
    {

        public VwOutOrderDetailsManager(GPFAssetsEntities ctx) : base(ctx)
        {

        }

        //public override bool Delete(Unit_tbl entity)
        //{
        //    return base.Delete(entity);
        //}
        ////public List <Car> GetStudentByGender(int genderId)
        ////{
        ////    return GetAll().Where(c => c.genderId == genderId).ToList();
        ////}
        //public bool update(int id, string Name, string address, int genderId)
        //{
        //    Unit_tbl st = GetById(id);
        //    //st.StudentName = Name;
        //    return Update(st);
        //}
        //public List<vwOutOrderDetails> GetNotDelAll()
        //{
        //    return GetAll().Where(c => c.IsDeleted == false || c.IsDeleted == null).ToList();
        //}
        public List<vwOutOrderDetails> GetNotDelAllByParam(DateTime? FromDate, DateTime? Todate, long? RoomId
            , int? DeptId, int? EmpId, long[] StoresIds, long? ItemId)
        {
            //stu => stu.CarNo.ToUpper().Contains(Search_Data.ToUpper()) || stu.CarType.ToUpper().Contains(Search_Data.ToUpper()));
            List<vwOutOrderDetails> List;
            if (StoresIds != null)
            {
                if (FromDate != null && Todate != null)
                {
                    //DbFunctions.TruncateTime(c.OutOrders.OutOrderDate) >= DbFunctions.TruncateTime(FromDate) && DbFunctions.TruncateTime(c.OutOrders.OutOrderDate) <= DbFunctions.TruncateTime(Todate)
                 List = GetAll().Where(c => (DbFunctions.TruncateTime(c.OutOrderDate) >= DbFunctions.TruncateTime(FromDate) && DbFunctions.TruncateTime(c.OutOrderDate) <= DbFunctions.TruncateTime(Todate))
                 && ((c.RoomId == RoomId || RoomId == null)
                 && (c.EmpId == EmpId || EmpId == null)
                 && (c.DeptId == DeptId || DeptId == null)
                 //&& (c.StoreId == StoreId || StoreId == null)
                 && (StoresIds.Contains(c.StoreId.Value))
                 && (c.ItemId == ItemId || ItemId == null)
                 && (c.IsDeleted == false || c.IsDeleted == null))
                ).ToList();
                }
                else
                {

                    List = GetAll().Where(c => ((c.RoomId == RoomId || RoomId == null)
                    && (c.EmpId == EmpId || EmpId == null)
                    && (c.DeptId == DeptId || DeptId == null)
                    //&& (c.StoreId == StoreId || StoreId == null)
                    && (StoresIds.Contains(c.StoreId.Value))
                    && (c.ItemId == ItemId || ItemId == null)
                    && (c.IsDeleted == false || c.IsDeleted == null))
                   ).ToList();
                }
            }
            else
            {
                List = GetAll().Where(c => (DbFunctions.TruncateTime(c.OutOrderDate) >= DbFunctions.TruncateTime(FromDate) && DbFunctions.TruncateTime(c.OutOrderDate) <= DbFunctions.TruncateTime(Todate))
                 && ((c.RoomId == RoomId || RoomId == null)
                 && (c.EmpId == EmpId || EmpId == null)
                 && (c.DeptId == DeptId || DeptId == null)
                 //&& (c.StoreId == StoreId || StoreId == null)
                 //&& (StoresIds.Contains(c.StoreId.Value))
                 && (c.ItemId == ItemId || ItemId == null)
                 && (c.IsDeleted == false || c.IsDeleted == null))
                ).ToList();
            }

            return GetAllDetails(List);
        }
        public List<vwOutOrderDetails> GetAllDetails(List<vwOutOrderDetails> List)
        {
            UnitOfWork UWork = new UnitOfWork();
            List<vwEmployees> empList = UWork.EmployeesManager.GetNotDelAll().ToList();
            List<vwDepartments> DeptList = UWork.DepartmentManager.GetNotDelAll().ToList();

            foreach (var Item in List)
            {
                if (Item.EmpId != null)
                {
                    int EmpId = int.Parse(Item.EmpId.ToString());
                    vwEmployees CurList = new vwEmployees();
                    CurList = empList.Where(c => c.Id == EmpId).FirstOrDefault();
                    if (CurList != null)
                    {
                        Item.EmpName = CurList.FULL_NAME_AR;
                    }
                    //Item.VmEmployees = CurList;
                }
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
        public List<vwOutOrderDetails> GetNotDelByOrderId(long? OutOrderId)
        {
            //stu => stu.CarNo.ToUpper().Contains(Search_Data.ToUpper()) || stu.CarType.ToUpper().Contains(Search_Data.ToUpper()));
            List<vwOutOrderDetails> List;
            //DbFunctions.TruncateTime(c.OutOrders.OutOrderDate) >= DbFunctions.TruncateTime(FromDate) && DbFunctions.TruncateTime(c.OutOrders.OutOrderDate) <= DbFunctions.TruncateTime(Todate)
            List = GetAll().Where((c =>(c.OutOrderId == OutOrderId)
             && (c.IsDeleted == false || c.IsDeleted == null))
            ).ToList();

            return GetAllDetails(List);
        }

    }
}