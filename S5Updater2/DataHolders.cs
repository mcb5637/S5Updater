namespace S5Updater2
{
    internal enum Status
    {
        Ok,
        Error,
    }

    internal class Resolution
    {
        internal readonly string Show;
        internal readonly string RegValue;
        internal readonly bool NeedsDev;

        public Resolution(string show, string regValue, bool needsDev)
        {
            Show = show;
            RegValue = regValue;
            NeedsDev = needsDev;
        }

        public override string ToString()
        {
            return Show;
        }
    }

    internal class Language
    {
        internal readonly string Show;
        internal readonly string RegValue;

        public Language(string show, string regValue)
        {
            Show = show;
            RegValue = regValue;
        }

        public override string ToString()
        {
            return Show;
        }
    }

    internal class PlayerColor
    {
        internal readonly string Name;
        internal readonly int Value;

        public PlayerColor(string name, int val)
        {
            Name = name;
            Value = val;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
