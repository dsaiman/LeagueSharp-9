using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using ProSeries.Utils.Drawings;

namespace ProSeries.Champions
{
    public class Jinx
    {
        internal static Spell W;
        internal static Spell E;
        internal static Spell R;

        public Jinx()
        {
            //Load spells
            W = new Spell(SpellSlot.W, 1500f);
            W.SetSkillshot(0.6f, 60f, 3300f, true, SkillshotType.SkillshotLine);

            E = new Spell(SpellSlot.E, 900f);
            E.SetSkillshot(0.7f, 120f, 1750f, false, SkillshotType.SkillshotCircle);

            R = new Spell(SpellSlot.R, 2000);
            R.SetSkillshot(0.6f, 140f, 1700f, false, SkillshotType.SkillshotLine);

            //Drawings
            Circles.Add("W Range", W);

            //Spell usage.
            var cMenu = new Menu("Combo", "combo");
            cMenu.AddItem(new MenuItem("combomana", "Minimum mana %")).SetValue(new Slider(5));
            cMenu.AddItem(new MenuItem("usecombow", "Use Zap", true).SetValue(true));
            cMenu.AddItem(new MenuItem("usecomboe", "Use Flame Chompers", true).SetValue(true));
            cMenu.AddItem(new MenuItem("usecombor", "Use Mega Death Rocket", true).SetValue(true));
            cMenu.AddItem(new MenuItem("usecombo", "Combo (active)")).SetValue(new KeyBind(32, KeyBindType.Press));
            ProSeries.Config.AddSubMenu(cMenu);

            var hMenu = new Menu("Harass", "harass");
            hMenu.AddItem(new MenuItem("harassmana", "Minimum mana %")).SetValue(new Slider(55));
            hMenu.AddItem(new MenuItem("useharassw", "Use Zap", true).SetValue(true));
            hMenu.AddItem(new MenuItem("useharass", "Harass (active)")).SetValue(new KeyBind(67, KeyBindType.Press));
            ProSeries.Config.AddSubMenu(hMenu);

            var wMenu = new Menu("Farming", "farming");
            wMenu.AddItem(new MenuItem("clearmana", "Minimum mana %")).SetValue(new Slider(35));
            wMenu.AddItem(new MenuItem("useclear", "Wave/Jungle (active)")).SetValue(new KeyBind(86, KeyBindType.Press));
            ProSeries.Config.AddSubMenu(wMenu);

            var mMenu = new Menu("Misc", "misc");
            mMenu.AddItem(new MenuItem("maxrdist", "Max R distance", true)).SetValue(new Slider(1500, 0, 3000));
            mMenu.AddItem(new MenuItem("useeimm", "Use E on Immobile", true)).SetValue(true);
            mMenu.AddItem(new MenuItem("useedash", "Use E on Dashing", true)).SetValue(true);
            ProSeries.Config.AddSubMenu(mMenu);

            //Events
            Game.OnUpdate += Game_OnGameUpdate;
            Orbwalking.AfterAttack += OrbwalkingOnAfterAttack;
        }

        private static void OrbwalkingOnAfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (!unit.IsValid || !unit.IsMe)
            {
                return;
            }

            if (!target.IsValid<Obj_AI_Hero>())
            {
                return;
            }

            var targetAsHero = (Obj_AI_Hero) target;
            if (ProSeries.Player.GetSpellDamage(targetAsHero, SpellSlot.W) / W.Delay >
                ProSeries.Player.GetAutoAttackDamage(targetAsHero, true) * (1 / ProSeries.Player.AttackDelay))
            {
                W.Cast(targetAsHero);
            }
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (ProSeries.CanCombo())
            {
                var wtarget = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Physical);
                if (wtarget.IsValidTarget() && W.IsReady())
                {
                    if (ProSeries.Config.Item("usecombow", true).GetValue<bool>())
                        W.CastIfHitchanceEquals(wtarget, HitChance.Medium);
                }

                if (ProSeries.Config.Item("usecomboe", true).GetValue<bool>() && E.IsReady())
                {
                    foreach (var target in ObjectManager.Get<Obj_AI_Hero>().Where(h => h.IsValidTarget(400) && h.IsMelee()))
                    {
                        E.CastIfHitchanceEquals(target, HitChance.High);
                    }
                }
            }

            if (ProSeries.CanHarass())
            {
                var wtarget = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Physical);
                if (wtarget.IsValidTarget() && W.IsReady() && ProSeries.IsWhiteListed(wtarget))
                {
                    if (ProSeries.Config.Item("useharassw", true).GetValue<bool>())
                        W.CastIfHitchanceEquals(wtarget, HitChance.Medium);
                }
            }


            if (E.IsReady())
            {
                foreach (var target in ObjectManager.Get<Obj_AI_Hero>().Where(h => h.IsValidTarget(E.Range)))
                {
                    if (ProSeries.Config.Item("useeimm", true).GetValue<bool>())
                        E.CastIfHitchanceEquals(target, HitChance.Immobile);

                    if (ProSeries.Config.Item("useedash", true).GetValue<bool>())
                        E.CastIfHitchanceEquals(target, HitChance.Dashing);
                }
            }

            if (ProSeries.Config.Item("usecombor", true).GetValue<bool>())
            {
                var maxDistance = ProSeries.Config.Item("maxrdist", true).GetValue<Slider>().Value;
                foreach (var target in ObjectManager.Get<Obj_AI_Hero>().Where(h => h.IsValidTarget(maxDistance)))
                {
                    var aaDamage = Orbwalking.InAutoAttackRange(target)
                        ? ProSeries.Player.GetAutoAttackDamage(target, true)
                        : 0;

                    if (target.Health - aaDamage <= ProSeries.Player.GetSpellDamage(target, SpellSlot.R))
                    {
                        R.Cast(target);
                    }
                }
            }
        }
    }
}