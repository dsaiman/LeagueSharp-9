using System;
using LeagueSharp;
using LeagueSharp.Common;
using ProSeries.Utils.Drawings;

namespace ProSeries.Champions
{
    public class Tristana
    {
        internal static Spell Q;
        internal static Spell E;
        internal static Spell R;
        internal static float TrueRange;
        public Tristana()
        {
            // Set spels
            Q = new Spell(SpellSlot.Q, 545);
            E = new Spell(SpellSlot.E, 545);
            R = new Spell(SpellSlot.R, 545);

            // Drawings
            Drawing.OnDraw += args =>
            {
                if (ProSeries.Config.Item("drawq").GetValue<Circle>().Active)
                {
                    Render.Circle.DrawCircle(ProSeries.Player.Position, TrueRange,
                        ProSeries.Config.Item("drawq").GetValue<Circle>().Color);
                }
            };

            var dMenu = new Menu("Drawings", "drawings");
            dMenu.AddItem(new MenuItem("drawq", "E Range")).SetValue(new Circle(true, System.Drawing.Color.White));
            ProSeries.Config.AddSubMenu(dMenu);

            // Spell usage
            var cMenu = new Menu("Combo", "combo");
            cMenu.AddItem(new MenuItem("combomana", "Minimum mana %")).SetValue(new Slider(5));
            cMenu.AddItem(new MenuItem("usecomboq", "Use Rapid Fire", true)).SetValue(true);
            cMenu.AddItem(new MenuItem("usecomboe", "Use Explosive Charge", true)).SetValue(true);
            cMenu.AddItem(new MenuItem("usecombor", "Use Buster Shot", true)).SetValue(true);
            cMenu.AddItem(new MenuItem("usecombo", "Combo (active)")).SetValue(new KeyBind(32, KeyBindType.Press));
            ProSeries.Config.AddSubMenu(cMenu);

            var hMenu = new Menu("Harass", "harass");
            hMenu.AddItem(new MenuItem("harassmana", "Minimum mana %")).SetValue(new Slider(55));
            hMenu.AddItem(new MenuItem("useharassq", "Use Rapid Fire", true)).SetValue(false);
            hMenu.AddItem(new MenuItem("useharasse", "Use Explosive Charge", true)).SetValue(true);
            hMenu.AddItem(new MenuItem("useharass", "Harass (active)")).SetValue(new KeyBind(67, KeyBindType.Press));
            ProSeries.Config.AddSubMenu(hMenu);

            var fMenu = new Menu("Farming", "farming");
            fMenu.AddItem(new MenuItem("clearmana", "Minimum mana %")).SetValue(new Slider(35));
            fMenu.AddItem(new MenuItem("useclearq", "Use Rapid Fire", true)).SetValue(false);
            fMenu.AddItem(new MenuItem("useclear", "Wave/Jungle (active)")).SetValue(new KeyBind(86, KeyBindType.Press));
            ProSeries.Config.AddSubMenu(fMenu);

            var mMenu = new Menu("Misc", "Misc");
            mMenu.AddItem(new MenuItem("usergap", "Use R on Gapcloser", true)).SetValue(true);
            ProSeries.Config.AddSubMenu(mMenu);

            // Events
            Game.OnUpdate += Game_OnUpdate;
            Orbwalking.AfterAttack += Orbwalking_AfterAttack;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;

        }

        internal static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (ProSeries.Config.Item("usecombo").GetValue<KeyBind>().Active &&
                ProSeries.Config.Item("usergap", true).GetValue<bool>())
            {
                if (gapcloser.Sender.IsValidTarget(TrueRange))
                    R.CastOnUnit(gapcloser.Sender);
            }
        }

        internal static void Orbwalking_AfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (!unit.IsValid || !unit.IsMe)
            {
                return;
            }

            if (ProSeries.CanCombo())
            {
                if (ProSeries.Config.Item("usecomboq", true).GetValue<bool>() &&
                    target.IsValid<Obj_AI_Hero>())
                {
                    Q.Cast();
                }
            }

            if (ProSeries.CanHarass())
            {
                if (ProSeries.Config.Item("useharassq", true).GetValue<bool>() &&
                    target.IsValid<Obj_AI_Hero>())
                {
                    Q.Cast();
                }
            }

            if (ProSeries.CanClear())
            {
                if (ProSeries.Config.Item("useclearq", true).GetValue<bool>() &&
                    target.IsValid<Obj_AI_Minion>())
                {
                    Q.Cast();
                }
            }
        }

        internal static void Game_OnUpdate(EventArgs args)
        {
            TrueRange = 545 + (7*ProSeries.Player.Level);

            if (ProSeries.CanCombo())
            {
                var etarget = TargetSelector.GetTarget(TrueRange, TargetSelector.DamageType.Physical);
                if (ProSeries.Config.Item("usecomboe", true).GetValue<bool>() &&
                    etarget.IsValid<Obj_AI_Hero>())
                {
                    E.CastOnUnit(etarget);
                }

            }

            if (ProSeries.CanHarass())
            {
                var etarget = TargetSelector.GetTarget(TrueRange, TargetSelector.DamageType.Physical);
                if (ProSeries.Config.Item("useharasse", true).GetValue<bool>() &&
                    etarget.IsValid<Obj_AI_Hero>())
                {
                    E.CastOnUnit(etarget);
                }

            }

            if (R.IsReady())
            {
                var target = TargetSelector.GetTarget(TrueRange, TargetSelector.DamageType.Physical);
                if (target.IsValidTarget() && ProSeries.Config.Item("usecombor", true).GetValue<bool>())
                {
                    if (target.Health <= ProSeries.Player.GetSpellDamage(target, SpellSlot.R))
                        R.CastOnUnit(target);
                }
            }
        }
    }
}
