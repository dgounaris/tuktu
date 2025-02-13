﻿using Hive.Movement;
using Hive.Pieces;

namespace Hive.Commands;

public class ValidMovesCommandHandler : ICommandHandler
{
    public string CommandString { get; } = "validmoves";

    public void Handle(Game game, string command)
    {
        try
        {
            var validMoves = game.GetAllValidMoves()
                .Select(it => $"{it.Piece.Print()} {PieceMoveParsingUtilities.PositionToMove(game.Board, it.NewPosition)}".Trim());
            Console.WriteLine($"{string.Join(';', validMoves)}");
            Console.WriteLine("ok");
        }
        catch (Exception e)
        {
            Console.WriteLine("Unknown error");
            Console.WriteLine(e.Message);
        }
    }
}