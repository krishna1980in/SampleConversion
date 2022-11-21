// ========================================================================
// Kartris - www.kartris.com
// Copyright 2016 CACTUSOFT

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
using Microsoft.VisualBasic;
using kartrisPagesDataTableAdapters;
using System.Web.HttpContext;
using CkartrisEnumerations;
using CkartrisFormatErrors;
using Braintree;

public class PaymentsBLL
{
    public static readonly TransactionStatus[] transactionSuccessStatuses = new[] {
        TransactionStatus.AUTHORIZED,
        TransactionStatus.AUTHORIZING,
        TransactionStatus.SETTLED,
        TransactionStatus.SETTLING,
        TransactionStatus.SETTLEMENT_CONFIRMED,
        TransactionStatus.SETTLEMENT_PENDING,
        TransactionStatus.SUBMITTED_FOR_SETTLEMENT
    };

    // <summary>
    // Braintree Transaction Request   
    // </summary>
    public static string BrainTreePayment(string paymentMethodNonce, decimal amount, short currencyId)
    {
        BraintreeGateway gateway;
        gateway = BrainTreePayment.BT_GetGateway();

        string merchAccId = "MerchantAccId_";
        string CurrencyCode = "";

        CurrencyCode = CurrenciesBLL.CurrencyCode(currencyId);
        // merchAccId = merchAccId.Concat(merchAccId, CurrencyCode)
        merchAccId = merchAccId + CurrencyCode;
        string gatewayCurrencyCode = BrainTreePayment.BT_GetBrainTreeConfigMember("ProcessCurrency");
        short gatewayCurrencyId = CurrenciesBLL.CurrencyID(gatewayCurrencyCode);
        merchAccId = BrainTreePayment.BT_GetBrainTreeConfigMember(merchAccId);
        TransactionRequest tRequest = new TransactionRequest();
        decimal convertedAmount = 0;
        if (merchAccId.Equals(""))
        {
            convertedAmount = Math.Round(CurrenciesBLL.ConvertCurrency(gatewayCurrencyId, amount, currencyId), BasketBLL.CurrencyRoundNumber);
            amount = convertedAmount;
        }
        else
            tRequest.MerchantAccountId = merchAccId;

        tRequest.Amount = amount;
        tRequest.PaymentMethodNonce = paymentMethodNonce;
        if (BrainTreePayment.BT_GetBrainTreeConfigMember("SubmitSettlement").ToLower().Equals("y"))
        {
            tRequest.Options = new TransactionOptionsRequest();
            tRequest.Options.SubmitForSettlement = true;
        }

        Result<Transaction> result = gateway.Transaction.Sale(tRequest);
        if (result.IsSuccess())
        {
            Transaction transaction = result.Target;
            return transaction.Id;
        }
        else if (!transactionSuccessStatuses.Contains(result.Transaction.Status))
            // ---------------------------------------
            // Something went wrong with the BrainTree Transaction
            // 
            // Throwing Exception
            // ---------------------------------------
            throw new BrainTreeException("Your transaction has a status of ", result.Transaction.Id, result.Message);
        else
        {
            // ---------------------------------------
            // Something went wrong with the BrainTree Transaction
            // 
            // Throwing Exception
            // ---------------------------------------
            string errorMessages = "";
            foreach (ValidationError singleError in result.Errors.DeepAll())
                errorMessages += "Error: " + Int32.Parse(singleError.Code) + " - " + singleError.Message + @"\n";
            throw new BrainTreeException(errorMessages, result.Transaction.Id);
        }
    }

    // <summary>
    // Generates the Braintree client token (necessary to call the Braintree Form)  
    // </summary>
    public static string GenerateClientToken()
    {
        BraintreeGateway gateway = new BraintreeGateway();
        string clientToken = "";

        gateway = BrainTreePayment.BT_GetGateway();
        clientToken = BT_GenerateClientToken(gateway);

        return clientToken;
    }
}

// <summary>
// BrainTree Aux. Class
// To avoid having a new Lib on the BIN folder
// </summary>
static class BrainTreePayment
{
    public static string BT_GenerateClientToken(BraintreeGateway gateway)
    {
        string clientToken;
        clientToken = gateway.ClientToken.generate();
        return clientToken;
    }

    public static BraintreeGateway BT_CreateGateway(string merchantId, string publicKey, string privateKey)
    {
        string status = BT_GetBrainTreeConfigMember("Status");
        Environment environment = Braintree.Environment.SANDBOX;
        if (status.ToLower().Equals("test"))
            environment = Braintree.Environment.SANDBOX;
        else
            environment = Braintree.Environment.PRODUCTION;
        BraintreeGateway gateway = new BraintreeGateway(environment, merchantId, publicKey, privateKey);
        return gateway;
    }

    public static BraintreeGateway BT_GetGateway()
    {
        BraintreeGateway gateway = new BraintreeGateway();
        string[] btConfig = BT_GetBrainTreeGatewayStartConfig();
        gateway = BT_CreateGateway(btConfig[0], btConfig[1], btConfig[2]);
        return gateway;
    }

    public static string[] BT_GetBrainTreeGatewayStartConfig()
    {
        try
        {
            string[] strSettings = new[] { "MerchantId", "PublicKey", "PrivateKey" };
            string[] toReturn = new[] { "", "", "" };

            ExeConfigurationFileMap objConfigFileMap = new ExeConfigurationFileMap();
            objConfigFileMap.ExeConfigFilename = Path.Combine(System.Web.HttpContext.Current.Request.PhysicalApplicationPath, @"Plugins\BrainTreePayment\BrainTreePayment.dll.config");
            objConfigFileMap.MachineConfigFilename = Path.Combine(System.Web.HttpContext.Current.Request.PhysicalApplicationPath, @"Uploads\resources\Machine.Config");
            System.Configuration.Configuration objConfiguration = ConfigurationManager.OpenMappedExeConfiguration(objConfigFileMap, ConfigurationUserLevel.None);

            ConfigurationSectionGroup objSectionGroup = objConfiguration.GetSectionGroup("applicationSettings");
            ClientSettingsSection appSettingsSection = (ClientSettingsSection)objSectionGroup.Sections.Item("Kartris.My.MySettings");
            for (int index = 0; index <= strSettings.Length() - 1; index++)
                toReturn[index] = appSettingsSection.Settings.Get(strSettings[index]).Value.ValueXml.InnerText;
            return toReturn;
        }
        catch (Exception ex)
        {
            return new[] { };
        }
    }

    public static string BT_GetBrainTreeConfigMember(string member)
    {
        try
        {
            string toReturn = "";
            string merchAccId = "";

            ExeConfigurationFileMap objConfigFileMap = new ExeConfigurationFileMap();
            objConfigFileMap.ExeConfigFilename = Path.Combine(System.Web.HttpContext.Current.Request.PhysicalApplicationPath, @"Plugins\BrainTreePayment\BrainTreePayment.dll.config");
            objConfigFileMap.MachineConfigFilename = Path.Combine(System.Web.HttpContext.Current.Request.PhysicalApplicationPath, @"Uploads\resources\Machine.Config");
            System.Configuration.Configuration objConfiguration = ConfigurationManager.OpenMappedExeConfiguration(objConfigFileMap, ConfigurationUserLevel.None);

            ConfigurationSectionGroup objSectionGroup = objConfiguration.GetSectionGroup("applicationSettings");
            ClientSettingsSection appSettingsSection = (ClientSettingsSection)objSectionGroup.Sections.Item("Kartris.My.MySettings");
            toReturn = appSettingsSection.Settings.Get(member).Value.ValueXml.InnerText;

            return toReturn;
        }
        catch (Exception ex)
        {
            return "";
        }
    }
}

// <summary>
// User defined exception
// 
// Added _transactionId returned by Braintree
// Added _status (the error, ex: gateway_rejected)
// Added _customMessage (Message + _status)
// </summary>
public class BrainTreeException : Exception
{
    private string _transactionId;
    private string _status;
    private string _customMessage;

    public string TransactionId
    {
        get
        {
            return _transactionId;
        }
        set
        {
            _transactionId = value;
        }
    }
    public string Status
    {
        get
        {
            return _status;
        }
        set
        {
            _status = value;
        }
    }
    public string CustomMessage
    {
        get
        {
            return _customMessage;
        }
        set
        {
            _customMessage = value;
        }
    }

    public BrainTreeException() : base()
    {
    }

    public BrainTreeException(string message) : base(message)
    {
    }

    public BrainTreeException(string Message, Exception inner) : base(Message, inner)
    {
    }

    public BrainTreeException(string Message, string transactionId) : base(Message)
    {
        this.TransactionId = transactionId;
    }


    public BrainTreeException(string Message, string transactionId, string status) : base(Message)
    {
        this.TransactionId = transactionId;
        this.Status = status;
        this.CustomMessage = this.Message + status;
    }
}
