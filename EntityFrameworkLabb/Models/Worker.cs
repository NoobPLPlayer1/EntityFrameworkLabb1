using System.ComponentModel.DataAnnotations;

public class Worker
{
    [Key] public int ID { get; set; }
    public string Name { get; set; }

    public Worker(string name) => Name = name;
    public Worker() { }
}
