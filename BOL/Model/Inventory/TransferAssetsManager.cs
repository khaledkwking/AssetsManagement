using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using DAL;
namespace BOL
{
    public class TransferAssetsManager : Repository<TransferAssets>
    {

        public const int DamagedItem = 3;
        public TransferAssetsManager(GPFAssetsEntities ctx) : base(ctx)
        {

        }

        public override bool Delete(TransferAssets entity)
        {
            return base.Delete(entity);
        }
       
        public bool update(int id)
        {
            TransferAssets st = GetById(id);
            //st.StudentName = Name;
            return Update(st);
        }
        public bool updateDept_Room_Employee(long stockid, int DeptId, long RoomId, int? EmpId)
        {

            TransferAssets st = GetById(stockid);
            //st.Depart_Id = DeptId;
            //st.Room_Id = RoomId;
            //st.Emp_Id = EmpId;
            //st.Item_AssDate = DateTime.Today;

            //st.StudentName = Name;
            return Update(st);

        }
        public List<TransferAssets> GetCastByName(string UnitSearch)
        {
            //stu => stu.CarNo.ToUpper().Contains(Search_Data.ToUpper()) || stu.CarType.ToUpper().Contains(Search_Data.ToUpper()));
            List<TransferAssets> List;
            List = GetAll().Where(c => (c.TransDate.ToString().ToUpper().Contains(UnitSearch.ToUpper()))
             && (c.IsDeleted == false || c.IsDeleted == null)).ToList();
            return GetDetials(List);
        }



        public List<TransferAssets> GetNotDelAll()
        {
            UnitOfWork UWork = new UnitOfWork();
            List<vwEmployees> empList = UWork.EmployeesManager.GetNotDelAll().ToList();
            List<vwDepartments> DeptList = UWork.DepartmentManager.GetNotDelAll().ToList();
          
            List<TransferAssets> List;

            List = GetAll().Where(c => c.IsDeleted == false || c.IsDeleted == null).ToList();

            List = GetDetials(List);

            return List;
        }

        public List<TransferAssets> GetNotDelAllByParam(DateTime? FromDate, DateTime? Todate, string UnitSearch)
        {
            UnitOfWork UWork = new UnitOfWork();
            List<vwEmployees> empList = UWork.EmployeesManager.GetNotDelAll().ToList();
            List<vwDepartments> DeptList = UWork.DepartmentManager.GetNotDelAll().ToList();
            List<TransferAssets> List;

            if (FromDate != null && Todate != null)
            {
                //DbFunctions.TruncateTime(c.OutOrders.OutOrderDate) >= DbFunctions.TruncateTime(FromDate) && DbFunctions.TruncateTime(c.OutOrders.OutOrderDate) <= DbFunctions.TruncateTime(Todate)
                List = GetAll().Where(c => (DbFunctions.TruncateTime(c.TransDate) >= DbFunctions.TruncateTime(FromDate) && DbFunctions.TruncateTime(c.TransDate) <= DbFunctions.TruncateTime(Todate))
                && (c.IsDeleted == false || c.IsDeleted == null)).ToList();

            }
            else
            {
                List = GetAll().Where(c=>c.IsDeleted == false || c.IsDeleted == null).ToList();

            }

            List = GetDetials(List);

            return List;
        }
       

        public List<TransferAssets> GetDetials(List<TransferAssets> List)
        {
            UnitOfWork UWork = new UnitOfWork();
            List<vwEmployees> empList = UWork.EmployeesManager.GetNotDelAll().ToList();
            List<vwDepartments> DeptList = UWork.DepartmentManager.GetNotDelAll().ToList();
            List<Room_tbl> RoomList = UWork.RoomsManager.GetNotDelAll().ToList();
             List<tbUsers> UsersList = UWork.UsersManager.GetAll().ToList();

            vwEmployees CurList = new vwEmployees();
            vwDepartments CurDeptList = new vwDepartments();
            Room_tbl CurRoomList = new Room_tbl();
            foreach (var Item in List)
            {
                if (Item.TransEmpId_From != null)
                {
                    int EmpId = int.Parse(Item.TransEmpId_From.ToString()); 
                    CurList = empList.Where(c => c.Id == EmpId).FirstOrDefault();
                    if (CurList != null)
                    {
                        Item.EmpNameFrom = CurList.FULL_NAME_AR;
                    }
                }
                if (Item.TransEmpId_To  != null)
                {
                    int EmpId = int.Parse(Item.TransEmpId_To.ToString());
                    CurList = empList.Where(c => c.Id == EmpId).FirstOrDefault();
                    if (CurList != null)
                    {
                        Item.EmpNameTo = CurList.FULL_NAME_AR;
                    }
                }
                if (Item.TransDeptId_From != null)
                {
                    int DeptId = int.Parse(Item.TransDeptId_From.ToString());
                    CurDeptList = DeptList.Where(c => c.Id == DeptId).FirstOrDefault();
                    if (CurDeptList != null)
                    {
                        Item.DeptNameFrom = CurDeptList.Name;
                    }
                }
                if (Item.TransDeptId_To != null)
                {
                    int DeptId = int.Parse(Item.TransDeptId_To.ToString());
                    CurDeptList = DeptList.Where(c => c.Id == DeptId).FirstOrDefault();
                    if (CurDeptList != null)
                    {
                        Item.DeptNameTo = CurDeptList.Name;
                    }
                }
                if (Item.TransRoomId_From  != null)
                {
                    int RoomId = int.Parse(Item.TransRoomId_From.ToString());
                    CurRoomList = RoomList.Where(c => c.Room_Id  == RoomId).FirstOrDefault();
                    if (CurRoomList != null)
                    {
                        Item.RoomNameFrom = CurRoomList.Room_Name;
                    }
                }
                if (Item.TransRoomId_To != null)
                {
                    int RoomId = int.Parse(Item.TransRoomId_To.ToString());
                    CurRoomList = RoomList.Where(c => c.Room_Id == RoomId).FirstOrDefault();
                    Item.RoomNameTo = CurRoomList.Room_Name;
                }
                if (Item.CreatedBy != null)
                {
                    int userId = Item.CreatedBy.GetValueOrDefault();
                    tbUsers CurUser = UsersList.Where(c => c.UserID == userId).FirstOrDefault();
                    if (CurUser != null)
                    {
                        Item.userName = CurUser.FullName;
                    }
                }
            }
            return List;
        }
              
       
    }
}