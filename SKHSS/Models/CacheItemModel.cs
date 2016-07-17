using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SKHSS.Models
{
    public class CacheItemModel
    {
        public String CCI_RecordID { get; set; }
        public String title { get; set; }
        public String content { get; set; }
        public String published { get; set; }
        public List<String> labels { get; set; }
        public CacheItemModel()
        {
        }
    }
}