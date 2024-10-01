// Copyright QUANTOWER LLC. © 2017-2024. All rights reserved.
using System;
using System.Drawing;
using TradingPlatform.BusinessLayer;

namespace IndicatorVolumeWeightedMovingAverage
{
	public class IndicatorVolumeWeightedMovingAverage : Indicator
    {
        [InputParameter("Period", 0, 1, 999, 1, 0)]
        public int Period = 14;
        [InputParameter("Sources prices", 1, variants: new object[]{
        "Close", PriceType.Close,
        "Open", PriceType.Open,
        "High", PriceType.High,
        "Low", PriceType.Low
        })]
        public PriceType SourcePrice = PriceType.Close;
        public override string ShortName => $"VWMA ({Period},{SourcePrice})";
        public IndicatorVolumeWeightedMovingAverage()
            : base()
        {
            Name = "Volume Weighted Moving Average";
            Description = "My indicator's annotation";

            AddLineSeries("VWMA", Color.CadetBlue, 2, LineStyle.Solid);

            SeparateWindow = false;
        }
        protected override void OnInit()
        {
        }
        protected override void OnUpdate(UpdateArgs args)
        {
            if (Period > Count)
                return;
            double average = 0;
            double weights = 0;
            for (int i = 0; i < Period; i++)
            {
                average += Volume(i) * GetPrice(SourcePrice, i);
                weights += Volume(i);
            }
            if (weights != 0)
                average = average / weights;
            SetValue(average);
        }
    }
}
