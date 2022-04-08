using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

/// <summary>
/// Summary description for PaymentRequest
/// </summary>
public class PaymentRequest
{
    public PaymentRequest()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public static string SendPaymentRequest(string endPoint, string postJsonString)
    {
        try
        {
            HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(endPoint);

            var postData = postJsonString;

            var data = Encoding.UTF8.GetBytes(postData);

            httpRequest.ProtocolVersion = HttpVersion.Version11;
            httpRequest.Method = "POST";
            httpRequest.ContentType = "application/json";

            httpRequest.ContentLength = data.Length;
            httpRequest.ReadWriteTimeout = 30000;
            httpRequest.Timeout = 15000;

            Stream stream = httpRequest.GetRequestStream();
            stream.Write(data, 0, data.Length);
            stream.Close();

            HttpWebResponse httpResponse = (HttpWebResponse)httpRequest.GetResponse();

            string jsonResponse = "";

            using (var reader = new StreamReader(httpResponse.GetResponseStream()))
            {
                string temp = null;
                while ((temp = reader.ReadLine()) != null)
                {
                    jsonResponse += temp;
                }
            }


            return jsonResponse;

        }
        catch (WebException e)
        {
            return e.Message;
        }
    }
}