using System.Net.Mail;

namespace CampusEvents.Backend;

public class EmailDomainValidator
{
    private readonly string _allowedDomain;

    public EmailDomainValidator(string allowedDomain = "univ.edu.ph")
    {
        _allowedDomain = allowedDomain;
    }

    public bool IsValid(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return false;
        }

        try
        {
            var trimmed = email.Trim();
            var address = new MailAddress(trimmed);
            return address.Address.Equals(trimmed, StringComparison.OrdinalIgnoreCase)
                && address.Host.Equals(_allowedDomain, StringComparison.OrdinalIgnoreCase);
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
