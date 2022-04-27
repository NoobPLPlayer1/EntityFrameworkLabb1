// See https://aka.ms/new-console-template for more information
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

Worker user;
Workplace workplace = new();
user = workplace.Workers.First();
//workplace.Workers.Add(user = new("Lisa Johansson"));
//workplace.Workers.Add(new("Marqus Andersson"));
//workplace.Workers.Add(new("Ronny Thomasson"));
//workplace.Workers.Add(new("Frida Pettersson"));

DateTime startDate = DateTime.Now.Date;
int days = 1;
Reason reason = Reason.Vacation;
string comment = "";

Menu requestMenu = new()
{
    Options = new()
    {
        (() => $"Start Date: {startDate.ToString("d")}", (i) => startDate = startDate.AddDays(i.Modifiers, i.Key == ConsoleKey.RightArrow ? 1 : i.Key == ConsoleKey.LeftArrow ? -1 : 0)),
        (() => $"Length: {days} days", (i) => days = Math.Max(0, days + (i.Key == ConsoleKey.RightArrow ? 1 : i.Key == ConsoleKey.LeftArrow ? -1 : 0))),
        (() => $"Reason: {reason}", (ConsoleKeyInfo i) => { reason = (Reason)Math.Clamp((int)(reason += i.Key == ConsoleKey.RightArrow ? 1 : i.Key == ConsoleKey.LeftArrow ? -1 : 0), 0, Enum.GetValues<Reason>().Length - 1); }
        ),
        (() => $"Comment: {comment}", (i) => {
            if (i.Key == ConsoleKey.Backspace && comment.Length > 0)
                comment = comment.Remove(comment.Length - 1);
            else if (i.KeyChar >= ' ' && i.KeyChar <= 'z')
                comment += i.KeyChar;
        }),
    }
};
requestMenu.Options.Add((() => "Submit", (i) => { if (i.Key == ConsoleKey.Enter) { workplace.VacationRequests.Add(new(user, startDate, TimeSpan.FromDays(days), reason) { Comment = comment, }); workplace.SaveChanges(); requestMenu.IsOpen = false; } }));



Menu usersView = new()
{

};
BuildUserRequests();
void BuildUserRequests()
{
    usersView.Options.Clear();

    foreach (var worker in workplace.Workers)
    {
        var item = worker;
        usersView.Options.Add((
            () => $"{item.Name} {(item == user ? " (selected)" : "")}",
            (ConsoleKeyInfo i) => { if (i.Key == ConsoleKey.Enter) user = item; }
        ));
    }
}
DateTime startTime = DateTime.Now.Date;
DateTime endTime = startTime.AddDays(7);
string nameSearch = "";
HashSet<Worker> selectedWorkers = new();

Menu requestView = new()
{

};
BuildViewRequests();
void BuildViewRequests()
{
    requestView.Options.Clear();
    requestView.Options.Add((() => $"Start Date: {startTime.ToString("d")}", (i) => { startTime = startTime.AddDays(i.Modifiers, i.Key == ConsoleKey.RightArrow ? 1 : i.Key == ConsoleKey.LeftArrow ? -1 : 0); BuildViewRequests(); }));
    requestView.Options.Add((() => $"End Date: {endTime.ToString("d")}", (i) => { endTime = endTime.AddDays(i.Modifiers, i.Key == ConsoleKey.RightArrow ? 1 : i.Key == ConsoleKey.LeftArrow ? -1 : 0); BuildViewRequests(); }));
    requestView.Options.Add((
        () => $"Search Name: {nameSearch}", (i) => {
            if (i.Key == ConsoleKey.Backspace && nameSearch.Length > 0)
                nameSearch = nameSearch.Remove(nameSearch.Length - 1);
            else if (i.KeyChar >= ' ' && i.KeyChar <= 'z')
                nameSearch += i.KeyChar;
            BuildViewRequests();
        }
    ));
    selectedWorkers.Clear();
    foreach (var w in from worker in workplace.Workers where worker.Name.Contains(nameSearch) select worker)
        selectedWorkers.Add(w);

    foreach (var req in workplace.VacationRequests)
    {
        var item = req;
        if (req.CreationTime >= startTime && req.CreationTime <= endTime && selectedWorkers.Contains(req.Worker))
            requestView.Options.Add((
                () => $"{req.CreationTime:d} by {req.Worker.Name}:".PadRight(32) + $"{req.Reason} from {req.From:d} until {req.End:d}".PadRight(64),
                (ConsoleKeyInfo i) => { if (i.Key == ConsoleKey.Delete) { workplace.VacationRequests.Remove(item); workplace.SaveChanges(); BuildViewRequests();  } }
            ));
    }
}

Menu menu = new()
{
    Options = new()
    {
        (() => "Submit Request", requestMenu.Show),
        (() => "Show Users", usersView.Show),
        (() => "Show Requests", requestView.Show),
        (() => "Save", (i) => { if (i.Key == ConsoleKey.Enter) workplace.SaveChanges(); }),
    }
};
menu.Show(new(default, ConsoleKey.Enter, false, false, false));
static class Ext
{
    public static DateTime AddDays(this DateTime date, ConsoleModifiers mod, int amount) => mod switch
    {
        ConsoleModifiers.Control | ConsoleModifiers.Shift => date.AddYears(amount),
        ConsoleModifiers.Control => date.AddMonths(amount),
        ConsoleModifiers.Shift => date.AddDays(amount * 7),
        _ => date.AddDays(amount),
    };
}

class Menu
{
    public List<(Func<string> name, Action<ConsoleKeyInfo> action)> Options = new();
    public bool IsOpen = false;
    private int selectedIndex = 0;
    private int maxSize = 0;
    public int SelectedIndex { get => Math.Clamp(selectedIndex, 0, Options.Count - 1); set => selectedIndex = Math.Clamp(value, 0, Options.Count - 1); }

    public void Show(ConsoleKeyInfo info)
    {
        if (info.Key != ConsoleKey.Enter)
            return;
        Console.Clear();
        IsOpen = true;
        do
        {
            Console.CursorTop = 0;
            maxSize = Math.Max(maxSize, Options.Count);
            for (int k = 0; k < Options.Count; k++)
            {
                Console.ForegroundColor = k == SelectedIndex ? ConsoleColor.Green : ConsoleColor.Gray;
                Console.WriteLine(Options[k].name().PadRight(Console.WindowWidth));
            }
            for (int k = Options.Count; k < maxSize; k++)
                Console.WriteLine("".PadRight(Console.WindowWidth));

            info = Console.ReadKey(true);

            if (info.Key == ConsoleKey.UpArrow)
                SelectedIndex -= 1;
            if (info.Key == ConsoleKey.DownArrow)
                SelectedIndex += 1;
            if (info.Key == ConsoleKey.Escape)
                IsOpen = false;

            Options[SelectedIndex].action(info);
        } while (IsOpen);
        Console.Clear();
    }
}

public class Worker
{
    [Key] public int ID { get; set; }
    public string Name { get; set; }

    public Worker(string name) => Name = name;
    public Worker() { }
}

public class Request
{
    [Key] public int ID { get; set; }
    public Worker Worker { get; set; }
    public DateTime CreationTime { get; set; } = DateTime.Now;
    public DateTime From { get; set; }
    public DateTime End { get; set; }
    public Reason Reason { get; set; }
    public string Comment { get; set; } = "";

    public Request(Worker worker, DateTime from, TimeSpan length, Reason reason) => (Worker, From, End, Reason) = (worker, from, from + length, reason);

    public Request() { }
}

public enum Reason
{
    Vacation,
    SickKid,
    Sickness,
    Other,
}

public class Workplace : DbContext
{
    public DbSet<Worker> Workers { get; set; }
    public DbSet<Request> VacationRequests { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder dbcob)
    {
        dbcob.UseSqlServer(@"data Source=.\SQLEXPRESS;initial catalog=RequestDb; integrated security=SSPI");
        base.OnConfiguring(dbcob);
    }
}