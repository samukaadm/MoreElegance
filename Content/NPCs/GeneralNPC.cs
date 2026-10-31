using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MoreElegance.Common.Systems;
using MoreElegance.Content.Items.Placeables;
using MoreElegance.Content.Items.Weapons;
using MoreElegance.Content.Projectiles.Weapons;
using MoreElegance.Content.Tiles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MoreElegance.Content.NPCs
{
    [AutoloadHead]
    public class GeneralNPC : ModNPC
    {
        public const string ShopName = "GeneralShop";

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 25;
            NPCID.Sets.ExtraFramesCount[Type] = 9;
            NPCID.Sets.AttackFrameCount[Type] = 4;
            NPCID.Sets.DangerDetectRange[Type] = 600;
            NPCID.Sets.AttackType[Type] = 3; // Melee/Thrust attack
            NPCID.Sets.AttackTime[Type] = 16;
            NPCID.Sets.AttackAverageChance[Type] = 1;
            NPCID.Sets.HatOffsetY[Type] = 2;

            // Biome & NPC happiness
            NPC.Happiness
                .SetBiomeAffection<ForestBiome>(AffectionLevel.Love)
                .SetBiomeAffection<HallowBiome>(AffectionLevel.Like)
                .SetBiomeAffection<CorruptionBiome>(AffectionLevel.Hate)
                .SetBiomeAffection<CrimsonBiome>(AffectionLevel.Hate)
                .SetNPCAffection(NPCID.ArmsDealer, AffectionLevel.Like)
                .SetNPCAffection(NPCID.Steampunker, AffectionLevel.Like)
                .SetNPCAffection(NPCID.Pirate, AffectionLevel.Dislike);
        }

        public override void SetDefaults()
        {
            NPC.townNPC = true;
            NPC.friendly = true;
            NPC.width = 24;
            NPC.height = 46;
            NPC.aiStyle = 7;
            NPC.damage = 25;
            NPC.defense = 30;
            NPC.lifeMax = 350;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0.4f;
            AnimationType = NPCID.Guide;
        }

        public override bool CanTownNPCSpawn(int numTownNPCs)
        {
            // The General arrives when the player shows interest in martial weapons
            for (int k = 0; k < Main.maxPlayers; k++)
            {
                Player player = Main.player[k];
                if (!player.active)
                    continue;

                foreach (Item item in player.inventory)
                {
                    if (item.damage > 0 && (item.DamageType == DamageClass.Melee || item.DamageType is Common.DamageClasses.ElegantMeleeDamageClass))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public override List<string> SetNPCNameList()
        {
            return new List<string>()
            {
                "Mustachio",
                "Reginald",
                "Archibald",
                "Bartholomew",
                "Percival",
                "Julian",
                "Alexander"
            };
        }

        public override string GetChat()
        {
            // Check for endgame reconstruction readiness
            if (WorldUpdateSystem.CanTriggerReconstruction())
            {
                return "Remarkable work, apprentice. The world is finally cleansed of all evil corruption, and the Moon Lord has fallen. Say the word, and I shall oversee the reconstruction of our grand aristocratic estate!";
            }

            WeightedRandom<string> chat = new();
            chat.Add("A true swordsman does not hack like a lumberjack. A single, refined thrust settles all disputes.");
            chat.Add("Every morning requires two essentials: a freshly steeped cup of Earl Grey, and half an hour of lunging drills.");
            chat.Add("My mustache? Hand-trimmed with the tip of my rapier. Perfection requires absolute discipline.");
            chat.Add("Brute strength is common. Elegance, however, is an art form possessed by few.");
            chat.Add("I once repelled an entire swarm of monsters in the city streets with a single continuous thrust technique: Tomboy.");

            if (Main.hardMode)
            {
                chat.Add("The monsters beyond these borders grow more aggressive. Ensure your footwork remains unimpeachable.");
            }

            return chat;
        }

        public override void SetChatButtons(ref string button, ref string button2)
        {
            button = "Go to Training";

            if (WorldUpdateSystem.CanTriggerReconstruction())
            {
                button2 = "Reconstruct Realm";
            }
            else
            {
                button2 = Language.GetTextValue("LegacyInterface.28"); // Standard "Shop" text
            }
        }

        public override void OnChatButtonClicked(bool firstButton, ref string shopName)
        {
            if (firstButton)
            {
                // Teleport local player to the Indestructible Training Room
                TeleportPlayerToTrainingRoom(Main.LocalPlayer);
            }
            else
            {
                if (WorldUpdateSystem.CanTriggerReconstruction())
                {
                    WorldUpdateSystem.TriggerReconstruction();
                }
                else
                {
                    shopName = ShopName;
                }
            }
        }

        public override void AddShops()
        {
            var npcShop = new NPCShop(Type, ShopName);
            npcShop.Add<RapierBase>();
            npcShop.Add<TrainingBlock>();
            npcShop.Add(ItemID.TargetDummy);
            npcShop.Add(ItemID.SharpeningStation);
            npcShop.Register();
        }

        /// <summary>
        /// Safely teleports the player to the training room at map boundaries (X = 50 * 16, Y = 200 * 16).
        /// Ensures the indestructible chamber exists so the player never suffocates or falls.
        /// </summary>
        public static void TeleportPlayerToTrainingRoom(Player player)
        {
            const int roomTileX = 50;
            const int roomTileY = 200;

            // Ensure the indestructible training chamber is built
            EnsureTrainingChamberExists(roomTileX, roomTileY);

            // Teleport target: center floor of the chamber
            Vector2 targetPos = new Vector2((roomTileX + 10) * 16, (roomTileY + 8) * 16);

            SoundEngine.PlaySound(SoundID.Item6, player.position);

            player.Teleport(targetPos, TeleportationStyleID.RodOfDiscord);
            player.velocity = Vector2.Zero;

            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                NetMessage.SendData(MessageID.TeleportEntity, -1, -1, null, 0, player.whoAmI, targetPos.X, targetPos.Y, TeleportationStyleID.RodOfDiscord);
            }

            SoundEngine.PlaySound(SoundID.Item6, player.position);
            Main.NewText("The General: 'En garde! Welcome to the Training Grounds. Perfect your stance.'", 220, 195, 120);
        }

        /// <summary>
        /// Generates a secure, indestructible chamber constructed entirely from TrainingTile blocks,
        /// platforms, training dummy targets, and lighting.
        /// </summary>
        private static void EnsureTrainingChamberExists(int startX, int startY)
        {
            int trainingTileType = ModContent.TileType<TrainingTile>();
            int width = 22;
            int height = 12;

            for (int x = startX; x <= startX + width; x++)
            {
                for (int y = startY; y <= startY + height; y++)
                {
                    Tile tile = Framing.GetTileSafely(x, y);

                    // Perimeter walls, floor, and ceiling made of indestructible TrainingTile
                    if (x == startX || x == startX + width || y == startY || y == startY + height)
                    {
                        tile.HasTile = true;
                        tile.TileType = (ushort)trainingTileType;
                        tile.TileColor = PaintID.None;
                        tile.Slope = SlopeType.Solid;
                    }
                    else
                    {
                        // Clear the interior
                        tile.HasTile = false;
                        tile.WallType = WallID.MarbleBlock;
                    }
                }
            }

            // Place entrance torches inside chamber
            WorldGen.PlaceTile(startX + 3, startY + 3, TileID.Torches, mute: true, forced: true, style: 0);
            WorldGen.PlaceTile(startX + width - 3, startY + 3, TileID.Torches, mute: true, forced: true, style: 0);

            // Sync tiles in multiplayer
            if (Main.netMode == NetmodeID.Server)
            {
                NetMessage.SendTileSquare(-1, startX, startY, width + 1, height + 1);
            }
        }

        public override void TownNPCAttackStrength(ref int damage, ref float knockback)
        {
            damage = 38;
            knockback = 5f;
        }

        public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown)
        {
            cooldown = 12;
            randExtraCooldown = 6;
        }

        public override void TownNPCAttackProj(ref int projType, ref int attackDelay)
        {
            projType = ModContent.ProjectileType<ProjRapierBase>();
            attackDelay = 1;
        }

        public override void TownNPCAttackProjSpeed(ref float multiplier, ref float gravityCorrection, ref float randomOffset)
        {
            multiplier = 4f;
            randomOffset = 0.1f;
        }
    }
}
