using LeagueSharp;
using LeagueSharp.Common;

namespace ProSeries.Utils.Items
{
    public class Item
    {
        internal virtual int Id { get; set; }

        internal virtual string Name { get; set; }

        internal virtual int Range { get; set; }

        public bool IsActive
        {
            get
            {
                var target = ProSeries.Orbwalker.GetTarget() as Obj_AI_Base;

                return target != null && ProSeries.Player.Distance(target, true) <= Range * Range &&
                       LeagueSharp.Common.Items.CanUseItem(Id) && MenuItem.GetValue<bool>();
            }
        }

        public MenuItem MenuItem { get; private set; }

        public Item CreateMenuItem(Menu parent, string menuItemName)
        {
            MenuItem = parent.AddItem(new MenuItem(menuItemName, "Use " + menuItemName).SetValue(true));

            return this;
        }

        public virtual void Use() {}
    }
}