using LeagueSharp.Common;

namespace ProSeries.Utils.Items
{
    public class Item
    {
        public bool IsActive
        {
            get { return MenuItem.GetValue<bool>(); }
        }

        public MenuItem MenuItem { get; private set; }

        public Item CreateMenuItem(Menu parent, string menuItemName)
        {
            MenuItem = parent.AddItem(new MenuItem(menuItemName, "Use " + menuItemName).SetValue(true));
            return this;
        }

        public virtual void Use()
        {
            
        }
    }
}
