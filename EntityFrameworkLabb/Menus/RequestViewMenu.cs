

class RequestViewMenu : Menu
{
    ProgramContext context;

    DateTime startTime = DateTime.Now.Date;
    DateTime endTime = DateTime.Now.Date.AddDays(7);
    string nameSearch = "";
    HashSet<Worker> selectedWorkers = new();
    public RequestViewMenu(ProgramContext context) => this.context = context;
    public override void Build()
    {
        Options.Clear();
        Options.Add((() => $"Start Date: {startTime.ToString("d")}", (i) => { startTime = startTime.AddDays(i.Modifiers, i.Key == ConsoleKey.RightArrow ? 1 : i.Key == ConsoleKey.LeftArrow ? -1 : 0); Build(); }));
        Options.Add((() => $"End Date: {endTime.ToString("d")}", (i) => { endTime = endTime.AddDays(i.Modifiers, i.Key == ConsoleKey.RightArrow ? 1 : i.Key == ConsoleKey.LeftArrow ? -1 : 0); Build(); }));
        Options.Add((
            () => $"Search Name: {nameSearch}", (i) => {
                if (i.Key == ConsoleKey.Backspace && nameSearch.Length > 0)
                    nameSearch = nameSearch.Remove(nameSearch.Length - 1);
                else if (i.KeyChar >= ' ' && i.KeyChar <= 'z')
                    nameSearch += i.KeyChar;
                Build();
            }
        ));
        selectedWorkers.Clear();
        foreach (var w in from worker in context.Workplace.Workers where worker.Name.Contains(nameSearch) select worker)
            selectedWorkers.Add(w);

        foreach (var req in context.Workplace.VacationRequests)
        {
            var item = req;
            if (req.CreationTime >= startTime && req.CreationTime <= endTime && selectedWorkers.Contains(req.Worker))
                Options.Add((
                    () => $"{req.CreationTime:d} by {req.Worker.Name} ({(DateTime.Now > req.End ? "Past" : DateTime.Now > req.From ? "Now" : "Upcoming")}):".PadRight(32) + $"{req.Reason} from {req.From:d} until {req.End:d}".PadRight(64),
                    (ConsoleKeyInfo i) => { if (i.Key == ConsoleKey.Delete) { context.Workplace.VacationRequests.Remove(item); context.Workplace.SaveChanges(); Build(); } }
                ));
        }
    }
}
