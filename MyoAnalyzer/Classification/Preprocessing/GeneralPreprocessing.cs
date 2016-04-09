using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyoAnalyzer.Classification.Preprocessing
{
    public class GeneralPreprocessing
    {
        private readonly List<IPreprocessor> _preprocessesToApply;

        public GeneralPreprocessing(List<PreprocessingType> preprocessingTypes)
        {
            _preprocessesToApply = new List<IPreprocessor>();

            foreach (var preprocessing in preprocessingTypes)
            {
                if (preprocessing == PreprocessingType.Demodulation)
                {
                    var preproces = new Demodulation();
                    _preprocessesToApply.Add(preproces);
                }
                else
                {
                    var preproces = new Demodulation();
                    _preprocessesToApply.Add(preproces);
                }
            }
        }

        public List<double[]> Process(List<double[]> data)
        {
            var model = data;

            foreach (var preprocess in _preprocessesToApply)
            {
                model = preprocess.Apply(model);
            }

            return model;
        } 
    }
}
