using LeagueSharp;

namespace ProSeries.Utils.Items
{
    class BOTRK : Item
    {
        private int Range { get { return 650; }}

        public override void Use()
        {
            Game.PrintChat("Kappa");
        }
    }
}
