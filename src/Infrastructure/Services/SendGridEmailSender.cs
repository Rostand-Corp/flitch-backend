using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services;
using SendGrid;
using SendGrid.Helpers.Mail;

public class SendGridEmailSender : IEmailSender
{
    private readonly ILogger _logger;
    public SendGridEmailSender(IOptions<AuthMessageSenderOptions> optionsAccessor,
        ILogger<SendGridEmailSender> logger)
    {
        Options = optionsAccessor.Value;
        _logger = logger;
    }
    public AuthMessageSenderOptions Options { get; }
    
    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        if (string.IsNullOrEmpty(Options.SendGridKey))
        {
            throw new Exception("Specify SendGridKey in config");
        }
        await Execute(Options.SendGridKey, email, subject, htmlMessage);
    }

    private async Task Execute(string apiKey, string email, string subject, string message)
    {
        var client = new SendGridClient(apiKey);
        var msg = new SendGridMessage()
        {
            From = new EmailAddress("flitchapp@gmail.com", "Flitch"), // todo: config
            Subject = subject,
            PlainTextContent = message,
            HtmlContent = message
        };
        msg.AddTo(new EmailAddress(email));
        msg.SetClickTracking(false, false);
        var response = await client.SendEmailAsync(msg); // Todo: Add cancellation token

        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation($"Email to {email} queued successfully!");
        }
        else
        {
            _logger.LogError($"Email to {email} has failed!");
        }

    }
}