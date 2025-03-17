using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Phonebook.selnoom.Data;
using Phonebook.selnoom.Menu;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

string connectionString = configuration.GetConnectionString("DefaultConnection");

var services = new ServiceCollection();
services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));
services.AddTransient<CategoryRepository>();
services.AddTransient<ContactRepository>();
services.AddTransient<Menu>();


var serviceProvider = services.BuildServiceProvider();

using (var scope = serviceProvider.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var menu = scope.ServiceProvider.GetRequiredService<Menu>();

    DatabaseInitializer.Initialize(dbContext);

    await menu.ShowMenu();
}
