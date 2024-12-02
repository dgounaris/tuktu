namespace Hive.Commands;

public class PrintCommandHandler : ICommandHandler
{
    public string CommandString { get; } = "p";

    public void Handle(Board board, string command)
    {
        try
        {
            board.Print();
        }
        catch (Exception e)
        {
            Console.WriteLine("Unknown error");
            Console.WriteLine(e.Message);
        }
    }
}