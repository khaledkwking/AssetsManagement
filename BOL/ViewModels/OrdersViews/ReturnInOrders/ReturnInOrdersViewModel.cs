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
    public class ReturnInOrdersViewModel
    {
        public ReturnInOrdersViewModel()
        {
            ItemsScanCheckList = new List<CheckBoxListItem>();
            ReturnInOrdersDetails = new List<ReturnInOrdersDetails>();
        }
        public int Size_Of_Page = 15;
        public int No_Of_Page = 1;
        private UnitOfWork unitWork = new UnitOfWork();

        public PagedList.IPagedList<ReturnInOrders> ReturnInOrders { get; set; }
        public List<ReturnInOrdersDetails> ReturnInOrdersDetails { get; set; }
        public List<tbl_ItemsStock> ScanItems { get; set; }

        //public InOrders InOrders { get; set; }

        public ReturnInOrders SelectedItem { get; set; }
        public string DisplayMode { get; set; }
        public string Sorting_Order { get; set; }
        public string Search_Data { get; set; }
        public string Filter_Value { get; set; }
        public int? PageNumber { get; set; }
        public int? PageCount { get; set; }


        [DefaultValue(1)]
        public int PostFlag { get; set; }
        public int searchType { get; set; }

        [DefaultValue(1)]
        public int ReaderType { get; set; }
        public string Barcode { get; set; }

        public string EditMode { get; set; }
        public IEnumerable<SelectListItem> FromStores { get; set; }
        public IEnumerable<SelectListItem> ToStores { get; set; }
        public IEnumerable<SelectListItem> Suppliers { get; set; }
        public IEnumerable<SelectListItem> Items { get; set; }
        public List<CheckBoxListItem> ItemsScanCheckList { get; set; }
        public long? FromStoreId { get; set; }
        public int ToStoreId { get; set; }
        public int? SupplierId { get; set; }
        public string IPAddress { get; set; }
        public bool TcpFlag { get; set; }
        //public void ItemsPopulateList(int? defaultEmpId, long? defaultRoomId, int ToFromFlag)
        //{
        //    //get all Main Categories
        //    if (defaultEmpId != null)
        //    {
        //        var allEmployees = unitWork.EmployeesManager.GetNotDelAll().OrderByDescending(m => m.Id).ToList();
        //        defaultEmpId = allEmployees.Select(m => m.Id).FirstOrDefault();
        //        var defaultDeptId = allEmployees.Select(m => m.Department_Id).FirstOrDefault();

        //        //get all Categories according to defaultCountryId
        //        List<Emp_rooms> allEmpRooms = unitWork.EmpRoomsManager.GetNotDelAll().Where(m => m.EmpId == defaultEmpId).ToList();
        //        List<vwDepartments> allDepartments = unitWork.DepartmentManager.GetNotDelAll().Where(m => m.Id == defaultDeptId).ToList();

        //        List<Room_tbl> allRooms = unitWork.RoomsManager.GetNotDelAll().ToList();

        //        var allRoom = (from p in allRooms // get Rooms table as p
        //                       join e in allEmpRooms // implement join as e in Emp_rooms table
        //                         on p.Room_Id equals e.RoomId //implement join on rows where p.RoomId == e.RoomId
        //                       select p).ToList();

        //        var CurRoomId = allEmpRooms.Select(m => m.RoomId).FirstOrDefault();
        //        var CurDeptId = allDepartments.Select(m => m.Id).FirstOrDefault();

        //        if (ToFromFlag == 1)
        //        {
        //            FromEmployees = new SelectList(allEmployees, "Id", "FULL_NAME_AR", defaultEmpId);
        //            FromRooms = new SelectList(allRoom, "Room_Id", "Room_Name", CurRoomId);
        //            FromDepartments = new SelectList(allDepartments, "Id", "Name", CurDeptId);

        //            FromRoomId = CurRoomId;
        //            FromEmpId = defaultEmpId.GetValueOrDefault();
        //            FromDeptId = CurDeptId;
        //        }
        //        //else if (ToFromFlag == 2)
        //        //{
        //        //    ToEmployees = new SelectList(allEmployees, "Id", "FULL_NAME_AR", defaultEmpId);
        //        //    ToRooms = new SelectList(allRoom, "Room_Id", "Room_Name", CurRoomId);
        //        //    ToDepartments = new SelectList(allDepartments, "Id", "Name", CurDeptId);

        //        //    ToRoomId = CurRoomId;
        //        //    ToEmpId = defaultEmpId.GetValueOrDefault();
        //        //    ToDeptId = CurDeptId;
        //        //}

        //    }

        //    if (defaultRoomId != null)
        //    {
        //        var allRooms = unitWork.RoomsManager.GetNotDelAll().OrderByDescending(m => m.Room_Id).ToList();
        //        List<Emp_rooms> allEmpRooms = unitWork.EmpRoomsManager.GetNotDelAll().Where(m => m.RoomId == defaultRoomId).ToList();
        //        List<vwDepartments> Departments = unitWork.DepartmentManager.GetNotDelAll().ToList();
        //        List<vwEmployees> Employees = unitWork.EmployeesManager.GetNotDelAll().ToList();

        //        var allDepartments = (from p in Departments // get Rooms table as p
        //                              join e in allEmpRooms // implement join as e in Emp_rooms table
        //                                on p.Id equals e.DeptId //implement join on rows where p.RoomId == e.RoomId
        //                              select p).ToList();

        //        var CurDepartmentId = allDepartments.Select(m => m.Id).FirstOrDefault();


        //        var allEmployees = (from p in Employees // get Rooms table as p
        //                            join e in allEmpRooms // implement join as e in Emp_rooms table
        //                                 on p.Id equals e.EmpId //implement join on rows where p.RoomId == e.RoomId
        //                            select p).ToList();

        //        var CurEmployeeId = allEmployees.Select(m => m.Id).FirstOrDefault();

        //        if (ToFromFlag == 1)
        //        {

        //            FromEmployees = new SelectList(allEmployees, "Id", "FULL_NAME_AR", CurEmployeeId);
        //            FromRooms = new SelectList(allRooms, "Room_Id", "Room_Name", defaultRoomId);
        //            FromDepartments = new SelectList(allDepartments, "Id", "Name", CurDepartmentId);
        //            FromRoomId = defaultRoomId.GetValueOrDefault();
        //            FromEmpId = CurEmployeeId;
        //            FromDeptId = CurDepartmentId;
        //        }
        //        //else if (ToFromFlag == 2)
        //        //{

        //        //    ToEmployees = new SelectList(allEmployees, "Id", "FULL_NAME_AR", CurEmployeeId);
        //        //    ToRooms = new SelectList(allRooms, "Room_Id", "Room_Name", defaultRoomId);
        //        //    ToDepartments = new SelectList(allDepartments, "Id", "Name", CurDepartmentId);
        //        //    ToRoomId = defaultRoomId.GetValueOrDefault();
        //        //    ToEmpId = CurEmployeeId;
        //        //    ToDeptId = CurDepartmentId;
        //        //}
        //    }


        //}

        //public void setDropDrownList(string type, long value, int ToOrFromFlag)
        //{
        //    int CurDepartmentId;
        //    int CurEmployeeId;
        //    long CurRoomId;
        //    //ItemsStructureViewModel model = new ItemsStructureViewModel();
        //    //if (ToOrFromFlag == 1)
        //    //{
        //        CurDepartmentId = FromDeptId;
        //        CurEmployeeId = FromEmpId.GetValueOrDefault();
        //        CurRoomId = FromRoomId.GetValueOrDefault();
        //    //}
        //    //else
        //    //{
        //    //    CurDepartmentId = ToDeptId;
        //    //    CurEmployeeId = ToEmpId.GetValueOrDefault();
        //    //    CurRoomId = ToRoomId.GetValueOrDefault();
        //    //}
        //    if (type == "FromEmpId" || type == "ToEmpId") // when select Employees dropdown list
        //    {

        //        var allEmployees = unitWork.EmployeesManager.GetNotDelAll().OrderByDescending(m => m.Id).ToList();
        //        List<Emp_rooms> allEmpRooms = unitWork.EmpRoomsManager.GetNotDelAll().Where(m => m.EmpId == value).ToList();
        //        vwEmployees Emp = unitWork.EmployeesManager.GetNotDelAll().Where(m => m.Id == value).ToList().FirstOrDefault();

        //        List<Room_tbl> allRooms = unitWork.RoomsManager.GetNotDelAll().ToList();
        //        var allRoom = (from p in allRooms // get Rooms table as p
        //                       join e in allEmpRooms // implement join as e in Emp_rooms table
        //                         on p.Room_Id equals e.RoomId //implement join on rows where p.RoomId == e.RoomId
        //                       select p).ToList();

        //        List<vwDepartments> allDepartments = unitWork.DepartmentManager.GetNotDelAll().Where(m => m.Id == Emp.Department_Id).ToList();


        //        CurRoomId = allEmpRooms.Select(m => m.RoomId).FirstOrDefault();
        //        CurDepartmentId = allDepartments.Select(m => m.Id).FirstOrDefault();

        //        if (type == "FromEmpId")
        //        {
        //            FromDeptId = CurDepartmentId;
        //            FromEmpId = int.Parse(value.ToString());
        //            FromRoomId = CurRoomId;
        //            CurEmployeeId = int.Parse(value.ToString());
        //            FromRooms = new SelectList(allRoom, "Room_Id", "Room_Name", CurRoomId);
        //            FromDepartments = new SelectList(allDepartments, "Id", "Name", CurDepartmentId);
        //            FromEmployees = new SelectList(allEmployees, "Id", "FULL_NAME_AR", value);
        //        }
        //        //else if (type == "ToEmpId")
        //        //{
        //        //    ToDeptId = CurDepartmentId;
        //        //    ToEmpId = int.Parse(value.ToString());
        //        //    ToRoomId = CurRoomId;
        //        //    ToRooms = new SelectList(allRoom, "Room_Id", "Room_Name", CurRoomId);
        //        //    ToDepartments = new SelectList(allDepartments, "Id", "Name", CurDepartmentId);
        //        //    ToEmployees = new SelectList(allEmployees, "Id", "FULL_NAME_AR", value);
        //        //}

        //    }
        //    else if (type == "FromRoomId" || type == "ToRoomId") // when select Rooms dropdown list
        //    {
        //        var allRooms1 = unitWork.RoomsManager.GetNotDelAll().OrderByDescending(m => m.Room_Id).ToList();

        //        List<Emp_rooms> allEmpRooms1 = unitWork.EmpRoomsManager.GetNotDelAll().Where(m => m.RoomId == value).ToList();
        //        List<vwDepartments> Departments = unitWork.DepartmentManager.GetNotDelAll().ToList();
        //        List<vwEmployees> Employees = unitWork.EmployeesManager.GetNotDelAll().ToList();

        //        List<vwDepartments> allDepartments1 = (from p in Departments // get Rooms table as p
        //                                               join e in allEmpRooms1 // implement join as e in Emp_rooms table
        //                                               on p.Id equals e.DeptId //implement join on rows where p.RoomId == e.RoomId
        //                                               select p).ToList();




        //        // select employees in the room
        //        var allEmployees1 = (from p in Employees // get Rooms table as p
        //                             join e in allEmpRooms1 // implement join as e in Emp_rooms table
        //                                  on p.Id equals e.EmpId //implement join on rows where p.RoomId == e.RoomId
        //                             select p).ToList();


        //        if (DisplayMode == "FromDeptId" && ToOrFromFlag == 1)
        //        {
        //            CurEmployeeId = allEmpRooms1.Where(m => m.DeptId == CurDepartmentId).FirstOrDefault().EmpId.GetValueOrDefault();
        //        }
        //        else if (DisplayMode == "FromEmpId" && ToOrFromFlag == 1)
        //        {
        //            CurDepartmentId = allEmpRooms1.Where(m => m.EmpId == CurEmployeeId).FirstOrDefault().DeptId.GetValueOrDefault();
        //        }
        //        else
        //        {
        //            CurEmployeeId = allEmployees1.Select(m => m.Id).FirstOrDefault();
        //            CurDepartmentId = allDepartments1.Select(m => m.Id).FirstOrDefault();
        //        }

        //        if (type == "FromRoomId")
        //        {
        //            FromDeptId = CurDepartmentId;
        //            FromEmpId = CurEmployeeId;
        //            FromRoomId = value;

        //            FromRooms = new SelectList(allRooms1, "Room_Id", "Room_Name", value);
        //            FromDepartments = new SelectList(allDepartments1, "Id", "Name", FromDeptId);

        //            FromEmployees = new SelectList(allEmployees1, "Id", "FULL_NAME_AR", FromEmpId);

        //        }
        //        //else if (type == "ToRoomId")
        //        //{
        //        //    ToDeptId = CurDepartmentId;
        //        //    ToEmpId = CurEmployeeId;
        //        //    ToRoomId = value;

        //        //    ToRooms = new SelectList(allRooms1, "Room_Id", "Room_Name", value);
        //        //    ToDepartments = new SelectList(allDepartments1, "Id", "Name", FromDeptId);
        //        //    ToEmployees = new SelectList(allEmployees1, "Id", "FULL_NAME_AR", FromEmpId);

        //        //}
        //    }

        //    else if (type == "FromDeptId")
        //    {
        //        var allEmployees2 = unitWork.EmployeesManager.GetNotDelAll().Where(m => m.Department_Id == value).ToList();
        //        var CurEmployeeId2 = allEmployees2.Select(m => m.Id).FirstOrDefault();
        //        if (type == "FromDeptId")
        //        {
        //            FromEmployees = new SelectList(allEmployees2, "Id", "FULL_NAME_AR", CurEmployeeId2);
        //        }
        //        //else if (type == "ToDeptId")
        //        //{
        //        //    ToEmployees = new SelectList(allEmployees2, "Id", "FULL_NAME_AR", CurEmployeeId2);
        //        //}
        //    }


        //}

    }
}