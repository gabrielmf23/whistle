//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebApplication3.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class NextMatch
    {
        public int Referee { get; set; }
        public int SelectedTeam { get; set; }
        public int AgainstTeam { get; set; }
        public string FieldControl { get; set; }
        public System.DateTime MatchDate { get; set; }
        public int Championship { get; set; }
    
        public virtual Championship Championship1 { get; set; }
        public virtual Team Team { get; set; }
        public virtual Referee Referee1 { get; set; }
        public virtual Team Team1 { get; set; }
    }
}
