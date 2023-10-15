using EmployeeManagementAPI.Models;
using EmployeeManagementAPI.Settings;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace EmployeeManagementAPI.Services
{
    public class MailService : IMailService // inherit IMailService. click on the error and implement the interface
    {
        private readonly MailSettings _mailSettings;
        private readonly IConfiguration _config;
        public MailService(IOptions<MailSettings> mailSettings, IConfiguration config) // IConfiguration connects you to properties inside 'appsettings.json'
        {
            _mailSettings = mailSettings.Value;
            _config = config;
        }

        public async Task SendEmailAsync(MailRequest mailRequest)       // This method is the implementation of the SendEmailAsync method from the IMailService interface. It takes a MailRequest object as a parameter, representing the details of the email to be sent.
        {
            var email = new MimeMessage(); // instance for 'MimeMessage' class created to access the properties inside. creates a new instance of MimeMessage, which is a class provided by the MimeKit library for handling email messages.
            email.Sender = MailboxAddress.Parse(_config["MailSettings:Mail"]); // Write your fake email ID here...MailboxAddress.Parse("ashmit.k@gmail.com") 
            email.To.Add(MailboxAddress.Parse(mailRequest.ToEmail)); // To whom you want to send email. That will be coming from mailRequest
            email.Subject = mailRequest.Subject; // These lines set the sender, recipient, and subject of the email based on the values provided in the MailSettings and mailRequest objects.
            var builder = new BodyBuilder(); // This class coming from Memekit. creates a new instance of BodyBuilder, which is used to construct the body of the email.
            if (mailRequest.Attachments != null) // Use the If loop if you want to add images in your blog // checks if there are any attachments to be added to the email
            {
                byte[] fileBytes; // Array of type 'byte' named 'fileBytes'; fileBytes =  [01010101111]
                foreach (var file in mailRequest.Attachments) // Attachement is a list of IFormFile...MailRequest Model // This foreach loop iterates over each attachment specified in the mailRequest.Attachments list.
                {
                    if (file.Length > 0)
                    {
                        using (var ms = new MemoryStream()) //This code block reads the content of the attachment (represented by file) and stores it as a byte array (fileBytes) using a MemoryStream. It is later added to the BodyBuilder as an attachment.
                        {
                            file.CopyTo(ms);                 // copy file content to memory stream 'ms'
                            fileBytes = ms.ToArray();        // converts the content of the MemoryStream ms into a byte array (fileBytes).
                        }
                        builder.Attachments.Add(file.FileName, fileBytes, ContentType.Parse(file.ContentType)); // Adding the atttachment 
                    }
                }
            }
            builder.HtmlBody = mailRequest.Body; // mailRequest.Body comes from the user, when he sends an email. The body of that email. This line sets the HTML body of the email using the mailRequest.Body property.
            email.Body = builder.ToMessageBody(); //This line sets the message body of the MimeMessage to the content of the BodyBuilder, which includes both the HTML body and attachments if any.
            using var smtp = new SmtpClient();    // This class comes from Mailkit
            smtp.Connect(_config["MailSettings:Host"],Convert.ToInt32(_config["MailSettings:Port"]), SecureSocketOptions.StartTls);
            smtp.Authenticate(_config["MailSettings:Mail"], _config["MailSettings:Password"]);
            await smtp.SendAsync(email);
            smtp.Disconnect(true); // Once email send, disconnect the smtp server
            /*These lines create a new SmtpClient, connect to the SMTP server using the settings from the IConfiguration, authenticate with the provided email and password, send the email asynchronously, and then disconnect from the SMTP server.*/
        }
    } //The end result is that the content of the file is read and stored in the fileBytes byte array, which can then be used to add the file as an attachment to the email (as shown in the rest of the code, where it adds the attachment using the builder.Attachments.Add() method).
}
/*
 * In summary, this MailService class is a basic implementation of sending emails using the MimeKit library and SmtpClient. 
 * The configuration settings like the sender's email, SMTP host, port, and password are read from the appsettings.json file and injected into the class through the IConfiguration interface. 
 * The method SendEmailAsync handles the actual process of creating and sending the email.
 
The code block you provided is used to read the content of a file (presumably an attachment in this case) and convert it into a byte array (fileBytes). Here's a step-by-step explanation of what the code does:

var ms = new MemoryStream(): This line creates a new MemoryStream object called ms. A MemoryStream is a stream that uses memory as a backing store instead of a file. It allows you to read from and write to memory as if it were a file.

file.CopyTo(ms): This line copies the content of the file to the MemoryStream ms. The file variable is assumed to be an instance of IFormFile, which is a representation of an uploaded file in ASP.NET Core.

fileBytes = ms.ToArray(): This line converts the content of the MemoryStream ms into a byte array (fileBytes). The ToArray() method of MemoryStream returns a new byte array that contains the current contents of the stream.

The end result is that the content of the file is read and stored in the fileBytes byte array, which can then be used to add the file as an attachment to the email (as shown in the rest of the code, where it adds the attachment using the builder.Attachments.Add() method).

Remember that this code assumes that the file variable is an instance of IFormFile, which represents a file uploaded via a form in ASP.NET Core. If file is a different type or comes from a different source, the implementation may vary accordingly.
*/