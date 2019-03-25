using HellLing.Model.Enums;

namespace HellLing.Model.Classes
{
    class Token
    {
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

        public Token()
        { }
        public Token(Lexem lexem, Position position, string text)
        {
            State = text.Substring(position.Start, position.End - position.Start);
            Lexem = lexem;
            Position = position;
        }
        public Token(Lexem lexem, Position position, string text, int code)
        {
            State = text;
            Lexem = lexem;
            Position = position;
            Code = code;
        }
        public void Set(Lexem lexem, Position position, string text)
        {
            State = text.Substring(position.Start, position.End - position.Start);
            Lexem = lexem;
            Position = position;
        }
        public override string ToString()
        {
            return $"Title={Title,-13} Code={Code,-4} State={State,-15} Start={Position.Start,-3} End={Position.End,-3}";
        }
    }
}
