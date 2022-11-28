partial class _AutoCompleteInput : System.Web.UI.UserControl
{
    private string c_MethodName;
    private string c_ContextKey;
    private string c_Behavior;

    public string MethodName
    {
        get
        {
            return autCompleteCtrl.ServiceMethod;
        }
        set
        {
            c_MethodName = value;
            autCompleteCtrl.ServiceMethod = c_MethodName;
        }
    }

    public string ContextKey
    {
        set
        {
            c_ContextKey = value;
        }
    }

    public string Behavior
    {
        set
        {
            autCompleteCtrl.BehaviorID = value;
        }
    }

    public string GetText()
    {
        return txtBox.Text;
    }

    public void SetText(string strText)
    {
        txtBox.Text = strText;
    }

    public void SetWidth(int intWidth)
    {
        txtBox.Width = intWidth;
    }

    public void ClearText()
    {
        txtBox.Text = "";
    }

    public void SetFoucs()
    {
        txtBox.Focus();
    }
}
