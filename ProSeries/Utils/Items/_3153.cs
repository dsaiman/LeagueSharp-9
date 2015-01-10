using LeagueSharp;
using LeagueSharp.Common;

namespace ProSeries.Utils.Items
{
    internal class _3153 : Item
    {
        internal override int Id
        {
            get { return 3153; }
        }

        internal override string Name
        {
            get { return "Blade of the Ruined King"; }
        }

        internal override int Range
        {
            get { return 450; }
        }

        public override void Use()
        {
            var target = ProSeries.Orbwalker.GetTarget() as Obj_AI_Base;

            if (target is Obj_AI_Hero &&
                ProSeries.Player.Health + ProSeries.Player.GetItemDamage(target, Damage.DamageItems.Botrk) <
                ProSeries.Player.MaxHealth)
            {
                LeagueSharp.Common.Items.UseItem(Id, target);
            }
        }
    }
}