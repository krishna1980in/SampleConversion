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

using SessionsData;
using SessionsDataTableAdapters;
using System.Web.HttpContext;
using KartSettingsManager;
using System.Collections;

public class SessionsBLL
{
    enum Using_Cookies
    {
        YES = 1,
        NO = 2,
        BACKUP = 3
    }

    private string strUsingCookies;

    private const string USING_COOKIES_YES = "yes";
    private const string USING_COOKIES_NO = "no";
    private const string USING_COOKIES_BACKUP = "backup";
    private const string VARIABLE_NAME = "s";
    private const int CODE_LENGTH = 6;
    private int DEFAULT_SESSION_EXPIRY;

    private SessionsTblAdptr _AdptrSessions = null/* TODO Change to default(_) if this is not a reference type */;
    private SessionValuesTblAdptr _AdptrSessionValues = null/* TODO Change to default(_) if this is not a reference type */;

    private string _SessionCode;
    private long _SessionID;


    private class KartSessionValues
    {
        private string strOriginalValue;
        private int numOriginalExpiry;

        // ID and name are not accessible, but required internally
        private int SESSV_ID;
        private int SESSV_SessionID;
        private string SESSV_Name;

        // This can be altered outside the class
        public string Value;                 // The value held
        public int Expiry;            // Minutes until the value expires
        public bool Deleted;              // Whether to flag the name/value for deletion

        private SessionValuesTblAdptr _AdptrSessionValues = null/* TODO Change to default(_) if this is not a reference type */;

        protected SessionValuesTblAdptr AdptrSessionValues
        {
            get
            {
                _AdptrSessionValues = new SessionValuesTblAdptr();
                return _AdptrSessionValues;
            }
        }
    }

    // '//
    public string SessionCode
    {
        get
        {
            if (_SessionCode == "")
                NewSession();
            return _SessionCode;
        }
    }

    public string SessionIP
    {
        get
        {
            return CkartrisEnvironment.GetClientIPAddress();
        }
    }

    public long SessionID
    {
        get
        {
            if (SessionID == 0)
                NewSession();
            return _SessionID;
        }
    }
    // '//

    protected SessionsTblAdptr AdptrSessions
    {
        get
        {
            _AdptrSessions = new SessionsTblAdptr();
            return _AdptrSessions;
        }
    }

    protected SessionValuesTblAdptr AdptrSessionValues
    {
        get
        {
            _AdptrSessionValues = new SessionValuesTblAdptr();
            return _AdptrSessionValues;
        }
    }

    public bool IsBrowser
    {
        get
        {
            try
            {
                string strHTTPUserAgent;
                strHTTPUserAgent = Strings.LCase(System.Web.HttpContext.Current.Request.ServerVariables["HTTP_USER_AGENT"]);
                if (strHTTPUserAgent.Contains("msie") || strHTTPUserAgent.Contains("gecko") || strHTTPUserAgent.Contains("opera") || strHTTPUserAgent.Contains("netscape") || strHTTPUserAgent.Contains("safari") || strHTTPUserAgent.Contains("webkit"))
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }

    private static string GetRandomString(int numLength)
    {
        string strRandomString = "";
        int numRandomNumber;

        // Generate a new seed based on the server timer
        VBMath.Randomize();

        // Loop for as many letters as we need
        while (Strings.Len(strRandomString) < numLength)
        {
            // Generate random number	
            numRandomNumber = Conversion.Int(VBMath.Rnd(1) * 36) + 1;
            if (numRandomNumber < 11)
                // If it's less than 11 then we'll do a number
                strRandomString = strRandomString + Strings.Chr(numRandomNumber + 47);
            else
                // Otherwise we'll do a letter; + 86 because 96 (min being 97, 'a') - 10 (the first 10 was for the number)
                strRandomString = strRandomString + Strings.Chr(numRandomNumber + 86);
        }

        // Zero and 'o' and '1' and 'I' are easily confused...
        // So we replace any of these with alternatives
        // To ensure best randomness, replace the numbers
        // with alternative letters and letters
        // with alternative numbers

        strRandomString = Strings.Replace(strRandomString, "0", "X");
        strRandomString = Strings.Replace(strRandomString, "1", "Y");
        strRandomString = Strings.Replace(strRandomString, "O", "4");
        strRandomString = Strings.Replace(strRandomString, "I", "9");

        return strRandomString;
    }
    // '//

    public void NewSession()
    {
        string COOKIE_NAME, strValue;
        // 'Dim SESS_Values As String = ""
        DateTime SESS_DateCreated, SESS_DateLastUpdated;
        int SESS_Expiry;
        System.Data.DataTable oDT = new System.Data.DataTable();

        if (IsBrowser)
        {

            // Set related config settings
            COOKIE_NAME = HttpSecureCookie.GetCookieName("Basket");
            DEFAULT_SESSION_EXPIRY = System.Convert.ToInt32(GetKartConfig("general.sessions.expiry"));

            strUsingCookies = LCase(Trim(GetKartConfig("general.sessions.usecookies")));

            // Try and find a session id somewhere. The cookie takes preference (so
            // if a user goes to a page in the history with an old ID, they won't
            // switch sessions and see an old basket, someone elses login etc)
            strValue = "";
            if (strUsingCookies != Strings.Trim(Strings.LCase(USING_COOKIES_NO)))
            {
                if (!(System.Web.HttpContext.Current.Request.Cookies[COOKIE_NAME] == null))
                    strValue = System.Web.HttpContext.Current.Request.Cookies[COOKIE_NAME].Item[VARIABLE_NAME];
                if (strValue == "")
                {
                    strValue = System.Web.HttpContext.Current.Request.QueryString[VARIABLE_NAME];
                    if (strValue == "")
                        strValue = System.Web.HttpContext.Current.Request.Form[VARIABLE_NAME];
                }
            }

            // Check it's the right length
            if (Strings.Len(strValue + "") < CODE_LENGTH + 1)
            {
                _SessionID = 0;
                _SessionCode = "";
            }
            else
            {
                // Get the random code prefix, followed by the ID
                _SessionID = strValue.Substring(CODE_LENGTH);
                _SessionCode = strValue;
            }

            // We set up the session. If we've got a session code then try and pull it
            // out of the database. Keep open the recordset for laters
            if (_SessionID > 0)
            {
                oDT = AdptrSessions.GetSessionValues(_SessionID, _SessionCode, CkartrisDisplayFunctions.NowOffset);
                if (oDT.Rows.Count > 0)
                {
                    _SessionCode = oDT.Rows[0].Item["SESS_Code"] + "";
                    SESS_DateLastUpdated = oDT.Rows[0].Item["SESS_DateLastUpdated"];
                    AdptrSessions.UpdateSessionsDateLastUpdated(CkartrisDisplayFunctions.NowOffset, _SessionID);
                }
                else
                {
                    _SessionCode = "";
                    _SessionID = 0;
                }
                oDT.Dispose();
            }

            // If we haven't got a session, need to create a new one. Need to do an
            // insert now so that we get the database ID
            if (_SessionID == 0)
            {
                _SessionCode = GetRandomString(CODE_LENGTH);
                SESS_DateCreated = CkartrisDisplayFunctions.NowOffset;
                SESS_DateLastUpdated = SESS_DateCreated;
                SESS_Expiry = DEFAULT_SESSION_EXPIRY;

                _SessionID = AdptrSessions.AddSessions(_SessionCode, SessionIP, SESS_DateCreated, SESS_DateLastUpdated, SESS_Expiry);
                _SessionCode = _SessionCode + _SessionID;
            }

            // Store the session code in a session-length cookie.
            if (strUsingCookies != Strings.Trim(Strings.LCase(USING_COOKIES_NO)))
            {
                if (System.Web.HttpContext.Current.Request.Cookies[COOKIE_NAME] == null)
                {
                    System.Web.HttpContext.Current.Response.Cookies[COOKIE_NAME][VARIABLE_NAME] = _SessionCode;
                    System.Web.HttpContext.Current.Response.Cookies[COOKIE_NAME].Expires = CkartrisDisplayFunctions.NowOffset.AddMinutes(DEFAULT_SESSION_EXPIRY);
                }
            }
        }
    }

    // Adds a new name/value pair
    public void Add(string strName, string strValue)
    {
        if (IsBrowser)
        {
            DEFAULT_SESSION_EXPIRY = System.Convert.ToInt32(GetKartConfig("general.sessions.expiry"));
            if (SessionValueExists(strName))
                AdptrSessionValues.UpdateSessionValues(SessionID, strName, strValue, DEFAULT_SESSION_EXPIRY);
            else
                AdptrSessionValues.InsertSessionValues(SessionID, strName, strValue, DEFAULT_SESSION_EXPIRY);
        }
    }

    // Edits a new name/value pair
    public void Edit(string strName, string strValue)
    {
        if (IsBrowser)
        {
            DEFAULT_SESSION_EXPIRY = System.Convert.ToInt32(GetKartConfig("general.sessions.expiry"));
            if (!(SessionValueExists(strName)))
                AdptrSessionValues.InsertSessionValues(SessionID, strName, strValue, DEFAULT_SESSION_EXPIRY);
            else
                AdptrSessionValues.UpdateSessionValues(SessionID, strName, strValue, DEFAULT_SESSION_EXPIRY);
        }
    }

    // Deletes an item
    public void Delete(string strName)
    {
        if (IsBrowser)
            AdptrSessionValues.DeleteSessionValues(SessionID, strName);
    }

    private bool SessionValueExists(string strName)
    {
        bool blnExists = false;
        if (IsBrowser)
            blnExists = AdptrSessionValues.GetCount(SessionID, strName) > 0;
        return blnExists;
    }

    public string Value(string strName)
    {
        string strValue = "";
        if (IsBrowser)
            strValue = AdptrSessionValues.GetValue(SessionID, strName) + "";
        return strValue;
    }

    public int Expiry(string strName)
    {
        int numExpiry;
        // 'If _IsBrowser Then
        // '	If Not objItems.ContainsKey(LCase(strName)) Then Call AddItem(strName, "")
        // '	numExpiry = objItems(LCase(strName)).Expiry
        // 'End If
        return numExpiry;
    }

    public SessionsDataTable GetSessions()
    {
        return AdptrSessions.GetData();
    }

    private bool AddSessions(string _SESS_Code, string _SESS__IP, string _SESS_DateCreated, string _SESS_DateLastUpdated, int _SESS_Expiry)
    {
        AdptrSessions.AddSessions(_SESS_Code, _SESS__IP, _SESS_DateCreated, _SESS_DateLastUpdated, _SESS_Expiry);
        return true;
    }

    private bool AddSessionValues(int _SESSV_SessionID, string _SESSV_Name, string _SESSV_Value, int _SESSV_Expiry)
    {
        AdptrSessionValues.InsertSessionValues(_SESSV_SessionID, _SESSV_Name, _SESSV_Value, _SESSV_Expiry);
        return true;
    }

    public long GetSessionID(string strSessionCode)
    {
        return AdptrSessions.GetSessionID(strSessionCode);
    }

    public void CleanExpiredSessionsData()
    {
        AdptrSessions.DeleteExpired(CkartrisDisplayFunctions.NowOffset);
    }
}
