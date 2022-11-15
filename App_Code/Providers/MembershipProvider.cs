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

using CkartrisDataManipulation;
using System.Web.Security;

public class KartrisMemberShipUser : MembershipUser
{
    private int _ID;
    private string _userName;
    private string _emailAddress;
    private int _customerGroupID;
    private decimal _customerDiscount;
    private int _defaultBillingID;
    private int _defaultShippingID;
    private bool _isAffiliate;
    private int _affiliateID;
    private decimal _AffiliateCommission;
    private int _DefaultLangugeID;
    private bool _isAuthorized;
    private bool _isSupportValid;
    private DateTime _SupportEndDate;
    private decimal _customerBalance;

    public KartrisMemberShipUser(int id, string emailAddress, int customerGroupID, decimal customerDiscount, int defaultBillingID, int defaultShippingID, int defaultLanguageID, bool isApproved, bool isAffiliate = false, int affiliateID = 0, decimal affiliatecommission = 0, bool isSupportValid = false, DateTime SupportEndDate = default(DateTime), decimal customerBalance = 0)
    {
        this.ID = id;
        this.Email = emailAddress;
        this.CustomerGroupID = customerGroupID;
        this.CustomerDiscount = customerDiscount;
        this.DefaultBillingAddressID = defaultBillingID;
        this.DefaultShippingAddressID = defaultShippingID;
        this.IsAffiliate = isAffiliate;
        this.AffiliateID = affiliateID;
        this.AffiliateCommision = affiliatecommission;
        this.DefaultLanguageID = defaultLanguageID;
        this.isAuthorized = isApproved;
        _isSupportValid = isSupportValid;
        _SupportEndDate = SupportEndDate;
        _customerBalance = customerBalance;
    }
    public int ID
    {
        get
        {
            return _ID;
        }
        set
        {
            _ID = value;
        }
    }
    public int CustomerGroupID
    {
        get
        {
            return _customerGroupID;
        }
        set
        {
            _customerGroupID = value;
        }
    }
    public decimal CustomerDiscount
    {
        get
        {
            return _customerDiscount;
        }
        set
        {
            _customerDiscount = value;
        }
    }
    public int DefaultBillingAddressID
    {
        get
        {
            return _defaultBillingID;
        }
        set
        {
            _defaultBillingID = value;
        }
    }
    public int DefaultShippingAddressID
    {
        get
        {
            return _defaultShippingID;
        }
        set
        {
            _defaultShippingID = value;
        }
    }
    public int DefaultLanguageID
    {
        get
        {
            return _DefaultLangugeID;
        }
        set
        {
            _DefaultLangugeID = value;
        }
    }
    public bool IsAffiliate
    {
        get
        {
            return _isAffiliate;
        }
        set
        {
            _isAffiliate = value;
        }
    }
    public int AffiliateID
    {
        get
        {
            if (_isAffiliate)
                return _affiliateID;
            else
                return 0;
        }
        set
        {
            if (IsAffiliate)
                _affiliateID = value;
            else
                _affiliateID = 0;
        }
    }
    public decimal AffiliateCommision
    {
        get
        {
            if (_isAffiliate)
                return _AffiliateCommission;
            else
                return 0;
        }
        set
        {
            _customerDiscount = value;
        }
    }
    public bool isAuthorized
    {
        get
        {
            return _isAuthorized;
        }
        set
        {
            _isAuthorized = value;
        }
    }
    public bool isSupportValid
    {
        get
        {
            return _isSupportValid;
        }
    }
    public DateTime SupportEndDate
    {
        get
        {
            return _SupportEndDate;
        }
    }
    public decimal CustomerBalance
    {
        get
        {
            return _customerBalance;
        }
    }
}

public class KartrisMembershipProvider : MembershipProvider
{
    private bool _requiresQuestionAndAnswer;
    private int _minRequiredPasswordLength;

    public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
    {

        // ===retrives the attribute values set in 
        // web.config and assign to local variables===

        if (config["requiresQuestionAndAnswer"] == "true")
            _requiresQuestionAndAnswer = true;
        base.Initialize(name, config);
    }

    public override string ApplicationName
    {
        get
        {
            return null;
        }
        set
        {
        }
    }

    public override bool ChangePassword(string username, string oldPassword, string newPassword)
    {
        return default(Boolean);
    }

    public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
    {
        return default(Boolean);
    }

    public override System.Web.Security.MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, ref System.Web.Security.MembershipCreateStatus status)
    {
        KartrisMemberShipUser KartrisUser;

        // ----perform checking all the relevant checks here
        // and set the status of the error accordingly, e.g.:
        // status = MembershipCreateStatus.InvalidPassword
        // status = MembershipCreateStatus.InvalidAnswer
        // status = MembershipCreateStatus.InvalidEmail

        // ---add the user to the database
        UsersBLL objUsersBLL = new UsersBLL();
        try
        {
            int U_ID = objUsersBLL.Add(username, password);
            if (U_ID > 0)
            {
                KartrisUser = new KartrisMemberShipUser(U_ID, username, 0, 0, 0, 0, 1, true);
                status = MembershipCreateStatus.Success;
                return KartrisUser;
            }
        }

        catch (Exception ex)
        {
            // ---failed; determine the reason why
            status = MembershipCreateStatus.UserRejected;
            return null/* TODO Change to default(_) if this is not a reference type */;
        }
        return null/* TODO Change to default(_) if this is not a reference type */;
    }

    public override bool DeleteUser(string username, bool deleteAllRelatedData)
    {
        return default(Boolean);
    }

    public override bool EnablePasswordReset
    {
        get
        {
            return default(Boolean);
        }
    }

    public override bool EnablePasswordRetrieval
    {
        get
        {
            return default(Boolean);
        }
    }

    public override System.Web.Security.MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, ref int totalRecords)
    {
        return null/* TODO Change to default(_) if this is not a reference type */;
    }

    public override System.Web.Security.MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, ref int totalRecords)
    {
        return null/* TODO Change to default(_) if this is not a reference type */;
    }

    public override System.Web.Security.MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, ref int totalRecords)
    {
        return null/* TODO Change to default(_) if this is not a reference type */;
    }

    public override int GetNumberOfUsersOnline()
    {
        return default(Integer);
    }

    public override string GetPassword(string username, string answer)
    {
        return null;
    }

    public new override System.Web.Security.MembershipUser GetUser(string username, bool userIsOnline)
    {
        KartrisMemberShipUser User;
        UsersBLL objUsersBLL = new UsersBLL();

        DataTable UserDetails = objUsersBLL.GetDetails(username);
        if (UserDetails.Rows.Count > 0)
        {
            bool blnSupportValid = false;

            if (FixNullFromDB(UserDetails.Rows(0)("U_SupportEndDate")) != null)
            {
                if (IsDate(UserDetails.Rows(0)("U_SupportEndDate")))
                {
                    if ((DateTime)UserDetails.Rows(0)("U_SupportEndDate") > CkartrisDisplayFunctions.NowOffset)
                        blnSupportValid = true;
                }
            }

            User = new KartrisMemberShipUser(UserDetails.Rows(0)("U_ID"), username, FixNullFromDB(UserDetails.Rows(0)("U_CustomerGroupID")), FixNullFromDB(UserDetails.Rows(0)("U_CustomerDiscount")), FixNullFromDB(UserDetails.Rows(0)("U_DefBillingAddressID")), FixNullFromDB(UserDetails.Rows(0)("U_DefShippingAddressID")), UserDetails.Rows(0)("U_LanguageID"), UserDetails.Rows(0)("U_Approved"), isSupportValid: blnSupportValid, SupportEndDate: FixNullFromDB(UserDetails.Rows(0)("U_SupportEndDate")), customerBalance: FixNullFromDB(UserDetails.Rows(0)("U_CustomerBalance")));
            return User;
        }
        else
            return null/* TODO Change to default(_) if this is not a reference type */;
    }

    public new override System.Web.Security.MembershipUser GetUser(object providerUserKey, bool userIsOnline)
    {
        return null/* TODO Change to default(_) if this is not a reference type */;
    }

    public override string GetUserNameByEmail(string email)
    {
        return null;
    }

    public override int MaxInvalidPasswordAttempts
    {
        get
        {
            return default(Integer);
        }
    }

    public override int MinRequiredNonAlphanumericCharacters
    {
        get
        {
            return default(Integer);
        }
    }

    public override int MinRequiredPasswordLength
    {
        get
        {
            return default(Integer);
        }
    }

    public override int PasswordAttemptWindow
    {
        get
        {
            return default(Integer);
        }
    }

    public override System.Web.Security.MembershipPasswordFormat PasswordFormat
    {
        get
        {
            return null/* TODO Change to default(_) if this is not a reference type */;
        }
    }

    public override string PasswordStrengthRegularExpression
    {
        get
        {
            return null;
        }
    }

    public override bool RequiresQuestionAndAnswer
    {
        get
        {
            if (_requiresQuestionAndAnswer == true)
                return true;
            else
                return false;
        }
    }

    public override bool RequiresUniqueEmail
    {
        get
        {
            return default(Boolean);
        }
    }

    public override string ResetPassword(string username, string answer)
    {
        return null;
    }

    public override bool UnlockUser(string userName)
    {
        return default(Boolean);
    }

    public override void UpdateUser(System.Web.Security.MembershipUser user)
    {
    }

    public override bool ValidateUser(string username, string password)
    {
        try
        {
            UsersBLL objUsersBLL = new UsersBLL();
            int U_ID = objUsersBLL.ValidateUser(username, password);
            if (U_ID > 0)
                // Dim UserDetails As DataTable = UsersBLL.GetDetails(username)
                // KartrisUser = New KartrisMemberShipUser(U_ID, username, UserDetails.Rows(0)("U_CustomerGroupID"), UserDetails.Rows(0)("U_CustomerDiscount"), UserDetails.Rows(0)("U_DefBillingAddressID"), _
                // UserDetails.Rows(0)("U_DefShippingAddressID"), UserDetails.Rows(0)("U_LanguageID"), True)
                // UserDetails.Rows(0)("U_Approved")
                // System.Web.HttpContext.Current.Session("KartrisUserCulture") = LanguagesBLL.GetCultureByLanguageID_s(UserDetails.Rows(0)("U_LanguageID"))
                return true;
            else
                return false;
        }
        catch (Exception ex)
        {
            Console.Write(ex.ToString());
            return false;
        }
    }
    public static bool UserExists(string username)
    {
        if (Membership.GetUser(username) != null)
            return true;
        return false;
    }
}
