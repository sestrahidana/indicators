// Copyright QUANTOWER LLC. © 2017-2023. All rights reserved.

using System;
using System.Drawing;
using TradingPlatform.BusinessLayer;

namespace Indicator_ATR_Trailing_stop_w_fib_retracements
{
    public class Indicator_ATR_Trailing_stop_w_fib_retracements : Indicator
    {
        [InputParameter("ATR Period", 0, 1, 9999)]
        public int Period = 5;
        [InputParameter("ATR Factor", 0, 0.1, 999, 0.1, 1)]
        public double ATRFactor = 3.5;
        [InputParameter("Type of Moving Average", 3, variants: new object[] {
        "Simple", MaMode.SMA,
        "Exponential", MaMode.EMA,
        "Smoothed Modified", MaMode.SMMA,
        "Linear Weighted", MaMode.LWMA})]
        public MaMode MaType = MaMode.SMA;


        public double fibLvl1 = 1.618, fibLvl2 = 2.618, fibLvl3 = 4.23, fib1, fib2, fib3, fibTgt1, fibTgt2, fibTgt3;
        public double trailStop, trend, trendUp, trendDown, ex, currClose, switchVal, diff, loss;
        public bool trendChange;
        private Indicator atr;
        public override string ShortName => $"ATRTrailStop ({this.Period})";
        public Indicator_ATR_Trailing_stop_w_fib_retracements()
            : base()
        {
            // Defines indicator's name and description.
            Name = "ATR Trailing stop w/ fib retracements";
            Description = "fib retracements on the atr trailing stop";

            // Defines line on demand with particular parameters.
            AddLineSeries("TrailStop", Color.White, 1, LineStyle.Solid);
            AddLineSeries("fibTgtDown1", Color.Red, 1, LineStyle.Solid);
            AddLineSeries("fibTgtDown2", Color.Red, 1, LineStyle.Solid);
            AddLineSeries("fibTgtDown3", Color.Red, 1, LineStyle.Solid);
            AddLineSeries("fibTgtUp1", Color.Green, 1, LineStyle.Solid);
            AddLineSeries("fibTgtUp2", Color.Green, 1, LineStyle.Solid);
            AddLineSeries("fibTgtUp3", Color.Green, 1, LineStyle.Solid);
            // By default indicator will be applied on main window of the chart
            SeparateWindow = false;
        }

        protected override void OnInit()
        {
            this.atr = Core.Indicators.BuiltIn.ATR(this.Period, this.MaType, Indicator.DEFAULT_CALCULATION_TYPE);
            this.AddIndicator(this.atr);
        }

        protected override void OnUpdate(UpdateArgs args)
        {
            double prevClose = currClose;

            currClose = GetPrice(PriceType.Close);
            double currHigh = GetPrice(PriceType.High);
            double currLow = GetPrice(PriceType.Low);

            loss = ATRFactor * atr.GetValue();
            double up = currClose - loss;
            double down = currClose + loss;

            double prevTrend = trend;
            double prevTrendUp = trendUp;
            double prevTrendDown = trendDown;

            trendUp = prevClose > prevTrendUp ? Math.Max(up, prevTrendUp) : up;
            trendDown = prevClose < prevTrendDown ? Math.Min(down, prevTrendDown) : down;
            trend = currClose > prevTrendDown ? 1 : currClose < prevTrendUp ? -1 : prevTrend;

            trailStop = trend == 1 ? trendUp : trendDown;

            double prevEx = ex;
            ex = (trend > 0 && prevTrend < 0) ? currHigh : (trend < 0 && prevTrend > 0) ? currLow : trend == 1 ? Math.Max(prevEx, currHigh) : trend == -1 ? Math.Min(prevEx, currLow) : prevEx;

            fib1 = ex + (trailStop - ex) * fibLvl1 / 100;
            fib2 = ex + (trailStop - ex) * fibLvl2 / 100;
            fib3 = ex + (trailStop - ex) * fibLvl3 / 100;

            double prevSwitchVal = switchVal;
            double prevDiff = diff;
            trendChange = trend == prevTrend ? false : true;
            switchVal = trendChange ? trailStop : prevSwitchVal;
            diff = trendChange ? currClose - switchVal : prevDiff;

            fibTgt1 = switchVal + (diff * fibLvl1);
            fibTgt2 = switchVal + (diff * fibLvl2);
            fibTgt3 = switchVal + (diff * fibLvl3);
            double prevfibTgt1 = fibTgt1;

            SetValue(trailStop);
            if (trend == 1 && fibTgt1 == prevfibTgt1)
            {
                if (fibTgt1 > trailStop)
                    SetValue(fibTgt1, 4);
                if (fibTgt2 > trailStop)
                    SetValue(fibTgt2, 5);
                if (fibTgt3 > trailStop)
                    SetValue(fibTgt3, 6);
            }
            if (trend == -1 && fibTgt1 == prevfibTgt1)
            {
                if (fibTgt1 < trailStop)
                    SetValue(fibTgt1, 1);
                if (fibTgt2 < trailStop)
                    SetValue(fibTgt2, 2);
                if (fibTgt3 < trailStop)
                    SetValue(fibTgt3, 3);
            }
        }
    }
}