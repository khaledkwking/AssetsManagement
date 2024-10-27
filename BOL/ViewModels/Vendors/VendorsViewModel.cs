using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DAL;
namespace BOL
{
    public class VendorsViewModel
    {

        private UnitOfWork unitWork = new UnitOfWork();
        public PagedList.IPagedList<Vendors> Vendors { get; set; }
        public Vendors SelectedVendor { get; set; }
        public  string DisplayMode { get; set; }
        public string Sorting_Order { get; set; }
        public  string Search_Data { get; set; }
        public string Filter_Value { get; set; }
        public  int? PageNumber { get; set; }
        public int? PageCount { get; set; }

        public SelectList ContractsList { get; set; }

        public void setDropDrownList(string type, int value, string defaultValue)
        {
            //ItemsStructureViewModel model = new ItemsStructureViewModel();
            switch (type)
            {
                case "VendorId":

                    var CategoryList = unitWork.VendorContractsManager.GetNotDelAll().Where(m => m.VendorId  == value).ToList();
                    ContractsList = new SelectList(CategoryList, "ContractId", "ContractName", defaultValue);
                    break;
               
            }

        }
    }
}