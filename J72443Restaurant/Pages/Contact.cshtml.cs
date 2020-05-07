using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using J72443Restaurant.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;


namespace J72443Restaurant.Pages
{
    public class ContactModel : PageModel
    {

        [BindProperty]
        public ContactFormModel Contact { get; set; }


        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }


            var mailbody = $@"A customer has just sent you an email!,

            Customer Information:

            Name: {Contact.Name}
            LastName: {Contact.LastName}
            Email: {Contact.Email}
            Message: ""{Contact.Message}""


            Cheers,
            J72443 Web Application contact form";

            SendEmail(mailbody);

            // create and send the mail here

            return RedirectToPage("Account/MessageSuccess");
        }


        private void SendEmail(string mailbody)
        {
            using (var message = new MailMessage(Contact.Email, "Restaurantj72443@gmail.com"))
            {
                message.To.Add(new MailAddress("Restaurantj72443@gmail.com"));
                message.From = new MailAddress(Contact.Email);
                message.Subject = "New E-Mail from my website";
                message.Body = mailbody;

                using (var client = new SmtpClient("smtp.gmail.com", 587))
                {
                    client.EnableSsl = true;
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential("Restaurantj72443@gmail.com", "Che19970704");
                    client.Send(message);

                }
            }
        }
    }
}