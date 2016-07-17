using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SKHSS.Models
{
    public class RegError
    {
        public String Message = "";
        public String id = "";
        public RegError()
        {

        }
        public RegError(String Message)
        {
            this.Message = Message;
        }
        public RegError(String Message, String id)
        {
            this.Message = Message;
            this.id = id;
        }
    }
}