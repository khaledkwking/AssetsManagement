using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using DAL;

namespace BOL
{
    public class UnitOfWork
    {
        private GPFAssetsEntities ctx = new GPFAssetsEntities();

        private GPFEmployeesEntities ctxHR = new GPFEmployeesEntities();


       
        

        public UsersDeptsManager UsersDeptsManager
        {
            get
            {
                return new UsersDeptsManager(ctx);
            }
        }
        public RequestOutOrdersManager RequestOutOrdersManager
        {
            get
            {
                return new RequestOutOrdersManager(ctx);
            }
        }

        public RequestOrdersDetailsManager RequestOrdersDetailsManager
        {
            get
            {
                return new RequestOrdersDetailsManager(ctx);
            }
        }


        public DepartmentManager DepartmentManager
        {
            get
            {
                return new DepartmentManager(ctxHR);
            }
        }

        public EmployeesManager EmployeesManager
        {
            get
            {
                return new EmployeesManager(ctxHR);
            }
        }

        public UnitsManager UnitsManager
        {
            get
            {
                return new UnitsManager(ctx);
            }
        }

        public UsersManager UsersManager
        {

            get
            {
                return new UsersManager(ctx);
            }
        }

        public BuildingsManager BuildingsManager
        {
            get
            {
                return new BuildingsManager(ctx);
            }
        }
        public FloorsManager FloorsManager
        {
            get
            {
                return new FloorsManager(ctx);
            }
        }

        public RoomsManager RoomsManager
        {
            get
            {
                return new RoomsManager(ctx);
            }
        }
        public CatMainManager CatMainManager
        {
            get
            {
                return new CatMainManager(ctx);
            }
        }

        public CatSubManager CatSubManager
        {
            get
            {
                return new CatSubManager(ctx);
            }
        }

        public CategoryManager CategoryManager
        {
            get
            {
                return new CategoryManager(ctx);
            }
        }

        public ItemsManager ItemsManager
        {
            get
            {
                return new ItemsManager(ctx);
            }
        }
        public ItemsStockManager ItemsStockManager
        {
            get
            {
                return new ItemsStockManager(ctx);
            }
        }

        public SuppliersManager SuppliersManager
        {
            get
            {
                return new SuppliersManager(ctx);
            }
        }


        public StatusManager StatusManager
        {
            get
            {
                return new StatusManager(ctx);
            }
        }

        public EmpRoomsManager EmpRoomsManager
        {
            get
            {
                return new EmpRoomsManager(ctx);
            }
        }

        public OutOrdersManager OutOrdersManager
        {
            get
            {
                return new OutOrdersManager(ctx);
            }
        }

        public OutOrdersDetailsManager OutOrdersDetailsManager
        {
            get
            {
                return new OutOrdersDetailsManager(ctx);
            }
        }
        public InOrdersDetailsManager InOrdersDetailsManager
        {
            get
            {
                return new InOrdersDetailsManager(ctx);
            }
        }

        public InOrdersManager InOrdersManager
        {
            get
            {
                return new InOrdersManager(ctx);
            }
        }
        public DestroyOrdersManager DestroyOrdersManager
        {
            get
            {
                return new DestroyOrdersManager(ctx);
            }
        }
        public DestroyOrdersDetailsManager DestroyOrdersDetailsManager
        {
            get
            {
                return new DestroyOrdersDetailsManager(ctx);
            }
        }
        public HandoverOrdersManager HandoverOrdersManager
        {
            get
            {
                return new HandoverOrdersManager(ctx);
            }
        }
        public HandOverOrdersDetailsManager HandOverOrdersDetailsManager
        {
            get
            {
                return new HandOverOrdersDetailsManager(ctx);
            }
        }
        public tbLookupsManager tbLookupsManager
        {
            get
            {
                return new tbLookupsManager(ctx);
            }
        }
        public TransferOrdersManager TransferOrdersManager
        {
            get
            {
                return new TransferOrdersManager(ctx);
            }
        }

        public TransferOrdersDetailsManager TransferOrdersDetailsManager
        {
            get
            {
                return new TransferOrdersDetailsManager(ctx);
            }
        }

        public ChangeQuantityOrdersDetailsManager ChangeQuantityOrdersDetailsManager
        {
            get
            {
                return new ChangeQuantityOrdersDetailsManager(ctx);
            }
        }

        public ChangeQuantityOrdersManager ChangeQuantityOrdersManager
        {
            get
            {
                return new ChangeQuantityOrdersManager(ctx);
            }
        }
        public ReturnOutOrdersDetailsManager ReturnOutOrdersDetailsManager
        {
            get
            {
                return new ReturnOutOrdersDetailsManager(ctx);
            }
        }

        public ReturnOutOrdersManager ReturnOutOrdersManager
        {
            get
            {
                return new ReturnOutOrdersManager(ctx);
            }
        }
        public ReturnInOrdersDetailsManager ReturnInOrdersDetailsManager
        {
            get
            {
                return new ReturnInOrdersDetailsManager(ctx);
            }
        }

        public ReturnInOrdersManager ReturnInOrdersManager
        {
            get
            {
                return new ReturnInOrdersManager(ctx);
            }
        }

        public SystemPagesManager SystemPagesManager
        {
            get
            {
                return new SystemPagesManager(ctx);
            }
        }

        public UsersPagesManager UsersPagesManager
        {
            get
            {
                return new UsersPagesManager(ctx);
            }
        }
        public UsersRolesManager UsersRolesManager
        {
            get
            {
                return new UsersRolesManager(ctx);
            }
        }
        public VwOutOrderDetailsManager VwOutOrderDetailsManager
        {
            get
            {
                return new VwOutOrderDetailsManager(ctx);
            }
        }
        public VwInOrderDetailsManager VwInOrderDetailsManager
        {
            get
            {
                return new VwInOrderDetailsManager(ctx);
            }
        }
        public vwDestroyOrdersDetailsManager vwDestroyOrdersDetailsManager
        {
            get
            {
                return new vwDestroyOrdersDetailsManager(ctx);
            }
        }
        public vwHandOverOrdersDetailsManager vwHandOverOrdersDetailsManager
        {
            get
            {
                return new vwHandOverOrdersDetailsManager(ctx);
            }
        }
        public vwChangeQuantityOrdersDetailsManager vwChangeQuantityOrdersDetailsManager
        {
            get
            {
                return new vwChangeQuantityOrdersDetailsManager(ctx);
            }
        }

        public vwTransferOrdersDetailsManager vwTransferOrdersDetailsManager
        {
            get
            {
                return new vwTransferOrdersDetailsManager(ctx);
            }
        }
        public vwAllOrdersManager vwAllOrdersManager
        {
            get
            {
                return new vwAllOrdersManager(ctx);
            }
        }
        public vwReturnOutOrdersDetailsManager vwReturnOutOrdersDetailsManager
        {
            get
            {
                return new vwReturnOutOrdersDetailsManager(ctx);
            }
        }
        public vwReturnInOrdersDetailsManager vwReturnInOrdersDetailsManager
        {
            get
            {
                return new vwReturnInOrdersDetailsManager(ctx);
            }
        }
        public UsersStoresManager UsersStoresManager
        {
            get
            {
                return new UsersStoresManager(ctx);
            }
        }
        public vwItemsStockManager vwItemsStockManager
        {
            get
            {
                return new vwItemsStockManager(ctx);
            }
        }
        public vwEmpRoomsManager vwEmpRoomsManager
        {
            get
            {
                return new vwEmpRoomsManager(ctx);
            }
        }
        public VendorContractsManager VendorContractsManager
        {
            get
            {
                return new VendorContractsManager(ctx);
            }
        }
        
        public ItemsStockDetailsManager ItemsStockDetailsManager
        {
            get
            {
                return new ItemsStockDetailsManager(ctx);
            }
        }
        public VendorsManager VendorsManager
        {
            get
            {
                return new VendorsManager(ctx);
            }
        }
        public TransferAssetsManager TransferAssetsManager
        {
            get
            {
                return new TransferAssetsManager(ctx);
            }
        }

        public TransferAssetsDetailsManager TransferAssetsDetailsManager
        {
            get
            {
                return new TransferAssetsDetailsManager(ctx);
            }
        }
        public vwTransferAssestsManager vwTransferAssestsManager
        {
            get
            {
                return new vwTransferAssestsManager(ctx);
            }
        }
        public SetupManager SetupManager
        {
            get
            {
                return new SetupManager(ctx);
            }
        }
        public UsersMainCatsManager UsersMainCatsManager
        {
            get
            {
                return new UsersMainCatsManager(ctx);
            }
        }
        
        public bool Save()
        {

            return ctx.SaveChanges() > 0;
        }
    }

}
public class CheckBoxListItem
{
    public long Id { get; set; }
    public long ItemId { get; set; }
    public string Name { get; set; }
    public bool IsSelected { get; set; }
}



