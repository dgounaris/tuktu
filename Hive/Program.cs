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
                Console.WriteLine("err");
                Console.WriteLine(ex.Message);
                Console.WriteLine("ok");
            }
        }
        else
        {
            Console.WriteLine("err Invalid command.");
            Console.WriteLine("ok");
        }
    }
    catch (Exception e)
    {
        Console.WriteLine("err Invalid command format.");
        Console.WriteLine("ok");
    }
}

Console.WriteLine("Game is over. Winner: " + (game.IsGameOver() == 0 ? "Black" : "White"));