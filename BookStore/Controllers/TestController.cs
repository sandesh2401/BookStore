using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Threading.Tasks;

namespace BookStore.Controllers
{
    public class TestController : Controller
    {
        private readonly IEmailSender _emailSender;

        // Constructor to inject IEmailSender
        public TestController(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }

        // Action method to send a test email
        public async Task<IActionResult> SendTestEmail()
        {
            // Sending a test email
            await _emailSender.SendEmailAsync(
                "yashchimade02@gmail.com", // Replace with the recipient's email address
                "Test Subject",           // Subject of the email
                "This is a test email."); // Body of the email

            // Return a confirmation message
            return Content("Test email sent successfully.");
        }
    }
}
