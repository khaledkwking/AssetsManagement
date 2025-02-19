﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using DAL;
namespace BOL
{
    public class RequestOrdersDetailsManager : Repository<RequestOrdersDetails>
    {
        
        public RequestOrdersDetailsManager(GPFAssetsEntities ctx) : base(ctx)
        {
           
        }
        public override bool Delete(RequestOrdersDetails entity)
        {
            return base.Delete(entity);
        }
        //public List <Car> GetStudentByGender(int genderId)
        //{
        //    return GetAll().Where(c => c.genderId == genderId).ToList();
        //}
        public bool update(int id)
        {
            RequestOrdersDetails st = GetById(id);

            return Update(st);
        }
        //public List<RequestOrdersDetails> GetCastByName(string UnitSearch)
        //{
        //    //stu => stu.CarNo.ToUpper().Contains(Search_Data.ToUpper()) || stu.CarType.ToUpper().Contains(Search_Data.ToUpper()));

        //    return GetNotDelAll().Where(c => c..ToUpper().Contains(UnitSearch.ToUpper())).ToList();
        //}
        public List<RequestOrdersDetails> GetByOrderId(long OrderId)
        {
            //stu => stu.CarNo.ToUpper().Contains(Search_Data.ToUpper()) || stu.CarType.ToUpper().Contains(Search_Data.ToUpper()));
            
            return GetNotDelAll().Where(c => c.ReqOrderId == OrderId).ToList();

        }
        public List<RequestOrdersDetails> GetByOrderId_MasterItem(long OrderId)
        {
            //stu => stu.CarNo.ToUpper().Contains(Search_Data.ToUpper()) || stu.CarType.ToUpper().Contains(Search_Data.ToUpper()));

            return GetNotDelAll().Where(c => c.ReqOrderId == OrderId && c.MasterItemFalg ==true ).ToList();

        }
        public RequestOrdersDetails GetByOrderDetId(long Id)
        {
            RequestOrdersDetails st = GetById(Id);
          
            return st;
        }
        public List<RequestOrdersDetails> GetNotDelAll()
        {
            return GetAll().Where(c => c.IsDeleted == false || c.IsDeleted == null).ToList();
        }
        public List<RequestOrdersDetails> GetByItemId(long ItemId, long ReqOrderId)
        {
            //stu => stu.CarNo.ToUpper().Contains(Search_Data.ToUpper()) || stu.CarType.ToUpper().Contains(Search_Data.ToUpper()));

            return GetNotDelAll().Where(c => c.ItemId == ItemId && (c.ReqOrderId== ReqOrderId) && (c.MasterItemFalg == false || c.MasterItemFalg == null )).ToList();

        }

        public List<RequestOrdersDetails> GetByAllItemId(long ItemId, long ReqOrderId)
        {
            //stu => stu.CarNo.ToUpper().Contains(Search_Data.ToUpper()) || stu.CarType.ToUpper().Contains(Search_Data.ToUpper()));

            return GetNotDelAll().Where(c => c.ItemId == ItemId && (c.ReqOrderId == ReqOrderId)).ToList();

        }

        //public List<RequestOrdersDetails> GetNotDelAllByParam(DateTime? FromDate ,DateTime? Todate, long? RoomId
        //    , int? DeptId, int? EmpId, long? StoreId, long? ItemId)
        //{
        //    //stu => stu.CarNo.ToUpper().Contains(Search_Data.ToUpper()) || stu.CarType.ToUpper().Contains(Search_Data.ToUpper()));
        //    List<RequestOrdersDetails> List;
        //    //DbFunctions.TruncateTime(c.OutOrders.OutOrderDate) >= DbFunctions.TruncateTime(FromDate) && DbFunctions.TruncateTime(c.OutOrders.OutOrderDate) <= DbFunctions.TruncateTime(Todate)
        //    List = GetAll().Where(c => (DbFunctions.TruncateTime(c.OutOrders.OutOrderDate) >= DbFunctions.TruncateTime(FromDate) && DbFunctions.TruncateTime(c.OutOrders.OutOrderDate) <= DbFunctions.TruncateTime(Todate))
        //     && ((c.OutOrders.RoomId == RoomId || RoomId == null)
        //     && (c.OutOrders.EmpId == EmpId || EmpId == null)
        //     && (c.OutOrders.DeptId == DeptId || DeptId == null)
        //     && (c.OutOrders.StoreId == StoreId || StoreId == null)
        //     && (c.ItemId == ItemId || ItemId == null)
        //     && (c.IsDeleted == false || c.IsDeleted == null))
        //    ).ToList();

        //    return GetAllDetails(List);
        //}
        //public List<RequestOrdersDetails> GetAllDetails(List<RequestOrdersDetails> List)
        //{
        //    UnitOfWork UWork = new UnitOfWork();
        //    List<vwEmployees> empList = UWork.EmployeesManager.GetNotDelAll().ToList();
        //    List<vwDepartments> DeptList = UWork.DepartmentManager.GetNotDelAll().ToList();

        //    foreach (var Item in List)
        //    {
        //        if (Item.OutOrders.EmpId  != null)
        //        {
        //            int EmpId = int.Parse(Item.OutOrders.EmpId.ToString());
        //            vwEmployees CurList = new vwEmployees();
        //            CurList = empList.Where(c => c.Id == EmpId).FirstOrDefault();
        //            Item.EmpName = CurList.FULL_NAME_AR;
        //            Item.OutOrders.VmEmployees = CurList;
        //        }
        //        if (Item.OutOrders.DeptId != null)
        //        {
        //            int DeptId = int.Parse(Item.OutOrders.DeptId.ToString());
        //            vwDepartments CurDeptList = new vwDepartments();
        //            CurDeptList = DeptList.Where(c => c.Id == DeptId).FirstOrDefault();
        //            Item.DeptName = CurDeptList.Name;
        //            Item.OutOrders.VmDepartments = CurDeptList;
        //        }

        //    }
        //    return List;
        //}

        //var allRoom = (from p in allRooms // get Rooms table as p
        //               join e in allEmpRooms // implement join as e in Emp_rooms table
        //                 on p.Room_Id equals e.RoomId //implement join on rows where p.RoomId == e.RoomId
        //               select p).ToList();
    }
}