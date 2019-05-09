using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellLing.Model.Classes
{
    class Exception
    {
        public Exception(Token token, int car, string error = "NULLABLE")
        {
            Token = token;
            Error = error;
            Car = car;
        }

        public Token Token { get; private set; }
        public string Error { get; private set; }
        public int Car { get; private set; }

        public override string ToString()
        {
            return $"{Token.ToString()} : Токен {Car}: {Error}";
        }

        public string GetLexem()
        {
            return $"{Token.GetLexem()} : Токен {Car}: {Error}";
        }
    }
}
