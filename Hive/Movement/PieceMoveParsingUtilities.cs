using Hive.Pieces;

namespace Hive.Movement;

public static class PieceMoveParsingUtilities
{
    public static Tuple<IPiece, Position> Parse(Board board, string move)
    {
        var parts = move.Split(' ');
        var color = parts[0][0] == 'w';
        var pieceId = parts[0][1];
        var pieceNumber = parts[0].Length == 3 ? parts[0][2] - '0' : 1;
        Position newPosition = new Position(0, 0);
        
        if (parts.Length == 1) // possible alternative notation for first move of game
        {
            parts = [parts[0], "."];
        }
        if (parts[1][0] == '.') {}
        else if (parts[1][0] == '\\' || parts[1][0] == '-' || parts[1][0] == '/')
        {
            var otherPieceAttachment = parts[1][0];
            var otherPieceColor = parts[1][1] == 'w';
            var otherPieceId = parts[1][2];
            var otherPieceNumber = parts[1].Length == 4 ? parts[1][3] - '0' : 1;
            var otherPiecePosition = board.GetPiece(otherPieceColor, otherPieceId, otherPieceNumber)!.Position!;
            if (otherPieceAttachment == '-')
            {
                newPosition = MovementUtilities.GetPositionW(otherPiecePosition!);
            }
            else if (otherPieceAttachment == '\\')
            {
                newPosition = MovementUtilities.GetPositionNW(otherPiecePosition!);
            }
            else if (otherPieceAttachment == '/')
            {
                newPosition = MovementUtilities.GetPositionSW(otherPiecePosition!);
            }
        }
        else
        {
            var otherPieceColor = parts[1][0] == 'w';
            var otherPieceId = parts[1][1];
            int otherPieceNumber;
            Position otherPiecePosition;
            if (parts[1].Length == 2) // move on top of single-occurence piece
            {
                otherPieceNumber = 1;
                otherPiecePosition = board.GetPiece(otherPieceColor, otherPieceId, otherPieceNumber)!.Position!;
                newPosition = otherPiecePosition;
            }
            else if (parts[1].Length == 3)
            {
                if (parts[1][2] != '/' && parts[1][2] != '\\' && parts[1][2] != '-') // move on top of multi-occurence piece
                {
                    otherPieceNumber = parts[1][2] - '0';
                    otherPiecePosition = board.GetPiece(otherPieceColor, otherPieceId, otherPieceNumber)!.Position!;
                    newPosition = otherPiecePosition;
                }
                else // move next to single-occurence piece
                {
                    otherPieceNumber = 1;
                    otherPiecePosition = board.GetPiece(otherPieceColor, otherPieceId, otherPieceNumber)!.Position!;
                    if (parts[1][2] == '-')
                    {
                        newPosition = MovementUtilities.GetPositionE(otherPiecePosition!);
                    }
                    else if (parts[1][2] == '\\')
                    {
                        newPosition = MovementUtilities.GetPositionSE(otherPiecePosition!);
                    }
                    else if (parts[1][2] == '/')
                    {
                        newPosition = MovementUtilities.GetPositionNE(otherPiecePosition!);
                    }
                }
            }
            else // move next to multi-occurence piece
            {
                otherPieceNumber = parts[1].Length == 4 ? parts[1][2] - '0' : 1;
                var otherPieceAttachment = parts[1][3];
                otherPiecePosition = board.GetPiece(otherPieceColor, otherPieceId, otherPieceNumber)!.Position!;
                if (otherPieceAttachment == '-')
                {
                    newPosition = MovementUtilities.GetPositionE(otherPiecePosition!);
                }
                else if (otherPieceAttachment == '\\')
                {
                    newPosition = MovementUtilities.GetPositionSE(otherPiecePosition!);
                }
                else if (otherPieceAttachment == '/')
                {
                    newPosition = MovementUtilities.GetPositionNE(otherPiecePosition!);
                }
            }
        }

        var piece = PieceUtilities.ResolvePieceFromId(pieceId);
        piece.PieceNumber = pieceNumber;
        piece.Color = color;
        piece.Position = newPosition;
        return new Tuple<IPiece, Position>(piece, newPosition);
    }

    public static string PositionToMove(Board board, Position position)
    {
        // on empty board
        if (board.GetPiecesOnBoardCount(true) == 0 && board.GetPiecesOnBoardCount(false) == 0)
        {
            return string.Empty;
        }
        // on top of other piece
        if (board.Get(position) is not null)
        {
            return board.Get(position)!.GetPieceIdentifier().ToString();
        }
        else if (board.Get(MovementUtilities.GetPositionSW(position)) is not null)
        {
            return $"{board.Get(MovementUtilities.GetPositionSW(position))!.Print()}/";
        }
        else if (board.Get(MovementUtilities.GetPositionW(position)) is not null)
        {
            return $"{board.Get(MovementUtilities.GetPositionW(position))!.Print()}-";
        }
        else if (board.Get(MovementUtilities.GetPositionNW(position)) is not null)
        {
            return $"{board.Get(MovementUtilities.GetPositionNW(position))!.Print()}\\";
        }
        else if (board.Get(MovementUtilities.GetPositionNE(position)) is not null)
        {
            return $"/{board.Get(MovementUtilities.GetPositionNE(position))!.Print()}";
        }
        else if (board.Get(MovementUtilities.GetPositionE(position)) is not null)
        {
            return $"-{board.Get(MovementUtilities.GetPositionE(position))!.Print()}";
        }
        else if (board.Get(MovementUtilities.GetPositionSE(position)) is not null)
        {
            return $"\\{board.Get(MovementUtilities.GetPositionSE(position))!.Print()}";
        }

        throw new InvalidOperationException($"Could not translate position {position} into valid move");
    }
}