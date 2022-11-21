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
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

using System.Web.HttpContext;
using kartrisUserData;
using kartrisUserDataTableAdapters;
using CkartrisEnumerations;
using CkartrisFormatErrors;

public class UsersBLL
{

    // Private _Detailsadptr As UserDetailsTblAdptr = Nothing
    private CustomerDetailsTblAdptr _CustomerDetailsAdptr = null/* TODO Change to default(_) if this is not a reference type */;
    private AddressesTblAdptr _AddressesAdptr = null/* TODO Change to default(_) if this is not a reference type */;
    private CustomerGroupsTblAdptr _CustomerGroupsAdptr = null/* TODO Change to default(_) if this is not a reference type */;
    private SuppliersTblAdptr _SuppliersAdptr = null/* TODO Change to default(_) if this is not a reference type */;
    private UsersTicketsDetailsTblAdptr _UsersTicketsAdptr = null/* TODO Change to default(_) if this is not a reference type */;

    protected CustomerGroupsTblAdptr CustomerGroupsAdptr
    {
        get
        {
            _CustomerGroupsAdptr = new CustomerGroupsTblAdptr();
            return _CustomerGroupsAdptr;
        }
    }
    protected CustomerDetailsTblAdptr CustomerDetailsAdptr
    {
        get
        {
            _CustomerDetailsAdptr = new CustomerDetailsTblAdptr();
            return _CustomerDetailsAdptr;
        }
    }
    protected AddressesTblAdptr AddressesAdptr
    {
        get
        {
            _AddressesAdptr = new AddressesTblAdptr();
            return _AddressesAdptr;
        }
    }
    protected SuppliersTblAdptr SuppliersAdptr
    {
        get
        {
            _SuppliersAdptr = new SuppliersTblAdptr();
            return _SuppliersAdptr;
        }
    }
    protected UsersTicketsDetailsTblAdptr UserTicketsAdptr
    {
        get
        {
            _UsersTicketsAdptr = new UsersTicketsDetailsTblAdptr();
            return _UsersTicketsAdptr;
        }
    }

    protected UserDetailsTblAdptr DetailsAdptr
    {
        get
        {
            return new UserDetailsTblAdptr();
        }
    }

    public void _Delete(int U_ID, bool blnReturnStock)
    {
        try
        {
            CustomerDetailsAdptr._Delete(U_ID, blnReturnStock);
        }
        catch (Exception ex)
        {
            ReportHandledError(ex, System.Reflection.MethodBase.GetCurrentMethod());
        }
    }

    public int _Update(int U_ID, string U_AccountHolderName, string U_EmailAddress, string U_Password, int U_LanguageID, int U_CustomerGroupID, double U_CustomerDiscount, bool blnUserApproved, bool blnUserAffiliate, double U_AffiliateCommission, DateTime U_SupportEndDate, string U_Notes)
    {
        try
        {
            Nullable<DateTime> dtNull = default(Date?);
            string strRandomSalt = Membership.GeneratePassword(20, 0);
            return CustomerDetailsAdptr._Update(U_ID, U_AccountHolderName, U_EmailAddress, Interaction.IIf(string.IsNullOrEmpty(U_Password), "", EncryptSHA256Managed(U_Password, strRandomSalt)), U_LanguageID, U_CustomerGroupID, U_CustomerDiscount, blnUserApproved, blnUserAffiliate, U_AffiliateCommission, Interaction.IIf(U_SupportEndDate == default(DateTime) | U_SupportEndDate == "#12:00:00 AM#", dtNull, U_SupportEndDate), U_Notes, strRandomSalt);
        }
        catch (Exception ex)
        {
            ReportHandledError(ex, System.Reflection.MethodBase.GetCurrentMethod());
            return default(Integer);
        }
    }

    public int _Add(string U_AccountHolderName, string U_EmailAddress, string U_Password, int U_LanguageID, int U_CustomerGroupID, double U_CustomerDiscount, bool blnUserApproved, bool blnUserAffiliate, double U_AffiliateCommission, DateTime U_SupportEndDate, string U_Notes)
    {
        try
        {
            Nullable<DateTime> dtNull = default(Date?);
            string strRandomSalt = Membership.GeneratePassword(20, 0);
            return CustomerDetailsAdptr._Add(U_AccountHolderName, U_EmailAddress, EncryptSHA256Managed(U_Password, strRandomSalt), U_LanguageID, U_CustomerGroupID, U_CustomerDiscount, blnUserApproved, blnUserAffiliate, U_AffiliateCommission, Interaction.IIf(U_SupportEndDate == default(DateTime) | U_SupportEndDate == "#12:00:00 AM#", dtNull, U_SupportEndDate), U_Notes, strRandomSalt);
        }
        catch (Exception ex)
        {
            ReportHandledError(ex, System.Reflection.MethodBase.GetCurrentMethod());
            return default(Integer);
        }
    }

    public void _AnonymizeAll()
    {
        CustomerDetailsAdptr._AnonymizeAll();
    }
    public DataTable _GetGuestList()
    {
        return CustomerDetailsAdptr.GetGuests();
    }

    public DataTable _GetAddressesByUserID(int U_ID, string ADR_Type)
    {
        return AddressesAdptr._GetData(U_ID, ADR_Type);
    }

    public int ValidateUser(string strEmailAddress, string strPassword)
    {
        return DetailsAdptr.Validate(strEmailAddress, EncryptSHA256Managed(strPassword, GetSaltByEmail(strEmailAddress)));
    }

    public DataTable GetDetails(string strEmailAddress)
    {
        return DetailsAdptr.GetData(strEmailAddress);
    }
    public int UpdateCustomerBalance(int CustomerID, decimal U_CustomerBalance)
    {
        return DetailsAdptr.UpdateCustomerBalance(CustomerID, U_CustomerBalance);
    }
    public string GetNameandEUVAT(int U_ID)
    {
        return DetailsAdptr.GetNameAndEUVAT(U_ID);
    }
    public string UpdateNameandEUVAT(int U_ID, string strName, string strEUVat)
    {
        return DetailsAdptr.UpdateNameAndEUVAT(U_ID, strName, strEUVat);
    }
    public int UpdateQBListID(int U_ID, string strQBListID)
    {
        try
        {
            return DetailsAdptr.UpdateQBListID(U_ID, strQBListID);
        }
        catch (Exception ex)
        {
            ReportHandledError(ex, System.Reflection.MethodBase.GetCurrentMethod());
        }
        return 0;
    }
    public string GetEmailByID(int U_ID)
    {
        return DetailsAdptr.GetEmailByID(U_ID);
    }

    // Now includes details of whether this user is a guest or not
    // Guest ones are tagged so we can delete/anonymize them later
    public int Add(string strEmailAddress, string strPassword, bool blnIsGuest = false)
    {
        try
        {
            string strRandomSalt = Membership.GeneratePassword(20, 0);
            return DetailsAdptr.Add(strEmailAddress, EncryptSHA256Managed(strPassword, strRandomSalt), strRandomSalt, CkartrisEnvironment.GetClientIPAddress(), blnIsGuest);
        }
        catch (Exception ex)
        {
            ReportHandledError(ex, System.Reflection.MethodBase.GetCurrentMethod());
        }
        return 0;
    }


    public int ChangePassword(int U_ID, string U_Password, string strNewPassword)
    {
        try
        {
            string strNewRandomSalt = Membership.GeneratePassword(20, 0);
            string strOldSalt = GetSaltByEmail(GetEmailByID(U_ID));
            return CustomerDetailsAdptr.ChangePassword(U_ID, EncryptSHA256Managed(U_Password, strOldSalt), EncryptSHA256Managed(strNewPassword, strNewRandomSalt), strNewRandomSalt);
        }
        catch (Exception ex)
        {
            ReportHandledError(ex, System.Reflection.MethodBase.GetCurrentMethod());
        }
        return 0;
    }

    public int ChangePasswordViaRecovery(int U_ID, string strNewPassword)
    {
        try
        {
            string strNewRandomSalt = Membership.GeneratePassword(20, 0);
            return DetailsAdptr.ChangePasswordfromRecovery(U_ID, EncryptSHA256Managed(strNewPassword, strNewRandomSalt), strNewRandomSalt);
        }
        catch (Exception ex)
        {
            ReportHandledError(ex, System.Reflection.MethodBase.GetCurrentMethod());
        }
        return 0;
    }

    public int ResetPassword(int U_ID, string strNewPassword)
    {
        try
        {
            string strOldSalt = GetSaltByEmail(GetEmailByID(U_ID));
            return CustomerDetailsAdptr.ResetPassword(U_ID, EncryptSHA256Managed(strNewPassword, strOldSalt), CkartrisDisplayFunctions.NowOffset.AddHours(1));
        }
        catch (Exception ex)
        {
            ReportHandledError(ex, System.Reflection.MethodBase.GetCurrentMethod());
        }
        return 0;
    }
    public string GetSaltByEmail(string U_EmailAddress)
    {
        try
        {
            return DetailsAdptr.GetSaltByEmail(U_EmailAddress);
        }
        catch (Exception ex)
        {
            ReportHandledError(ex, System.Reflection.MethodBase.GetCurrentMethod());
        }
        return null;
    }

    public string EncryptSHA256Managed(string ClearString, string SaltString, bool blnIsBackend = false)
    {
        System.Text.UnicodeEncoding uEncode = new System.Text.UnicodeEncoding();
        byte[] bytClearString = null;
        if (blnIsBackend)
        {
            if (string.IsNullOrEmpty(SaltString))
                bytClearString = uEncode.GetBytes(ClearString + ConfigurationManager.AppSettings("hashsalt"));
            else
                bytClearString = uEncode.GetBytes(ConfigurationManager.AppSettings("hashsalt") + ClearString + SaltString);
        }
        else
            bytClearString = uEncode.GetBytes(SaltString + ClearString + ConfigurationManager.AppSettings("hashsalt"));

        System.Security.Cryptography.SHA256Managed sha = new System.Security.Cryptography.SHA256Managed();
        byte[] hash = sha.ComputeHash(bytClearString);
        return Convert.ToBase64String(hash);
    }

    // _GetCustomerGroups - This is used by the Customer Details page on the backend. Please don't delete.
    public DataTable _GetCustomerGroups(byte numLanguageID)
    {
        return CustomerGroupsAdptr._GetData(numLanguageID);
    }

    public DataTable _GetCustomerDetails(int U_ID)
    {
        return CustomerDetailsAdptr.GetData(U_ID);
    }

    public DataTable _GetDataBySearchTerm(string strSearchTerm, int intPageIndex, int intPageSize, bool blnIsAffiliates = false, bool blnIsMailingList = false, int intCustomerGroupID = 0, bool blnIsAffiliateApproved = false)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            using (SqlCommand cmdSQL = new SqlCommand("_spKartrisUsers_ListBySearchTerm"))
            {
                cmdSQL.Connection = sqlConn;
                cmdSQL.CommandType = CommandType.StoredProcedure;
                cmdSQL.Parameters.AddWithValue("@SearchTerm", strSearchTerm);
                cmdSQL.Parameters.AddWithValue("@PageIndex", intPageIndex);
                cmdSQL.Parameters.AddWithValue("@PageSize", intPageSize);
                cmdSQL.Parameters.AddWithValue("@isAffiliate", blnIsAffiliates);
                cmdSQL.Parameters.AddWithValue("@isMailingList", blnIsMailingList);
                cmdSQL.Parameters.AddWithValue("@CustomerGroupID", intCustomerGroupID);
                cmdSQL.Parameters.AddWithValue("@isAffiliateApproved", blnIsAffiliateApproved);
                using (SqlDataAdapter adpGetData = new SqlDataAdapter(cmdSQL))
                {
                    DataTable tblUserData = new DataTable();
                    adpGetData.Fill(tblUserData);
                    return tblUserData;
                }
            }
        }
    }
    public int _GetDataBySearchTermCount(string strSearchTerm, bool blnIsAffiliates = false, bool blnisMailingList = false, int intCustomerGroupID = 0, bool blnIsAffiliateApproved = false)
    {
        return CustomerDetailsAdptr._GetDataBySearchTermCount(strSearchTerm, blnIsAffiliates, blnisMailingList, intCustomerGroupID, blnIsAffiliateApproved);
    }


    /// <summary>
    ///     ''' Get customer EU VAT number
    ///     ''' </summary>
    ///     ''' <remarks>
    ///     ''' Can use this to pull through existing one into checkout for repeat
    ///     ''' orders
    ///     ''' </remarks>
    public string GetCustomerEUVATNumber(int U_ID)
    {
        string strEUVATNumber = "";
        DataTable tblCustomer = _GetCustomerDetails(U_ID);
        try
        {
            strEUVATNumber = CkartrisDataManipulation.FixNullFromDB(tblCustomer.Rows(0).Item("U_CardholderEUVATNum"));
        }
        catch (Exception ex)
        {
        }

        return strEUVATNumber;
    }

    /// <summary>
    ///     ''' Clean guest email address
    ///     ''' </summary>
    ///     ''' <remarks>
    ///     ''' For guest accounts, the email field contains 
    ///     ''' extra text at the end to ensure uniqueness. If
    ///     ''' the admins look at a guest account in the back
    ///     ''' end, we want to ensure they don't see this.
    ///     ''' </remarks>
    public static string CleanGuestEmailUsername(string strEmail)
    {
        byte numPipeLast = 0;
        // The email may have |GUEST|[ID] at the end of it. If so,
        // we want to remove this and return just the email
        // address.
        byte numPipePenultimate = 0;
        try
        {
            // Find position of the last pipe char.
            numPipeLast = strEmail.LastIndexOf("|");

            if (numPipeLast > 0)
                // Trim last part off
                strEmail = Strings.Left(strEmail, numPipeLast);

            // Find position of the last pipe char.
            numPipePenultimate = strEmail.LastIndexOf("|");

            if (numPipePenultimate > 0)
                // Trim last part off
                strEmail = Strings.Left(strEmail, numPipePenultimate);
        }
        catch (Exception ex)
        {
        }


        return strEmail;
    }


    protected internal DataTable _GetCustomerGroupsForCache()
    {
        return CustomerGroupsAdptr._GetForCache();
    }

    public bool _AddCustomerGroups(DataTable dtbElements, float pCG_Discount, bool pCG_Live, ref string strMsg)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmdSQL = sqlConn.CreateCommand;
            cmdSQL.CommandText = "_spKartrisCustomerGroups_Add";
            SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
            cmdSQL.CommandType = CommandType.StoredProcedure;
            try
            {
                cmdSQL.Parameters.AddWithValue("@NewCG_ID", 0).Direction = ParameterDirection.Output;
                cmdSQL.Parameters.AddWithValue("@CG_Discount", pCG_Discount);
                cmdSQL.Parameters.AddWithValue("@CG_Live", pCG_Live);

                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();
                cmdSQL.Transaction = savePoint;

                // ' 1. Add The Main Info.
                cmdSQL.ExecuteNonQuery();

                if (cmdSQL.Parameters("@NewCG_ID").Value == null || cmdSQL.Parameters("@NewCG_ID").Value == DBNull.Value)
                    throw new ApplicationException(System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_ErrorMsgDBCustom"));

                short numNewCG_ID;
                numNewCG_ID = cmdSQL.Parameters("@NewCG_ID").Value;

                // ' 2. Add the Language Elements
                if (!LanguageElementsBLL._AddLanguageElements(dtbElements, LANG_ELEM_TABLE_TYPE.CustomerGroups, numNewCG_ID, sqlConn, savePoint))
                    throw new ApplicationException(System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_ErrorMsgDBCustom"));

                savePoint.Commit();
                strMsg = System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_OperationCompletedSuccessfully");
                return true;
            }
            catch (Exception ex)
            {
                if (!savePoint == null)
                    savePoint.Rollback();
                ReportHandledError(ex, System.Reflection.MethodBase.GetCurrentMethod(), strMsg);
            }
            finally
            {
                if (sqlConn.State == ConnectionState.Open)
                {
                    sqlConn.Close(); savePoint.Dispose();
                }
            }
        }
        return false;
    }

    public bool _UpdateCustomerGroups(DataTable dtbElements, short pCG_ID, float pCG_Discount, bool pCG_Live, ref string strMsg)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmdSQL = sqlConn.CreateCommand;
            cmdSQL.CommandText = "_spKartrisCustomerGroups_Update";
            SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
            cmdSQL.CommandType = CommandType.StoredProcedure;
            try
            {
                cmdSQL.Parameters.AddWithValue("@CG_ID", pCG_ID);
                cmdSQL.Parameters.AddWithValue("@CG_Discount", pCG_Discount);
                cmdSQL.Parameters.AddWithValue("@CG_Live", pCG_Live);


                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();
                cmdSQL.Transaction = savePoint;

                // ' 1. Add The Main Info.
                cmdSQL.ExecuteNonQuery();

                // ' 2. Update the Language Elements
                if (!LanguageElementsBLL._UpdateLanguageElements(dtbElements, LANG_ELEM_TABLE_TYPE.CustomerGroups, pCG_ID, sqlConn, savePoint))
                    throw new ApplicationException(System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_ErrorMsgDBCustom"));

                savePoint.Commit();
                strMsg = System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_OperationCompletedSuccessfully");
                return true;
            }
            catch (Exception ex)
            {
                if (!savePoint == null)
                    savePoint.Rollback();
                ReportHandledError(ex, System.Reflection.MethodBase.GetCurrentMethod(), strMsg);
            }
            finally
            {
                if (sqlConn.State == ConnectionState.Open)
                {
                    sqlConn.Close(); savePoint.Dispose();
                }
            }
        }
        return false;
    }


    protected internal DataTable _GetSuppliersForCache()
    {
        return SuppliersAdptr._GetData();
    }

    public bool _AddSuppliers(string pSupplierName, bool pSupplier_Live, ref string strMsg)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmdSQL = sqlConn.CreateCommand;
            cmdSQL.CommandText = "_spKartrisSuppliers_Add";
            SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
            cmdSQL.CommandType = CommandType.StoredProcedure;
            try
            {
                cmdSQL.Parameters.AddWithValue("@NewSUP_ID", 0).Direction = ParameterDirection.Output;
                cmdSQL.Parameters.AddWithValue("@SUP_Name", pSupplierName);
                cmdSQL.Parameters.AddWithValue("@SUP_Live", pSupplier_Live);

                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();
                cmdSQL.Transaction = savePoint;

                // ' Add the supplier's info.
                cmdSQL.ExecuteNonQuery();

                if (cmdSQL.Parameters("@NewSUP_ID").Value == null || cmdSQL.Parameters("@NewSUP_ID").Value == DBNull.Value)
                    throw new ApplicationException(System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_ErrorMsgDBCustom"));

                savePoint.Commit();
                strMsg = System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_OperationCompletedSuccessfully");
                return true;
            }
            catch (Exception ex)
            {
                if (!savePoint == null)
                    savePoint.Rollback();
                ReportHandledError(ex, System.Reflection.MethodBase.GetCurrentMethod(), strMsg);
            }
            finally
            {
                if (sqlConn.State == ConnectionState.Open)
                {
                    sqlConn.Close(); savePoint.Dispose();
                }
            }
        }
        return false;
    }

    public bool _UpdateSuppliers(short pSupplier_ID, string pSupplierName, bool pSupplier_Live, ref string strMsg)
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmdSQL = sqlConn.CreateCommand;
            cmdSQL.CommandText = "_spKartrisSuppliers_Update";
            SqlTransaction savePoint = null/* TODO Change to default(_) if this is not a reference type */;
            cmdSQL.CommandType = CommandType.StoredProcedure;
            try
            {
                cmdSQL.Parameters.AddWithValue("@SUP_ID", pSupplier_ID);
                cmdSQL.Parameters.AddWithValue("@SUP_Name", pSupplierName);
                cmdSQL.Parameters.AddWithValue("@SUP_Live", pSupplier_Live);

                sqlConn.Open();
                savePoint = sqlConn.BeginTransaction();
                cmdSQL.Transaction = savePoint;

                // ' Update the supplier's info.
                cmdSQL.ExecuteNonQuery();

                savePoint.Commit();
                strMsg = System.Web.HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_OperationCompletedSuccessfully");
                return true;
            }
            catch (Exception ex)
            {
                if (!savePoint == null)
                    savePoint.Rollback();
                ReportHandledError(ex, System.Reflection.MethodBase.GetCurrentMethod(), strMsg);
            }
            finally
            {
                if (sqlConn.State == ConnectionState.Open)
                {
                    sqlConn.Close(); savePoint.Dispose();
                }
            }
        }
        return false;
    }


    public DataTable _GetUsersTicketDetails()
    {
        return UserTicketsAdptr._GetData();
    }
    public DataTable _GetTicketDetailsByUser(int numUserID)
    {
        return UserTicketsAdptr._GetByUserID(numUserID);
    }
}
