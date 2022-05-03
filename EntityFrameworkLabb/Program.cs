Workplace workplace = new();

ProgramContext context = new(workplace, workplace.Workers.First());
UserViewMenu usersView = new(context);
RequestViewMenu requestView = new(context);
RequestMenu requestMenu = new(context);
Menu menu = new()
{
    Options = new()
    {
        (() => "Submit Request", requestMenu.Show),
        (() => "Show Users", usersView.Show),
        (() => "Show Requests", requestView.Show),
        (() => "Save", (i) => { if (i.Key == ConsoleKey.Enter) workplace.SaveChanges(); }
        ),
    }
};

menu.Show();
