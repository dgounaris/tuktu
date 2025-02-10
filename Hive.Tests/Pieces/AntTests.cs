using Hive.Pieces;

namespace Hive.Tests.Pieces;

public class AntTests
{
    [Fact]
    public void GetValidMovesOnEmptyBoardSucceeds()
    {
        var board = new Board();
        var piece = board.GetPiece(true, 'A', 1);

        var moves = piece.GetValidMoves(board).ToList();
        
        Assert.Single(moves);
        Assert.Null(moves[0].PreviousPosition);
        Assert.Equal(new Position(0, 0), moves[0].NewPosition);
    }

    [Theory]
    [InlineData(":wbA1+0-3wA1-1-1bB1-1-2wB1-2-1**@bA*@bB***@bG*@bQ**@bS**@wA*@wB***@wG*@wQ**@wS", 9)]
    public void GetValidMovesOnBoardSucceeds(string notation, int validMoves)
    {
        var board = new Board();
        board.LoadFromNotation(notation);
        var piece = board.GetPiece(true, 'A', 1);
        var positionBefore = piece.Position;

        var moves = piece.GetValidMoves(board).ToList();
        
        Assert.Equal(validMoves, moves.Count);
        Assert.Equal(positionBefore, piece.Position);
    }
}