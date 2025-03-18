using Spectre.Console;
using System.Text.RegularExpressions;

namespace Phonebook.selnoom.Helper;

public static class Validation
{
    public static string GetValidatedPhoneNumber(List<string> existingPhoneNumbers)
    {
        while (true)
        {
            string phoneNumber = AnsiConsole.Prompt(
                new TextPrompt<string>("Please enter the contact [green]phone number (required!)[/] or [blue]0[/] to return:")
            );

            if (phoneNumber == "0")
            {
                return phoneNumber;
            }

            if (phoneNumber.Length != 11 || !phoneNumber.All(char.IsDigit))
            {
                AnsiConsole.MarkupLine("[red]Phone number must be exactly 11 digits and contain only numbers.[/]");
                continue;
            }

            if (existingPhoneNumbers.Contains(phoneNumber))
            {
                AnsiConsole.MarkupLine("[red]This phone number already exists. Please enter a different one.[/]");
                continue;
            }

            return phoneNumber;
        }
    }

    public static string GetValidatedEmail()
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
