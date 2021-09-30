//using MTGAHelper.Lib.Config;
//using Serilog;
//using System;
//using System.Net;
//using System.Net.Mail;

//namespace MTGAHelper.Lib.Logging
//{
//    internal class LogWithGmail
//    {
//        IConfigApp configApp;

//        public LogWithGmail(IConfigApp configApp)
//        {
//            this.configApp = configApp;
//        }

//        public void Error(string messageTemplate, params object[] propertyValues)
//        {
//            Log.Error(messageTemplate, propertyValues);
//            //SendEmail(null, messageTemplate, propertyValues);
//        }

//        public void Error(Exception ex, string messageTemplate, params object[] propertyValues)
//        {
//            Log.Error(ex, messageTemplate, propertyValues);
//            //SendEmail(ex, messageTemplate, propertyValues);
//        }

//        public void Fatal(Exception ex, string messageTemplate, params object[] propertyValues)
//        {
//            Log.Fatal(ex, messageTemplate, propertyValues);
//            //SendEmail(ex, messageTemplate, propertyValues);
//        }

//        private void SendEmail(Exception ex, string messageTemplate, object[] propertyValues)
//        {
//            var config = configApp.configModel.LogWithGmail;
//            var client = new SmtpClient
//            {
//                Host = "smtp.gmail.com",
//                Port = 587,
//                EnableSsl = true,
//                UseDefaultCredentials = false,
//                DeliveryMethod = SmtpDeliveryMethod.Network
//            };
//            client.Credentials = new NetworkCredential(config.Username, config.Password);

//            var body = string.Format(messageTemplate, propertyValues);
//            if (ex != null)
//                body += $"{Environment.NewLine}{ex.ToString()}";

//            MailMessage mailMessage = new MailMessage();
//            mailMessage.From = new MailAddress(config.From);
//            mailMessage.To.Add(config.To);
//            mailMessage.Body = body;
//            mailMessage.Subject = "MTGAHelper Error";

//            client.Send(mailMessage);
//        }
//    }
//}