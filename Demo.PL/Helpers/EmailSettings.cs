using Demo.DAL.Models;
using System.Net.Mail;
using System.Net;

namespace Demo.PL.Helpers
{
	public static class EmailSettings
	{
		public static void SendEmail(Email email)
		{
			var Clint = new SmtpClient("smtp.gmail.com", 587);
			Clint.EnableSsl = true;
			Clint.Credentials = new NetworkCredential("melhdadd15@gmail.com", "nymzxqqcbzihwjqy");
			Clint.Send("melhdadd15@gmail.com", email.To, email.Subject, email.Body);
		}
	}
}
