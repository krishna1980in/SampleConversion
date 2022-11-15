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

using CkartrisDisplayFunctions;
using CkartrisFormatErrors;
using CkartrisDataManipulation;
using System.Web.HttpContext;

public class FeedsBLL
{
    // We don't use table adaptors here, as it's far simpler to set a long
    // command timeout this way, and pulling all the product on the site is
    // often something that would otherwise time out on a big site
    public static DataTable _GetFeedData()
    {
        string strConnString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ToString();
        using (SqlConnection sqlConn = new SqlConnection(strConnString))
        {
            SqlCommand cmdExecuteQuery = sqlConn.CreateCommand;
            cmdExecuteQuery.CommandText = "_spKartrisFeeds_GetItems";

            cmdExecuteQuery.CommandType = CommandType.StoredProcedure;
            cmdExecuteQuery.CommandTimeout = 3600;

            try
            {
                DataTable tblExport = new DataTable();
                using (SqlDataAdapter adptr = new SqlDataAdapter(cmdExecuteQuery))
                {
                    adptr.Fill(tblExport);
                    return tblExport;
                }
            }
            catch (Exception ex)
            {
                ReportHandledError(ex, System.Reflection.MethodBase.GetCurrentMethod());
            }
            finally
            {
            }
        }
        return null/* TODO Change to default(_) if this is not a reference type */;
    }
}
