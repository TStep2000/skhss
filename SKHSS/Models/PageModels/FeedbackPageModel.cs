using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SKHSS.Models.PageModels
{
    public class FeedbackPageModel
    {
        public String Name { get; set; }
        public String Email { get; set; }
        public String Message { get; set; }

        public String Role { get; set; }
        public String Browser { get; set; }
        public String BrowserString { get; set; }
        public String OS { get; set; }
        public FeedbackPageModel()
        {
            Name = "";
            Message = "";
            Role = "";
            Browser = "";
            OS = "";
        }
    }
}