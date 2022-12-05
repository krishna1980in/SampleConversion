using System;
using System.IO;
using KartSettingsManager;
using Microsoft.Web.Administration;
using System.Threading;
using System.Globalization;
using System.Net.Mail;
using System.Web.HttpContext;
using System.Web.UI;
using System.Text;
using System.Xml;
using MailKit.Net.Smtp;
using MailKit;
using MimeKit;
using System.Data;
using System.Data.SqlClient;






public sealed class CkartrisEnumerations
{
	
	public const decimal KARTRIS_VERSION = 3.3M;
	public static readonly DateTime KARTRIS_VERSION_ISSUE_DATE = new DateTime(2022, 6, 8); //' MM/dd/yyyy
	
	public enum LANG_ELEM_TABLE_TYPE
	{
		Versions = 1,
		Products = 2,
		Categories = 3,
		Attributes = 4,
		Options = 5,
		OptionGroups = 6,
		Promotions = 7,
		Pages = 8,
		ShippingMethods = 9,
		ShippingZones = 10,
		Destination = 11,
		CustomerGroups = 12,
		Currencies = 13,
		AttributeValues = 14,
		PromotionStrings = 15,
		News = 16,
		KnowledgeBase = 17,
		AttributeOption = 18
	}
	
	public enum LANG_ELEM_FIELD_NAME
	{
		Name = 1,
		Description = 2,
		PageTitle = 3,
		MetaDescription = 4,
		MetaKeywords = 5,
		Text = 6,
		StrapLine = 7,
		URLName = 8
	}
	
	public enum DML_OPERATION
	{
		INSERT = 1,
		UPDATE = 2,
		DELETE = 3,
		CLONE = 4
	}
	
	public enum ADMIN_LOG_TABLE
	{
		Config = 1,
		Currency = 2,
		Languages = 3,
		SiteText = 4,
		Logins = 5,
		DataRecords = 6,
		ExecuteQuery = 7,
		Triggers = 8,
		Orders = 9
	}
	
	public enum MESSAGE_TYPE
	{
		Confirmation = 1,
		ErrorMessage = 2,
		Information = 3
	}
	
	public enum SEARCH_TYPE
	{
		classic = 1,
		advanced = 2
	}
}

/// <summary>
/// Error formatting and handling
/// </summary>

public sealed class CkartrisFormatErrors
{
	/// <summary>
	/// Log the errors
	/// </summary>
	public static void LogError(string strDescription)
	{
		
		bool blnCultureChanged = false;
		string strCurrentCulture = "";
		
		try
		{
			if (HttpContext.Current.Session("KartrisUserCulture") != null)
			{
				strCurrentCulture = HttpContext.Current.Session("KartrisUserCulture").ToString();
			}
			Thread.CurrentThread.CurrentUICulture = new CultureInfo(LanguagesBLL.GetCultureByLanguageID_s(LanguagesBLL.GetDefaultLanguageID()));
			Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(LanguagesBLL.GetCultureByLanguageID_s(LanguagesBLL.GetDefaultLanguageID()));
			blnCultureChanged = true;
		}
		catch (Exception)
		{
			if (strCurrentCulture != "")
			{
				Thread.CurrentThread.CurrentUICulture = new CultureInfo(strCurrentCulture);
				Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(strCurrentCulture);
				blnCultureChanged = false;
			}
		}
		
		if (GetKartConfig("general.errorlogs.enabled") == "y")
		{
			string strErrorDescription = "";
			
			if (strDescription != "")
			{
				strErrorDescription = strDescription;
			}
			else
			{
				strErrorDescription = System.Convert.ToString(HttpContext.Current.Server.GetLastError.ToString());
			}
			
			string strDirPath = "";
			string strFilePath = "";
			
			strDirPath = System.Convert.ToString(HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings("ErrorLogPath") + "/Errors/" + Strings.Format(DateTime.Now, "yyyy.MM") + "/"));
			strFilePath = strDirPath + System.Convert.ToString(DateTime.Now.Year)+"." + Strings.Format(DateTime.Now, "MM") +"." + Strings.Format(DateTime.Now, "dd") +".config";
			
			if (!Directory.Exists(strDirPath))
			{
				Directory.CreateDirectory(strDirPath);
			}
			
			StreamWriter swtErrors = null;
			if (!File.Exists(strFilePath))
			{
				swtErrors = File.CreateText(strFilePath);
			}
			else
			{
				swtErrors = File.AppendText(strFilePath);
			}
			
			swtErrors.WriteLine("-----------------------------------------------------------------------------");
			try
			{
				swtErrors.WriteLine(">>" + Strings.Space(5) + "Version:" + CkartrisEnumerations.KARTRIS_VERSION);
				swtErrors.WriteLine(">>" + Strings.Space(5) + "URL:" + HttpContext.Current.Request.Url.ToString());
				swtErrors.WriteLine(">>" + Strings.Space(5) + "Page:" + HttpContext.Current.Session("previous_page"));
			}
			catch (Exception)
			{
				
			}
			swtErrors.WriteLine("-----------------------------------------------------------------------------");
			swtErrors.WriteLine(">>" + Strings.Space(5) + DateTime.Now.ToString());
			try
			{
				swtErrors.WriteLine(">>" + Strings.Space(5) + CkartrisEnvironment.GetClientIPAddress());
			}
			catch (Exception)
			{
				
			}
			swtErrors.WriteLine(">>" + Strings.Space(5) + "DESCRIPTION:");
			swtErrors.WriteLine(strErrorDescription);
			
			if (strErrorDescription.Contains("Format of the initialization string does not conform to specification starting at index 0"))
			{
				//This error can generally be cleared by a recycle
				CkartrisBLL.RecycleAppPool();
				swtErrors.WriteLine("We believe this happens due to app pool issues. Please see http://www.kartris.com/Knowledgebase/Error__k-55.aspx for help.");
			}
			
			swtErrors.WriteLine("");
			swtErrors.WriteLine("==================================================");
			swtErrors.WriteLine("");
			swtErrors.WriteLine("");
			swtErrors.Flush();
			swtErrors.Close();
		}
		
		if (blnCultureChanged && !string.IsNullOrEmpty(strCurrentCulture))
		{
			Thread.CurrentThread.CurrentUICulture = new CultureInfo(strCurrentCulture);
			Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(strCurrentCulture);
		}
		
		HttpContext.Current.Server.ClearError();
		
		if (string.IsNullOrEmpty(strDescription))
		{
			HttpContext.Current.Server.Transfer("~/Error.aspx");
		}
		
	}
	
	/// <summary>
	/// Report a handled error
	/// </summary>
	public static void ReportHandledError(Exception _ex, Reflection.MethodBase _Source, ref string _msg)
	{
		bool blnLogsEnabled = true;
		try
		{
			blnLogsEnabled = GetKartConfig("general.errorlogs.enabled") == "y";
		}
		catch (Exception)
		{
			
		}
		
		if (blnLogsEnabled)
		{
			bool blnCultureChanged = false;
			string strCurrentCulture = "";
			try
			{
				strCurrentCulture = HttpContext.Current.Session("KartrisUserCulture").ToString();
				Thread.CurrentThread.CurrentUICulture = new CultureInfo(LanguagesBLL.GetCultureByLanguageID_s(LanguagesBLL.GetDefaultLanguageID()));
				Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(LanguagesBLL.GetCultureByLanguageID_s(LanguagesBLL.GetDefaultLanguageID()));
				blnCultureChanged = true;
			}
			catch (Exception)
			{
				if (strCurrentCulture != "")
				{
					Thread.CurrentThread.CurrentUICulture = new CultureInfo(strCurrentCulture);
					Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(strCurrentCulture);
					blnCultureChanged = false;
				}
			}
			string strDirPath = System.Convert.ToString(HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings("ErrorLogPath") + "/Errors/" + Strings.Format(DateTime.Now, "yyyy.MM") + "/"));
			string strFilePath = strDirPath + System.Convert.ToString(DateTime.Now.Year)+"." + Strings.Format(DateTime.Now, "MM") +"." + Strings.Format(DateTime.Now, "dd") +".config";
			
			if (!Directory.Exists(strDirPath))
			{
				Directory.CreateDirectory(strDirPath);
			}
			
			StreamWriter swtErrors = null;
			if (!File.Exists(strFilePath))
			{
				swtErrors = File.CreateText(strFilePath);
			}
			else
			{
				swtErrors = File.AppendText(strFilePath);
			}
			
			GenerateErrorMessage(_ex, ref _msg);
			
			swtErrors.WriteLine("-----------------------------------------------------------------------------");
			swtErrors.WriteLine(">>    " + _Source.ReflectedType.FullName +"." + _Source.Name);
			swtErrors.WriteLine("-----------------------------------------------------------------------------");
			swtErrors.WriteLine(">>    " + _ex.GetType().ToString());
			swtErrors.WriteLine(">>    " + DateTime.Now.ToString());
			swtErrors.WriteLine(">>" + Strings.Space(5) + "Version:" + CkartrisEnumerations.KARTRIS_VERSION);
			swtErrors.WriteLine(">>" + Strings.Space(5) + "URL:" + HttpContext.Current.Request.Url.ToString());
			swtErrors.WriteLine(">>    " + CkartrisEnvironment.GetClientIPAddress());
			swtErrors.WriteLine(">>    CUSTOM MESSAGE:");
			swtErrors.WriteLine(_msg);
			try
			{
				swtErrors.WriteLine(">>    NUMBER:" + ((SqlException) _ex).Number.ToString());
			}
			catch (Exception)
			{
			}
			swtErrors.WriteLine(">>    MESSAGE:");
			swtErrors.WriteLine(_ex.Message);
			swtErrors.WriteLine(">>    STACK:");
			swtErrors.WriteLine(_ex.StackTrace);
			swtErrors.WriteLine("==================================================");
			swtErrors.WriteLine("");
			swtErrors.WriteLine("");
			swtErrors.Flush();
			swtErrors.Close();
			
			if (blnCultureChanged)
			{
				Thread.CurrentThread.CurrentUICulture = new CultureInfo(strCurrentCulture);
				Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(strCurrentCulture);
			}
		}
	}
	
	/// <summary>
	/// Report an unhandled error
	/// </summary>
	public static void ReportUnHandledError()
	{
		bool blnLogsEnabled = true;
		try
		{
			blnLogsEnabled = GetKartConfig("general.errorlogs.enabled") == "y";
		}
		catch (Exception)
		{
			
		}
		
		if (blnLogsEnabled)
		{
			string strErrorDescription = System.Convert.ToString(HttpContext.Current.Server.GetLastError.ToString());
			try
			{
				if (!strErrorDescription.Contains("System.Security.Cryptography.RijndaelManagedTransform.DecryptData"))
				{
					string strDirPath = "";
					string strFilePath = "";
					
					strDirPath = System.Convert.ToString(HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings("ErrorLogPath") + "/Errors/" + Strings.Format(DateTime.Now, "yyyy.MM") + "/"));
					strFilePath = strDirPath + System.Convert.ToString(DateTime.Now.Year)+"." + Strings.Format(DateTime.Now, "MM") +"." + Strings.Format(DateTime.Now, "dd") +".config";
					
					if (!Directory.Exists(strDirPath))
					{
						Directory.CreateDirectory(strDirPath);
					}
					
					StreamWriter swtErrors = null;
					if (!File.Exists(strFilePath))
					{
						swtErrors = File.CreateText(strFilePath);
					}
					else
					{
						swtErrors = File.AppendText(strFilePath);
					}
					
					swtErrors.WriteLine("-----------------------------------------------------------------------------");
					swtErrors.WriteLine(">>" + Strings.Space(5) + "Unhandled Error occurred in Page:" + System.Web.HttpContext.Current.Request.Url.ToString());
					swtErrors.WriteLine("-----------------------------------------------------------------------------");
					swtErrors.WriteLine(">>" + Strings.Space(5) + DateTime.Now.ToString());
					try
					{
						swtErrors.WriteLine(">>" + Strings.Space(5) + "Version:" + CkartrisEnumerations.KARTRIS_VERSION);
						swtErrors.WriteLine(">>" + Strings.Space(5) + "URL:" + HttpContext.Current.Request.Url.ToString());
						swtErrors.WriteLine(">>" + Strings.Space(5) + CkartrisEnvironment.GetClientIPAddress());
					}
					catch (Exception)
					{
						
					}
					
					
					//Let's add some extra info for errors we
					//see alot and recognize
					if (strErrorDescription.Contains("Format of the initialization string does not conform to specification starting at index 0"))
					{
						//This error can generally be cleared by a recycle
						CkartrisBLL.RecycleAppPool();
						swtErrors.WriteLine(". . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . .");
						swtErrors.WriteLine("We believe this happens due to app pool issues. Please see ");
						swtErrors.WriteLine("http://www.kartris.com/Knowledgebase/Error__k-55.aspx for help.");
						swtErrors.WriteLine(". . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . .");
					}
					
					
					{
						
						strErrorDescription.Contains("The resource object with key 'PageTitle_WelcomeToKartris' was not found");
						
						swtErrors.WriteLine(". . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . .");
						swtErrors.WriteLine("HINT: The error below suggests a problem connecting to your database and retrieving");
						swtErrors.WriteLine("data. It could be that your db connection string in the web.config is wrong, but");
						swtErrors.WriteLine("but more likely your db lacks the permissions for the user that the web site is");
						swtErrors.WriteLine("running as. Your site is running as " + System.Security.Principal.WindowsIdentity.GetCurrent().Name);
						swtErrors.WriteLine("so check this user has permissions on the database too.");
						swtErrors.WriteLine(". . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . .");
					}
					
					//Now full error message
					swtErrors.WriteLine(">>" + Strings.Space(5) + "DESCRIPTION:");
					swtErrors.WriteLine(strErrorDescription);
					swtErrors.WriteLine("");
					swtErrors.WriteLine("==================================================");
					swtErrors.WriteLine("");
					swtErrors.WriteLine("");
					swtErrors.Flush();
					swtErrors.Close();
				}
			}
			catch (Exception)
			{
			}
		}
	}
	
	/// <summary>
	/// Generate an error message
	/// </summary>
	private static void GenerateErrorMessage(Exception _ex, ref string _msg)
	{
		if ((string) (_ex.GetType().ToString()) == "System.Data.SqlClient.SqlException")
		{
			SqlException ex = (SqlException) _ex;
			if ((int) ex.Number == 4060)
			{
				_msg += System.Convert.ToString(HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_ErrorMsgDBAvailability"));
			}
			else if ((int) ex.Number == 18456)
			{
				_msg += System.Convert.ToString(HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_ErrorMsgDBAuthentication"));
			}
			else if ((int) ex.Number == 10054)
			{
				_msg += System.Convert.ToString(HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_ErrorMsgDBConnectivity"));
			}
			else if ((int) ex.Number == 4929)
			{
				_msg += System.Convert.ToString(HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_ErrorMsgDBChangeViolation"));
			}
			else if ((int) ex.Number == 547)
			{
				_msg += System.Convert.ToString(HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_ErrorMsgDBReferenceViolation"));
			}
			else if (((int) ex.Number == 2627) || ((int) ex.Number == 2601))
			{
				_msg += System.Convert.ToString(HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_ErrorMsgDBDuplicateViolation"));
			}
			else if ((int) ex.Number == 245)
			{
				_msg += System.Convert.ToString(HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_ErrorMsgDBDataTypeViolation"));
			}
			else if (((((int) ex.Number == 229) || ((int) ex.Number == 230)) || ((int) ex.Number == 262)) || ((int) ex.Number == 300))
			{
				_msg += System.Convert.ToString(HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_ErrorMsgDBPermissionDenied"));
			}
			else if (((int) ex.Number == 2812) || ((int) ex.Number == 208))
			{
				_msg += System.Convert.ToString(HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_ErrorMsgDBInvalidObject"));
			}
			else if ((int) ex.Number == 201)
			{
				_msg += System.Convert.ToString(HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_ErrorMsgDBValueNotSupplied"));
			}
			else if ((int) ex.Number == 3609)
			{
				_msg += System.Convert.ToString(HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_ErrorMsgDBObjectIsLocked"));
			}
			else
			{
				_msg += System.Convert.ToString(HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_ErrorMsgDBGeneralError"));
			}
		}
		else if ((string) (_ex.GetType().ToString()) == "System.ApplicationException")
		{
			_msg += System.Convert.ToString(_ex.Message);
		}
		else
		{
			_msg += System.Convert.ToString(HttpContext.GetGlobalResourceObject("_Kartris", "ContentText_ErrorMsgGeneral"));
		}
	}
}

/// <summary>
/// Replace accented characters with non-accented equivalents
/// </summary>
public sealed class CkartrisDisplayFunctions
{
	
	public static string ReplaceAccentedCharacters(string strInput)
	{
		strInput = strInput.Replace("�", "a");
		strInput = strInput.Replace("�", "a");
		strInput = strInput.Replace("�", "a");
		strInput = strInput.Replace("�", "a'");
		strInput = strInput.Replace("�", "a'");
		strInput = strInput.Replace("�", "A");
		strInput = strInput.Replace("�", "A");
		strInput = strInput.Replace("�", "A");
		strInput = strInput.Replace("�", "A");
		strInput = strInput.Replace("�", "A");
		strInput = strInput.Replace("�", "e");
		strInput = strInput.Replace("�", "e");
		strInput = strInput.Replace("�", "e'");
		strInput = strInput.Replace("�", "e'");
		strInput = strInput.Replace("�", "E'");
		strInput = strInput.Replace("�", "E'");
		strInput = strInput.Replace("�", "E");
		strInput = strInput.Replace("�", "E");
		strInput = strInput.Replace("�", "i");
		strInput = strInput.Replace("�", "i");
		strInput = strInput.Replace("�", "i'");
		strInput = strInput.Replace("�", "i'");
		strInput = strInput.Replace("�", "I");
		strInput = strInput.Replace("�", "I");
		strInput = strInput.Replace("�", "I");
		strInput = strInput.Replace("�", "I");
		strInput = strInput.Replace("�", "o");
		strInput = strInput.Replace("�", "o");
		strInput = strInput.Replace("�", "o");
		strInput = strInput.Replace("�", "o'");
		strInput = strInput.Replace("�", "o'");
		strInput = strInput.Replace("�", "O");
		strInput = strInput.Replace("�", "O");
		strInput = strInput.Replace("�", "O");
		strInput = strInput.Replace("�", "O");
		strInput = strInput.Replace("�", "O");
		strInput = strInput.Replace("�", "ss");
		strInput = strInput.Replace("�", "u");
		strInput = strInput.Replace("�", "u");
		strInput = strInput.Replace("�", "u'");
		strInput = strInput.Replace("�", "u'");
		strInput = strInput.Replace("�", "U");
		strInput = strInput.Replace("�", "U");
		strInput = strInput.Replace("�", "U");
		strInput = strInput.Replace("�", "U");
		strInput = strInput.Replace("�", "n");
		strInput = strInput.Replace("�", "Y");
		strInput = strInput.Replace("�", "y");
		return strInput;
	}
	
	/// <summary>
	/// Clean up file names to ascii
	/// </summary>
	public static string SanitizeProductName(string strInput)
	{
		strInput = strInput.Replace(" ", "e");
		RegularExpressions.Regex objRegex = new RegularExpressions.Regex("[^A-Za-z 0-9 \\.,\\?'\"!@#\\$%\\^&\\*\\(\\)-_=\\+;:<>\\/\\\\\\|\\}\\{\\[\\]`~]*�");
		return System.Convert.ToString(objRegex.Replace(strInput, ""));
	}
	
	/// <summary>
	/// Fix decimal values to remove trailing zeroes
	/// </summary>
	public static decimal FixDecimal(decimal numInput)
	{
		return (decimal) ((double) ((int) (numInput * 10000)) / 10000);
	}
	
	/// <summary>
	/// Clean URL
	/// </summary>
	public static string CleanURL(string strInput)
	{
		//will need to be replaced by a regex
		//strInput = strInput.Replace("""", "")
		//strInput = strInput.Replace("*", "")
		strInput = strInput.Replace("\r\n", "");
		strInput = strInput.Replace(Constants.vbCr, "");
		strInput = strInput.Replace(System.Convert.ToString("\t"), "");
		strInput = strInput.Replace("//", "/");
		strInput = strInput.Replace("�", "o");
		strInput = strInput.Replace("./", "_/");
		
		//Return strInput.Replace("&", "-And-")
		
		string[] strArrayChars = "\",',\\,:,*,?,|,(,),%,#,&,>,<".Split(',');
		for (numCount = 0; numCount <= (strArrayChars.Length - 1); numCount++)
		{
			strInput = System.Convert.ToString(strInput.Replace(strArrayChars[numCount], (strArrayChars[numCount] == "%") ? "pc" : ""));
		}
		return strInput;
	}
	
	/// <summary>
	/// This truncates descriptions to specified length if too long, and appends "..."
	/// </summary>
	public static string TruncateDescription(object strDescription, int numCharacters)
	{
		strDescription = StripHTML(strDescription); //uses another Kartris function
		
		if (Strings.Len(strDescription) > numCharacters)
		{
			return Strings.Left(System.Convert.ToString(strDescription), numCharacters) +"...";
		}
		
		return System.Convert.ToString( strDescription);
	}
	
	/// <summary>
	/// This truncates display values on back end and adds "..."
	/// </summary>
	public static string TruncateDescriptionBack(object strDescription, int numCharacters)
	{
		if (!string.IsNullOrEmpty(System.Convert.ToString(CkartrisDataManipulation.FixNullFromDB(strDescription))))
		{
			if (Strings.Len(strDescription) > numCharacters)
			{
				return Strings.Left(System.Convert.ToString(strDescription), numCharacters) +"...";
			}
		}
		else
		{
			return "";
		}
		return System.Convert.ToString( strDescription);
	}
	
	/// <summary>
	/// Strips HTML tags from text
	/// </summary>
	public static string StripHTML(object strInput)
	{
		string strOutput = "";
		try
		{
			strOutput = System.Convert.ToString(RegularExpressions.Regex.Replace(strInput.ToString(), "<(.|\\n)*?>", string.Empty));
			
			//This bit trims extra chars, probably because this function
			//was used incorrectly in some places to sanitize URLs - use CleanURL for that.
			//Dim strArrayChars() As String = Split("',/,\,:,*,?,|,(,),%", ",")
			//For numCount = 0 To UBound(strArrayChars)
			//strOutput = Replace(strOutput, strArrayChars(numCount), IIf(strArrayChars(numCount) = "%", "pc", ""))
			//Next
			
			//This removes half-formed tags with an opening < but no close tag
			if (strOutput.Contains("<"))
			{
				strOutput = strOutput.Substring(0, strOutput.IndexOf("<") + 0);
			}
		}
		catch
		{
			strOutput = "";
		}
		
		return strOutput;
	}
	
	/// <summary>
	/// Format date strings
	/// </summary>
	public static string FormatDate(DateTime datDate, char chrType, byte numLanguageID)
	{
		string strOutput = "-";
		DateTime datOld = new DateTime(1982, 1, 1);
		if (datDate < datOld)
		{
			strOutput = "-";
		}
		else
		{
			string strFormat = System.Convert.ToString(LanguagesBLL.GetDateFormat(numLanguageID, chrType));
			try
			{
				strOutput = System.Convert.ToString((System.Convert.ToDateTime(CkartrisDataManipulation.FixNullFromDB(datDate))).ToString(strFormat));
				CultureInfo.CreateSpecificCulture(CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
			}
			catch (Exception)
			{
				strOutput = Strings.Format(CkartrisDataManipulation.FixNullFromDB(datDate), strFormat);
			}
		}
		
		return strOutput;
	}
	
	/// <summary>
	/// Format backward date strings
	/// </summary>
	public static string FormatBackwardsDate(DateTime datDate)
	{
		string strOutput = "-";
		DateTime datOld = new DateTime(1982, 1, 1);
		if (datDate < datOld)
		{
			strOutput = "-";
		}
		else
		{
			string strFormat = "yyyy/MM/dd";
			strOutput = Strings.Format(CkartrisDataManipulation.FixNullFromDB(datDate), strFormat);
		}
		
		return strOutput;
	}
	
	/// <summary>
	/// Returns Kartris Shop Date
	/// </summary>
	/// <returns>Now + timeoffset config</returns>
	/// <remarks></remarks>
	public static DateTime NowOffset()
	{
		return System.DateTime.Now.AddHours(System.Convert.ToInt32(GetKartConfig("general.timeoffset")));
	}
	
	public static object RemoveXSS(object objText)
	{
		if (objText != null && ReferenceEquals(objText.GetType(), Type.GetType("System.String")))
		{
			objText = objText.Replace("<", "&lt;"); //<
			objText = objText.Replace(">", "&gt;"); //>
			objText = objText.Replace("'", "&apos;"); //apostophe
			objText = objText.Replace("\"", "&#x22;"); // "
			objText = objText.Replace(")", "&#x29"); // )
			objText = objText.Replace("(", "&#x28"); // (
		}
		return objText;
	}
	
}

/// <summary>
/// Various functions for manipulating and fixing data
/// </summary>
public sealed class CkartrisDataManipulation
{
	
	/// <summary>
	/// The in-built ASP.NET mod function really isn't that great to use. Because of how
	/// it treats Double types, you can end up with the odd very small decimal portions of
	/// whole numbers which result in comparisons being wrong. This function fixes the input
	/// values and output to Decimal type which doesn't suffer this problem.
	/// </summary>
	public static decimal SafeModulus(decimal numQuantity, string strUnitSize)
	{
		numQuantity = System.Convert.ToDecimal(Math.Round(numQuantity, 4));
		decimal numUnitSize = System.Convert.ToDecimal(Math.Round(decimal.Parse(strUnitSize), 4));
		
		return numQuantity % numUnitSize;
	}
	
	/// <summary>
	/// We can fix a number to be between certain ranges (e.g. zero and 100), if non-numeric, set as lower bound
	/// </summary>
	public static double FixNumber(object numInput, double numLowerBound, double numUpperBound)
	{
		double numOutput = 0;
		try
		{
			numOutput = System.Convert.ToDouble(Convert.ToDouble(numInput));
		}
		catch (Exception)
		{
			numOutput = 0;
		}
		if (numOutput > numUpperBound)
		{
			numOutput = numUpperBound;
		}
		if (numOutput < numLowerBound)
		{
			numOutput = numLowerBound;
		}
		return numOutput;
	}
	
	/// <summary>
	/// Check number is a number
	/// We use this to sanitize submissions in
	/// the front end of number values, especially
	/// those from querystrings, to avoid untrapped
	/// errors.
	/// </summary>
	public static long NumSafe(string strInput, long numDefault)
	{
		long numOutput = numDefault;
		try
		{
			//Attempt to convert input value to
			//number
			numOutput = long.Parse(strInput);
		}
		catch
		{
			numOutput = numDefault;
		}
		return numOutput;
	}
	
	/// <summary>
	/// If we expect strings but might get a Null, use this function on the value first
	/// </summary>
	public static string FixNullToString(object objInput)
	{
		string strOutput = "";
		try
		{
			strOutput = System.Convert.ToString(Convert.ToString(objInput));
		}
		catch (Exception)
		{
			strOutput = "";
		}
		return strOutput;
	}
	
	/// <summary>
	/// If you want to INSERT or UPDATE the db, use this function which returns NULL for non-entered data fields.
	/// </summary>
	/// <param name="objInput">The data field that you want to check.</param>
	/// <param name="chrType">The type of the data field.(s->String, i->Integer, b->Boolean, d->Double, and c->Char "</param>
	/// <remarks>By Mohammad</remarks>
	public static object FixNullToDB(object objInput, char chrType)
	{
		switch (chrType)
		{
			case 's': //String
				if (ReferenceEquals(objInput, null) || (string) objInput == "")
				{
					return DBNull.Value;
				}
				else
				{
					return System.Convert.ToString(objInput);
				}
				break;
			case 'i': //Integer
				if (ReferenceEquals(objInput, null) || (int) objInput == 0)
				{
					//If objInput = 0 OrElse objInput Is Nothing Then
					return DBNull.Value;
				}
				else
				{
					return System.Convert.ToInt32(objInput);
				}
				break;
			case 'b': //Boolean
				if (ReferenceEquals(objInput, null))
				{
					return false;
				}
				else
				{
					return System.Convert.ToBoolean(objInput);
				}
				break;
			case 'd': //Double
				if (ReferenceEquals(objInput, null))
				{
					//If objInput = 0 OrElse objInput Is Nothing Then
					return DBNull.Value;
				}
				else
				{
					return System.Convert.ToDouble(objInput);
				}
				break;
			case 'c': //Char
				if ((string) objInput == "" || ReferenceEquals(objInput, null))
				{
					return DBNull.Value;
				}
				else
				{
					return System.Convert.ToChar(objInput);
				}
				break;
			case 'g': //Single
				if (ReferenceEquals(objInput, null) || objInput == 0.0)
				{
					//If objInput = 0 OrElse objInput Is Nothing Then
					return DBNull.Value;
				}
				else
				{
					return System.Convert.ToSingle(objInput);
				}
				break;
			case 'z': //decimal
				if (ReferenceEquals(objInput, null))
				{
					//If objInput = 0 OrElse objInput Is Nothing Then
					return DBNull.Value;
				}
				else
				{
					return System.Convert.ToDecimal(objInput);
				}
				break;
			default:
				break;
				
		}
		return objInput;
	}
	
	/// <summary>
	/// If you are reading from the db, use this function which returns Nothing(the default value of the passed param.)
	///  instead of having a DBNULL which in most cases, generates a run-time error.
	/// </summary>
	/// <param name="objInput">The datacolumn's value that you want to check</param>
	/// <remarks>By Mohammad</remarks>
	public static object FixNullFromDB(object objInput)
	{
		if (ReferenceEquals(objInput, DBNull.Value))
		{
			return null;
		}
		return objInput;
	}
	
	/// <summary>
	/// Handles the decimal point to be saved in the db as (dot)
	/// </summary>
	public static decimal HandleDecimalValues(string strValue)
	{
		return decimal.Parse( strValue.Replace(",", "."));
	}
	
	/// <summary>
	/// Handles the decimal point display in the backend
	/// </summary>
	public static string _HandleDecimalValues(string strValue)
	{
		string strBackUserCulture = System.Convert.ToString(LanguagesBLL.GetCultureByLanguageID_s(System.Convert.ToInt32(Current.Session("_LANG"))));
		CultureInfo c = new CultureInfo(strBackUserCulture);
		return strValue.Replace(".", c.NumberFormat.CurrencyDecimalSeparator);
	}
	
	/// <summary>
	/// Handles the decimal point in strings
	/// </summary>
	public static string HandleDecimalValuesString(string strValue)
	{
		strValue = strValue.Replace(",", ".");
		if (!strValue.Contains("."))
		{
			strValue += ".00";
		}
		else
		{
			//Maybe has one char after dot
			string[] aryText = strValue.Split('.');
			if (aryText[1].Length == 1)
			{
				strValue += "0";
			}
		}
		return strValue;
	}
	
	/// <summary>
	/// Get product ID from querystring
	/// </summary>
	public static int _GetProductID()
	{
		int numProductID = 0;
		try
		{
			numProductID = System.Convert.ToInt32(HttpContext.Current.Request.QueryString("ProductID"));
		}
		catch (Exception)
		{
			numProductID = 0;
		}
		return numProductID;
	}
	
	/// <summary>
	/// Get category ID from querystring
	/// </summary>
	public static int _GetCategoryID()
	{
		int numCategoryID = 0;
		try
		{
			numCategoryID = System.Convert.ToInt32(HttpContext.Current.Request.QueryString("CategoryID"));
		}
		catch (Exception)
		{
			numCategoryID = 0;
		}
		return numCategoryID;
	}
	
	/// <summary>
	/// Get site ID from querystring
	/// </summary>
	public static int _GetSiteID()
	{
		int numSiteID = 0;
		try
		{
			numSiteID = System.Convert.ToInt32(HttpContext.Current.Request.QueryString("SiteID"));
		}
		catch (Exception)
		{
			numSiteID = 0;
		}
		return numSiteID;
	}
	/// <summary>
	/// Get parent categories from QueryString
	/// </summary>
	public static string _GetParentCategory()
	{
		string numParentCategoryIDs = "";
		try
		{
			numParentCategoryIDs = System.Convert.ToString(HttpContext.Current.Request.QueryString("strParent"));
		}
		catch (Exception)
		{
			numParentCategoryIDs = "";
		}
		return numParentCategoryIDs;
	}
	
	/// <summary>
	/// Get a QueryString value from the current URL. Can be used to extract IDs and other numeric querystring values.
	/// ** Integers only so don't use with querystrings that have decimal points!
	/// </summary>
	/// <param name="strQS"> The Query String parameter name you want to get.(e.g. ProductID,CategoryID)</param>
	/// <remarks>Is this required? Ok. =) By Medz </remarks>
	public static int GetIntegerQS(string strQS)
	{
		int intID = 0;
		try
		{
			intID = System.Convert.ToInt32(HttpContext.Current.Request.QueryString(strQS));
		}
		catch (Exception)
		{
			intID = 0;
		}
		return intID;
	}
	
	/// <summary>
	/// Check it is a valid user agent
	/// </summary>
	public static bool IsValidUserAgent()
	{
		try
		{
			string strTemp = System.Convert.ToString(ConfigurationManager.AppSettings("ExcludedUserAgents").ToString());
			strTemp = strTemp.ToUpper();
			
			string[] strExcludedUserAgents = new string[] {""};
			strExcludedUserAgents = strTemp.Split(",".ToCharArray());
			
			string strUserAgent = "";
			strUserAgent = System.Convert.ToString(HttpContext.Current.Request.ServerVariables("HTTP_USER_AGENT").ToString());
			strUserAgent = strUserAgent.ToUpper();
			
			int i = 0;
			for (i = 0; i <= strExcludedUserAgents.Length - 1; i++)
			{
				if (strUserAgent.Contains(strExcludedUserAgents[i]))
				{
					return false;
				}
			}
			return true;
		}
		catch (Exception)
		{
			return false;
		}
	}
	
	/// <summary>
	/// Create string of parameters for stored procedures
	/// </summary>
	public static string CreateQuery(SqlCommand sqlCmd)
	{
		StringBuilder sbdParameters = new StringBuilder("");
		try
		{
			sbdParameters.Append(sqlCmd.CommandText);
			foreach (SqlParameter sqlParam in sqlCmd.Parameters)
			{
				sbdParameters.Append("##" + sqlParam.ParameterName + "=" + FixNullToString(sqlParam.Value) + ",");
			}
			if (sbdParameters.ToString().EndsWith(","))
			{
				return sbdParameters.ToString().TrimEnd(",");
			}
		}
		catch (Exception)
		{
			return "";
		}
		
		return sbdParameters.ToString();
	}
	
	/// <summary>
	/// reads the category hierarchy again from the db
	/// </summary>
	/// <remarks></remarks>
	public static void RefreshSiteMap()
	{
		_CategorySiteMapProvider _CatSiteMap = (_CategorySiteMapProvider) (SiteMap.Providers("_CategorySiteMapProvider"));
		_CatSiteMap.ResetSiteMap();
		CategorySiteMapProvider CatSiteMap = (CategorySiteMapProvider) SiteMap.Provider;
		CatSiteMap.RefreshSiteMap();
	}
	
	/// <summary>
	/// Fetch the specified HTML Email Template from the currently used Skin
	/// </summary>
	/// <param name="strTemplateType"></param>
	/// <param name="intLanguageID">Optional. Will get ID of the currently selected Language if not specified.</param>
	/// <returns>returns a string variable that contains the content of the specified HTML template</returns>
	/// <remarks></remarks>
	public static string RetrieveHTMLEmailTemplate(string strTemplateType, short intLanguageID)
	{
		string strEmailTemplateText = "";
		string strCulture = "";
		string strLanguage = "";
		string strSkinFolder = "";
		string strEmailTemplatePath_Culture = "";
		string strEmailTemplatePath_Language = "";
		
		//if language ID is not passed then retrieve the ID from session
		if (intLanguageID == 0)
		{
			intLanguageID = System.Convert.ToInt16(CkartrisBLL.GetLanguageIDfromSession);
		}
		
		//retrieve the culture, and derive the language code too
		strCulture = System.Convert.ToString(LanguagesBLL.GetCultureByLanguageID_s(intLanguageID)); //full culture, e.g. EN-GB
		strLanguage = strCulture.Substring(0, 2); //basic language part, e.g. EN
		
		//retrieve the currently used skin
		strSkinFolder = System.Convert.ToString(LanguagesBLL.GetTheme(intLanguageID));
		if (string.IsNullOrEmpty(strSkinFolder))
		{
			strSkinFolder = "Kartris";
		}
		
		//This is where a culture-specific template would be found (e.g. EN-GB)
		strEmailTemplatePath_Culture = Current.Server.MapPath("~/Skins/" + strSkinFolder + "/Templates/Email_" &
		strTemplateType (+ "_" + strCulture +".html"));
		
		//This is where a general language template would be found (e.g. EN)
		strEmailTemplatePath_Language = Current.Server.MapPath("~/Skins/" + strSkinFolder + "/Templates/Email_" &
		strTemplateType (+ "_" + strLanguage +".html"));
		
		
		//We first look for exact culture template, if not found,
		//we look for a broader one for just the language
		if (File.Exists(strEmailTemplatePath_Culture))
		{
			
			//We have an exact culture match for template, e.g. EN-GB
			//This means you can have different templates for US and British
			//English, Portuguese from Portugal or Brazil, etc.
			FileStream objFileStream = new FileStream(strEmailTemplatePath_Culture, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
			StreamReader objStreamReader = new StreamReader(objFileStream);
			strEmailTemplateText = System.Convert.ToString(objStreamReader.ReadToEnd());
			objFileStream.Close();
			
		}
		else if (File.Exists(strEmailTemplatePath_Language))
		{
			
			//We have an basic culture match for template, e.g. EN
			//This means we can use a single template for all
			//versions of English; most stores will probably want
			//to go this route for simplicity
			FileStream objFileStream = new FileStream(strEmailTemplatePath_Language, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
			StreamReader objStreamReader = new StreamReader(objFileStream);
			strEmailTemplateText = System.Convert.ToString(objStreamReader.ReadToEnd());
			objFileStream.Close();
			
		}
		
		//strip out HTML comments in the template as we don't need them in the emails
		strEmailTemplateText = System.Convert.ToString(Regex.Replace(strEmailTemplateText, "<!--(.|\\s)*?-->", string.Empty));
		
		return strEmailTemplateText;
	}
	
	/// <summary>
	/// Send Email using Mailkit
	/// </summary>
	/// <param name="strFrom">From address</param>
	/// <param name="strTo">To address</param>
	/// <param name="strSubject">Subject line</param>
	/// <param name="strBody">Body text</param>
	/// <param name="strReplyTo">Reply To address (OPTIONAL)</param>
	/// <param name="strFromName">From name (OPTIONAL)</param>
	/// <param name="sendEncoding">Encoding (OPTIONAL)</param>
	/// <param name="strAttachment">Path to attachment(OPTIONAL)</param>
	/// <param name="blnSendAsHTML">Boolean to send as HTML (OPTIONAL)</param>
	/// <param name="sendPriority">Priority (OPTIONAL)</param>
	/// <param name="objBCCAddress">BCC address (OPTIONAL)</param>
	/// <param name="objAdditionalToAddresses">Additional 'To' addresses (OPTIONAL)</param>
	/// <returns>boolean, true if function runs without error</returns>
	/// <remarks></remarks>
	public static object SendEmail()
	{
		string strReplyTo = "";
		string strFromName  = "";
		Encoding sendEncoding  = null;
		string strAttachment = "";
		bool blnSendAsHTML  = false;
		MailPriority sendPriority  = MailPriority.Normal;
		MailAddressCollection objBCCAddress  = null;
		MailAddressCollection objAdditionalToAddresses  = null;
		
		//Belt and braces, let's ensure we don't mail the guest
		//email addresses
		strTo = UsersBLL.CleanGuestEmailUsername(strTo);
		
		try
		{
			if (Strings.LCase(System.Convert.ToString(GetKartConfig("general.email.method"))) == "write")
			{
				
				//Write method - use javascript alert box to display email
				Page pagCurrentPage = HttpContext.Current.Handler;
				if (pagCurrentPage != null)
				{
					string strBCCString = "";
					if (objBCCAddress != null)
					{
						strBCCString = "BCC: ";
						foreach (MailAddress objItem in objBCCAddress)
						{
							if (strBCCString == "BCC: ")
							{
								strBCCString += System.Convert.ToString(objItem.Address);
							}
							else
							{
								strBCCString += "; " + objItem.Address;
							}
						}
						strBCCString += "\\n";
					}
					
					//Do replacements so HTML will display properly in the JS alert popup
					if (blnSendAsHTML)
					{
						strBody = "*HTML AND BODY TAGS are stripped when using WRITE emailmethod*" + "\r\n" + CkartrisBLL.ExtractHTMLBodyContents(strBody);
					}
					strBody = strBody.Replace("\\", "\\\\");
					strBody = strBody.Replace("<", "\\<");
					strBody = strBody.Replace(">", "\\>");
					strBody = strBody.Replace("\"", "\\\"");
					strBody = strBody.Replace("'", "\\'");
					strBody = strBody.Replace("&", "\\&");
					strBody = strBody.Replace("\r\n", "\\n");
					strBody = strBody.Replace(Constants.vbLf, "\\n");
					string strBodyText = string.Format("alert('FROM: " + strFrom + "\\nTO: " + strTo + "\\n" + strBCCString + "SUBJECT: " + strSubject + "\\n\\nBODY:\\n {0}');", strBody);
					ScriptManager.RegisterStartupScript(pagCurrentPage, pagCurrentPage.GetType, "WriteEmail", strBodyText, true);
					
					//CurrentPage.ClientScript.RegisterStartupScript(CurrentPage.GetType, "WriteEmail", strBodyText, True)
				}
			}
			else
			{
				if (Strings.LCase(System.Convert.ToString(GetKartConfig("general.email.method"))) != "off")
				{
					
					//Send the Email via MailKit & MimeKit
					MailboxAddress objEmailFrom = null;
					if (!string.IsNullOrEmpty(System.Convert.ToString(strFromName)))
					{
						objEmailFrom = new MailboxAddress(strFromName, strFrom);
					}
					else
					{
						objEmailFrom = MailboxAddress.Parse(strFrom);
					}
					MailboxAddress objEmailTo = MailboxAddress.Parse(strTo);
					MimeMessage objMailMessage = new MimeMessage();
					objMailMessage.From.Add(objEmailFrom);
					objMailMessage.To.Add(objEmailTo);
					if (!string.IsNullOrEmpty(System.Convert.ToString(strReplyTo)))
					{
						objMailMessage.ReplyTo.Add(MailboxAddress.Parse(strReplyTo));
					}
					if (objAdditionalToAddresses != null)
					{
						foreach (MailAddress objItem in objAdditionalToAddresses)
						{
							objMailMessage.To.Add(MailboxAddress.Parse(objItem.Address));
						}
					}
					if (objBCCAddress != null)
					{
						foreach (MailAddress objItem in objBCCAddress)
						{
							objMailMessage.Bcc.Add(MailboxAddress.Parse(objItem.Address));
						}
					}
					objMailMessage.Subject = strSubject;
					objMailMessage.Priority = 1; //normal (0=non urgent, 1 normal, 2 urgent)
					BodyBuilder builder = new BodyBuilder();
					builder.TextBody = strBody; //plain
					if (blnSendAsHTML)
					{
						builder.HtmlBody = strBody; //html
					}
					if (!string.IsNullOrEmpty(System.Convert.ToString(strAttachment)))
					{
						foreach (string strAttachmentPath in strAttachment.Split(","))
						{
							builder.Attachments.Add(strAttachmentPath);
						}
					}
					objMailMessage.Body = builder.ToMessageBody();
					object Sso = Security.SecureSocketOptions.None;
					switch (System.Convert.ToInt32(GetKartConfig("general.email.smtpportnumber")))
					{
						case 25:
						case 587:
						case 110:
						case 143:
							Sso = Security.SecureSocketOptions.None;
							break;
						case 465:
						case 995:
						case 993:
							Sso = Security.SecureSocketOptions.SslOnConnect;
							break;
						default:
							Sso = Security.SecureSocketOptions.None;
							break;
					}
					MailKit.Net.Smtp.SmtpClient objMail = new MailKit.Net.Smtp.SmtpClient();
					objMail.Connect(GetKartConfig("general.email.mailserver"), System.Convert.ToInt32(GetKartConfig("general.email.smtpportnumber")), Sso);
					string strUserName = System.Convert.ToString(GetKartConfig("general.email.smtpauthusername"));
					if (!string.IsNullOrEmpty(strUserName))
					{
						objMail.Authenticate(strUserName, GetKartConfig("general.email.smtpauthpassword"));
					}
					objMail.Send(objMailMessage);
					objMail.Disconnect(true);
				}
			}
			return true;
		}
		catch (Exception ex)
		{
			CkartrisFormatErrors.ReportHandledError(ex, System.Reflection.MethodBase.GetCurrentMethod);
			return false;
		}
		return true;
	}
	
	/// <summary>
	/// Stream a file download
	/// </summary>
	public static void DownloadFile(string strFileName)
	{
		string strFilePath = System.Convert.ToString(Current.Server.MapPath(GetKartConfig("general.uploadfolder") + strFileName));
		System.IO.FileInfo filTarget = new System.IO.FileInfo(strFilePath);
		if (filTarget.Exists)
		{
			Current.Response.Clear();
			Current.Response.AddHeader("Content-Disposition", "attachment; filename=" + strFileName.Replace(" ", "_"));
			Current.Response.AddHeader("Content-Length", filTarget.Length.ToString());
			Current.Response.ContentType = "application/octet-stream";
			Current.Response.TransmitFile(strFilePath);
			Current.Response.End();
		}
	}
	
	/// <summary>
	/// Remove download files
	/// </summary>
	public static void RemoveDownloadFiles(string strFiles)
	{
		if (string.IsNullOrEmpty(strFiles) || strFiles == "##")
		{
			return ;
		}
		string[] arrFiles = strFiles.Split("##".ToCharArray());
		string strUploadPath = System.Convert.ToString(GetKartConfig("general.uploadfolder"));
		for (int i = 0; i <= arrFiles.Length - 1; i++)
		{
			try
			{
				if (File.Exists(Current.Server.MapPath(strUploadPath + arrFiles[i])))
				{
					File.SetAttributes(Current.Server.MapPath(strUploadPath + arrFiles[i]), FileAttributes.Normal);
					File.Delete(Current.Server.MapPath(strUploadPath + arrFiles[i]));
				}
			}
			catch (Exception)
			{
			}
		}
	}
}


/// <summary>
/// Searching
/// </summary>
public sealed class CKartrisSearchManager
{
	/// <summary>
	/// Create a cookie to store search prefs
	/// </summary>
	public static void CreateSearchCookie()
	{
		if (ReferenceEquals(Current.Request.Cookies(HttpSecureCookie.GetCookieName("Search")), null))
		{
			
			HttpCookie cokSearch = new HttpCookie(HttpSecureCookie.GetCookieName("Search"));
			cokSearch.Values["exactPhrase"] = "";
			cokSearch.Values["searchtype"] = "";
			cokSearch.Values["keywords"] = "";
			cokSearch.Values["searchMethod"] = "";
			cokSearch.Values["minPrice"] = "";
			cokSearch.Values["maxPrice"] = "";
			
			Current.Response.Cookies.Add(cokSearch);
			
		}
	}
	
	/// <summary>
	/// Rewrite search cookie
	/// </summary>
	public static object UpdateSearchCookie()
	{
		string strSearchMethod  = null; int sngMinPrice  = -1; int sngMaxPrice  = -1;
		string strKeywords = strSearchedText + ",";
		strKeywords = ValidateSearchKeywords(strKeywords);
		
		if (!ReferenceEquals(Current.Request.Cookies(HttpSecureCookie.GetCookieName("Search")), null))
		{
			HttpCookie cokSearch = new HttpCookie(HttpSecureCookie.GetCookieName("Search"));
			cokSearch.Values["exactPhrase"] = Current.Server.UrlEncode(strSearchedText);
			cokSearch.Values["type"] = enumType.ToString();
			cokSearch.Values["keywords"] = strKeywords;
			cokSearch.Values["searchMethod"] = (string.IsNullOrEmpty(System.Convert.ToString(strSearchMethod))) ? (GetKartConfig("frontend.search.defaultmethod")) : (Strings.LCase(System.Convert.ToString(strSearchMethod)));
			cokSearch.Values["minPrice"] = sngMinPrice;
			cokSearch.Values["maxPrice"] = sngMaxPrice;
			
			Current.Response.Cookies.Add(cokSearch);
		}
		
		return strKeywords;
	}
	
	/// <summary>
	/// Exclude certain keywords from search
	/// </summary>
	/// <remarks>Each language can have its own set of excluded words, since we read them from language string</remarks>
	public static string ValidateSearchKeywords(string strKeywords)
	{
		
		string strInvalidKeywords = System.Convert.ToString(GetGlobalResourceObject("Kartris", "ContentText_ExcludedSearchKeywords"));
		StringBuilder sbdValidKeywords = new StringBuilder("");
		
		strKeywords = strKeywords.Replace(" ", ",");
		strKeywords = strKeywords.Replace(",,", ",");
		
		//Escape single quotes as they seem to throw
		//errors if not
		strKeywords = strKeywords.Replace("'", "''");
		
		string[] arrKeywords = new string[] {};
		arrKeywords = strKeywords.Split(',');
		
		string[] arrInvalidKeywords = new string[] {};
		arrInvalidKeywords = strInvalidKeywords.Split(',');
		
		for (int i = 0; i <= arrInvalidKeywords.GetUpperBound(0); i++)
		{
			for (int j = 0; j <= arrKeywords.GetUpperBound(0); j++)
			{
				if (arrKeywords[j].Length == 1)
				{
					arrKeywords[j] = "";
				}
				else
				{
					if (string.Compare(arrInvalidKeywords[i], arrKeywords[j], true) == 0)
					{
						arrKeywords[j] = "";
					}
				}
			}
		}
		
		for (int i = 0; i <= arrKeywords.GetUpperBound(0); i++)
		{
			if (arrKeywords[i] != "")
			{
				sbdValidKeywords.Append(arrKeywords[i]);
				sbdValidKeywords.Append(",");
			}
		}
		
		if (sbdValidKeywords.ToString().EndsWith(","))
		{
			//'//
			string strTemp = System.Convert.ToString(sbdValidKeywords.ToString().TrimEnd(","));
			sbdValidKeywords = new StringBuilder();
			sbdValidKeywords.Append(strTemp);
		}
		
		if (sbdValidKeywords.ToString().StartsWith(","))
		{
			sbdValidKeywords.Length = 0;
			sbdValidKeywords.Capacity = 0;
			sbdValidKeywords.Append(sbdValidKeywords.ToString().TrimStart(","));
		}
		
		
		if (GetKartConfig("") == "y")
		{
			sbdValidKeywords.Length = 0;
			sbdValidKeywords.Capacity = 0;
			sbdValidKeywords.Append(CreateAllPossibleKeywords(System.Convert.ToString(sbdValidKeywords.ToString())));
		}
		
		return sbdValidKeywords.ToString();
	}
	
	/// <summary>
	/// Create list of all possible keyword combinations to run search
	/// </summary>
	private static string CreateAllPossibleKeywords(string pstrKeywords)
	{
		string strKeywords = pstrKeywords;
		strKeywords = strKeywords.Replace(" ", ",");
		
		string[] arrKeywords = new string[] {};
		arrKeywords = strKeywords.Split(",".ToCharArray());
		
		StringBuilder sbdNewKeys = new StringBuilder("");
		StringBuilder sbdNewCombination = new StringBuilder("");
		
		for (int i = 0; i <= arrKeywords.GetUpperBound(0) - 1; i++)
		{
			
			for (int k = 0; k <= arrKeywords.GetUpperBound(0) - i - 1; k++)
			{
				sbdNewCombination.Length = 0;
				sbdNewCombination.Capacity = 0;
				sbdNewCombination.Append(arrKeywords[i]);
				for (int j = i + 1; j <= arrKeywords.GetUpperBound(0) - k; j++)
				{
					sbdNewCombination.Append(" ");
					sbdNewCombination.Append(arrKeywords[j]);
				}
				sbdNewKeys.Append(sbdNewCombination.ToString());
				sbdNewKeys.Append(",");
			}
			
		}
		
		string[,] arrNewKeys = new string[,];
		string[] arrDummyKeys = new string[] {};
		StringBuilder sbdToPrint = new StringBuilder("");
		if (sbdNewKeys.ToString().EndsWith(","))
		{
			sbdNewKeys = new StringBuilder(sbdNewKeys.ToString().TrimEnd(","));
		}
		arrDummyKeys = sbdNewKeys.ToString().Split(",");
		arrNewKeys = (string[,]) Microsoft.VisualBasic.CompilerServices.Utils.CopyArray((Array) arrNewKeys, new string[arrDummyKeys.Length, 2]);
		for (int i = 0; i <= arrDummyKeys.GetUpperBound(0); i++)
		{
			arrNewKeys[i, 0] = arrDummyKeys[i];
			arrNewKeys[i, 1] = arrDummyKeys[i].Length;
		}
		
		for (int i = 0; i <= arrNewKeys.GetUpperBound(0); i++)
		{
			for (int j = 0; j <= arrNewKeys.GetUpperBound(0) - 1; j++)
			{
				string[,] strTemp = new string[2, 2];
				if (System.Convert.ToInt32(arrNewKeys[j, 1]) < System.Convert.ToInt32(arrNewKeys[i, 1]))
				{
					strTemp[0, 0] = arrNewKeys[j, 0];
					strTemp[0, 1] = arrNewKeys[j, 1];
					
					arrNewKeys[j, 0] = arrNewKeys[i, 0];
					arrNewKeys[j, 1] = arrNewKeys[i, 1];
					
					arrNewKeys[i, 0] = strTemp[0, 0];
					arrNewKeys[i, 1] = strTemp[0, 1];
				}
			}
		}
		
		for (int i = 0; i <= arrNewKeys.GetUpperBound(0); i++)
		{
			sbdToPrint.Append(arrNewKeys[i, 0]);
			sbdToPrint.Append(",");
		}
		sbdToPrint.Append(strKeywords);
		
		return sbdToPrint.ToString();
	}
	
	public static object HighLightResultText()
	{
		string ByValstrKeywordsstring;
		strContent = CkartrisDisplayFunctions.StripHTML(strContent);
		if (string.IsNullOrEmpty(System.Convert.ToString(strContent)))
		{
			return strContent;
		}
		
		//We use a temp pre/suffix to avoid the text of these
		//getting replaced themselves. For example, a search
		//for 'class' would replace class with a span tag, but
		//then the class attribute of that would itself get
		//replaced. No one is going to search for "[" or "[[".
		string strTempPrefix = "[[[[[";
		string strTempSuffix = "]]]]]";
		
		string strPrefix = "<span class=\"highlight\">";
		string strSuffix = "</span>";
		
		StringBuilder sdbTextHighlight = new StringBuilder(""); // A string builder that will hold the new value for each keyword
		string[] strNewKeywords = new string[] {""}; // An array of strings that will contain the keywords
		strNewKeywords = strKeywords.Split(",");
		
		int numStartingIndex = 0;
		int numKeywordLength = 0;
		
		for (int i = 0; i <= strNewKeywords.Length - 1; i++)
		{
			numStartingIndex = 0;
			numKeywordLength = strNewKeywords[i].Length;
			
			while (numStartingIndex >= 0 && numStartingIndex < strContent.Length)
			{
				numStartingIndex = System.Convert.ToInt32(strContent.IndexOf(strNewKeywords[i], numStartingIndex, StringComparison.OrdinalIgnoreCase));
				if (numStartingIndex != -1)
				{
					sdbTextHighlight.Append(strTempPrefix);
					sdbTextHighlight.Append(strContent.Substring(numStartingIndex, numKeywordLength));
					sdbTextHighlight.Append(strTempSuffix);
					strContent = Strings.Left(System.Convert.ToString(strContent), numStartingIndex) + sdbTextHighlight.ToString() &
					Strings.Mid(System.Convert.ToString(strContent), numStartingIndex + numKeywordLength + 1);
					numStartingIndex += System.Convert.ToInt32(sdbTextHighlight.Length);
					sdbTextHighlight.Length = 0;
					sdbTextHighlight.Capacity = 0; //Clear the content of the string builder to be used again
				}
			}
		}
		
		//replace our highlight placeholder text [[[[[ with the
		//actual HTML tags (span) we want to use.
		strContent = Strings.Replace(System.Convert.ToString(strContent), strTempPrefix, strPrefix, 1, -1, (Microsoft.VisualBasic.CompareMethod) 0);
		strContent = Strings.Replace(System.Convert.ToString(strContent), strTempSuffix, strSuffix, 1, -1, (Microsoft.VisualBasic.CompareMethod) 0);
		
		return strContent;
	}
	
}
/// <summary>
/// Useful class to easily use the BLL Classes' Functionality
/// </summary>
public sealed class CkartrisBLL
{
	
	/// <summary>
	///  Get the currently selected/active language id from session
	/// </summary>
	/// <remarks>returns the default language if we there's no session yet</remarks>
	public static short GetLanguageIDfromSession(string strWhere)
	{
		string strLanguageSessionName = "";
		if (strWhere == "f")
		{
			strLanguageSessionName = "LANG";
		}
		else
		{
			strLanguageSessionName = "_LANG";
		}
		if (ReferenceEquals(HttpContext.Current.Session, null))
		{
			return System.Convert.ToInt16(GetKartConfig("frontend.languages.default"));
		}
		else if (string.IsNullOrEmpty(System.Convert.ToString(HttpContext.Current.Session.Item(strLanguageSessionName))))
		{
			return System.Convert.ToInt16(GetKartConfig("frontend.languages.default"));
		}
		else
		{
			return (short) (System.Convert.ToInt32(HttpContext.Current.Session.Item(strLanguageSessionName)));
		}
	}
	
	/// <summary>
	/// Try to refresh all the cache that Kartris uses.
	/// </summary>
	/// <returns></returns>
	/// <remarks></remarks>
	public static bool RefreshKartrisCache()
	{
		//Clear caches we set programmatically
		try
		{
			RefreshCache();
			RefreshCurrencyCache();
			LanguagesBLL.GetLanguages(true); //Refresh cache for the front end dropdown
			RefreshLanguagesCache();
			RefreshLETypesFieldsCache();
			RefreshTopListProductsCache();
			RefreshNewestProductsCache();
			RefreshFeaturedProductsCache();
			RefreshCustomerGroupsCache();
			RefreshSuppliersCache();
			RefreshTaxRateCache();
			RefreshSiteNewsCache();
			CategorySiteMapProvider CatSiteMap = (CategorySiteMapProvider) SiteMap.Provider;
			CatSiteMap.RefreshSiteMap();
			LanguageStringProviders.Refresh();
			LoadSkinConfigToCache();
			foreach (DictionaryEntry dicEntry in HttpContext.Current.Cache)
			{
				HttpContext.Current.Cache.Remove((string) dicEntry.Key);
			}
		}
		catch (Exception)
		{
			return false;
		}
		return true;
	}
	
	/// <summary>
	/// Restarts the whole Kartris Application. Requires either Full Trust (HttpRuntime.UnloadAppDomain) or Write access to web.config.
	/// </summary>
	public static bool RestartKartrisApplication()
	{
		try
		{
			HttpRuntime.UnloadAppDomain();
		}
		catch
		{
			//unloading the appdomain failed - possibly because we're not running on full trust
			//try to modify the lastmodified attribute of the web.config file instead to trigger application restart.
			try
			{
				string ConfigPath = HttpContext.Current.Request.PhysicalApplicationPath + "\\web.config";
				System.IO.File.SetLastWriteTimeUtc(ConfigPath, DateTime.UtcNow);
			}
			catch
			{
				return false;
			}
			
		}
		return true;
	}
	
	/// <summary>
	/// Recycles the app pool for the application; we can try to trap issues where things go
	/// wrong such as where there are SQL timeouts which can result in the app pool crashing
	/// due to rapid fail protection, or issues like the currency cache code returning
	/// </summary>
	public static bool RecycleAppPool()
	{
		//Try
		//    Using iisManager As New ServerManager()
		//        Dim colSites As SiteCollection = iisManager.Sites
		//        For Each strSiteName As Site In colSites
		//            If strSiteName.Name = System.Web.Hosting.HostingEnvironment.ApplicationHost.GetSiteName() Then
		//                iisManager.ApplicationPools(strSiteName.Applications("/").ApplicationPoolName).Recycle()
		//                Return True
		//            End If
		//        Next
		//        Return False
		//    End Using
		//Catch ex As Exception
		//    'maybe don't have right permissions
		//    Return False
		//End Try
		return false;
	}
	
	
	/// <summary>
	/// Get the WebShopURL from config settings
	/// </summary>
	public static string WebShopURL()
	{
		try
		{
			//Check whether we need SSL or not.
			//Make sure we lowercase the entered domain as
			//otherwise this check is case sensitive, which
			//we don't want.
			if (GetKartConfig("general.security.ssl") == "e") 
			{
				
				return Strings.Replace(System.Convert.ToString(GetKartConfig("general.webshopurl").ToLower()), "http://", "https://", 1, -1, (Microsoft.VisualBasic.CompareMethod) 0);
				
			}
		}
			catch (Exception)
			{
			}
			return GetKartConfig("general.webshopurl");
		}
		
		/// <summary>
		/// Return the webshop URL but with http
		/// regardless of what page you're on
		/// </summary>
		public static string WebShopURLhttp()
		{
			return GetKartConfig("general.webshopurl").ToLower();
		}
		
		/// <summary>
		/// Return the webshop URL but with http
		/// regardless of what page you're on
		/// </summary>
		public static string WebShopURLhttps()
		{
			return Strings.Replace(System.Convert.ToString(GetKartConfig("general.webshopurl").ToLower()), "http://", "https://", 1, -1, (Microsoft.VisualBasic.CompareMethod) 0);
		}
		
		/// <summary>
		/// Get the WebShop folder from config settings
		/// </summary>
		public static string WebShopFolder()
		{
			return GetKartConfig("general.webshopfolder");
		}
		
		/// <summary>
		/// Find Skin
		/// We use the term 'Theme' for legacy reasons.
		/// (moved here from pagebaseclass as useful in places,
		/// especially for linking to images such as 'not available')
		/// </summary>
		public static string SkinMaster(System.Web.UI.Page objPage, int numLanguage)
		{
			//We used a theme specified in the language record, if none
			//then use the default 'Kartris' one.
			string strSkinFolder = System.Convert.ToString(LanguagesBLL.GetTheme(numLanguage));
			string strMasterPage = System.Convert.ToString(LanguagesBLL.GetMasterPage(numLanguage));
			
			//If skin not set, use default
			if (strSkinFolder == "")
			{
				strSkinFolder = "Kartris";
			}
			
			if (objPage.MasterPageFile.ToLower().Contains("skins/kartris/template.master"))
			{
				//The .aspx page is set to use the defaults,
				//so we assume we either use skin specified in
				//back end, or the default.
				if (!string.IsNullOrEmpty(strMasterPage))
				{
					//Have a master page specified in language
					//settings, use this
					return "~/Skins/" + strSkinFolder + "/" + strMasterPage;
				}
				else
				{
					//Use default skin
					return "~/Skins/" + strSkinFolder + "/Template.master";
				}
			}
			else
			{
				//Do nothing - we use the master page specified
				//in the .aspx page itself
				return "";
			}
			
		}
		
		/// <summary>
		/// Find Skin in the Skins.Config file
		/// We use the term 'Theme' for legacy reasons.
		/// (moved here from pagebaseclass as useful in places,
		/// especially for linking to images such as 'not available')
		/// </summary>
		public static object SkinMasterConfig()
		{
			string ByValintCustomerIDint, ByValintCustomerGroupIDint, ByValstrScriptNamestring;
			string strSkinFolder = "";
			
			try
			{
				if (intCustomerID > 0)
				{
					strSkinFolder = FindMatchingSkin("Customer", "", System.Convert.ToInt32(intCustomerID));
				}
				
				if (strSkinFolder == "" && intCustomerGroupID > 0)
				{
					strSkinFolder = FindMatchingSkin("CustomerGroup", "", System.Convert.ToInt32(intCustomerGroupID));
				}
				
				if (strSkinFolder == "" && intProductID > 0)
				{
					strSkinFolder = FindMatchingSkin("Product", "", System.Convert.ToInt32(intProductID));
				}
				
				if (strSkinFolder == "" && intCategoryID > 0)
				{
					strSkinFolder = FindMatchingSkin("Category", "", System.Convert.ToInt32(intCategoryID));
				}
				
				if (strSkinFolder == "" && strScriptName != "")
				{
					strSkinFolder = FindMatchingSkin("Script", System.Convert.ToString(strScriptName), 0);
				}
				
			}
			catch (Exception)
			{
				
			}
			
			if (strSkinFolder != "")
			{
				return "~/Skins/" + strSkinFolder + "/Template.master";
			}
			else
			{
				return "";
			}
		}
		
		/// <summary>
		/// Find Skin
		/// We use the term 'Theme' for legacy reasons.
		/// (moved here from pagebaseclass as useful in places,
		/// especially for linking to images such as 'not available')
		/// </summary>
		public static string Skin(int numLanguage)
		{
			//We used a theme specified in the language record, if none
			//then use the default 'Kartris' one.
			string strSkin = System.Convert.ToString(LanguagesBLL.GetTheme(numLanguage));
			
			if (!string.IsNullOrEmpty(strSkin))
			{
				return strSkin;
			}
			else
			{
				return "Kartris";
			}
		}
		
		/// <summary>
		/// Load Skin config to cache
		/// (if there is one)
		/// </summary>
		public static void LoadSkinConfigToCache()
		{
			try
			{
				if (File.Exists(Current.Server.MapPath("~/Skins/Skins.config")))
				{
					DataTable tblSkinRules = new DataTable();
					tblSkinRules.Columns.Add("ScriptName", typeof(string));
					tblSkinRules.Columns.Add("Type", typeof(string));
					tblSkinRules.Columns.Add("ID", typeof(int));
					tblSkinRules.Columns.Add("SkinName", typeof(string));
					
					XmlDocument docXML = new XmlDocument();
					docXML.Load(Current.Server.MapPath("~/Skins/Skins.Config"));
					
					ReadSkinRules(tblSkinRules, docXML, "Customer");
					ReadSkinRules(tblSkinRules, docXML, "CustomerGroup");
					ReadSkinRules(tblSkinRules, docXML, "Product");
					ReadSkinRules(tblSkinRules, docXML, "Category");
					ReadSkinRules(tblSkinRules, docXML, "Script");
					
					if (HttpRuntime.Cache("tblSkinRules") != null)
					{
						HttpRuntime.Cache.Remove("tblSkinRules");
					}
					
					HttpRuntime.Cache.Add("tblSkinRules",
					tblSkinRules(null, DateTime.MaxValue, TimeSpan.Zero),
					Caching.CacheItemPriority.High, null);
				}
			}
			catch (Exception)
			{
				
			}
		}
		
		/// <summary>
		/// Read skin rules
		/// </summary>
		private static void ReadSkinRules(DataTable tblSkinRules, XmlDocument xmlDoc, string strType)
		{
			XmlNodeList lstNodes = null;
			XmlNode ndeRule = null;
			
			try
			{
				lstNodes = xmlDoc.SelectNodes("/configuration/" + strType + "SkinRules/" + strType);
				
				//Loop through the nodes
				foreach (XmlNode tempLoopVar_ndeRule in lstNodes)
				{
					ndeRule = tempLoopVar_ndeRule;
					DataRow drwRule = tblSkinRules.NewRow;
					drwRule["Type"] = strType;
					
					if (strType.ToLower() == "script")
					{
						drwRule["ScriptName"] = ndeRule.Attributes.GetNamedItem("Name").Value;
						drwRule["ID"] = 0;
					}
					else
					{
						drwRule["ScriptName"] = "";
						drwRule["ID"] = ndeRule.Attributes.GetNamedItem("ID").Value;
					}
					
					drwRule["SkinName"] = ndeRule.Attributes.GetNamedItem("SkinName").Value;
					tblSkinRules.Rows.Add(drwRule);
				}
			}
			catch (Exception)
			{
				
			}
			
		}
		
		/// <summary>
		/// Find skin that matches rules
		/// </summary>
		private static string FindMatchingSkin(string strType, string strScriptname, int intID)
		{
			DataTable tblSkinRules = null;
			if (ReferenceEquals(HttpRuntime.Cache("tblSkinRules"), null))
			{
				LoadSkinConfigToCache();
			}
			else
			{
				tblSkinRules = (DataTable) (HttpRuntime.Cache("tblSkinRules"));
			}
			
			DataRow[] drwSkinRules = null;
			
			if (strType.ToLower() == "script")
			{
				drwSkinRules = tblSkinRules.Select("ScriptName = '" + strScriptname + "' AND Type = '" + strType + "' ");
			}
			else
			{
				drwSkinRules = tblSkinRules.Select("ID = " + System.Convert.ToString(intID) + " AND Type = '" + strType + "' ");
			}
			
			if (drwSkinRules != null && drwSkinRules.Count() > 0)
			{
				foreach (DataRow drwSkinRule in drwSkinRules)
				{
					if (strType.ToLower() == "script")
					{
						if (drwSkinRule["ScriptName"].ToString().ToLower() == strScriptname.ToLower())
						{
							return System.Convert.ToString( drwSkinRule["SkinName"]);
						}
					}
					else
					{
						if ((int) drwSkinRule["ID"] == intID)
						{
							return System.Convert.ToString( drwSkinRule["SkinName"]);
						}
					}
				}
			}
			return "";
		}
		
		/// <summary>
		/// Format the 'items' section of order/confirmation emails
		/// </summary>
		public static string GetItemEmailText(string strName, string strDescription, double numExTax, double numIncTax, double numTaxAmount, double numTaxRate, int CurrencyID)
		{
			int CUR_ID = 0;
			if (CurrencyID > 0)
			{
				CUR_ID = CurrencyID;
			}
			else
			{
				CUR_ID = System.Convert.ToInt32(HttpContext.Current.Session("CUR_ID"));
			}
			
			StringBuilder sbdItemEmailText = new StringBuilder();
			
			sbdItemEmailText.Append(GetGlobalResourceObject("Email", "EmailText_OrderEmailBreaker") + " " + strName + "\r\n");
			if (strDescription != "")
			{
				sbdItemEmailText.Append(" " + strDescription + "\r\n");
			}
			if (GetKartConfig("general.tax.pricesinctax") == "n" || GetKartConfig("frontend.display.showtax") == "y")
			{
				sbdItemEmailText.Append(" ");
				sbdItemEmailText.Append(CurrenciesBLL.FormatCurrencyPrice(CUR_ID, numExTax, false));
				
				if (ConfigurationManager.AppSettings("TaxRegime").ToLower() != "us" && ConfigurationManager.AppSettings("TaxRegime").ToLower() != "simple")
				{
					sbdItemEmailText.Append(" + ");
					sbdItemEmailText.Append(CurrenciesBLL.FormatCurrencyPrice(CUR_ID, numTaxAmount, false));
					sbdItemEmailText.Append(" ");
					sbdItemEmailText.Append(GetGlobalResourceObject("Kartris", "ContentText_Tax"));
					sbdItemEmailText.Append(" (");
					sbdItemEmailText.Append(numTaxRate * 100 + "%)");
				}
				
				sbdItemEmailText.Append("\r\n");
			}
			else
			{
				sbdItemEmailText.Append(" ");
				sbdItemEmailText.Append(CurrenciesBLL.FormatCurrencyPrice(CUR_ID, numIncTax, false));
				sbdItemEmailText.Append("\r\n");
			}
			return sbdItemEmailText.ToString();
		}
		
		/// <summary>
		/// Format the 'modifier' section of order/confirmation emails
		/// </summary>
		public static string GetBasketModifierEmailText(BasketModifier objBasketModifier, string strName, string strDescription, int CurrencyID)
		{
			string returnValue = "";
			BasketModifier with_1 = objBasketModifier;
			returnValue = GetItemEmailText(strName, strDescription, with_1.ExTax, with_1.IncTax, with_1.TaxAmount, with_1.TaxRate, CurrencyID);
			return returnValue;
		}
		
		/// <summary>
		/// Format the table 'rows' section of order/confirmation HTML emails
		/// </summary>
		public static object GetHTMLEmailRowText()
		{
			string ByValstrDescriptionstring, 
			ByValnumExTaxdouble, 
			ByValnumIncTaxdouble, 
			ByValnumTaxAmountdouble, 
			ByValnumTaxRatedouble;
			int CurrencyID  = 0;
			long VersionID  = 0;
			long ProductID  = 0;
			int CUR_ID = 0;
			string strImageURL = "";
			string strImageTag = "";
			if (CurrencyID > 0)
			{
				CUR_ID = System.Convert.ToInt32(CurrencyID);
			}
			else
			{
				CUR_ID = System.Convert.ToInt32(HttpContext.Current.Session("CUR_ID"));
			}
			
			StringBuilder sbdHTMLRowText = new StringBuilder();
			sbdHTMLRowText.Append("<tr class=\"row_item\"><td class=\"col1\">");
			
			//## START modification mart 14 sep 2017
			
			strImageURL = System.Convert.ToString(BasketBLL.GetImageURL(VersionID, ProductID));
			if (strImageURL != "")
			{
				strImageTag = "<img src=\"" + strImageURL + "\" align = \"right\" />";
				sbdHTMLRowText.Append(strImageTag);
			}
			
			//## END modification mart 14 sep 2017
			
			//Put the name and description on the first column
			sbdHTMLRowText.Append("<strong>" + strName + "</strong><br/>");
			if (strDescription != "")
			{
				sbdHTMLRowText.Append(" " + strDescription.Replace("\r\n", "<br />") + "<br/>");
			}
			
			sbdHTMLRowText.Append("</td><td class=\"col2\"> @ ");
			//Then put the price on the second
			if (GetKartConfig("general.tax.pricesinctax") == "n" || GetKartConfig("frontend.display.showtax") == "y")
			{
				sbdHTMLRowText.Append(CurrenciesBLL.FormatCurrencyPrice(CUR_ID, numExTax, false));
				
				if (ConfigurationManager.AppSettings("TaxRegime").ToLower() != "us" && ConfigurationManager.AppSettings("TaxRegime").ToLower() != "simple")
				{
					sbdHTMLRowText.Append(" + ");
					sbdHTMLRowText.Append(CurrenciesBLL.FormatCurrencyPrice(CUR_ID, numTaxAmount, false));
					sbdHTMLRowText.Append(" ");
					sbdHTMLRowText.Append(GetGlobalResourceObject("Kartris", "ContentText_Tax"));
					sbdHTMLRowText.Append(" (");
					sbdHTMLRowText.Append(numTaxRate * 100 + "%)");
				}
				
				//sbdHTMLRowText.Append(vbCrLf)
			}
			else
			{
				sbdHTMLRowText.Append(CurrenciesBLL.FormatCurrencyPrice(CUR_ID, numIncTax, false));
				//sbdHTMLRowText.Append(vbCrLf)
			}
			sbdHTMLRowText.Append("</td></tr>");
			return sbdHTMLRowText.ToString();
		}
		
		/// <summary>
		/// Format the 'modifier' section of order/confirmation HTML emails
		/// </summary>
		public static string GetBasketModifierHTMLEmailText(BasketModifier objBasketModifier, string strName, string strDescription, int CurrencyID)
		{
			string returnValue = "";
			BasketModifier with_1 = objBasketModifier;
			returnValue = System.Convert.ToString(GetHTMLEmailRowText(strName, strDescription, with_1.ExTax, with_1.IncTax, with_1.TaxAmount, with_1.TaxRate, CurrencyID));
			return returnValue;
		}
		/// <summary>
		/// Extract the text inside <body></body> tags
		/// </summary>
		public static string ExtractHTMLBodyContents(string strHTML)
		{
			try
			{
				//remove the opening body tag and anything before it
				strHTML = strHTML.Substring(strHTML.ToLower().IndexOf("<body>") + 7 - 1);
				//remove the closing body tag and anything after it
				strHTML = strHTML.Substring(0, strHTML.ToLower().IndexOf("</body>"));
				
				//remove these template tags if present
				strHTML = strHTML.Replace("[poofflinepaymentdetails]", string.Empty);
				strHTML = strHTML.Replace("[bitcoinpaymentdetails]", string.Empty);
				strHTML = strHTML.Replace("[storeowneremailheader]", string.Empty);
				return strHTML;
			}
			catch (Exception)
			{
				//Error occurred, most likely missing template
				CkartrisFormatErrors.LogError("An error occurred processsing the email template. This can happen if you do not have the required " &
				"mail templates in your Skin's template folder. Each template should have the appropriate language "&
				"culture. See http://www.kartris.com/Knowledgebase/HTML-email-templates__k-52.aspx for more information.");
				return "An error occured - most likely the required template is missing. See http://www.kartris.com/Knowledgebase/HTML-email-templates__k-52.aspx for more information.";
			}
			
		}
		
		/// <summary>
		/// Push Kartris Notification to User Devices
		/// </summary>
		/// <param name="DataType"></param>
		/// <returns></returns>
		/// <remarks></remarks>
		public static string PushKartrisNotification(string DataType)
		{
			
			//send push notification requests if pushnotification config is enabled
			if (GetKartConfig("general.pushnotifications.enabled") == "y")
			{
				try
				{
					long DataValue = 0;
					OrdersBLL objOrdersBLL = new OrdersBLL();
					if (DataType.ToLower() == "s")
					{
						int numUnassignedTickets = 0;
						int numAwaitingTickets = 0;
						TicketsBLL._TicketsCounterSummary(numUnassignedTickets, numAwaitingTickets, 0);
						DataValue = numUnassignedTickets;
					}
					else if (DataType.ToLower() == "o")
					{
						DataValue = System.Convert.ToInt64(objOrdersBLL._GetByStatusCount(OrdersBLL.ORDERS_LIST_CALLMODE.INVOICE));
					}
					
					com.kartris.livetile.Service1 svcNotifications = new com.kartris.livetile.Service1();
					com.kartris.livetile.KartrisNotificationData KartrisNotification = new com.kartris.livetile.KartrisNotificationData();
					
					DataTable tblLoginsList = LoginsBLL.GetLoginsList;
					foreach (DataRow dtLogin in tblLoginsList.Rows)
					{
						string strXML = System.Convert.ToString(CkartrisDataManipulation.FixNullFromDB(dtLogin.Item("LOGIN_PushNotifications")));
						bool blnLoginLive = System.Convert.ToBoolean(dtLogin.Item("LOGIN_Live"));
						if (!string.IsNullOrEmpty(strXML) && blnLoginLive)
						{
							XmlDocument xmlDoc = new XmlDocument();
							xmlDoc.LoadXml(strXML);
							XmlNodeList xmlNodeList = xmlDoc.GetElementsByTagName("Device");
							foreach (XmlNode node in xmlNodeList)
							{
								//check first if the push notification device is set to live
								if (node.ChildNodes(3).InnerText.ToLower() == "true")
								{
									KartrisNotification.ClientWindowsStoreAppChannelURI = node.ChildNodes(2).InnerText;
									//append device platform info to the notification type value
									KartrisNotification.NotificationDataType = DataType + ":" + node.ChildNodes(1).InnerText;
									KartrisNotification.NotificationDataCount = DataValue;
									KartrisNotification.NotificationDataCountSpecified = true;
									KartrisNotification.KartrisWebShopURL = WebShopURL();
									svcNotifications.SendNotification(KartrisNotification);
								}
							}
						}
					}
					return "Success";
				}
				catch (Exception ex)
				{
					return " - Error encountered while trying to send notification  - " + ex.Message;
				}
				
			}
			else
			{
				return "NOT ENABLED";
			}
			
		}
	}


/// <summary>
/// Handle product combinations (all permutations of options for stock tracking)
/// </summary>
public sealed class CkartrisCombinations
{
	
	public const int MAX_NUMBER_OF_COMBINATIONS = 500; //paul changed to 500 for 1.4
	
	/// <summary>
	/// Easy way to tell difference between options and combinations product
	/// </summary>
	public static bool IsCombinationsProduct(double numProductID)
	{
		//Need to see if there are combinations, if not, this is a normal options product
		DataTable tblCurrentCombinations = new DataTable();
		VersionsBLL objVersionsBLL = new VersionsBLL();
		tblCurrentCombinations = objVersionsBLL._GetCombinationsByProductID(numProductID);
		
		if (tblCurrentCombinations.Rows.Count == 0)
		{
			//No combinations, not a combinations product
			return false;
		}
		else
		{
			//Has combinations, is a combinations product
			return true;
		}
	}
	
	/// <summary>
	/// Return all possible permutations
	/// </summary>
	public static bool GetProductCombinations(int pProductID, ref DataTable tblFinalResult, byte pLanguageID)
	{
		
		int intProductID = pProductID;
		
		DataTable tblProductOptionGroups = new DataTable();
		tblProductOptionGroups = OptionsBLL._GetOptionGroupsByProductID(intProductID);
		
		DataTable tblProductMandatoryOptions = new DataTable();
		DataTable tblProductOptionalOptions = new DataTable();
		
		DataTable[] arrTblProductMandatoryOptions = new DataTable[] {null};
		DataTable[] arrTblProductOptionalOptions = new DataTable[] {null};
		DataTable[] arrTblProductOptionalOptionsCombinations = new DataTable[] {null};
		
		int intIndex = 0;
		
		foreach (DataRow row in tblProductOptionGroups.Rows)
		{
			if (System.Convert.ToBoolean(row["P_OPTG_MustSelected"]))
			{
				tblProductMandatoryOptions = new DataTable();
				tblProductMandatoryOptions = OptionsBLL._GetOptionsByProductAndGroup(intProductID, System.Convert.ToInt32(row["P_OPTG_OptionGroupID"]), pLanguageID);
				
				//' Un-necessary columns should be removed from the table, these columns
				//'   came from the table that holds the records (from the DAL)
				tblProductMandatoryOptions.Columns.Remove("P_OPT_OptionID");
				tblProductMandatoryOptions.Columns.Remove("P_OPT_ProductID");
				tblProductMandatoryOptions.Columns.Remove("P_OPT_OrderByValue");
				tblProductMandatoryOptions.Columns.Remove("P_OPT_PriceChange");
				tblProductMandatoryOptions.Columns.Remove("P_OPT_WeightChange");
				tblProductMandatoryOptions.Columns.Remove("P_OPT_Selected");
				
				//' No need for the PrimaryKey
				tblProductMandatoryOptions.PrimaryKey = null;
				
				//' Redim the mandatory array to hold the new mandatory option
				intIndex = arrTblProductMandatoryOptions.GetUpperBound(0) + 1;
				Array.Resize(ref arrTblProductMandatoryOptions, intIndex + 1);
				arrTblProductMandatoryOptions[intIndex - 1] = tblProductMandatoryOptions;
			}
			else
			{
				tblProductOptionalOptions = new DataTable();
				tblProductOptionalOptions = OptionsBLL._GetOptionsByProductAndGroup(intProductID, System.Convert.ToInt32(row["P_OPTG_OptionGroupID"]), pLanguageID);
				
				//' Un-necessary columns should be removed from the table, these columns
				//'   came from the table that holds the records (from the DAL)
				tblProductOptionalOptions.Columns.Remove("P_OPT_OptionID");
				tblProductOptionalOptions.Columns.Remove("P_OPT_ProductID");
				tblProductOptionalOptions.Columns.Remove("P_OPT_OrderByValue");
				tblProductOptionalOptions.Columns.Remove("P_OPT_PriceChange");
				tblProductOptionalOptions.Columns.Remove("P_OPT_WeightChange");
				tblProductOptionalOptions.Columns.Remove("P_OPT_Selected");
				
				//' No need for the PrimaryKey
				tblProductOptionalOptions.PrimaryKey = null;
				
				//' Redim the mandatory array to hold the new optional option
				intIndex = arrTblProductOptionalOptions.GetUpperBound(0) + 1;
				Array.Resize(ref arrTblProductOptionalOptions, intIndex + 1);
				arrTblProductOptionalOptions[intIndex - 1] = tblProductOptionalOptions;
			}
		}
		
		DataTable tblMandatoryCombinations = new DataTable();
		DataTable tblOptionalCombinations = new DataTable();
		
		if (arrTblProductMandatoryOptions.GetUpperBound(0) > 0)
		{
			tblMandatoryCombinations = arrTblProductMandatoryOptions[0].Copy();
		}
		
		//' Creating the Mandatory Data
		for (int i = 1; i <= arrTblProductMandatoryOptions.GetUpperBound(0) - 1; i++)
		{
			tblMandatoryCombinations = JoinTablesForCombination(arrTblProductMandatoryOptions[i], tblMandatoryCombinations, false).Copy();
		}
		
		//' Creating the Optional Data
		for (int i = 0; i <= arrTblProductOptionalOptions.GetUpperBound(0) - 1; i++)
		{
			tblOptionalCombinations.Merge(arrTblProductOptionalOptions[i]);
		}
		for (int i = 0; i <= arrTblProductOptionalOptions.GetUpperBound(0) - 1; i++)
		{
			if (!RecursiveOptionalCombinations(tblOptionalCombinations, i,arrTblProductOptionalOptions[i], arrTblProductOptionalOptions))
			{
				
				return false;
			}
		}
		
		//' Merging the Manadatory & Optional data together, & produce the final result.
		tblFinalResult = new DataTable();
		if (tblMandatoryCombinations.Rows.Count > 0) //' If the options contain a manditory options to select
		{
			tblFinalResult = tblMandatoryCombinations.Copy(); //' Need to copy the Manditory Combinations 1st
			tblFinalResult.Merge(JoinTablesForCombination(tblOptionalCombinations, tblMandatoryCombinations, true));
		}
		else //' If the options don't contain a manditory options to select
		{
			tblFinalResult = tblOptionalCombinations;
		}
		
		tblProductOptionGroups.Dispose();
		tblProductOptionalOptions.Dispose();
		tblProductMandatoryOptions.Dispose();
		tblOptionalCombinations.Dispose();
		tblMandatoryCombinations.Dispose();
		
		if (tblFinalResult.Rows.Count > MAX_NUMBER_OF_COMBINATIONS)
		{
			return false;
		}
		return true;
		
	}
	
	/// <summary>
	/// Recursive routine to create combinations
	/// </summary>
	public static bool RecursiveOptionalCombinations(ref DataTable tblResult, int intIndex, DataTable tblOptionalOption, DataTable[] arrOptionalData)
	{
		if (intIndex < arrOptionalData.GetUpperBound(0) - 1)
		{
			try
			{
				for (int i = intIndex + 1; i <= arrOptionalData.GetUpperBound(0) - 1; i++)
				{
					DataTable tblOriginal = tblOptionalOption.Copy();
					DataTable tblJoinedData = JoinTablesForCombination(arrOptionalData[i], tblOriginal, false).Copy();
					tblResult.Merge(tblJoinedData);
					if (tblResult.Rows.Count > MAX_NUMBER_OF_COMBINATIONS)
					{
						return false;
					}
					if (!RecursiveOptionalCombinations(ref tblResult, i, tblJoinedData, arrOptionalData))
					{
						return false;
					}
				}
			}
			catch (Exception)
			{
			}
		}
		return true;
	}
	
	/// <summary>
	/// Do table joins using LINQ, returns result of join as table
	/// </summary>
	public static DataTable JoinTablesForCombination(DataTable tblNewData, DataTable tblResult, bool useIDListForBoth)
	{
		
		DataTable tblTemp = new DataTable();
		tblTemp = tblResult.Copy();
		
		//' CROSS JOIN by Language
		object tblJoinedResults = tblNewData;
		object b = null;
		
		tblResult.Rows.Clear();
		
		//' For each new Joined row .. it will be added to the result.
		foreach (object itmRow in tblJoinedResults)
		{
			string strID_List = itmRow.b("ID_List") + "," + itmRow.a("OPT_ID");
			if (useIDListForBoth)
			{
				strID_List = itmRow.b("ID_List") + "," + itmRow.a("ID_List");
			}
			tblResult.Rows.Add(null,
			itmRow.b("OPT_Name") + "," + itmRow.a("OPT_Name"),
			itmRow.a("LANG_ID"), strID_List);
		}
		
		return tblResult;
	}
	
}
/// <summary>
/// Handle images
/// </summary>
public sealed class CkartrisImages
{
	
	public const string strImagesPath = "~/Images/";
	public const string strCategoryImagesPath = "~/Images/Categories";
	public const string strProductImagesPath = "~/Images/Products";
	public const string strPromotionImagesPath = "~/Images/Promotions";
	public const string strThemesPath = "~/Skins";
	
	public enum IMAGE_TYPE
	{
		enum_CategoryImage = 1,
		enum_ProductImage = 2,
		enum_VersionImage = 3,
		enum_PromotionImage = 4,
		enum_OtherImage = 5,
		enum_OptionSwatch = 6,
		enum_AttributeSwatch = 7
	}
	
	public enum IMAGE_SIZE
	{
		enum_Thumb = 1,
		enum_Normal = 2,
		enum_Large = 3,
		enum_Auto = 4,
		enum_MiniThumb = 5
	}
	
	/// <summary>
	/// Format URL to image folder
	/// </summary>
	public static string CreateFolderURL(IMAGE_TYPE pImageType, string pItemID, string pParentID)
	{
		
		string strFolderURL = "~/";
		
		switch (pImageType)
		{
			case IMAGE_TYPE.enum_CategoryImage:
				strFolderURL += "Images/Categories/" + pItemID + "/";
				break;
			case IMAGE_TYPE.enum_ProductImage:
				strFolderURL += "Images/Products/" + pItemID + "/";
				break;
			case IMAGE_TYPE.enum_VersionImage:
				strFolderURL += "Images/Products/" + pParentID + "/" + pItemID + "/";
				break;
			case IMAGE_TYPE.enum_PromotionImage:
				strFolderURL += "Images/Promotions/" + pItemID + "/";
				break;
			case IMAGE_TYPE.enum_OptionSwatch:
				strFolderURL += "Images/OptionGroups/" + pItemID + "/";
				break;
			case IMAGE_TYPE.enum_AttributeSwatch:
				strFolderURL += "Images/Attributes/" + pItemID + "/";
				break;
			default:
				break;
				
		}
		
		return strFolderURL;
	}
	
	/// <summary>
	/// Set image
	/// </summary>
	public static void SetImage()
	{
		string ByValeImgTypeIMAGE_TYPE,
		ByValstrImageNamestring;
		IMAGE_SIZE eImgSize  = IMAGE_SIZE.enum_Auto;
		bool blnSizeFromConfig  = false;
		
		string strImgPath = strImagesPath;
		string strItemPlaceHolderConfig = "";
		strImgPath += "Kartris/" + strImageName +".gif";
		
		if (eImgSize != IMAGE_SIZE.enum_Auto && blnSizeFromConfig)
		{
			string stConfigKey = "frontend.display.images.";
			if (eImgSize == IMAGE_SIZE.enum_Thumb)
			{
				stConfigKey += "thumb";
			}
			else if (eImgSize == IMAGE_SIZE.enum_Normal)
			{
				stConfigKey += "normal";
			}
			else if (eImgSize == IMAGE_SIZE.enum_Large)
			{
				stConfigKey += "large";
			}
			else if (eImgSize == IMAGE_SIZE.enum_MiniThumb)
			{
				strItemPlaceHolderConfig = "minithumb";
				stConfigKey += "minithumb";
			}
			else
			{
				stConfigKey = "thumb";
			}
			
			objImg.Width = new WebControls.Unit(GetKartConfig(stConfigKey +".width"), WebControls.UnitType.Pixel);
			objImg.Height = new WebControls.Unit(GetKartConfig(stConfigKey +".height"), WebControls.UnitType.Pixel);
		}
		
		if (!File.Exists(HttpContext.Current.Server.MapPath(strImgPath)))
		{
			if (KartSettingsManager.GetKartConfig("frontend.display.image." + strItemPlaceHolderConfig +".placeholder") == "y")
			{
				strImgPath = strThemesPath + "/" + CkartrisBLL.Skin(Current.Session("LANG")) + "/Images/no_image_available.png";
			}
			else
			{
				objImg.Visible = false;
				return;
			}
		}
		
		try
		{
			objImg.ImageUrl = strImgPath;
			objImg.Visible = true;
		}
		catch (System.Exception)
		{
			//objImg.Visible = False
			//Exit Sub
		}
		
		
	}
	
	/// <summary>
	/// Finds a control recursively. Note: finds the first match that exists
	/// </summary>
	/// <remarks>Medz</remarks>
	public static Control FindControlRecursive(Control ctlParentContainerID, string ctlControlIDToFind)
	{
		if (ctlParentContainerID.ID == ctlControlIDToFind)
		{
			return ctlParentContainerID;
		}
		
		foreach (Control ctlChild in ctlParentContainerID.Controls)
		{
			Control ctlFound = FindControlRecursive(ctlChild, ctlControlIDToFind);
			
			if (ctlFound != null)
			{
				return ctlFound;
			}
		}
		return null;
	}
	
	/// <summary>
	/// Remove/delete an image
	/// </summary>
	public static void RemoveImages(IMAGE_TYPE pImageType, string pItemID, string pParentID)
	{
		
		string strFolderURL = "~/";
		
		switch (pImageType)
		{
			case IMAGE_TYPE.enum_CategoryImage:
				strFolderURL += "Images/Categories/" + pItemID + "/";
				break;
			case IMAGE_TYPE.enum_ProductImage:
				strFolderURL += "Images/Products/" + pItemID + "/";
				break;
			case IMAGE_TYPE.enum_VersionImage:
				strFolderURL += "Images/Products/" + pParentID + "/" + pItemID + "/";
				break;
			case IMAGE_TYPE.enum_PromotionImage:
				strFolderURL += "Images/Promotions/" + pItemID + "/";
				break;
			default:
				break;
				
		}
		
		if (Directory.Exists(HttpContext.Current.Server.MapPath(strFolderURL)))
		{
			if (!HttpContext.Current.Application("isMediumTrust"))
			{
				Directory.Delete(HttpContext.Current.Server.MapPath(strFolderURL), true);
			}
			else
			{
				FileInfo[] filInfo;
				DirectoryInfo dirInfo = new DirectoryInfo(HttpContext.Current.Server.MapPath(strFolderURL));
				filInfo = dirInfo.GetFiles();
				for (int i = 0; i <= filInfo.Length - 1; i++)
				{
					filInfo[i].Delete();
				}
				Directory.Delete(HttpContext.Current.Server.MapPath(strFolderURL), true);
			}
		}
	}
	
	/// <summary>
	/// Remove product related images
	/// </summary>
	public static void RemoveProductsRelatedImages()
	{
		
		//' Delete the categories' images
		DirectoryInfo dirCategories = new DirectoryInfo(HttpContext.Current.Server.MapPath(CkartrisImages.strCategoryImagesPath));
		if (dirCategories.Exists)
		{
			foreach (DirectoryInfo d in dirCategories.GetDirectories())
			{
				try
				{
					Directory.Delete(HttpContext.Current.Server.MapPath(CkartrisImages.strCategoryImagesPath + "/" + d.Name), true);
				}
				catch (Exception)
				{
				}
			}
		}
		
		//' Delete the products' images
		DirectoryInfo dirProducts = new DirectoryInfo(HttpContext.Current.Server.MapPath(CkartrisImages.strProductImagesPath));
		if (dirProducts.Exists)
		{
			foreach (DirectoryInfo d in dirProducts.GetDirectories())
			{
				try
				{
					Directory.Delete(HttpContext.Current.Server.MapPath(CkartrisImages.strProductImagesPath + "/" + d.Name), true);
				}
				catch (Exception)
				{
				}
			}
		}
		
		//' Delete the promotions' images
		DirectoryInfo dirPromotions = new DirectoryInfo(HttpContext.Current.Server.MapPath(CkartrisImages.strPromotionImagesPath));
		if (dirPromotions.Exists)
		{
			foreach (DirectoryInfo d in dirPromotions.GetDirectories())
			{
				try
				{
					Directory.Delete(HttpContext.Current.Server.MapPath(CkartrisImages.strPromotionImagesPath + "/" + d.Name), true);
				}
				catch (Exception)
				{
				}
			}
		}
	}
	
	/// <summary>
	/// compress images
	/// </summary>
	public static void CompressImage(string strImagePath, long numQuality)
	{
		int numMaxWidth = 1500;
		int numMaxHeight = 4000;
		try
		{
			int numImageNewWidth = 0;
			int numImageNewHeight = 0;
			
			System.Drawing.Image objImgOriginal = null;
			objImgOriginal = System.Drawing.Image.FromFile(strImagePath);
			objImgOriginal.RotateFlip(System.Drawing.RotateFlipType.Rotate180FlipNone);
			objImgOriginal.RotateFlip(System.Drawing.RotateFlipType.Rotate180FlipNone);
			
			if (System.Convert.ToDouble(objImgOriginal.Height / numMaxHeight) < System.Convert.ToDouble(objImgOriginal.Width / numMaxWidth))
			{
				//Resize by width
				numImageNewWidth = numMaxWidth;
				numImageNewHeight = System.Convert.ToInt32(objImgOriginal.Height / (objImgOriginal.Width / numMaxWidth));
			}
			else
			{
				//Resize by height
				numImageNewHeight = numMaxHeight;
				numImageNewWidth = System.Convert.ToInt32(objImgOriginal.Width / (objImgOriginal.Height / numMaxHeight));
			}
			
			//If new height/width bigger than old ones, cancel
			if (numImageNewHeight > objImgOriginal.Height || numImageNewWidth > objImgOriginal.Width)
			{
				numImageNewHeight = System.Convert.ToInt32(objImgOriginal.Height);
				numImageNewWidth = System.Convert.ToInt32(objImgOriginal.Width);
			}
			
			System.Drawing.Image objImgCompressed = objImgOriginal.GetThumbnailImage(numImageNewWidth, numImageNewHeight, null, null);
			objImgOriginal.Dispose();
			
			object Info = System.Drawing.Imaging.ImageCodecInfo.GetImageEncoders();
			System.Drawing.Imaging.EncoderParameters Params = new System.Drawing.Imaging.EncoderParameters(1);
			@Params.Param[0] = new System.Drawing.Imaging.EncoderParameter(System.Drawing.Imaging.Encoder.Quality, numQuality);
			
			objImgCompressed.Save(strImagePath, Info[1], @Params);
			objImgCompressed.Dispose();
			
		}
		catch (Exception)
		{
			
		}
	}
}
public sealed class CkartrisMedia
{
	public CkartrisMedia()
	{
		// VBConversions Note: Non-static class variable initialization is below.  Class variables cannot be initially assigned non-static values in C#.
		strMediaFolder = KartSettingsManager.GetKartConfig("general.uploadfolder") + "Media/";
		
	}
	static string strMediaFolder; // VBConversions Note: Initial value cannot be assigned here since it is non-static.  Assignment has been moved to the class constructors.
	
	public static void RemoveMedia(int numMediaID)
	{
		FileInfo[] filInfo;
		DirectoryInfo dirInfo = new DirectoryInfo(HttpContext.Current.Server.MapPath(strMediaFolder));
		filInfo = dirInfo.GetFiles(numMediaID +".*");
		for (int i = 0; i <= filInfo.Length - 1; i++)
		{
			filInfo[i].Delete();
		}
		filInfo = dirInfo.GetFiles(numMediaID + "_thumb.*");
		for (int i = 0; i <= filInfo.Length - 1; i++)
		{
			filInfo[i].Delete();
		}
	}
}
/// <summary>
/// Taxes
/// </summary>
public sealed class CkartrisTaxes
{
	public static float CalculateTax(float pIncTaxPrice, float pTaxRate)
	{
		bool blnIncTax = false; //= IIf(GetKartConfig("general.tax.pricesinctax") = "y", True, False)
		bool blnShowTax = false; //= IIf(GetKartConfig("frontend.display.showtax") = "y", True, False)
		
		if (ConfigurationManager.AppSettings("TaxRegime").ToLower() == "us" || ConfigurationManager.AppSettings("TaxRegime").ToLower() == "simple")
		{
			blnIncTax = false;
			blnShowTax = false;
		}
		else
		{
			blnIncTax = GetKartConfig("general.tax.pricesinctax") == "y";
			blnShowTax = GetKartConfig("frontend.display.showtax") == "y";
		}
		
		if (blnShowTax)
		{
			if (blnIncTax)
			{
				float snglTax = pTaxRate;
				float snglExTax = 0.0F;
				float snglIncTax = pIncTaxPrice;
				snglExTax = (float) (snglIncTax * (1 / (1 + (snglTax / 100))));
				
				return snglExTax;
			}
		}
		
		return pIncTaxPrice;
	}
}
/// <summary>
/// Create graphs
/// </summary>
public sealed class CkartrisGraphs
{
	public static string StatGraph(double numMaxScale, double numValue)
	{
		//Dim strGraph As String = ""
		StringBuilder stbGraph = new StringBuilder("");
		int numPercentage = 0;
		try
		{
			numPercentage = System.Convert.ToInt32((numValue / numMaxScale) * 100);
		}
		catch (Exception)
		{
			numPercentage = 0;
		}
		
		
		//Create graph containing div
		stbGraph.Append("<div class=\"stat_container\">");
		stbGraph.Append("<div class=\"stat_outside\">");
		stbGraph.Append("<div class=\"stat_inside\" style=\"width:" + System.Convert.ToString(numPercentage) + "%\">");
		
		stbGraph.Append("</div>");
		stbGraph.Append("</div>");
		stbGraph.Append("</div>");
		
		return stbGraph.ToString();
	}
}
/// <summary>
/// CSV export
/// </summary>
public sealed class CKartrisCSVExporter
{
	
	private static char FieldDelimiter;
	private static char StringDelimiter;
	
	/// <summary>
	/// Write to CSV
	/// </summary>
	public static void WriteToCSV()
	{
		int intFieldDelimiter; int intStringDelimiter;
		FieldDelimiter = intFieldDelimiter == 0 ? " " : (Strings.Chr(System.Convert.ToInt32(intFieldDelimiter)));
		StringDelimiter = intStringDelimiter == 0 ? "" : (Strings.Chr(System.Convert.ToInt32(intStringDelimiter)));
		
		StringBuilder strData = new StringBuilder("");
		strData.AppendLine(WriteColumnName(tblToExport.Columns));
		
		foreach (DataRow row in tblToExport.Rows)
		{
			strData.AppendLine(WriteDataInfo(tblToExport.Columns, row));
		}
		
		byte[] data = System.Text.UTF8Encoding.UTF8.GetBytes(strData.ToString());
		
		Current.Response.Clear();
		Current.Response.AddHeader("Content-Type", "application/Excel; charset=utf-8");
		Current.Response.AddHeader("Content-Disposition", "inline;filename=" + strFileName.Replace(" ", "_") +".csv");
		Current.Response.AddHeader("Content-Length", data.Length.ToString());
		Current.Response.ContentEncoding = Encoding.Unicode;
		Current.Response.BinaryWrite(data);
		Current.Response.End();
		Current.Response.Flush();
		
	}
	
	/// <summary>
	/// Write data
	/// </summary>
	private static string WriteDataInfo(DataColumnCollection cols, DataRow row)
	{
		StringBuilder stbData = new StringBuilder("");
		foreach (DataColumn col in cols)
		{
			AddDelimiter(System.Convert.ToString(CkartrisDataManipulation.FixNullFromDB(row[col.ColumnName])), stbData, col.DataType);
		}
		return stbData.ToString().TrimEnd(FieldDelimiter);
	}
	
	/// <summary>
	/// Write column name
	/// </summary>
	private static string WriteColumnName(DataColumnCollection cols)
	{
		string columnNames = "";
		foreach (DataColumn col in cols)
		{
			if (StringDelimiter == null)
			{
				columnNames += col.ColumnName + FieldDelimiter;
			}
			else
			{
				columnNames += StringDelimiter + col.ColumnName + System.Convert.ToString(StringDelimiter) + System.Convert.ToString(FieldDelimiter);
			}
		}
		columnNames = columnNames.TrimEnd(FieldDelimiter);
		return columnNames;
		
	}
	
	/// <summary>
	/// Add separation delimiter
	/// </summary>
	private static void AddDelimiter(string value, StringBuilder stbData, Type colType)
	{
		if (value != null && !string.IsNullOrEmpty(value))
		{
			if (((((string) colType.FullName == "System.Int16") || ((string) colType.FullName == "System.Int32")) || ((string) colType.FullName == "System.Int64")) || (colType.FullName == "System.Double"))
			{
				
				stbData.Append(value.Replace(FieldDelimiter.ToString(), "/"));
			}
			else if ((string) colType.FullName == "System.String")
			{
				value = value.Replace('\n', " ").Replace('\r', " ");
				value = value.Replace("\"", "\"\"");
				if (StringDelimiter == null)
				{
					stbData.Append("" + value.Replace(FieldDelimiter.ToString(), "/"));
				}
				else
				{
					stbData.Append(StringDelimiter + "" + value + System.Convert.ToString(StringDelimiter));
				}
			}
			else
			{
				if (StringDelimiter == null)
				{
					stbData.Append(value.Replace(FieldDelimiter.ToString(), "/"));
				}
				else
				{
					stbData.Append(StringDelimiter + value + System.Convert.ToString(StringDelimiter));
				}
			}
		}
		stbData.Append(FieldDelimiter);
	}
}
/// <summary>
/// This class tries to give a chance to recover from certain
/// types of crash by restarting the app pool. This code is
/// experimental
/// </summary>
public sealed class CkartrisRecovery
{
	//Experimental code to try to recycle app pool
}
/// <summary>
/// Class relating to the kartris hosting environment
/// and other tools relating to it
/// </summary>
public sealed class CkartrisEnvironment
{
	public static string GetClientIPAddress()
	{
		string strClientIP = "";
		if (GetKartConfig("general.security.ssl") == "e")
		{
			//using cloudflare or similar, try to find the client IP
			try
			{
				strClientIP = System.Convert.ToString(HttpContext.Current.Request.ServerVariables("HTTP_CF_CONNECTING_IP"));
			}
			catch (Exception)
			{
				//maybe not cloudflare
			}
			
			if (string.IsNullOrEmpty(strClientIP))
			{
				strClientIP = System.Convert.ToString(Current.Request.ServerVariables("HTTP_X_FORWARDED_FOR"));
			}
			if (string.IsNullOrEmpty(strClientIP))
			{
				strClientIP = "unknown";
			}
		}
		else
		{
			strClientIP = System.Convert.ToString(HttpContext.Current.Request.ServerVariables("REMOTE_ADDR"));
		}
		return strClientIP;
	}
}

/// <summary>
/// Introduced v3.0001 to handle brexit-related chaos, this
/// has some functions used to decide whether certain items
/// like EU VAT and EORI numbers should be displaced to
/// customers
/// </summary>
public sealed class CkartrisRegionalSettings
{
	/// <summary>
	/// Show EU VAT field on checkout?
	/// </summary>
	/// <param name="strBaseCountryIsoCode">Two-letter ISO code of the base country (general.tax.euvatcountry)</param>
	/// <param name="strAddressCountryIsoCode">Two-letter ISO code of the shipping country selected by user</param>
	/// <param name="blnAddressCountryTax">Tax in destination country? (D_Tax)</param>
	/// <param name="strTaxExtra">Tax extra info, we now set this to EU if using UK/VAT tax regime for EU countries</param>
	/// <param name="blnShowEUVATForDomestic">For some EU countries like Portugal, the VAT number is nearly always collected where possible, even for domestic customers, as is same as general tax ID</param>
	/// <returns>boolean</returns>
	public static bool ShowEUVATField(string strBaseCountryIsoCode, string strAddressCountryIsoCode, bool blnAddressCountryTax, string strTaxExtra, bool blnShowEUVATForDomestic)
	{
		//The EU VAT field should show in the following situations
		//1. Both country A and B are different, but both have tax due (suggests both are EU)
		//2. Same EU country and blnShowEUVATForDomestic is true
		//3. UK/Non-EU and an EU country (determined by the TaxExtra field, since they won't have tax turned on)
		if ((strBaseCountryIsoCode != strAddressCountryIsoCode) && blnAddressCountryTax)
		{
			return true;
		}
		else if (strTaxExtra == "EU" && blnShowEUVATForDomestic)
		{
			return true;
		}
		else if ((strBaseCountryIsoCode != strAddressCountryIsoCode) && strTaxExtra == "EU")
		{
			return true;
		}
		return false;
	}
	
	/// <summary>
	/// Show EORI field on checkout?
	/// </summary>
	/// <param name="blnAddressCountryTax">Tax in destination country? (D_Tax)</param>
	/// <param name="strTaxExtra">Tax extra info, we now set this to EU if using UK/VAT tax regime for EU countries</param>
	/// <remarks>EORI is a company ID number used for importing/exporting from the EU. It should show for EU customers ordering on a site based outside the EU.</remarks>
	/// <returns>boolean</returns>
	public static bool ShowEORIField(bool blnAddressCountryTax, string strTaxExtra)
	{
		//The EORI field should show in the following situations
		//1. Base country non-EU, client country EU
		if ((!blnAddressCountryTax) && strTaxExtra == "EU")
		{
			return true;
		}
		return false;
	}
}
