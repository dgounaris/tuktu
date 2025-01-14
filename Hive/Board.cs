using System.Collections;
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

    private List<IPiece> _pieces;

    public Board()
    {
        _pieces = InitializePieces();
    }

    /*public void Set(IPiece piece)
    {
        if (piece.Position is null)
        {
            throw new ArgumentException("Piece position is null");
        }

        _pieces.Add(piece);
    }*/

    public int GetPiecesOnBoardCount(bool color)
    {
        return _pieces.Count(it => it.Color == color && it.Position is not null);
    }

    public int GetPiecesInHandCount(bool color)
    {
        return _pieces.Count(it => it.Color == color && it.Position is null);
    }
    
    public IPiece? Get(Position position)
    {
        return _pieces.LastOrDefault(it => it.Position == position);
    }
    
    public List<IPiece> GetAll(Position position)
    {
        return _pieces.Where(it => it.Position == position).ToList();
    }
    
    public List<IPiece> GetAll(bool color)
    {
        return _pieces.Where(it => it.Color == color).ToList();
    }

    public void Remove(Position position)
    {
        _pieces.RemoveAt(_pieces.FindLastIndex(it => it.Position == position));
    }
    
    public IPiece? GetPiece(bool color, char pieceId, int pieceNumber)
    {
        return _pieces.SingleOrDefault(it => 
            it.Color == color && it.GetPieceIdentifier() == pieceId && it.PieceNumber == pieceNumber);
    }

    public List<Position> IsPositionConnectedToHive(Position position)
    {
        var connectedPositions = new List<Position>();
        foreach (var iterPosition in MovementUtilities.GetSurroundingPositions(position))
        {
            if (Get(iterPosition) is not null)
            {
                connectedPositions.Add(iterPosition);
            }
        }
        return connectedPositions;
    }

    // todo test this
    public bool IsPieceHiveConnectivitySignificant(IPiece piece)
    {
        var idx = _pieces.FindIndex(it => piece == it);
        _pieces.Remove(piece);
        var result = IsHiveConnected();
        _pieces.Insert(idx, piece);
        return !result;
    }

    public bool IsHiveConnected()
    {
        var onBoardPieces = _pieces.Where(it => it.Position is not null).ToList();
        if (onBoardPieces.Count == 0)
        {
            return true;
        }

        var lookupPieces = new Queue<IPiece>();
        var discoveredPieces = new List<IPiece>();
        lookupPieces.Enqueue(onBoardPieces.First());
        while (lookupPieces.Count > 0)
        {
            var pivot = lookupPieces.Dequeue();
            foreach (var surroundingPlace in MovementUtilities.GetSurroundingPositions(pivot.Position!))
            {
                var piecesOnSurroundingPlace = GetAll(surroundingPlace);
                if (!piecesOnSurroundingPlace.Any() || discoveredPieces.Contains(piecesOnSurroundingPlace.First()))
                {
                    continue;
                }
                foreach (var pieceOnSurroundingPlace in piecesOnSurroundingPlace)
                {
                    discoveredPieces.Add(pieceOnSurroundingPlace);
                }
                lookupPieces.Enqueue(piecesOnSurroundingPlace.First());
            }
        }
        
        return discoveredPieces.Count == onBoardPieces.Count;
    }

    public IEnumerable<Position> GetInitializablePositions(bool color)
    {
        var onBoardPieces = _pieces.Where(it => it.Position is not null).ToList();

        var positions = new List<Position>();
        if (onBoardPieces.Count == 0)
        {
            positions.Add(new Position(0, 0));
            return positions;
        }

        foreach (var piece in onBoardPieces.Where(it => it.Position is not null))
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
        return positions.Distinct();
    }

    public bool IsPositionInitializable(Position position, bool color)
    {
        var onBoardPieces = _pieces.Where(it => it.Position is not null).ToList();

        if (onBoardPieces.Any(it => it.Position!.Q == position.Q && it.Position.R == position.R))
        {
            return false;
        }
        
        var existingBoardPieces = onBoardPieces.Count;
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
            var pieceOnPosition = onBoardPieces.FirstOrDefault(it =>
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
        
        var surroundingPositions = MovementUtilities.GetSurroundingPositions(originalPosition).ToList();
        var indexOfProposedPosition = surroundingPositions.FindIndex(it => it.Q == proposedPosition.Q && it.R == proposedPosition.R);
        
        if (indexOfProposedPosition == -1)
        {
            return false;
        }

        if (Get(surroundingPositions[Math.Max(indexOfProposedPosition + 5, 0) % 6]) is not null && Get(surroundingPositions[(indexOfProposedPosition + 1) % 6]) is not null)
        {
            return false;
        }
        
        // when a piece slides, it should still be able to touch at least one piece it was previously touching, otherwise it's disconnect-reconnect for hive
        var adjacentPositionsWithPieces = surroundingPositions.Where(it => Get(it) is not null);
        var candidateAdjacentPositionsWithPieces = MovementUtilities.GetSurroundingPositions(proposedPosition)
            .Where(it => Get(it) is not null);
        return adjacentPositionsWithPieces.Intersect(candidateAdjacentPositionsWithPieces).Any();
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

        var onTopPieceStack = new Stack<IPiece>();
        foreach (var pieceInBoard in piecesInBoard)
        {
            var pieceInBoardMatch = _pieceInBoardRegex.Match(pieceInBoard);
            var piece = GetPiece(
                pieceInBoardMatch.Groups[1].Value == "w",
                pieceInBoardMatch.Groups[2].Value[0],
                string.IsNullOrWhiteSpace(pieceInBoardMatch.Groups[3].Value)
                    ? 1
                    : pieceInBoardMatch.Groups[3].Value[0] - '0')!;
            
            if (string.IsNullOrWhiteSpace(pieceInBoardMatch.Groups[4].Value)) // piece on top of piece
            {
                onTopPieceStack.Push(piece);
            }
            else
            {
                piece.Position = new Position(int.Parse(pieceInBoardMatch.Groups[4].Value),
                    int.Parse(pieceInBoardMatch.Groups[5].Value));
                while (onTopPieceStack.Count > 0)
                {
                    var onTopPiece = onTopPieceStack.Pop();
                    onTopPiece.Position = piece.Position;
                    // shuffle items to make sure stacking works
                    _pieces.Remove(onTopPiece);
                    _pieces.Add(onTopPiece);
                }
            }
        }

        // todo test if resetting a board by notation works (re-nulls positions etc)
        foreach (var pieceInHand in piecesInHand)
        {
            var pieceInHandMatch = _pieceInHandRegex.Match(pieceInHand);
            int matchAsteriskCount = 0;
            foreach (var _ in pieceInHandMatch.Groups[1].Captures)
            {
                matchAsteriskCount++;
                var piece = GetPiece(
                    pieceInHandMatch.Groups[2].Value == "w",
                    pieceInHandMatch.Groups[3].Value[0],
                    _pieces.Where(it => it.Position is not null)
                        .Count(it => it.Color == (pieceInHandMatch.Groups[2].Value == "w") &&
                                     it.GetPieceIdentifier() == pieceInHandMatch.Groups[3].Value[0]) + matchAsteriskCount);
                piece.Position = null;
            }
        }
    }

    public void Print()
    {
        Console.WriteLine("Board state:");
        var piecesOnBoard = _pieces.Where(it => it.Position is not null).ToList();
        if (piecesOnBoard.Count == 0)
        {
            Console.WriteLine("Empty board");
            return;
        }
        var westMostPos = piecesOnBoard.MinBy(it => it!.Position!.Q)!.Position!.Q;
        var northMostPos = piecesOnBoard.MinBy(it => it!.Position!.R)!.Position!.R;
        var eastMostPos = piecesOnBoard.MaxBy(it => it!.Position!.Q)!.Position!.Q;
        var southMostPos = piecesOnBoard.MaxBy(it => it!.Position!.R)!.Position!.R;
        
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
        
        // todo print stacked sequences
    }

    private List<IPiece> InitializePieces()
    {
        var pieces = new List<IPiece>();
        for (int coloridx = 0; coloridx <= 1; coloridx++)
        {
            pieces.Add(
                new Queen
                {
                    Color = coloridx == 0,
                    PieceNumber = 1
                }
            );
            for (var i = 1; i <= 2; i++)
            {
                pieces.Add(new Beetle()
                    {
                        Color = coloridx == 0,
                        PieceNumber = i
                    }
                );
            }

            for (var i = 1; i <= 3; i++)
            {
                pieces.Add(new Grasshopper()
                    {
                        Color = coloridx == 0,
                        PieceNumber = i
                    }
                );
            }

            for (var i = 1; i <= 2; i++)
            {
                pieces.Add(new Spider()
                    {
                        Color = coloridx == 0,
                        PieceNumber = i
                    }
                );
            }

            for (var i = 1; i <= 3; i++)
            {
                pieces.Add(new Ant()
                    {
                        Color = coloridx == 0,
                        PieceNumber = i
                    }
                );
            }
        }

        return pieces;
    }
}