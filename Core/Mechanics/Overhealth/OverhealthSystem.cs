using GoldLeaf.Core.Mechanics.Overhealth;
using GoldLeaf.Items;
using GoldLeaf.Items.Sky;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using static GoldLeaf.Core.Helper;
using static Terraria.ModLoader.ModContent;

namespace GoldLeaf.Core.Mechanics.Overhealth
{
    public abstract class OverhealthPool : ILoadable
    {
        public OverhealthPool()
        {
            size = 1;
            duration = DefaultDuration;
            timer = 0;
        }

        public virtual int MaxSize => 20;
        public virtual int TimeToDecrement => 5;
        public virtual int AmountToDecrement => 1;
        public virtual int DefaultDuration => 240;

        public int size = 1;
        public int duration = 240;
        public int timer = 0;

        public virtual bool PreUpdateTime(Player player) => true;
        public void UpdateTime(Player player)
        {
            if (duration > 0)
            {
                duration--;
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
        
        public int Overhealth => GetTotalOverhealth(Player);
        public int visualOverhealth = 0;

        public override void Load()
        {
            
        }
        public override void Unload() 
        {
            
        }

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
        public static void AddOverhealth<T>(Player player, int size = 1) where T : OverhealthPool, new()
        {
            T pool = new() { size = size }; pool.duration = pool.DefaultDuration;

            if (pool.size == 0)
                return;

            OverhealthManager manager = player.GetModPlayer<OverhealthManager>();
            OverhealthPool foundPool = GetOverhealthPool<T>(player);

            if (foundPool == null && pool.size > 0)
            {
                manager.overhealthPools.Add(pool);
            }
            else
            {
                foundPool.size = Math.Clamp(foundPool.size + pool.size, 0, foundPool.MaxSize);
                foundPool.duration = Math.Max(pool.duration, foundPool.duration);
                foundPool.timer = 0;
            }
        }
        public static void AddOverhealth(Player player, OverhealthPool pool)
        {
            if (pool.size == 0)
                return;

            OverhealthManager manager = player.GetModPlayer<OverhealthManager>();
            var foundPool = manager.overhealthPools.Find(p => p.GetType() == pool.GetType());
            
            if (foundPool == null && pool.size > 0)
            {
                manager.overhealthPools.Add(pool);
            }
            else
            {
                foundPool.size = Math.Clamp(foundPool.size + pool.size, 0, foundPool.MaxSize);
                foundPool.duration = Math.Max(pool.duration, foundPool.duration);
                foundPool.timer = 0;
            }
        }

        public static void SetOverhealth<T>(Player player, int size) where T : OverhealthPool, new()
        {
            T pool = new() { size = size }; pool.duration = pool.DefaultDuration;

            OverhealthManager manager = player.GetModPlayer<OverhealthManager>();
            var foundPool = manager.overhealthPools.Find(p => p.GetType() == pool.GetType());

            if (foundPool == null && pool.size > 0)
                manager.overhealthPools.Add(pool);
            else
                foundPool = pool;
        }
        public static void SetOverhealth(Player player, OverhealthPool pool)
        {
            OverhealthManager manager = player.GetModPlayer<OverhealthManager>();
            var foundPool = manager.overhealthPools.Find(p => p.GetType() == pool.GetType());

            if (foundPool == null && pool.size > 0)
                manager.overhealthPools.Add(pool);
            else
                foundPool = pool;
        }
        
        public static OverhealthPool GetOverhealthPool<T>(Player player) where T : OverhealthPool
        {
            OverhealthManager manager = player.GetModPlayer<OverhealthManager>();

            foreach (OverhealthPool foundPool in manager.overhealthPools)
            {
                if (foundPool is T pool)
                {
                    return pool;
                }
            }
            return null;
        }
        public static OverhealthPool GetOverhealthPool(Player player, OverhealthPool pool) => player.GetModPlayer<OverhealthManager>().overhealthPools.Find(p => p.GetType() == pool.GetType());

        public override void OnHurt(Player.HurtInfo info)
        {
            if (GetTotalOverhealth(Player) > 0)
            {
                int totalOverhealthLost = 0;

                foreach (OverhealthPool pool in overhealthPools)
                {
                    int overhealthLost = Math.Min(pool.size, info.Damage - totalOverhealthLost);
                    
                    totalOverhealthLost += overhealthLost;
                    pool.size -= overhealthLost;
                    
                    pool.OnHurt(Player, info, overhealthLost);

                    if (totalOverhealthLost >= info.Damage) 
                        break;
                }
                Player.statLife += totalOverhealthLost;

                if (Player.HasItem(ItemType<DebugItem>()))
                    Main.NewText("Overhealth Lost: " + totalOverhealthLost, ColorHelper.Overhealth);
                return;
            }
            UpdateVisualOverhealth();
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
            RemoveEmptyPools();
            UpdateVisualOverhealth();
        }
        private void RemoveEmptyPools()
        {
            for (int i = 0; i < overhealthPools.Count; i++)
            {
                if (overhealthPools[i].size <= 0)
                    overhealthPools.RemoveAt(i);
            }
        }
        private void UpdateVisualOverhealth()
        {
            if (visualOverhealth == Overhealth)
                return;

            visualOverhealth = (int)MathHelper.Lerp(visualOverhealth, Overhealth, 0.15f);
            
            if (visualOverhealth < GetTotalOverhealth(Player))
                visualOverhealth++;
            if (visualOverhealth > GetTotalOverhealth(Player))
                visualOverhealth--;

            visualOverhealth = Math.Clamp(visualOverhealth, 0, GetTotalOverhealth(Player));
        }

        /*public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
        {
            ModPacket packet = Mod.GetPacket();
            packet.Write((byte)GoldLeaf.MessageType.OverhealthSync);
            packet.Write((byte)Player.whoAmI);
            packet.Send(toWho, fromWho);
        }
        public void ReceivePlayerSync(BinaryReader reader)
        {
            //extraSegments = reader.ReadByte();
        }

        public override void CopyClientState(ModPlayer targetCopy)
        {
            OverhealthManager clone = (OverhealthManager)targetCopy;
            clone.overhealthPools = overhealthPools;
        }
        public override void SendClientChanges(ModPlayer clientPlayer)
        {
            OverhealthManager clone = (OverhealthManager)clientPlayer;

            if (overhealth != clone.overhealth)
                SyncPlayer(toWho: -1, fromWho: Main.myPlayer, newPlayer: false);
        }*/
    }
}

namespace GoldLeaf.Core
{
    public static partial class Helper
    {
        public static int Overhealth(this Player player) => OverhealthManager.GetTotalOverhealth(player);
        public static int GetOverhealthOfType<T>(this Player player) where T : OverhealthPool => OverhealthManager.GetOverhealthPool<T>(player) == null ? 0 : OverhealthManager.GetOverhealthPool<T>(player).size;
        public static void AddOverhealth<T>(this Player player, int size = 1) where T : OverhealthPool, new() => OverhealthManager.AddOverhealth<T>(player, size);
        public static void AddOverhealth(Player player, OverhealthPool pool) => OverhealthManager.AddOverhealth(player, pool);
        public static void SetOverhealthPool<T>(this Player player, int size) where T : OverhealthPool, new() => OverhealthManager.SetOverhealth<T>(player, size);
        public static void SetOverhealth(Player player, OverhealthPool pool) => OverhealthManager.SetOverhealth(player, pool);
        public static OverhealthPool GetOverhealthPool<T>(this Player player) where T : OverhealthPool => OverhealthManager.GetOverhealthPool<T>(player);
        public static OverhealthPool GetOverhealthPool(this Player player, OverhealthPool pool) => OverhealthManager.GetOverhealthPool(player, pool);
    }
}
