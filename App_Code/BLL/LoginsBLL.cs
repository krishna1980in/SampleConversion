// ========================================================================
// Kartris - www.kartris.com
// Copyright 2021 CACTUSOFT

// GNU GENERAL PUBLIC LICENSE v2
// This program is free software distributed under the GPL without any
// warranty.
// www.gnu.org/licenses/gpl-2.0.html

// KARTRIS COMMERCIAL LICENSE
// If a valid license.config issued by Cactusoft is present, the KCL
// overrides the GPL v2.
// www.kartris.com/t-Kartris-Commercial-License.aspx
// ========================================================================
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using System.Data.SqlClient;
using kartrisLoginData;
using kartrisLoginDataTableAdapters;
using CkartrisFormatErrors;
using System.Web.HttpContext;
using CkartrisDataManipulation;
using CkartrisEnumerations;

public class LoginsBLL
{
    private static LoginsTblAdptr _Adptr = null/* TODO Change to default(_) if this is not a reference type */;

    protected static LoginsTblAdptr Adptr
    {
        get
        {
            _Adptr = new LoginsTblAdptr();
            return _Adptr;
        }
    }

    /// <summary>
    ///     ''' Validate admin login
    ///     ''' Check username and password against db
    ///     ''' </summary>
    ///     ''' <param name="UserName">The UserName (login)</param>
    ///     ''' <param name="Password">The password</param>
    ///     ''' <param name="blnDirect">If true, ID and pw are run directly against the db (i.e. assumed password supplied is hashed already)</param>
    ///     ''' <remarks></remarks>
    public static int Validate(string UserName, string Password, bool blnDirect = false)
    {
        if (blnDirect)
            return Adptr._Validate(UserName, Password);
        else
        {
            UsersBLL objUsersBLL = new UsersBLL();
            string strUserSalt = _GetSaltByUserName(UserName);
            bool blnUserValidated = Adptr._Validate(UserName, objUsersBLL.EncryptSHA256Managed(Password, strUserSalt, true));

            KartrisDBBLL._AddAdminLog(HttpContext.Current.Session("_User"), ADMIN_LOG_TABLE.Logins, "Login", "username: " + UserName + " ---  password(hashed): " + objUsersBLL.EncryptSHA256Managed(Password, strUserSalt, true), UserName);

            // LogError("Login info: " & "username: " & UserName & " ---  password(hashed): " & UsersBLL.EncryptSHA256Managed(Password, strUserSalt, True))

            // Password still doesn't use hash salt so update login record
            if (blnUserValidated && string.IsNullOrEmpty(strUserSalt))
            {
                string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
                string strRandomSalt = Membership.GeneratePassword(20, 0);
                using (System.Data.SqlClient.SqlConnection sqlConn = new System.Data.SqlClient.SqlConnection(strConnString))
                {
                    SqlCommand cmdUpdateLogin = sqlConn.CreateCommand();
                    cmdUpdateLogin.CommandText = "_spKartrisLogins_UpdatePassword";

                    System.Data.SqlClient.SqlTransaction savePoint = null;
                    cmdUpdateLogin.CommandType = CommandType.StoredProcedure;

                    try
                    {
                        int ReturnedID;
                        int LOGIN_ID = LoginsBLL._GetIDbyName(UserName);
                        {
                            var withBlock = cmdUpdateLogin;
                            withBlock.Parameters.AddWithValue("@LOGIN_ID", LOGIN_ID);
                            withBlock.Parameters.AddWithValue("@LOGIN_Password", objUsersBLL.EncryptSHA256Managed(Password, strRandomSalt, true));
                            withBlock.Parameters.AddWithValue("@LOGIN_SaltValue", strRandomSalt);
                            sqlConn.Open();
                            savePoint = sqlConn.BeginTransaction();
                            withBlock.Transaction = savePoint;
                            ReturnedID = withBlock.ExecuteScalar();
                        }

                        if (ReturnedID != LOGIN_ID)
                            throw new Exception("Login ID And the Updated ID don't match");

                        KartrisDBBLL._AddAdminLog(HttpContext.Current.Session("_User"), ADMIN_LOG_TABLE.Logins, System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_OperationCompletedSuccessfully"), CreateQuery(cmdUpdateLogin), UserName, sqlConn, savePoint);

                        savePoint.Commit();
                        sqlConn.Close();
                    }
                    catch (Exception ex)
                    {
                        ReportHandledError(ex, System.Reflection.MethodBase.GetCurrentMethod());
                        if (!savePoint == null)
                            savePoint.Rollback();
                    }
                    finally
                    {
                        if (sqlConn.State == ConnectionState.Open)
                            sqlConn.Close();
                    }
                }
            }
            return blnUserValidated;
        }
    }

    /// <summary>
    ///     ''' Get login details
    ///     ''' </summary>
    ///     ''' <param name="UserName">The UserName (login)</param>
    ///     ''' <remarks></remarks>
    public static DataTable GetDetails(string UserName)
    {
        return Adptr.GetData(UserName);
    }

    /// <summary>
    ///     ''' Delete login
    ///     ''' </summary>
    ///     ''' <param name="LOGIN_ID">The db ID of the login record</param>
    ///     ''' <remarks></remarks>
    public static int Delete(int LOGIN_ID)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (System.Data.SqlClient.SqlConnection sqlConn = new System.Data.SqlClient.SqlConnection(strConnString))
        {
            SqlCommand cmdDeleteLogin = sqlConn.CreateCommand();
            cmdDeleteLogin.CommandText = "_spKartrisLogins_Delete";

            System.Data.SqlClient.SqlTransaction savePoint = null;
            cmdDeleteLogin.CommandType = CommandType.StoredProcedure;

            try
            {
                int ReturnedID;
                {
                    var withBlock = cmdDeleteLogin;
                    withBlock.Parameters.AddWithValue("@LOGIN_ID", LOGIN_ID);
                    sqlConn.Open();
                    savePoint = sqlConn.BeginTransaction();
                    withBlock.Transaction = savePoint;
                    ReturnedID = withBlock.ExecuteScalar();
                }

                if (ReturnedID != LOGIN_ID)
                    throw new Exception("Login ID and the Deleted ID don't match");

                KartrisDBBLL._AddAdminLog(HttpContext.Current.Session("_User"), ADMIN_LOG_TABLE.Logins, System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_OperationCompletedSuccessfully"), CreateQuery(cmdDeleteLogin), LOGIN_ID, sqlConn, savePoint);

                savePoint.Commit();
                sqlConn.Close();
                return ReturnedID;
            }
            catch (Exception ex)
            {
                ReportHandledError(ex, System.Reflection.MethodBase.GetCurrentMethod());
                if (!savePoint == null)
                    savePoint.Rollback();
                return 0;
            }
            finally
            {
                if (sqlConn.State == ConnectionState.Open)
                    sqlConn.Close();
            }
        }
    }

    /// <summary>
    ///     ''' Update login
    ///     ''' </summary>
    ///     ''' <param name="LOGIN_ID">The db ID of the login record</param>
    ///     ''' <param name="LOGIN_UserName">The UserName (login)</param>
    ///     ''' <param name="LOGIN_Password">Password (raw)</param>
    ///     ''' <param name="LOGIN_Live">Whether login is activated/live</param>
    ///     ''' <param name="LOGIN_Orders">Whether login can admin orders and customer data</param>
    ///     ''' <param name="LOGIN_Products">Whether login can admin product data</param>
    ///     ''' <param name="LOGIN_Config">Whether login can change config settings and other administration tasks</param>
    ///     ''' <param name="LOGIN_Protected">Default logins are protected, so you cannot delete all logins from a site</param>
    ///     ''' <param name="LOGIN_LanguageID">Language ID for interface for this login</param>
    ///     ''' <param name="LOGIN_EmailAddress">Email address</param>
    ///     ''' <param name="LOGIN_Tickets">Whether login can deal with support tickets</param>
    ///     ''' <param name="LOGIN_Pushnotifications">Key for push notifications on Android or Windows apps</param>
    ///     ''' <remarks></remarks>
    public static void Update(int LOGIN_ID, string LOGIN_UserName, string LOGIN_Password, bool LOGIN_Live, bool LOGIN_Orders, bool LOGIN_Products, bool LOGIN_Config, bool LOGIN_Protected, int LOGIN_LanguageID, string LOGIN_EmailAddress, bool LOGIN_Tickets, string LOGIN_Pushnotifications)
    {
        UsersBLL objUsersBLL = new UsersBLL();
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        string strRandomSalt = Membership.GeneratePassword(20, 0);
        using (System.Data.SqlClient.SqlConnection sqlConn = new System.Data.SqlClient.SqlConnection(strConnString))
        {
            SqlCommand cmdUpdateLogin = sqlConn.CreateCommand();
            cmdUpdateLogin.CommandText = "_spKartrisLogins_Update";

            System.Data.SqlClient.SqlTransaction savePoint = null;
            cmdUpdateLogin.CommandType = CommandType.StoredProcedure;

            try
            {
                int ReturnedID;
                {
                    var withBlock = cmdUpdateLogin;
                    withBlock.Parameters.AddWithValue("@LOGIN_ID", LOGIN_ID);
                    withBlock.Parameters.AddWithValue("@LOGIN_Username", LOGIN_UserName);
                    withBlock.Parameters.AddWithValue("@LOGIN_Password", IIf(string.IsNullOrEmpty(LOGIN_Password), "", objUsersBLL.EncryptSHA256Managed(LOGIN_Password, strRandomSalt, true)));
                    withBlock.Parameters.AddWithValue("@LOGIN_Live", LOGIN_Live);
                    withBlock.Parameters.AddWithValue("@LOGIN_Orders", LOGIN_Orders);
                    withBlock.Parameters.AddWithValue("@LOGIN_Products", LOGIN_Products);
                    withBlock.Parameters.AddWithValue("@LOGIN_Config", LOGIN_Config);
                    withBlock.Parameters.AddWithValue("@LOGIN_Protected", LOGIN_Protected);
                    withBlock.Parameters.AddWithValue("@LOGIN_LanguageID", LOGIN_LanguageID);
                    withBlock.Parameters.AddWithValue("@LOGIN_EmailAddress", LOGIN_EmailAddress);
                    withBlock.Parameters.AddWithValue("@LOGIN_Tickets", LOGIN_Tickets);
                    withBlock.Parameters.AddWithValue("@LOGIN_SaltValue", strRandomSalt);
                    withBlock.Parameters.AddWithValue("@LOGIN_PushNotifications", LOGIN_Pushnotifications);
                    sqlConn.Open();
                    savePoint = sqlConn.BeginTransaction();
                    withBlock.Transaction = savePoint;
                    ReturnedID = withBlock.ExecuteScalar();
                }

                if (ReturnedID != LOGIN_ID)
                    throw new Exception("Login ID and the Updated ID don't match");

                KartrisDBBLL._AddAdminLog(HttpContext.Current.Session("_User"), ADMIN_LOG_TABLE.Logins, System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_OperationCompletedSuccessfully"), CreateQuery(cmdUpdateLogin), LOGIN_UserName, sqlConn, savePoint);

                savePoint.Commit();
                sqlConn.Close();
            }
            catch (Exception ex)
            {
                ReportHandledError(ex, System.Reflection.MethodBase.GetCurrentMethod());
                if (!savePoint == null)
                    savePoint.Rollback();
            }
            finally
            {
                if (sqlConn.State == ConnectionState.Open)
                    sqlConn.Close();
            }
        }
    }

    /// <summary>
    ///     ''' Add login
    ///     ''' </summary>
    ///     ''' <param name="LOGIN_UserName">The UserName (login)</param>
    ///     ''' <param name="LOGIN_Password">Password (raw)</param>
    ///     ''' <param name="LOGIN_Live">Whether login is activated/live</param>
    ///     ''' <param name="LOGIN_Orders">Whether login can admin orders and customer data</param>
    ///     ''' <param name="LOGIN_Products">Whether login can admin product data</param>
    ///     ''' <param name="LOGIN_Config">Whether login can change config settings and other administration tasks</param>
    ///     ''' <param name="LOGIN_Protected">Default logins are protected, so you cannot delete all logins from a site</param>
    ///     ''' <param name="LOGIN_LanguageID">Language ID for interface for this login</param>
    ///     ''' <param name="LOGIN_EmailAddress">Email address</param>
    ///     ''' <param name="LOGIN_Tickets">Whether login can deal with support tickets</param>
    ///     ''' <param name="LOGIN_Pushnotifications">Key for push notifications on Android or Windows apps</param>
    ///     ''' <remarks></remarks>
    public static void Add(string LOGIN_UserName, string LOGIN_Password, bool LOGIN_Live, bool LOGIN_Orders, bool LOGIN_Products, bool LOGIN_Config, bool LOGIN_Protected, int LOGIN_LanguageID, string LOGIN_EmailAddress, bool LOGIN_Tickets, string LOGIN_Pushnotifications)
    {
        UsersBLL objUsersBLL = new UsersBLL();
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        string strRandomSalt = Membership.GeneratePassword(20, 0);
        using (System.Data.SqlClient.SqlConnection sqlConn = new System.Data.SqlClient.SqlConnection(strConnString))
        {
            SqlCommand cmdAddLogin = sqlConn.CreateCommand();
            cmdAddLogin.CommandText = "_spKartrisLogins_Add";

            System.Data.SqlClient.SqlTransaction savePoint = null;
            cmdAddLogin.CommandType = CommandType.StoredProcedure;

            try
            {
                int ReturnedID;
                {
                    var withBlock = cmdAddLogin;
                    withBlock.Parameters.AddWithValue("@LOGIN_Username", LOGIN_UserName);
                    withBlock.Parameters.AddWithValue("@LOGIN_Password", objUsersBLL.EncryptSHA256Managed(LOGIN_Password, strRandomSalt, true));
                    withBlock.Parameters.AddWithValue("@LOGIN_Live", LOGIN_Live);
                    withBlock.Parameters.AddWithValue("@LOGIN_Orders", LOGIN_Orders);
                    withBlock.Parameters.AddWithValue("@LOGIN_Products", LOGIN_Products);
                    withBlock.Parameters.AddWithValue("@LOGIN_Config", LOGIN_Config);
                    withBlock.Parameters.AddWithValue("@LOGIN_Protected", LOGIN_Protected);
                    withBlock.Parameters.AddWithValue("@LOGIN_LanguageID", LOGIN_LanguageID);
                    withBlock.Parameters.AddWithValue("@LOGIN_EmailAddress", LOGIN_EmailAddress);
                    withBlock.Parameters.AddWithValue("@LOGIN_Tickets", LOGIN_Tickets);
                    withBlock.Parameters.AddWithValue("@LOGIN_SaltValue", strRandomSalt);
                    withBlock.Parameters.AddWithValue("@LOGIN_PushNotifications", LOGIN_Pushnotifications);
                    sqlConn.Open();
                    savePoint = sqlConn.BeginTransaction();
                    withBlock.Transaction = savePoint;
                    ReturnedID = withBlock.ExecuteScalar();
                }

                if (ReturnedID == 0)
                    throw new Exception("ID is 0? Something's not right");

                KartrisDBBLL._AddAdminLog(HttpContext.Current.Session("_User"), ADMIN_LOG_TABLE.Logins, System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_OperationCompletedSuccessfully"), CreateQuery(cmdAddLogin), LOGIN_UserName, sqlConn, savePoint);

                savePoint.Commit();
                sqlConn.Close();
            }
            catch (Exception ex)
            {
                ReportHandledError(ex, System.Reflection.MethodBase.GetCurrentMethod());
                if (!savePoint == null)
                    savePoint.Rollback();
            }
            finally
            {
                if (sqlConn.State == ConnectionState.Open)
                    sqlConn.Close();
            }
        }
    }

    /// <summary>
    ///     ''' Find login ID from UserName
    ///     ''' </summary>
    ///     ''' <param name="UserName">The UserName (login)</param>
    ///     ''' <remarks></remarks>
    public static string _GetIDbyName(string UserName)
    {
        return Adptr._GetIDbyName(UserName);
    }

    /// <summary>
    ///     ''' Find logins with support ticket access
    ///     ''' </summary>
    ///     ''' <remarks></remarks>
    public static DataTable _GetSupportUsers()
    {
        return Adptr._GetSupportTicketsUsers();
    }

    /// <summary>
    ///     ''' Find logins with support ticket access (front end version)
    ///     ''' </summary>
    ///     ''' <remarks></remarks>
    public static DataTable GetSupportUsers()
    {
        return Adptr.GetSupportTicketsUsers();
    }

    /// <summary>
    ///     ''' Find a login's salt value (random string used for hashing password)
    ///     ''' </summary>
    ///     ''' <param name="UserName">The UserName (login)</param>
    ///     ''' <remarks></remarks>
    public static string _GetSaltByUserName(string UserName)
    {
        return Adptr._GetSaltByUserName(UserName);
    }

    /// <summary>
    ///     ''' List of all logins
    ///     ''' </summary>
    ///     ''' <remarks></remarks>
    public static DataTable GetLoginsList()
    {
        return Adptr._GetList();
    }
}
