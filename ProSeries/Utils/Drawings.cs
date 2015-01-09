using System;
using System.Collections.Generic;
using LeagueSharp;
using LeagueSharp.Common;
using Color = System.Drawing.Color;
namespace ProSeries.Utils
{
    static class Drawings
    {
        private static readonly Dictionary<string, object> RangeCircles = new Dictionary<string, object>();

        static Drawings()
        {
            Drawing.OnDraw += DrawingOnOnDraw;
        }

        private static void DrawingOnOnDraw(EventArgs args)
        {
            foreach (var circle in RangeCircles)
            {
                var c = ProSeries.Config.SubMenu("Drawings").Item(circle.Key, true).GetValue<Circle>();
                var range = 0f;
                if (circle.Value is Spell)
                {
                    range = ((Spell)circle.Value).Range;
                }

                if (c.Active)
                {
                    Utility.DrawCircle(ProSeries.Player.Position, range, c.Color);
                }
            }
        }

        internal static void AddRangeCircle(string name, object spellOrCallBack)
        {
            ProSeries.Config.SubMenu("Drawings").AddItem(new MenuItem(name, name, true).SetValue(new Circle(true, Color.White, 100)));
            RangeCircles.Add(name, spellOrCallBack);
        }
    }
}
