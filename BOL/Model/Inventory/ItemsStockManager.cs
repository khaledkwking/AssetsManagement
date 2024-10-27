using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using DAL;
namespace BOL
{
    public class ItemsStockManager : Repository<tbl_ItemsStock>
    {

        public const int DamagedItem = 3;
        public ItemsStockManager(GPFAssetsEntities ctx) : base(ctx)
        {

        }

        public override bool Delete(tbl_ItemsStock entity)
        {
            return base.Delete(entity);
        }
        //public List <Car> GetStudentByGender(int genderId)
        //{
        //    return GetAll().Where(c => c.genderId == genderId).ToList();
        //}
        public bool update(int id, string Name, string address, int genderId)
        {
            tbl_ItemsStock st = GetById(id);
            //st.StudentName = Name;
            return Update(st);
        }
        public bool updateDept_Room_Employee(long stockid, int DeptId, long RoomId, int? EmpId)
        {

            tbl_ItemsStock st = GetById(stockid);
            st.Depart_Id = DeptId;
            st.Room_Id = RoomId;
            st.Emp_Id = EmpId;
            st.Item_AssDate = DateTime.Today;
            //st.StudentName = Name;
            return Update(st);
        }
        public List<tbl_ItemsStock> GetCastByUnitName(string UnitSearch, long? RoomId)
        {
            //stu => stu.CarNo.ToUpper().Contains(Search_Data.ToUpper()) || stu.CarType.ToUpper().Contains(Search_Data.ToUpper()));
            List<tbl_ItemsStock> List;
            List= GetAll().Where(c => (c.Item_BarCode.ToUpper().Contains(UnitSearch.ToUpper()) || c.Item_RFID.ToUpper().Contains(UnitSearch.ToUpper())
            || c.Item_tbl.Item_Name.ToUpper().Contains(UnitSearch.ToUpper())
            || c.Item_Serial.ToUpper().Contains(UnitSearch.ToUpper())) 
            && (c.IsDeleted == false || c.IsDeleted == null)
            && (c.Room_Id == RoomId || RoomId == null)
            && (c.AssetFlag == false || c.AssetFlag == null)

            ).ToList();
            return GetDetials(List);
        }
        public List<tbl_ItemsStock> GetByItemName(string UnitSearch, List<CatMain_tbl> MainCats)
        {
            var listOfMainCatsId = MainCats.Select(r => r.CatMain_Id).ToList();
            //stu => stu.CarNo.ToUpper().Contains(Search_Data.ToUpper()) || stu.CarType.ToUpper().Contains(Search_Data.ToUpper()));
            List<tbl_ItemsStock> NewList;

            NewList = GetAll().Where(c => listOfMainCatsId.Contains(c.Item_tbl.CatSub_tbl.Category_tbl.CatMain_tbl.CatMain_Id)
            && (c.Item_BarCode.ToUpper().Contains(UnitSearch.ToUpper()) || c.Item_RFID.ToUpper().Contains(UnitSearch.ToUpper())
             || c.Item_tbl.Item_Name.ToUpper().Contains(UnitSearch.ToUpper())
             || c.Item_Serial.ToUpper().Contains(UnitSearch.ToUpper()) || UnitSearch==null || UnitSearch == "")
             && (c.IsDeleted == false || c.IsDeleted == null)).ToList();
            
            //NewList = NewList.Where(c => listOfMainCatsId.Contains(c.Item_tbl.CatSub_tbl.Category_tbl.CatMain_tbl.CatMain_Id)).ToList();

         
            return GetDetials(NewList);
        }
        // Search  by ItemId
        public tbl_ItemsStock GetByItemId(long ItemId, long StoreId)
        {
            //stu => stu.CarNo.ToUpper().Contains(Search_Data.ToUpper()) || stu.CarType.ToUpper().Contains(Search_Data.ToUpper()));

            return GetAll().Where(c => c.Item_Id== ItemId && c.Room_Id == StoreId).ToList().FirstOrDefault();
        }
        public List <tbl_ItemsStock> GetByAllItemId(long ItemId, long StoreId)
        {
            //stu => stu.CarNo.ToUpper().Contains(Search_Data.ToUpper()) || stu.CarType.ToUpper().Contains(Search_Data.ToUpper()));

            return GetAll().Where(c => c.Item_Id == ItemId && c.Room_Id == StoreId).ToList();
        }

        public tbl_ItemsStock GetByBarcode(string Barcode, long StoreId)
        {
            //stu => stu.CarNo.ToUpper().Contains(Search_Data.ToUpper()) || stu.CarType.ToUpper().Contains(Search_Data.ToUpper()));
            return GetAll().Where(c => c.Item_BarCode.ToUpper() == Barcode.ToUpper() && c.Room_Id == StoreId
            && (c.IsDeleted == false || c.IsDeleted == null)).ToList().FirstOrDefault();
        }
        public tbl_ItemsStock GetByRFIDOutSide(string RFID, long StoreId)
        {
            //stu => stu.CarNo.ToUpper().Contains(Search_Data.ToUpper()) || stu.CarType.ToUpper().Contains(Search_Data.ToUpper()));
           
            return GetAll().Where(c => c.Item_RFID.ToUpper() == RFID.ToUpper() && c.Room_Id != StoreId
            && (c.IsDeleted == false || c.IsDeleted == null)).ToList().FirstOrDefault();
        }
        public tbl_ItemsStock GetByRFID(string RFID, long? StoreId)
        {
            if (StoreId==null)
            {
                return GetAll().Where(c => c.Item_RFID.ToUpper() == RFID.ToUpper()
                && (c.IsDeleted == false || c.IsDeleted == null)).ToList().FirstOrDefault();
            }
            else
            {
                return GetAll().Where(c => c.Item_RFID.ToUpper() == RFID.ToUpper() && c.Room_Id == StoreId
                && (c.IsDeleted == false || c.IsDeleted == null)).ToList().FirstOrDefault();
            }
            //stu => stu.CarNo.ToUpper().Contains(Search_Data.ToUpper()) || stu.CarType.ToUpper().Contains(Search_Data.ToUpper()));
         
        }
        public tbl_ItemsStock GetByBarcodeOutSide(string Barcode, long StoreId)
        {
            //stu => stu.CarNo.ToUpper().Contains(Search_Data.ToUpper()) || stu.CarType.ToUpper().Contains(Search_Data.ToUpper()));

            return GetAll().Where(c => c.Item_BarCode.ToUpper() == Barcode.ToUpper() && c.Room_Id != StoreId).ToList().FirstOrDefault();
        }
        public List<tbl_ItemsStock> GetNotDelByDeptId(int? DeptId, long  RoomId, int? EmpId)
        {
            UnitOfWork UWork = new UnitOfWork();
            List<vwEmployees> empList = UWork.EmployeesManager.GetNotDelAll().ToList();
            List<vwDepartments> DeptList = UWork.DepartmentManager.GetNotDelAll().ToList();
            List<tbl_ItemsStock> List;

            List = GetAll().Where(c => (c.Depart_Id == DeptId || DeptId == null) && (c.Room_Id == RoomId) && (c.IsDeleted == false || c.IsDeleted == null)).ToList();

            List = GetDetials(List);

            return List;
        }
        public List<tbl_ItemsStock> GetNotDelAll()
        {
            UnitOfWork UWork = new UnitOfWork();
            List<vwEmployees> empList = UWork.EmployeesManager.GetNotDelAll().ToList();
            List<vwDepartments> DeptList = UWork.DepartmentManager.GetNotDelAll().ToList();
            List<tbl_ItemsStock> List;

            List = GetAll().Where(c => c.IsDeleted == false || c.IsDeleted == null).ToList();

            List = GetDetials(List);

            return List;
        }
        public List<tbl_ItemsStock> GetNotDelAllAndDamaged(string UnitSearch)
        {
            UnitOfWork UWork = new UnitOfWork();
            List<vwEmployees> empList = UWork.EmployeesManager.GetNotDelAll().ToList();
            List<vwDepartments> DeptList = UWork.DepartmentManager.GetNotDelAll().ToList();
            List<tbl_ItemsStock> List;
            if (!String.IsNullOrEmpty(UnitSearch))
            {
                        List = GetAll().Where(c => (c.Item_BarCode.ToUpper().Contains(UnitSearch.ToUpper()) || c.Item_RFID.ToUpper().Contains(UnitSearch.ToUpper())

                  || c.Item_tbl.Item_Name.ToUpper().Contains(UnitSearch.ToUpper())) && (c.IsDeleted == true && c.Item_StateId == DamagedItem)
               ).ToList();
            }
            else
            {
                List = GetAll().Where(c => c.IsDeleted == true && c.Item_StateId == DamagedItem).ToList();
            }

            //List = GetAll().Where(c => c.IsDeleted == true && c.Item_StateId == DamagedItem).ToList();



            return List;
        }
        public List<tbl_ItemsStock> GetNotDelOutsideStore(string RFID, long? RoomId)
        {


            List<tbl_ItemsStock> List;


            if (RoomId != null)
            {
                List = GetAll().Where(c => (c.IsDeleted == false || c.IsDeleted == null) && c.Item_StateId != DamagedItem && c.Room_Id != RoomId && c.Item_RFID.ToUpper() == RFID.ToUpper()).ToList();
            }
            else
            {
                List = GetAll().Where(c => (c.IsDeleted == false || c.IsDeleted == null) && c.Item_StateId != DamagedItem && c.Item_RFID.ToUpper() == RFID.ToUpper()).ToList();
            }

            List = GetDetials(List);

            //ListFilter = GetAll().Where(c => c.Item_RFID.ToUpper().Contains(param.ToUpper())).ToList ();
            //ListFilter = (from obj in List where obj.Item_RFID.ToUpper().Contains(param.ToUpper()) select obj).ToList();
            //List.Where(obj => DbFunctions.Like(obj.Item_RFID, param));

            return List;
        }
        public List<tbl_ItemsStock> GetNotDelAllLike(string RFID, long? RoomId)
        {


            List<tbl_ItemsStock> List;


            if (RoomId != null)
            {
                List = GetAll().Where(c => (c.IsDeleted == false || c.IsDeleted == null) && c.Item_StateId != DamagedItem && c.Room_Id == RoomId && c.Item_RFID.ToUpper() == RFID.ToUpper()).ToList();
            }
            else
            {
                List = GetAll().Where(c => (c.IsDeleted == false || c.IsDeleted == null) && c.Item_StateId != DamagedItem && c.Item_RFID.ToUpper() == RFID.ToUpper()).ToList();
                if (List.Count==0)
                {
                    List = GetAll().Where(c => (c.IsDeleted == true) && c.Item_StateId == DamagedItem && c.Item_RFID.ToUpper() == RFID.ToUpper()).ToList();
                }
                
               
            }

            List = GetDetials(List);

            //ListFilter = GetAll().Where(c => c.Item_RFID.ToUpper().Contains(param.ToUpper())).ToList ();
            //ListFilter = (from obj in List where obj.Item_RFID.ToUpper().Contains(param.ToUpper()) select obj).ToList();
            //List.Where(obj => DbFunctions.Like(obj.Item_RFID, param));

            return List;
        }
        public List<tbl_ItemsStock> GetDetials(List<tbl_ItemsStock> List)
        {
            UnitOfWork UWork = new UnitOfWork();
            List<vwEmployees> empList = UWork.EmployeesManager.GetAll().ToList();// GetNotDelAll().ToList();
            List<vwDepartments> DeptList = UWork.DepartmentManager.GetNotDelAll().ToList();
            foreach (var Item in List)
            {
                if (Item.Emp_Id != null)
                {
                    int EmpId = int.Parse(Item.Emp_Id.ToString());
                    vwEmployees CurList = new vwEmployees();
                    CurList = empList.Where(c => c.Id == EmpId).FirstOrDefault();
                    Item.VmEmployees = CurList;
                }
                if (Item.Depart_Id != null)
                {
                    int DeptId = int.Parse(Item.Depart_Id.ToString());
                    vwDepartments CurDeptList = new vwDepartments();

                    CurDeptList = DeptList.Where(c => c.Id == DeptId).FirstOrDefault();

                    Item.VmDepartments = CurDeptList;
                }
            }
            return List;
        }

        public List<tbl_ItemsStock> GetByParam(long? ItemId, int? DeptId, long? RoomId, int? EmpId, int? ContractId)
        {
            UnitOfWork UWork = new UnitOfWork();
           
            List<tbl_ItemsStock> List;

            List = GetAll().Where(c => (c.Depart_Id == DeptId || DeptId==null) && (c.Room_Id == RoomId || RoomId==null) 
            && (c.Emp_Id == EmpId || EmpId == null) &&  (c.Item_Id  == ItemId || ItemId == null) &&
            (c.ContractId == ContractId || ContractId == null) &&
            (c.IsDeleted == false || c.IsDeleted == null)
             && (c.AssetFlag == false || c.AssetFlag == null)).ToList();
                       

            List = GetDetials(List);

            return List;
        }
        public List<tbl_ItemsStock> GetByInventoryParam(long? ItemId, long[] StoresIds,long? MainCatId, long? CatId, long? SubCatId)
        {
            UnitOfWork UWork = new UnitOfWork();

            List<tbl_ItemsStock> List=null;
            if (StoresIds != null)
            {
                List = GetAll().Where(c => (c.Item_Id == ItemId || ItemId == null) && (c.Item_tbl.CatSub_tbl.Category_tbl.CatMain_tbl.CatMain_Id  == MainCatId || MainCatId == null)
                 && (c.Item_tbl.CatSub_tbl.Category_tbl.Cat_Id == CatId || CatId == null)
                   && (c.Item_tbl.CatSub_tbl.CatSub_Id == SubCatId || SubCatId == null)
                && (StoresIds.Contains(c.Room_Id.Value))
                && (c.IsDeleted == false || c.IsDeleted == null)).ToList();
            }

            return List;
        }
        public List<tbl_ItemsStock> GetByLessThanLimit(long? ItemId, long? StoreId, long? MainCatId)
        {
            UnitOfWork UWork = new UnitOfWork();

            List<tbl_ItemsStock> List;

            List = GetAll().Where(c => (c.Item_Id  == ItemId || ItemId == null) &&
            (c.Room_Id == StoreId || StoreId == null) &&
            (c.Item_tbl.CatSub_tbl.Category_tbl.CatMain_tbl.CatMain_Id == MainCatId || MainCatId == null) &&
             (c.ItemQty <= c.Item_Stock_Limit) && (c.Item_Stock_Limit != null) &&
             (c.Item_tbl.CountableFlag == true) &&
             (c.IsDeleted == false || c.IsDeleted == null)).ToList();


            List = GetDetials(List);

            return List;
        }

        public List<tbl_ItemsStock> GetByContractID(int? contractId)
        {


            List<tbl_ItemsStock> List;
           
            List = GetAll().Where(c => (c.IsDeleted == false || c.IsDeleted == null) && c.Item_StateId != DamagedItem && c.ContractId  == contractId ).ToList();
            
            
            List = GetDetials(List);

            //ListFilter = GetAll().Where(c => c.Item_RFID.ToUpper().Contains(param.ToUpper())).ToList ();
            //ListFilter = (from obj in List where obj.Item_RFID.ToUpper().Contains(param.ToUpper()) select obj).ToList();
            //List.Where(obj => DbFunctions.Like(obj.Item_RFID, param));

            return List;
        }
    }
}