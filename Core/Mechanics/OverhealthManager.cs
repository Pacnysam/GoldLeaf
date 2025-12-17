using GoldLeaf.Effects.Dusts;
using GoldLeaf.Items.Dyes;
using GoldLeaf.Items.Grove;
using GoldLeaf.Items.Nightshade;
using GoldLeaf.Items.VanillaBossDrops;
using GoldLeaf.Items.Vanity;
using GoldLeaf.Tiles.Decor;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Graphics.Renderers;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using static GoldLeaf.Core.Helper;
using static Terraria.ModLoader.ModContent;

namespace GoldLeaf.Core.Mechanics
{
    public class VigorPool : OverhealthPool
    {
        public override int maxSize => 40;
    }

    public abstract class OverhealthPool
    {
        public virtual int maxSize => 20;
        public int size = 0;
        public int timer = 5;

        public void ModifyTotal(int amount)
        {
            size = Math.Clamp(size + amount, 0, maxSize);
        }
        public void SetTotal(int amount)
        {
            size = Math.Clamp(amount, 0, maxSize);
        }

        public virtual void Update() 
        {
            if (timer-- <= 0)
            {
                size--;
                timer = 5;
            }
        }
    }

    public class OverhealthManager : ModPlayer
    {
        public List<OverhealthPool> overhealthPools;
        public int Overhealth
        {
            get
            {
                int total = 0;
                foreach (OverhealthPool pool in overhealthPools)
                {
                    total += pool.size;
                }

                return total;
            }
        }

        public static void AddOverhealthPool(Player player, OverhealthPool pool)
        {
            OverhealthPool foundPool = null;
            foreach (OverhealthPool currentPool in player.GetModPlayer<OverhealthManager>().overhealthPools)
            {
                if (currentPool.GetType() == pool.GetType()) 
                {
                    foundPool = currentPool;
                    break;
                }
            }

            if (foundPool == null)
                player.GetModPlayer<OverhealthManager>().overhealthPools.Add(pool);
            else
            {
                foundPool.size += pool.size;
                foundPool.timer = Math.Max(foundPool.timer, pool.timer);
            }
        }

        public override void OnHurt(Player.HurtInfo info)
        {
            return;
            if (Overhealth > 0)
            {
                int overhealthLost = Math.Min(Overhealth, info.Damage);

                Player.statLife += overhealthLost;

                for (; overhealthLost > 0; overhealthLost--)
                {
                    OverhealthPool pool = overhealthPools.First();
                    pool.size -= Math.Min(pool.size, overhealthLost); overhealthLost -= Math.Min(pool.size, overhealthLost);

                    if (pool.size <= 0)
                        overhealthPools.Remove(pool);
                }
            }
        }

        public override void Load()
        {
            //overhealthPools = [];
        }
        public override void Unload()
        {
            //overhealthPools.Clear();
        }

        public override void PostUpdate()
        {
            return;
            if (overhealthPools.Any()) 
            {
                UpdateOverhealthPools();
            }
        }

        public override void UpdateDead()
        {
            //overhealthPools.Clear();
        }

        private void UpdateOverhealthPools()
        {
            foreach (OverhealthPool pool in overhealthPools)
            {
                pool.Update();

                if (pool.size <= 0) overhealthPools.Remove(pool);
                if (pool.size > pool.maxSize) pool.size = pool.maxSize;
            }
        }
    }
}
