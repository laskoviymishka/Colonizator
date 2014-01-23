namespace GameLogic.Game
{
    public class ToasterUpdateArgs
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public ToastType Type { get; set; }
    }

    public enum ToastType
    {
        Info = 0,
        Warning = 1,
        Error = 2,
        Success = 3
    }
}