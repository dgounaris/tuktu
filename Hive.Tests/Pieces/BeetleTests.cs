using Hive.Pieces;

namespace Hive.Tests.Pieces;

public class BeetleTests
{
    [Fact]
    public void GetValidMovesOnEmptyBoardSucceeds()
    {
        var board = new Board();

        var moves = board.GetPiece(true, 'B', 1).GetValidMoves(board).ToList();
        
        Assert.Single(moves);
        Assert.Null(moves[0].PreviousPosition);
        Assert.Equal(new Position(0, 0), moves[0].NewPosition);
    }

    [Theory]
    [InlineData(":wbB2+0+0wB2wQ+0-2bA1+0-3bA3+1-1wA2+1-3bA2+2-2wA1+2-3bB1-1-2wB1-2-1***@bG*@bQ**@bS*@wA***@wG**@wS", 6)]
    [InlineData(":wwB2wQ+0-2bA1+0-3wA2+0-4bA2+1-4bB1-1-2bB2-2+0wB1-2-1bA3-2-2wA1-3+0***@bG*@bQ**@bS*@wA***@wG**@wS", 4)]
    [InlineData(":wwQ+0-2wB2+0-3wA2+0-4bA2+1-4bA1-1-1bB1-1-2bB2-2+0wB1-2-1bA3-2-2wA1-3+0***@bG*@bQ**@bS*@wA***@wG**@wS", 0)]
    [InlineData(":wwQ+0-2bA2+0-3wA2+0-4wB2+0-5bA1-1-1bB1-1-2bB2-2+0wB1-2-1bA3-2-2wA1-3+0***@bG*@bQ**@bS*@wA***@wG**@wS", 3)]
    [InlineData(":wwQ+0-2wB2bA2+0-3wA2+0-4bA1-1-1bB1-1-2bB2-2+0wB1-2-1bA3-2-2wA1-3+0***@bG*@bQ**@bS*@wA***@wG**@wS", 6)]
    [InlineData(":wwQ+0-2bB2bA2+0-3wA2+0-4bA1-1-1wB2wB1bB1-1-2bA3-2-2wA1-3-1***@bG*@bQ**@bS*@wA***@wG**@wS", 6)]
    public void GetValidMovesOnBoardSucceeds(string notation, int validMoves)
    {
        var board = new Board();
        board.LoadFromNotation(notation);
        var piece = board.GetPiece(true, 'B', 2);

        var moves = piece.GetValidMoves(board).ToList();
        
        Assert.Equal(validMoves, moves.Count);
    }
}