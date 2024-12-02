﻿using Hive.Movement;
using Hive.Pieces;

namespace Hive;

public class Board
{
    private List<IPiece> _pieces = new List<IPiece>();

    public void Set(IPiece piece)
    {
        if (piece.Position is null)
        {
            throw new ArgumentException("Piece position is null");
        }

        _pieces.Add(piece);
    }

    public IPiece? Get(Position position)
    {
        // last or default will come in handy when we have beetles functionality working
        return _pieces.LastOrDefault(it => it.Position.Q == position.Q && it.Position.R == position.R);
    }

    public void Remove(Position position)
    {
        // last will come in handy when we have beetles functionality working
        _pieces.RemoveAt(_pieces.FindLastIndex(it => it.Position == position));
    }
    
    public IPiece? GetPiece(bool color, char pieceId, int pieceNumber)
    {
        return _pieces.SingleOrDefault(it => 
            it.Color == color && it.GetPieceIdentifier() == pieceId && it.PieceNumber == pieceNumber);
    }

    public IEnumerable<Position> GetSurroundingPositions(Position position)
    {
        var positions = new List<Position>
        {
            MovementUtilities.GetPositionSW(position),
            MovementUtilities.GetPositionW(position),
            MovementUtilities.GetPositionNW(position),
            MovementUtilities.GetPositionNE(position),
            MovementUtilities.GetPositionE(position),
            MovementUtilities.GetPositionSE(position)
        };
        return positions;
    }

    public bool IsPositionConnectedToHive(Position position)
    {
        foreach (var iterPosition in GetSurroundingPositions(position))
        {
            if (Get(iterPosition) is not null)
            {
                return true;
            }
        }
        return false;
    }

    public bool IsPositionInitializable(Position position, bool color)
    {
        if (_pieces.Any(it => it.Position!.Q == position.Q && it.Position.R == position.R))
        {
            return false;
        }
        
        var existingBoardPieces = _pieces.Count;
        if (existingBoardPieces < 2)
        {
            return true;
        }
        var surroundingPositions = GetSurroundingPositions(position);
        var surroundingSameColor = 0;
        var surroundingOtherColor = 0;
        foreach (var surroundingPosition in surroundingPositions)
        {
            var pieceOnPosition = _pieces.FirstOrDefault(it =>
                it.Position!.Q == surroundingPosition.Q && it.Position.R == surroundingPosition.R);
            if (pieceOnPosition is null)
            {
                continue;
            }
            else if (pieceOnPosition.Color == color)
            {
                surroundingSameColor++;
            }
            else if (pieceOnPosition.Color != color)
            {
                surroundingOtherColor++;
            }
        }
        return surroundingSameColor > 0 && surroundingOtherColor == 0;
    }

    public bool IsAdjacentPositionSlideReachable(Position originalPosition, Position proposedPosition)
    {
        if (Get(proposedPosition) is not null)
        {
            return false;
        }
        
        var surroundingPositions = GetSurroundingPositions(originalPosition).ToList();
        var indexOfProposedPosition = surroundingPositions.FindIndex(it => it.Q == proposedPosition.Q && it.R == proposedPosition.R);
        
        if (indexOfProposedPosition == -1)
        {
            return false;
        }

        if (Get(surroundingPositions[Math.Max(indexOfProposedPosition - 1, 0) % 6]) is not null && Get(surroundingPositions[(indexOfProposedPosition + 1) % 6]) is not null)
        {
            return false;
        }

        return true;
    }

    public void Print()
    {
        Console.WriteLine("Board state:");
        if (_pieces.Count == 0)
        {
            Console.WriteLine("Empty board");
            return;
        }
        var westMostPos = _pieces.MinBy(it => it!.Position!.Q)!.Position!.Q;
        var northMostPos = _pieces.MinBy(it => it!.Position!.R)!.Position!.R;
        var eastMostPos = _pieces.MaxBy(it => it!.Position!.Q)!.Position!.Q;
        var southMostPos = _pieces.MaxBy(it => it!.Position!.R)!.Position!.R;
        
        for (var r = northMostPos - 2; r <= southMostPos + 2; r++)
        {
            if (Math.Abs(r) % 2 == 1)
            {
                Console.Write("  ");
            }
            for (var q = westMostPos - 2; q <= eastMostPos + 2; q++)
            {
                var pieceOnLocation = _pieces.FirstOrDefault(it => it.Position!.Q == q && it.Position.R == r);
                if (pieceOnLocation is null)
                {
                    Console.Write("  .  ");
                }
                else
                {
                    string colorAsStr;
                    if (pieceOnLocation.Color)
                    {
                        colorAsStr = "w";
                    }
                    else
                    {
                        colorAsStr = "b";
                    }
                    Console.Write($" {colorAsStr}{pieceOnLocation.GetPieceIdentifier()}{pieceOnLocation.PieceNumber} ");
                }
            }
            Console.WriteLine();
        }
    }
}