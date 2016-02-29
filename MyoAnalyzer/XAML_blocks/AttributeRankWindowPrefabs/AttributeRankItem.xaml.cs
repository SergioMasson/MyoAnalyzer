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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MyoAnalyzer.XAML_blocks.AttributeRankWindowPrefabs
{
    /// <summary>
    /// Interaction logic for AttributeRankItem.xaml
    /// </summary>
    public partial class AttributeRankItem : UserControl
    {
        public AttributeRankItem(string channel, double average1, double average2, double std, double totalpoints)
        {
            InitializeComponent();
            Channel.Text = channel;
            Average1.Text = average1.ToString("#.##", System.Globalization.CultureInfo.InvariantCulture); 
            Average2.Text = average2.ToString("#.##", System.Globalization.CultureInfo.InvariantCulture);
            STD.Text = std.ToString("#.##", System.Globalization.CultureInfo.InvariantCulture);
            Total.Text = totalpoints.ToString("#.##", System.Globalization.CultureInfo.InvariantCulture);

        }
    }
}
