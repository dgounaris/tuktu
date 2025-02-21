namespace Hive.Commands;

public class InfoCommandHandler : ICommandHandler
{
    public string CommandString { get; } = "info";
    public void Handle(Game game, string command)
    {
        Console.WriteLine("id Tuktu v0.1");
    }
}