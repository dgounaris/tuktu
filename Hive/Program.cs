// See https://aka.ms/new-console-template for more information

using Hive;
using Hive.Commands;

var commandHandlers = new List<ICommandHandler>
{
    new PrintCommandHandler(),
    new PlayCommandHandler(),
    new ImportCommandHandler(),
    new EvaluateCommandHandler(),
    new NewGameCommandHandler(),
    new PassCommandHandler(),
    new ValidMovesCommandHandler(),
    new UndoCommandHandler(),
    new BestMoveCommandHandler()
};

var game = new Game();

while (game.IsGameOver() == -1)
{
    var command = Console.ReadLine();
    try
    {
        var commandId = command?.Split(' ')[0];
        var handler = commandHandlers.FirstOrDefault(c => c.CommandString == commandId);
        if (handler is not null)
        {
            try
            {
                handler.Handle(game, command ?? string.Empty);
                Console.WriteLine("ok");
            }
            catch (Exception ex)
            {
                Console.WriteLine("error");
                Console.WriteLine(ex.Message);
            }
        }
        else
        {
            Console.WriteLine("Invalid command.");
        }
    }
    catch (Exception e)
    {
        Console.WriteLine("Unknown error");
        Console.WriteLine(e.Message);
    }
}

Console.WriteLine("Game is over. Winner: " + (game.IsGameOver() == 0 ? "Black" : "White"));