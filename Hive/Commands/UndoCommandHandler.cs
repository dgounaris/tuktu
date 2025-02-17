using Hive.Movement;
using Hive.Pieces;

namespace Hive.Commands;

public class UndoCommandHandler : ICommandHandler
{
    public string CommandString { get; } = "undo";

    public void Handle(Game game, string command)
    {
        try
        {
            var movesToUndo = 1;
            if (command.Split(' ').Length > 1)
            {
                movesToUndo = int.Parse(command.Split(' ')[1]);
            }

            while (movesToUndo > 0)
            {
                game.UndoLastMove();
                movesToUndo--;
            }
            game.PrintUHP();
            Console.WriteLine("ok");
        }
        catch (Exception e)
        {
            Console.WriteLine("Unknown error");
            Console.WriteLine(e.Message);
        }
    }
}