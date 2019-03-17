namespace HellLing.Model.Enums
{
    enum Lexem
    {
        /// <summary>
        /// Идентефикатор
        /// </summary>
        TID = 1,
        /// <summary>
        /// Константа целая
        /// </summary>
        TConstInt,
        /// <summary>
        /// main
        /// </summary>
        _main,
        /// <summary>
        /// for
        /// </summary>
        _for,
        /// <summary>
        /// Константа символьная
        /// </summary>
        TChar,
        /// <summary>
        /// Константа с плавающей точкой
        /// </summary>
        TConstDouble,
        /// <summary>
        /// Константа символьная
        /// </summary>
        TConstChar,
        /// <summary>
        /// =
        /// </summary>
        TSave,
        /// <summary>
        /// ==
        /// </summary>
        TEQ,
        /// <summary>
        /// !=
        /// </summary>
        TNQ,
        /// <summary>
        /// <
        /// </summary>
        TLT,
        /// <summary>
        /// >
        /// </summary>
        TGT,
        /// <summary>
        /// <=
        /// </summary>
        TLE,
        /// <summary>
        /// >=
        /// </summary>
        TGE,
        /// <summary>
        /// +
        /// </summary>
        TPlus,
        /// <summary>
        /// ++
        /// </summary>
        TInc,
        /// <summary>
        /// -
        /// </summary>
        TMinus,
        /// <summary>
        /// --
        /// </summary>
        TDec,
        /// <summary>
        /// *
        /// </summary>
        TMult,
        /// <summary>
        /// /
        /// </summary>
        TDiv,
        /// <summary>
        /// !
        /// </summary>
        TNot,
        /// <summary>
        /// ;
        /// </summary>
        TTZpt,
        /// <summary>
        /// ,
        /// </summary>
        TZpt,
        /// <summary>
        /// .
        /// </summary>
        TDot,
        /// <summary>
        /// (
        /// </summary>
        TLS,
        /// <summary>
        /// )
        /// </summary>
        TRS,
        /// <summary>
        /// {
        /// </summary>
        TFLS,
        /// <summary>
        /// }
        /// </summary>
        TFRS,
        /// <summary>
        /// [
        /// </summary>
        TCLS,
        /// <summary>
        /// ]
        /// </summary>
        TCRS,
        /// <summary>
        /// Пробелы и новая строка
        /// </summary>
        TIgn,
        /// <summary>
        /// Коментарии
        /// </summary>
        TCom,
        /// <summary>
        /// int
        /// </summary>
        _int,
        /// <summary>
        /// double
        /// </summary>
        _double,
        /// <summary>
        /// Константа
        /// </summary>
        _const,
        /// <summary>
        /// Пустой тип
        /// </summary>
        _void,
        /// <summary>
        /// Возврат
        /// </summary>
        _return,
        /// <summary>
        /// Конец файла
        /// </summary>
        TEOF = 300,
        /// <summary>
        /// Ошибка
        /// </summary>
        TError = 403,
        /// <summary>
        /// Начало файла
        /// </summary>
        TSOF = 301,
    }
}
