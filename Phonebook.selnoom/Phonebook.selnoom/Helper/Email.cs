using Spectre.Console;
using System.Diagnostics;

namespace Phonebook.selnoom.Helper;

static class Email
{
    public static void SendEmailToContact(string emailAddress, string subject ,string body)
    {
        if (string.IsNullOrWhiteSpace(emailAddress))
        {
            AnsiConsole.MarkupLine("[red]No email address provided for this contact![/]");
            return;
        }

        string mailtoLink = $"mailto:{emailAddress}?subject={subject}&body={body}";

        try
        {
            Process.Start(new ProcessStartInfo(mailtoLink) { UseShellExecute = true });
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Failed to open email client: {ex.Message}[/]");
        }
    }
}
