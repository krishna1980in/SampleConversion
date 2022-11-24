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
using Microsoft.VisualBasic;

partial class UserControls_Back_AdminExecuteQuery : System.Web.UI.UserControl
{
    protected void btnExecuteQuery_Click(object sender, System.EventArgs e)
    {
        ExecuteQuery();
    }

    public void ExecuteQuery()
    {
        int numAffectedRecords = 0;
        string strMessage = "";
        DataTable tblReturnedRecords = new DataTable();
        if (KartrisDBBLL._ExecuteQuery(txtSqlQuery.Text, numAffectedRecords, tblReturnedRecords, System.Web.UI.UserControl.Session["_User"], strMessage))
        {
            if (txtSqlQuery.Text.ToUpper.StartsWith("SELECT") || txtSqlQuery.Text.ToUpper.StartsWith("INSERT") || txtSqlQuery.Text.ToUpper.StartsWith("UPDATE") || txtSqlQuery.Text.ToUpper.StartsWith("DELETE"))
            {
                litRecordsReturned.Text = tblReturnedRecords.Rows.Count;
                litRecordsAffected.Text = numAffectedRecords;
                if (tblReturnedRecords.Rows.Count > 0)
                {
                    gvwReturnedRecords.DataSource = tblReturnedRecords;
                    gvwReturnedRecords.DataBind();
                    mvwResult.SetActiveView(viwResultData);
                }
                else if (tblReturnedRecords.Rows.Count == 0)
                    mvwResult.SetActiveView(viwNoResults);
                mvwQueryType.SetActiveView(viwNonProcedure);
            }
            else if (txtSqlQuery.Text.ToUpper.StartsWith("ALTER PROCEDURE") || txtSqlQuery.Text.ToUpper.StartsWith("CREATE PROCEDURE"))
                mvwQueryType.SetActiveView(viwProcedure);
            litQueryExecutedSucceeded.Text = txtSqlQuery.Text;
            mvwQuery.SetActiveView(viwQuerySucceeded);
        }
        else
        {
            litQueryFailedError.Text = strMessage;
            litQueryExecutedFailed.Text = txtSqlQuery.Text;
            mvwQuery.SetActiveView(viwQueryFailed);
        }
    }

    protected void lnkBtnBackFailed_Click(object sender, System.EventArgs e)
    {
        BackToQueryExecution();
    }

    protected void lnkBtnBackSucceeded_Click(object sender, System.EventArgs e)
    {
        BackToQueryExecution();
    }

    public void BackToQueryExecution()
    {
        litRecordsAffected.Text = string.Empty;
        litRecordsReturned.Text = string.Empty;
        litQueryExecutedSucceeded.Text = string.Empty;
        gvwReturnedRecords.DataSource = null;
        gvwReturnedRecords.DataBind();
        mvwQuery.SetActiveView(viwExecuteQuery);
    }
}
