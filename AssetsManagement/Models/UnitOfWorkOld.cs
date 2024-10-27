using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using DAL;

namespace AssetsManagement
{
    public class UnitOfWorkOld
    {
        private CARACCOUNTWebEntities ctx = new CARACCOUNTWebEntities();

        public CarsManager CarManager
        {

            get
            {
                return new CarsManager(ctx);
            }
        }
        public CarStatusManager StatusManager
        {
            get
            {
                return new CarStatusManager(ctx);
            }
        }
        public CustomersManager Customermanager
        {
            get
            {
                return new CustomersManager(ctx);
            }
        }
        public ModelsManager modelmanager
        {
            get
            {
                return new ModelsManager(ctx);
            }
        }
        public UsersManager UserManager
        {
            get
            {
                return new UsersManager(ctx);
            }
        }
        public PaymentsManager Payementmanager
        {
            get
            {
                return new PaymentsManager(ctx);
            }
        }
        //public NatManager NatManager
        //{
        //    get
        //    {
        //        return new NatManager(ctx);
        //    }
        //}

        //public OfficesManager OfficesManager
        //{
        //    get
        //    {
        //        return new OfficesManager(ctx);
        //    }
        //}
        public bool Save()
        {

            return ctx.SaveChanges() > 0;
        }
    }
}