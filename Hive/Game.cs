namespace Hive;

public class Game
{
    public Board Board;
    private Player[] _players = new Player[2];
    private int _currentPlayerIndex = -1;
    
    public Game()
    {
        Board = new Board();
        _players[0] = new Player();
        _players[1] = new Player();
    }
    
    public void StartGame()
    {
        _currentPlayerIndex = 0;
    }
    
    public void EndTurn()
    {
        _currentPlayerIndex = (_currentPlayerIndex + 1) % 2;
    }

    // returns -1 for no game over, otherwise returns the index of the winning player
    public async Task<int> IsGameOver()
    {
        for (int i = 0; i < _players.Length; i++)
        {
            if (_players[i].Queen.Position != null)
            {
                bool isBeeSurrounded = true;
                foreach (var position in Board.GetSurroundingPositions(_players[i].Queen.Position!))
                {
                    if (Board.Get(position) is not null)
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
}