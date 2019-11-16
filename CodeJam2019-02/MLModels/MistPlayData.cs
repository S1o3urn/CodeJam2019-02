using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ML.Data;

namespace CodeJam2019_02.MLModels
{

    /// <summary>
    /// Holds the data used for prediction
    /// </summary>
    public class MistPlayData
    {
        [LoadColumn(0)]
        public int MinBinAge;

        [LoadColumn(1)]
        public int MaxBinAge;

        [LoadColumn(2)]
        public int CountryID;

        [LoadColumn(3)]
        public int GameInstallYear;

        [LoadColumn(4)]
        public int GameInstallMonth;

        [LoadColumn(5)]
        public int GameInstallDay;

        [LoadColumn(6)]
        public int GameInstallHour;

        [LoadColumn(7)]
        public int Gender;

        [LoadColumn(8)]
        public int OSVersion;

        [LoadColumn(9)]
        public int OSUpdate;

        [LoadColumn(10)]
        public int OSPatch;

        [LoadColumn(11)]
        public int Ref;

        [LoadColumn(12)]
        public int SourceID;

        [LoadColumn(13)]
        public int nShoppingApps;

        [LoadColumn(14)]
        public int nTotalApps;

        [LoadColumn(15), ColumnName("Label")]
        public int AmountSpend;
    }
}
