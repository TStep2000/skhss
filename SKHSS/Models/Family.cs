using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SKHSS.Helpers;

namespace SKHSS.Models
{
    public class Family
    {
        
        private SKHSSEntities db = new SKHSSEntities();
        public Parent Parent = new Parent();
        public UserLogin ParentLogin = new UserLogin();
        public List<Child> Children = new List<Child>();
        public List<Teammate> CurrentRegistered = new List<Teammate>();
        public Order CurrentOrder = new Order();
        public Family(Guid id)
        {
            this.Parent = db.Parents.Single(m => m.PRT_RecordID == id); ;
            this.ParentLogin = db.UserLogins.Single(m => m.USR_RecordID == Parent.PRT_USR_RecordID);
            this.Children = db.Children.Where(m => m.CLD_PRT_RecordID == Parent.PRT_RecordID).ToList();
            this.CurrentRegistered = db.Teammates.Where(m => m.Child.CLD_PRT_RecordID == Parent.PRT_RecordID && m.TMT_Year == Global.CurrentYear && m.TMT_SeasonID == Global.CurrentSeasonID).OrderBy(m=>m.Child.CLD_Birthdate).ToList();
            this.CurrentOrder = db.Orders.SingleOrDefault(m => m.ORD_PRT_RecordID == Parent.PRT_RecordID && m.ORD_Year == Global.CurrentYear && m.ORD_SeasonID == Global.CurrentSeasonID);
        }
        public Family(Parent Parent)
        {
            this.Parent = Parent;
            this.ParentLogin = db.UserLogins.Single(m => m.USR_RecordID == Parent.PRT_USR_RecordID);
            this.Children = db.Children.Where(m => m.CLD_PRT_RecordID == Parent.PRT_RecordID).ToList();
            this.CurrentRegistered = db.Teammates.Where(m => m.Child.CLD_PRT_RecordID == Parent.PRT_RecordID && m.TMT_Year == Global.CurrentYear && m.TMT_SeasonID == Global.CurrentSeasonID).OrderBy(m => m.Child.CLD_Birthdate).ToList();
            this.CurrentOrder = db.Orders.SingleOrDefault(m => m.ORD_PRT_RecordID == Parent.PRT_RecordID && m.ORD_Year == Global.CurrentYear && m.ORD_SeasonID == Global.CurrentSeasonID);
        }
        public Family(List<Teammate> CurrentRegistered)
        {
            this.Parent = CurrentRegistered.First().Child.Parent;
            this.ParentLogin = db.UserLogins.Single(m => m.USR_RecordID == Parent.PRT_USR_RecordID);
            this.Children = db.Children.Where(m => m.CLD_PRT_RecordID == Parent.PRT_RecordID).ToList();
            this.CurrentRegistered = CurrentRegistered.OrderBy(m => m.Child.CLD_Birthdate).ToList();
            this.CurrentOrder = db.Orders.SingleOrDefault(m => m.ORD_PRT_RecordID == Parent.PRT_RecordID && m.ORD_Year == Global.CurrentYear && m.ORD_SeasonID == Global.CurrentSeasonID);
        }
    }
}