// Copyright QUANTOWER LLC. © 2017-2023. All rights reserved.

using System;
using System.Drawing;
using TradingPlatform.BusinessLayer;

namespace IndicatorSimpleMovingAverageSlope
{
	public class IndicatorSimpleMovingAverageSlope : Indicator
    {
        [InputParameter("Window", 0, 1, 9999)]
        public int window = 20;

        [InputParameter("Threshold(cloud)", 0, 0.1, 999, 0.1, 1)]
        public double threshold = 0.6;
        public double sma_slope,smap, normalized_sma_slope;
        Indicator sma, sd;
        private Trend currentTrend, currentTrend2;
	
	public override string ShortName => $"SMAS ({window}; {threshold})";
        public IndicatorSimpleMovingAverageSlope()
            : base()
        {
            Name = "Simple Moving Average Slope";
            Description = "The Simple Moving Average Slope indicator is a technical analysis tool designed to help traders detect the direction and strength of the current trend in the price of an asset.";

            AddLineLevel(0, "0'Line", Color.Blue, 1, LineStyle.Solid);
            AddLineLevel(0.6, "Up'Line", Color.Blue, 1, LineStyle.Solid);
            AddLineLevel(-0.6, "Down'Line", Color.Blue, 1, LineStyle.Solid);
            AddLineSeries("SMAS", Color.Blue, 1, LineStyle.Solid);
            AddLineSeries("upCloud", Color.Blue, 1, LineStyle.Solid);
            AddLineSeries("downCloud", Color.Blue, 1, LineStyle.Solid);
	    LinesSeries[1].Visible = false;
	    LinesSeries[2].Visible = false;

            SeparateWindow = true;
        }

        protected override void OnInit()
        {
            this.sma = Core.Indicators.BuiltIn.SMA(this.window,PriceType.Close);
            this.AddIndicator(this.sma);
	    this.sd = Core.Indicators.BuiltIn.SD(this.window, PriceType.Close,MaMode.SMA);
	    this.AddIndicator(this.sd);
            smap = sma.GetValue();
        }

        protected override void OnUpdate(UpdateArgs args)
        {
	    if (args.Reason == UpdateReason.NewTick)
                return;
            sma_slope = sma.GetValue() - smap;
            normalized_sma_slope = sma_slope / (sd.GetValue()/window);
            smap = sma.GetValue();
            SetValue(normalized_sma_slope);
            SetValue(threshold, 1);
            SetValue(-threshold, 2);
            var newTrend = normalized_sma_slope > threshold ? Trend.Up :Trend.Unknown;
            if (currentTrend != newTrend)
            {
                EndCloud(0, 1, GetColorByTrend(currentTrend));
                BeginCloud(0, 1, GetColorByTrend(newTrend));
            }
            var newTrend2 = normalized_sma_slope < -threshold ? Trend.Down : Trend.Unknown;
            if (currentTrend2 != newTrend2)
            {
                EndCloud(0, 2, GetColorByTrend(currentTrend2));
                BeginCloud(0, 2, GetColorByTrend(newTrend2));
            }
            currentTrend = newTrend;
            currentTrend2 = newTrend2;
            
        }
        private Color GetColorByTrend(Trend trend) => trend switch
        {
            Trend.Up => Color.Green,
            Trend.Down => Color.Red,
            _ => Color.Empty
        };

        private enum Trend
        {
            Unknown,
            Up,
            Down
        }
    }

}
