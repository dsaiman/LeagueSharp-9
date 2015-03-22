using System;
using LeagueSharp;
using LeagueSharp.Common;

namespace ProSeries.Utils.Items
{
    internal class _3042 : Item
    {
        public _3042()
        {
            Obj_AI_Base.OnProcessSpellCast += OnSpellCast;
        }

        internal override string Name
        {
            get { return "Muramana"; }
        }

        internal override int Id
        {
            get { return Game.MapId == GameMapId.CrystalScar ? 3043 : 3042; }
        }

        internal static bool CanMuramana;
        internal static string[] ValidList =
        {
            "ezrealmysticshot", "ezrealtrueshotbarrage", "ezrealarcaneshift",
            "urgotheatseekinglineqqmissile", "urgotheatseekingmissile", "volley",
            "lucianq", "lucianr"
        };

        public override void OnUpdate()
        {
            if (!CanMuramana)
            {
                var manamune = ProSeries.Player.GetSpellSlot("Muramana");
                if (manamune != SpellSlot.Unknown && ProSeries.Player.HasBuff("Muramana"))
                {
                    ProSeries.Player.Spellbook.CastSpell(manamune);
                }
            }
        }

        public override void Use()
        {
            if (CanMuramana)
            {
                var manamune = ProSeries.Player.GetSpellSlot("Muramana");
                if (manamune != SpellSlot.Unknown && !ProSeries.Player.HasBuff("Muramana"))
                {
                    ProSeries.Player.Spellbook.CastSpell(manamune);
                    Utility.DelayAction.Add(400, () => CanMuramana = false);
                }
            }
        }

        internal static void OnSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe)
            {
                return;
            }

            foreach (var sname in ValidList)
            {
                if (sname == args.SData.Name.ToLower())
                    CanMuramana = true;
            }

            if (args.SData.IsAutoAttack() &&
               (ProSeries.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo ||
                args.Target.Type == GameObjectType.obj_AI_Hero))
            {
                CanMuramana = true;
            }

            else
            {
                Utility.DelayAction.Add(400, () => CanMuramana = false);
            }

        }
    }
}
