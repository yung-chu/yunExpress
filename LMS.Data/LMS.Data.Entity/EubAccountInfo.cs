//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace LMS.Data.Entity
{
    #pragma warning disable 1573
    using System;
    using System.Collections.Generic;
    using LighTake.Infrastructure.Seedwork;
    
    public partial class EubAccountInfo : Entity
    {
        public virtual int EubAccountId { get; set; }
        public virtual string Account { get; set; }
        public virtual int ShippingMethodId { get; set; }
        public virtual string AuthorizationCode { get; set; }
        public virtual string CountryCode { get; set; }
        public virtual string State { get; set; }
        public virtual string City { get; set; }
        public virtual string ZipCode { get; set; }
        public virtual string Phone { get; set; }
        public virtual string CompanyName { get; set; }
        public virtual string Address { get; set; }
        public virtual string Email { get; set; }
        public virtual int Status { get; set; }
        public virtual System.DateTime CreatedOn { get; set; }
        public virtual string CreatedBy { get; set; }
        public virtual System.DateTime LastUpdatedOn { get; set; }
        public virtual string LastUpdatedBy { get; set; }
        public virtual string ServerUrl { get; set; }
        public virtual string Name { get; set; }
        public virtual string Mobile { get; set; }
        public virtual string County { get; set; }
    }
}
