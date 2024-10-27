using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DAL.EmpsTableAdapters;
namespace BOL
{
    [System.ComponentModel.DataObject()]
    public partial class Emps
    {
        /// <summary>
        /// Declaration to make a connection in between the Dataset and the class 
        /// </summary>
        #region "Declaration"
        private EmpsTableAdapter _EmpsTableAdapters = null;
        protected EmpsTableAdapter EmpsTableAdapter
        {
            get
            {
                if (_EmpsTableAdapters == null)
                {
                    _EmpsTableAdapters = new EmpsTableAdapter();
                }
                return _EmpsTableAdapters;
            }
        }
        #endregion


        /// <summary>
        /// Read Section is for connect between the dataset and the user controls to execute the select statements 
        /// </summary>
        #region "Read Section"
        public DAL.Emps.EmpsDataTable  Read(string ActionCode, long id)
        {
            return EmpsTableAdapter.Read(ActionCode, id);
        }
        #endregion


        /// <summary>
        /// DML Section is for connect between the dataset and the user controls  execute the insert,update and delete statements 
        /// </summary>
        #region "DML Section"
        public long DML(string ActionCode, long id, string Name, string Address, string Tel, string CivilId, int Gender, int QualificationId, int CountryId)
        {
            long Result = 0;
            Result = Convert.ToInt64((EmpsTableAdapter.DML(ActionCode, id, Name, Address, Tel,CivilId,Gender,QualificationId,CountryId)));
            return Result;
        }
        #endregion
    }
}
