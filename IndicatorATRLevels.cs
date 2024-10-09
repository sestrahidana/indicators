// Copyright QUANTOWER LLC. © 2017-2024. All rights reserved.

using System;
using System.Drawing;
using TradingPlatform.BusinessLayer;

namespace IndicatorATRLevels
{

    public class IndicatorATRLevels : Indicator
    {
        [InputParameter("ATR Period", 0, 1, 9999)]
        public int period = 14;

        private Indicator atr;

        private HistoricalData dailyHistoricalData;
        public override string ShortName => $"ATRLevels ({this.period})";
        public IndicatorATRLevels()
            : base()
        {
            // Defines indicator's name and description.
            Name = "ATR Levels";
            Description = "It takes the Opening Price for the day and add/substract ATR to the price and marks this on the chart.";

            // Defines line on demand with particular parameters.
            AddLineSeries("atr 50", Color.CadetBlue, 1, LineStyle.Solid);
            AddLineSeries("atr 100", Color.Red, 1, LineStyle.Solid);
            AddLineSeries("atr 150", Color.Green, 1, LineStyle.Solid);
            AddLineSeries("atr 200", Color.White, 1, LineStyle.Solid);
            AddLineSeries("ntr 50", Color.CadetBlue, 1, LineStyle.Solid);
            AddLineSeries("ntr 100", Color.Red, 1, LineStyle.Solid);
            AddLineSeries("ntr 150", Color.Green, 1, LineStyle.Solid);
            AddLineSeries("ntr 200", Color.White, 1, LineStyle.Solid);
            // By default indicator will be applied on main window of the chart
            SeparateWindow = false;

        }

        protected override void OnInit()
        {
            this.dailyHistoricalData = this.Symbol.GetHistory(Period.DAY1, Time(Count-1).Date.AddDays(-10));
            this.atr = Core.Indicators.BuiltIn.ATR(this.period, MaMode.SMA, Indicator.DEFAULT_CALCULATION_TYPE);
            this.AddIndicator(this.atr);
        }

        protected override void OnUpdate(UpdateArgs args)
        {
            int index = (int)dailyHistoricalData.GetIndexByTime(Time().Date.AddDays(0).Ticks, SeekOriginHistory.End);
            HistoryItemBar DayBar = (HistoryItemBar)dailyHistoricalData[index];
            double d_atr = atr.GetValue();

            double atr_50 = DayBar.Open + (d_atr * 0.5);
            double atr_100 = DayBar.Open + (d_atr * 1);
            double atr_150 = DayBar.Open + (d_atr * 1.5);
            double atr_200 = DayBar.Open + (d_atr * 2);

            double ntr_50 = DayBar.Open - (d_atr * 0.5);
            double ntr_100 = DayBar.Open - (d_atr * 1);
            double ntr_150 =  DayBar.Open - (d_atr * 1.5);
            double ntr_200 =  DayBar.Open - (d_atr * 2);

            SetValue(atr_50, 0);
            SetValue(atr_100, 1);
            SetValue(atr_150, 2);
            SetValue(atr_200, 3);

            SetValue(ntr_50, 4);
            SetValue(ntr_100, 5);
            SetValue(ntr_150, 6);
            SetValue(ntr_200, 7);
        }
    }
}
