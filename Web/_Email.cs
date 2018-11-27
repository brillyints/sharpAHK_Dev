using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using sharpAHK;

namespace sharpAHK_Dev
{
    public partial class _Web
    {
        public class Email
        {

            public void EmailCoder(string EmailBodyText, int Options = 0)  // used to debug / alert programmer while testing
            {
                _AHK ahk = new _AHK();
                ahk.MsgBox("Email Coder Function Not Configured Here Yet");

                ////ex: 
                ////  Email mail = new Email();
                ////  mail.EmailCoder(ErrorText); 


                //try
                //{
                //    string IpAddress = "10.10.55.6";
                //    string Port = "25";
                //    string FromAddress = "Sky@group.com";
                //    string EmailList = "Jason@group.com";

                //    if (Options == 1)
                //    {
                //        EmailList = "Jason@group.com; Chase@group.com;";
                //    }

                //    SmtpClient Smtp_Server = new SmtpClient(IpAddress, Convert.ToInt32(Port));
                //    MailMessage e_mail = new MailMessage();

                //    Smtp_Server.Credentials = new NetworkCredential();
                //    Smtp_Server.UseDefaultCredentials = true;
                //    Smtp_Server.DeliveryMethod = SmtpDeliveryMethod.Network;

                //    Smtp_Server.EnableSsl = false;

                //    e_mail = new MailMessage();
                //    e_mail.From = new MailAddress(FromAddress);

                //    DataTable impDtListofErasure = new DataTable();
                //    DataTable dtList = new DataTable();
                //    //string EmailString = "";
                //    Int16 i = 0;

                //    e_mail = null;

                //    e_mail = new MailMessage();
                //    e_mail.From = new MailAddress(FromAddress);

                //    foreach (string strX in EmailList.Split(';'))
                //    {
                //        //if (strX.Trim.Length > 0)
                //        if (strX.Length > 0)
                //        {
                //            e_mail.To.Add(new MailAddress(strX));
                //        }
                //    }

                //    i = 0;

                //    e_mail.Subject = "SkyNet SSR Integration";
                //    e_mail.IsBodyHtml = false;


                //    //e_mail.Body = EmailBodyText;
                //    e_mail.Body = EmailBodyText;


                //    Smtp_Server.Send(e_mail);

                //    //System.Environment.Exit(0);

                //}
                //catch (Exception error_t)
                //{
                //    //MessageBox.Show("SkyNet SSR Integration: " + error_t.Message);
                //    //System.Environment.Exit(0);
                //}
                //finally
                //{
                //    //MessageBox.Show("Email Sent"); 
                //    //System.Environment.Exit(0);
                //}
            }

            //==== GMail =======

            #region === Email / Gmail ===

            // Send Gmail (From Gmail Account) - Can include list of file paths to add as attachments

            public bool Gmail_Coder(string Subject = "Namtrak Says Sup AGAIN?", string Body = "Mail with attachment? Maybe so...", List<string> Attachments = null, object To = null)
            {
                _AHK ahk = new _AHK();
                _Database.SQLite sqlite = new _Database.SQLite();
                _StatusBar sb = new _StatusBar();

                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
                mail.From = new MailAddress("lucidmethod@gmail.com");

                // default email address if nothing provided
                if (To == null) { mail.To.Add("lucidmethod@gmail.com"); }
                else
                {
                    string VarType = To.GetType().ToString();  //determine what kind of variable was passed into function

                    // user assed in 1 email address as string 
                    if (VarType == "System.String") { mail.To.Add(To.ToString()); }

                    // user passed in list of email addresses to send to 
                    else if (VarType == "System.Collections.Generic.List`1[System.String]")
                    {
                        //ahk.MsgBox("String List");
                        List<string> ToList = (List<string>)To;
                        foreach (string add in ToList) { mail.To.Add(add); }
                    }
                    else
                    {
                        ahk.MsgBox("Gmail Coder Function | Unable To Use VarType " + VarType);
                        return false;
                    }
                }


                mail.Subject = Subject;
                mail.Body = Body;

                if (Attachments != null && Attachments.Count > 0)
                {
                    System.Net.Mail.Attachment attachment;

                    foreach (string file in Attachments)
                    {
                        attachment = new System.Net.Mail.Attachment(file);
                        mail.Attachments.Add(attachment);
                    }
                }

                string gmailL = ConfigurationManager.AppSettings["GmailLogin"];  // read setting from app.config
                string gmailP = ConfigurationManager.AppSettings["GmailPass"];  // read setting from app.config


                //SmtpServer.EnableSsl = (SmtpServer.Port == 465);
                SmtpServer.Port = 587;
                SmtpServer.Credentials = new System.Net.NetworkCredential(gmailL, gmailP);
                SmtpServer.EnableSsl = true;

                try
                {
                    SmtpServer.Send(mail);
                    sb.StatusBar("Sent Gmail");
                    return true;
                }
                catch (Exception ex)
                {
                    ahk.MsgBox(ex.ToString());
                    sb.StatusBar("ERROR SENDING GMAIL");
                    return false;
                }

            }


            #endregion

        }

    }
}
