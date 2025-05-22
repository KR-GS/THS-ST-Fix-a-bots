using System.Collections.Generic;
public class Sequence
{
    public int Start { get; private set; }
    public int Difference { get; private set; }
    public List<int> Terms { get; private set; }

    public Sequence(int start, int diff)
    {
        Start = start;
        Difference = diff;
        Terms = new List<int>();
        for (int i = 0; i < 6; i++)
            Terms.Add(start + i * diff);
    }
}