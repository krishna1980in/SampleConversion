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
using System.Collections;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace Kartris
{
    public class ChangePasswordAdapter : System.Web.UI.WebControls.Adapters.WebControlAdapter
    {
        private enum State
        {
            ChangePassword,
            Failed,
            Success
        }
        private State _state = State.ChangePassword;

        private WebControlAdapterExtender _extender = null/* TODO Change to default(_) if this is not a reference type */;
        private WebControlAdapterExtender Extender
        {
            get
            {
                if (((IsNothing(_extender) && (!Information.IsNothing(System.Web.UI.WebControls.Adapters.WebControlAdapter.Control))) || ((!IsNothing(_extender)) && (!System.Web.UI.WebControls.Adapters.WebControlAdapter.Control.Equals(_extender.AdaptedControl)))))
                    _extender = new WebControlAdapterExtender(System.Web.UI.WebControls.Adapters.WebControlAdapter.Control);

                System.Diagnostics.Debug.Assert(!IsNothing(_extender), "CSS Friendly adapters internal error", "Null extender instance");
                return _extender;
            }
        }

        public ChangePasswordAdapter()
        {
            _state = State.ChangePassword;
        }

        // / ///////////////////////////////////////////////////////////////////////////////
        // / PROTECTED        

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ChangePassword changePwd = System.Web.UI.WebControls.Adapters.WebControlAdapter.Control;
            if ((Extender.AdapterEnabled && (!Information.IsNothing(changePwd))))
            {
                RegisterScripts();

                changePwd.ChangedPassword += OnChangedPassword;
                changePwd.ChangePasswordError += OnChangePasswordError;
                _state = State.ChangePassword;
            }
        }

        protected override void CreateChildControls()
        {
            base.CreateChildControls();

            ChangePassword changePwd = System.Web.UI.WebControls.Adapters.WebControlAdapter.Control;
            if (((!Information.IsNothing(changePwd)) && (changePwd.Controls.Count == 2)))
            {
                if ((!Information.IsNothing(changePwd.ChangePasswordTemplate)))
                {
                    changePwd.ChangePasswordTemplateContainer.Controls.Clear();
                    changePwd.ChangePasswordTemplate.InstantiateIn(changePwd.ChangePasswordTemplateContainer);
                    changePwd.ChangePasswordTemplateContainer.DataBind();
                }

                if ((!Information.IsNothing(changePwd.SuccessTemplate)))
                {
                    changePwd.SuccessTemplateContainer.Controls.Clear();
                    changePwd.SuccessTemplate.InstantiateIn(changePwd.SuccessTemplateContainer);
                    changePwd.SuccessTemplateContainer.DataBind();
                }

                changePwd.Controls.Add(new ChangePasswordCommandBubbler());
            }
        }

        protected void OnChangedPassword(object sender, EventArgs e)
        {
            _state = State.Success;
        }

        protected void OnChangePasswordError(object sender, EventArgs e)
        {
            if ((_state != State.Success))
                _state = State.Failed;
        }

        protected override void RenderBeginTag(HtmlTextWriter writer)
        {
            if ((Extender.AdapterEnabled))
                Extender.RenderBeginTag(writer, "Kartris-ChangePassword");
            else
                base.RenderBeginTag(writer);
        }

        protected override void RenderEndTag(HtmlTextWriter writer)
        {
            if ((Extender.AdapterEnabled))
                Extender.RenderEndTag(writer);
            else
                base.RenderEndTag(writer);
        }

        protected override void RenderContents(HtmlTextWriter writer)
        {
            if ((Extender.AdapterEnabled))
            {
                ChangePassword changePwd = System.Web.UI.WebControls.Adapters.WebControlAdapter.Control;
                if ((!Information.IsNothing(changePwd)))
                {
                    if (((_state == State.ChangePassword) || (_state == State.Failed)))
                    {
                        if ((!Information.IsNothing(changePwd.ChangePasswordTemplate)))
                            changePwd.ChangePasswordTemplateContainer.RenderControl(writer);
                        else
                        {
                            WriteChangePasswordTitlePanel(writer, changePwd);
                            WriteInstructionPanel(writer, changePwd);
                            WriteHelpPanel(writer, changePwd);
                            WriteUserPanel(writer, changePwd);
                            WritePasswordPanel(writer, changePwd);
                            WriteNewPasswordPanel(writer, changePwd);
                            WriteConfirmNewPasswordPanel(writer, changePwd);
                            if ((_state == State.Failed))
                                WriteFailurePanel(writer, changePwd);
                            WriteSubmitPanel(writer, changePwd);
                            WriteCreateUserPanel(writer, changePwd);
                            WritePasswordRecoveryPanel(writer, changePwd);
                        }
                    }
                    else if ((_state == State.Success))
                    {
                        if ((!Information.IsNothing(changePwd.SuccessTemplate)))
                            changePwd.SuccessTemplateContainer.RenderControl(writer);
                        else
                        {
                            WriteSuccessTitlePanel(writer, changePwd);
                            WriteSuccessTextPanel(writer, changePwd);
                            WriteContinuePanel(writer, changePwd);
                        }
                    }
                }
            }
            else
                base.RenderContents(writer);
        }

        // / ///////////////////////////////////////////////////////////////////////////////
        // / PRIVATE        

        private void RegisterScripts()
        {
        }

        // ///////////////////////////////////////////////////////
        // Step 1: change password
        // ///////////////////////////////////////////////////////

        private void WriteChangePasswordTitlePanel(HtmlTextWriter writer, ChangePassword changePwd)
        {
            if ((!string.IsNullOrEmpty(changePwd.ChangePasswordTitleText)))
            {
                string className = "";
                if (((!Information.IsNothing(changePwd.TitleTextStyle)) && (!string.IsNullOrEmpty(changePwd.TitleTextStyle.CssClass))))
                    className = changePwd.TitleTextStyle.CssClass + " ";
                className += "Kartris-ChangePassword-ChangePasswordTitlePanel";
                WebControlAdapterExtender.WriteBeginDiv(writer, className, "");
                WebControlAdapterExtender.WriteSpan(writer, "", changePwd.ChangePasswordTitleText);
                WebControlAdapterExtender.WriteEndDiv(writer);
            }
        }

        private void WriteInstructionPanel(HtmlTextWriter writer, ChangePassword changePwd)
        {
            if ((!string.IsNullOrEmpty(changePwd.InstructionText)))
            {
                string className = "";
                if (((!Information.IsNothing(changePwd.InstructionTextStyle)) && (!string.IsNullOrEmpty(changePwd.InstructionTextStyle.CssClass))))
                    className = changePwd.InstructionTextStyle.CssClass + " ";
                className += "Kartris-ChangePassword-InstructionPanel";
                WebControlAdapterExtender.WriteBeginDiv(writer, className, "");
                WebControlAdapterExtender.WriteSpan(writer, "", changePwd.InstructionText);
                WebControlAdapterExtender.WriteEndDiv(writer);
            }
        }

        private void WriteFailurePanel(HtmlTextWriter writer, ChangePassword changePwd)
        {
            string className = "";
            if (((!Information.IsNothing(changePwd.FailureTextStyle)) && (!string.IsNullOrEmpty(changePwd.FailureTextStyle.CssClass))))
                className = changePwd.FailureTextStyle.CssClass + " ";
            className += "Kartris-ChangePassword-FailurePanel";
            WebControlAdapterExtender.WriteBeginDiv(writer, className, "");
            WebControlAdapterExtender.WriteSpan(writer, "", changePwd.ChangePasswordFailureText);
            WebControlAdapterExtender.WriteEndDiv(writer);
        }

        private void WriteHelpPanel(HtmlTextWriter writer, ChangePassword changePwd)
        {
            if (((!string.IsNullOrEmpty(changePwd.HelpPageIconUrl)) || (!string.IsNullOrEmpty(changePwd.HelpPageText))))
            {
                WebControlAdapterExtender.WriteBeginDiv(writer, "Kartris-ChangePassword-HelpPanel", "");
                WebControlAdapterExtender.WriteImage(writer, changePwd.HelpPageIconUrl, "Help");
                WebControlAdapterExtender.WriteLink(writer, changePwd.HyperLinkStyle.CssClass, changePwd.HelpPageUrl, "Help", changePwd.HelpPageText);
                WebControlAdapterExtender.WriteEndDiv(writer);
            }
        }

        private void WriteUserPanel(HtmlTextWriter writer, ChangePassword changePwd)
        {
            if ((changePwd.DisplayUserName))
            {
                TextBox textbox = changePwd.ChangePasswordTemplateContainer.FindControl("UserName");
                if ((!Information.IsNothing(textbox)))
                {
                    System.Web.UI.Adapters.ControlAdapter.Page.ClientScript.RegisterForEventValidation(textbox.UniqueID);
                    WebControlAdapterExtender.WriteBeginDiv(writer, "Kartris-ChangePassword-UserPanel", "");
                    Extender.WriteTextBox(writer, false, changePwd.LabelStyle.CssClass, changePwd.UserNameLabelText, changePwd.TextBoxStyle.CssClass, changePwd.ChangePasswordTemplateContainer.ID + "_UserName", changePwd.UserName);
                    WebControlAdapterExtender.WriteRequiredFieldValidator(writer, (RequiredFieldValidator)changePwd.ChangePasswordTemplateContainer.FindControl("UserNameRequired"), changePwd.ValidatorTextStyle.CssClass, "UserName", changePwd.UserNameRequiredErrorMessage);
                    WebControlAdapterExtender.WriteEndDiv(writer);
                }
            }
        }

        private void WritePasswordPanel(HtmlTextWriter writer, ChangePassword changePwd)
        {
            TextBox textbox = changePwd.ChangePasswordTemplateContainer.FindControl("CurrentPassword");
            if ((!Information.IsNothing(textbox)))
            {
                System.Web.UI.Adapters.ControlAdapter.Page.ClientScript.RegisterForEventValidation(textbox.UniqueID);
                WebControlAdapterExtender.WriteBeginDiv(writer, "Kartris-ChangePassword-PasswordPanel", "");
                Extender.WriteTextBox(writer, true, changePwd.LabelStyle.CssClass, changePwd.PasswordLabelText, changePwd.TextBoxStyle.CssClass, changePwd.ChangePasswordTemplateContainer.ID + "_CurrentPassword", "");
                WebControlAdapterExtender.WriteRequiredFieldValidator(writer, (RequiredFieldValidator)changePwd.ChangePasswordTemplateContainer.FindControl("CurrentPasswordRequired"), changePwd.ValidatorTextStyle.CssClass, "CurrentPassword", changePwd.PasswordRequiredErrorMessage);
                WebControlAdapterExtender.WriteEndDiv(writer);
            }
        }

        private void WriteNewPasswordPanel(HtmlTextWriter writer, ChangePassword changePwd)
        {
            TextBox textbox = changePwd.ChangePasswordTemplateContainer.FindControl("NewPassword");
            if ((!Information.IsNothing(textbox)))
            {
                System.Web.UI.Adapters.ControlAdapter.Page.ClientScript.RegisterForEventValidation(textbox.UniqueID);
                WebControlAdapterExtender.WriteBeginDiv(writer, "Kartris-ChangePassword-NewPasswordPanel", "");
                Extender.WriteTextBox(writer, true, changePwd.LabelStyle.CssClass, changePwd.NewPasswordLabelText, changePwd.TextBoxStyle.CssClass, changePwd.ChangePasswordTemplateContainer.ID + "_NewPassword", "");
                WebControlAdapterExtender.WriteRequiredFieldValidator(writer, (RequiredFieldValidator)changePwd.ChangePasswordTemplateContainer.FindControl("NewPasswordRequired"), changePwd.ValidatorTextStyle.CssClass, "NewPassword", changePwd.NewPasswordRequiredErrorMessage);
                WebControlAdapterExtender.WriteRegularExpressionValidator(writer, (RegularExpressionValidator)changePwd.ChangePasswordTemplateContainer.FindControl("RegExpValidator"), changePwd.ValidatorTextStyle.CssClass, "NewPassword", changePwd.NewPasswordRegularExpressionErrorMessage, changePwd.NewPasswordRegularExpression);
                WebControlAdapterExtender.WriteEndDiv(writer);
            }
        }

        private void WriteConfirmNewPasswordPanel(HtmlTextWriter writer, ChangePassword changePwd)
        {
            TextBox textbox = changePwd.ChangePasswordTemplateContainer.FindControl("ConfirmNewPassword");
            if ((!Information.IsNothing(textbox)))
            {
                System.Web.UI.Adapters.ControlAdapter.Page.ClientScript.RegisterForEventValidation(textbox.UniqueID);
                WebControlAdapterExtender.WriteBeginDiv(writer, "Kartris-ChangePassword-ConfirmNewPasswordPanel", "");
                Extender.WriteTextBox(writer, true, changePwd.LabelStyle.CssClass, changePwd.ConfirmNewPasswordLabelText, changePwd.TextBoxStyle.CssClass, changePwd.ChangePasswordTemplateContainer.ID + "_ConfirmNewPassword", "");
                WebControlAdapterExtender.WriteRequiredFieldValidator(writer, (RequiredFieldValidator)changePwd.ChangePasswordTemplateContainer.FindControl("ConfirmNewPasswordRequired"), changePwd.ValidatorTextStyle.CssClass, "ConfirmNewPassword", changePwd.ConfirmPasswordRequiredErrorMessage);
                WebControlAdapterExtender.WriteCompareValidator(writer, (CompareValidator)changePwd.ChangePasswordTemplateContainer.FindControl("NewPasswordCompare"), changePwd.ValidatorTextStyle.CssClass, "ConfirmNewPassword", changePwd.ConfirmPasswordCompareErrorMessage, "NewPassword");
                WebControlAdapterExtender.WriteEndDiv(writer);
            }
        }

        private void WriteSubmitPanel(HtmlTextWriter writer, ChangePassword changePwd)
        {
            WebControlAdapterExtender.WriteBeginDiv(writer, "Kartris-ChangePassword-SubmitPanel", "");

            string id = "ChangePassword";
            id += Interaction.IIf(changePwd.ChangePasswordButtonType == ButtonType.Button, "Push", "");
            string idWithType = WebControlAdapterExtender.MakeIdWithButtonType(id, changePwd.ChangePasswordButtonType);
            Control btn = changePwd.ChangePasswordTemplateContainer.FindControl(idWithType);
            if ((!Information.IsNothing(btn)))
            {
                System.Web.UI.Adapters.ControlAdapter.Page.ClientScript.RegisterForEventValidation(btn.UniqueID);

                PostBackOptions options = new PostBackOptions(btn, "", "", false, false, false, true, true, changePwd.UniqueID);
                string javascript = "javascript:" + System.Web.UI.Adapters.ControlAdapter.Page.ClientScript.GetPostBackEventReference(options);
                javascript = System.Web.UI.Adapters.ControlAdapter.Page.Server.HtmlEncode(javascript);

                Extender.WriteSubmit(writer, changePwd.ChangePasswordButtonType, changePwd.ChangePasswordButtonStyle.CssClass, changePwd.ChangePasswordTemplateContainer.ID + "_" + id, changePwd.ChangePasswordButtonImageUrl, javascript, changePwd.ChangePasswordButtonText);
            }

            id = "Cancel";
            id += Interaction.IIf(changePwd.ChangePasswordButtonType == ButtonType.Button, "Push", "");
            idWithType = WebControlAdapterExtender.MakeIdWithButtonType(id, changePwd.CancelButtonType);
            btn = changePwd.ChangePasswordTemplateContainer.FindControl(idWithType);
            if ((!Information.IsNothing(btn)))
            {
                System.Web.UI.Adapters.ControlAdapter.Page.ClientScript.RegisterForEventValidation(btn.UniqueID);
                Extender.WriteSubmit(writer, changePwd.CancelButtonType, changePwd.CancelButtonStyle.CssClass, changePwd.ChangePasswordTemplateContainer.ID + "_" + id, changePwd.CancelButtonImageUrl, "", changePwd.CancelButtonText);
            }

            WebControlAdapterExtender.WriteEndDiv(writer);
        }

        private void WriteCreateUserPanel(HtmlTextWriter writer, ChangePassword changePwd)
        {
            if (((!string.IsNullOrEmpty(changePwd.CreateUserUrl)) || (!string.IsNullOrEmpty(changePwd.CreateUserText))))
            {
                WebControlAdapterExtender.WriteBeginDiv(writer, "Kartris-ChangePassword-CreateUserPanel", "");
                WebControlAdapterExtender.WriteImage(writer, changePwd.CreateUserIconUrl, "Create user");
                WebControlAdapterExtender.WriteLink(writer, changePwd.HyperLinkStyle.CssClass, changePwd.CreateUserUrl, "Create user", changePwd.CreateUserText);
                WebControlAdapterExtender.WriteEndDiv(writer);
            }
        }

        private void WritePasswordRecoveryPanel(HtmlTextWriter writer, ChangePassword changePwd)
        {
            if (((!string.IsNullOrEmpty(changePwd.PasswordRecoveryUrl)) || (!string.IsNullOrEmpty(changePwd.PasswordRecoveryText))))
            {
                WebControlAdapterExtender.WriteBeginDiv(writer, "Kartris-ChangePassword-PasswordRecoveryPanel", "");
                WebControlAdapterExtender.WriteImage(writer, changePwd.PasswordRecoveryIconUrl, "Password recovery");
                WebControlAdapterExtender.WriteLink(writer, changePwd.HyperLinkStyle.CssClass, changePwd.PasswordRecoveryUrl, "Password recovery", changePwd.PasswordRecoveryText);
                WebControlAdapterExtender.WriteEndDiv(writer);
            }
        }

        // ///////////////////////////////////////////////////////
        // Step 2: success
        // ///////////////////////////////////////////////////////

        private void WriteSuccessTitlePanel(HtmlTextWriter writer, ChangePassword changePwd)
        {
            if ((!string.IsNullOrEmpty(changePwd.SuccessTitleText)))
            {
                string className = "";
                if (((!Information.IsNothing(changePwd.TitleTextStyle)) && (!string.IsNullOrEmpty(changePwd.TitleTextStyle.CssClass))))
                    className = changePwd.TitleTextStyle.CssClass + " ";
                className += "Kartris-ChangePassword-SuccessTitlePanel";
                WebControlAdapterExtender.WriteBeginDiv(writer, className, "");
                WebControlAdapterExtender.WriteSpan(writer, "", changePwd.SuccessTitleText);
                WebControlAdapterExtender.WriteEndDiv(writer);
            }
        }

        private void WriteSuccessTextPanel(HtmlTextWriter writer, ChangePassword changePwd)
        {
            if ((!string.IsNullOrEmpty(changePwd.SuccessText)))
            {
                string className = "";
                if (((!Information.IsNothing(changePwd.SuccessTextStyle)) && (!string.IsNullOrEmpty(changePwd.SuccessTextStyle.CssClass))))
                    className = changePwd.SuccessTextStyle.CssClass + " ";
                className += "Kartris-ChangePassword-SuccessTextPanel";
                WebControlAdapterExtender.WriteBeginDiv(writer, className, "");
                WebControlAdapterExtender.WriteSpan(writer, "", changePwd.SuccessText);
                WebControlAdapterExtender.WriteEndDiv(writer);
            }
        }

        private void WriteContinuePanel(HtmlTextWriter writer, ChangePassword changePwd)
        {
            WebControlAdapterExtender.WriteBeginDiv(writer, "Kartris-ChangePassword-ContinuePanel", "");

            string id = "Continue";
            id += Interaction.IIf((changePwd.ChangePasswordButtonType == ButtonType.Button), "Push", "");
            string idWithType = WebControlAdapterExtender.MakeIdWithButtonType(id, changePwd.ContinueButtonType);
            Control btn = changePwd.SuccessTemplateContainer.FindControl(idWithType);
            if ((!Information.IsNothing(btn)))
            {
                System.Web.UI.Adapters.ControlAdapter.Page.ClientScript.RegisterForEventValidation(btn.UniqueID);
                Extender.WriteSubmit(writer, changePwd.ContinueButtonType, changePwd.ContinueButtonStyle.CssClass, changePwd.SuccessTemplateContainer.ID + "_" + id, changePwd.ContinueButtonImageUrl, "", changePwd.ContinueButtonText);
            }

            WebControlAdapterExtender.WriteEndDiv(writer);
        }
    }

    public class ChangePasswordCommandBubbler : Control
    {
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            if ((Control.Page.IsPostBack))
            {
                ChangePassword changePassword = Control.NamingContainer;
                if ((!Information.IsNothing(changePassword)))
                {
                    Control container = changePassword.ChangePasswordTemplateContainer;
                    if ((!Information.IsNothing(container)))
                    {
                        CommandEventArgs cmdArgs = null;
                        string[] prefixes = new[] { "ChangePassword", "Cancel", "Continue" };

                        string[] postfixes = new[] { "PushButton", "Image", "Link" };

                        string prefix;
                        foreach (var prefix in prefixes)
                        {
                            string postfix;
                            foreach (var postfix in postfixes)
                            {
                                string id = prefix + postfix;
                                Control ctrl = container.FindControl(id);
                                if (((!Information.IsNothing(ctrl)) && (!string.IsNullOrEmpty(Control.Page.Request.Params.Get(ctrl.UniqueID)))))
                                {
                                    switch (prefix)
                                    {
                                        case "ChangePassword":
                                            {
                                                cmdArgs = new CommandEventArgs(changePassword.ChangePasswordButtonCommandName, this);
                                                break;
                                                break;
                                            }

                                        case "Cancel":
                                            {
                                                cmdArgs = new CommandEventArgs(changePassword.CancelButtonCommandName, this);
                                                break;
                                                break;
                                            }

                                        case "Continue":
                                            {
                                                cmdArgs = new CommandEventArgs(changePassword.ContinueButtonCommandName, this);
                                                break;
                                                break;
                                            }
                                    }
                                }
                            }
                            if ((!Information.IsNothing(cmdArgs)))
                                break;
                        }

                        if (((!Information.IsNothing(cmdArgs)) && (cmdArgs.CommandName == changePassword.ChangePasswordButtonCommandName)))
                        {
                            Control.Page.Validate();
                            if ((!Control.Page.IsValid))
                                cmdArgs = null;
                        }

                        if ((!Information.IsNothing(cmdArgs)))
                            Control.RaiseBubbleEvent(this, cmdArgs);
                    }
                }
            }
        }
    }
}
