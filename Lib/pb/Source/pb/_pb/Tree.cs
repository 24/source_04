namespace pb
{
    public enum TreeFilter
    {
        Select         = 0x0000,
        DontSelect     = 0x0001,
        Skip           = 0x0002,
        Stop           = 0x0004
    }

    public enum TreeOpe
    {
        Source = 0,
        Child,
        Siblin,
        Parent
    }

    public class TreeValue<T>
    {
        public int Index;
        public T Value;
        public int Level;
        public TreeOpe TreeOpe;
        public bool Selected;
        public bool Skip;
        public bool Stop;
    }

    public class TraceTreeValue<T>
    {
        public int Index;
        public T Value;
        public int Level;
        public string TreeOpe;
        public bool Selected;
        public bool Skip;
        public bool Stop;
    }
}
