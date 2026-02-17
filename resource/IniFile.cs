using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Web;

namespace LMWholesale.resource
{
    public class IniFile
    {
        [DllImport("kernel32")]
        static extern int GetPrivateProfileString(string Section, string Key, string Value, StringBuilder Result, int Size, string FileName);

        [DllImport("kernel32")]
        static extern int GetPrivateProfileString(string Section, int Key, string Value, [MarshalAs(UnmanagedType.LPArray)] byte[] Result, int Size, string FileName);

        [DllImport("kernel32")]
        static extern int GetPrivateProfileString(int Section, string Key, string Value, [MarshalAs(UnmanagedType.LPArray)] byte[] Result, int Size, string FileName);

        public string path;
        public IniFile(string path)
        {
            this.path = path;
        }

        public string[] GetSectionNames()
        {
            for (int maxsize = 500; true; maxsize *= 2)
            {
                byte[] bytes = new byte[maxsize];
                int size = GetPrivateProfileString(0, "", "", bytes, maxsize, path);

                if (size < maxsize - 2)
                {
                    string Selected = Encoding.ASCII.GetString(bytes, 0, size - (size > 0 ? 1 : 0));
                    return Selected.Split(new char[] { '\0' });
                }
            }
        }

        public string[] GetEntryNames(string section)
        {
            for (int maxsize = 500; true; maxsize *= 2)
            {
                byte[] bytes = new byte[maxsize];
                int size = GetPrivateProfileString(section, 0, "", bytes, maxsize, path);

                if (size < maxsize - 2)
                {
                    string entries = Encoding.ASCII.GetString(bytes, 0,
                                              size - (size > 0 ? 1 : 0));
                    return entries.Split(new char[] { '\0' });
                }
            }
        }

        public string GetEntryValue(string section, string entry, string _default = "")
        {
            for (int maxsize = 250; true; maxsize *= 2)
            {
                StringBuilder result = new StringBuilder(maxsize);
                int size = GetPrivateProfileString(section, entry, _default, result, maxsize, path);
                if (size < maxsize - 1)
                {
                    return result.ToString();
                }
            }
        }
    }
}