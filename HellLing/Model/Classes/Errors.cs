using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellLing.Model.Classes
{
    class Errors
    {
        public List<Exception> exceptions { get; private set; }
        public Errors()
        {
            exceptions = new List<Exception>();
        }
        public void Add(Token token, int car, string error)
        {
            exceptions.Add(new Exception(token, car, error));
        }
        public List<string> GetErrors()
        {
            return exceptions.Select(x => x.GetLexem()).ToList();
        }

        public void PrintErrorCode()
        {
            string text = Token.Text;
            List<int> colored = GetErrorMass();
            for (int i = 0; i < text.Count(); i++)
            {
                Console.ResetColor();
                if (colored.Contains(i))
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                }
                Console.Write(text[i]);
            }
            Console.ResetColor();
        }

        List<int> GetErrorMass()
        {
            List<int> vs = new List<int>();
            foreach (var item in exceptions)
            {
                for (int i = item.Token.Position.Start-1; i < item.Token.Position.End; i++)
                {
                    if (!vs.Contains(i))
                    {
                        vs.Add(i);
                    }
                }
            }
            return vs;
        }
    }
}
