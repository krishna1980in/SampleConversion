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

using System.Web.UI;
using System.Web.UI.WebControls;

// All due credit and kudos to Andrew Tokeley http://andrewtokeley.net/
// for this excellent piece of code

namespace Kartris
{
    /// <summary>
    ///     ''' The wizard adpater will render the standard wizard control using div's rather than tables.
    ///     ''' </summary>
    public class WizardAdapter : System.Web.UI.WebControls.Adapters.WebControlAdapter
    {
        private WebControlAdapterExtender _extender = null/* TODO Change to default(_) if this is not a reference type */;
        // -- CSS class names for the div's
        private const string CSS_WIZARD = "wizard";
        private const string CSS_STEP = "step";
        private const string CSS_NAV = "nav";
        private const string CSS_HEADER = "header";
        private const string CSS_SIDEBAR = "sidebar";
        private const string CSS_ACTIVE = "active";
        // -- The ID's of the underlying control's containers.  
        private const string CONTAINERID_FINISHNAVIGATION_TEMPLATE = "FinishNavigationTemplateContainerID";
        private const string CONTAINERID_STARTNAVIGATION_TEMPLATE = "StartNavigationTemplateContainerID";
        private const string CONTAINERID_STEPNAVIGATION_TEMPLATE = "StepNavigationTemplateContainerID";
        private const string CONTAINERID_HEADER = "HeaderContainer";
        private const string CONTAINERID_SIDEBAR = "SideBarContainer";
        // -- The ID's of the underlying controls within the containers - the ID's are important as the underlying
        // -- control (I assume) uses these to wire up the event handlers to respond to button clicks etc.
        private const string CONTROLID_STARTNEXT = "StartNext";
        private const string CONTROLID_STEPPREVIOUS = "StepPrevious";
        private const string CONTROLID_STEPNEXT = "StepNext";
        private const string CONTROLID_FINISHPREVIOUS = "FinishPrevious";
        private const string CONTROLID_FINISH = "Finish";
        private const string CONTROLID_CANCEL = "Cancel";
        private const string CONTROLID_SIDEBARLIST = "SideBarList";
        private const string CONTROLID_SIDEBARBUTTON = "SideBarButton";

        public WizardAdapter()
        {
        }

        private WebControlAdapterExtender Extender
        {
            get
            {
                if (((_extender == null) && (!System.Web.UI.WebControls.Adapters.WebControlAdapter.Control == null)) || ((!_extender == null) && (!System.Web.UI.WebControls.Adapters.WebControlAdapter.Control == _extender.AdaptedControl)))
                    _extender = new WebControlAdapterExtender(System.Web.UI.WebControls.Adapters.WebControlAdapter.Control);
                System.Diagnostics.Debug.Assert(!_extender == null, "CSS Friendly adapters internal error", "Null extender instance");
                return _extender;
            }
        }

        protected new override void RenderBeginTag(HtmlTextWriter writer)
        {
            if (Extender.AdapterEnabled)
                Extender.RenderBeginTag(writer, CSS_WIZARD);
            else
                base.RenderBeginTag(writer);
        }
        protected new override void RenderEndTag(HtmlTextWriter writer)
        {
            if (Extender.AdapterEnabled)
                Extender.RenderEndTag(writer);
            else
                base.RenderEndTag(writer);
        }
        protected new override void RenderContents(HtmlTextWriter writer)
        {
            if (Extender.AdapterEnabled)
            {
                Wizard wizard = System.Web.UI.WebControls.Adapters.WebControlAdapter.Control as Wizard;
                if (!wizard == null)
                {
                    // -- Render Side Bar
                    RenderSideBar(writer, wizard);
                    // -- Render Header
                    RenderHeader(writer, wizard);
                    // -- Render WizardStep
                    RenderStep(writer, wizard);
                    // -- If on first page of wizard
                    if (wizard.ActiveStepIndex == 0)
                        RenderStartNavigation(writer, wizard);
                    else if (wizard.ActiveStepIndex < (wizard.WizardSteps.Count - 2))
                        RenderStepNavigation(writer, wizard);
                    else if (wizard.ActiveStepIndex == wizard.WizardSteps.Count - 2)
                        RenderFinishNavigation(writer, wizard);
                    else
                    {
                    }
                }
            }
            else
                // -- Use the default rendering of the control
                base.RenderContents(writer);
        }

        private void RenderFinishNavigation(HtmlTextWriter writer, Wizard wizard)
        {
            RenderNavigation(writer, wizard, CONTAINERID_FINISHNAVIGATION_TEMPLATE, wizard.FinishNavigationTemplate);
        }
        private void RenderStartNavigation(HtmlTextWriter writer, Wizard wizard)
        {
            RenderNavigation(writer, wizard, CONTAINERID_STARTNAVIGATION_TEMPLATE, wizard.StartNavigationTemplate);
        }
        private void RenderStepNavigation(HtmlTextWriter writer, Wizard wizard)
        {
            if (wizard.ActiveStep is TemplatedWizardStep)
                RenderNavigation(writer, wizard, CONTAINERID_STEPNAVIGATION_TEMPLATE, ((TemplatedWizardStep)wizard.ActiveStep).CustomNavigationTemplate);
            else
                RenderNavigation(writer, wizard, CONTAINERID_STEPNAVIGATION_TEMPLATE, wizard.StepNavigationTemplate);
        }
        private void RenderNavigation(HtmlTextWriter writer, Wizard wizard, string containerID, ITemplate template)
        {
            // -- Locate the name of the underlying container that will host the step navigation controls
            // -- You just have to know this name - it is used by the underyling Wizard control
            Control container = wizard.FindControl(containerID);
            // -- Check the container exists - academic it will always exist 
            if (!container == null)
            {
                // -- Start the step navigation with a DIV
                WebControlAdapterExtender.WriteBeginDiv(writer, CSS_NAV, "");
                // -- If a Template has been defined then use this
                if (!template == null)
                {
                    template.InstantiateIn(container);
                    container.RenderControl(writer);
                }
                else if (!(wizard.ActiveStep is TemplatedWizardStep))
                {
                    switch (containerID)
                    {
                        case CONTAINERID_STARTNAVIGATION_TEMPLATE:
                            {
                                // -- Only display Next and (optionally) Cancel
                                RenderSubmit(writer, wizard, container, wizard.StartNextButtonStyle, wizard.StartNextButtonImageUrl, wizard.StartNextButtonText, wizard.StartNextButtonType, CONTROLID_STARTNEXT);
                                if (wizard.DisplayCancelButton)
                                    RenderSubmit(writer, wizard, container, wizard.CancelButtonStyle, wizard.CancelButtonImageUrl, wizard.CancelButtonText, wizard.CancelButtonType, CONTROLID_CANCEL);
                                break;
                                break;
                            }

                        case CONTAINERID_STEPNAVIGATION_TEMPLATE:
                            {
                                // -- Display Previous (if AllowReturn true in previous step), Next and (optionally) Cancel
                                if (wizard.WizardSteps[wizard.ActiveStepIndex - 1].AllowReturn)
                                    RenderSubmit(writer, wizard, container, wizard.StepPreviousButtonStyle, wizard.StepPreviousButtonImageUrl, wizard.StepPreviousButtonText, wizard.StepPreviousButtonType, CONTROLID_STEPPREVIOUS);
                                RenderSubmit(writer, wizard, container, wizard.StepNextButtonStyle, wizard.StepNextButtonImageUrl, wizard.StepNextButtonText, wizard.StepNextButtonType, CONTROLID_STEPNEXT);
                                if (wizard.DisplayCancelButton)
                                    RenderSubmit(writer, wizard, container, wizard.CancelButtonStyle, wizard.CancelButtonImageUrl, wizard.CancelButtonText, wizard.CancelButtonType, CONTROLID_CANCEL);
                                break;
                                break;
                            }

                        case CONTAINERID_FINISHNAVIGATION_TEMPLATE:
                            {
                                // -- Display Previous, Complete and (optionally) Cancel
                                RenderSubmit(writer, wizard, container, wizard.FinishPreviousButtonStyle, wizard.FinishPreviousButtonImageUrl, wizard.FinishPreviousButtonText, wizard.FinishPreviousButtonType, CONTROLID_FINISHPREVIOUS);
                                RenderSubmit(writer, wizard, container, wizard.FinishCompleteButtonStyle, wizard.FinishCompleteButtonImageUrl, wizard.FinishCompleteButtonText, wizard.FinishCompleteButtonType, CONTROLID_FINISH);
                                if (wizard.DisplayCancelButton)
                                    RenderSubmit(writer, wizard, container, wizard.CancelButtonStyle, wizard.CancelButtonImageUrl, wizard.CancelButtonText, wizard.CancelButtonType, CONTROLID_CANCEL);
                                break;
                                break;
                            }

                        default:
                            {
                                break;
                                break;
                            }
                    }
                }
            }
            WebControlAdapterExtender.WriteEndDiv(writer);
        }
        private void RenderSubmit(HtmlTextWriter writer, Wizard wizard, Control container, Style buttonStyle, string buttonImageUrl, string buttonText, ButtonType buttonType, string submitControlRootName)
        {
            // -- Locate the corresponding control placeholder that the Wizard has defined within it's base 
            // -- Control heirarchy. For example, StepNextButton.
            string idWithType = WebControlAdapterExtender.MakeIdWithButtonType(submitControlRootName, buttonType);
            Control btn = container.FindControl(idWithType);
            string id = container.ID + "_" + submitControlRootName;
            // -- Will only be null if the submitControlRootName value was passed in. 
            if (!btn == null)
            {
                // -- Register the id of the button placeholder so that we can raise postback events
                System.Web.UI.Adapters.ControlAdapter.Page.ClientScript.RegisterForEventValidation(btn.UniqueID);
                // -- Only use client side (javascript based) submits for Links
                bool clientSubmit = true; // *** (buttonType = buttonType.Link)
                string javascript = "";
                if (clientSubmit)
                {
                    PostBackOptions options = new PostBackOptions(btn, "", "", false, false, false, clientSubmit, true, wizard.ID);
                    javascript = "javascript:" + System.Web.UI.Adapters.ControlAdapter.Page.ClientScript.GetPostBackEventReference(options);
                    javascript = System.Web.UI.Adapters.ControlAdapter.Page.Server.HtmlEncode(javascript);
                }
                Extender.WriteSubmit(writer, buttonType, buttonStyle.CssClass, id, buttonImageUrl, javascript, buttonText);
            }
        }
        private void RenderStep(HtmlTextWriter writer, Wizard wizard)
        {
            WebControlAdapterExtender.WriteBeginDiv(writer, CSS_STEP, "");
            foreach (Control control in wizard.ActiveStep.Controls)
                control.RenderControl(writer);
            WebControlAdapterExtender.WriteEndDiv(writer);
        }
        private void RenderHeader(HtmlTextWriter writer, Wizard wizard)
        {
            WebControlAdapterExtender.WriteBeginDiv(writer, CSS_HEADER, "");
            if (!wizard.HeaderTemplate == null)
            {
                string containerID = CONTAINERID_HEADER;
                Control container = wizard.FindControl(containerID);
                // -- Not sure why I don't need this line but it works anyway!
                // wizard.HeaderTemplate.InstantiateIn(container);
                container.RenderControl(writer);
            }
            else
                writer.Write(wizard.HeaderText);
            WebControlAdapterExtender.WriteEndDiv(writer);
        }
        private void RenderSideBar(HtmlTextWriter writer, Wizard wizard)
        {
            if (wizard.DisplaySideBar)
            {
                WebControlAdapterExtender.WriteBeginDiv(writer, CSS_SIDEBAR, "");
                string containerID = CONTAINERID_SIDEBAR;
                Control container = wizard.FindControl(containerID);
                if (!wizard.SideBarTemplate == null)
                    // -- Not sure why I don't need this line but it works anyway!
                    // wizard.SideBarTemplate.InstantiateIn(container);
                    container.RenderControl(writer);
                else
                {
                    Control listContainer = container.FindControl(CONTROLID_SIDEBARLIST);
                    int listIndex = 0;
                    writer.WriteLine();
                    foreach (WizardStep step in wizard.WizardSteps)
                    {
                        // -- Find the control within the container that contains the linkbutton
                        Control control = listContainer.Controls[listIndex];
                        // -- Find the LinkButton itself
                        LinkButton linkButton = control.FindControl(CONTROLID_SIDEBARBUTTON) as LinkButton;
                        // -- Get the postback javascript code and register the LinkButton control so that we can raise postback events 
                        string javascript = System.Web.UI.Adapters.ControlAdapter.Page.ClientScript.GetPostBackClientHyperlink(linkButton, "", true);
                        // -- Render the LinkButton using Anchors
                        writer.WriteBeginTag("a");
                        if (wizard.ActiveStepIndex == listIndex)
                            writer.WriteAttribute("class", CSS_ACTIVE);
                        writer.WriteAttribute("href", javascript);
                        writer.WriteAttribute("id", linkButton.ClientID);
                        writer.Write(HtmlTextWriter.TagRightChar);
                        writer.Write(step.Title);
                        writer.WriteEndTag("a");
                        writer.WriteLine();
                        System.Math.Max(System.Threading.Interlocked.Increment(ref listIndex), listIndex - 1);
                    }
                }
                WebControlAdapterExtender.WriteEndDiv(writer);
            }
        }
    }
}
