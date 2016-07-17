using SKHSS.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SKHSS.Models.PageModels
{
    public class RegisterPageChild
    {
        public Boolean Reg { get; set; }
        public Boolean Uni { get; set; }
        public Int32 TeamSelect { get; set; }
        public Int32 UniSize { get; set; }
        public Child Child { get; set; }
        public RegisterPageChild()
        {
            Reg = false;
            Uni = false;
            TeamSelect = -1;
            UniSize = 0;
            Child = new Child();
        }
        public RegisterPageChild(Child Child)
        {
            this.Child = Child;
            Teammate t = Data.GetCurrentTeammate(Child);
            Reg = (t != null);
            if (Reg)
            {
                Uni = t.TMT_ShirtID.HasValue;
                UniSize = (Uni ? t.TMT_ShirtID.Value : -1);
                TeamSelect = t.TMT_TEM_TeamID;
            }
            else
            {
                Uni = false;
                UniSize = -1;
                TeamSelect = -1;
            }
        }
    }
}