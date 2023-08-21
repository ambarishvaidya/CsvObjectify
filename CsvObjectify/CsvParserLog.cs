namespace CsvObjectify
{
    public static class CsvParserLog
    {
        internal static Action<string> Error;
        internal static Action<string> Warning;
        internal static Action<string> Info;        

        public static void SetLogger(Action<string> error, Action<string> warning, Action<string> info)
        {
            Error = error;
            Warning = warning;
            Info = info;
        }
    }
}
