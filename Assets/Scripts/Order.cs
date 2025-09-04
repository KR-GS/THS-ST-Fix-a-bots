[System.Serializable]
public class Order
{
    public string customername;
    public string robotname;

    public bool needsTool;
    public bool needsPaint;
    public bool needsWire;

    public bool toolDone;
    public bool paintDone;
    public bool wireDone;

    public bool IsComplete()
    {
        if (needsTool && !toolDone) return false;
        if (needsPaint && !paintDone) return false;
        if (needsWire && !wireDone) return false;
        return true;
    }
}
