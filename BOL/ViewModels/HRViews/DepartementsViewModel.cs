using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DAL;
namespace BOL
{
    public class DepartementsViewModel
    {

        private UnitOfWork unitWork = new UnitOfWork();
        public PagedList.IPagedList<vwDepartments> Departements { get; set; }
        public vwDepartments SelectedItem { get; set; }
        public  string DisplayMode { get; set; }
        public string Sorting_Order { get; set; }
        public  string Search_Data { get; set; }
        public string Filter_Value { get; set; }
        public  int? PageNumber { get; set; }
        public int? PageCount { get; set; }

        public SelectList vwEmployees { get; set; }
        public SelectList vwRooms { get; set; }

        public SelectList vwContracts { get; set; }


        public void setDropDrownList(string type, int value, string defaultValue)
        {
            //ItemsStructureViewModel model = new ItemsStructureViewModel();
            switch (type)
            {
                case "DeptId":

                    var CategoryList = unitWork.EmployeesManager.GetNotDelAll().Where(m => m.Department_Id == value).ToList();
                    vwEmployees = new SelectList(CategoryList, "Id", "FULL_NAME_AR", defaultValue);
                    break;
                case "FloorId":

                    var RoomsList = unitWork.RoomsManager.GetNotDelAll().Where(m => m.Floor_Id  == value).ToList();
                    vwRooms = new SelectList(RoomsList, "Room_Id", "Room_Name", defaultValue);
                    
                    break;
                case "VendorId":

                    var ContractList = unitWork.VendorContractsManager.GetNotDelAllByVendorId(value).ToList();
                    vwContracts = new SelectList(ContractList, "ContractId", "ContractName", defaultValue);

                    break;
                    
            }

        }

    }
}