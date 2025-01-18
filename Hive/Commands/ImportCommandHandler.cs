namespace Hive.Commands;

public class ImportCommandHandler : ICommandHandler
{
    public string CommandString { get; } = "i";

    public void Handle(Game game, string command)
    {
        try
        {
            game.Board.LoadFromNotation(command[(command.IndexOf(' ') + 1)..]);
            game.Board.Print();
        }
        catch (Exception e)
        {
            Console.WriteLine("Unknown error");
            Console.WriteLine(e.Message + "\n" + e.StackTrace);
        }
    }
}