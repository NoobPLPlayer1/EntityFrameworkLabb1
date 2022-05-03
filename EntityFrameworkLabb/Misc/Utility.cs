static class Utility
{
    public static DateTime AddDays(this DateTime date, ConsoleModifiers mod, int amount) => mod switch
    {
        ConsoleModifiers.Control | ConsoleModifiers.Shift => date.AddYears(amount),
        ConsoleModifiers.Control => date.AddMonths(amount),
        ConsoleModifiers.Shift => date.AddDays(amount * 7),
        _ => date.AddDays(amount),
    };
}
