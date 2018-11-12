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
    
    public partial class Matches
    {
        public int Referee { get; set; }
        public int HomeTeam { get; set; }
        public int AwayTeam { get; set; }
        public string Result { get; set; }
        public int YC_Home { get; set; }
        public int RC_Home { get; set; }
        public int YC_Away { get; set; }
        public int RC_Away { get; set; }
        public System.DateTime MatchDate { get; set; }
        public int Championship { get; set; }
        public int Home_Goals_Period_1 { get; set; }
        public int Home_Goals_Period_2 { get; set; }
        public int Home_Goals_Total { get; set; }
        public int Away_Goals_Period_1 { get; set; }
        public int Away_Goals_Period_2 { get; set; }
        public int Away_Goals_Total { get; set; }
        public int Home_Yellow_Card1_Period_1 { get; set; }
        public int Home_Yellow_Card1_Period_2 { get; set; }
        public int Home_Yellow_Card1_Total { get; set; }
        public int Home_Yellow_Card2_Period_1 { get; set; }
        public int Home_Yellow_Card2_Period_2 { get; set; }
        public int Home_Yellow_Card2_Total { get; set; }
        public int Home_Red_Card_Period_1 { get; set; }
        public int Home_Red_Card_Period_2 { get; set; }
        public int Home_Red_Card_Total { get; set; }
        public int Away_Yellow_Card1_Period_1 { get; set; }
        public int Away_Yellow_Card1_Period_2 { get; set; }
        public int Away_Yellow_Card1_Total { get; set; }
        public int Away_Yellow_Card2_Period_1 { get; set; }
        public int Away_Yellow_Card2_Period_2 { get; set; }
        public int Away_Yellow_Card2_Total { get; set; }
        public int Away_Red_Card_Period_1 { get; set; }
        public int Away_Red_Card_Period_2 { get; set; }
        public int Away_Red_Card_Total { get; set; }
    
        public virtual Championship Championship1 { get; set; }
        public virtual Team Team { get; set; }
        public virtual Team Team1 { get; set; }
        public virtual Referee Referee1 { get; set; }
    }
}
