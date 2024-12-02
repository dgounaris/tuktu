using System.Drawing;
using Hive.Pieces;

namespace Hive.Tests;

public class BoardTests
{
    [Fact]
    public void SetBoardPieceSucceeds()
    {
        var board = new Board();
        var position = new Position(0, -1);
        var piece = new Ant { Position = position, Color = true, PieceNumber = 1 };
        
        board.Set(piece);
        
        Assert.Equal(board.Get(new Position(0, -1)), piece);
    }
    
    [Fact]
    public void SetBoardPieceWithNullPositionFails()
    {
        var board = new Board();
        var piece = new Ant { Position = null, Color = true, PieceNumber = 1 };
        
        Assert.Throws<ArgumentException>(() => board.Set(piece));
    }

    [Fact]
    public void PrintSucceeds()
    {
        var board = new Board();
        board.Print();
    }
    
    [Theory]
    [InlineData(0, 0)]
    public void GetSurroundingPositionsSucceeds(int q, int p)
    {
        var board = new Board();
        var position = new Position(q, p);
        
        var surroundingPositions = board.GetSurroundingPositions(position);
        
        Assert.Equal(6, surroundingPositions.Count());
    }

    [Theory]
    [InlineData(-1, -1, true)]
    [InlineData(0, -2, true)]
    [InlineData(1, -2, true)]
    [InlineData(1, -1, true)]
    [InlineData(0, 0, true)]
    [InlineData(-1, 0, true)]
    [InlineData(-3, 0, false)]
    [InlineData(-1, -2, false)]
    [InlineData(1, 0, false)]
    public void IsPositionConnectedToHiveSucceeds(int q, int r, bool expectedResult)
    {
        var board = new Board();
        board.Set(new Ant { Position = new Position(0, -1), Color = true, PieceNumber = 1 });

        var result = board.IsPositionConnectedToHive(new Position(q, r));
        
        Assert.Equal(expectedResult, result);
    }
    
    [Theory]
    [InlineData(0, -1, true, false)]
    [InlineData(0, -1, false, false)]
    [InlineData(-1, -1, true, true)]
    [InlineData(-1, -1, false, false)]
    [InlineData(0, -2, true, true)]
    [InlineData(1, -1, true, false)]
    [InlineData(0, 0, true, false)]
    [InlineData(-1, 0, true, true)]
    [InlineData(1, -2, false, false)]
    [InlineData(1, -2, true, false)]
    [InlineData(2, -2, false, true)]
    public void IsPositionInitializableSucceeds(int q, int r, bool color, bool expectedResult)
    {
        var board = new Board();
        board.Set(new Ant { Position = new Position(0, -1), Color = true, PieceNumber = 1 });
        board.Set(new Ant { Position = new Position(1, -1), Color = false, PieceNumber = 1 });

        var result = board.IsPositionInitializable(new Position(q, r), color);
        
        Assert.Equal(expectedResult, result);
    }

    [Theory]
    [InlineData(0, -1, 0, 0, false)]
    [InlineData(0, 0, 0, -1, false)]
    [InlineData(2, -1, 1, 0, false)]
    [InlineData(2, -1, 3, -1, true)]
    [InlineData(2, -1, 3, -2, true)]
    [InlineData(2, -1, 2, -2, true)]
    [InlineData(2, -1, 2, 0, true)]
    [InlineData(2, -1, 3, 0, false)]
    [InlineData(-1, -1, 0, -1, true)]
    [InlineData(2, 0, 1, 1, true)]
    public void IsAdjacentPositionSlideReachableSucceeds(int qOrigin, int rOrigin, int qProposed, int rProposed, bool expectedResult)
    {
        var board = new Board();
        board.Set(new Beetle { Position = new Position(-1, 0), Color = true, PieceNumber = 1 });
        board.Set(new Ant { Position = new Position(-1, 1), Color = false, PieceNumber = 1 });
        board.Set(new Queen { Position = new Position(0, 1), Color = false, PieceNumber = 1 });
        board.Set(new Grasshopper { Position = new Position(1, 0), Color = false, PieceNumber = 1 });
        board.Set(new Ant { Position = new Position(1, -1), Color = false, PieceNumber = 1 });

        var result = board.IsAdjacentPositionSlideReachable(new Position(qOrigin, rOrigin), new Position(qProposed, rProposed));
        
        Assert.Equal(expectedResult, result);
    }
}