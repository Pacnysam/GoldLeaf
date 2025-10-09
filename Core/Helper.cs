using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.UI;
using Terraria.ModLoader;
using static Terraria.WorldGen;
using static Terraria.ModLoader.ModContent;
using Terraria.Localization;
using ReLogic.Content;
using GoldLeaf.Items.Blizzard;
using GoldLeaf.Items.Blizzard.Armor;
using System.IO;
using System.Diagnostics.Metrics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using Newtonsoft.Json.Linq;


namespace GoldLeaf.Core //most of this is snatched from starlight river and spirit, i (pacnysam) did not code most of this!
{
    public static partial class Helper
    {
        public static Rectangle ToRectangle(this Vector2 vector) => new Rectangle(0, 0, (int)vector.X, (int)vector.Y);
        public static Vector2 ScreenSize => new (Main.screenWidth, Main.screenHeight);

        public static bool OnScreen(Vector2 pos) => pos.X > -16 && pos.X < Main.screenWidth + 16 && pos.Y > -16 && pos.Y < Main.screenHeight + 16;

        public static bool OnScreen(Rectangle rect) => rect.Intersects(new Rectangle(0, 0, Main.screenWidth, Main.screenHeight));

        public static bool OnScreen(Vector2 pos, Vector2 size) => OnScreen(new Rectangle((int)pos.X, (int)pos.Y, (int)size.X, (int)size.Y));

        public static float Ratio(float width, float height) => height / width;

        public static Vector3 Vec3(this Vector2 vector) => new Vector3(vector.X, vector.Y, 0);

        public static Vector3 ScreenCoord(this Vector3 vector) => new(-1 + (vector.X / Main.screenWidth * 2), (-1 + (vector.Y / Main.screenHeight * 2f)) * -1, 0);

        public static float WorldTimer => Main.GlobalTimeWrappedHourly;

        public static bool IsVanitySet(Player player, int head, int body, int legs) 
        {
            if (player.armor[0].type == head && player.armor[10].type == ItemID.None || player.armor[10].type == head &&
                player.armor[1].type == body && player.armor[11].type == ItemID.None || player.armor[11].type == body &&
                player.armor[2].type == legs && player.armor[12].type == ItemID.None || player.armor[12].type == legs) return true;
            return false;
        }
        public static bool IsVanitySet(Player player, int head, int body)
        {
            if (player.armor[0].type == head && player.armor[10].type == ItemID.None || player.armor[10].type == head &&
                player.armor[1].type == body && player.armor[11].type == ItemID.None || player.armor[11].type == body) return true;
            return false;
        }

        public static string EmptyTexString => "GoldLeaf/Textures/Empty";
        public static Texture2D EmptyTex => Request<Texture2D>("GoldLeaf/Textures/Empty").Value;

        public static string SetBonusKey => Language.GetTextValue(Main.ReversedUpDownArmorSetBonuses ? "Key.UP" : "Key.DOWN");
        public static string SetBonusSecondaryKey => Language.GetTextValue(Main.ReversedUpDownArmorSetBonuses ? "Key.DOWN" : "Key.UP");

        public static float LerpFloat(float min, float max, float val)
        {
            float difference = max - min;
            return min + (difference * val);
        }

        public static float LavaLayer => Main.maxTilesY * 0.72f;

        public static bool CheckCircularCollision(Vector2 center, int radius, Rectangle hitbox)
        {
            if (Vector2.Distance(center, hitbox.TopLeft()) <= radius) return true;
            if (Vector2.Distance(center, hitbox.TopRight()) <= radius) return true;
            if (Vector2.Distance(center, hitbox.BottomLeft()) <= radius) return true;
            return Vector2.Distance(center, hitbox.BottomRight()) <= radius;
        }

        public static bool CheckConicalCollision(Vector2 center, int radius, float angle, float width, Rectangle hitbox)
        {
            if (CheckPoint(center, radius, hitbox.TopLeft(), angle, width)) return true;
            if (CheckPoint(center, radius, hitbox.TopRight(), angle, width)) return true;
            if (CheckPoint(center, radius, hitbox.BottomLeft(), angle, width)) return true;
            return CheckPoint(center, radius, hitbox.BottomRight(), angle, width);
        }

        private static bool CheckPoint(Vector2 center, int radius, Vector2 check, float angle, float width)
        {
            float thisAngle = (center - check).ToRotation() % 6.28f;
            return Vector2.Distance(center, check) <= radius && thisAngle > angle - width && thisAngle < angle + width;
        }

        /*public static bool PointInTile(Vector2 point)
        {
            Point16 startCoords = new Point16((int)point.X / 16, (int)point.Y / 16);
            for(int x = -1; x <= 1; x++)
                for(int y = -1; y <= 1; y++)
                {                 
                    var thisPoint = startCoords + new Point16(x, y);

                    if (!WorldGen.InWorld(thisPoint.X, thisPoint.Y)) return false;

                        var tile = Framing.GetTileSafely(thisPoint);
                    if(tile. == 1)
                    {
                        var rect = new Rectangle(thisPoint.X * 16, thisPoint.Y * 16, 16, 16);
                        if (rect.Contains(point.ToPoint())) return true;
                    }
                }

            return false;
        }*/

        public static float Counter(this Projectile projectile) => projectile.GetGlobalProjectile<GoldLeafProjectile>().counter;
        public static float Counter(this NPC npc) => npc.GetGlobalNPC<GoldLeafNPC>().counter;

        public static bool Stunned(this Player player) => player.CCed || player.GetModPlayer<GoldLeafPlayer>().stunned;
        public static bool CanBeStunned(this NPC npc) => (!npc.boss && !NPCID.Sets.ShouldBeCountedAsBoss[npc.type] && npc.aiStyle != NPCAIStyleID.Worm && npc.knockBackResist != 0f);

        public static bool ZoneGrove(this Player player) => player.GetModPlayer<GoldLeafPlayer>().ZoneGrove;
        public static bool ZoneForest(this Player Player)
        {
            return !Player.ZoneJungle
                && !Player.ZoneDungeon
                && !Player.ZoneCorrupt
                && !Player.ZoneCrimson
                && !Player.ZoneHallow
                && !Player.ZoneSnow
                && !Player.ZoneUndergroundDesert
                && !Player.ZoneGlowshroom
                && !Player.ZoneMeteor
                && !Player.ZoneBeach
                && !Player.ZoneDesert
                && !Player.GetModPlayer<GoldLeafPlayer>().ZoneGrove
                && Player.ZoneOverworldHeight;
        }

        public static string TicksToTime(int ticks)
        {
            int sec = ticks / 60;
            return sec / 60 + ":" + (sec % 60 < 10 ? "0" + sec % 60 : "" + sec % 60);
        }

        public static int TimeToTicks(float hours, float min, float sec)
        {
            return (int)((hours * 216000) + (min * 3600) + (sec * 60));
        }
        public static int TimeToTicks(float min, float sec)
        {
            return (int)((min * 3600) + (sec * 60));
        }
        public static int TimeToTicks(float sec)
        {
            return (int)(sec * 60);
        }

        public static float CompareAngle(float baseAngle, float targetAngle)
        {
            return (baseAngle - targetAngle + (float)Math.PI * 3) % MathHelper.TwoPi - (float)Math.PI;
        }

        public static float ConvertAngle(float angleIn)
        {
            return CompareAngle(0, angleIn) + (float)Math.PI;
        }

        public static string WrapString(string input, int length, DynamicSpriteFont font, float scale)
        {
            string output = "";
            string[] words = input.Split();

            string line = "";
            foreach (string str in words)
            {
                if (str == "NEWBLOCK")
                {
                    output += "\n\n";
                    line = "";
                    continue;
                }

                if (font.MeasureString(line).X * scale < length)
                {
                    output += " " + str;
                    line += " " + str;
                }
                else
                {
                    output += "\n" + str;
                    line = str;
                }
            }
            return output;
        }

        public static List<T> RandomizeList<T>(List<T> input)
        {
            int n = input.Count();
            while (n > 1)
            {
                n--;
                int k = Main.rand.Next(n + 1);
                T value = input[k];
                input[k] = input[n];
                input[n] = value;
            }
            return input;
        }

        public static Player FindNearestPlayer(Vector2 position)
        {
            Player player = null;

            for (int k = 0; k < Main.maxPlayers; k++)
                if (Main.player[k] != null && Main.player[k].active && (player == null || Vector2.DistanceSquared(position, Main.player[k].Center) < Vector2.DistanceSquared(position, player.Center)))
                    player = Main.player[k];
            return player;
        }

        public static float Opacity(this Gore gore) => 1f - gore.alpha / 255f;

        public static bool Toggle(this bool input) => !input;
        public static float RandomNegative(this float num) => Main.rand.NextBool() ? num : -num;
        public static int RandomNegative(this int num) => Main.rand.NextBool() ? num : -num;

        public static void WritePoint16(this BinaryWriter writer, Point16 point) { writer.Write(point.X); writer.Write(point.Y); }
        public static Point16 ReadPoint16(this BinaryReader reader) => new(reader.ReadInt16(), reader.ReadInt16());

        public static float BezierEase(float time)
        {
            return time * time / (2f * (time * time - time) + 1f);
        }

        public static T[] FastUnion<T>(this T[] front, T[] back)
        {
            T[] combined = new T[front.Length + back.Length];

            Array.Copy(front, combined, front.Length);
            Array.Copy(back, 0, combined, front.Length, back.Length);

            return combined;
        }


        public static bool ConsumeItems(this Item[] inventory, Predicate<Item> predicate, int count)
        {
            var items = inventory.GetItems(predicate, count);

            // If the sum of items is less than the required amount, don't bother.
            if (items.Sum(i => inventory[i].stack) < count)
                return false;

            for (int i = 0; i < items.Count; i++)
            {
                Item item = inventory[items[i]];

                // If we're at the last item stack, and we're not going to consume the whole thing, just decrease its segmentTimer by the amount needed.
                if (i == items.Count - 1 && count < item.stack)
                {
                    item.stack -= count;
                }
                // Otherwise, delete the item and decrement segmentTimer as needed.
                else
                {
                    count -= item.stack;
                    item.TurnToAir();
                }
            }
            return true;
        }

        /// <summary>
        /// Gets a list of item indeces from an inventory matching a predicate.
        /// </summary>
        /// <param name="inventory">The pool of items to search.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="stopCountingAt">The number of items to search before stopping.</param>
        /// <returns>The items matching the predicate.</returns>
        public static List<int> GetItems(this Item[] inventory, Predicate<Item> predicate, int stopCountingAt = int.MaxValue)
        {
            var indeces = new List<int>();
            for (int i = 0; i < inventory.Length; i++)
            {
                if (stopCountingAt <= 0)
                    break;
                if (predicate(inventory[i]))
                {
                    indeces.Add(i);
                    stopCountingAt -= inventory[i].stack;
                }
            }
            return indeces;
        }
        public static bool HasItem(this Player player, int type, int count = 1)
        {
            int items = 0;

            for (int k = 0; k < player.inventory.Length; k++)
            {
                Item item = player.inventory[k];
                if (item.type == type) items += item.stack;
            }

            return items >= count;
        }

        public static bool HasAccessory(Player player, int item, bool vanity) 
        {
            int maxAccessoryIndex = 5 + player.extraAccessorySlots;
            int index = 3; if (vanity) index = 13;

            for (int i = index; i < index + maxAccessoryIndex; i++)
            {
                if (player.armor[i].type == item)
                {
                    return true;
                }
            }
            return false;
        }
        public static bool HasAccessory(this Player player, int item)
        {
            int maxAccessoryIndex = 5 + player.extraAccessorySlots;

            for (int i = 3; i < 3 + maxAccessoryIndex; i++)
            {
                if (player.armor[i].type == item)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool TryTakeItem(Player player, int type, int count = 1)
        {
            if (player.HasItem(type, count))
            {
                int toTake = count;

                for (int k = 0; k < player.inventory.Length; k++)
                {
                    Item item = player.inventory[k];

                    if (item.type == type)
                    {
                        int stack = item.stack;
                        for (int i = 0; i < stack; i++)
                        {
                            item.stack--;
                            if (item.stack == 0) item.TurnToAir();

                            toTake--;
                            if (toTake <= 0) break;
                        }
                    }
                    if (toTake == 0) break;
                }

                return true;
            }
            else return false;
        }

        public static void NewItemSpecific(Vector2 position, Item item)
        {
            int targetIndex = 400;
            Main.item[400] = new Item(); //Vanilla seems to make sure to set the dummy here, so I will too.

            if (Main.netMode != NetmodeID.MultiplayerClient) //Main.Item index finder from vanilla
            {
                for (int j = 0; j < 400; j++)
                {
                    if (!Main.item[j].active && Main.timeItemSlotCannotBeReusedFor[j] == 0)
                    {
                        targetIndex = j;
                        break;
                    }
                }
            }

            if (targetIndex == 400 && Main.netMode != NetmodeID.MultiplayerClient) //some sort of vanilla failsafe if no safe index is found it seems?
            {
                int num2 = 0;
                for (int k = 0; k < 400; k++)
                {
                    if (Main.item[k].timeSinceItemSpawned - Main.timeItemSlotCannotBeReusedFor[k] > num2)
                    {
                        num2 = Main.item[k].timeSinceItemSpawned - Main.timeItemSlotCannotBeReusedFor[k];
                        targetIndex = k;
                    }
                }
            }

            Main.item[targetIndex] = item;
            Main.item[targetIndex].position = position;

            if (ItemSlot.Options.HighlightNewItems && item.type >= ItemID.None) //vanilla item highlight system
            {
                item.newAndShiny = true;
            }
            if (Main.netMode == NetmodeID.Server) //syncing
            {
                NetMessage.SendData(MessageID.SyncItem, -1, -1, null, targetIndex, 0, 0f, 0f, 0, 0, 0);
                item.FindOwner(item.whoAmI);
            }
            else if (Main.netMode == NetmodeID.SinglePlayer)
            {
                item.playerIndexTheItemIsReservedFor = Main.myPlayer;
            }
        }

        public static Item NewItemPerfect(Vector2 position, Vector2 velocity, int type, int stack = 1, bool noBroadcast = false, int prefixGiven = 0, bool noGrabDelay = false, bool reverseLookup = false)
        {
            int targetIndex = 400;
            Main.item[400] = new Item(); //Vanilla seems to make sure to set the dummy here, so I will too.

            if (Main.netMode != NetmodeID.MultiplayerClient) //Main.Item index finder from vanilla
            {
                if (reverseLookup)
                {
                    for (int i = 399; i >= 0; i--)
                    {
                        if (!Main.item[i].active && Main.timeItemSlotCannotBeReusedFor[i] == 0)
                        {
                            targetIndex = i;
                            break;
                        }
                    }
                }
                else
                {
                    for (int j = 0; j < 400; j++)
                    {
                        if (!Main.item[j].active && Main.timeItemSlotCannotBeReusedFor[j] == 0)
                        {
                            targetIndex = j;
                            break;
                        }
                    }
                }
            }
            if (targetIndex == 400 && Main.netMode != NetmodeID.MultiplayerClient) //some sort of vanilla failsafe if no safe index is found it seems?
            {
                int num2 = 0;
                for (int k = 0; k < 400; k++)
                {
                    if (Main.item[k].timeSinceItemSpawned - Main.timeItemSlotCannotBeReusedFor[k] > num2)
                    {
                        num2 = Main.item[k].timeSinceItemSpawned - Main.timeItemSlotCannotBeReusedFor[k];
                        targetIndex = k;
                    }
                }
            }
            Main.timeItemSlotCannotBeReusedFor[targetIndex] = 0; //normal stuff
            Item item = Main.item[targetIndex];
            item.SetDefaults(type, false);
            item.Prefix(prefixGiven);
            item.position = position;
            item.velocity = velocity;
            item.active = true;
            item.timeSinceItemSpawned = 0;
            item.stack = stack;

            item.wet = Collision.WetCollision(item.position, item.width, item.height); //not sure what this is, from vanilla

            if (ItemSlot.Options.HighlightNewItems && item.type >= ItemID.None) //vanilla item highlight system
            {
                item.newAndShiny = true;
            }
            if (Main.netMode == NetmodeID.Server && !noBroadcast) //syncing
            {
                NetMessage.SendData(MessageID.SyncItem, -1, -1, null, targetIndex, noGrabDelay ? 1 : 0, 0f, 0f, 0, 0, 0);
                item.FindOwner(item.whoAmI);
            }
            else if (Main.netMode == NetmodeID.SinglePlayer)
            {
                item.playerIndexTheItemIsReservedFor = Main.myPlayer;
            }
            return item;
        }

        public static bool IsInRangeOfMe(Projectile projectile, NPC target, float maxDistance, out float myDistance)
        {
            myDistance = Vector2.Distance(target.Center, projectile.Center);

            if (myDistance < maxDistance && !projectile.CanHitWithOwnBody(target))
                myDistance = float.PositiveInfinity;

            return myDistance <= maxDistance;
        }

        public static bool IsWeapon(this Item item) => item.type != ItemID.None && item.stack > 0 && item.useStyle > ItemUseStyleID.None && (item.damage > 0 || item.useAmmo > 0 && item.useAmmo != AmmoID.Solution);

        public static NetworkText QuickDeathReason(string key, Player player) => NetworkText.FromKey("Mods.GoldLeaf.DeathReasons." + key, player.name);
        public static NetworkText QuickDeathReason(string key, Player player, int variants) => NetworkText.FromKey("Mods.GoldLeaf.DeathReasons." + key + ".Variant" + (1 + Main.rand.Next(variants)), player.name);

        public static void DropItem(this Entity ent, int type, IEntitySource source, int stack = 1)
        {
            int i = Item.NewItem(source, ent.Hitbox, type, stack);
            if (Main.netMode != NetmodeID.SinglePlayer)
                NetMessage.SendData(MessageID.SyncItem, -1, -1, null, i);
        }

        public static void DropItem(this Entity ent, int type, float chance, IEntitySource source, int stack = 1)
        {
            if (Main.rand.NextDouble() < chance)
            {
                int i = Item.NewItem(source, ent.Hitbox, type, stack);
                if (Main.netMode != NetmodeID.SinglePlayer)
                    NetMessage.SendData(MessageID.SyncItem, -1, -1, null, i);
            }
        }

        public static void ReduceBuffTime(this Player player, int buffType, int timeChange) 
        {
            int buffTime = player.buffTime[player.FindBuffIndex(buffType)] - timeChange;
            player.ClearBuff(buffType);
            if (buffTime > 2) player.AddBuff(buffType, buffTime);
        }

        public static bool IsTargetValid(NPC npc) => npc.active && !npc.friendly && !npc.immortal && !npc.dontTakeDamage && npc.lifeMax > 5;
        public static bool IsValid(this NPC npc) => IsTargetValid(npc);

        public static double Distribution(int pos, int maxVal, float posOffset = 0.5f, float maxChance = 100f)
        {
            return -Math.Pow((20 * (pos - (posOffset * maxVal))) / maxVal, 2) + maxChance;
        }

        public static void OutlineRect(Rectangle rect, int tileType)
        {
            for (int i = 0; i < rect.Width; i++)
                PlaceTile(rect.X + i, rect.Y, tileType, true, true);

            for (int i = 0; i < rect.Width; i++)
                PlaceTile(rect.X + i, rect.Y + rect.Height, tileType, true, true);

            for (int i = 0; i < rect.Height; i++)
                PlaceTile(rect.X, rect.Y + i, tileType, true, true);

            for (int i = 0; i < rect.Height; i++)
                PlaceTile(rect.X + rect.Width, rect.Y + i, tileType, true, true);
        }

        public static bool TileNearby(Point position, int distance, int type/*, Predicate<Tile> predicate*/)
        {
            for (int i = Math.Clamp(position.X - distance, 0, Main.maxTilesX); i <= position.X + distance || i > Main.maxTilesX; i++)
            {
                for (int j = Math.Clamp(position.Y - distance, 0, Main.maxTilesY); j <= position.Y + distance || j > Main.maxTilesY; j++)
                {
                    if (Vector2.Distance(new Vector2(position.X, position.Y), new Vector2(i, j)) <= distance && Main.tile[i, j].TileType == type)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /*public static Point FindTileNearby(Point position, int distance, int type)
        {
            for (int i = position.X - distance; i <= position.X + distance; i++)
            {
                for (int j = position.Y - distance; j <= position.Y + distance; j++)
                {
                    if (Vector2.Distance(new Vector2(position.X, position.Y), new Vector2(i, j)) <= distance && Main.tile[i, j].TileType == type)
                    {
                        return new Point(i, j);
                    }
                }
            }
            return position;
        }*/

        public static bool IsValidDebuff(Player player, int buffindex)
        {
            int bufftype = player.buffType[buffindex];
            bool vitalbuff = (BuffSets.Cooldown[bufftype] || !BuffSets.IsRemovable[bufftype] || BuffSets.Cosmetic[bufftype]);
            return player.buffTime[buffindex] > 2 && Main.debuff[bufftype] && !Main.buffNoTimeDisplay[bufftype] && !Main.vanityPet[bufftype] && !vitalbuff;
        }

        public static bool IsValidDebuff(int bufftype, int time)
        {
            bool vitalbuff = (BuffSets.Cooldown[bufftype] || !BuffSets.IsRemovable[bufftype] || BuffSets.Cosmetic[bufftype]);
            return time > 2 && Main.debuff[bufftype] && !Main.buffNoTimeDisplay[bufftype] && !Main.vanityPet[bufftype] && !vitalbuff;
        }

        public static string CoolBuffTex(string input) => (GetInstance<VisualConfig>().CoolBuffs/*Main.AssetSourceController.ActiveResourcePackList.EnabledPacks.Select(x => x.Name).Contains("Cool Buffs")*/ ? input + "Cool" : input); //this works but im removing this once autoload them
        //public static string RadcapTex(string input) => (GetInstance<GraphicsConfig>().CoolBuffs ? input + "Rad" : input);
    }
}

