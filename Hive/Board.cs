using System.Text.RegularExpressions;
using Hive.Movement;
using Hive.Pieces;

namespace Hive;

public class Board
{
    private static Regex _notationRegex =
        new Regex(":(w|b)((?:w|b)(?:A|G|B|Q|S)(?:1|2|3)?(?:(?:\\+|-)\\d+)?(?:(?:\\+|-)\\d+)?)*((?:\\*)*@(?:w|b)(?:A|G|B|Q|S))*", RegexOptions.Compiled);
    private static Regex _pieceInBoardRegex = new Regex("(w|b)(A|G|B|Q|S)(1|2|3)?((?:\\+|-)\\d+)?((?:\\+|-)\\d+)?", RegexOptions.Compiled);
    private static Regex _pieceInHandRegex = new Regex("(\\*)*@(w|b)(A|G|B|Q|S)", RegexOptions.Compiled);
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

    public bool IsPositionConnectedToHive(Position position)
    {
        foreach (var iterPosition in MovementUtilities.GetSurroundingPositions(position))
        {
            if (Get(iterPosition) is not null)
            {
                return true;
            }
        }
        return false;
    }

    // todo test this
    public IEnumerable<Position> GetInitializablePositions(bool color)
    {
        var positions = new List<Position>();
        if (_pieces.Count == 0)
        {
            positions.Add(new Position(0, 0));
            return positions;
        }

        foreach (var piece in _pieces)
        {
            var surroundingPositions = MovementUtilities.GetSurroundingPositions(piece.Position!);
            foreach (var surroundingPosition in surroundingPositions)
            {
                if (IsPositionInitializable(surroundingPosition, color))
                {
                    positions.Add(surroundingPosition);
                }
            }
        }
        return positions;
    }

    public bool IsPositionInitializable(Position position, bool color)
    {
        if (_pieces.Any(it => it.Position!.Q == position.Q && it.Position.R == position.R))
        {
            return false;
        }
        
        var existingBoardPieces = _pieces.Count;
        if (existingBoardPieces == 0)
        {
            return position.Q == 0 && position.R == 0;
        }
        if (existingBoardPieces == 1)
        {
            return true;
        }
        var surroundingPositions = MovementUtilities.GetSurroundingPositions(position);
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
        // todo check if during slide, hive gets split
        if (Get(proposedPosition) is not null)
        {
            return false;
        }
        
        var surroundingPositions = MovementUtilities.GetSurroundingPositions(originalPosition).ToList();
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

    public void LoadFromNotation(string notation)
    {
        var notationMatch = _notationRegex.Match(notation);
        if (!notationMatch.Success)
        {
            throw new ArgumentException("Invalid notation structure");
        }

        var piecesInBoard = notationMatch.Groups[2].Captures.Select(it => it.Value);
        var piecesInHand = notationMatch.Groups[3].Captures.Select(it => it.Value);

        foreach (var pieceInBoard in piecesInBoard)
        {
            var pieceInBoardMatch = _pieceInBoardRegex.Match(pieceInBoard);
            var piece = PieceUtilities.ResolvePieceFromId(pieceInBoardMatch.Groups[2].Value[0]);
            piece.Color = pieceInBoardMatch.Groups[1].Value == "w";

            piece.PieceNumber = string.IsNullOrWhiteSpace(pieceInBoardMatch.Groups[3].Value)
                ? 1
                : pieceInBoardMatch.Groups[3].Value[0] - '0';
            piece.Position = new Position(int.Parse(pieceInBoardMatch.Groups[4].Value), int.Parse(pieceInBoardMatch.Groups[5].Value));
            
            Set(piece);
        }
        
        // todo handle pieces in hand
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

            int qOffset = 0;
            if (r < 0)
            {
                qOffset = (r-1) / 2;
            }
            else if (r > 0)
            {
                qOffset = r / 2;
            }
            for (var q = westMostPos - 2 - qOffset; q <= eastMostPos + 2 - qOffset; q++)
            {
                var pieceOnLocation = _pieces.LastOrDefault(it => it.Position!.Q == q && it.Position.R == r);
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