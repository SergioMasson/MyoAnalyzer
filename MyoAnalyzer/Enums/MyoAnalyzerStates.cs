namespace MyoAnalyzer.Enums
{
    public enum AppState
    {
        Started,
        Waiting,
        Acquiring,
        Live,
    }

    public enum TrainMethods
    {
        Kernel,
        LinearSVM,
        NeuralNetworks
    }

    public enum Gestures
    {      
        Close = 1,
        Open = 2,
        Rock = 3,
        Like = 4,
        One = 5,
        None = 0
    }  
}
