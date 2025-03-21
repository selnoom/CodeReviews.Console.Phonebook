﻿namespace Phonebook.selnoom.Models;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; }

    public ICollection<Contact> Contacts { get; set; } = new List<Contact>();
}
