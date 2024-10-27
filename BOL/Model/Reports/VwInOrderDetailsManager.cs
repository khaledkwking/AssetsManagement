using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using DAL;
namespace BOL
{
    public class VwInOrderDetailsManager : Repository<vwInOrderDetails>
    {

        public VwInOrderDetailsManager(GPFAssetsEntities ctx) : base(ctx)
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
        //public List<vwInOrderDetails> GetNotDelAll()
        //{
        //    return GetAll().Where(c => c.IsDeleted == false || c.IsDeleted == null).ToList();
        //}
        public List<vwInOrderDetails> GetNotDelAllByParam(DateTime? FromDate, DateTime? Todate, long? SupplierId
            , long? StoreId, long? ItemId)
        {
            //stu => stu.CarNo.ToUpper().Contains(Search_Data.ToUpper()) || stu.CarType.ToUpper().Contains(Search_Data.ToUpper()));
            List<vwInOrderDetails> List;
            //DbFunctions.TruncateTime(c.OutOrders.OutOrderDate) >= DbFunctions.TruncateTime(FromDate) && DbFunctions.TruncateTime(c.OutOrders.OutOrderDate) <= DbFunctions.TruncateTime(Todate)
            if (FromDate != null && Todate != null)
            {
                List = GetAll().Where(c => (DbFunctions.TruncateTime(c.InOrderDate) >= DbFunctions.TruncateTime(FromDate) && DbFunctions.TruncateTime(c.InOrderDate) <= DbFunctions.TruncateTime(Todate))
                  && ((c.SupplierId_From == SupplierId || SupplierId == null)
                  && (c.StoreId_To == StoreId || StoreId == null)
                  && (c.ItemId == ItemId || ItemId == null)
                  && (c.IsDeleted == false || c.IsDeleted == null))
                 ).ToList();
            }
            else
            {
                List = GetAll().Where(c=>(c.SupplierId_From == SupplierId || SupplierId == null)
                  && ((c.StoreId_To == StoreId || StoreId == null)
                  && (c.ItemId == ItemId || ItemId == null)
                  && (c.IsDeleted == false || c.IsDeleted == null))).ToList();
            }
            return List;
            //return GetAllDetails(List);
        }
        //public List<vwInOrderDetails> GetAllDetails(List<vwInOrderDetails> List)
        //{
        //    UnitOfWork UWork = new UnitOfWork();
        //    List<vwEmployees> empList = UWork.EmployeesManager.GetNotDelAll().ToList();
        //    List<vwDepartments> DeptList = UWork.DepartmentManager.GetNotDelAll().ToList();

        //    foreach (var Item in List)
        //    {
        //        if (Item.EmpId != null)
        //        {
        //            int EmpId = int.Parse(Item.EmpId.ToString());
        //            vwEmployees CurList = new vwEmployees();
        //            CurList = empList.Where(c => c.Id == EmpId).FirstOrDefault();
        //            Item.EmpName = CurList.FULL_NAME_AR;
        //            //Item.VmEmployees = CurList;
        //        }
        //        if (Item.DeptId != null)
        //        {
        //            int DeptId = int.Parse(Item.DeptId.ToString());
        //            vwDepartments CurDeptList = new vwDepartments();
        //            CurDeptList = DeptList.Where(c => c.Id == DeptId).FirstOrDefault();
        //            Item.DeptName = CurDeptList.Name;
        //            //Item.VmDepartments = CurDeptList;
        //        }

        //    }
        //    return List;
        //}
        public List<vwInOrderDetails> GetNotDelByOrderId(long? InOrderDetId)
        {
            //stu => stu.CarNo.ToUpper().Contains(Search_Data.ToUpper()) || stu.CarType.ToUpper().Contains(Search_Data.ToUpper()));
            List<vwInOrderDetails> List;
            //DbFunctions.TruncateTime(c.OutOrders.OutOrderDate) >= DbFunctions.TruncateTime(FromDate) && DbFunctions.TruncateTime(c.OutOrders.OutOrderDate) <= DbFunctions.TruncateTime(Todate)
            List = GetAll().Where((c =>(c.InOrderDetId == InOrderDetId)
             && (c.IsDeleted == false || c.IsDeleted == null))
            ).ToList();
            return List;
            //return GetAllDetails(List);
        }

    }
}