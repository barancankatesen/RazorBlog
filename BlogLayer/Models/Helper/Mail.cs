using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using BlogLayer.CF;
using BlogLayer.Models.DataModel;
using System.Web.Mvc;


namespace BlogLayer.Models.Helper
{
    public class Mail
    {
        public bool MailGonder(string GelenMail, string Konu, string Mesaj)
        {
            try
            {
                RazorBlogContext _db = new RazorBlogContext();
                string MailFrom = _db.SistemMails.FirstOrDefault(x => x.SistemMailID == 1).Mail;
                string MailPassword = _db.SistemMails.FirstOrDefault(x => x.SistemMailID == 1).Password;
                MailMessage Email = new MailMessage();

                Email.From = new MailAddress(MailFrom);
                Email.IsBodyHtml = true;
                Email.Subject = Konu;
                Email.Body = Mesaj;
                Email.To.Add(MailFrom);
                Email.BodyEncoding = System.Text.Encoding.UTF8;


                string Host = "smtp.gmail.com";
                string smtpUserName = MailFrom;
                string smtpPassword = MailPassword;
                int smtpPort = 465;
                SmtpClient smtp = new SmtpClient(Host, smtpPort);
                smtp.EnableSsl = true;
                smtp.Credentials = new System.Net.NetworkCredential(smtpUserName, smtpPassword);
                smtp.Send(Email);
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }
    }
}