using System.ComponentModel.DataAnnotations;

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
