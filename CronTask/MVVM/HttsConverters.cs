using System;
using System.Windows;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Markup;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace CronTask.MVVM
{
   public class HttsConverters : IValueConverter
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

            Console.WriteLine("Certificate error: {0}", sslPolicyErrors);

            // Do not allow this client to communicate with unauthenticated servers. 
            return false;
        }

    }
}
