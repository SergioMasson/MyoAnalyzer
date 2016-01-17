using System.Windows;

namespace MyoAnalyzer
{
    /// <summary>
    /// Interaction logic for ErroMassege.xaml
    /// </summary>
    public partial class ErroMassege : Window
    {
        private readonly string _massegeToDisplay;

        public ErroMassege(string massege)
        {
            _massegeToDisplay = massege;
            InitializeComponent();
        }

        private void On_Loaded(object sender, RoutedEventArgs e)
        {
            this.ErroText.Text = _massegeToDisplay;
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
