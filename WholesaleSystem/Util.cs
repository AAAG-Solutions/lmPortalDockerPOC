using System;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using Microsoft.Win32;

using LMWholesale.resource;

namespace LMWholesale {

    /// <summary>
    ///     This class is a generalized class that should only contain basic convenience functionality and/or system resources
    /// </summary>
    public class Util
    {

        public static string DefaultWebservicesHive = @"Software\Liquid Motors\WholesalePortal\WebServices";
        public static string DefaultWebservicesHive64 = @"Software\Wow6432Node\Liquid Motors\WholesalePortal\WebServices";
        public static string DefaultPortalHive = @"Software\Liquid Motors\WholesalePortal\Portal";
        public static string DefaultPortalHive64 = @"Software\Wow6432Node\Liquid Motors\Portal";

        public static readonly JavaScriptSerializer serializer = new JavaScriptSerializer();
        public static readonly IniFile ini = new IniFile(HttpContext.Current.Server.MapPath("~/WholesalePortal.ini"));

        public static string GetIniEntry(string key, string defaultVal = "")
        {
            string returnVal = ini.GetEntryValue(ini.GetEntryValue("Settings", "Environment"), key);
            if (string.IsNullOrEmpty(returnVal))
                returnVal = defaultVal;

            return returnVal;
        }

        // #TODO: Don't need this for now. Leaving here whenever we move completely away from .ini to .json/.config
        //public static string GetWebConfiguration(string key, string defaultVal = "")
        //{
        //    string val = WebConfigurationManager.AppSettings[key];
        //    if (String.IsNullOrEmpty(val))
        //        val = defaultVal;
        //    return val;
        //}

        public static string GetRegistryString(string regname, string hive, string defaultval = "")
        {
            string val = defaultval;

            if (hive.CompareTo("webservice") == 0)
            {
                val = GetWebservicesRegString(regname, RegistryView.Registry64, DefaultWebservicesHive64, "!NONE");
            }
            else if (hive.CompareTo("portal") == 0)
            {
                val = GetPortalRegString(regname, RegistryView.Registry64, DefaultPortalHive64, "!NONE");
            }

            return val;
        }

        private static string GetWebservicesRegString(string regname, RegistryView view, string hive, string defaultVal = "")
        {
            try
            {
                using (RegistryKey baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, view))
                {
                    using (RegistryKey key = Registry.LocalMachine.OpenSubKey(hive))
                    {
                        if (key != null)
                        {
                            Object regval = key.GetValue(regname);
                            if (regval != null)
                                return regval.ToString();
                            else
                                GetWebservicesRegString(regname, RegistryView.Registry32, DefaultWebservicesHive, "!NONE");
                        }
                    }
                }
            }
            catch { } // fall thru and return default

            return defaultVal;
        }

        private static string GetPortalRegString(string regname, RegistryView view, string hive, string defaultVal = "")
        {
            try
            {
                using (RegistryKey baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, view))
                {
                    using (RegistryKey key = Registry.LocalMachine.OpenSubKey(hive))
                    {
                        if (key != null)
                        {
                            Object regval = key.GetValue(regname);
                            if (regval != null)
                                return regval.ToString();
                            else
                                GetPortalRegString(regname, RegistryView.Registry32, DefaultPortalHive, "!NONE");
                        }
                    }
                }
            }
            catch { } // fall thru and return default

            return defaultVal;
        }

        public static string cleanString(string input)
        {
            return input.Replace("'", "\'").Replace("\"", "\'").Replace("\"", "\\\"");
        }

        public static int SafeStringToInt(string input)
        {
            return SafeStringToInt(input, 0);
        }

        public static int SafeStringToInt(string input, int defaultValue)
        {
            if (input == null)
                return defaultValue;
            int result;
            try
            {
                result = int.Parse(input);
            }
            catch
            {
                result = defaultValue;
            }
            return result;
        }

        public static double SafeStringToDouble(string input)
        {
            double result;
            try
            {
                input = input.Replace("$", "");
                input = input.Replace(",", "");
                result = double.Parse(input);
            }
            catch
            {
                result = 0;
            }
            return result;
        }

        public static string FormatString(string input, string type)
        {
            StringBuilder formattedString = new StringBuilder();

            if (type == "lowercase")
            {
                string[] stringSplit = input.Split(' ');

                if (stringSplit.Length == 1)
                {
                    string format = stringSplit[0];
                    if (format.Contains("-"))
                    {
                        string[] sl = format.Split('-');
                        format = $"{sl[0][0]}{sl[0].Substring(1).ToLower()}-{sl[1][0]}{sl[1].Substring(1).ToLower()}";
                        formattedString.Append(format);
                    }
                    else
                    {
                        string formatted = $"{format[0]}{format.Substring(1).ToLower()}";
                        formattedString.Append(formatted);
                    }
                }
                else
                {
                    foreach (string s in stringSplit)
                    {
                        string format = s;
                        if (format.Contains("-"))
                        {
                            string[] sl = s.Split('-');
                            format = $"{sl[0][0]}{sl[0].Substring(1).ToLower()}-{sl[1][0]}{sl[1].Substring(1).ToLower()}";
                            formattedString.Append(format);
                        }
                        else
                        {
                            string formatted = $"{s[0]}{s.Substring(1).ToLower()} ";
                            formattedString.Append(formatted);
                        }
                    }
                }
            }

            return formattedString.ToString();
        }

        public static string CreateCSV(string input)
        {
            try
            {
                if (input == null)
                    return string.Empty;

                bool containsQuote = false;
                bool containsComma = false;
                int len = input.Length;

                for (int i = 0; i < len && (containsQuote == false || containsComma == false); i++)
                {
                    char ch = input[i];
                    if (ch == '"')
                    {
                        containsQuote = true;
                    }
                    else if (ch == ',')
                    {
                        containsComma = true;
                    }
                }

                if (containsQuote && containsComma)
                    input = input.Replace("\"", "\"\"");

                if (containsComma)
                    return "\"" + input + "\"";
                else
                    return input;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}