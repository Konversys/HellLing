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
            return exceptions.Select(x => x.ToString()).ToList();
        }

        public void PrintErrorCode()
        {
            string text = Token.Text;
            for (int i = 0; i < text.Count(); i++)
            {
                Console.ResetColor();
                if (true)
                {

                }
                Console.Write(text[i]);
            }
        }
    }
}
