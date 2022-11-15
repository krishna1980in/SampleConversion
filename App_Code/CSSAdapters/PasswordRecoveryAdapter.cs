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
    public class PasswordRecoveryAdapter : System.Web.UI.WebControls.Adapters.WebControlAdapter
    {
        private enum State
        {
            UserName,
            VerifyingUser,
            UserLookupError,
            Question,
            VerifyingAnswer,
            AnswerLookupError,
            SendMailError,
            Success
        }
        private State _state = State.UserName;
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

        private MembershipProvider PasswordRecoveryMembershipProvider
        {
            get
            {
                MembershipProvider provider = Membership.Provider;
                PasswordRecovery passwordRecovery = System.Web.UI.WebControls.Adapters.WebControlAdapter.Control;
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
                If ((Not IsNothing(passwordRecovery)) AndAlso (Not String.IsNullOrEmpty(passwordRecovery.MembershipProvider)) AndAlso (Not IsNothing(Membership.Providers(passwordRecovery.MembershipProvider)))) Then
                    provider = Membership.Providers(passwordRecovery.MembershipProvider)
                End If

 */
                return provider;
            }
        }

        public PasswordRecoveryAdapter()
        {
            _state = State.UserName;
            _currentErrorText = "";
        }

        // / ///////////////////////////////////////////////////////////////////////////////
        // / PROTECTED        

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PasswordRecovery passwordRecovery = System.Web.UI.WebControls.Adapters.WebControlAdapter.Control;
            if ((Extender.AdapterEnabled && (!Information.IsNothing(passwordRecovery))))
            {
                RegisterScripts();
                passwordRecovery.AnswerLookupError += OnAnswerLookupError;
                passwordRecovery.SendMailError += OnSendMailError;
                passwordRecovery.UserLookupError += OnUserLookupError;
                passwordRecovery.VerifyingAnswer += OnVerifyingAnswer;
                passwordRecovery.VerifyingUser += OnVerifyingUser;
                _state = State.UserName;
                _currentErrorText = "";
            }
        }

        protected override void CreateChildControls()
        {
            base.CreateChildControls();

            PasswordRecovery passwordRecovery = System.Web.UI.WebControls.Adapters.WebControlAdapter.Control;
            if ((!Information.IsNothing(passwordRecovery)))
            {
                if (((!Information.IsNothing(passwordRecovery.UserNameTemplate)) && (!Information.IsNothing(passwordRecovery.UserNameTemplateContainer))))
                {
                    passwordRecovery.UserNameTemplateContainer.Controls.Clear();
                    passwordRecovery.UserNameTemplate.InstantiateIn(passwordRecovery.UserNameTemplateContainer);
                    passwordRecovery.UserNameTemplateContainer.DataBind();
                }

                if (((!Information.IsNothing(passwordRecovery.QuestionTemplate)) && (!Information.IsNothing(passwordRecovery.QuestionTemplateContainer))))
                {
                    passwordRecovery.QuestionTemplateContainer.Controls.Clear();
                    passwordRecovery.QuestionTemplate.InstantiateIn(passwordRecovery.QuestionTemplateContainer);
                    passwordRecovery.QuestionTemplateContainer.DataBind();
                }

                if (((!Information.IsNothing(passwordRecovery.SuccessTemplate)) && (!Information.IsNothing(passwordRecovery.SuccessTemplateContainer))))
                {
                    passwordRecovery.SuccessTemplateContainer.Controls.Clear();
                    passwordRecovery.SuccessTemplate.InstantiateIn(passwordRecovery.SuccessTemplateContainer);
                    passwordRecovery.SuccessTemplateContainer.DataBind();
                }
            }
        }

        protected void OnAnswerLookupError(object sender, EventArgs e)
        {
            _state = State.AnswerLookupError;
            PasswordRecovery passwordRecovery = System.Web.UI.WebControls.Adapters.WebControlAdapter.Control;
            if ((!Information.IsNothing(passwordRecovery)))
            {
                _currentErrorText = passwordRecovery.GeneralFailureText;
                if ((!string.IsNullOrEmpty(passwordRecovery.QuestionFailureText)))
                    _currentErrorText = passwordRecovery.QuestionFailureText;
            }
        }

        protected void OnSendMailError(object sender, SendMailErrorEventArgs e)
        {
            if ((!e.Handled))
            {
                _state = State.SendMailError;
                _currentErrorText = e.Exception.Message;
            }
        }

        protected void OnUserLookupError(object sender, EventArgs e)
        {
            _state = State.UserLookupError;
            PasswordRecovery passwordRecovery = System.Web.UI.WebControls.Adapters.WebControlAdapter.Control;
            if ((!Information.IsNothing(passwordRecovery)))
            {
                _currentErrorText = passwordRecovery.GeneralFailureText;
                if ((!string.IsNullOrEmpty(passwordRecovery.UserNameFailureText)))
                    _currentErrorText = passwordRecovery.UserNameFailureText;
            }
        }

        protected void OnVerifyingAnswer(object sender, LoginCancelEventArgs e)
        {
            _state = State.VerifyingAnswer;
        }

        protected void OnVerifyingUser(object sender, LoginCancelEventArgs e)
        {
            _state = State.VerifyingUser;
        }

        protected override void RenderBeginTag(HtmlTextWriter writer)
        {
            if ((Extender.AdapterEnabled))
                Extender.RenderBeginTag(writer, "Kartris-PasswordRecovery");
            else
                base.RenderBeginTag(writer);
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            PasswordRecovery passwordRecovery = System.Web.UI.WebControls.Adapters.WebControlAdapter.Control;
            if ((!Information.IsNothing(passwordRecovery)))
                string provider = passwordRecovery.MembershipProvider;

            // By this time we have finished doing our event processing.  That means that if errors have
            // occurred, the event handlers (OnAnswerLookupError, OnSendMailError or 
            // OnUserLookupError) will have been called.  So, if we were, for example, verifying the
            // user and didn't cause an error then we know we can move on to the next step, getting
            // the answer to the security question... if the membership system demands it.

            switch (_state)
            {
                case State.AnswerLookupError:
                    {
                        // Leave the state alone because we hit an error.
                        break;
                        break;
                    }

                case State.Question:
                    {
                        // Leave the state alone. Render a form to get the answer to the security question.
                        _currentErrorText = "";
                        break;
                        break;
                    }

                case State.SendMailError:
                    {
                        // Leave the state alone because we hit an error.
                        break;
                        break;
                    }

                case State.Success:
                    {
                        // Leave the state alone. Render a concluding message.
                        _currentErrorText = "";
                        break;
                        break;
                    }

                case State.UserLookupError:
                    {
                        // Leave the state alone because we hit an error.
                        break;
                        break;
                    }

                case State.UserName:
                    {
                        // Leave the state alone. Render a form to get the user name.
                        _currentErrorText = "";
                        break;
                        break;
                    }

                case State.VerifyingAnswer:
                    {
                        // Success! We did not encounter an error while verifying the answer to the security question.
                        _state = State.Success;
                        _currentErrorText = "";
                        break;
                        break;
                    }

                case State.VerifyingUser:
                    {
                        // We have a valid user. We did not encounter an error while verifying the user.
                        if ((PasswordRecoveryMembershipProvider.RequiresQuestionAndAnswer))
                            _state = State.Question;
                        else
                            _state = State.Success;
                        _currentErrorText = "";
                        break;
                        break;
                    }
            }
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
                PasswordRecovery passwordRecovery = System.Web.UI.WebControls.Adapters.WebControlAdapter.Control;
                if ((!Information.IsNothing(passwordRecovery)))
                {
                    if (((_state == State.UserName) || (_state == State.UserLookupError)))
                    {
                        if (((!Information.IsNothing(passwordRecovery.UserNameTemplate)) && (!Information.IsNothing(passwordRecovery.UserNameTemplateContainer))))
                        {
                            Control c;
                            foreach (var c in passwordRecovery.UserNameTemplateContainer.Controls)
                                c.RenderControl(writer);
                        }
                        else
                        {
                            WriteTitlePanel(writer, passwordRecovery);
                            WriteInstructionPanel(writer, passwordRecovery);
                            WriteHelpPanel(writer, passwordRecovery);
                            if ((_state == State.UserLookupError))
                                WriteFailurePanel(writer, passwordRecovery);
                            WriteUserPanel(writer, passwordRecovery);
                            WriteSubmitPanel(writer, passwordRecovery);
                        }
                    }
                    else if (((_state == State.Question) || (_state == State.AnswerLookupError)))
                    {
                        if (((!Information.IsNothing(passwordRecovery.QuestionTemplate)) && (!Information.IsNothing(passwordRecovery.QuestionTemplateContainer))))
                        {
                            Control c;
                            foreach (var c in passwordRecovery.QuestionTemplateContainer.Controls)
                                c.RenderControl(writer);
                        }
                        else
                        {
                            WriteTitlePanel(writer, passwordRecovery);
                            WriteInstructionPanel(writer, passwordRecovery);
                            WriteHelpPanel(writer, passwordRecovery);
                            if ((_state == State.AnswerLookupError))
                                WriteFailurePanel(writer, passwordRecovery);
                            WriteUserPanel(writer, passwordRecovery);
                            WriteQuestionPanel(writer, passwordRecovery);
                            WriteAnswerPanel(writer, passwordRecovery);
                            WriteSubmitPanel(writer, passwordRecovery);
                        }
                    }
                    else if ((_state == State.SendMailError))
                        WriteFailurePanel(writer, passwordRecovery);
                    else if ((_state == State.Success))
                    {
                        if (((!Information.IsNothing(passwordRecovery.SuccessTemplate)) && (!Information.IsNothing(passwordRecovery.SuccessTemplateContainer))))
                        {
                            Control c;
                            foreach (var c in passwordRecovery.SuccessTemplateContainer.Controls)
                                c.RenderControl(writer);
                        }
                        else
                            WriteSuccessTextPanel(writer, passwordRecovery);
                    }
                    else
                        // We should never get here.
                        System.Diagnostics.Debug.Fail("The PasswordRecovery control adapter was asked to render a state that it does not expect.");
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
        // Step 1: user name
        // ///////////////////////////////////////////////////////

        private void WriteTitlePanel(HtmlTextWriter writer, PasswordRecovery passwordRecovery)
        {
            if (((_state == State.UserName) || (_state == State.UserLookupError)))
            {
                if ((!string.IsNullOrEmpty(passwordRecovery.UserNameTitleText)))
                {
                    string className = "";
                    if (((!Information.IsNothing(passwordRecovery.TitleTextStyle)) && (!string.IsNullOrEmpty(passwordRecovery.TitleTextStyle.CssClass))))
                        className = passwordRecovery.TitleTextStyle.CssClass + " ";
                    className += "Kartris-PasswordRecovery-UserName-TitlePanel";
                    WebControlAdapterExtender.WriteBeginDiv(writer, className, "");
                    WebControlAdapterExtender.WriteSpan(writer, "", passwordRecovery.UserNameTitleText);
                    WebControlAdapterExtender.WriteEndDiv(writer);
                }
            }
            else if (((_state == State.Question) || (_state == State.AnswerLookupError)))
            {
                if ((!string.IsNullOrEmpty(passwordRecovery.QuestionTitleText)))
                {
                    string className = "";
                    if (((!Information.IsNothing(passwordRecovery.TitleTextStyle)) && (!string.IsNullOrEmpty(passwordRecovery.TitleTextStyle.CssClass))))
                        className = passwordRecovery.TitleTextStyle.CssClass + " ";
                    className += "Kartris-PasswordRecovery-Question-TitlePanel";
                    WebControlAdapterExtender.WriteBeginDiv(writer, className, "");
                    WebControlAdapterExtender.WriteSpan(writer, "", passwordRecovery.QuestionTitleText);
                    WebControlAdapterExtender.WriteEndDiv(writer);
                }
            }
        }

        private void WriteInstructionPanel(HtmlTextWriter writer, PasswordRecovery passwordRecovery)
        {
            if (((_state == State.UserName) || (_state == State.UserLookupError)))
            {
                if ((!string.IsNullOrEmpty(passwordRecovery.UserNameInstructionText)))
                {
                    string className = "";
                    if (((!Information.IsNothing(passwordRecovery.InstructionTextStyle)) && (!string.IsNullOrEmpty(passwordRecovery.InstructionTextStyle.CssClass))))
                        className = passwordRecovery.InstructionTextStyle.CssClass + " ";
                    className += "Kartris-PasswordRecovery-UserName-InstructionPanel";
                    WebControlAdapterExtender.WriteBeginDiv(writer, className, "");
                    WebControlAdapterExtender.WriteSpan(writer, "", passwordRecovery.UserNameInstructionText);
                    WebControlAdapterExtender.WriteEndDiv(writer);
                }
            }
            else if (((_state == State.Question) || (_state == State.AnswerLookupError)))
            {
                if ((!string.IsNullOrEmpty(passwordRecovery.QuestionInstructionText)))
                {
                    string className = "";
                    if (((!Information.IsNothing(passwordRecovery.InstructionTextStyle)) && (!string.IsNullOrEmpty(passwordRecovery.InstructionTextStyle.CssClass))))
                        className = passwordRecovery.InstructionTextStyle.CssClass + " ";
                    className += "Kartris-PasswordRecovery-Question-InstructionPanel";
                    WebControlAdapterExtender.WriteBeginDiv(writer, className, "");
                    WebControlAdapterExtender.WriteSpan(writer, "", passwordRecovery.QuestionInstructionText);
                    WebControlAdapterExtender.WriteEndDiv(writer);
                }
            }
        }

        private void WriteFailurePanel(HtmlTextWriter writer, PasswordRecovery passwordRecovery)
        {
            if ((!string.IsNullOrEmpty(_currentErrorText)))
            {
                string className = "";
                if (((!Information.IsNothing(passwordRecovery.FailureTextStyle)) && (!string.IsNullOrEmpty(passwordRecovery.FailureTextStyle.CssClass))))
                    className = passwordRecovery.FailureTextStyle.CssClass + " ";
                className += "Kartris-PasswordRecovery-FailurePanel";
                WebControlAdapterExtender.WriteBeginDiv(writer, className, "");
                WebControlAdapterExtender.WriteSpan(writer, "", _currentErrorText);
                WebControlAdapterExtender.WriteEndDiv(writer);
            }
        }

        private void WriteHelpPanel(HtmlTextWriter writer, PasswordRecovery passwordRecovery)
        {
            if (((!string.IsNullOrEmpty(passwordRecovery.HelpPageIconUrl)) || (!string.IsNullOrEmpty(passwordRecovery.HelpPageText))))
            {
                WebControlAdapterExtender.WriteBeginDiv(writer, "Kartris-PasswordRecovery-HelpPanel", "");
                WebControlAdapterExtender.WriteImage(writer, passwordRecovery.HelpPageIconUrl, "Help");
                WebControlAdapterExtender.WriteLink(writer, passwordRecovery.HyperLinkStyle.CssClass, passwordRecovery.HelpPageUrl, "Help", passwordRecovery.HelpPageText);
                WebControlAdapterExtender.WriteEndDiv(writer);
            }
        }

        private void WriteUserPanel(HtmlTextWriter writer, PasswordRecovery passwordRecovery)
        {
            if (((_state == State.UserName) || (_state == State.UserLookupError)))
            {
                Control container = passwordRecovery.UserNameTemplateContainer;
                TextBox textBox = null;
                if ((!Information.IsNothing(container)))
                    textBox = (TextBox)container.FindControl("UserName");
                RequiredFieldValidator rfv = null;
                if ((!Information.IsNothing(textBox)))
                    rfv = (RequiredFieldValidator)container.FindControl("UserNameRequired");
                string id = "";
                if ((!Information.IsNothing(rfv)))
                    id = container.ID + "_" + textBox.ID;
                if ((!string.IsNullOrEmpty(id)))
                {
                    System.Web.UI.Adapters.ControlAdapter.Page.ClientScript.RegisterForEventValidation(textBox.UniqueID);
                    WebControlAdapterExtender.WriteBeginDiv(writer, "Kartris-PasswordRecovery-UserName-UserPanel", "");
                    Extender.WriteTextBox(writer, false, passwordRecovery.LabelStyle.CssClass, passwordRecovery.UserNameLabelText, passwordRecovery.TextBoxStyle.CssClass, id, passwordRecovery.UserName);
                    WebControlAdapterExtender.WriteRequiredFieldValidator(writer, rfv, passwordRecovery.ValidatorTextStyle.CssClass, "UserName", passwordRecovery.UserNameRequiredErrorMessage);
                    WebControlAdapterExtender.WriteEndDiv(writer);
                }
            }
            else if (((_state == State.Question) || (_state == State.AnswerLookupError)))
            {
                WebControlAdapterExtender.WriteBeginDiv(writer, "Kartris-PasswordRecovery-Question-UserPanel", "");
                Extender.WriteReadOnlyTextBox(writer, passwordRecovery.LabelStyle.CssClass, passwordRecovery.UserNameLabelText, passwordRecovery.TextBoxStyle.CssClass, passwordRecovery.UserName);
                WebControlAdapterExtender.WriteEndDiv(writer);
            }
        }

        private void WriteSubmitPanel(HtmlTextWriter writer, PasswordRecovery passwordRecovery)
        {
            if (((_state == State.UserName) || (_state == State.UserLookupError)))
            {
                Control container = passwordRecovery.UserNameTemplateContainer;
                string id = Interaction.IIf(!Information.IsNothing(container), container.ID + "_Submit", "Submit");

                string idWithType = WebControlAdapterExtender.MakeIdWithButtonType("Submit", passwordRecovery.SubmitButtonType);
                Control btn = null;
                if ((!Information.IsNothing(container)))
                    btn = container.FindControl(idWithType);

                if ((!Information.IsNothing(btn)))
                {
                    System.Web.UI.Adapters.ControlAdapter.Page.ClientScript.RegisterForEventValidation(btn.UniqueID);

                    PostBackOptions options = new PostBackOptions(btn, "", "", false, false, false, true, true, passwordRecovery.UniqueID);
                    string javascript = "javascript:" + System.Web.UI.Adapters.ControlAdapter.Page.ClientScript.GetPostBackEventReference(options);
                    javascript = System.Web.UI.Adapters.ControlAdapter.Page.Server.HtmlEncode(javascript);

                    WebControlAdapterExtender.WriteBeginDiv(writer, "Kartris-PasswordRecovery-UserName-SubmitPanel", "");
                    Extender.WriteSubmit(writer, passwordRecovery.SubmitButtonType, passwordRecovery.SubmitButtonStyle.CssClass, id, passwordRecovery.SubmitButtonImageUrl, javascript, passwordRecovery.SubmitButtonText);
                    WebControlAdapterExtender.WriteEndDiv(writer);
                }
            }
            else if (((_state == State.Question) || (_state == State.AnswerLookupError)))
            {
                Control container = passwordRecovery.QuestionTemplateContainer;
                string id = Interaction.IIf(!Information.IsNothing(container), container.ID + "_Submit", "Submit");
                string idWithType = WebControlAdapterExtender.MakeIdWithButtonType("Submit", passwordRecovery.SubmitButtonType);
                Control btn = null;
                if ((!Information.IsNothing(container)))
                    btn = container.FindControl(idWithType);

                if (!btn == null)
                {
                    System.Web.UI.Adapters.ControlAdapter.Page.ClientScript.RegisterForEventValidation(btn.UniqueID);

                    PostBackOptions options = new PostBackOptions(btn, "", "", false, false, false, true, true, passwordRecovery.UniqueID);
                    string javascript = "javascript:" + System.Web.UI.Adapters.ControlAdapter.Page.ClientScript.GetPostBackEventReference(options);
                    javascript = System.Web.UI.Adapters.ControlAdapter.Page.Server.HtmlEncode(javascript);

                    WebControlAdapterExtender.WriteBeginDiv(writer, "Kartris-PasswordRecovery-Question-SubmitPanel", "");
                    Extender.WriteSubmit(writer, passwordRecovery.SubmitButtonType, passwordRecovery.SubmitButtonStyle.CssClass, id, passwordRecovery.SubmitButtonImageUrl, javascript, passwordRecovery.SubmitButtonText);
                    WebControlAdapterExtender.WriteEndDiv(writer);
                }
            }
        }

        // ///////////////////////////////////////////////////////
        // Step 2: question
        // ///////////////////////////////////////////////////////

        private void WriteQuestionPanel(HtmlTextWriter writer, PasswordRecovery passwordRecovery)
        {
            WebControlAdapterExtender.WriteBeginDiv(writer, "Kartris-PasswordRecovery-QuestionPanel", "");
            Extender.WriteReadOnlyTextBox(writer, passwordRecovery.LabelStyle.CssClass, passwordRecovery.QuestionLabelText, passwordRecovery.TextBoxStyle.CssClass, passwordRecovery.Question);
            WebControlAdapterExtender.WriteEndDiv(writer);
        }

        private void WriteAnswerPanel(HtmlTextWriter writer, PasswordRecovery passwordRecovery)
        {
            Control container = passwordRecovery.QuestionTemplateContainer;
            TextBox textBox = null;
            if ((!Information.IsNothing(container)))
                textBox = (TextBox)container.FindControl("Answer");
            RequiredFieldValidator rfv = null;
            if ((!Information.IsNothing(textBox)))
                rfv = (RequiredFieldValidator)container.FindControl("AnswerRequired");
            string id = "";
            if ((!Information.IsNothing(rfv)))
                id = container.ID + "_" + TextBox.ID;
            if ((!string.IsNullOrEmpty(id)))
            {
                System.Web.UI.Adapters.ControlAdapter.Page.ClientScript.RegisterForEventValidation(textBox.UniqueID);
                WebControlAdapterExtender.WriteBeginDiv(writer, "Kartris-PasswordRecovery-AnswerPanel", "");
                Extender.WriteTextBox(writer, false, passwordRecovery.LabelStyle.CssClass, passwordRecovery.AnswerLabelText, passwordRecovery.TextBoxStyle.CssClass, id, "");
                WebControlAdapterExtender.WriteRequiredFieldValidator(writer, rfv, passwordRecovery.ValidatorTextStyle.CssClass, "Answer", passwordRecovery.AnswerRequiredErrorMessage);
                WebControlAdapterExtender.WriteEndDiv(writer);
            }
        }

        // ///////////////////////////////////////////////////////
        // Step 3: success
        // ///////////////////////////////////////////////////////

        private void WriteSuccessTextPanel(HtmlTextWriter writer, PasswordRecovery passwordRecovery)
        {
            if ((!string.IsNullOrEmpty(passwordRecovery.SuccessText)))
            {
                string className = "";
                if (((!Information.IsNothing(passwordRecovery.SuccessTextStyle)) && (!string.IsNullOrEmpty(passwordRecovery.SuccessTextStyle.CssClass))))
                    className = passwordRecovery.SuccessTextStyle.CssClass + " ";
                className += "Kartris-PasswordRecovery-SuccessTextPanel";
                WebControlAdapterExtender.WriteBeginDiv(writer, className, "");
                WebControlAdapterExtender.WriteSpan(writer, "", passwordRecovery.SuccessText);
                WebControlAdapterExtender.WriteEndDiv(writer);
            }
        }
    }
}
