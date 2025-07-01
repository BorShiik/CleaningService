namespace CleanDeal.Model.Models.Email;

public class EmailSettings
{
    public string Provider { get; set; } = "Smtp";
    public SmtpSettings Smtp { get; set; } = new();
    public SendGridSettings SendGrid { get; set; } = new();
    public ResendSettings Resend { get; set; } = new();
    public string FromName { get; set; } = "Cleaning Service";
    public string FromEmail { get; set; } = "noreply@cleaning.deal";
}

public class ResendSettings
{
    public string? ApiKey { get; set; } = null;
    public string? From { get; set; }
}

public class SmtpSettings
{
    public string Host { get; set; } = "";
    public int Port { get; set; } = 25;
    public string User { get; set; } = "";
    public string Password { get; set; } = "";
    public bool EnableSsl { get; set; } = true;
}

public class SendGridSettings
{
    public string ApiKey { get; set; } = "";
    public string? From { get; set; }
}

