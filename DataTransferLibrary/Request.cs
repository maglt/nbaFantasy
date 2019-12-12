using System;

namespace DataTransferLibrary
{
    public class Request
    {
        public string WebRequestContent(string webserviceURL)
        {
            try
            {
                var jsonResponse = "";
                var webRequest = System.Net.WebRequest.Create(webserviceURL);

                if (webRequest != null)
                {
                    webRequest.Method = "GET";
                    webRequest.Timeout = 12000;
                    webRequest.ContentType = "application/json";

                    using (System.IO.Stream s = webRequest.GetResponse().GetResponseStream())
                    {
                        using (System.IO.StreamReader sr = new System.IO.StreamReader(s))
                        {
                            jsonResponse = sr.ReadToEnd();               

                        }
                    }

                }

                return jsonResponse;
            }
          
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }

        }

    }
}
