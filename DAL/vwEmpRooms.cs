
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


namespace DAL
{

using System;
    using System.Collections.Generic;
    
public partial class vwEmpRooms
{

    public long Room_Id { get; set; }

    public int Floor_Id { get; set; }

    public string Room_Name { get; set; }

    public string Room_Code { get; set; }

    public string Room_NameEn { get; set; }

    public string Room_BarCode { get; set; }

    public string Room_RFID { get; set; }

    public Nullable<int> DeptQuota { get; set; }

    public bool StoreFlag { get; set; }

    public Nullable<int> DeptId { get; set; }

    public Nullable<int> EmpId { get; set; }

    public int Building_Id { get; set; }

    public string Building_Name { get; set; }

    public string Building_NameEn { get; set; }

    public string Floor_Name { get; set; }

    public string Floor_NameEn { get; set; }

    public Nullable<int> RoomTypeId { get; set; }

    public string roomTypeDesc { get; set; }

}

}
