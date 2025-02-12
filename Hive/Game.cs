using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Hive.Movement;
using Hive.Pieces;

[assembly:InternalsVisibleTo("Hive.Tests")]

namespace Hive;

public class Game
{
    private static Regex _notationRegex =
        new Regex(":(w|b)((?:w|b)(?:A|G|B|Q|S)(?:1|2|3)?(?:(?:\\+|-)\\d+)?(?:(?:\\+|-)\\d+)?)*((?:\\*)*@(?:w|b)(?:A|G|B|Q|S))*", RegexOptions.Compiled);
    public Board Board;
    private bool currentPlayerColor = true;
    private int _currentTurn = -1;
    private Stack<Move> MoveHistory = new ();
    
    public Game()
    {
        Board = new Board();
    }

    public void StartGame()
    {
        if (_currentTurn > -1)
        {
            throw new InvalidOperationException("Game already started");
        }
        
        currentPlayerColor = true;
        _currentTurn = 1;
    }
    
    public string PrintHistory()
    {
        return string.Join(", ", MoveHistory.Reverse().Select(it => $"{it.Piece.Print()} {it.PreviousPosition} {it.NewPosition}"));
    }
    
    public void PlayMove(string move)
    {
        if (_currentTurn == -1)
        {
            throw new InvalidOperationException("Game not started");
        }
        
        var (parsedPiece, parsedPosition) = PieceMoveParsingUtilities.Parse(Board, move);
        var boardPiece = Board.GetPiece(parsedPiece.Color, parsedPiece.GetPieceIdentifier(), parsedPiece.PieceNumber);
        var moveObj = new Move
        {
            Piece = boardPiece,
            PreviousPosition = boardPiece.Position,
            NewPosition = parsedPosition,
            MoveString = move
        };
        if (boardPiece.Color != currentPlayerColor)
        {
            throw new InvalidOperationException("Invalid move: piece color does not match current player");
        }
        if (GetAllValidMoves().Contains(moveObj) is false)
        {
            throw new InvalidOperationException($"Invalid move: piece {parsedPiece.Print()}, new position {parsedPosition}");
        }
        
        MoveHistory.Push(moveObj);
        
        Board.Set(boardPiece, parsedPosition);
        currentPlayerColor = !currentPlayerColor;
        _currentTurn++;
    }
    
    internal void TrustedPlayMove(Move move)
    {
        MoveHistory.Push(move);

        if (move.MoveType != MoveType.Pass)
        {
            Board.Set(move.Piece, move.NewPosition);
        }

        currentPlayerColor = !currentPlayerColor;
        _currentTurn++;
    }
    
    internal void TrustedPlayMove(IPiece piece, Position newPosition)
    {
        var moveObj = new Move
        {
            Piece = piece,
            PreviousPosition = piece.Position,
            NewPosition = newPosition,
            MoveString = $"{piece.Print()} {piece.Position} {newPosition}"
        };
        
        MoveHistory.Push(moveObj);
        
        Board.Set(piece, newPosition);
        currentPlayerColor = !currentPlayerColor;
        _currentTurn++;
    }
    
    public void UndoLastMove()
    {
        var popSucceeded = MoveHistory.TryPop(out var historyMove);
        if (!popSucceeded)
        {
            return;
        }

        if (historyMove!.MoveType != MoveType.Pass)
        {
            Board.Set(historyMove.Piece, historyMove.PreviousPosition);
        }

        currentPlayerColor = !currentPlayerColor;
        _currentTurn--;
    }

    public void Print()
    {
        Board.Print();
        Console.WriteLine("Board notation representation: " + ParseToNotation());
    }

    public void PrintUHP()
    {
        if (_currentTurn == -1)
        {
            throw new InvalidOperationException("Game not started");
        }
        var printText = string.Join(';',
            "Base",
            _currentTurn > 1 ? "InProgress" : "NotStarted",
            _currentTurn % 2 == 1 ? $"White[{_currentTurn %2}]" : $"Black[{_currentTurn %2}]");
        if (MoveHistory.Count != 0)
        {
            printText += $";{string.Join(';', MoveHistory.Reverse().Select(it => it.MoveString).ToArray())}";
        }
        Console.WriteLine(printText);
    }

    public string ParseToNotation()
    {
        return $":{(currentPlayerColor ? "w" : "b")}{Board.ParseToNotation()}";
    }
    
    public void LoadFromNotation(string notation)
    {
        var notationMatch = _notationRegex.Match(notation);
        if (!notationMatch.Success)
        {
            throw new ArgumentException("Invalid notation structure");
        }
        currentPlayerColor = notation[1] == 'w';
        Board.LoadFromNotation(notationMatch.Value);
    }

    public void LoadFromUHP(string uhpCommand)
    {
        var commandParts = uhpCommand.Split(';');
        if (commandParts[0] != "Base" && commandParts[0] != string.Empty)
        {
            throw new InvalidOperationException($"Unsupported game type {commandParts[0]}");
        }

        StartGame();

        if (commandParts.Length >= 3)
        {
            var parsedCurrentPlayerColor = commandParts[2].StartsWith("White");
            var parsedCurrentTurn = int.Parse(commandParts[2].Substring(
                commandParts[2].IndexOf('[') + 1,
                commandParts[2].IndexOf(']') - commandParts[2].IndexOf('[') - 1));
            foreach (var commandPart in commandParts.Skip(3))
            {
                PlayMove(commandPart);
            }

            if ((_currentTurn+1)/2 != parsedCurrentTurn) // +1 to avoid floating point inaccuracy
            {
                throw new InvalidOperationException($"Invalid turn passed in command: {parsedCurrentTurn}");
            }

            if (parsedCurrentPlayerColor != currentPlayerColor)
            {
                throw new InvalidOperationException($"Invalid active player passed in command: {parsedCurrentPlayerColor}");
            }
        }
    }

    // returns -1 for no game over, otherwise returns the index of the winning player
    public int IsGameOver()
    {
        for (int i = 0; i <= 1; i++)
        {
            if (Board.GetPiece(i == 0, 'Q', 1)!.Position != null)
            {
                bool isBeeSurrounded = true;
                foreach (var position in MovementUtilities.GetSurroundingPositions(Board.GetPiece(i == 0, 'Q', 1)!.Position!))
                {
                    if (Board.Get(position) is null)
                    {
                        isBeeSurrounded = false;
                    }
                }
                if (isBeeSurrounded)
                {
                    return i;
                }
            }
        }
        return -1;
    }
    
    public List<Move> GetAllValidMoves()
    {
        var allValidMoves = new List<Move>();
        
        if (IsGameOver() != -1)
        {
            return allValidMoves;
        }

        // can't play queen as first move
        if (Board.GetPiecesOnBoardCount(currentPlayerColor) == 0)
        {
            var uninitializedPieces = Board.GetAll(currentPlayerColor).Where(it => it.Position is null && it.GetPieceIdentifier() != 'Q')
                .OrderBy(it => it.PieceNumber) // prioritize lower piece number
                .DistinctBy(it => it.GetPieceIdentifier());
            foreach (var piece in uninitializedPieces)
            {
                allValidMoves.AddRange(piece.GetValidMoves(Board).ToList());
            }

            return allValidMoves;
        }

        if (Board.GetPiece(currentPlayerColor, 'Q', 1).Position is null)
        {
            if (Board.GetPiecesOnBoardCount(currentPlayerColor) == 3) // must play queen on 4th move
            {
                var queen = Board.GetPiece(currentPlayerColor, 'Q', 1);
                allValidMoves.AddRange(queen.GetValidMoves(Board).ToList());
                return allValidMoves;
            }
            else // can't move pieces before placing queen
            {
                var uninitializedPieces = Board.GetAll(currentPlayerColor).Where(it => it.Position is null)
                    .OrderBy(it => it.PieceNumber) // prioritize lower piece number
                    .DistinctBy(it => it.GetPieceIdentifier());
                ;
                foreach (var piece in uninitializedPieces)
                {
                    allValidMoves.AddRange(piece.GetValidMoves(Board).ToList());
                }

                return allValidMoves;
            }
        }
        
        foreach (var piece in Board.GetAll(currentPlayerColor).Where(it => it.Position is not null))
        {
            allValidMoves.AddRange(piece.GetValidMoves(Board).ToList());
        }
        
        foreach (var piece in Board.GetAll(currentPlayerColor).Where(it => it.Position is null)
                     .OrderBy(it => it.PieceNumber) // prioritize lower piece number
                     .DistinctBy(it => it.GetPieceIdentifier()))
        {
            allValidMoves.AddRange(piece.GetValidMoves(Board).ToList());
        }

        if (allValidMoves.Count == 0)
        {
            allValidMoves.Add(new Move { MoveType = MoveType.Pass });
        }
        
        return allValidMoves;
    }
}