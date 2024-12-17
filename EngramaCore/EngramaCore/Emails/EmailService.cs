using EngramaCore.Results;

using Microsoft.Extensions.Options;

using System.Net;
using System.Net.Mail;

namespace EngramaCore.Emails
{
	public class EmailService
	{
		private readonly SmtpSettings _smtpSettings;

		public EmailService(IOptions<SmtpSettings> smtpSettings)
		{
			_smtpSettings = smtpSettings.Value;
		}

		public async Task<Response<GenericResponse>> SendEmailAsync(ModelEmail modelEmail)
		{

			var response = new Response<GenericResponse>();
			response.Data = new GenericResponse(false, "");

			try
			{
				var smtpClient = new SmtpClient(_smtpSettings.Server)
				{
					Port = _smtpSettings.Port,
					Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password),
					EnableSsl = true  // Ensure SSL/TLS is enabled
				};

				var mailMessage = new MailMessage
				{
					From = new MailAddress(_smtpSettings.SenderEmail, _smtpSettings.SenderName),
					Subject = modelEmail.Subject,
					Body = modelEmail.Body,
					IsBodyHtml = true
				};

				mailMessage.To.Add(modelEmail.RecipientEmail);

				await smtpClient.SendMailAsync(mailMessage);

				response.IsSuccess = true;
				response.Data = new GenericResponse(true, "Email Sent");
			}
			catch (SmtpException smtpEx)
			{
				// Handle SMTP-specific exceptions
				Console.WriteLine($"SMTP Exception: {smtpEx.StatusCode} - {smtpEx.Message}");
				response.IsSuccess = false;
				response.Message = ($"SMTP Exception: {smtpEx.StatusCode} - {smtpEx.Message}");
			}
			catch (Exception ex)
			{
				// Handle general exceptions
				Console.WriteLine($"General Exception: {ex.Message}");
				response.IsSuccess = false;
				response.Message = ex.Message;
			}

			return response;
		}
	}

}
