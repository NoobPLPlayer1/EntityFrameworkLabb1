class UserViewMenu : Menu
{
    ProgramContext context;
    public UserViewMenu(ProgramContext context) => this.context = context;
    string userNameSearch = "";
    public override void Build()
    {
        Options.Clear();
        Options.Add((
        () => $"Search Name: {userNameSearch}", (i) => {
            if (i.Key == ConsoleKey.Backspace && userNameSearch.Length > 0)
                userNameSearch = userNameSearch.Remove(userNameSearch.Length - 1);
            else if (i.KeyChar >= ' ' && i.KeyChar <= 'z')
                userNameSearch += i.KeyChar;
            Build();
        }
        ));
        foreach (var worker in context.Workplace.Workers)
        {
            var item = worker;
            if (item.Name.Contains(userNameSearch))
                Options.Add((
                    () => $"{item.Name}, Request Count {context.Workplace.VacationRequests.Count((r) => r.Worker == item)} {(item == context.User ? " (selected)" : "")}",
                    (ConsoleKeyInfo i) => { if (i.Key == ConsoleKey.Enter) context.User = item; }
                ));
        }
    }
}
