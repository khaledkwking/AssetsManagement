using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using DAL;
namespace BOL
{
    [Serializable]
    public class ItemsStockViewModel
    {

        public int Size_Of_Page = 15;
        public int No_Of_Page = 1;
        private UnitOfWork unitWork = new UnitOfWork();
        public ItemsStockViewModel() 
        {
            ItemsStockCheckList = new List<CheckBoxListItem>();
            ToItemsStockCheckList = new List<CheckBoxListItem>();

            Status = unitWork.StatusManager.GetAll().Select(option => new SelectListItem
            {
                Text = option.AName,
                Value = option.Item_StateId.ToString()
            });
        }

        //public PagedList.IPagedList<tbl_ItemsStock> ItemsStock { get; set; }
        public PagedList.IPagedList<tbl_ItemsStock> ItemsStock { get; set; }
        public PagedList.IPagedList<tbl_ItemsStock> ToItemsStock { get; set; }

        public List<CheckBoxListItem> ItemsStockCheckList { get; set; }
        public List<CheckBoxListItem> ToItemsStockCheckList { get; set; }
      

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public tbl_ItemsStock SelectedItem { get; set; }
      
        public string Sorting_Order { get; set; }
        public  string Search_Data { get; set; }
        public string Filter_Value { get; set; }
        public  int? PageNumber { get; set; }
        public int? PageCount { get; set; }
        public long? FromRoomId { get; set; }
        public long? ToRoomId { get; set; }
        public int FromDeptId { get; set; }
        public int ToDeptId { get; set; }

        public int? FromEmpId { get; set; }
        public int? ToEmpId { get; set; }

        

        public string DisplayMode { get; set; }
        public string ToDisplayMode { get; set; }
        [DefaultValue(1)]
        public int ActionType { get; set; }
        public int PostFlag { get; set; }
        public int searchType { get; set; }
        public int searchTypeTo { get; set; }

        public IEnumerable<SelectListItem> Status { get; set; }
        public IEnumerable<SelectListItem> CatMain { get; set; }
        public IEnumerable<SelectListItem> Category { get; set; }
        public IEnumerable<SelectListItem> CatSub { get; set; }
        public IEnumerable<SelectListItem> Items { get; set; }

        public IEnumerable<SelectListItem> FromRooms { get; set; }
        public IEnumerable<SelectListItem> FromEmployees { get; set; }
        public IEnumerable<SelectListItem> FromDepartments { get; set; }

        public IEnumerable<SelectListItem> ToRooms { get; set; }
        public IEnumerable<SelectListItem> ToEmployees { get; set; }
        public IEnumerable<SelectListItem> ToDepartments { get; set; }


        public IEnumerable<SelectListItem> vWVendors { get; set; }
        public IEnumerable<SelectListItem> vwContracts { get; set; }

        public int? VendorId { get; set; }
        public int? ContractId { get; set; }

        public bool? DisolayZero { get; set; }


        public void ItemsPopulateList(int? defaultEmpId, long? defaultRoomId,int ToFromFlag)
        {
            //get all Main Categories
            int CurEmpId = defaultEmpId.GetValueOrDefault();
            if (defaultEmpId != null)
            {
                var allEmployees = unitWork.EmployeesManager.GetNotDelAll().OrderByDescending(m => m.Id).ToList();
                 defaultEmpId = allEmployees.Select(m => m.Id).FirstOrDefault();
                var defaultDeptId = allEmployees.Select(m => m.Department_Id).FirstOrDefault();

                //get all Categories according to defaultCountryId
                List <Emp_rooms> allEmpRooms = unitWork.EmpRoomsManager.GetNotDelAll().Where(m => m.EmpId == defaultEmpId).ToList();
                List <vwDepartments> allDepartments = unitWork.DepartmentManager.GetNotDelAll().Where(m => m.Id == defaultDeptId).ToList();

                List<Room_tbl> allRooms = unitWork.RoomsManager.GetOnlyRoomsAll().ToList();

                var allRoom = (from p in allRooms // get Rooms table as p
                          join e in allEmpRooms // implement join as e in Emp_rooms table
                            on p.Room_Id equals e.RoomId //implement join on rows where p.RoomId == e.RoomId
                          select p).ToList();
              
                var CurRoomId = allEmpRooms.Select(m => m.RoomId).FirstOrDefault();
                var CurDeptId = allDepartments.Select(m => m.Id).FirstOrDefault();

                if (ToFromFlag == 1)
                {
                    if (CurEmpId != 0)
                    {
                        FromRoomId = CurRoomId;
                        FromEmpId = defaultEmpId.GetValueOrDefault();
                        FromDeptId = CurDeptId;
                        
                    }
                    else
                    {
                        FromRoomId = 0;
                        FromEmpId = 0;
                        FromDeptId = 0;
                    }
                    FromEmployees = new SelectList(allEmployees, "Id", "FULL_NAME_AR", defaultEmpId);
                    FromRooms = new SelectList(allRoom, "Room_Id", "Room_Name", CurRoomId);
                    FromDepartments = new SelectList(allDepartments, "Id", "Name", CurDeptId);
                }
                else if (ToFromFlag == 2)
                {
                    if (CurEmpId != 0)
                    {
                        ToRoomId = CurRoomId;
                        ToEmpId = defaultEmpId.GetValueOrDefault();
                        ToDeptId = CurDeptId;
                    }
                    else
                    {
                        ToRoomId = 0;
                        ToEmpId = 0;
                        ToDeptId = 0;
                    }
                    ToEmployees = new SelectList(allEmployees, "Id", "FULL_NAME_AR", defaultEmpId);
                    ToRooms = new SelectList(allRoom, "Room_Id", "Room_Name", CurRoomId);
                    ToDepartments = new SelectList(allDepartments, "Id", "Name", CurDeptId);

                   
                }

            }

            if (defaultRoomId != null)
            {
                var allRooms = unitWork.RoomsManager.GetOnlyRoomsAll().OrderByDescending(m => m.Room_Id).ToList();
                List<Emp_rooms> allEmpRooms = unitWork.EmpRoomsManager.GetNotDelAll().Where(m => m.RoomId  == defaultRoomId).ToList();
                List<vwDepartments> Departments = unitWork.DepartmentManager.GetNotDelAll().ToList();
                List<vwEmployees> Employees = unitWork.EmployeesManager.GetNotDelAll().ToList();

                var allDepartments = (from p in Departments // get Rooms table as p
                               join e in allEmpRooms // implement join as e in Emp_rooms table
                                 on p.Id  equals e.DeptId //implement join on rows where p.RoomId == e.RoomId
                               select p).ToList();

                var CurDepartmentId = allDepartments.Select(m => m.Id).FirstOrDefault();

               
                var allEmployees = (from p in Employees // get Rooms table as p
                                   join e in allEmpRooms // implement join as e in Emp_rooms table
                                        on p.Id equals e.EmpId //implement join on rows where p.RoomId == e.RoomId
                                      select p).ToList();

                var CurEmployeeId = allEmployees.Select(m => m.Id).FirstOrDefault();

                if (ToFromFlag==1)
                {
                    
                    FromEmployees = new SelectList(allEmployees, "Id", "FULL_NAME_AR", CurEmployeeId);
                    FromRooms = new SelectList(allRooms, "Room_Id", "Room_Name", defaultRoomId);
                    FromDepartments = new SelectList(allDepartments, "Id", "Name", CurDepartmentId);
                    FromRoomId = defaultRoomId.GetValueOrDefault();
                    FromEmpId = CurEmployeeId;
                    FromDeptId = CurDepartmentId;
                }
                else if (ToFromFlag==2)
                {
                  
                    ToEmployees = new SelectList(allEmployees, "Id", "FULL_NAME_AR", CurEmployeeId);
                    ToRooms = new SelectList(allRooms, "Room_Id", "Room_Name", defaultRoomId);
                    ToDepartments = new SelectList(allDepartments, "Id", "Name", CurDepartmentId);
                    ToRoomId = defaultRoomId.GetValueOrDefault();
                    ToEmpId = CurEmployeeId;
                    ToDeptId = CurDepartmentId;
                }
            }

       
        }

        public void setDropDrownList(string type, long value,int ToOrFromFlag)
        {
            int CurDepartmentId ;
            int CurEmployeeId ;
            long CurRoomId ;
            //ItemsStructureViewModel model = new ItemsStructureViewModel();
            if (ToOrFromFlag == 1)
            {
                 CurDepartmentId = FromDeptId;
                 CurEmployeeId = FromEmpId.GetValueOrDefault();
                 CurRoomId = FromRoomId.GetValueOrDefault();
            }
            else  
            {
                 CurDepartmentId = ToDeptId;
                 CurEmployeeId = ToEmpId.GetValueOrDefault();
                 CurRoomId = ToRoomId.GetValueOrDefault();
            }
            if (type == "FromEmpId" || type == "ToEmpId") // when select Employees dropdown list
            {
                
                var allEmployees = unitWork.EmployeesManager.GetNotDelAll().OrderByDescending(m => m.Id).ToList();
                List<Emp_rooms> allEmpRooms = unitWork.EmpRoomsManager.GetNotDelAll().Where(m => m.EmpId == value).ToList();
                vwEmployees Emp = unitWork.EmployeesManager.GetNotDelAll().Where(m => m.Id == value).ToList().FirstOrDefault();

                List<Room_tbl> allRooms = unitWork.RoomsManager.GetOnlyRoomsAll().ToList();
                var allRoom = (from p in allRooms // get Rooms table as p
                               join e in allEmpRooms // implement join as e in Emp_rooms table
                                 on p.Room_Id equals e.RoomId //implement join on rows where p.RoomId == e.RoomId
                               select p).ToList();

                List<vwDepartments> allDepartments = unitWork.DepartmentManager.GetNotDelAll().Where(m => m.Id == Emp.Department_Id).ToList();

                if (CurRoomId == null || CurRoomId==0)
                {
                    CurRoomId = allRoom.Select(m => m.Room_Id).FirstOrDefault();
                }
                else if (allRoom.Where(m => m.Room_Id == CurRoomId).FirstOrDefault() == null)
                {
                    CurRoomId = allRoom.Select(m => m.Room_Id).FirstOrDefault();
                }
                CurDepartmentId = allDepartments.Select(m => m.Id).FirstOrDefault();

                if (type == "FromEmpId")
                {
                    FromDeptId = CurDepartmentId;
                    FromEmpId = int.Parse(value.ToString());
                    FromRoomId = CurRoomId;
                    CurEmployeeId = int.Parse(value.ToString()); 
                    FromRooms = new SelectList(allRoom, "Room_Id", "Room_Name", CurRoomId);
                    FromDepartments = new SelectList(allDepartments, "Id", "Name", CurDepartmentId);
                    FromEmployees = new SelectList(allEmployees, "Id", "FULL_NAME_AR", value);
                }
                else if (type == "ToEmpId")
                {
                    ToDeptId = CurDepartmentId;
                    ToEmpId = int.Parse(value.ToString());
                    ToRoomId = CurRoomId;
                    ToRooms = new SelectList(allRoom, "Room_Id", "Room_Name", CurRoomId);
                    ToDepartments = new SelectList(allDepartments, "Id", "Name", CurDepartmentId);
                    ToEmployees = new SelectList(allEmployees, "Id", "FULL_NAME_AR", value);
                }

            }
            else if (type == "FromRoomId" || type == "ToRoomId") // when select Rooms dropdown list
            {
                var allRooms1 = unitWork.RoomsManager.GetOnlyRoomsAll().OrderByDescending(m => m.Room_Id).ToList();

                List<Emp_rooms> allEmpRooms1 = unitWork.EmpRoomsManager.GetNotDelAll().Where(m => m.RoomId == value).ToList();
                List<vwDepartments> Departments = unitWork.DepartmentManager.GetNotDelAll().ToList();
                List<vwEmployees> Employees = unitWork.EmployeesManager.GetNotDelAll().ToList();

                List<vwDepartments> allDepartments1 = (from p in Departments // get Rooms table as p
                                                       join e in allEmpRooms1 // implement join as e in Emp_rooms table
                                                       on p.Id equals e.DeptId //implement join on rows where p.RoomId == e.RoomId
                                                       select p).ToList();




                // select employees in the room
                var allEmployees1 = (from p in Employees // get Rooms table as p
                                     join e in allEmpRooms1 // implement join as e in Emp_rooms table
                                          on p.Id equals e.EmpId //implement join on rows where p.RoomId == e.RoomId
                                     select p).ToList();


                if (DisplayMode == "FromDeptId" && ToOrFromFlag==1)   
                {
                    CurEmployeeId = allEmpRooms1.Where(m => m.DeptId == CurDepartmentId).FirstOrDefault().EmpId.GetValueOrDefault();
                }
                else if (ToDisplayMode == "ToDeptId" && ToOrFromFlag==2)
                {
                    CurEmployeeId = allEmpRooms1.Where(m => m.DeptId == CurDepartmentId).FirstOrDefault().EmpId.GetValueOrDefault();
                }
                else if (DisplayMode == "FromEmpId" && ToOrFromFlag==1)
                {
                    CurDepartmentId = allEmpRooms1.Where(m => m.EmpId == CurEmployeeId).FirstOrDefault().DeptId.GetValueOrDefault();
                }
                else if (ToDisplayMode == "ToEmpId" && ToOrFromFlag==2)
                {
                    CurDepartmentId = allEmpRooms1.Where(m => m.EmpId == CurEmployeeId).FirstOrDefault().DeptId.GetValueOrDefault();
                }
                else
                {
                    CurEmployeeId = allEmployees1.Select(m => m.Id).FirstOrDefault();
                    CurDepartmentId = allDepartments1.Select(m => m.Id).FirstOrDefault();
                }

                if (type == "FromRoomId")
                {
                    FromDeptId = CurDepartmentId;
                    FromEmpId = CurEmployeeId;
                    FromRoomId = value;

                    FromRooms = new SelectList(allRooms1, "Room_Id", "Room_Name", value);
                    FromDepartments = new SelectList(allDepartments1, "Id", "Name", FromDeptId);

                    FromEmployees = new SelectList(allEmployees1, "Id", "FULL_NAME_AR", FromEmpId);

                }
                else if (type == "ToRoomId")
                {
                    ToDeptId = CurDepartmentId;
                    ToEmpId = CurEmployeeId;
                    ToRoomId = value;

                    ToRooms = new SelectList(allRooms1, "Room_Id", "Room_Name", value);
                    ToDepartments = new SelectList(allDepartments1, "Id", "Name", FromDeptId);
                    ToEmployees = new SelectList(allEmployees1, "Id", "FULL_NAME_AR", FromEmpId);

                }
            }

            else if (type == "FromDeptId" || type == "ToDeptId")
            {
                var allEmployees2 = unitWork.EmployeesManager.GetNotDelAll().Where(m => m.Department_Id == value).ToList();
                var CurEmployeeId2 = allEmployees2.Select(m => m.Id).FirstOrDefault();
                if (type == "FromDeptId")
                {
                    FromEmployees = new SelectList(allEmployees2, "Id", "FULL_NAME_AR", CurEmployeeId2);
                }
                else if (type == "ToDeptId")
                {
                    ToEmployees = new SelectList(allEmployees2, "Id", "FULL_NAME_AR", CurEmployeeId2);
                }
            }
           

        }
        
     
    }
}