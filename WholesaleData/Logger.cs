using System;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading;


namespace LMWholesale.WholesaleData
{
    public class Logger
    {
        public static string basePath;
        public static int maxDataLength;
        public static bool debug;

        public Logger(string _basename, bool debugMode = false, int max_data_length = 2048)
        {
            basePath = _basename;
            debug = debugMode;
            maxDataLength = max_data_length;
        }

        public void LogLine(string kSession, string line)
        {
            // if debug is set to true,
            // we will continue to create dir tree and log file for debugging.
            // Otherwise, do nothing
            if (!debug)
                return;

            if (line.Length > maxDataLength)
                line = line.Substring(0, maxDataLength) + " .... ";
            line = FormatLine(line);
            try
            {
                String fname = GetFName(kSession);
                if (!System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(fname)))
                    System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(fname));

                // in case we get bad write attempts
                bool written = false;
                int tries = 0;
                while (!written && tries < 10)
                {
                    try
                    {
                        using (System.IO.FileStream fs = new System.IO.FileStream(fname, System.IO.FileMode.Append, System.IO.FileAccess.Write, System.IO.FileShare.Read, 8, System.IO.FileOptions.None))
                        {
                            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(fs, Encoding.ASCII))
                            {
                                sw.WriteLine(line);
                            }
                        }
                        written = true;
                    }
                    catch
                    {
                        tries++;
                        Thread.Sleep(10);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An exception was generated while attempting to access a session log.  Message is: " + ex.Message);
            }
        }

        private static string FormatLine(string line)
        {
            return string.Format("[{0}][{1}] {2}", DateTime.Now.ToString("HH:mm:ss.ffff"), Thread.CurrentThread.ManagedThreadId, line);
        }

        private static string GetFName(string kSession)
        {
            return string.Format("{2}{0}_{1}.log", kSession, DateTime.Now.ToString("yyyyMMdd"), basePath);
        }
    }
}