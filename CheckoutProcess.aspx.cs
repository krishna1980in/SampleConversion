using System;
using System.Web.UI.WebControls;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace EmployeeManagementSystem
{
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
    partial class checkout_process : System.Web.UI.Page
    {
        private Kartris.Interfaces.PaymentGateway clsPlugin;

        public checkout_process()
        {
            Load += Page_Load;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            string strOutput = "";
            string strGatewayStatus = "";
            string strGatewayName = "";
            if (Session["_CallBackMessage"] is not null & Session["_PostBackURL"] is not null)
            {
                strOutput = Conversions.ToString(Session["_CallBackMessage"]);
                btnSubmit.PostBackUrl = Session["_PostBackURL"];
            }
            else
            {
                Kartris.Basket BasketObject = (Kartris.Basket)Session["BasketObject"];
                strGatewayName = Conversions.ToString(Session["GateWayName"]);
                Kartris.Interfaces.PaymentGateway clsPlugin;
                clsPlugin = Payment.PPLoader(strGatewayName);
                var GatewayBasketBLL = new Kartris.Basket();
                string BasketXML;
                if (clsPlugin.RequiresBasketItems)
                {
                    int intGatewayCurrencyID = 0;
                    if (!(string.IsNullOrEmpty(clsPlugin.Currency) | string.IsNullOrWhiteSpace(clsPlugin.Currency)))
                    {
                        intGatewayCurrencyID = CurrenciesBLL.CurrencyID(clsPlugin.Currency);
                    }
                    if (intGatewayCurrencyID > 0 && intGatewayCurrencyID != Conversions.ToInteger(Session["CUR_ID"]))
                    {
                        {
                            ref var withBlock = ref GatewayBasketBLL;
                            // Retrieve Basket Items from Session
                            withBlock.CurrencyID = intGatewayCurrencyID;
                            withBlock.LoadBasketItems();
                            withBlock.Validate(false);
                            withBlock.CalculateTotals();
                        }
                    }
                    else
                    {
                        GatewayBasketBLL = BasketObject;
                    }

                    // Hack, remove promotions as these cause issues 
                    // with serialization at gateways later
                    GatewayBasketBLL.objPromotions.Clear();
                    GatewayBasketBLL.objPromotionsDiscount.Clear();

                    BasketXML = Payment.Serialize(GatewayBasketBLL);
                }
                else
                {
                    BasketXML = null;
                }

                strOutput = clsPlugin.ProcessOrder(Session["objOrder"], BasketXML);
                btnSubmit.PostBackUrl = clsPlugin.PostbackURL;
                strGatewayStatus = clsPlugin.Status;
                clsPlugin = default;
            }

            switch (Strings.LCase(strGatewayStatus) ?? "")
            {
                case "test":
                    {
                        litGatewayTestForwarding.Text = GetLocalResourceObject("ContentText_GatewayTestForwarding");
                        litGatewayTestForwarding.Visible = true;
                        break;
                    }
                case "fake":
                    {
                        litGatewayTestForwarding.Text = GetLocalResourceObject("ContentText_GatewayFake");
                        litGatewayTestForwarding.Visible = true;
                        break;
                    }

                default:
                    {
                        litGatewayTestForwarding.Visible = false;
                        string strScript = string.Format("document.getElementById('{0}').disabled = true;", btnSubmit.ClientID);
                        Page.ClientScript.RegisterOnSubmitStatement(Page.GetType(), "disabledSubmitButton", strScript);
                        Page.ClientScript.RegisterStartupScript(Page.GetType(), "Submit", string.Format("document.getElementById('{0}').click();", btnSubmit.ClientID), true);
                        btnSubmit.Text = GetGlobalResourceObject("Basket", "ContentText_PleaseWait"); // "Redirecting to payment gateway.*"
                        break;
                    }
            }


            GateWayPanel.Visible = true;
            // modify submit button's properties
            btnSubmit.UseSubmitBehavior = true;
            MainPanel.Visible = false;
            Page.Title = Conversions.ToString(Operators.ConcatenateObject(Operators.ConcatenateObject(GetGlobalResourceObject("Basket", "FormButton_Checkout"), " | "), Server.HtmlEncode(Conversions.ToString(GetGlobalResourceObject("Kartris", "Config_Webshopname")))));

            Session["objOrder"] = null;
            Session["BasketObject"] = null;

            if (!string.IsNullOrEmpty(strOutput))
            {

                var arrOutput = Strings.Split(strOutput, ":-:");
                string[] arrPair;

                if (arrOutput is not null && arrOutput.Count == 1 && Strings.InStr(arrOutput[0].ToString(), ":*:") == 0)
                {
                    CkartrisFormatErrors.LogError("Can't process plugin output file - " + strGatewayName + " output:" + strOutput);
                }

                foreach (var strPair in arrOutput)
                {
                    arrPair = Strings.Split(strPair, ":*:");
                    if (Information.UBound(arrPair) > 0)
                    {
                        if (Strings.UCase(arrPair[0]) == "FORM_NAME")
                        {
                            Page.Form.Name = arrPair[1];
                        }
                        else if (Strings.LCase(strGatewayStatus) == "fake")
                        {
                            {
                                var withBlock1 = GateWayPanel.Controls;
                                var litLineStart = new Literal();
                                litLineStart.Text = "<li><span class=\"Kartris-DetailsView-Name\">";
                                withBlock1.Add(litLineStart);

                                var lblControl = new Label();
                                lblControl.Text = arrPair[0] + ": ";
                                withBlock1.Add(lblControl);

                                // Middle of line
                                var litLineMiddle = new Literal();
                                litLineMiddle.Text = "</span><span class=\"Kartris-DetailsView-Value\">";
                                withBlock1.Add(litLineMiddle);

                                var txtControl = new TextBox();
                                txtControl.ID = arrPair[0];
                                txtControl.Text = arrPair[1];
                                withBlock1.Add(txtControl);

                                // End of line
                                var litLineEnd = new Literal();
                                litLineEnd.Text = "</span></li>";
                                withBlock1.Add(litLineEnd);
                            }
                        }
                        else
                        {
                            var hfPair = new HiddenField();
                            Kartris.Interfaces.Utils.SetHF(hfPair, arrPair[0], arrPair[1]);
                            GateWayPanel.Controls.Add(hfPair);
                        }
                    }
                }
            }
        }

    }
}