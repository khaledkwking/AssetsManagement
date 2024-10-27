using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using DAL;
namespace BOL
{
    public class vwTransferAssestsManager : Repository<vwTransferAssests>
    {

        public vwTransferAssestsManager(GPFAssetsEntities ctx) : base(ctx)
        {

        }

        
        public List<vwTransferAssests> GetNotDelAllByParam(DateTime? FromDate, DateTime? Todate
            , long? RoomId,int? EmpId,int? DeptId)
        {
            //stu => stu.CarNo.ToUpper().Contains(Search_Data.ToUpper()) || stu.CarType.ToUpper().Contains(Search_Data.ToUpper()));
            List<vwTransferAssests> List=null;
            if (FromDate != null && Todate != null)
            {
                //DbFunctions.TruncateTime(c.OutOrders.OutOrderDate) >= DbFunctions.TruncateTime(FromDate) && DbFunctions.TruncateTime(c.OutOrders.OutOrderDate) <= DbFunctions.TruncateTime(Todate)
                List = GetAll().Where(c => (DbFunctions.TruncateTime(c.TransDate ) >= DbFunctions.TruncateTime(FromDate) && DbFunctions.TruncateTime(c.TransDate ) <= DbFunctions.TruncateTime(Todate))
             && ((c.TransRoomId_From   == RoomId || RoomId == null)
          
                 && (c.TransDeptId_From   == DeptId || DeptId == null)
                  && (c.TransEmpId_From  == EmpId || EmpId == null))

            ).ToList();
            }
            else
            {
                List = GetAll().Where(c=>(c.TransRoomId_From == RoomId || RoomId == null)
                && (c.TransDeptId_From == DeptId || DeptId == null)
                 && (c.TransEmpId_From == EmpId || EmpId == null)                          

            ).ToList();
            }

            //return List;
            return GetAllDetails(List);
        }

        public List<vwTransferAssests> GetNotDelAllById(long? TansId)
        {
            //stu => stu.CarNo.ToUpper().Contains(Search_Data.ToUpper()) || stu.CarType.ToUpper().Contains(Search_Data.ToUpper()));
            List<vwTransferAssests> List = null;
           
                //DbFunctions.TruncateTime(c.OutOrders.OutOrderDate) >= DbFunctions.TruncateTime(FromDate) && DbFunctions.TruncateTime(c.OutOrders.OutOrderDate) <= DbFunctions.TruncateTime(Todate)
          List = GetAll().Where(c => c.TansId  == TansId).ToList();
                      
            

            //return List;
            return GetAllDetails(List);
        }

        public List<vwTransferAssests> GetAllDetails(List<vwTransferAssests> List)
        {
            UnitOfWork UWork = new UnitOfWork();
            List<vwEmployees> empList = UWork.EmployeesManager.GetNotDelAll().ToList();
            List<vwDepartments> DeptList = UWork.DepartmentManager.GetNotDelAll().ToList();
            vwEmployees CurList = new vwEmployees();
            vwDepartments CurDeptList = new vwDepartments();
            foreach (var Item in List)
            {
                if (Item.TransEmpId_From != null)
                {
                    int EmpId = int.Parse(Item.TransEmpId_From.ToString());
                   
                    CurList = empList.Where(c => c.Id == EmpId).FirstOrDefault();
                    if (CurList != null)
                    {
                        Item.EmpFromName = CurList.FULL_NAME_AR;
                    }
                   
                }
                if (Item.TransEmpId_To != null)
                {
                    int EmpId = int.Parse(Item.TransEmpId_To.ToString());

                    CurList = empList.Where(c => c.Id == EmpId).FirstOrDefault();
                    if (CurList != null)
                    {
                        Item.EmpToName = CurList.FULL_NAME_AR;
                    }

                }
                if (Item.TransDeptId_From != null)
                {
                    int DeptId = int.Parse(Item.TransDeptId_From.ToString());
                 
                    CurDeptList = DeptList.Where(c => c.Id == DeptId).FirstOrDefault();
                    if (CurDeptList != null)
                    {
                        Item.DeptFromName = CurDeptList.Name;
                    }
                    
                }
                if (Item.TransDeptId_To != null)
                {
                    int DeptId = int.Parse(Item.TransDeptId_To .ToString());

                    CurDeptList = DeptList.Where(c => c.Id == DeptId).FirstOrDefault();
                    if (CurDeptList != null)
                    {
                        Item.DeptToName = CurDeptList.Name;
                    }

                }
            }
            return List;
        }

    }
}