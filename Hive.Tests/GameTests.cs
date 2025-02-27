﻿using System.Diagnostics;
using System.Drawing;
using Hive.Movement;
using Xunit.Abstractions;

namespace Hive.Tests;

public class GameTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public GameTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Theory]
    [InlineData(":wwB2+0+0bG2+0-1bG1+0-2bA1+0-3bB1wB1+1-1wQ+1-2wG2+1-4bA2+2-2wA1+2-3wA2+2-4wG1+3-3wA3+3-4*@bA*@bB*@bG*@bQ**@bS*@wG**@wS", -1)]
    [InlineData(":wwB2+0+0bG2+0-1bG1+0-2bA1+0-3bB1wB1+1-1wQ+1-2wG2+1-4bA2+2-2wA1+2-3wA2+2-4wG1+3-3wA3+3-4bQ+4-4*@bA*@bB*@bG**@bS*@wG**@wS", -1)]
    [InlineData(":wwB2+0+0bG2+0-1bG1+0-2bA1+0-3bB1wB1+1-1wQ+1-2bQ+1-3wG2+1-4bA2+2-2wA1+2-3wG1+3-3wA3+3-4wA2+4-3*@bA*@bB*@bG**@bS*@wG**@wS", 0)]
    [InlineData(":wbG2+0-1wA2+0-2wG1+0-3bB1wB1+1-1wA3+1-2bQ+1-3wG2+1-4wQ+2-1bA2+2-2wA1+2-3wB2+2-4bG1+3-2bA1+3-4*@bA*@bB*@bG**@bS*@wG**@wS", 1)]
    public void IsGameOverSucceeds(string notation, int expectedResult)
    {
        var board = new Board();
        var game = new Game();
        game.Board = board;
        board.LoadFromNotation(notation);
        
        var result = game.IsGameOver();
        
        Assert.Equal(expectedResult, result);
    }

    [Fact]
    public void PrintSucceeds()
    {
        var board = new Board();
        board.Print();
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
        board.GetPiece(true, 'A', 1)!.Position = new Position(0, -1);

        var result = board.IsPositionConnectedToHive(new Position(q, r));
        
        Assert.Equal(expectedResult, result.Count > 0);
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
        board.GetPiece(true, 'A', 1)!.Position = new Position(0, -1);
        board.GetPiece(false, 'A', 1)!.Position = new Position(1, -1);

        var result = board.IsPositionInitializable(new Position(q, r), color);
        
        Assert.Equal(expectedResult, result);
    }
    
    [Theory]
    [InlineData(":wwG1+1-1bG1+2-1***@bA**@bB**@bG*@bQ**@bS***@wA**@wB**@wG*@wQ**@wS",  3, 3)]
    [InlineData(":wwG1+1-1wA1+1-2bG1+2-1bB1+3-2***@bA*@bB**@bG*@bQ**@bS**@wA**@wB**@wG*@wQ**@wS",  5, 5)]
    [InlineData(":wbG1+0+0wG3+0+1wG1+0-1wQ+0-2bB1+1+0wA1+1-2wG2+2-1wS1+3+0bS1+3-1wA2+4-2bS2+5-2bG2+6-3bQ-1+1***@bA*@bB*@bG*@wA**@wB*@wS", 11, 7)]
    [InlineData(":w***@bA**@bB***@bG*@bQ**@bS***@wA**@wB***@wG*@wQ**@wS", 1, 1)]
    [InlineData(":wwQ+0+0***@bA**@bB***@bG*@bQ**@bS***@wA**@wB***@wG**@wS", 6, 6)]
    [InlineData(":wwA3+0+0wG1+0-1bB1+0-2wA2+1+0wA1+1-3bB2+2-1bG1+2-2bG2+2-3wB1+3-2bA1+3-3wQ-1+1wG2-1-1wG3-2+0wS1-2+1**@bA*@bG*@bQ**@bS*@wB*@wS", 11, 2)]
    public void GetInitializablePositionsSucceeds(string boardState, int initializablePositionsWhite, int initializablePositionsBlack)
    {
        var board = new Board();
        board.LoadFromNotation(boardState);

        var resultWhite = board.GetInitializablePositions(true);
        var resultBlack = board.GetInitializablePositions(false);
       
        Assert.Equal(initializablePositionsWhite, resultWhite.Count());
        Assert.Equal(initializablePositionsBlack, resultBlack.Count());
    }

    [Theory]
    [InlineData(":wbQ+0-1wS1+0-2bA1+1-1bG1+2-2wB1-1+0wG1-1-1**@bA**@bB**@bG**@bS***@wA*@wB**@wG*@wQ*@wS", 3, 3, 8, 8)]
    [InlineData(":bbG1+0+0wG3+0+1wG1+0-1wQ+0-2bB1+1+0wA1+1-2wG2+2-1wS1+3+0bS1+3-1wA2+4-2bS2+5-2bG2+6-3bQ-1+1***@bA*@bB*@bG*@wA**@wB*@wS", 7, 6, 4, 5)]
    public void LoadFromNotationSucceeds(string notation, int activePiecesWhite, int activePiecesBlack, int handPiecesWhite, int handPiecesBlack)
    {
        var board = new Board();
        board.LoadFromNotation(notation);
        
        Assert.Equal(activePiecesWhite, board.GetPiecesOnBoardCount(true));
        Assert.Equal(activePiecesBlack, board.GetPiecesOnBoardCount(false));
        Assert.Equal(handPiecesWhite, board.GetPiecesInHandCount(true));
        Assert.Equal(handPiecesBlack, board.GetPiecesInHandCount(false));
    }
    
    [Fact]
    public void LoadFromNotationWithBeetlesOnTopOfPiecesSucceeds()
    {
        var board = new Board();
        board.LoadFromNotation(":wbA1+0-3bB2wA1-1-1wB1wB2bB1-1-2**@bA***@bG*@bQ**@bS**@wA***@wG*@wQ**@wS");
        
        Assert.Equal(3, board.GetPiecesOnBoardCount(true));
        Assert.Equal(3, board.GetPiecesOnBoardCount(false));
        Assert.Equal(8, board.GetPiecesInHandCount(true));
        Assert.Equal(8, board.GetPiecesInHandCount(false));
        
        Assert.Equal('B', board.Get(new Position(-1, -2 ))!.GetPieceIdentifier());
        Assert.Equal(1, board.Get(new Position(-1, -2 ))!.PieceNumber);
        Assert.True(board.Get(new Position(-1, -2 ))!.Color);
        
        board.RemoveFromBoard(new Position(-1, -2 ));
        Assert.Equal('B', board.Get(new Position(-1, -2 ))!.GetPieceIdentifier());
        Assert.Equal(2, board.Get(new Position(-1, -2 ))!.PieceNumber);
        Assert.True(board.Get(new Position(-1, -2 ))!.Color);
        
        board.RemoveFromBoard(new Position(-1, -2 ));
        Assert.Equal('B', board.Get(new Position(-1, -2 ))!.GetPieceIdentifier());
        Assert.Equal(1, board.Get(new Position(-1, -2 ))!.PieceNumber);
        Assert.False(board.Get(new Position(-1, -2 ))!.Color);
    }
    
    [Fact]
    public void UndoLastMoveSucceeds()
    {
        var game = new Game();
        game.StartGame();
        game.PlayMove("wA1 .");
        game.PlayMove("bG1 wA1-");
        game.PlayMove("wB1 \\wA1");
        game.PlayMove("bQ1 bG1\\");
        game.PlayMove("wA2 /wA1");
        
        game.UndoLastMove();
        Assert.Null(game.Board.GetPiece(true, 'A', 2).Position);
        
        game.PlayMove("wA2 /wA1");
        game.PlayMove("bQ1 wA1\\");
        game.PlayMove("wQ1 /wB1");
        game.PlayMove("bG1 -wQ1");
        game.PlayMove("wB1 wA1");
        game.PlayMove("bA1 -bG1");
        
        game.UndoLastMove();
        game.UndoLastMove();
        game.UndoLastMove();
        
        Assert.Equal(-1, game.Board.GetPiece(true, 'B', 1).Position!.R);
        Assert.Equal(0, game.Board.GetPiece(true, 'B', 1).Position!.Q);
        Assert.Equal(0, game.Board.GetPiece(false, 'G', 1).Position!.R);
        Assert.Equal(1, game.Board.GetPiece(false, 'G', 1).Position!.Q);
    }

    [Fact]
    public void PassPlayMoveSucceeds()
    {
        var game = new Game();
        game.StartGame();
        
        game.PlayMove("pass");
        game.PlayMove("bS1");
    }
    
    [Fact]
    public void UndoLastMoveWithEmptyMoveHistorySucceeds()
    {
        var game = new Game();
        var board = new Board();
        game.Board = board;
        
        game.UndoLastMove();
        game.UndoLastMove();
    }
    
    [Theory]
    [InlineData(":wbA1+0-3bB2wA1-1-1wB1wB2bB1-1-2**@bA***@bG*@bQ**@bS**@wA***@wG*@wQ**@wS", true)]
    [InlineData(":wwB1+3-2bA1+3-3wA1+3-4bS1-1-2**@bA**@bB***@bG*@bQ*@bS**@wA*@wB***@wG*@wQ**@wS", false)]
    [InlineData(":wwB1+3-2bB1bA1+3-3wA1+3-4bS1-1-2**@bA*@bB***@bG*@bQ*@bS**@wA*@wB***@wG*@wQ**@wS", false)]
    [InlineData(":wwA2+0-5wQ+1-2wB1+3-2bB1bA1+3-3wA1+3-4bS1-1-2**@bA*@bB***@bG*@bQ*@bS*@wA*@wB***@wG**@wS", false)]
    [InlineData(":wwS1+0-1wA2+0-5wQ+1-2bQ+1-4wA3+1-5wB2+2-3bA2+2-4wB1+3-2bB1bA1+3-3wA1+3-4bG1-1-1bS1-1-2*@bA*@bB**@bG*@bS***@wG*@wS", true)]
    [InlineData(":wwG2+0-1bA1+0-3wG1+1-1wA1+1-3wB1+2-2bB1+2-3wS1-1-1wA2-1-2**@bA*@bB***@bG*@bQ**@bS*@wA*@wB*@wG*@wQ*@wS", true)]
    public void IsHiveConnectedSucceeds(string notation, bool expectedResult)
    {
        var board = new Board();
        board.LoadFromNotation(notation);
        
        var result = board.IsHiveConnected();
        
        Assert.Equal(expectedResult, result);
    }
    
    [Theory]
    [InlineData(":wbA1+0-3bB2wA1-1-1wB1wB2bB1-1-2**@bA***@bG*@bQ**@bS**@wA***@wG*@wQ**@wS", 0, -3, false)]
    [InlineData(":wwS1+0-1wA2+0-5wQ+1-2bQ+1-4wA3+1-5wB2+2-3bA2+2-4wB1+3-2bB1bA1+3-3wA1+3-4bG1-1-1bS1-1-2*@bA*@bB**@bG*@bS***@wG*@wS", 1, -2, true)]
    [InlineData(":wwG2+0-1bA1+0-3wG1+1-1wA1+1-3wB1+2-2bB1+2-3wS1-1-1wA2-1-2**@bA*@bB***@bG*@bQ**@bS*@wA*@wB*@wG*@wQ*@wS", -1, -1, false)]
    public void IsPieceHiveConnectivitySignificantSucceeds(string notation, int qPiece, int rPiece, bool expectedResult)
    {
        var board = new Board();
        board.LoadFromNotation(notation);
        
        var result = board.IsPieceHiveConnectivitySignificant(board.Get(new Position(qPiece, rPiece))!);
        
        Assert.Equal(expectedResult, result);
    }

    [Theory]
    [InlineData("Base;NotStarted;White[1]", 0)]
    [InlineData("Base;InProgress;White[3];wS1;bG1 -wS1;wA1 wS1/;bG2 /bG1", 4)]
    [InlineData("Base", 0)]
    [InlineData("", 0)]
    public void LoadFromUHPSucceeds(string notation, int expectedPiecesOnBoard)
    {
        var game = new Game();
        
        game.LoadFromUHP(notation);
        
        Assert.Equal(expectedPiecesOnBoard, game.Board.GetPiecesOnBoardCount(true) + game.Board.GetPiecesOnBoardCount(false));
    }
    
    [Theory]
    [InlineData(0, -1, 0, 0, false)]
    [InlineData(0, 0, 0, -1, false)]
    [InlineData(2, -1, 1, 0, false)]
    [InlineData(2, -1, 3, -1, false)]
    [InlineData(2, -1, 3, -2, false)]
    [InlineData(2, -1, 2, -2, true)]
    [InlineData(2, -1, 2, 0, true)]
    [InlineData(2, -1, 3, 0, false)]
    [InlineData(-1, -1, 0, -1, true)]
    [InlineData(2, 0, 1, 1, true)]
    public void IsAdjacentPositionSlideReachableSucceeds(int qOrigin, int rOrigin, int qProposed, int rProposed, bool expectedResult)
    {
        var board = new Board();
        board.LoadFromNotation(":wwQ+0+1bG1+1+0wA1+1-1wB1-1+0bA1-1+1**@bA**@bB**@bG*@bQ**@bS**@wA*@wB***@wG**@wS");

        var result = board.IsAdjacentPositionSlideReachable(new Position(qOrigin, rOrigin), new Position(qProposed, rProposed));
        
        Assert.Equal(expectedResult, result);
    }
    
    [Theory]
    [InlineData(1, 4)]
    [InlineData(2, 96)]
    [InlineData(3, 1440)]
    [InlineData(4, 21600)]
    [InlineData(5, 516240)]
    //[InlineData(6, 12219480)]
    //[InlineData(7, 181641900)]
    //[InlineData(8, 2657392800)]
    public void GetAllValidMovesSucceeds(int depth, long expectedMoves, string startingPosition = "")
    {
        var game = new Game();
        if (startingPosition != string.Empty)
        {
            game.LoadFromNotation(startingPosition);
        }
        else
        {
            game.StartGame();
        }

        var result = new List<(string, Move)>();

        var stopwatch = Stopwatch.StartNew();
        result = GetAllValidMovesInternal(depth-1, game);
        stopwatch.Stop();
        
        _testOutputHelper.WriteLine($"Time: {stopwatch}");
        
        Assert.Equal(expectedMoves, result.Count);
    }

    private List<(string gameHistory, Move it)> GetAllValidMovesInternal(int depth, Game game)
    {
        if (depth == 0)
        {
            var gameHistory = game.PrintHistory();
            return game.GetAllValidMoves().Select(it => (gameHistory, it)).ToList();
        }
        
        if (game.IsGameOver() != -1)
        {
            return new List<(string, Move)>();
        }

        var result = new List<(string, Move)>();
        var validMoves = game.GetAllValidMoves();
        
        foreach (var move in validMoves)
        {
            game.TrustedPlayMove(move);
            result.AddRange(GetAllValidMovesInternal(depth - 1, game));
            game.UndoLastMove();
        }

        return result;
    }
}