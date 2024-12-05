namespace Hive;

public record Position
{
    public int Q { get; set; }
    public int R { get; set; }
    
    public Position(int q, int r)
    {
        Q = q;
        R = r;
    }
}