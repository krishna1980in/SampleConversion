using System;
using CkartrisBLL;
using CkartrisDataManipulation;
using Kartris.Interfaces.Utils;
using KartSettingsManager;

internal partial class Callback : PageBaseClass
{
    private string strEntity = "";
    private string strRef = "";
    private string strAmount = "";

    private Kartris.Interfaces.PaymentGateway clsPlugin;

    public Callback()
    {
        this.Load += Page_Load;
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            string strEntity = "";
            string strRef = "";
            string strAmount = "";
            string strGateway = "";
            string strAction = "";
            string strGatewayName = "";
            // Callback Step 0 - normal callback
            // Callback Step 1 - update order but don't display full HTML if d=off QS is passed, write gateway dll output to screen
            // Callback Step 2 - don't update order, just display result as usual
            int intCallbackStep;
            try
            {
                intCallbackStep = Request.QueryString("step");
            }
            catch (Exception ex)
            {
                intCallbackStep = 0;
            }
            bool blnFullDisplay = true;
            if (Request.QueryString("d") == "off")
                blnFullDisplay = false;
            try
            {
                strGateway = Request.QueryString("g").ToLower;
                strAction = Request.QueryString("a").ToLower;
            }
            catch (Exception ex)
            {
            }

            if (strGateway == "easypay")
            {
                if (strAction == "mbrefer")
                {
                    var EasypayPayment = Session("EasypayPayment");
                    System.Collections.Specialized.StringDictionary sdCallbackFields = ConvertCallbackDataStr2Dict(EasypayPayment);
                    strEntity = sdCallbackFields["npcentity"];
                    valEntity.Text = strEntity;
                    strRef = sdCallbackFields["payment_cluster_key"];
                    valRef.Text = addBlanks(strRef, 3);
                    strAmount = sdCallbackFields["npcamount"];
                    valAmount.Text = strAmount + " €";
                }
            }

        }
    }

    private string addBlanks(string str, int eachChar)
    {
        string strResult = "";
        int lastValue = 0;
        for (int value = 0, loopTo = str.Length; value <= loopTo; value++)
        {
            if (value > 0 & value % eachChar == 0)
            {
                strResult += str.Substring(lastValue, eachChar) + " ";
                lastValue = value;
            }
            else if (value == str.Length)  // Add the Last Part
            {
                strResult += str.Substring(lastValue);
            }
        }
        return strResult;
    }


}