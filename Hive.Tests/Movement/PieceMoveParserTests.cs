using Hive.Movement;
using Hive.Pieces;

namespace Hive.Tests.Movement;

public class PieceMoveParserTests
{
    private PieceMoveParser _pieceMoveParser;
    
    public PieceMoveParserTests()
    {
        _pieceMoveParser = new PieceMoveParser();
    }
    
    [Theory]
    [InlineData("wB .", typeof(Beetle), 1, true, 0, 0)]
    [InlineData("wB2 .", typeof(Beetle), 2, true, 0, 0)]
    [InlineData("bQ .", typeof(Queen), 1, false, 0, 0)]
    [InlineData("bQ", typeof(Queen), 1, false, 0, 0)]
    public void PieceMoveOnEmptyBoardIsParsed(string move, Type type, int pieceNumber, bool color, int q, int r)
    {
        var board = new Board();
        var (piece, position) = _pieceMoveParser.Parse(board, move);
        Assert.NotNull(piece);
        Assert.Equal(type, piece.GetType());
        Assert.Equal(pieceNumber, piece.PieceNumber);
        Assert.Equal(color, piece.Color);
        Assert.Equal(q, piece.Position!.Q);
        Assert.Equal(r, piece.Position!.R);
    }
    
    [Theory]
    [InlineData("bB1 -wA1", typeof(Beetle), 1, false, -1, -1)]
    [InlineData("bB2 \\wA1", typeof(Beetle), 2, false, 0, -2)]
    [InlineData("bB1 /wA1", typeof(Beetle), 1, false, -1, 0)]
    [InlineData("bB1 wA1/", typeof(Beetle), 1, false, 1, -2)]
    [InlineData("bB1 wA1\\", typeof(Beetle), 1, false, 0, 0)]
    [InlineData("bB1 wA1-", typeof(Beetle), 1, false, 1, -1)]
    [InlineData("bB1 wQ-", typeof(Beetle), 1, false, 2, -2)]
    [InlineData("wB2 -wQ", typeof(Beetle), 2, true, 0, -2)]
    [InlineData("bB2 bG1-", typeof(Beetle), 2, false, 2, -1)]
    [InlineData("wB1 wA1-", typeof(Beetle), 1, true, 1, -1)]
    [InlineData("wB2 wA1", typeof(Beetle), 2, true, 0, -1)]
    public void PieceMoveOnNonEmptyBoardIsParsed(string move, Type type, int pieceNumber, bool color, int q, int r)
    {
        var board = new Board();
        board.GetPiece(true, 'A', 1)!.Position = new Position(0, -1);
        board.GetPiece(false, 'G', 1)!.Position = new Position(1, -1);
        board.GetPiece(true, 'Q', 1)!.Position = new Position(1, -2);
        var (piece, position) = _pieceMoveParser.Parse(board, move);
        Assert.NotNull(piece);
        Assert.Equal(type, piece.GetType());
        Assert.Equal(pieceNumber, piece.PieceNumber);
        Assert.Equal(color, piece.Color);
        Assert.Equal(q, piece.Position!.Q);
        Assert.Equal(r, piece.Position!.R);
    }
    
    [Theory]
    [InlineData("wB2 wA1", typeof(Beetle), 2, true, 0, -1)]
    public void PieceMoveOnTopOfOtherPieceIsParsed(string move, Type type, int pieceNumber, bool color, int q, int r)
    {
        var board = new Board();
        board.GetPiece(true, 'A', 1)!.Position = new Position(0, -1);
        board.GetPiece(true, 'G', 1)!.Position = new Position(1, -1);
        board.GetPiece(true, 'Q', 1)!.Position = new Position(1, -2);
        var (piece, position) = _pieceMoveParser.Parse(board, move);
        Assert.NotNull(piece);
        Assert.Equal(type, piece.GetType());
        Assert.Equal(pieceNumber, piece.PieceNumber);
        Assert.Equal(color, piece.Color);
        Assert.Equal(q, piece.Position!.Q);
        Assert.Equal(r, piece.Position!.R);
    }
}