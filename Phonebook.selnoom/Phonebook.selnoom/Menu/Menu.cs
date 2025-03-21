﻿using Phonebook.selnoom.Data;
using Phonebook.selnoom.Models;
using Spectre.Console;
using Phonebook.selnoom.Helper;
using Phonebook.selnoom.Sms;

namespace Phonebook.selnoom.Menu;

public class Menu
{
    private readonly ContactRepository _contactRepository;
    private readonly CategoryRepository _categoryRepository;
    private readonly SmsService _smsService;

    public Menu(ContactRepository contactRepository, CategoryRepository categoryRepository, SmsService smsService)
    {
        _contactRepository = contactRepository;
        _categoryRepository = categoryRepository;
        _smsService = smsService;
    }

    public async Task ShowMenu()
    {
        while (true)
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
                    await ShowCategoryMenu();
                    break;
                case MainMenuChoices.Email:
                    await SendEmailMenu();
                    break;
                case MainMenuChoices.SMS:
                    await SendSmsMenu();
                    break;
                case MainMenuChoices.Exit:
                    return;
                default:
                    break;
            }
        }
    }

    private async Task ShowContactsMenu()
    {
        AnsiConsole.Clear();
        List<Contact> contacts = await _contactRepository.GetContacts();
        if (contacts.Any())
        {
            ShowContacts(contacts);
            AnsiConsole.WriteLine("");
        }

        SubMenuChoices contactChoice = AnsiConsole.Prompt(
            new SelectionPrompt<SubMenuChoices>()
                .Title("Please select an option for the contacts:")
                .AddChoices(Enum.GetValues<SubMenuChoices>())
        );

        switch (contactChoice)
        {
            case SubMenuChoices.Create:
                await CreateContact(contacts);
                break;
            case SubMenuChoices.View:
                await ViewContacts();
                break;
            case SubMenuChoices.Edit:
                await UpdateContact(contacts);
                break;
            case SubMenuChoices.Delete:
                await DeleteContact();
                break;
            case SubMenuChoices.Exit:
                return;
            default:
                break;
        }
    }
    public async Task CreateContact(List<Contact> contacts)
    {
        AnsiConsole.Clear();

        string name = AnsiConsole.Prompt(new TextPrompt<string>("Please enter the contact [green]name[/] or [blue]0[/] to return:").AllowEmpty());
        if (name == "0") return;

        List<string> phoneNumbers = contacts.Select(x => x.PhoneNumber).ToList();
        string phoneNumber = Validation.GetValidatedPhoneNumber(phoneNumbers);
        if (phoneNumber == "0") return;

        string email = Validation.GetValidatedEmail();
        if (email == "0") return;

        int? categoryId = null;

        List<Category> categories = await _categoryRepository.GetCategories();
        if (categories.Any())
        {
            categoryId = ShowCategoryChoices(categories);
        }
        if (categoryId == 0) return;

        await _contactRepository.CreateContact(name, email, phoneNumber, categoryId);

        AnsiConsole.MarkupLine("\n[green]Contact created successfully![/]");
        AnsiConsole.Prompt(new TextPrompt<string>("Press Enter to continue...").AllowEmpty());
    }

    public async Task ViewContacts()
    {
        AnsiConsole.Clear();
        List<Contact> contacts = await _contactRepository.GetContacts();
        if (contacts.Any())
        {
            ShowContacts(contacts);
        }
        else
        {
            AnsiConsole.MarkupLine("[red]No contacts saved[/]");
        }

        AnsiConsole.Prompt(new TextPrompt<string>("\nPress Enter to continue...").AllowEmpty());
    }

    public async Task UpdateContact(List<Contact> contacts)
    {
        Contact? selectedContact = await ChooseContact();
        if (selectedContact == null)
        {
            AnsiConsole.Prompt(new TextPrompt<string>("Press Enter to continue...").AllowEmpty());
            return;
        }

        var newName = AnsiConsole.Prompt(
            new TextPrompt<string>("Enter new [green]name[/] (leave empty to keep current, type 0 to cancel):")
                .AllowEmpty()
        );
        if (newName == "0") return;

        var newEmail = Validation.GetValidatedEmail();
        if (newEmail == "0") return;

        List<string> phoneNumbers = contacts.Select(x => x.PhoneNumber).ToList();
        var newPhoneNumber = Validation.GetValidatedPhoneNumber(phoneNumbers);
        if (newPhoneNumber == "0") return;

        newName = string.IsNullOrWhiteSpace(newName) ? selectedContact.Name : newName;
        newEmail = string.IsNullOrWhiteSpace(newEmail) ? selectedContact.Email : newEmail;
        newPhoneNumber = string.IsNullOrWhiteSpace(newPhoneNumber) ? selectedContact.PhoneNumber : newPhoneNumber;

        var categories = await _categoryRepository.GetCategories();
        int? newCategoryId = selectedContact.CategoryId;

        if (categories.Any())
        {
            newCategoryId = ShowCategoryChoices(categories);
            if (newCategoryId == 0) 
            {
                return;
            }
        }

        await _contactRepository.UpdateContact(
            selectedContact.Id,
            newName,
            newEmail,
            newPhoneNumber,
            newCategoryId
        );

        AnsiConsole.MarkupLine("[green]Contact updated successfully![/]");
        AnsiConsole.Prompt(new TextPrompt<string>("\nPress Enter to continue...").AllowEmpty());
    }

    public async Task DeleteContact()
    {
        Contact? selectedContact = await ChooseContact();
        if (selectedContact == null)
        {
            AnsiConsole.Prompt(new TextPrompt<string>("Press Enter to continue...").AllowEmpty());
            return;
        }

        await _contactRepository.DeleteContact(selectedContact.Id);

        AnsiConsole.MarkupLine("[green]Contact deleted successfully![/]");
        AnsiConsole.Prompt(new TextPrompt<string>("\nPress Enter to continue...").AllowEmpty());
    }


    public void ShowContacts(List<Contact> contacts)
    {
        foreach(Contact contact in contacts)
        {
            var categoryName = contact.Category?.Name ?? "";
            AnsiConsole.WriteLine($"Name: {contact.Name}\tNumber: {contact.PhoneNumber}\tEmail: {contact.Email}\tCategory: {categoryName}");
        }
    }

    public string ShowContactChoices(List<Contact> contacts)
    {
        var contactChoices = new List<string> { "Return to Menu" };
        contactChoices.AddRange(contacts.Select(c => $"{c.Id} - {c.Name} ({c.PhoneNumber})"));

        string selectedContactStr = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Select a [green]contact[/] to update:")
                .PageSize(10)
                .AddChoices(contactChoices)
        );

        return selectedContactStr;
    }

    public async Task<Contact?> ChooseContact()
    {
        AnsiConsole.Clear();
        List<Contact> contacts = await _contactRepository.GetContacts();
        if (contacts.Count == 0)
        {
            AnsiConsole.MarkupLine("[red]No contacts found![/]");
            return null;
        }

        var selectedContactStr = ShowContactChoices(contacts);
        if (selectedContactStr == "Return to Menu")
        {
            return null;
        }

        int selectedId = int.Parse(selectedContactStr.Split('-')[0].Trim());
        Contact? selectedContact = contacts.FirstOrDefault(c => c.Id == selectedId);
        if (selectedContact == null)
        {
            AnsiConsole.MarkupLine("[red]Contact not found![/]");
            return null;
        }

        return selectedContact;
    }

    public async Task<Contact?> ChooseContactByCategory(int categoryId)
    {
        AnsiConsole.Clear();

        List<Contact> contacts = await _contactRepository.GetContactsByCategoryId(categoryId);
        if (contacts.Count == 0)
        {
            AnsiConsole.MarkupLine("[red]No contacts found in this category![/]");
            return null;
        }

        var selectedContactStr = ShowContactChoices(contacts);
        if (selectedContactStr == "Return to Menu")
        {
            return null;
        }

        int selectedId = int.Parse(selectedContactStr.Split('-')[0].Trim());
        Contact? selectedContact = contacts.FirstOrDefault(c => c.Id == selectedId);
        if (selectedContact == null)
        {
            AnsiConsole.MarkupLine("[red]Contact not found![/]");
            return null;
        }

        return selectedContact;
    }


    public async Task ShowCategoryMenu()
    {
        await ShowCategories();

        SubMenuChoices contactChoice = AnsiConsole.Prompt(
            new SelectionPrompt<SubMenuChoices>()
                .Title("Please select an option for the cateogories:")
                .AddChoices(Enum.GetValues<SubMenuChoices>())
        );

        switch (contactChoice)
        {
            case SubMenuChoices.Create:
                await CreateCategory();
                break;
            case SubMenuChoices.View:
                await ViewCategories();
                break;
            case SubMenuChoices.Edit:
                await UpdateCategory();
                break;
            case SubMenuChoices.Delete:
                await DeleteCategory();
                break;
            case SubMenuChoices.Exit:
                return;
            default:
                break;
        }
    }

    public async Task CreateCategory()
    {
        AnsiConsole.Clear();

        string name = AnsiConsole.Prompt(new TextPrompt<string>("Please enter the category [green]name[/] or [blue]0[/] to return:"));
        if (name == "0") return;

        await _categoryRepository.CreateCategory(name);

        AnsiConsole.MarkupLine("\n[green]Category created successfully![/]");
        AnsiConsole.Prompt(new TextPrompt<string>("Press Enter to continue...").AllowEmpty());
    }

    public async Task ViewCategories()
    {
        List<Category>? categories = await ShowCategories();
        if (!categories.Any())
        {
            AnsiConsole.MarkupLine("[red]No categories saved![/]");
        }

        string choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Would you like to view the contacts of a category?")
                .AddChoices("Yes", "No")
        );

        switch (choice)
        {
            case "Yes":
                await ShowContactsInCateogry(categories);
                break;
            case "No":
                return;
            default:
                break;
        }
    }

    public async Task UpdateCategory()
    {
        AnsiConsole.Clear();
        List<Category> categories = await _categoryRepository.GetCategories();
        if (!categories.Any())
        {
            AnsiConsole.MarkupLine("[red]No categories saved![/]");
            AnsiConsole.Prompt(new TextPrompt<string>("\nPress Enter to continue...").AllowEmpty());
            return;
        }

        Category category = ChooseCategory(categories);
        if (category == null)
        {
            return;
        }

        var newName = AnsiConsole.Prompt(
            new TextPrompt<string>("Enter new [green]name[/] (leave empty to keep current, type 0 to cancel):")
                .AllowEmpty()
        );
        if (newName == "0") return;

        newName = string.IsNullOrWhiteSpace(newName) ? category.Name : newName;
        await _categoryRepository.UpdateCategory(category.Id, newName);

        AnsiConsole.MarkupLine("[green]Category updated successfully![/]");
        AnsiConsole.Prompt(new TextPrompt<string>("\nPress Enter to continue...").AllowEmpty());
    }

    public async Task DeleteCategory()
    {
        AnsiConsole.Clear();
        List<Category> categories = await _categoryRepository.GetCategories();
        if (!categories.Any())
        {
            AnsiConsole.MarkupLine("[red]No categories saved![/]");
            AnsiConsole.Prompt(new TextPrompt<string>("\nPress Enter to continue...").AllowEmpty());
            return;
        }

        Category? category = ChooseCategory(categories);
        if (category == null)
        {
            return;
        }

        await _categoryRepository.DeleteCategory(category.Id);

        AnsiConsole.MarkupLine("[green]Category deleted successfully![/]");
        AnsiConsole.Prompt(new TextPrompt<string>("\nPress Enter to continue...").AllowEmpty());
    }

    public async Task ShowContactsInCateogry(List<Category> categories)
    {
        AnsiConsole.Clear();
        Category? category = ChooseCategory(categories);
        if (category == null)
        {
            return;
        }

        List<Contact> contacts = await _contactRepository.GetContactsByCategoryId(category.Id);
        ShowContacts(contacts);
        AnsiConsole.Prompt(new TextPrompt<string>("\nPress Enter to continue...").AllowEmpty());
    }

    public async Task<List<Category>?> ShowCategories()
    {
        AnsiConsole.Clear();
        List<Category> cateogries = await _categoryRepository.GetCategories();
        if (!cateogries.Any())
        {
            return null;
        }
        AnsiConsole.MarkupLine("[blue]Categories:[/]");
        foreach (Category category in cateogries)
        {
            AnsiConsole.WriteLine($"{category.Name}");
        }
        return cateogries;
    }

    public int? ShowCategoryChoices(List<Category> categories)
    {
        int? categoryId = null;
        var categoryChoices = new List<string> { "None", "Return to menu" };
        categoryChoices.AddRange(categories.Select(c => c.Name));

        var selectedCategory = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Select a [green]category[/] for the contact:")
                .AddChoices(categoryChoices)
        );

        if (selectedCategory == "Return to menu")
        {
            categoryId = 0;
        }
        else if (selectedCategory != "None")
        {
            Category? cat = categories.FirstOrDefault(c => c.Name == selectedCategory);
            if (cat != null)
            {
                categoryId = cat.Id;
            }
        }
            return categoryId;
    }

    public Category? ChooseCategory(List<Category> categories)
    {
        var categoryChoices = new List<string> { "Return to menu" };
        categoryChoices.AddRange(categories.Select(c => c.Name));

        var selectedCategory = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Select a [green]category[/] for the contact:")
                .AddChoices(categoryChoices)
        );

        if (selectedCategory == "Return to menu")
        {
            return null;
        }
        else
        {
            return categories.FirstOrDefault(c => c.Name == selectedCategory);
        }
    }

    public async Task SendEmailMenu()
    {
        string choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Would you like to view all contacts or select a category?")
                .AddChoices("Contacts", "Categories", "Exit")
        );

        switch (choice)
        {
            case "Contacts":
                Contact? contact = await ChooseContact();
                SendEmailToContact(contact);
                break;
            case "Categories":
                List<Category> categories = await _categoryRepository.GetCategories();
                if (!categories.Any())
                {
                    AnsiConsole.MarkupLine("[red]No categories saved![/]");
                    AnsiConsole.Prompt(new TextPrompt<string>("\nPress Enter to continue...").AllowEmpty());
                    return;
                }
                Category? category = ChooseCategory(categories);
                Contact? categoryContact = await ChooseContactByCategory(category.Id);
                SendEmailToContact(categoryContact);
                break;
            case "Exit":
                return;
            default:
                break;
        }
    }

    public void SendEmailToContact(Contact contact)
    {
        AnsiConsole.Clear();
        if (contact.Email == null)
        {
            AnsiConsole.MarkupLine("[red]No categories saved![/]");
            AnsiConsole.Prompt(new TextPrompt<string>("\nPress Enter to continue...").AllowEmpty());
            return;
        }

        string subject = AnsiConsole.Prompt(new TextPrompt<string>("Please enter the [green]subject[/] of your email or [blue]0[/] to return:"));
        if (subject == "0") return;

        string message = AnsiConsole.Prompt(new TextPrompt<string>("Please enter the [green]message[/] of your email or [blue]0[/] to return:"));
        if (message == "0") return;

        Email.SendEmailToContact(contact.Email, subject, message);

        AnsiConsole.MarkupLine("[green]Success![/]");
        AnsiConsole.Prompt(new TextPrompt<string>("\nPress Enter to continue...").AllowEmpty());
    }

    public async Task SendSmsMenu()
    {
        string choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Would you like to view all contacts or select a category?")
                .AddChoices("Contacts", "Categories", "Exit")
        );

        switch (choice)
        {
            case "Contacts":
                Contact? contact = await ChooseContact();
                SendSmsToContact(contact);
                break;
            case "Categories":
                List<Category> categories = await _categoryRepository.GetCategories();
                if (!categories.Any())
                {
                    AnsiConsole.MarkupLine("[red]No categories saved![/]");
                    AnsiConsole.Prompt(new TextPrompt<string>("\nPress Enter to continue...").AllowEmpty());
                    return;
                }
                Category? category = ChooseCategory(categories);
                Contact? categoryContact = await ChooseContactByCategory(category.Id);
                SendSmsToContact(categoryContact);
                break;
            case "Exit":
                return;
            default:
                break;
        }
    }

    public void SendSmsToContact(Contact contact)
    {
        AnsiConsole.Clear();
        string message = AnsiConsole.Prompt(new TextPrompt<string>("Please enter the [green]message[/] of your SMS or [blue]0[/] to return:"));
        if (message == "0") return;

        _smsService.SendSms(contact.PhoneNumber, message);

        AnsiConsole.Prompt(new TextPrompt<string>("\nPress Enter to continue...").AllowEmpty());
    }
}
