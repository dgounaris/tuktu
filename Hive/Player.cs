using Hive.Pieces;

namespace Hive;

public class Player
{
    public Queen Queen;
    public Beetle[] Beetles = new Beetle[2];
    public Grasshopper[] Grasshoppers = new Grasshopper[3];
    public Spider[] Spiders = new Spider[2];
    public Ant[] Ants = new Ant[3];
    
    public Player()
    {
        Queen = new Queen();
        for (var i = 0; i < 2; i++)
        {
            Beetles[i] = new Beetle();
        }
        for (var i = 0; i < 3; i++)
        {
            Grasshoppers[i] = new Grasshopper();
        }
        for (var i = 0; i < 2; i++)
        {
            Spiders[i] = new Spider();
        }
        for (var i = 0; i < 3; i++)
        {
            Ants[i] = new Ant();
        }
    }
}