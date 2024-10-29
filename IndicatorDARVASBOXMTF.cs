// Copyright QUANTOWER LLC. © 2017-2023. All rights reserved.

using System;
using System.Drawing;
using TradingPlatform.BusinessLayer;

namespace IndicatorDARVASBOXMTF
{
	public class IndicatorDARVASBOXMTF : Indicator
    {
        [InputParameter("BOX LENGTH", 0, 1, 9999)]
        public int boxp = 5;
        double LL, k1,k1p, k2, k3;
        int countbars = 0;
        public override string ShortName => $"DBOX MTF({this.boxp})";
        public IndicatorDARVASBOXMTF()
            : base()
        {
            // Defines indicator's name and description.
            Name = "DARVAS BOX MTF";
            Description = "My indicator's annotation";

            // Defines line on demand with particular parameters.
            AddLineSeries("DBOX MTF:TBbox", Color.Green, 1, LineStyle.Solid);
            AddLineSeries("DBOX MTF:BBbox", Color.Red, 1, LineStyle.Solid);
            // By default indicator will be applied on main window of the chart
            SeparateWindow = false;
        }

        protected override void OnInit()
        {
            k1 = High();
        }

        protected override void OnUpdate(UpdateArgs args)
        {
            k1p = k1;
            double llp = LL;
            LL = Low();
            k1 = High();
            k2 = High();
            k3 = High();
            for (int i = 0; i<boxp;i++)
            {
                LL = Math.Min(GetPrice(PriceType.Low,i),LL);
                k1 = Math.Max(GetPrice(PriceType.High,i),k1);
                if(i<boxp-1)
                    k2 = Math.Max(GetPrice(PriceType.High, i),k2);
                if (i<boxp-2)
                    k3 = Math.Max(GetPrice(PriceType.High, i),k3);
            }
            if (High() > k1p)
                countbars++;
            else countbars = 0;
            double TopBox = countbars == boxp - 2 && k3 < k2 ? k1 : k1p;
            double BottomBox = countbars == boxp - 2 && k3 < k2 ? LL : llp;
            SetValue(TopBox);
            SetValue(BottomBox,1);
        }
    }
}
