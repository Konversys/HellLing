using HellLing.Model.Enums;

namespace HellLing.Model.Classes
{
    class Token
    {
        public static string Text { get; set; }
        public Lexem Lexem { get; private set; }
        /// <summary>
        /// Название лексемы
        /// </summary>
        public string Title { get { return Lexem.ToString(); } }
        /// <summary>
        /// Код лексемы
        /// </summary>
        int code;
        public int Code { get { return (int)Lexem; } private set { code = value; } }
        /// <summary>
        /// Значение лексемы
        /// </summary>
        public string State { get; set; }
        /// <summary>
        /// Позиция лексемы
        /// </summary>
        public Position Position { get; set; }
        public int Line { get; set; }
        public int Sumbol { get; set; }


        public Token()
        { }
        public Token(Lexem lexem, Position position)
        {
            State = Text.Substring(position.Start, position.End - position.Start);
            Line = 0;
            int LastN = 0;
            for (int i = 0; i <= position.Start; i++)
            {
                if (Text[i] == '\n')
                {
                    LastN = i;
                    Line++;
                }
            }
            Sumbol = position.Start - LastN;
            Lexem = lexem;
            Position = position;
        }
        public Token(Lexem lexem, Position position, string text, int code)
        {
            State = text;
            Lexem = lexem;
            Position = position;
            Code = code;
            Line = 0;
            int LastN = 0;
            for (int i = 0; i <= position.Start; i++)
            {
                if (Text[i] == '\n')
                {
                    LastN = i;
                    Line++;
                }
            }
            Sumbol = position.Start - LastN;
        }
        public void Set(Lexem lexem, Position position)
        {
            State = Text.Substring(position.Start, position.End - position.Start);
            Lexem = lexem;
            Position = position;
            Line = 0;
            int LastN = 0;
            for (int i = 0; i <= position.Start; i++)
            {
                if (Text[i] == '\n')
                {
                    LastN = i;
                    Line++;
                }
            }
            Sumbol = position.Start - LastN;
        }
        public override string ToString()
        {
            //return $"Title={Title,-13} Code={Code,-4} State={State,-15} Start={Position.Start,-3} End={Position.End,-3} Line={Line} Sumbol={Sumbol}";
            return $" Line={Line,-3} Sumbol={Sumbol,-3} State={State,-15}";
        }
    }
}
