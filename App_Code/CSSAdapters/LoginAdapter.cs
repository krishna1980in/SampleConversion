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
    public class LoginAdapter : System.Web.UI.WebControls.Adapters.WebControlAdapter
    {
        private enum State
        {
            LoggingIn,
            Failed,
            Success
        }
        private State _state = State.LoggingIn;

        protected WebControlAdapterExtender This = null/* TODO Change to default(_) if this is not a reference type */;
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

        public LoginAdapter()
        {
            _state = State.LoggingIn;
        }

        // / ///////////////////////////////////////////////////////////////////////////////
        // / PROTECTED        

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Login login = System.Web.UI.WebControls.Adapters.WebControlAdapter.Control;
            if ((Extender.AdapterEnabled && (!Information.IsNothing(login))))
            {
                RegisterScripts();

                login.LoggedIn += OnLoggedIn;
                login.LoginError += OnLoginError;
                _state = State.LoggingIn;
            }
        }

        protected override void CreateChildControls()
        {
            base.CreateChildControls();
            Login login = System.Web.UI.WebControls.Adapters.WebControlAdapter.Control;
            if (((!Information.IsNothing(login)) && (login.Controls.Count == 1) && (!Information.IsNothing(login.LayoutTemplate))))
            {
                Control container = login.Controls[0];
                if ((!Information.IsNothing(container)))
                {
                    container.Controls.Clear();
                    login.LayoutTemplate.InstantiateIn(container);
                    container.DataBind();
                }
            }
        }

        protected void OnLoggedIn(object sender, EventArgs e)
        {
            _state = State.Success;
        }

        protected void OnLoginError(object sender, EventArgs e)
        {
            _state = State.Failed;
        }

        protected override void RenderBeginTag(HtmlTextWriter writer)
        {
            if (Extender.AdapterEnabled)
                Extender.RenderBeginTag(writer, "Kartris-Login");
            else
                base.RenderBeginTag(writer);
        }

        protected override void RenderEndTag(HtmlTextWriter writer)
        {
            if (Extender.AdapterEnabled)
                Extender.RenderEndTag(writer);
            else
                base.RenderEndTag(writer);
        }

        protected override void RenderContents(HtmlTextWriter writer)
        {
            if (Extender.AdapterEnabled)
            {
                Login login = System.Web.UI.WebControls.Adapters.WebControlAdapter.Control;
                if ((!Information.IsNothing(login)))
                {
                    if ((!Information.IsNothing(login.LayoutTemplate)))
                    {
                        if ((login.Controls.Count == 1))
                        {
                            Control container = login.Controls[0];
                            if ((!Information.IsNothing(container)))
                            {
                                Control c;
                                foreach (var c in container.Controls)
                                    c.RenderControl(writer);
                            }
                        }
                    }
                    else
                    {
                        WriteTitlePanel(writer, login);
                        WriteInstructionPanel(writer, login);
                        WriteHelpPanel(writer, login);
                        WriteUserPanel(writer, login);
                        WritePasswordPanel(writer, login);
                        WriteRememberMePanel(writer, login);
                        if ((_state == State.Failed))
                            WriteFailurePanel(writer, login);
                        WriteSubmitPanel(writer, login);
                        WriteCreateUserPanel(writer, login);
                        WritePasswordRecoveryPanel(writer, login);
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

        private void WriteTitlePanel(HtmlTextWriter writer, Login login)
        {
            if ((!string.IsNullOrEmpty(login.TitleText)))
            {
                string className = "";
                if (((!Information.IsNothing(login.TitleTextStyle)) && (!string.IsNullOrEmpty(login.TitleTextStyle.CssClass))))
                    className = login.TitleTextStyle.CssClass + " ";
                className += "Kartris-Login-TitlePanel";
                WebControlAdapterExtender.WriteBeginDiv(writer, className, "");
                WebControlAdapterExtender.WriteSpan(writer, "", login.TitleText);
                WebControlAdapterExtender.WriteEndDiv(writer);
            }
        }

        private void WriteInstructionPanel(HtmlTextWriter writer, Login login)
        {
            if ((!string.IsNullOrEmpty(login.InstructionText)))
            {
                string className = "";
                if (((!Information.IsNothing(login.InstructionTextStyle)) && (!string.IsNullOrEmpty(login.InstructionTextStyle.CssClass))))
                    className = login.InstructionTextStyle.CssClass + " ";
                className += "Kartris-Login-InstructionPanel";
                WebControlAdapterExtender.WriteBeginDiv(writer, className, "");
                WebControlAdapterExtender.WriteSpan(writer, "", login.InstructionText);
                WebControlAdapterExtender.WriteEndDiv(writer);
            }
        }

        private void WriteFailurePanel(HtmlTextWriter writer, Login login)
        {
            if ((!string.IsNullOrEmpty(login.FailureText)))
            {
                string className = "";
                if (((!Information.IsNothing(login.FailureTextStyle)) && (!string.IsNullOrEmpty(login.FailureTextStyle.CssClass))))
                    className = login.FailureTextStyle.CssClass + " ";
                className += "Kartris-Login-FailurePanel";
                WebControlAdapterExtender.WriteBeginDiv(writer, className, "");
                WebControlAdapterExtender.WriteSpan(writer, "", login.FailureText);
                WebControlAdapterExtender.WriteEndDiv(writer);
            }
        }

        private void WriteHelpPanel(HtmlTextWriter writer, Login login)
        {
            if (((!string.IsNullOrEmpty(login.HelpPageIconUrl)) || (!string.IsNullOrEmpty(login.HelpPageText))))
            {
                WebControlAdapterExtender.WriteBeginDiv(writer, "Kartris-Login-HelpPanel", "");
                WebControlAdapterExtender.WriteImage(writer, login.HelpPageIconUrl, "Help");
                WebControlAdapterExtender.WriteLink(writer, login.HyperLinkStyle.CssClass, login.HelpPageUrl, "Help", login.HelpPageText);
                WebControlAdapterExtender.WriteEndDiv(writer);
            }
        }

        private void WriteUserPanel(HtmlTextWriter writer, Login login)
        {
            System.Web.UI.Adapters.ControlAdapter.Page.ClientScript.RegisterForEventValidation(login.FindControl("UserName").UniqueID);
            WebControlAdapterExtender.WriteBeginDiv(writer, "Kartris-Login-UserPanel", "");
            Extender.WriteTextBox(writer, false, login.LabelStyle.CssClass, login.UserNameLabelText, login.TextBoxStyle.CssClass, "UserName", login.UserName);
            WebControlAdapterExtender.WriteRequiredFieldValidator(writer, (RequiredFieldValidator)login.FindControl("UserNameRequired"), login.ValidatorTextStyle.CssClass, "UserName", login.UserNameRequiredErrorMessage);
            WebControlAdapterExtender.WriteEndDiv(writer);
        }

        private void WritePasswordPanel(HtmlTextWriter writer, Login login)
        {
            System.Web.UI.Adapters.ControlAdapter.Page.ClientScript.RegisterForEventValidation(login.FindControl("Password").UniqueID);
            WebControlAdapterExtender.WriteBeginDiv(writer, "Kartris-Login-PasswordPanel", "");
            Extender.WriteTextBox(writer, true, login.LabelStyle.CssClass, login.PasswordLabelText, login.TextBoxStyle.CssClass, "Password", "");
            WebControlAdapterExtender.WriteRequiredFieldValidator(writer, (RequiredFieldValidator)login.FindControl("PasswordRequired"), login.ValidatorTextStyle.CssClass, "Password", login.PasswordRequiredErrorMessage);
            WebControlAdapterExtender.WriteEndDiv(writer);
        }

        private void WriteRememberMePanel(HtmlTextWriter writer, Login login)
        {
            if (login.DisplayRememberMe)
            {
                System.Web.UI.Adapters.ControlAdapter.Page.ClientScript.RegisterForEventValidation(login.FindControl("RememberMe").UniqueID);
                WebControlAdapterExtender.WriteBeginDiv(writer, "Kartris-Login-RememberMePanel", "");
                Extender.WriteCheckBox(writer, login.LabelStyle.CssClass, login.RememberMeText, login.CheckBoxStyle.CssClass, "RememberMe", login.RememberMeSet);
                WebControlAdapterExtender.WriteEndDiv(writer);
            }
        }

        private void WriteSubmitPanel(HtmlTextWriter writer, Login login)
        {
            string id = "Login";
            string idWithType = WebControlAdapterExtender.MakeIdWithButtonType(id, login.LoginButtonType);
            Control btn = login.FindControl(idWithType);
            if ((!Information.IsNothing(btn)))
            {
                System.Web.UI.Adapters.ControlAdapter.Page.ClientScript.RegisterForEventValidation(btn.UniqueID);

                WebControlAdapterExtender.WriteBeginDiv(writer, "Kartris-Login-SubmitPanel", "");

                PostBackOptions options = new PostBackOptions(btn, "", "", false, false, false, true, true, login.UniqueID);
                string javascript = "javascript:" + System.Web.UI.Adapters.ControlAdapter.Page.ClientScript.GetPostBackEventReference(options);
                javascript = System.Web.UI.Adapters.ControlAdapter.Page.Server.HtmlEncode(javascript);

                Extender.WriteSubmit(writer, login.LoginButtonType, login.LoginButtonStyle.CssClass, id, login.LoginButtonImageUrl, javascript, login.LoginButtonText);

                WebControlAdapterExtender.WriteEndDiv(writer);
            }
        }

        private void WriteCreateUserPanel(HtmlTextWriter writer, Login login)
        {
            if (((!string.IsNullOrEmpty(login.CreateUserUrl)) || (!string.IsNullOrEmpty(login.CreateUserText))))
            {
                WebControlAdapterExtender.WriteBeginDiv(writer, "Kartris-Login-CreateUserPanel", "");
                WebControlAdapterExtender.WriteImage(writer, login.CreateUserIconUrl, "Create user");
                WebControlAdapterExtender.WriteLink(writer, login.HyperLinkStyle.CssClass, login.CreateUserUrl, "Create user", login.CreateUserText);
                WebControlAdapterExtender.WriteEndDiv(writer);
            }
        }

        private void WritePasswordRecoveryPanel(HtmlTextWriter writer, Login login)
        {
            if (((!string.IsNullOrEmpty(login.PasswordRecoveryUrl)) || (!string.IsNullOrEmpty(login.PasswordRecoveryText))))
            {
                WebControlAdapterExtender.WriteBeginDiv(writer, "Kartris-Login-PasswordRecoveryPanel", "");
                WebControlAdapterExtender.WriteImage(writer, login.PasswordRecoveryIconUrl, "Password recovery");
                WebControlAdapterExtender.WriteLink(writer, login.HyperLinkStyle.CssClass, login.PasswordRecoveryUrl, "Password recovery", login.PasswordRecoveryText);
                WebControlAdapterExtender.WriteEndDiv(writer);
            }
        }
    }
}
