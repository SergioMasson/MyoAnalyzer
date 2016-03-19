using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MyoAnalyzer.Classification;


namespace MyoAnalyzer.XAML_blocks
{
    /// <summary>
    /// Interaction logic for ClassificationTestWindow.xaml
    /// </summary>
    public partial class ClassificationTestWindow : Window
    {

        public ITrainner Trainner;

        public ClassificationTestWindow(ITrainner trainner)
        {
            Trainner = trainner;

            InitializeComponent();
            GesturesComboBox.Items.Add("Open");
            GesturesComboBox.Items.Add("Close");
            GesturesComboBox.Items.Add("Rock`n Roll");
            GesturesComboBox.Items.Add("Like");
            GesturesComboBox.Items.Add("One");
        }

        private void LoadPose_Click(object sender, RoutedEventArgs e)
        {
            XAML_blocks.GesturePanel newGesture = new XAML_blocks.GesturePanel(GesturesComboBox.Items[GesturesComboBox.SelectedIndex].ToString());

            GesturesPanel.Children.Add(newGesture);
        }
    }
}
