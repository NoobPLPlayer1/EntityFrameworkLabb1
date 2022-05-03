
class ProgramContext
{
    public ProgramContext(Workplace workplace, Worker user)
    {
        Workplace = workplace;
        User = user;
    }

    public Workplace Workplace { get; }
    public Worker User { get; set; }
}