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

using System.Web.HttpContext;
using System.Data.Common;
using CkartrisBLL;
using CkartrisDataManipulation;
using System.Web.Configuration;
using System.Xml;
using System.Xml.Serialization;
using KartSettingsManager;
using System.Web.UI.TemplateControl;
using System.Web.UI;

public class Payment
{
    public static object Deserialize(string strValue, Type strObjectType)
    {
        StringReader strStringReader = new StringReader(strValue);
        XmlSerializer oXS; // = New XmlSerializer(strObjectType)

        if (strObjectType == typeof(Kartris.Basket))
        {
            Type[] extraTypes = new Type[2];
            extraTypes[0] = typeof(List<Kartris.Promotion>);
            extraTypes[1] = typeof(PromotionBasketModifier);
            oXS = new XmlSerializer(typeof(Kartris.Basket), extraTypes);
        }
        else
            oXS = new XmlSerializer(strObjectType);

        Deserialize = oXS.Deserialize(strStringReader);
    }

    public static string Serialize(object strObject)
    {
        try
        {
            // Dim oXS As XmlSerializer = New XmlSerializer(strObject.GetType)
            XmlSerializer oXS; // = New XmlSerializer(strObject.GetType)

            if (strObject.GetType() == typeof(Kartris.Basket))
            {
                Type[] extraTypes = new Type[2];
                extraTypes[0] = typeof(List<Kartris.Promotion>);
                extraTypes[1] = typeof(PromotionBasketModifier);
                oXS = new XmlSerializer(typeof(Kartris.Basket), extraTypes);
            }
            else
                oXS = new XmlSerializer(strObject.GetType());
            StringWriter oStrW = new StringWriter();

            // Serialize the object into an XML string
            oXS.Serialize(oStrW, strObject);
            Serialize = oStrW.ToString();
            oStrW.Close();
        }
        catch (Exception ex)
        {
            DumpException(ex);
            return null;
        }
    }

    public static void DumpException(Exception ex)
    {
        WriteExceptionInfo(ex);
        ex = ex.InnerException;
        if (ex != null)
        {
            WriteExceptionInfo(ex.InnerException);
            ex = ex.InnerException;
        }
    }
    public static void WriteExceptionInfo(Exception ex)
    {
        CkartrisFormatErrors.LogError("Message: " + ex.Message + Constants.vbCrLf + "Exception Type: " + ex.GetType().FullName + Constants.vbCrLf + "Source: " + ex.Source + Constants.vbCrLf + "StrackTrace: " + ex.StackTrace + Constants.vbCrLf);
    }

    public static Interfaces.PaymentGateway PPLoader(string Path)
    {
        Hashtable m_clsPlugins = new Hashtable();
        Type clsType;
        Type clsInterface;
        bool boolAdded;
        Collection clsPlugins = new Collection();
        System.Reflection.Assembly clsAssembly;
        // we're requesting the plugin at the path specified to be added to Kartris. the first step in this process
        // is to try to load the assembly using Reflection. this will give us access to the definitions of the classes defined in 
        // that assembly, and we can look for any classes that implement our PaymentGateway interface...
        try
        {
            string GatewayPath = System.Web.HttpContext.Current.Request.PhysicalApplicationPath + @"Plugins\" + Path + @"\" + Path + ".dll";
            clsAssembly = System.Reflection.Assembly.LoadFrom(GatewayPath);
        }
        catch (Exception ex)
        {
            throw new System.Exception("An error occured while attempting to access the specified Assembly. - " + ex.Message);
        }
        try
        {
            // look for appropriate types... 
            foreach (var clsType in clsAssembly.GetTypes())
            {
                // only look at types we can create... 
                if (clsType.IsPublic == true)
                {
                    // ignore abstract classes... 
                    if (!((clsType.Attributes & System.Reflection.TypeAttributes.Abstract) == System.Reflection.TypeAttributes.Abstract))
                    {
                        // check for the implementation of the specified interface... 
                        clsInterface = clsType.GetInterface("PaymentGateway", true);
                        // process if valid...
                        if (!(clsInterface == null))
                        {
                            // create a unique key to identify this plugin (so we don't add it twice)...
                            string sPluginKey = string.Concat(Path, "_", clsType.FullName);
                            // check to see if we already have this plugin added...
                            if (!m_clsPlugins.ContainsKey(sPluginKey))
                            {
                                // load the plugin into memory, creating an instance of it...
                                return LoadPlugin(Path, clsType.FullName);
                                // store a reference to the plugin locally...
                                m_clsPlugins.Add(sPluginKey, PPLoader);
                            }
                            // set a flag indicating that we have added at least one plugin from the associated file...
                            boolAdded = true;
                        }
                    }
                }
            }
            return null/* TODO Change to default(_) if this is not a reference type */;
        }
        catch (Exception ex)
        {
            return null/* TODO Change to default(_) if this is not a reference type */;
        }
    }

    private static Interfaces.PaymentGateway LoadPlugin(string AssemblyPath, string ClassName)
    {
        object clsRet;
        System.Reflection.Assembly clsAssembly;
        string GatewayPath = System.Web.HttpContext.Current.Request.PhysicalApplicationPath + @"Plugins\" + AssemblyPath + @"\" + AssemblyPath + ".dll";
        // load the assembly...
        clsAssembly = System.Reflection.Assembly.LoadFrom(GatewayPath);
        // create an instance of the class that implements the PaymentGateway interface...
        clsRet = clsAssembly.CreateInstance(ClassName);
        // return the new instance...
        return (Interfaces.PaymentGateway)clsRet;
    }

    public static Interfaces.ShippingGateway SPLoader(string Path)
    {
        Hashtable m_clsPlugins = new Hashtable();
        Type clsType;
        Type clsInterface;
        bool boolAdded;
        Collection clsPlugins = new Collection();
        System.Reflection.Assembly clsAssembly = null;
        // we're requesting the plugin at the path specified to be added to Kartris. the first step in this process
        // is to try to load the assembly using Reflection. this will give us access to the definitions of the classes defined in 
        // that assembly, and we can look for any classes that implement our PaymentGateway interface...
        try
        {
            string GatewayPath = System.Web.HttpContext.Current.Request.PhysicalApplicationPath + @"Plugins\" + Path + @"\" + Path + ".dll";
            if (Strings.InStr(Path, "GoogleCheckout"))
                return null/* TODO Change to default(_) if this is not a reference type */;
            clsAssembly = System.Reflection.Assembly.LoadFrom(GatewayPath);
        }
        catch (Exception ex)
        {
            CkartrisFormatErrors.LogError("An error occured while attempting to access the specified Assembly. - " + ex.Message);
        }
        try
        {
            // look for appropriate types... 
            foreach (var clsType in clsAssembly.GetTypes())
            {
                // only look at types we can create... 
                if (clsType.IsPublic == true)
                {
                    // ignore abstract classes... 
                    if (!((clsType.Attributes & System.Reflection.TypeAttributes.Abstract) == System.Reflection.TypeAttributes.Abstract))
                    {
                        // check for the implementation of the specified interface... 
                        clsInterface = clsType.GetInterface("ShippingGateway", true);
                        // process if valid...
                        if (!(clsInterface == null))
                        {
                            // create a unique key to identify this plugin (so we don't add it twice)...
                            string sPluginKey = string.Concat(Path, "_", clsType.FullName);
                            // check to see if we already have this plugin added...
                            if (!m_clsPlugins.ContainsKey(sPluginKey))
                            {
                                // load the plugin into memory, creating an instance of it...
                                return LoadSPlugin(Path, clsType.FullName);
                                // store a reference to the plugin locally...
                                m_clsPlugins.Add(sPluginKey, SPLoader);
                            }
                            // set a flag indicating that we have added at least one plugin from the associated file...
                            boolAdded = true;
                        }
                    }
                }
            }
            return null/* TODO Change to default(_) if this is not a reference type */;
        }
        catch (Exception ex)
        {
            CkartrisFormatErrors.LogError("Assembly loaded but there seem to be a problem reading it. - " + Path + " - " + ex.Message);
        }
        return null/* TODO Change to default(_) if this is not a reference type */;
    }

    private static Interfaces.ShippingGateway LoadSPlugin(string AssemblyPath, string ClassName)
    {
        object clsRet;
        System.Reflection.Assembly clsAssembly;
        string GatewayPath = System.Web.HttpContext.Current.Request.PhysicalApplicationPath + @"Plugins\" + AssemblyPath + @"\" + AssemblyPath + ".dll";
        // load the assembly...
        clsAssembly = System.Reflection.Assembly.LoadFrom(GatewayPath);
        // create an instance of the class that implements the PaymentGateway interface...
        clsRet = clsAssembly.CreateInstance(ClassName);
        // return the new instance...
        return (Interfaces.ShippingGateway)clsRet;
    }

    public static string GetPluginFriendlyName(string strGatewayName)
    {
        try
        {
            string strSettingName = "FriendlyName(" + LanguagesBLL.GetCultureByLanguageID_s(System.Convert.ToInt32(System.Web.HttpContext.Current.Session["LANG"])) + ")";

            ExeConfigurationFileMap objConfigFileMap = new ExeConfigurationFileMap();
            objConfigFileMap.ExeConfigFilename = Path.Combine(System.Web.HttpContext.Current.Request.PhysicalApplicationPath, @"Plugins\" + strGatewayName + @"\" + strGatewayName + ".dll.config");
            objConfigFileMap.MachineConfigFilename = Path.Combine(System.Web.HttpContext.Current.Request.PhysicalApplicationPath, @"Uploads\resources\Machine.Config");
            System.Configuration.Configuration objConfiguration = ConfigurationManager.OpenMappedExeConfiguration(objConfigFileMap, ConfigurationUserLevel.None);

            ConfigurationSectionGroup objSectionGroup = objConfiguration.GetSectionGroup("applicationSettings");
            ClientSettingsSection appSettingsSection = (ClientSettingsSection)objSectionGroup.Sections.Item("Kartris.My.MySettings");
            return appSettingsSection.Settings.Get(strSettingName).Value.ValueXml.InnerText;
        }
        catch (Exception ex)
        {
            return "";
        }
    }
    public static bool isAnonymousCheckoutEnabled(string strGatewayName)
    {
        try
        {
            string strSettingName = "AnonymousCheckout";

            ExeConfigurationFileMap objConfigFileMap = new ExeConfigurationFileMap();
            objConfigFileMap.ExeConfigFilename = Path.Combine(System.Web.HttpContext.Current.Request.PhysicalApplicationPath, @"Plugins\" + strGatewayName + @"\" + strGatewayName + ".dll.config");
            objConfigFileMap.MachineConfigFilename = Path.Combine(System.Web.HttpContext.Current.Request.PhysicalApplicationPath, @"Uploads\resources\Machine.Config");
            System.Configuration.Configuration objConfiguration = ConfigurationManager.OpenMappedExeConfiguration(objConfigFileMap, ConfigurationUserLevel.None);

            ConfigurationSectionGroup objSectionGroup = objConfiguration.GetSectionGroup("applicationSettings");
            ClientSettingsSection appSettingsSection = (ClientSettingsSection)objSectionGroup.Sections.Item("Kartris.My.MySettings");
            bool blnAnonymousCheckoutValue = false;
            try
            {
                blnAnonymousCheckoutValue = System.Convert.ToBoolean(appSettingsSection.Settings.Get(strSettingName).Value.ValueXml.InnerText);
            }
            catch (Exception ex)
            {
            }
            return blnAnonymousCheckoutValue;
        }
        catch (Exception ex)
        {
            return "";
        }
    }
}
