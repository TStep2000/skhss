using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Text;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using SKHSS.Models;
using System.Data.Entity.Validation;
using SKHSS.Helpers;

namespace SKHSS.Authentication
{
    public class SKHSSMembershipProvider : MembershipProvider
    {

        #region Unimplemented MembershipProvider Methods
        public override string ApplicationName
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            throw new NotImplementedException();
        }
        public override bool EnablePasswordReset
        {
            get { throw new NotImplementedException(); }
        }
        public override bool EnablePasswordRetrieval
        {
            get { throw new NotImplementedException(); }
        }
        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }
        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }
        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }
        public override int GetNumberOfUsersOnline()
        {
            throw new NotImplementedException();
        }
        public override string GetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }
        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            throw new NotImplementedException();
        }
        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            throw new NotImplementedException();
        }
        public override string GetUserNameByEmail(string email)
        {
            throw new NotImplementedException();
        }
        public override int MaxInvalidPasswordAttempts
        {
            get { throw new NotImplementedException(); }
        }
        public override int MinRequiredNonAlphanumericCharacters
        {
            get { throw new NotImplementedException(); }
        }
        public override int MinRequiredPasswordLength
        {
            get { throw new NotImplementedException(); }
        }
        public override int PasswordAttemptWindow
        {
            get { throw new NotImplementedException(); }
        }
        public override MembershipPasswordFormat PasswordFormat
        {
            get { throw new NotImplementedException(); }
        }
        public override string PasswordStrengthRegularExpression
        {
            get { throw new NotImplementedException(); }
        }
        public override bool RequiresQuestionAndAnswer
        {
            get { throw new NotImplementedException(); }
        }
        public override bool RequiresUniqueEmail
        {
            get { throw new NotImplementedException(); }
        }
        public override string ResetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }
        public override bool UnlockUser(string userName)
        {
            throw new NotImplementedException();
        }
        public override void UpdateUser(MembershipUser user)
        {
            throw new NotImplementedException();
        }
        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            throw new NotImplementedException();
        }
        
        public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        {
            throw new NotImplementedException();
        }
        #endregion

        public int ChangePassword(string email, string oldPassword, string newPassword, string confPassword)
        {
            SKHSSUser sku = GetUser(null,email);
            if (sku.Password.Equals(oldPassword) && newPassword.Equals(confPassword))
            {
                UserLogin usr = Data.GetUL(m => m.USR_RecordID == sku.USR_RecordID);
                usr.USR_Password = newPassword;
                Data.SaveDB();
                return 0;
            }
            else if (!sku.Password.Equals(oldPassword))
            {
                return 1;
            }
            else if (!newPassword.Equals(confPassword))
            {
                return 2;
            }
            else
            {
                return 3;
            }
        }
        public SKHSSUser CreateUser(string username, string password, string email, string phone, int roleId)
        {
            if (email != "" && Data.GetLogin(email.ToLower()) != null)
            {
                throw new ArgumentException("An account with that email already exists");
            }
            UserLogin login = new UserLogin();
            login.USR_Username = username;
            login.USR_Password = password;
            login.USR_Email = email;
            login.USR_Phone = phone;
            login.USR_ROL_RoleID = roleId;
            Boolean found = false;
            Int32 id = Data.GetULs().Count();
            while (!found)
            {
                if (Data.GetUL(m => m.USR_UserID == id) == null)
                {
                    found = true;
                    login.USR_UserID = id;
                }
                else
                {
                    id++;
                }
            }
            login.USR_RecordID = Guid.NewGuid();
            SKHSSUser user = new SKHSSUser(login);
            Data.GetULs().Add(user.UserLogin);
            try
            {
                Data.SaveDB();
            }
            catch (DbEntityValidationException e)
            {
                var str = "";
                foreach (var eve in e.EntityValidationErrors)
                {
                    str += "Entity of type " + eve.Entry.Entity.GetType().Name + " in state " + eve.Entry.State + " has the following validation errors:";
                    /*Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);*/
                    foreach (var ve in eve.ValidationErrors)
                    {
                        str += "- Property: " + ve.PropertyName + ", Error: " + ve.ErrorMessage + "--------";
                        /*Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);*/
                    }
                }
                throw;
            }
            return user;
        }
        public SKHSSUser GetUser(string Username, string Email)
        {
            if (Username != null && Username != "")
            {
                try
                {
                    UserLogin user = Data.GetUL(p => p.USR_Username == Username.ToLower());
                    if (user != null)
                    {
                        return new SKHSSUser(user);
                    }
                }
                catch (Exception e)
                {
                    if (e.Source == ".Net SqlClient Data Provider" || e.Source == "System.Data.Entity")
                    {
                        throw e;
                    }
                }
            }
            if (Email != null && Email != "")
            {
                try
                {
                    UserLogin user = Data.GetUL(p => p.USR_Email == Email.ToLower());
                    if (user != null)
                    {
                        return new SKHSSUser(user);
                    }
                    
                }
                catch (Exception e)
                {
                    if (e.Source == ".Net SqlClient Data Provider" || e.Source == "System.Data.Entity")
                    {
                        throw e;
                    }
                }
            }
            return null;
        }
        public SKHSSUser GetUser(Guid USR_RecordID)
        {
            try
            {
                UserLogin user = Data.GetUL(p => p.USR_RecordID == USR_RecordID);
                if (user != null)
                {
                    return new SKHSSUser(user);
                }
            }
            catch 
            {
                return null;
            }
            return null;
        }
        public SKHSSUser GetUser(Guid? USR_RecordID)
        {
            return GetUser((Guid)USR_RecordID);
        }
        public override bool ValidateUser(string email, string password)
        {
            if (string.IsNullOrEmpty(password.Trim())) return false;
            SKHSSUser user = GetUser(null,email);
            if (user == null) return false;
            if (user.Password.CompareTo(password) != 0) return false;
            return false;
        }
        /// <summary>
        /// Procuses an MD5 hash string of the password
        /// </summary>
        /// <param name="password">password to hash</param>
        /// <returns>MD5 Hash string</returns>
        protected string EncryptPassword(string password)
        {
            //we use codepage 1252 because that is what sql server uses
            byte[] pwdBytes = Encoding.GetEncoding(1252).GetBytes(password);
            byte[] hashBytes = System.Security.Cryptography.MD5.Create().ComputeHash(pwdBytes);
            return Encoding.GetEncoding(1252).GetString(hashBytes);

        }
        

    }
}