public class Task
{
    public int Id { get; set; }
    public string  Name { get; set; }
    public string Description { get; set; }
    public DateTime DueDate { get; set; } = DateTime.Now;
    public Status Status { get; set; } = Status.Pending;

    public override string ToString()
    {
        return $"Id: {Id}, Name: {Name}, Status: {Status},\nDescription: {Description},\nDueDate: {DueDate}\n";
    }
}
public enum Status
{
    Pending = 1,
    Done
}