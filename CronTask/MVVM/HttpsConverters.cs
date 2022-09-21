using System;
using System.Windows;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Markup;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace CronTask.MVVM
{
   public class HttpsConverters : IValueConverter
    {

        public object Convert(
           object value,
           Type targetType,
           object parameter,
           CultureInfo culture)
        {
            return false;
        }


        public object ConvertBack(
          object value,
          Type targetType,
          object parameter,
          CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public static class HttpCertificate 
    {
        /// <summary>
        /// 获取域名证书
        /// 
        /// </summary>
        /// <param name="strDNSEntry">域名www.baidu.com</param>
        /// <returns></returns>
        public static X509Certificate2 DownloadSslCertificate(string strDNSEntry)
        {

            X509Certificate2 cert = null;
            using (TcpClient client = new TcpClient())
            {
                //ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;           
                client.Connect(strDNSEntry, 443);

                SslStream ssl = new SslStream(client.GetStream(), false, new RemoteCertificateValidationCallback(ValidateServerCertificate), null);
                try
                {
                    ssl.ReadTimeout = 2000;
                    ssl.AuthenticateAsClient(strDNSEntry);
                }
                catch (AuthenticationException e)
                {

                    ssl.Close();
                    client.Close();
                    return cert;
                }
                catch (Exception e)
                {

                    ssl.Close();
                    client.Close();
                    return cert;
                }
                cert = new X509Certificate2(ssl.RemoteCertificate);
                ssl.Close();
                client.Close();
                return cert;
            }
        }


        public static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;

            
           // Console.WriteLine("Certificate error: {0}", sslPolicyErrors);

            // Do not allow this client to communicate with unauthenticated servers. 
            return false;
        }

        #region cookie设置
        private static CookieContainer m_Cookie = new CookieContainer();

        public static void SetHttpCookie(CookieContainer cookie)
        {
            m_Cookie = cookie;
        }
        #endregion

        #region HttpDownloadFile 下载文件

        public static MemoryStream HttpDownloadFile(string url)
        {
            // 设置参数
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "GET";
            request.CookieContainer = m_Cookie;

            //发送请求并获取相应回应数据
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            //直到request.GetResponse()程序才开始向目标网页发送Post请求
            Stream responseStream = response.GetResponseStream();

            //创建写入流
            MemoryStream stream = new MemoryStream();

            byte[] bArr = new byte[1024];
            int size = responseStream.Read(bArr, 0, (int)bArr.Length);
            while (size > 0)
            {
                stream.Write(bArr, 0, size);
                size = responseStream.Read(bArr, 0, (int)bArr.Length);
            }
            stream.Seek(0, SeekOrigin.Begin);
            responseStream.Close();
            return stream;
        }

        #endregion
    }
}
