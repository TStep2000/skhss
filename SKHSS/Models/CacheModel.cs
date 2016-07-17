using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SKHSS.Models
{
    public class CacheModel
    {
        public String CAC_RecordID { get; set; }
        public String CAC_Type { get; set; }
        public String CAC_Date { get; set; }
        public List<CacheItemModel> items { get; set; }
        public CacheModel()
        {
        }
    }
}