using System.Text.RegularExpressions;
using Hive.Movement;
using Hive.Pieces;

namespace Hive;

public class Game
{
    private static Regex _notationRegex =
        new Regex(":(w|b)((?:w|b)(?:A|G|B|Q|S)(?:1|2|3)?(?:(?:\\+|-)\\d+)?(?:(?:\\+|-)\\d+)?)*((?:\\*)*@(?:w|b)(?:A|G|B|Q|S))*", RegexOptions.Compiled);
    public Board Board;
    private bool currentPlayerColor = true;
    private int _currentTurn = -1;
    private Stack<(IPiece, Position?, Position)> MoveHistory = new ();
    
    public Game()
    {
        Board = new Board();
    }

    public void StartGame()
    {
        currentPlayerColor = true;
        _currentTurn = 0;
    }
    
    public void PlayMove(string move)
    {
        var (parsedPiece, parsedPosition) = PieceMoveParsingUtilities.Parse(Board, move);
        var boardPiece = Board.GetPiece(parsedPiece.Color, parsedPiece.GetPieceIdentifier(), parsedPiece.PieceNumber);
        if (boardPiece.GetValidMoves(Board).Contains(parsedPosition) is false) // todo should check against 'board.GetAllValidMoves' instead
        {
            throw new InvalidOperationException($"Invalid move: piece {parsedPiece.Print()}, new position {parsedPosition}");
        }
        
        MoveHistory.Push((boardPiece, boardPiece.Position, parsedPosition));
        Board.Set(boardPiece, parsedPosition);
        currentPlayerColor = !currentPlayerColor;
        _currentTurn++;
    }
    
    // todo use trustedPlayMove method instead of using Board set/unset
    internal void TrustedPlayMove(string move)
    {
        var (parsedPiece, parsedPosition) = PieceMoveParsingUtilities.Parse(Board, move);
        var boardPiece = Board.GetPiece(parsedPiece.Color, parsedPiece.GetPieceIdentifier(), parsedPiece.PieceNumber);
        
        MoveHistory.Push((boardPiece, boardPiece.Position, parsedPosition));
        Board.Set(boardPiece, parsedPosition);
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
        var (piece, previousPosition, _) = historyMove;
        Board.Set(piece, previousPosition);
        currentPlayerColor = !currentPlayerColor;
        _currentTurn--;
    }

    public void Print()
    {
        Board.Print();
        Console.WriteLine("Board notation representation: " + ParseToNotation());
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
    
    public Dictionary<IPiece, List<Position>> GetAllValidMoves()
    {
        Dictionary<IPiece, List<Position>> allValidMoves = new Dictionary<IPiece, List<Position>>();

        // can't play queen as first move
        if (Board.GetPiecesOnBoardCount(currentPlayerColor) == 0)
        {
            var uninitializedPieces = Board.GetAll(currentPlayerColor).Where(it => it.Position is null && it.GetPieceIdentifier() != 'Q')
                .DistinctBy(it => it.GetPieceIdentifier());
            foreach (var piece in uninitializedPieces)
            {
                allValidMoves.Add(piece, piece.GetValidMoves(Board).ToList());
            }

            return allValidMoves;
        }

        if (Board.GetPiece(currentPlayerColor, 'Q', 1).Position is null)
        {
            if (Board.GetPiecesOnBoardCount(currentPlayerColor) == 3) // must play queen on 4th move
            {
                var queen = Board.GetPiece(currentPlayerColor, 'Q', 1);
                allValidMoves.Add(queen, queen.GetValidMoves(Board).ToList());
                return allValidMoves;
            }
            else // can't move pieces before placing queen
            {
                var uninitializedPieces = Board.GetAll(currentPlayerColor).Where(it => it.Position is null)
                    .DistinctBy(it => it.GetPieceIdentifier());
                ;
                foreach (var piece in uninitializedPieces)
                {
                    allValidMoves.Add(piece, piece.GetValidMoves(Board).ToList());
                }

                return allValidMoves;
            }
        }
        else if (IsGameOver() != -1)
        {
            return allValidMoves;
        }
        
        foreach (var piece in Board.GetAll(currentPlayerColor))
        {
            allValidMoves.Add(piece, piece.GetValidMoves(Board).ToList());
        }

        return allValidMoves;
    }
}