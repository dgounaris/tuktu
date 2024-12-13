using Hive.Pieces;

namespace Hive.Tests.Pieces;

public class QueenTests
{
    [Fact]
    public void GetValidMovesOnEmptyBoardSucceeds()
    {
        var board = new Board();
        var piece = new Queen { Color = true, PieceNumber = 1 };

        var moves = piece.GetValidMoves(board).ToList();
        
        Assert.Single(moves);
        Assert.Equal(new Position(0, 0), moves[0]);
    }

    [Theory]
    [InlineData(":wwQ+0-2bA1+0-3wA1-1-1bB1-1-2wB1-2-1**@bA*@bB***@bG*@bQ**@bS**@wA*@wB***@wG**@wS", 2)]
    [InlineData(":wbA2+0-1wQ+0-2bA1+0-3wA1+1-2bB1-1-2wB1-2-1*@bA*@bB***@bG*@bQ**@bS**@wA*@wB***@wG**@wS", 0)]
    [InlineData(":wbA2+0-1wQ+0-2bA1+0-3wA1+1-2bB2+1-3bB1-1-2wB1-2-1*@bA***@bG*@bQ**@bS**@wA*@wB***@wG**@wS", 0)]
    [InlineData(":wwQ+0-2bA1+0-3wA1+1-2bB2+1-3bA2+2-2bB1-1-2wB1-2-1*@bA***@bG*@bQ**@bS**@wA*@wB***@wG**@wS", 2)]
    [InlineData(":wwQ+0-2bB2+1-1wA1+1-2bA2+2-2bA1+2-3bB1-1-2wB1-2-1*@bA***@bG*@bQ**@bS**@wA*@wB***@wG**@wS", 0)]
    [InlineData(":wbG1+0-2bB2+1-1wA1+1-2bA2+2-2bA1+2-3bB1-1-2wQ-1-3wB1-2-1*@bA**@bG*@bQ**@bS**@wA*@wB***@wG**@wS", 2)]
    [InlineData(":wbB2+0-1wQ+0-2bA1+0-3bA3+1-1bA2+2-2wA1+2-3bB1-1-2wB1-2-1***@bG*@bQ**@bS**@wA*@wB***@wG**@wS", 0)]
    [InlineData(":wbB2+0+0wQ+0-2bA1+0-3bA3+1-1wA2+1-3bA2+2-2wA1+2-3bB1-1-2wB1-2-1***@bG*@bQ**@bS*@wA*@wB***@wG**@wS", 2)]
    [InlineData(":wbB2+0+0wB2wQ+0-2bA1+0-3bA3+1-1wA2+1-3bA2+2-2wA1+2-3bB1-1-2wB1-2-1***@bG*@bQ**@bS*@wA***@wG**@wS", 0)]
    public void GetValidMovesOnBoardSucceeds(string notation, int validMoves)
    {
        var board = new Board();
        board.LoadFromNotation(notation);
        var piece = board.GetPiece(true, 'Q', 1);

        var moves = piece.GetValidMoves(board).ToList();
        
        Assert.Equal(validMoves, moves.Count);
    }
}