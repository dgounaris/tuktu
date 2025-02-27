﻿using Hive.Movement;

namespace Hive.Pieces;

public interface IPiece
{
    public Position? Position { get; set; }
    public char GetPieceIdentifier();
    
    // True - white // False - black
    public bool Color { get; set; }
    
    // 1st, 2nd, ... player's piece
    public int PieceNumber { get; set; }

    public IEnumerable<Move> GetValidMoves(Board board);

    public string Print()
    {
        return (Color ? "w" : "b") + GetPieceIdentifier() + PieceNumber;
    }
}