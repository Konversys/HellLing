using HellLing.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellLing.Model.Intertreter
{
    class Var
    {
        public EType Type { get; private set; }
        public string Title { get; private set; }
        public string Value { get; private set; }
        public bool IsArg { get; private set; }

        public Var(EType type, string title, string value, bool isArg = false)
        {
            Type = type;
            Title = title;
            Value = value;
            IsArg = isArg;
        }

        public void Assign(string newValue)
        {
            Value = newValue;
        }
     }
}
