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
    public class CreateUserWizardAdapter : System.Web.UI.WebControls.Adapters.WebControlAdapter
    {
        private enum State
        {
            CreateUser,
            Failed,
            Success
        }
        private State _state = State.CreateUser;
        private string _currentErrorText = "";

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

        private MembershipProvider WizardMembershipProvider
        {
            get
            {
                MembershipProvider provider = Membership.Provider;
                CreateUserWizard wizard = System.Web.UI.WebControls.Adapters.WebControlAdapter.Control;
                ;/* Cannot convert MultiLineIfBlockSyntax, System.ArgumentOutOfRangeException: Index was out of range. Must be non-negative and less than the size of the collection.
Parameter name: index
   at System.ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument argument, ExceptionResource resource)
   at ICSharpCode.CodeConverter.CSharp.VisualBasicConverter.NodesVisitor.VisitSimpleArgument(SimpleArgumentSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleArgumentSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.Visit(SyntaxNode node)
   at ICSharpCode.CodeConverter.CSharp.CommentConvertingNodesVisitor.DefaultVisit(SyntaxNode node)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.VisitSimpleArgument(SimpleArgumentSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleArgumentSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at ICSharpCode.CodeConverter.CSharp.VisualBasicConverter.NodesVisitor.<>c__DisplayClass83_0.<ConvertArguments>b__0(ArgumentSyntax a, Int32 i)
   at System.Linq.Enumerable.<SelectIterator>d__5`2.MoveNext()
   at System.Linq.Enumerable.WhereEnumerableIterator`1.MoveNext()
   at Microsoft.CodeAnalysis.CSharp.SyntaxFactory.SeparatedList[TNode](IEnumerable`1 nodes)
   at ICSharpCode.CodeConverter.CSharp.VisualBasicConverter.NodesVisitor.VisitArgumentList(ArgumentListSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentListSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.Visit(SyntaxNode node)
   at ICSharpCode.CodeConverter.CSharp.CommentConvertingNodesVisitor.DefaultVisit(SyntaxNode node)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.VisitArgumentList(ArgumentListSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentListSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at ICSharpCode.CodeConverter.CSharp.VisualBasicConverter.NodesVisitor.VisitInvocationExpression(InvocationExpressionSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.InvocationExpressionSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.Visit(SyntaxNode node)
   at ICSharpCode.CodeConverter.CSharp.CommentConvertingNodesVisitor.DefaultVisit(SyntaxNode node)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.VisitInvocationExpression(InvocationExpressionSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.InvocationExpressionSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at ICSharpCode.CodeConverter.CSharp.VisualBasicConverter.NodesVisitor.VisitSimpleArgument(SimpleArgumentSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleArgumentSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.Visit(SyntaxNode node)
   at ICSharpCode.CodeConverter.CSharp.CommentConvertingNodesVisitor.DefaultVisit(SyntaxNode node)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.VisitSimpleArgument(SimpleArgumentSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleArgumentSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at ICSharpCode.CodeConverter.CSharp.VisualBasicConverter.NodesVisitor.<>c__DisplayClass83_0.<ConvertArguments>b__0(ArgumentSyntax a, Int32 i)
   at System.Linq.Enumerable.<SelectIterator>d__5`2.MoveNext()
   at System.Linq.Enumerable.WhereEnumerableIterator`1.MoveNext()
   at Microsoft.CodeAnalysis.CSharp.SyntaxFactory.SeparatedList[TNode](IEnumerable`1 nodes)
   at ICSharpCode.CodeConverter.CSharp.VisualBasicConverter.NodesVisitor.VisitArgumentList(ArgumentListSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentListSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.Visit(SyntaxNode node)
   at ICSharpCode.CodeConverter.CSharp.CommentConvertingNodesVisitor.DefaultVisit(SyntaxNode node)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.VisitArgumentList(ArgumentListSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentListSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at ICSharpCode.CodeConverter.CSharp.VisualBasicConverter.NodesVisitor.VisitInvocationExpression(InvocationExpressionSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.InvocationExpressionSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.Visit(SyntaxNode node)
   at ICSharpCode.CodeConverter.CSharp.CommentConvertingNodesVisitor.DefaultVisit(SyntaxNode node)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.VisitInvocationExpression(InvocationExpressionSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.InvocationExpressionSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at ICSharpCode.CodeConverter.CSharp.VisualBasicConverter.NodesVisitor.VisitUnaryExpression(UnaryExpressionSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.UnaryExpressionSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.Visit(SyntaxNode node)
   at ICSharpCode.CodeConverter.CSharp.CommentConvertingNodesVisitor.DefaultVisit(SyntaxNode node)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.VisitUnaryExpression(UnaryExpressionSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.UnaryExpressionSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at ICSharpCode.CodeConverter.CSharp.VisualBasicConverter.NodesVisitor.VisitParenthesizedExpression(ParenthesizedExpressionSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.ParenthesizedExpressionSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.Visit(SyntaxNode node)
   at ICSharpCode.CodeConverter.CSharp.CommentConvertingNodesVisitor.DefaultVisit(SyntaxNode node)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.VisitParenthesizedExpression(ParenthesizedExpressionSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.ParenthesizedExpressionSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at ICSharpCode.CodeConverter.CSharp.VisualBasicConverter.NodesVisitor.VisitBinaryExpression(BinaryExpressionSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.BinaryExpressionSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.Visit(SyntaxNode node)
   at ICSharpCode.CodeConverter.CSharp.CommentConvertingNodesVisitor.DefaultVisit(SyntaxNode node)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.VisitBinaryExpression(BinaryExpressionSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.BinaryExpressionSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at ICSharpCode.CodeConverter.CSharp.VisualBasicConverter.NodesVisitor.VisitParenthesizedExpression(ParenthesizedExpressionSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.ParenthesizedExpressionSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.Visit(SyntaxNode node)
   at ICSharpCode.CodeConverter.CSharp.CommentConvertingNodesVisitor.DefaultVisit(SyntaxNode node)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.VisitParenthesizedExpression(ParenthesizedExpressionSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.ParenthesizedExpressionSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at ICSharpCode.CodeConverter.CSharp.VisualBasicConverter.MethodBodyVisitor.VisitMultiLineIfBlock(MultiLineIfBlockSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.MultiLineIfBlockSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.Visit(SyntaxNode node)
   at ICSharpCode.CodeConverter.CSharp.CommentConvertingMethodBodyVisitor.ConvertWithTrivia(SyntaxNode node)
   at ICSharpCode.CodeConverter.CSharp.CommentConvertingMethodBodyVisitor.DefaultVisit(SyntaxNode node)

Input: 
                If ((Not IsNothing(wizard)) AndAlso (Not String.IsNullOrEmpty(wizard.MembershipProvider)) AndAlso (Not IsNothing(Membership.Providers(wizard.MembershipProvider)))) Then
                    provider = Membership.Providers(wizard.MembershipProvider)
                End If

 */
                return provider;
            }
        }

        public CreateUserWizardAdapter()
        {
            _state = State.CreateUser;
            _currentErrorText = "";
        }

        // / ///////////////////////////////////////////////////////////////////////////////
        // / PROTECTED        

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            CreateUserWizard wizard = System.Web.UI.WebControls.Adapters.WebControlAdapter.Control;
            if ((Extender.AdapterEnabled && (!Information.IsNothing(wizard))))
            {
                RegisterScripts();
                wizard.CreatedUser += OnCreatedUser;
                wizard.CreateUserError += OnCreateUserError;
                _state = State.CreateUser;
                _currentErrorText = "";
            }
        }

        protected void OnCreatedUser(object sender, EventArgs e)
        {
            _state = State.Success;
            _currentErrorText = "";
        }

        protected void OnCreateUserError(object sender, CreateUserErrorEventArgs e)
        {
            _state = State.Failed;
            _currentErrorText = "An error has occurred. Please try again.";

            CreateUserWizard wizard = System.Web.UI.WebControls.Adapters.WebControlAdapter.Control;
            if ((!Information.IsNothing(wizard)))
            {
                _currentErrorText = wizard.UnknownErrorMessage;
                switch (e.CreateUserError)
                {
                    case object _ when MembershipCreateStatus.DuplicateEmail:
                        {
                            _currentErrorText = wizard.DuplicateEmailErrorMessage;
                            break;
                            break;
                        }

                    case object _ when MembershipCreateStatus.DuplicateUserName:
                        {
                            _currentErrorText = wizard.DuplicateUserNameErrorMessage;
                            break;
                            break;
                        }

                    case object _ when MembershipCreateStatus.InvalidAnswer:
                        {
                            _currentErrorText = wizard.InvalidAnswerErrorMessage;
                            break;
                            break;
                        }

                    case object _ when MembershipCreateStatus.InvalidEmail:
                        {
                            _currentErrorText = wizard.InvalidEmailErrorMessage;
                            break;
                            break;
                        }

                    case object _ when MembershipCreateStatus.InvalidPassword:
                        {
                            _currentErrorText = wizard.InvalidPasswordErrorMessage;
                            break;
                            break;
                        }

                    case object _ when MembershipCreateStatus.InvalidQuestion:
                        {
                            _currentErrorText = wizard.InvalidQuestionErrorMessage;
                            break;
                            break;
                        }
                }
            }
        }

        protected override void RenderBeginTag(HtmlTextWriter writer)
        {
            if ((Extender.AdapterEnabled))
                Extender.RenderBeginTag(writer, "Kartris-CreateUserWizard");
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
                CreateUserWizard wizard = System.Web.UI.WebControls.Adapters.WebControlAdapter.Control;
                if ((!Information.IsNothing(wizard)))
                {
                    TemplatedWizardStep activeStep = wizard.ActiveStep;
                    if ((!Information.IsNothing(activeStep)))
                    {
                        if ((!Information.IsNothing(activeStep.ContentTemplate)))
                        {

                            // activeStep.RenderControl(writer)

                            // Medz edit - Added code to clear the controls inside the activestep before rendering
                            WebControl creatediv = new WebControl(HtmlTextWriterTag.Div);
                            activeStep.ContentTemplate.InstantiateIn(creatediv);
                            activeStep.ContentTemplateContainer.Controls.Clear();
                            activeStep.ContentTemplateContainer.Controls.Add(creatediv);
                            creatediv.RenderControl(writer);

                            if ((wizard.CreateUserStep.Equals(activeStep)))
                                WriteCreateUserButtonPanel(writer, wizard);
                        }
                        else if ((wizard.CreateUserStep.Equals(activeStep)))
                        {
                            WriteHeaderTextPanel(writer, wizard);
                            WriteStepTitlePanel(writer, wizard);
                            WriteInstructionPanel(writer, wizard);
                            WriteHelpPanel(writer, wizard);
                            WriteUserPanel(writer, wizard);
                            WritePasswordPanel(writer, wizard);
                            WritePasswordHintPanel(writer, wizard);
                            WriteConfirmPasswordPanel(writer, wizard);
                            WriteEmailPanel(writer, wizard);
                            WriteQuestionPanel(writer, wizard);
                            WriteAnswerPanel(writer, wizard);
                            WriteFinalValidators(writer, wizard);
                            if ((_state == State.Failed))
                                WriteFailurePanel(writer, wizard);
                            WriteCreateUserButtonPanel(writer, wizard);
                        }
                        else if ((wizard.CompleteStep.Equals(activeStep)))
                        {
                            WriteStepTitlePanel(writer, wizard);
                            WriteSuccessTextPanel(writer, wizard);
                            WriteContinuePanel(writer, wizard);
                            WriteEditProfilePanel(writer, wizard);
                        }
                        else
                            System.Diagnostics.Debug.Fail("The adapter isn't equipped to handle a CreateUserWizard with a step that is neither templated nor either the CreateUser step or the Complete step.");
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
        // Step 1: Create user step
        // ///////////////////////////////////////////////////////

        private void WriteHeaderTextPanel(HtmlTextWriter writer, CreateUserWizard wizard)
        {
            if ((!string.IsNullOrEmpty(wizard.HeaderText)))
            {
                string className = "";
                if (((!Information.IsNothing(wizard.HeaderStyle)) && (!string.IsNullOrEmpty(wizard.HeaderStyle.CssClass))))
                    className = wizard.HeaderStyle.CssClass + " ";
                className += "Kartris-CreateUserWizard-HeaderTextPanel";
                WebControlAdapterExtender.WriteBeginDiv(writer, className, "");
                WebControlAdapterExtender.WriteSpan(writer, "", wizard.HeaderText);
                WebControlAdapterExtender.WriteEndDiv(writer);
            }
        }

        private void WriteStepTitlePanel(HtmlTextWriter writer, CreateUserWizard wizard)
        {
            if ((!string.IsNullOrEmpty(wizard.ActiveStep.Title)))
            {
                string className = "";
                if (((!Information.IsNothing(wizard.TitleTextStyle)) && (!string.IsNullOrEmpty(wizard.TitleTextStyle.CssClass))))
                    className = wizard.TitleTextStyle.CssClass + " ";
                className += "Kartris-CreateUserWizard-StepTitlePanel";
                WebControlAdapterExtender.WriteBeginDiv(writer, className, "");
                WebControlAdapterExtender.WriteSpan(writer, "", wizard.ActiveStep.Title);
                WebControlAdapterExtender.WriteEndDiv(writer);
            }
        }

        private void WriteInstructionPanel(HtmlTextWriter writer, CreateUserWizard wizard)
        {
            if ((!string.IsNullOrEmpty(wizard.InstructionText)))
            {
                string className = "";
                if (((!Information.IsNothing(wizard.InstructionTextStyle)) && (!string.IsNullOrEmpty(wizard.InstructionTextStyle.CssClass))))
                    className = wizard.InstructionTextStyle.CssClass + " ";
                className += "Kartris-CreateUserWizard-InstructionPanel";
                WebControlAdapterExtender.WriteBeginDiv(writer, className, "");
                WebControlAdapterExtender.WriteSpan(writer, "", wizard.InstructionText);
                WebControlAdapterExtender.WriteEndDiv(writer);
            }
        }

        private void WriteFailurePanel(HtmlTextWriter writer, CreateUserWizard wizard)
        {
            string className = "";
            if (((!Information.IsNothing(wizard.ErrorMessageStyle)) && (!string.IsNullOrEmpty(wizard.ErrorMessageStyle.CssClass))))
                className = wizard.ErrorMessageStyle.CssClass + " ";
            className += "Kartris-CreateUserWizard-FailurePanel";
            WebControlAdapterExtender.WriteBeginDiv(writer, className, "");
            WebControlAdapterExtender.WriteSpan(writer, "", _currentErrorText);
            WebControlAdapterExtender.WriteEndDiv(writer);
        }

        private void WriteHelpPanel(HtmlTextWriter writer, CreateUserWizard wizard)
        {
            if (((!string.IsNullOrEmpty(wizard.HelpPageIconUrl)) || (!string.IsNullOrEmpty(wizard.HelpPageText))))
            {
                WebControlAdapterExtender.WriteBeginDiv(writer, "Kartris-CreateUserWizard-HelpPanel", "");
                WebControlAdapterExtender.WriteImage(writer, wizard.HelpPageIconUrl, "Help");
                WebControlAdapterExtender.WriteLink(writer, wizard.HyperLinkStyle.CssClass, wizard.HelpPageUrl, "Help", wizard.HelpPageText);
                WebControlAdapterExtender.WriteEndDiv(writer);
            }
        }

        private void WriteUserPanel(HtmlTextWriter writer, CreateUserWizard wizard)
        {
            TextBox textBox = wizard.FindControl("CreateUserStepContainer").FindControl("UserName");
            if ((!Information.IsNothing(textBox)))
            {
                System.Web.UI.Adapters.ControlAdapter.Page.ClientScript.RegisterForEventValidation(textBox.UniqueID);
                WebControlAdapterExtender.WriteBeginDiv(writer, "Kartris-CreateUserWizard-UserPanel", "");
                Extender.WriteTextBox(writer, false, wizard.LabelStyle.CssClass, wizard.UserNameLabelText, wizard.TextBoxStyle.CssClass, "CreateUserStepContainer_UserName", wizard.UserName);
                WebControlAdapterExtender.WriteRequiredFieldValidator(writer, (RequiredFieldValidator)wizard.FindControl("CreateUserStepContainer").FindControl("UserNameRequired"), wizard.ValidatorTextStyle.CssClass, "UserName", wizard.UserNameRequiredErrorMessage);
                WebControlAdapterExtender.WriteEndDiv(writer);
            }
        }

        private void WritePasswordPanel(HtmlTextWriter writer, CreateUserWizard wizard)
        {
            TextBox textBox = wizard.FindControl("CreateUserStepContainer").FindControl("Password");
            if ((!Information.IsNothing(textBox)))
            {
                System.Web.UI.Adapters.ControlAdapter.Page.ClientScript.RegisterForEventValidation(textBox.UniqueID);
                WebControlAdapterExtender.WriteBeginDiv(writer, "Kartris-CreateUserWizard-PasswordPanel", "");
                Extender.WriteTextBox(writer, true, wizard.LabelStyle.CssClass, wizard.PasswordLabelText, wizard.TextBoxStyle.CssClass, "CreateUserStepContainer_Password", "");
                WebControlAdapterExtender.WriteRequiredFieldValidator(writer, (RequiredFieldValidator)wizard.FindControl("CreateUserStepContainer").FindControl("PasswordRequired"), wizard.ValidatorTextStyle.CssClass, "Password", wizard.PasswordRequiredErrorMessage);
                WebControlAdapterExtender.WriteEndDiv(writer);
            }
        }

        private void WritePasswordHintPanel(HtmlTextWriter writer, CreateUserWizard wizard)
        {
            if ((!string.IsNullOrEmpty(wizard.PasswordHintText)))
            {
                WebControlAdapterExtender.WriteBeginDiv(writer, "Kartris-CreateUserWizard-PasswordHintPanel", "");
                WebControlAdapterExtender.WriteSpan(writer, "", wizard.PasswordHintText);
                WebControlAdapterExtender.WriteEndDiv(writer);
            }
        }

        private void WriteConfirmPasswordPanel(HtmlTextWriter writer, CreateUserWizard wizard)
        {
            TextBox textBox = wizard.FindControl("CreateUserStepContainer").FindControl("ConfirmPassword");
            if ((!Information.IsNothing(textBox)))
            {
                System.Web.UI.Adapters.ControlAdapter.Page.ClientScript.RegisterForEventValidation(textBox.UniqueID);
                WebControlAdapterExtender.WriteBeginDiv(writer, "Kartris-CreateUserWizard-ConfirmPasswordPanel", "");
                Extender.WriteTextBox(writer, true, wizard.LabelStyle.CssClass, wizard.ConfirmPasswordLabelText, wizard.TextBoxStyle.CssClass, "CreateUserStepContainer_ConfirmPassword", "");
                WebControlAdapterExtender.WriteRequiredFieldValidator(writer, (RequiredFieldValidator)wizard.FindControl("CreateUserStepContainer").FindControl("ConfirmPasswordRequired"), wizard.ValidatorTextStyle.CssClass, "ConfirmPassword", wizard.ConfirmPasswordRequiredErrorMessage);
                WebControlAdapterExtender.WriteEndDiv(writer);
            }
        }

        private void WriteEmailPanel(HtmlTextWriter writer, CreateUserWizard wizard)
        {
            TextBox textBox = wizard.FindControl("CreateUserStepContainer").FindControl("Email");
            if ((!Information.IsNothing(textBox)))
            {
                System.Web.UI.Adapters.ControlAdapter.Page.ClientScript.RegisterForEventValidation(textBox.UniqueID);
                WebControlAdapterExtender.WriteBeginDiv(writer, "Kartris-CreateUserWizard-EmailPanel", "");
                Extender.WriteTextBox(writer, false, wizard.LabelStyle.CssClass, wizard.EmailLabelText, wizard.TextBoxStyle.CssClass, "CreateUserStepContainer_Email", "");
                WebControlAdapterExtender.WriteRequiredFieldValidator(writer, (RequiredFieldValidator)wizard.FindControl("CreateUserStepContainer").FindControl("EmailRequired"), wizard.ValidatorTextStyle.CssClass, "Email", wizard.EmailRequiredErrorMessage);
                WebControlAdapterExtender.WriteEndDiv(writer);
            }
        }

        private void WriteQuestionPanel(HtmlTextWriter writer, CreateUserWizard wizard)
        {
            if (((!IsNothing(WizardMembershipProvider)) && WizardMembershipProvider.RequiresQuestionAndAnswer))
            {
                TextBox textBox = wizard.FindControl("CreateUserStepContainer").FindControl("Question");
                if ((!Information.IsNothing(textBox)))
                {
                    System.Web.UI.Adapters.ControlAdapter.Page.ClientScript.RegisterForEventValidation(textBox.UniqueID);
                    WebControlAdapterExtender.WriteBeginDiv(writer, "Kartris-CreateUserWizard-QuestionPanel", "");
                    Extender.WriteTextBox(writer, false, wizard.LabelStyle.CssClass, wizard.QuestionLabelText, wizard.TextBoxStyle.CssClass, "CreateUserStepContainer_Question", "");
                    WebControlAdapterExtender.WriteRequiredFieldValidator(writer, (RequiredFieldValidator)wizard.FindControl("CreateUserStepContainer").FindControl("QuestionRequired"), wizard.ValidatorTextStyle.CssClass, "Question", wizard.QuestionRequiredErrorMessage);
                    WebControlAdapterExtender.WriteEndDiv(writer);
                }
            }
        }

        private void WriteAnswerPanel(HtmlTextWriter writer, CreateUserWizard wizard)
        {
            if (((!IsNothing(WizardMembershipProvider)) && WizardMembershipProvider.RequiresQuestionAndAnswer))
            {
                TextBox textBox = wizard.FindControl("CreateUserStepContainer").FindControl("Answer");
                if ((!Information.IsNothing(textBox)))
                {
                    System.Web.UI.Adapters.ControlAdapter.Page.ClientScript.RegisterForEventValidation(textBox.UniqueID);
                    WebControlAdapterExtender.WriteBeginDiv(writer, "Kartris-CreateUserWizard-AnswerPanel", "");
                    Extender.WriteTextBox(writer, false, wizard.LabelStyle.CssClass, wizard.AnswerLabelText, wizard.TextBoxStyle.CssClass, "CreateUserStepContainer_Answer", "");
                    WebControlAdapterExtender.WriteRequiredFieldValidator(writer, (RequiredFieldValidator)wizard.FindControl("CreateUserStepContainer").FindControl("AnswerRequired"), wizard.ValidatorTextStyle.CssClass, "Answer", wizard.AnswerRequiredErrorMessage);
                    WebControlAdapterExtender.WriteEndDiv(writer);
                }
            }
        }

        private void WriteFinalValidators(HtmlTextWriter writer, CreateUserWizard wizard)
        {
            WebControlAdapterExtender.WriteBeginDiv(writer, "Kartris-CreateUserWizard-FinalValidatorsPanel", "");
            WebControlAdapterExtender.WriteCompareValidator(writer, (CompareValidator)wizard.FindControl("CreateUserStepContainer").FindControl("PasswordCompare"), wizard.ValidatorTextStyle.CssClass, "ConfirmPassword", wizard.ConfirmPasswordCompareErrorMessage, "Password");
            WebControlAdapterExtender.WriteRegularExpressionValidator(writer, (RegularExpressionValidator)wizard.FindControl("CreateUserStepContainer").FindControl("PasswordRegExpValidator"), wizard.ValidatorTextStyle.CssClass, "Password", wizard.PasswordRegularExpressionErrorMessage, wizard.PasswordRegularExpression);
            WebControlAdapterExtender.WriteRegularExpressionValidator(writer, (RegularExpressionValidator)wizard.FindControl("CreateUserStepContainer").FindControl("EmailRegExpValidator"), wizard.ValidatorTextStyle.CssClass, "Email", wizard.EmailRegularExpressionErrorMessage, wizard.EmailRegularExpression);
            WebControlAdapterExtender.WriteEndDiv(writer);
        }

        private void WriteCreateUserButtonPanel(HtmlTextWriter writer, CreateUserWizard wizard)
        {
            Control btnParentCtrl = wizard.FindControl("__CustomNav0");
            if (!btnParentCtrl == null)
            {
                string id = "_CustomNav0_StepNextButton";
                string idWithType = WebControlAdapterExtender.MakeIdWithButtonType("StepNextButton", wizard.CreateUserButtonType);
                Control btn = btnParentCtrl.FindControl(idWithType);
                if ((!Information.IsNothing(btn)))
                {
                    System.Web.UI.Adapters.ControlAdapter.Page.ClientScript.RegisterForEventValidation(btn.UniqueID);

                    PostBackOptions options = new PostBackOptions(btn, "", "", false, false, false, true, true, wizard.ID);
                    string javascript = "javascript:" + System.Web.UI.Adapters.ControlAdapter.Page.ClientScript.GetPostBackEventReference(options);
                    javascript = System.Web.UI.Adapters.ControlAdapter.Page.Server.HtmlEncode(javascript);

                    WebControlAdapterExtender.WriteBeginDiv(writer, "Kartris-CreateUserWizard-CreateUserButtonPanel", "");

                    Extender.WriteSubmit(writer, wizard.CreateUserButtonType, wizard.CreateUserButtonStyle.CssClass, id, wizard.CreateUserButtonImageUrl, javascript, wizard.CreateUserButtonText);

                    if ((wizard.DisplayCancelButton))
                        Extender.WriteSubmit(writer, wizard.CancelButtonType, wizard.CancelButtonStyle.CssClass, "_CustomNav0_CancelButton", wizard.CancelButtonImageUrl, "", wizard.CancelButtonText);

                    WebControlAdapterExtender.WriteEndDiv(writer);
                }
            }
        }

        // ///////////////////////////////////////////////////////
        // Complete step
        // ///////////////////////////////////////////////////////

        private void WriteSuccessTextPanel(HtmlTextWriter writer, CreateUserWizard wizard)
        {
            if ((!string.IsNullOrEmpty(wizard.CompleteSuccessText)))
            {
                string className = "";
                if (((!Information.IsNothing(wizard.CompleteSuccessTextStyle)) && (!string.IsNullOrEmpty(wizard.CompleteSuccessTextStyle.CssClass))))
                    className = wizard.CompleteSuccessTextStyle.CssClass + " ";
                className += "Kartris-CreateUserWizard-SuccessTextPanel";
                WebControlAdapterExtender.WriteBeginDiv(writer, className, "");
                WebControlAdapterExtender.WriteSpan(writer, "", wizard.CompleteSuccessText);
                WebControlAdapterExtender.WriteEndDiv(writer);
            }
        }

        private void WriteContinuePanel(HtmlTextWriter writer, CreateUserWizard wizard)
        {
            string id = "ContinueButton";
            string idWithType = WebControlAdapterExtender.MakeIdWithButtonType(id, wizard.ContinueButtonType);
            Control btn = wizard.FindControl("CompleteStepContainer").FindControl(idWithType);
            if ((!Information.IsNothing(btn)))
            {
                System.Web.UI.Adapters.ControlAdapter.Page.ClientScript.RegisterForEventValidation(btn.UniqueID);
                WebControlAdapterExtender.WriteBeginDiv(writer, "Kartris-CreateUserWizard-ContinuePanel", "");
                Extender.WriteSubmit(writer, wizard.ContinueButtonType, wizard.ContinueButtonStyle.CssClass, "CompleteStepContainer_ContinueButton", wizard.ContinueButtonImageUrl, "", wizard.ContinueButtonText);
                WebControlAdapterExtender.WriteEndDiv(writer);
            }
        }

        private void WriteEditProfilePanel(HtmlTextWriter writer, CreateUserWizard wizard)
        {
            if (((!string.IsNullOrEmpty(wizard.EditProfileUrl)) || (!string.IsNullOrEmpty(wizard.EditProfileText))))
            {
                WebControlAdapterExtender.WriteBeginDiv(writer, "Kartris-CreateUserWizard-EditProfilePanel", "");
                WebControlAdapterExtender.WriteImage(writer, wizard.EditProfileIconUrl, "Edit profile");
                WebControlAdapterExtender.WriteLink(writer, wizard.HyperLinkStyle.CssClass, wizard.EditProfileUrl, "EditProfile", wizard.EditProfileText);
                WebControlAdapterExtender.WriteEndDiv(writer);
            }
        }
    }
}
