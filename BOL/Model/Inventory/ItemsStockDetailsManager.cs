using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using DAL;
namespace BOL
{
    public class ItemsStockDetailsManager : Repository<tbl_ItemsStockDetails>
    {

        public const int DamagedItem = 3;
        public ItemsStockDetailsManager(GPFAssetsEntities ctx) : base(ctx)
        {

        }
        public override bool Delete(tbl_ItemsStockDetails entity)
        {
            return base.Delete(entity);
        }
        public bool update(int id)
        {
            tbl_ItemsStockDetails st = GetById(id);
            //st.StudentName = Name;
            return Update(st);
        }
        
        public List<tbl_ItemsStockDetails> GetCastByUnitName(string UnitSearch, long? StockId)
        {
            //stu => stu.CarNo.ToUpper().Contains(Search_Data.ToUpper()) || stu.CarType.ToUpper().Contains(Search_Data.ToUpper()));
            List<tbl_ItemsStockDetails> List;
            List= GetAll().Where(c => (c.BarCode.ToUpper().Contains(UnitSearch.ToUpper()) 
            || c.Notes.ToUpper().Contains(UnitSearch.ToUpper()))
            && (c.StockId ==StockId)
            && (c.IsDeleted == false || c.IsDeleted == null)
            ).ToList();
            //return GetDetials(List);
            return List;
        }
        public List<tbl_ItemsStockDetails> GetByStockId(long StockId)
        {
            //stu => stu.CarNo.ToUpper().Contains(Search_Data.ToUpper()) || stu.CarType.ToUpper().Contains(Search_Data.ToUpper()));
            return GetNotDelAll().Where(c => (c.StockId == StockId) && (c.IsDeleted == false || c.IsDeleted == null)).ToList();
        }
        public List<tbl_ItemsStockDetails> GetByInOrderDetId(long InOrderDetId)
        {
            //stu => stu.CarNo.ToUpper().Contains(Search_Data.ToUpper()) || stu.CarType.ToUpper().Contains(Search_Data.ToUpper()));
            return GetNotDelAll().Where(c => (c.InOrderDetId == InOrderDetId) && (c.IsDeleted == false || c.IsDeleted == null)).ToList();
        }
        public List<tbl_ItemsStockDetails> GetByStockDetId(long StockDetId)
        {
            //stu => stu.CarNo.ToUpper().Contains(Search_Data.ToUpper()) || stu.CarType.ToUpper().Contains(Search_Data.ToUpper()));
            return GetNotDelAll().Where(c => (c.StockDetId == StockDetId) && (c.IsDeleted == false || c.IsDeleted == null)).ToList();
        }
        // Search  by ItemId
        public tbl_ItemsStockDetails GetByItemId(long ItemId, long StoreId)
        {
            //stu => stu.CarNo.ToUpper().Contains(Search_Data.ToUpper()) || stu.CarType.ToUpper().Contains(Search_Data.ToUpper()));
            return GetNotDelAll().Where(c => c.tbl_ItemsStock.Item_Id== ItemId && c.tbl_ItemsStock.Room_Id == StoreId).ToList().FirstOrDefault();
        }

        public tbl_ItemsStockDetails GetByBarcode(string Barcode, long StoreId)
        {
            //stu => stu.CarNo.ToUpper().Contains(Search_Data.ToUpper()) || stu.CarType.ToUpper().Contains(Search_Data.ToUpper()));

            return GetNotDelAll().Where(c => c.BarCode .ToUpper() == Barcode.ToUpper() && c.tbl_ItemsStock.Room_Id  == StoreId).ToList().FirstOrDefault();
        }
       
        
        public List<tbl_ItemsStockDetails> GetNotDelAll()
        {
            UnitOfWork UWork = new UnitOfWork();
          
            List<tbl_ItemsStockDetails> List;

            List = GetAll().Where(c => c.IsDeleted == false || c.IsDeleted == null).ToList();

            //List = GetDetials(List);

            return List;
        }
                   
        //public List<tbl_ItemsStock> GetDetials(List<tbl_ItemsStock> List)
        //{
        //    UnitOfWork UWork = new UnitOfWork();
        //    List<vwEmployees> empList = UWork.EmployeesManager.GetNotDelAll().ToList();
        //    List<vwDepartments> DeptList = UWork.DepartmentManager.GetNotDelAll().ToList();
        //    foreach (var Item in List)
        //    {
        //        if (Item.Emp_Id != null)
        //        {
        //            int EmpId = int.Parse(Item.Emp_Id.ToString());
        //            vwEmployees CurList = new vwEmployees();
        //            CurList = empList.Where(c => c.Id == EmpId).FirstOrDefault();
        //            Item.VmEmployees = CurList;
        //        }
        //        if (Item.Depart_Id != null)
        //        {
        //            int DeptId = int.Parse(Item.Depart_Id.ToString());
        //            vwDepartments CurDeptList = new vwDepartments();

        //            CurDeptList = DeptList.Where(c => c.Id == DeptId).FirstOrDefault();

        //            Item.VmDepartments = CurDeptList;
        //        }
        //    }
        //    return List;
        //}
                
    }
}