using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DAL;
namespace BOL
{
    public class VendorContractsManager : Repository<Vendor_Contracts>
    {

      

        public VendorContractsManager(GPFAssetsEntities ctx) : base(ctx)
        {


        }
        public override bool Delete(Vendor_Contracts entity)
        {
            return base.Delete(entity);
        }
        //public List <Car> GetStudentByGender(int genderId)
        //{
        //    return GetAll().Where(c => c.genderId == genderId).ToList();
        //}
        public bool update(int id)
        {
            Vendor_Contracts st = GetById(id);
            //st.StudentName = Name;
            return Update(st);
        }
        public List<Vendor_Contracts> GetCastByUnitName(string UnitSearch)
        {
            //stu => stu.CarNo.ToUpper().Contains(Search_Data.ToUpper()) || stu.CarType.ToUpper().Contains(Search_Data.ToUpper()));

            return GetAll().Where(c => c.ContractName.ToUpper().Contains(UnitSearch.ToUpper()) || c.ContractNameEn.ToUpper().Contains(UnitSearch.ToUpper())).ToList();
        }
        public List<Vendor_Contracts> GetNotDelAll()
        {
            return GetAll().Where(c => c.IsDeleted == false || c.IsDeleted == null).ToList();
        }
        public List<Vendor_Contracts> GetNotDelAllByVendorId(long VendorId)
        {
            return GetAll().Where(c=>(c.IsDeleted == false || c.IsDeleted == null) && c.VendorId == VendorId).ToList();
        }
        public List<Vendor_Contracts> GetNotDelAllByContractIdId(int ContractId)
        {
            return GetAll().Where(c => (c.IsDeleted == false || c.IsDeleted == null) && c.ContractId == ContractId).ToList();
        }
    }
}