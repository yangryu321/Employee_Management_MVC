using SendGrid;
using SendGrid.Helpers.Mail;

namespace EmployeeManagementSystem.Utilities
{
    public interface IEmailService 
    {
        Task<bool> SendEmailAsync(string email, string subject, string body);
    }
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            this._configuration = configuration;
        }


        public async Task<bool> SendEmailAsync(string email, string subject, string body)
        {
            var apiKey = _configuration["SendGrid:ApiKey"];
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("syliujob@gmail.com", "Shiyang Liu");
            var to = new EmailAddress(email);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent: body, htmlContent: body);

            try
            {
                var response = await client.SendEmailAsync(msg);
                return response.StatusCode == System.Net.HttpStatusCode.Accepted;
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                return false;
            }
        }
    }
}
