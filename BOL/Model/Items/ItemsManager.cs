using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DAL;

    public class ItemsManager : Repository <Item_tbl>
    {


    public ItemsManager(GPFAssetsEntities ctx) : base(ctx)
        {

        }

        public override bool Delete(Item_tbl entity)
        {
            return base.Delete(entity);
        }
        //public List <Car> GetStudentByGender(int genderId)
        //{
        //    return GetAll().Where(c => c.genderId == genderId).ToList();
        //}
        public bool update(int id, string Name, string address, int genderId)
        {
             Item_tbl st = GetById(id);
            //st.StudentName = Name;
            return Update(st);
        }
    public List<Item_tbl> GetCastByUnitName(string UnitSearch)
    {
        //stu => stu.CarNo.ToUpper().Contains(Search_Data.ToUpper()) || stu.CarType.ToUpper().Contains(Search_Data.ToUpper()));

        return GetAll().Where(c => c.Item_Name .ToUpper().Contains(UnitSearch.ToUpper()) || c.Item_NameEn.ToUpper().Contains(UnitSearch.ToUpper())).ToList();
    }
    public List<Item_tbl> GetNotDelAll()
    {
        return GetAll().Where(c => c.IsDeleted == false || c.IsDeleted == null).ToList();
    }
    public List<Item_tbl> GetNotDelAllByType() // get countable items
    {
        return GetAll().Where( c=>(c.CountableFlag == true) && (c.IsDeleted == false || c.IsDeleted == null)  ).ToList();
    }
    // display item with generate Barcode
    public List<Item_tbl> GetNotDelAllNewBarcode() // get countable items
    {
        return GetAll().Where(c => (c.CountableFlag == true) && (c.CatSub_tbl.GenerateBarcodeFlag == true) && (c.IsDeleted == false || c.IsDeleted == null)).ToList();
    }
    public List<Item_tbl> GetNotDelAllByMainCat(List<CatMain_tbl> MainCats)
    {
        var listOfMainCatsId = MainCats.Select(r => r.CatMain_Id).ToList();
        //stu => stu.CarNo.ToUpper().Contains(Search_Data.ToUpper()) || stu.CarType.ToUpper().Contains(Search_Data.ToUpper()));
        List<Item_tbl> NewList;

        NewList = GetAll().Where(c => listOfMainCatsId.Contains(c.CatSub_tbl.Category_tbl.CatMain_tbl.CatMain_Id) && c.OutFlag==true).ToList ();


        return NewList; //GetAll().Where(c => (c.CatSub_tbl.Category_tbl.CatMain_tbl.CatMain_Id == MainCatId || MainCatId == null)).ToList ();

    }
}
