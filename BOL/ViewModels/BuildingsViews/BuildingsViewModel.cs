using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DAL;
namespace BOL
{
    public class BuildingsViewModel
    {
      
      
        public PagedList.IPagedList<Building_tbl> Buildings { get; set; }
        public Building_tbl SelectedBuilding { get; set; }
        public  string DisplayMode { get; set; }
        public string Sorting_Order { get; set; }
        public  string Search_Data { get; set; }
        public string Filter_Value { get; set; }
        public  int? PageNumber { get; set; }
        public int? PageCount { get; set; }
        public SelectList Floors { get; set; }
        public void setDropDrownList(string type, int value)
        {
            //ItemsStructureViewModel model = new ItemsStructureViewModel();
            UnitOfWork unitWork = new UnitOfWork();
            switch (type)
            {
                case "BuildingId":

                    var CategoryList = unitWork.FloorsManager.GetNotDelAll().Where(m => m.Building_Id == value).ToList();
                    Floors = new SelectList(CategoryList, "Floor_Id", "Floor_Name");

                    break;
            }

        }

    }
}