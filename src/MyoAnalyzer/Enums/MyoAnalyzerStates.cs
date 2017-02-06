namespace MyoAnalyzer.Enums
{
    public enum AppState
    {
        Started,
        Waiting,
        Acquiring,
        Live,
        Streaming
    }  

    public enum Gestures
    {      
        Close = 1,
        Open = 2,
        Rock = 3,
        Like = 4,
        One = 5,
        Pinch = 6,
        None = 0
    }

    public enum StreamStatus
    {
        Streaming,
        Awayting,
        NotReady,
        None
    }
}
