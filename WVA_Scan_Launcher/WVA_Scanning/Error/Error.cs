using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WVA_Scan_Launcher.JSON;

namespace WVA_Scan_Launcher
{
    class Error
    {
        private static bool ShouldWriteError { get; set; }

        public static void Report(string error)
        {
            SendError(error);
            if (ShouldWriteError)
            {
                PrintToLog(error);
            }
        }

        public static void Report(Exception error)
        {
            SendError(error.ToString());
            if (ShouldWriteError)
            {
                PrintToLog(error.ToString());
            }
        }

        private static void SendError(string error)
        {
            try
            {
                string actNum = Account.GetAccountNumber();

                ErrorOutput errorOutput = new ErrorOutput(actNum, error);

                var json = JsonConvert.SerializeObject(errorOutput);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://ws2.wisvis.com/aws/scanner/error_handler.rb");
                request.Method = "POST";
                request.Timeout = 10000;

                System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
                Byte[] byteArray = encoding.GetBytes(json);

                request.ContentLength = byteArray.Length;
                request.ContentType = @"application/json";

                using (Stream dataStream = request.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                }

                WebResponse response = request.GetResponse();
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                    var json_Message = reader.ReadToEnd();
                    var jsonResponse = JsonConvert.DeserializeObject<Json_Response>(json_Message);

                    if (jsonResponse.Status == "SUCCESS")
                    {
                        ShouldWriteError = false;
                    }
                    else if (jsonResponse.Message == "FAIL")
                    {
                        ShouldWriteError = true;
                        PrintToLog(jsonResponse.Message);
                    }
                    else
                    {
                        throw new System.InvalidOperationException("Invalid parameter returned.");
                    }
                    reader.Close();
                }
                response.Close();

                if ((((HttpWebResponse)response).StatusDescription) != "OK")
                {
                    throw new System.InvalidOperationException("Attempted to connect but a connection could not be established.");
                }
            }
            catch (Exception e)
            {
                PrintToLog(e.ToString());
            }
        }

        private static void PrintToLog(string error)
        {
            try
            {
                if (!Directory.Exists(Path.DirErrorLog)) { }
                Directory.CreateDirectory(Path.DirErrorLog);

                if (!File.Exists(Path.DirErrorLog + @"\ErrorLog.txt"))
                {
                    var file = File.Create(Path.DirErrorLog + @"\ErrorLog.txt");
                    file.Close();
                }

                string time = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
                using (System.IO.StreamWriter writer = new System.IO.StreamWriter((Path.DirErrorLog + @"\ErrorLog.txt"), true))
                {
                    writer.WriteLine("-----------------------------------------------------------------------------------");
                    writer.WriteLine("");
                    writer.WriteLine("(TIME: " + time + ")");
                    writer.WriteLine("(ERROR:" + error + ")");
                    writer.WriteLine("");
                    writer.WriteLine("-----------------------------------------------------------------------------------");
                    writer.Close();
                }
            }
            catch { };
        }
    }
}
