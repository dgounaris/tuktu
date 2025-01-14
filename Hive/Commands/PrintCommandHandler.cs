namespace Hive.Commands;

public class PrintCommandHandler : ICommandHandler
{
    public string CommandString { get; } = "p";

    public void Handle(Game game, string command)
    {
        try
        {
            game.Board.Print();
        }
        catch (Exception e)
        {
            Console.WriteLine("Unknown error");
            Console.WriteLine(e.Message);
        }
    }
}