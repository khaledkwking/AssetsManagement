using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DAL;
namespace BOL
{
    public class RoomsManager : Repository<Room_tbl>
    {

        public RoomsManager(GPFAssetsEntities ctx) : base(ctx)
        {

        }

        //public override bool SaveLog(string ActionCode, int Id, string ActionType, int ActionUserId)
        //{
        //    return base.SaveLog(ActionCode, Id, ActionType, ActionUserId);
        //}

        public override bool Delete(Room_tbl entity)
        {
            return base.Delete(entity);
        }
        //public List <Car> GetStudentByGender(int genderId)
        //{
        //    return GetAll().Where(c => c.genderId == genderId).ToList();
        //}
        public bool update(int id, string Name, string address, int genderId)
        {
            Room_tbl st = GetById(id);

            return Update(st);
        }
        public List<Room_tbl> GetCastByUnitName(string UnitSearch)
        {
            //stu => stu.CarNo.ToUpper().Contains(Search_Data.ToUpper()) || stu.CarType.ToUpper().Contains(Search_Data.ToUpper()));
            List<Room_tbl> List = new List<Room_tbl>();
            List = GetNotDelAll();
            List = List.Where(c => c.Room_Name != null).ToList();
            List = List.Where(c => c.Room_Name.ToUpper() == UnitSearch.ToUpper()).ToList();
            return List;
        }

        public List<Room_tbl> GetNotDelAll()
        {
            UnitOfWork unitWork = new UnitOfWork();
            List<Room_tbl> List  = GetAll().Where(c => c.IsDeleted == false || c.IsDeleted == null).ToList();
            
            foreach (var Item in List)
            {
                string EmpNames = "";
                if (Item.Emp_rooms != null)
                {
                    Item.busyRoom = false;
                    foreach (var RoomEmpItem in Item.Emp_rooms)
                    {
                        if (RoomEmpItem.IsDeleted == false || RoomEmpItem.IsDeleted == null)
                        {
                            if (RoomEmpItem.EmpId != null)
                            {
                                if (RoomEmpItem.EmpId != 0)
                                {
                                    List<vwEmployees> EmpsList = unitWork.EmployeesManager.GetEmployeeByEmpId(RoomEmpItem.EmpId.GetValueOrDefault()).ToList();
                                    RoomEmpItem.VmEmployees = EmpsList.FirstOrDefault ();
                                    Item.busyRoom = true;
                                    if (!string.IsNullOrEmpty(RoomEmpItem.VmEmployees.FULL_NAME_AR) && RoomEmpItem.VmEmployees.Deleted_at == null)
                                    {
                                        if (EmpNames == "")
                                        {
                                            EmpNames = RoomEmpItem.VmEmployees.FULL_NAME_AR;
                                        }
                                        else
                                        {
                                            EmpNames = RoomEmpItem.VmEmployees.FULL_NAME_AR + " - " + EmpNames;
                                        }
                                    } 
                                }
                            }
                        }
                       
                    }
                    Item.EmpNames = EmpNames;
                }
               
            }
            return List;
        }
        public List<Room_tbl> GetInventoriesAll()
        {
            return GetNotDelAll().Where(c => c.StoreFlag == true && (c.IsDeleted == false || c.IsDeleted == null)).ToList();
        }
        public List<Room_tbl> GetInventoriesAllNoEmp()
        {
            return GetAll().Where(c => c.StoreFlag == true && (c.IsDeleted == false || c.IsDeleted == null)).ToList();
        }
        public List<Room_tbl> GetOnlyRoomsAll()
        {
            return GetNotDelAll().Where(c => c.StoreFlag == false || c.StoreFlag == null ).ToList();
        }
        public List<Room_tbl> GetUserInventories(int CuruserId)
        {
            UnitOfWork unitWork = new UnitOfWork();
            List<Room_tbl> allRooms = GetNotDelAll().Where(c => c.StoreFlag == true &&(c.IsDeleted == false || c.IsDeleted == null)).ToList();
            //int userId = SesssionUser.GetCurrentUserId();

            List <tbUsersStores> UsersStores = unitWork.UsersStoresManager.GetAll().Where(m => m.UserID  == CuruserId && m.Accessing ==true ).ToList();
          
            var UserInvetories = (from p in allRooms // get Rooms table as p
                           join e in UsersStores // implement join as e in Emp_rooms table
                             on p.Room_Id equals e.StoreID //implement join on rows where p.RoomId == e.RoomId
                           select p).ToList();
            return UserInvetories;
        }

        
    }
}