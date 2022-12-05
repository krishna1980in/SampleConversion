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
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;

using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.Compilation;
using System.Resources;
using System.Collections;
using System.Collections.Specialized;
using kartrisLanguageDataTableAdapters;
using kartrisLanguageData;


/// <summary>

/// ''' uses the standard ResourceProviderFactory in .NET framework - here we're creating a class that inherits this

/// ''' provider to get resources from Kartris Language Strings table instead from a physical resource file.

/// ''' </summary>
public sealed class SqlResourceProviderFactory : ResourceProviderFactory
{
    public SqlResourceProviderFactory()
    {
    }

    public new override IResourceProvider CreateGlobalResourceProvider(string classKey)
    {
        return new SqlResourceProvider(null, classKey);
    }

    public new override IResourceProvider CreateLocalResourceProvider(string virtualPath)
    {
        virtualPath = Path.GetFileName(virtualPath);
        return new SqlResourceProvider(virtualPath, null);
    }
    // inner class 
    public sealed class SqlResourceProvider : IResourceProvider
    {
        private string _virtualPath;
        private string _className;
        private IDictionary _resourceCache;


        private static object CultureNeutralKey = new object();

        public SqlResourceProvider(string virtualPath, string className)
        {
            _virtualPath = virtualPath;
            _className = className;
            try
            {
                LanguageStringProviders.LoadedProviders.Add(this);
            }
            catch (Exception ex)
            {
                CkartrisBLL.RecycleAppPool();
            }
        }

        public void ClearResourceCache()
        {
            // *** Just force the resource manager to be completely reloaded
            this._resourceCache.Clear();
        }
        private IDictionary GetResourceCache(string cultureName)
        {
            object cultureKey;

            if (cultureName != null)
                cultureKey = cultureName;
            else
                cultureKey = CultureNeutralKey;

            if (_className == "RESETme")
                _resourceCache = null;
            if (_resourceCache == null)
                _resourceCache = new ListDictionary();

            IDictionary resourceDict = _resourceCache[cultureKey] as IDictionary;

            if (resourceDict == null)
            {
                resourceDict = SqlResourceHelper.GetResources(_virtualPath, _className, cultureName, false, null);
                _resourceCache[cultureKey] = resourceDict;
            }

            return resourceDict;
        }

        private object GetObject(string resourceKey, CultureInfo culture)
        {
            string cultureName = null;
            if (culture != null)
                cultureName = culture.Name;
            else
                cultureName = CultureInfo.CurrentUICulture.Name;



            object value = GetResourceCache(cultureName)[resourceKey];
            if (value == null)
            {
                // resource is missing for current culture 
                SqlResourceHelper.AddResource(resourceKey, _virtualPath, _className, cultureName);
                value = GetResourceCache(null)[resourceKey];
            }

            if (value == null)
                // the resource is really missing, no default exists 
                SqlResourceHelper.AddResource(resourceKey, _virtualPath, _className, string.Empty);

            return value;
        }

        private IResourceReader ResourceReader
        {
            get
            {
                return new SqlResourceReader(GetResourceCache(null));
            }
        }
    }
    // inner class 
    private sealed class SqlResourceReader : IResourceReader
    {
        private IDictionary _resources;

        public SqlResourceReader(IDictionary resources)
        {
            _resources = resources;
        }

        IDictionaryEnumerator IResourceReader.GetEnumerator()
        {
            return _resources.GetEnumerator();
        }


        public void Close()
        {
        }

         IEnumerator IEnumerable.GetEnumerator()
        {
            return _resources.GetEnumerator();
        }

        public void Dispose()
        {
        }
    }
}


internal sealed class SqlResourceHelper
{
    private SqlResourceHelper()
    {
    }

    /// <summary>
    ///     ''' 
    ///     ''' </summary>
    static SqlResourceHelper()
    {
    }

    private static int GetLangIDfromCulture(string cultureName)
    {
        var Adptr = new LanguagesTblAdptr();
        // Return Adptr.GetLanguageIDByCulture_s(cultureName)
        return LanguagesBLL.GetLanguageIDByCulture_s(cultureName);
    }

    /// <summary>
    ///     ''' Get Language String
    ///     ''' </summary>

    public static IDictionary GetResources(string virtualPath, string className, string cultureName, bool designMode, IServiceProvider serviceProvider)
    {
        var Adptr = new LanguageStringsTblAdptr();

        // Initialize KartrisCore connection string
        if (string.IsNullOrEmpty(Adptr.Connection.ConnectionString))
            // My.Settings.Item("LanguageConnection") = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ConnectionString
            Adptr.Connection.ConnectionString = ConfigurationManager.ConnectionStrings("KartrisSQLConnection").ConnectionString;

        LanguageStringsDataTable dtLanguageStrings = new LanguageStringsDataTable();
        DataRow drLanguageStrings;

        short LANG_ID;

        if (!String.IsNullOrEmpty(cultureName))
        {
            try
            {
                LANG_ID = GetLangIDfromCulture(cultureName);
            }
            catch (Exception ex)
            {
                LANG_ID = 1;
            }
        }
        else
            LANG_ID = 1;
        // get resource values 
        if (!String.IsNullOrEmpty(virtualPath))
            // Get Local resources 
            Adptr.FillByVirtualPath(dtLanguageStrings, LANG_ID, virtualPath);
        else if (!String.IsNullOrEmpty(className))
            // Get Global resources 
            Adptr.FillByClassName(dtLanguageStrings, LANG_ID, className);
        else
            // Neither virtualPath or className provided, unknown if Local or Global resource 
            throw new Exception("SqlResourceHelper.GetResources() - virtualPath or className missing from parameters.");


        ListDictionary resources = new ListDictionary();
        try
        {
            foreach (var drLanguageStrings in dtLanguageStrings.Rows)
            {
                if (drLanguageStrings.Item("ls_value").ToString == "" || drLanguageStrings.Item("ls_value").ToString == null)
                {
                    string strLsName = drLanguageStrings.Item("ls_name").ToString;
                    resources.Add(drLanguageStrings.Item("ls_name").ToString, "$_" + Strings.Right(strLsName, strLsName.Length - strLsName.IndexOf("_") - 1));
                }
                else
                    resources.Add(drLanguageStrings.Item("ls_name").ToString, drLanguageStrings.Item("ls_value").ToString);
            }
        }
        catch (Exception e)
        {
            SqlConnection.ClearPool(Adptr.Connection);
            throw new Exception(e.Message, e);
        }
        finally
        {
        }
        return resources;
    }

    /// <summary>
    ///     ''' New language string
    ///     ''' </summary>
    public static void AddResource(string resource_name, string virtualPath, string className, string cultureName)
    {
    }
}

public class LanguageStringProviders
{
    /// <summary>
    ///     ''' Keep track of loaded providers so we can unload them
    ///     ''' </summary>
    internal static List<SqlResourceProviderFactory.SqlResourceProvider> LoadedProviders = new List<SqlResourceProviderFactory.SqlResourceProvider>();

    /// <summary>
    ///     ''' Allow unloading of all loaded providers
    ///     ''' </summary>
    public static void Refresh()
    {
        foreach (SqlResourceProviderFactory.SqlResourceProvider provider in LoadedProviders)
            provider.ClearResourceCache();
    }
}

public interface SqlResourceProvider
{
    /// <summary>
    ///     ''' Interface method used to force providers to register themselves
    ///     ''' with LanguageStringProviders.LoadProviders
    ///     ''' </summary>
    void ClearResourceCache();
}
