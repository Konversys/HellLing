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
    }
}
