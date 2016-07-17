using SKHSS.Models;
using SKHSS.Models.PageModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Serialization;

namespace SKHSS.Helpers
{
    public class Validation
    {
        private static JavaScriptSerializer js = new JavaScriptSerializer(new SimpleTypeResolver());
        #region objectvalidation
        public static Dictionary<String, List<RegError>> ValidateObject(ObjectModel OM, String id)
        {
            Dictionary<String, List<RegError>> Errors = new Dictionary<String, List<RegError>>();
            String usrchk = @"^[a-zA-Z0-9_-]{5,15}$";
            if (OM.Username != null && OM.Username == "")
            {
                AddError(ref Errors, "Username", "req", id);
            }
            else if (OM.Username != null && !Regex.IsMatch(OM.Username, usrchk))
            {
                AddError(ref Errors, "Username", "Username must be between 5-15 characters, and have no special characters.", id);
            }
            if (Data.GetLogin(OM.Username) != null && !(OM.UserRecordID != null && Data.GetLogin(OM.Username).USR_RecordID == Guid.Parse(OM.UserRecordID)))
            {
                AddError(ref Errors, "Username", "Username is already registered", id);
            }

            String emailchk = @"[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}";
            /*if ((OM.Email != null && OM.Email != "" && !Regex.IsMatch(OM.Email, emailchk)))
            {
                AddError(ref Errors, "Email", "Please enter a valid email address. (e.g. username@domain.com)", id);
            }

            if (Data.GetLogin(OM.Email) != null && !(OM.UserRecordID != null && Data.GetLogin(OM.Email).USR_RecordID == Guid.Parse(OM.UserRecordID)))
            {
                AddError(ref Errors, "Email", "Email is already registered", id);
            }*/

            String passchk = @".{5,15}";
            if (OM.Password != null && OM.Password == "")
            {
                AddError(ref Errors, "Password", "req", id);
            }
            else if (OM.Password != null && !Regex.IsMatch(OM.Password, passchk))
            {
                AddError(ref Errors, "Password", "Password must be between 5-15 characters", id);
            }
            /*if (OM.Phone != null && OM.Phone == "")
            {
                AddError(ref Errors, "Phone", "req", id);
            }
            else if (OM.Phone != null)
            {
                String rs = "";
                for (int i = 0; i < OM.Phone.Length; i++)
                {
                    if (Char.IsDigit(OM.Phone[i]))
                        rs += OM.Phone[i];
                }
                OM.Phone = rs;
                if (OM.Phone.Length != 10)
                {
                    AddError(ref Errors, "Phone", "Phone # should be 10 numbers including area code", id);
                }
            }*/
            if (OM.Parent != null)
            {
                Dictionary<String, List<RegError>> Others = ValidateParentObject(OM.Parent, id);
                Errors = Errors.Concat(Others).ToDictionary(m => m.Key, m => m.Value);
            }
            return Errors;
        }
        public static Dictionary<String, List<RegError>> ValidateParentObject(ObjectModelParent OM, String id)
        {
            Dictionary<String, List<RegError>> Errors = new Dictionary<String, List<RegError>>();
            if (OM.FatherName != null && OM.FatherName == "" && OM.MotherName != null && OM.MotherName == "")
            {
                AddError(ref Errors, "FatherName", "At least one parent name is required", id);
                AddError(ref Errors, "MotherName", "", id);
            }
            if (OM.FatherEmail != null && OM.FatherEmail == "" && OM.MotherEmail != null && OM.MotherEmail == "")
            {
                AddError(ref Errors, "FatherEmail", "At least one contact email is required", id);
                AddError(ref Errors, "MotherEmail", "", id);
            }
            if (OM.PrimaryEmail != null)
            {
                if (OM.PrimaryEmail == "True" && (OM.MotherEmail == null || OM.MotherEmail == ""))
                {
                    AddError(ref Errors, "MotherEmail", "Primary Email is required", id);
                }
                if (OM.PrimaryEmail == "False" && (OM.FatherEmail == null || OM.FatherEmail == ""))
                {
                    AddError(ref Errors, "FatherEmail", "Primary Email is required", id);
                }
            }
            if (OM.FatherPhone != null && OM.FatherPhone == "" && OM.MotherPhone != null && OM.MotherPhone == "")
            {
                AddError(ref Errors, "FatherPhone", "At least one contact phone # is required", id);
                AddError(ref Errors, "MotherPhone", "", id);
            }
            if (OM.PrimaryPhone != null)
            {
                if (OM.PrimaryPhone == "True" && (OM.MotherPhone == null || OM.MotherPhone == ""))
                {
                    AddError(ref Errors, "MotherPhone", "Primary Phone # is required", id);
                }
                if (OM.PrimaryPhone == "False" && (OM.FatherPhone == null || OM.FatherPhone == ""))
                {
                    AddError(ref Errors, "FatherPhone", "Primary Phone # is required", id);
                }
            }
            if (OM.LastName != null && OM.LastName == "")
            {
                AddError(ref Errors, "LastName", "req", id);
            }
            if (OM.Address != null && OM.Address == "")
            {
                AddError(ref Errors, "Address", "req", id);
            }
            if (OM.City != null && OM.City == "")
            {
                AddError(ref Errors, "City", "req", id);
            }
            Int32 outt = 0;
            if (OM.Zipcode != null && OM.Zipcode == "")
            {
                AddError(ref Errors, "Zipcode", "req", id);
            }
            else if (OM.Zipcode != null && (!Int32.TryParse(OM.Zipcode, out outt) || OM.Zipcode.Length != 5))
            {
                AddError(ref Errors, "Zipcode", "Zipcode must be 5 numbers", id);
            }
            if (OM.Children != null)
            {
                for (int i = 0; i < OM.Children.Count; i++)
                {
                    Dictionary<String, List<RegError>> Others = ValidateChildObject(OM.Children[i], id, "c" + i);
                    Errors = Errors.Concat(Others).ToDictionary(m => m.Key, m => m.Value);
                }
            }
            return Errors;
        }
        public static Dictionary<String, List<RegError>> ValidateOrderObject(ObjectModelOrder OM, String id)
        {
            Dictionary<String, List<RegError>> Errors = new Dictionary<String, List<RegError>>();
            if (OM.Payments != null)
            {
                for (int i = 0; i < OM.Payments.Count; i++)
                {
                    Dictionary<String, List<RegError>> Others = ValidatePaymentObject(OM.Payments[i], id);
                    Errors = Errors.Concat(Others).ToDictionary(m => m.Key, m => m.Value);
                }
            }
            return Errors;
        }
        public static Dictionary<String, List<RegError>> ValidatePaymentObject(ObjectModelPayment OM, String id)
        {
            Dictionary<String, List<RegError>> Errors = new Dictionary<String, List<RegError>>();
            return Errors;
        }
        public static Dictionary<String, List<RegError>> ValidateChildObject(ObjectModelChild OM, String id, String lvlid = "")
        {
            Dictionary<String, List<RegError>> Errors = new Dictionary<String, List<RegError>>();
            if (OM.FirstName != null && OM.FirstName == "")
            {
                AddError(ref Errors, lvlid+"FirstName", "req", id);
            }
            if (OM.Birthdate != null && OM.Birthdate == "")
            {
                AddError(ref Errors, lvlid + "Birthdate", "req", id);
            }
            if (OM.Birthdate != null && !_BirthdateIsValid(OM.Birthdate))
            {
                AddError(ref Errors, lvlid + "Birthdate", "Please enter birthdate in MM/DD/YYYY format", id);
            }
            if (OM.Gender != null && OM.Gender == "")
            {
                AddError(ref Errors, lvlid + "GenderContainer", "req", id);
            }
            if (OM.Teammates != null)
            {
                for (int i = 0; i < OM.Teammates.Count; i++)
                {
                    Dictionary<String, List<RegError>> Others = ValidateTeammateObject(OM.Teammates[i], id, lvlid + "t" + i);
                    Errors = Errors.Concat(Others).ToDictionary(m => m.Key, m => m.Value);
                }
            }
            return Errors;
        }
        public static Dictionary<String, List<RegError>> ValidateTeammateObject(ObjectModelTeammate OM, String id, String lvlid = "")
        {
            Dictionary<String, List<RegError>> Errors = new Dictionary<String, List<RegError>>();

            return Errors;
        }
        public static Dictionary<String, List<RegError>> ValidateCoachObject(ObjectModelCoach OM, String id)
        {
            Dictionary<String, List<RegError>> Errors = new Dictionary<String, List<RegError>>();

            return Errors;
        }
        public static Dictionary<String, List<RegError>> ValidateRegObject(RegisterPageModel OM, String id)
        {
            Dictionary<String, List<RegError>> Errors = new Dictionary<String, List<RegError>>();

            return Errors;
        }
        private static void AddError(ref Dictionary<String, List<RegError>> Errors, String key, String message, String id)
        {
            if (Errors.ContainsKey(key))
            {
                Errors[key].Add(new RegError(message, id));
            }
            else
            {
                Errors.Add(key, new List<RegError> { new RegError(message, id) });
            }
        }
        #endregion
        #region validationrules
        public static Boolean _BirthdateIsValid(String Birthdate)
        {
            DateTime b = new DateTime();
            if (DateTime.TryParse(Birthdate, out b))
            {
                return true;
            }
            return false;
        }
        /*public Boolean BirthdateIsValid(String Birthdate)
        {
            return _BirthdateIsValid(Birthdate);
        }*/
        #endregion
    }
}