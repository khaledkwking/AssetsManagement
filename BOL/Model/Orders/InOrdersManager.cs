using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DAL;
namespace BOL
{
    public class InOrdersManager : Repository<InOrders>
    {

        public InOrdersManager(GPFAssetsEntities ctx) : base(ctx)
        {

        }
        public override bool Delete(InOrders entity)
        {
            return base.Delete(entity);
        }
        //public List <Car> GetStudentByGender(int genderId)
        //{
        //    return GetAll().Where(c => c.genderId == genderId).ToList();
        //}
        public InOrders GetByOrderId(long Id)
        {
            UnitOfWork UWork = new UnitOfWork();
           
            InOrders st = GetById(Id);
            st.InOrdersDetails=st.InOrdersDetails.Where(c => c.IsDeleted == false || c.IsDeleted == null).ToList();
            
            st.ReturnInOrders = st.ReturnInOrders.Where(c => c.IsDeleted == false || c.IsDeleted == null).ToList();
            
            if (st != null)
            {
                if (st.IsDeleted == false || st.IsDeleted == null)
                {
                    //if (!st.IsDeleted.GetValueOrDefault())
                    //{
                        if (st.StoreId_To != null)
                        {
                            long StoreId_To = st.StoreId_To.GetValueOrDefault();
                            Room_tbl CurRoomList;
                            CurRoomList = UWork.RoomsManager.GetById(StoreId_To);
                            st.ToStore = CurRoomList;
                        }
                    //}
                }
            }
            return st;

        }
        public bool update(int id)
        {
            InOrders st = GetById(id);

            return Update(st);
        }
        public List<InOrders> GetCastByName(string UnitSearch)
        {
            //stu => stu.CarNo.ToUpper().Contains(Search_Data.ToUpper()) || stu.CarType.ToUpper().Contains(Search_Data.ToUpper()));

            return GetAll().Where(c => c.Remarks.ToUpper().Contains(UnitSearch.ToUpper()) ||
            c.InOrderId.ToString().ToUpper().Contains(UnitSearch.ToUpper())).ToList();
        }
        public List<InOrders> GetNotDelAll()
        {
            UnitOfWork UWork = new UnitOfWork();
            List<Room_tbl> InventoriesList = UWork.RoomsManager.GetInventoriesAll().ToList();

            List<InOrders> List;

            List = GetAll().Where(c => c.IsDeleted == false || c.IsDeleted == null).ToList();


            foreach (var Item in List)
            {
                if (Item.StoreId_From != null)
                {
                    long StoreId_From = Item.StoreId_From.GetValueOrDefault();
                    Room_tbl CurList = new Room_tbl();
                    CurList = InventoriesList.Where(c => c.Room_Id == StoreId_From).FirstOrDefault();
                    Item.FromStore = CurList;
                }
                if (Item.StoreId_To != null)
                {
                    long StoreId_To = Item.StoreId_To.GetValueOrDefault();
                    Room_tbl CurRoomList = new Room_tbl();
                    CurRoomList = InventoriesList.Where(c => c.Room_Id == StoreId_To).FirstOrDefault();
                    Item.ToStore = CurRoomList;
                }
            }
            return List;
            //return GetAll().Where(c => c.IsDeleted == false || c.IsDeleted == null).ToList();
        }

        public List<InOrders> GetNotDelAllByUserId(int userId)
        {
            UnitOfWork UWork = new UnitOfWork();
            List<Room_tbl> InventoriesList = UWork.RoomsManager.GetInventoriesAll().ToList();

            List<InOrders> List;
            List<Room_tbl> Inventories = UWork.RoomsManager.GetUserInventories(userId).ToList();
            var listOfRoomId = Inventories.Select(r => r.Room_Id).ToList();
            
            List = GetAll().Where(c => c.IsDeleted == false || c.IsDeleted == null).ToList();
           

            List<InOrders> NewList = List.Where(c => listOfRoomId.Contains(c.StoreId_To.GetValueOrDefault())).ToList();

            foreach (var Item in NewList)
            {
                if (Item.StoreId_From != null)
                {
                    long StoreId_From = Item.StoreId_From.GetValueOrDefault();
                    Room_tbl CurList = new Room_tbl();
                    CurList = InventoriesList.Where(c => c.Room_Id == StoreId_From).FirstOrDefault();
                    Item.FromStore = CurList;
                }
                if (Item.StoreId_To != null)
                {
                    long StoreId_To = Item.StoreId_To.GetValueOrDefault();
                    Room_tbl CurRoomList = new Room_tbl();
                    CurRoomList = InventoriesList.Where(c => c.Room_Id == StoreId_To).FirstOrDefault();
                    Item.ToStore = CurRoomList;
                }


                Item.ReturnInOrders = Item.ReturnInOrders.Where(m => m.IsDeleted == false || m.IsDeleted == null).ToList();
            }
            return NewList;
            //return GetAll().Where(c => c.IsDeleted == false || c.IsDeleted == null).ToList();
        }
    }
}