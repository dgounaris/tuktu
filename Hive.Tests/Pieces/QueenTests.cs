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
}