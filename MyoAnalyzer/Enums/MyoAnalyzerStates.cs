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
        Open = 1,
        Close,
        Rock,
        Like,
        One,
        None = 0
    }  
}
