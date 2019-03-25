using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace WVA_Scan_Launcher
{
    class API
    {
        public static bool NeedsUpdate()
        {
            try
            {
                bool needsUpdate = false;

                JSON_Output jsonOutput = new JSON_Output();
            
                var json = JsonConvert.SerializeObject(jsonOutput);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://ws2.wisvis.com/aws/scanner/json_check_update.rb");
                request.Method = "POST";
                request.Timeout = 10000;

                UTF8Encoding encoding = new System.Text.UTF8Encoding();
                byte[] byteArray = encoding.GetBytes(json);

                request.ContentLength = byteArray.Length;
                request.ContentType = @"application/json";

                using (Stream dataStream = request.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                }

                // Read response from api 
                WebResponse response = request.GetResponse();
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                    var json_Message = reader.ReadToEnd();
                    var jsonResponse = JsonConvert.DeserializeObject<JSON_Payload>(json_Message);

                    if (jsonResponse.Message == "True")
                        needsUpdate = true;
                    else if (jsonResponse.Message == "False")
                        needsUpdate = false;
                    else
                        throw new InvalidOperationException("Invalid parameter returned.");
                    reader.Close();                        
                }
                response.Close();

                if ((((HttpWebResponse)response).StatusDescription) != "OK")
                {
                    throw new System.InvalidOperationException("Attempted to connect to endpoint but a connection could not be established.");
                }

                // Check update status after streams have been cleaned up.
                switch (needsUpdate)
                {
                    case true:
                        return true;
                    case false:
                        return false;
                    default:
                        return false;
                }
            }
            catch(Exception ex)
            {
                Error.Report(ex);
                return false;
            }
        }
        
        public static void GetUpdate()
        {
            try
            {
                // Make sure temp dir is available
                if (!Directory.Exists(Path.publicDocs + Path.tempDir))
                    Directory.CreateDirectory(Path.publicDocs + Path.tempDir);

                // Delete any old versions of msi in temp
                if (File.Exists(Path.publicDocs + Path.tempDir + Path.msiName))
                    File.Delete(Path.publicDocs + Path.tempDir + Path.msiName);

                // Make 3 attemps to download update file
                int tryDownloadCt = 0;
               
                while (true)
                {
                    if (tryDownloadCt >= 3)
                        throw new Exception("Max number of tries reached downloading update file!");

                    try
                    {
                        tryDownloadCt++;
                        DownloadFile("https://ws2.wisvis.com/aws/scanner/CurrentMSI/WVA_Scan_App.msi", Path.publicDocs + Path.tempDir + Path.msiName);

                        if (File.Exists(Path.publicDocs + Path.tempDir + Path.msiName))
                            break;
                    }
                    catch
                    {
                        continue;
                    }
                }
            }
            catch (Exception ex)
            {
                Error.Report(ex);
            }
        }

        public static void DownloadFile(string url, string downloadPath)
        {
            using (var client = new WebClient())
            {
                client.DownloadFile(url, downloadPath);
            }
        }

    }
}
