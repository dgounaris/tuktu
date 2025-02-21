namespace Hive.Commands;

public class NewGameCommandHandler : ICommandHandler
{
    public string CommandString { get; } = "newgame";
    
    public void Handle(Game game, string command)
    {
        var newGameSetup = command.IndexOf(' ') != -1 ?
            command[(command.IndexOf(' ') + 1)..] : string.Empty;
        game.LoadFromUHP(newGameSetup);
        game.PrintUHP();
    }
}