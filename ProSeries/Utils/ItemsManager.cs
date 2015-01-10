using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LeagueSharp;
using LeagueSharp.Common;
using ProSeries.Utils.Items;

namespace ProSeries.Utils
{
    public static class ItemsManager
    {
        private static readonly List<Item> Items = new List<Item>();
        private static Menu ItemsSubMenu { get { return ProSeries.Config.SubMenu("Items"); } }

        static ItemsManager()
        {
            Game.OnGameUpdate += Game_OnGameUpdate;
        }

        public static void Initialize()
        {
            const string @namespace = "ProSeries.Utils.Items";
            var q = from t in Assembly.GetExecutingAssembly().GetTypes()
                    where t.IsClass && t.Name != "Item" && t.Namespace == @namespace
                    select t;
            q.ToList().ForEach(t => LoadItem((Item) Activator.CreateInstance(t), t.Name));
        }

        static void Game_OnGameUpdate(EventArgs args)
        {
            Items.Where(item => item.IsActive).ToList().ForEach(item => item.Use());
        }

        public static void LoadItem(Item item, string name)
        {
            Items.Add(item.CreateMenuItem(ItemsSubMenu, name));
        }
    }
}
