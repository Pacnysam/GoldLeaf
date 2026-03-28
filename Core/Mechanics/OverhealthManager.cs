using GoldLeaf.Core.Mechanics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using static GoldLeaf.Core.Helper;
using static Terraria.ModLoader.ModContent;

namespace GoldLeaf.Core.Mechanics
{
    //ok this is a mess i should maybe possibly consider rewriting this
    public class OverhealthPool(/*string key*/) : ILoadable
    {
        public virtual int MaxSize => 20;
        public virtual int TimeToDecrement => 5;
        public virtual int AmountToDecrement => 1;

        public int size = 1;
        public int duration = 240;
        public int timer = 0;
        public bool Active => size > 0;

        public virtual bool PreUpdateTime(Player player)
        {
            return true;
        }

        public void UpdateTime(Player player)
        {
            if (duration > 0)
            {
                duration--;
                Main.NewText(duration, Color.Gold);
            }
            else
            {
                if (TimeToDecrement == 0)
                {
                    size = 0;
                }
                else if (timer++ >= TimeToDecrement)
                {
                    size -= AmountToDecrement;
                    timer = 0;
                    Main.NewText(player.Overhealth());
                }
            }
        }
        public virtual void OnHurt(Player player, Player.HurtInfo info, int amountLost) { }
        public virtual void Update(Player player) { }
        public virtual void Load(Mod mod) { }
        public virtual void Unload() { }
    }

    public class OverhealthManager : ModPlayer
    {
        public List<OverhealthPool> overhealthPools = [];
        
        public int overhealth = 0;

        public static int GetTotalOverhealth(Player player)
        {
            int total = 0;
            foreach (OverhealthPool pool in player.GetModPlayer<OverhealthManager>().overhealthPools)
            {
                if (pool != null && pool.size > 0)
                    total += pool.size;
            }
            return total;
        }

        public static void AddOverhealth(Player player, OverhealthPool pool)
        {
            if (pool.size == 0)
            {
                return;
            }

            OverhealthManager manager = player.GetModPlayer<OverhealthManager>();
            OverhealthPool foundPool = GetOverhealthPool(player, pool);

            if (foundPool == null && pool.size > 0)
            {
                manager.overhealthPools.Add(pool);
            }
            else
            {
                foundPool.size = Math.Clamp(foundPool.size + pool.size, 0, foundPool.MaxSize);
                foundPool.duration = Math.Max(pool.duration, foundPool.duration);
                foundPool.timer = 0;

                if (foundPool.size <= 0)
                    manager.overhealthPools.Remove(foundPool);
            }
        }
        public static OverhealthPool GetOverhealthPool(Player player, OverhealthPool poolType)
        {
            OverhealthManager manager = player.GetModPlayer<OverhealthManager>();
            foreach (OverhealthPool pool in manager.overhealthPools)
            {
                if (pool.GetType() == poolType.GetType())
                    return pool;
            }
            return null;
        }

        public override void OnHurt(Player.HurtInfo info)
        {
            if (GetTotalOverhealth(Player) > 0)
            {
                int totalOverhealthLost = 0;

                foreach (OverhealthPool pool in overhealthPools)
                {
                    int overhealthLost = Math.Min(pool.size, info.Damage);
                    
                    totalOverhealthLost += overhealthLost;
                    pool.size -= overhealthLost;
                    
                    pool.OnHurt(Player, info, totalOverhealthLost);

                    if (totalOverhealthLost >= info.Damage) 
                        break;
                }
                Player.statLife += totalOverhealthLost;
                Main.NewText("Overhealth Lost: " + totalOverhealthLost, ColorHelper.Overhealth);
                return;

                //int overhealthLost = Math.Min(Overhealth, info.Damage);
                for (; totalOverhealthLost > 0; totalOverhealthLost--)
                {
                    OverhealthPool pool = overhealthPools.First();
                    pool.size -= Math.Min(pool.size, totalOverhealthLost); totalOverhealthLost -= Math.Min(pool.size, totalOverhealthLost);

                    if (pool.size <= 0)
                        overhealthPools.Remove(pool);
                }
            }
        }

        public override void PostUpdateBuffs()
        {
            if (overhealthPools.Count != 0) 
            {
                UpdateOverhealthPools();
            }
        }

        public override void UpdateDead()
        {
            overhealthPools.Clear();
        }

        private static void UpdateTime(OverhealthPool pool) 
        {
            if (pool.duration > 0)
            {
                pool.duration--;
                Main.NewText(pool.duration, Color.Gold);
            }
            else
            {
                if (pool.TimeToDecrement == 0)
                {
                    pool.size = 0;
                }
                else if (pool.timer++ >= pool.TimeToDecrement)
                {
                    pool.size -= pool.AmountToDecrement;
                    pool.timer = 5;
                }
            }
        }

        private void UpdateOverhealthPools()
        {
            foreach (OverhealthPool pool in overhealthPools)
            {
                if (pool == null)
                    continue;

                if (pool.PreUpdateTime(Player))
                    pool.UpdateTime(Player);

                pool.Update(Player);

                if (pool.size > pool.MaxSize) pool.size = pool.MaxSize;
            }
            overhealth = GetTotalOverhealth(Player);
        }
    }
}

namespace GoldLeaf.Core
{
    public static partial class Helper
    {
        public static int Overhealth(this Player player) => player.GetModPlayer<OverhealthManager>().overhealth;
    }
}
