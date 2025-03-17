using Phonebook.selnoom.Data;
using Phonebook.selnoom.Models;
using Spectre.Console;

namespace Phonebook.selnoom.Menu;

public class Menu
{
    private readonly ContactRepository _contactRepository;
    private readonly CategoryRepository _categoryRepository;

    public Menu(ContactRepository contactRepository, CategoryRepository categoryRepository)
    {
        _contactRepository = contactRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task ShowMenu()
    {
        AnsiConsole.Clear();

        var menuChoice = AnsiConsole.Prompt(
                new SelectionPrompt<MainMenuChoices>()
                    .Title("Please select an option:")
                    .AddChoices(Enum.GetValues<MainMenuChoices>())
            );

        switch (menuChoice)
        {
            case MainMenuChoices.Contacts:
                await ShowContactsMenu();
                break;
            case MainMenuChoices.Categories:

                break;
            case MainMenuChoices.Exit:
                return;
            default:
                break;
        }
    }

    private async Task ShowContactsMenu()
    {
        AnsiConsole.Clear();
        List<Contact> contacts = await _contactRepository.GetContacts();
        if (contacts.Count > 0)
        {
            ShowContacts(contacts);
            AnsiConsole.WriteLine("");
        }

        var contactChoice = AnsiConsole.Prompt(
                new SelectionPrompt<SubMenuChoices>()
                    .Title("Please select an option for the contacts:")
                    .AddChoices(Enum.GetValues<SubMenuChoices>())
            );

        switch (contactChoice)
        {
            case SubMenuChoices.Create:
                await CreateContact();
                break;
            case SubMenuChoices.View:

                break;
            case SubMenuChoices.Edit:

                break;
            case SubMenuChoices.Delete:

                break;
            case SubMenuChoices.Exit:

                break;
            default:
                break;
        }
    }
    public async Task CreateContact()
    {
        AnsiConsole.Clear();

        //These guys should allow empty/null
        string name = AnsiConsole.Ask<string>("Please enter the contact [green]name[/]:");
        string phoneNumber = AnsiConsole.Ask<string>("Please enter the contact [green]phone number (required!)[/]:");
        string email = AnsiConsole.Ask<string>("Please enter the contact [green]email[/]:");

        List<Category> categories = await _categoryRepository.GetCategories();
        //TODO show cateogries and none in prompt and let user choose
    }



    public void ShowContacts(List<Contact> contacts)
    {
        foreach(Contact contact in contacts)
        {
            AnsiConsole.WriteLine($"Name: {contact.Name}\tNumber: {contact.PhoneNumber}\tEmail: {contact.Email}\tCategory: {contact.Category.Name}");
        }
    }
}
