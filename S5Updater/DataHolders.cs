using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S5Updater
{
    class Resolution
    {
        internal string Show;
        internal string RegValue;
        internal bool NeedsDev;

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

    class Language
    {
        internal string Show;
        internal string RegValue;

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

    class PlayerColor
    {
        internal string Name;
        internal int Value;

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
