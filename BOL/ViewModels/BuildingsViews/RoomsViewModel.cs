using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DAL;
namespace BOL
{
    public class RoomsViewModel
    {
      
      
        public PagedList.IPagedList<Room_tbl> Rooms { get; set; }
        public Room_tbl  SelectedRoom { get; set; }
        public  string DisplayMode { get; set; }
        public string Sorting_Order { get; set; }
        public  string Search_Data { get; set; }
        public string Filter_Value { get; set; }
        public  int? PageNumber { get; set; }
        public int? PageCount { get; set; }
        public int? CurrentStoreFlag { get; set; }

        public string CurFloorId { get; set; }
        public string CurBulidingId { get; set; }
        public int? ShowEmptyFlag { get; set; }
        public IEnumerable<SelectListItem> Depatments { get; set; }
        public IEnumerable<SelectListItem> RoomStatus { get; set; }
        public IEnumerable<SelectListItem> RoomTypes { get; set; }
        public Boolean StoreFlagStr
        //{
            { get; set; }
            //get
            //{
            //    //return SelectedRoom.StoreFlag ?? false;
            //};
            //set
            //{

            //};
        //}
    }
}