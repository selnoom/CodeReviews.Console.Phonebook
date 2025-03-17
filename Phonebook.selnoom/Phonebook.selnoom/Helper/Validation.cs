using Spectre.Console;
using System.Text.RegularExpressions;

namespace Phonebook.selnoom.Helper;

public class Validation
{
    public string GetValidatedPhoneNumber()
    {
        string phoneNumber = AnsiConsole.Prompt(
            new TextPrompt<string>("Please enter the contact [green]phone number (required!)[/] or [blue]0[/] to return:")
        );

        while (phoneNumber != "0" && (phoneNumber.Length != 11 || !phoneNumber.All(char.IsDigit)))
        {
            AnsiConsole.MarkupLine("[red]Phone number must be exactly 11 digits and contain only numbers.[/]");
            phoneNumber = AnsiConsole.Prompt(
                new TextPrompt<string>("Please enter the contact [green]phone number (required!)[/] or [blue]0[/] to return:")
            );
        }

        return phoneNumber;
    }

    public string GetValidatedEmail()
    {
        string email = AnsiConsole.Prompt(
            new TextPrompt<string>("Please enter the contact [green]email[/] or [blue]0[/] to return:")
                .AllowEmpty()
        );

        while (email != "0" && !string.IsNullOrWhiteSpace(email) && !Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
        {
            AnsiConsole.MarkupLine("[red]Invalid email address format.[/]");
            email = AnsiConsole.Prompt(
                new TextPrompt<string>("Please enter the contact [green]email[/] or [blue]0[/] to return:")
                    .AllowEmpty()
            );
        }

        return email;
    }
}
