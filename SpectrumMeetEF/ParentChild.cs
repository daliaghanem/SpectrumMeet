//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SpectrumMeetEF
{
    using System;
    using System.Collections.Generic;
    
    public partial class ParentChild
    {
        public int ParentChildID { get; set; }
        public bool isPrimary { get; set; }
        public int ChildID { get; set; }
        public int AccountID { get; set; }
    
        public virtual Account Account { get; set; }
        public virtual Child Child { get; set; }
    }
}