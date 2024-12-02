// See https://aka.ms/new-console-template for more information

using Hive;
using Hive.Commands;

var commandHandlers = new List<ICommandHandler>
{
    new PrintCommandHandler(),
    new MoveCommandHandler()
};

var game = new Game();

while (await game.IsGameOver() == -1)
{
    var command = Console.ReadLine();
    var commandId = command?[0].ToString();
    var handler = commandHandlers.FirstOrDefault(c => c.CommandString == commandId);
    if (handler is not null)
    {
        handler.Handle(game.Board, command ?? string.Empty);
    }
    else
    {
        Console.WriteLine("Invalid command.");
    }
}