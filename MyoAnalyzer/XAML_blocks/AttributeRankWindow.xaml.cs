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
using MyoAnalyzer.Classification.Extraceter;
using MyoAnalyzer.Classification.Ranker;
using MyoAnalyzer.XAML_blocks.AttributeRankWindowPrefabs;

namespace MyoAnalyzer.XAML_blocks
{
    /// <summary>
    /// Interaction logic for AttributeRankWindow.xaml
    /// </summary>
    public partial class AttributeRankWindow : Window
    {
        private List<Pose> Poses;

        private IExtracter FeatureExtracter;
        

        public AttributeRankWindow(List<Pose> poses)
        {
            bool[] channelsToTrain = new bool[8] {true, true, true, true, true, true, true ,true};
            InitializeComponent();
            FeatureExtracter = new AverageEnergyExtracter(channelsToTrain);
            Channel1Average.Text = "Average " + poses.First().GestureName;
            Channel2Average.Text = "Average " + poses.Last().GestureName;
            Poses = poses;
        }

        private void RankAttributes_Click(object sender, RoutedEventArgs e)
        {

            AttributesPannel.Children.Clear();

            if (string.IsNullOrEmpty(NumberOfAttributes.Text))
            {
                return;
            }

            try
            {
                var numberOfAttributes = 0;

                if (!int.TryParse(NumberOfAttributes.Text, out numberOfAttributes))
                {
                    ErroMassege.Visibility = Visibility.Visible;
                    return;
                }

                ErroMassege.Visibility = Visibility.Hidden;

                List<AttributeRankItem> rankedAttributes = RankAttributes(numberOfAttributes);

                foreach (var rankItem in rankedAttributes)
                {
                    AttributesPannel.Children.Add(rankItem);
                }
             
            }
            catch (FormatException exception)
            {
                ErroMassege.Visibility = Visibility.Visible;
                return;
            }

        }

        private List<AttributeRankItem> RankAttributes(int numberOfAttributes)
        {
            List<AttributeRankItem> AtributeRankList = new List<AttributeRankItem>();

            FeatureRanker FeatureRanker = new FeatureRanker();

            double[][] rawData1 = FeatureExtracter.ExtractFeaturesFromMany(Poses.First());

            double[][] rawData2 = FeatureExtracter.ExtractFeaturesFromMany(Poses.Last());

            foreach (var VARIABLE in FeatureRanker.RankFeatures(rawData1, rawData2, numberOfAttributes))
            {
                AttributeRankItem AttributeRankItem = new AttributeRankItem(VARIABLE[0].ToString(), VARIABLE[1], VARIABLE[2], VARIABLE[3], VARIABLE[4]);

                AtributeRankList.Add(AttributeRankItem);
            }

            return AtributeRankList;


        }        
    }
}
