
class RequestMenu : Menu
{
    ProgramContext context;
    DateTime startDate = DateTime.Now.Date;
    Reason reason = Reason.Vacation;
    int days = 1;
    string comment = "";

    public RequestMenu(ProgramContext context)
    {
        this.context = context;
    }

    public override void Build()
    {
        Options = new()
        {
            (() => $"Start Date: {startDate.ToString("d")}", (i) => startDate = startDate.AddDays(i.Modifiers, i.Key == ConsoleKey.RightArrow ? 1 : i.Key == ConsoleKey.LeftArrow ? -1 : 0)),
            (() => $"Length: {days} days", (i) => days = Math.Max(0, days + (i.Key == ConsoleKey.RightArrow ? 1 : i.Key == ConsoleKey.LeftArrow ? -1 : 0))),
            (() => $"Reason: {reason}", (ConsoleKeyInfo i) => { reason = (Reason)Math.Clamp((int)(reason += i.Key == ConsoleKey.RightArrow ? 1 : i.Key == ConsoleKey.LeftArrow ? -1 : 0), 0, Enum.GetValues<Reason>().Length - 1); }
            ),
            (() => $"Comment: {comment}", (i) =>
            {
                if (i.Key == ConsoleKey.Backspace && comment.Length > 0)
                    comment = comment.Remove(comment.Length - 1);
                else if (i.KeyChar >= ' ' && i.KeyChar <= 'z')
                    comment += i.KeyChar;
            }
            ),
            (() => "Submit", (i) => { if (i.Key == ConsoleKey.Enter) { context.Workplace.VacationRequests.Add(new(context.User, startDate, TimeSpan.FromDays(days), reason) { Comment = comment, }); context.Workplace.SaveChanges(); IsOpen = false; } }
            ),
        };
    }
}