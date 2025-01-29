using System.Collections;
using System.Collections.Immutable;
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

    public void Set(IPiece piece, Position? p)
    {
        var selectedPiece = _pieces.Single(it => it == piece);
        _pieces.Remove(selectedPiece);
        selectedPiece.Position = p;
        _pieces.Add(selectedPiece);
    }

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

    public void RemoveFromBoard(Position position)
    {
        Get(position)!.Position = null;
    }
    
    public IPiece GetPiece(bool color, char pieceId, int pieceNumber)
    {
        return _pieces.SingleOrDefault(it => 
            it.Color == color && it.GetPieceIdentifier() == pieceId && it.PieceNumber == pieceNumber) ??
               throw new ArgumentException($"Piece not found: {color} {pieceId} {pieceNumber}");
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

    public bool IsPieceHiveConnectivitySignificant(IPiece piece)
    {
        var position = piece.Position;
        piece.Position = null;
        var result = IsHiveConnected();
        piece.Position = position;
        return !result;
    }

    public bool IsHiveConnected()
    {
        var onBoardPieces = _pieces.Where(it => it.Position is not null)
            // stacked pieces can be considered as one piece
            .GroupBy(it => it.Position)
            .Select(it => it.Last())
            .ToList();
        if (onBoardPieces.Count == 0)
        {
            return true;
        }

        var lookupPieces = new Queue<IPiece>();
        var discoveredPieces = new List<IPiece>();
        lookupPieces.Enqueue(onBoardPieces.First());
        discoveredPieces.Add(onBoardPieces.First());
        while (lookupPieces.Count > 0)
        {
            var pivot = lookupPieces.Dequeue();
            foreach (var surroundingPlace in MovementUtilities.GetSurroundingPositions(pivot.Position!))
            {
                var pieceOnSurroundingPlace = Get(surroundingPlace);
                if (pieceOnSurroundingPlace is null || discoveredPieces.Contains(pieceOnSurroundingPlace))
                {
                    continue;
                }
                discoveredPieces.Add(pieceOnSurroundingPlace);
                lookupPieces.Enqueue(pieceOnSurroundingPlace);
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

        foreach (var piece in onBoardPieces)
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
            var pieceOnPosition = onBoardPieces.LastOrDefault(it =>
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
        
        var backupList = _pieces.ToImmutableList();
        try
        {
            _pieces.ForEach(it => it.Position = null);

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

            foreach (var pieceInHand in piecesInHand)
            {
                var pieceInHandMatch = _pieceInHandRegex.Match(pieceInHand);
                int matchAsteriskCount = 0;
                foreach (var _ in pieceInHandMatch.Groups[1].Captures)
                {
                    matchAsteriskCount++;
                }

                var piecesOfTypeOnBoard = _pieces.Where(it => it.Position is not null &&
                                                              it.Color == (pieceInHandMatch.Groups[2].Value == "w") &&
                                                              it.GetPieceIdentifier() ==
                                                              pieceInHandMatch.Groups[3].Value[0]).ToList();
                var piecesOfType = _pieces.Where(it => it.Color == (pieceInHandMatch.Groups[2].Value == "w") &&
                                                       it.GetPieceIdentifier() == pieceInHandMatch.Groups[3].Value[0]).ToList();
                if (piecesOfTypeOnBoard.Count + matchAsteriskCount != piecesOfType.Count)
                {
                    throw new ArgumentException(
                        $"Too many pieces ({piecesOfTypeOnBoard.Count + matchAsteriskCount}) of type {pieceInHandMatch.Groups[3].Value[0]} and color {pieceInHandMatch.Groups[2].Value == "w"}");
                }
            }
        }
        catch (Exception e)
        {
            _pieces = backupList.ToList();
            Console.WriteLine(e);
            throw;
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
                var pieceOnLocation = _pieces.LastOrDefault(it => it.Position?.Q == q && it.Position?.R == r);
                if (pieceOnLocation is null)
                {
                    Console.Write("  .  ");
                }
                else
                {
                    Console.Write($" {pieceOnLocation.Print()} ");
                }
            }
            Console.WriteLine();
        }

        var stackedPiecesList = piecesOnBoard.GroupBy(it => it.Position).Where(it => it.Count() > 1)
            .Select(it => it.Reverse().ToList()).ToList();
        if (stackedPiecesList.Count > 0)
        {
            Console.WriteLine("Stacked pieces sequences:");
            foreach (var stackedPieces in stackedPiecesList)
            {
                Console.Write($"{stackedPieces.First().Print()}");
                foreach (var piece in stackedPieces.Skip(1))
                {
                    Console.Write($" -> {piece.Print()}");
                }
                Console.WriteLine();
            }
        }

        var whitePiecesOnHand = _pieces.Where(it => it.Position is null && it.Color).ToList();
        var blackPiecesOnHand = _pieces.Where(it => it.Position is null && !it.Color).ToList();
        Console.WriteLine("White pieces on hand:");
        foreach (var piece in whitePiecesOnHand)
        {
            Console.Write($"{piece.Print()}  ");
        }
        Console.WriteLine("\nBlack pieces on hand:");
        foreach (var piece in blackPiecesOnHand)
        {
            Console.Write($"{piece.Print()}  ");
        }
        Console.WriteLine();
    }

    public string ParseToNotation()
    {
        var groupedBoardPiecesByPosition = _pieces.Where(it => it.Position is not null).GroupBy(it => it.Position).ToList();
        var groupedHandPiecesByType = _pieces.Where(it => it.Position is null).GroupBy(it => $"{(it.Color ? "w" : "b")}{it.GetPieceIdentifier()}");

        var notation = "";
        
        foreach (var boardPiecesGroup in groupedBoardPiecesByPosition)
        {
            foreach (var piece in boardPiecesGroup.Reverse())
            {
                notation += piece.Print();
            }
            notation += $"{boardPiecesGroup.First().Position!.Q:+#;-#;+0}{boardPiecesGroup.First().Position!.R:+#;-#;+0}";
        }
        
        foreach (var handPiecesGroup in groupedHandPiecesByType)
        {
            foreach (var handPiece in handPiecesGroup)
            {
                notation += "*";
            }
            notation += $"@{handPiecesGroup.Key}";
        }
        
        return notation;
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
                    PieceNumber = 1,
                    Position = null
                }
            );
            for (var i = 1; i <= 2; i++)
            {
                pieces.Add(new Beetle()
                    {
                        Color = coloridx == 0,
                        PieceNumber = i,
                        Position = null
                    }
                );
            }

            for (var i = 1; i <= 3; i++)
            {
                pieces.Add(new Grasshopper()
                    {
                        Color = coloridx == 0,
                        PieceNumber = i,
                        Position = null
                    }
                );
            }

            for (var i = 1; i <= 2; i++)
            {
                pieces.Add(new Spider()
                    {
                        Color = coloridx == 0,
                        PieceNumber = i,
                        Position = null
                    }
                );
            }

            for (var i = 1; i <= 3; i++)
            {
                pieces.Add(new Ant()
                    {
                        Color = coloridx == 0,
                        PieceNumber = i,
                        Position = null
                    }
                );
            }
        }

        return pieces;
    }
}