using Hive.Movement;
using Hive.Pieces;

namespace Hive;

public class Board
{
    private IPiece?[,] _board = new IPiece?[15, 15];
    private int _offset = 7;

    public void Set(IPiece piece)
    {
        if (piece.Position is null)
        {
            throw new ArgumentException("Piece position is null");
        }

        _board[piece.Position.Q + _offset, piece.Position.R + _offset] = piece;
    }

    public IPiece? Get(Position position)
    {
        return _board[position.Q + _offset, position.R + _offset];
    }

    public void Remove(Position position)
    {
        _board[position.Q + _offset, position.R + _offset] = null;
    }
    
    public IPiece? GetPiece(bool color, char pieceId, int pieceNumber)
    {
        foreach (var boardTile in _board)
        {
            if (boardTile is null)
            {
                continue;
            }
            if (boardTile.Color == color && boardTile.GetPieceIdentifier() == pieceId && boardTile.PieceNumber == pieceNumber)
            {
                return boardTile;
            }
        }

        return null;
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
        if (_board[position.Q + _offset, position.R + _offset] is not null)
        {
            return false;
        }
        var existingBoardPieces = _board.Cast<IPiece?>().Count(it => it is not null);
        if (existingBoardPieces < 2)
        {
            return true;
        }
        var surroundingPositions = GetSurroundingPositions(position);
        var surroundingSameColor = 0;
        var surroundingOtherColor = 0;
        foreach (var surroundingPosition in surroundingPositions)
        {
            if (_board[surroundingPosition.Q + _offset, surroundingPosition.R + _offset] is null)
            {
                continue;
            }
            else if (_board[surroundingPosition.Q + _offset, surroundingPosition.R + _offset]?.Color == color)
            {
                surroundingSameColor++;
            }
            else if (_board[surroundingPosition.Q + _offset, surroundingPosition.R + _offset]?.Color != color)
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
        if (!_board.Cast<IPiece?>().Any(it => it is not null))
        {
            Console.WriteLine("Empty board");
            return;
        }
        var westMostPos = _board.Cast<IPiece?>().Where(it => it is not null).MinBy(it => it!.Position!.Q)!.Position!.Q + _offset;
        var northMostPos = _board.Cast<IPiece?>().Where(it => it is not null).MinBy(it => it!.Position!.R)!.Position!.R + _offset;
        var eastMostPos = _board.Cast<IPiece?>().Where(it => it is not null).MaxBy(it => it!.Position!.Q)!.Position!.Q + _offset;
        var southMostPos = _board.Cast<IPiece?>().Where(it => it is not null).MaxBy(it => it!.Position!.R)!.Position!.R + _offset;
        
        for (var r = northMostPos - 2; r <= southMostPos + 2; r++)
        {
            if (Math.Abs(r - _offset) % 2 == 1)
            {
                Console.Write("  ");
            }
            for (var q = westMostPos - 2; q <= eastMostPos + 2; q++)
            {
                if (_board[q, r] is null)
                {
                    Console.Write("  .  ");
                }
                else
                {
                    string colorAsStr;
                    if (_board[q, r]!.Color)
                    {
                        colorAsStr = "w";
                    }
                    else
                    {
                        colorAsStr = "b";
                    }
                    Console.Write($" {colorAsStr}{_board[q, r]!.GetPieceIdentifier()}{_board[q, r]!.PieceNumber} ");
                }
            }
            Console.WriteLine();
        }
    }
}