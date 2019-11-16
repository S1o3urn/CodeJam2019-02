using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ML.Data;

namespace CodeJam2019_02.MLModels
{

    /// <summary>
    /// Holds the Prediction on consumer spending
    /// </summary>
    public class MistPlayDataPrediction
    {
        [ColumnName("Score")]
        public int AmountSpend;
    }
}
