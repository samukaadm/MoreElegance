using System;
using Microsoft.Xna.Framework;
using MoreElegance.Content.Tiles;
using Terraria;
using Terraria.Audio;
using Terraria.Chat;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace MoreElegance.Common.Systems
{
    /// <summary>
    /// ModSystem managing endgame state checks and the massive world-reconstruction event.
    /// Reconstructs the forest spawn town into a majestic Aristocratic Palace when Moon Lord is downed
    /// and all world corruption/crimson is eradicated (0 evil tiles).
    /// </summary>
    public class WorldUpdateSystem : ModSystem
    {
        public static bool HasReconstructedWorld { get; set; } = false;

        public override void SaveWorldData(TagCompound tag)
        {
            tag["HasReconstructedWorld"] = HasReconstructedWorld;
        }

        public override void LoadWorldData(TagCompound tag)
        {
            HasReconstructedWorld = tag.GetBool("HasReconstructedWorld");
        }

        public override void ClearWorld()
        {
            HasReconstructedWorld = false;
        }

        /// <summary>
        /// Evaluates whether the world's Corruption and Crimson tile counts are completely eradicated (0).
        /// WorldGen.corruptTiles and WorldGen.crimsonTiles represent the aggregated evil tile metrics.
        /// </summary>
        public static bool IsWorldCompletelyPurified()
        {
            return WorldGen.corruptTiles <= 0 && WorldGen.crimsonTiles <= 0;
        }

        /// <summary>
        /// Checks if the two core conditions for the endgame reconstruction event are met:
        /// 1. Hardmode & Moon Lord defeated (Main.hardMode && NPC.downedMoonlord).
        /// 2. World Corruption / Crimson tile count is exactly 0.
        /// </summary>
        public static bool CanTriggerReconstruction()
        {
            return Main.hardMode && NPC.downedMoonlord && IsWorldCompletelyPurified() && !HasReconstructedWorld;
        }

        /// <summary>
        /// Executes the dynamic endgame world reconstruction.
        /// Demolishes makeshift wooden housing in the Forest spawn biome and erects
        /// a grand Aristocratic Palace with duel halls, training blocks, and marble columns.
        /// </summary>
        public static void TriggerReconstruction()
        {
            if (HasReconstructedWorld)
                return;

            HasReconstructedWorld = true;

            // Find world spawn anchor
            int originX = Main.spawnTileX;
            int originY = Main.spawnTileY;

            // Find true ground surface below spawn point
            int groundY = originY;
            while (groundY < Main.maxTilesY - 200 && !Main.tile[originX, groundY].HasTile)
            {
                groundY++;
            }

            // Build the Grand Aristocratic Palace
            ConstructGrandPalace(originX, groundY);

            // Announce world event
            string announcement = "★ The world has been cleansed of all darkness! The General's Grand Palace of Elegance has been erected! ★";
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                Main.NewText(announcement, 245, 215, 110);
            }
            else if (Main.netMode == NetmodeID.Server)
            {
                ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral(announcement), new Color(245, 215, 110));
            }

            // Play celebratory orchestral fanfare and seismic feedback
            SoundEngine.PlaySound(SoundID.Roar, new Vector2(originX * 16, groundY * 16));
            SoundEngine.PlaySound(SoundID.Item29, new Vector2(originX * 16, groundY * 16));
        }

        /// <summary>
        /// Procedural WorldGen logic: clears old wood boxes and generates an ornate palace.
        /// </summary>
        private static void ConstructGrandPalace(int centerX, int groundY)
        {
            const int halfWidth = 45;
            const int palaceHeight = 32;
            int startX = centerX - halfWidth;
            int endX = centerX + halfWidth;
            int floorY = groundY;
            int roofY = groundY - palaceHeight;

            int trainingTile = ModContent.TileType<TrainingTile>();
            ushort marbleBlock = TileID.MarbleBlock;
            ushort smoothMarble = TileID.SmoothMarble;
            ushort goldBrick = TileID.GoldBrick;
            ushort glass = TileID.Glass;
            ushort marbleWall = WallID.MarbleBlock;
            ushort glassWall = WallID.Glass;
            ushort goldWall = WallID.GoldBrick;

            // Step 1: Clear area of makeshift wood boxes, dirt, and debris
            for (int x = startX - 2; x <= endX + 2; x++)
            {
                for (int y = roofY - 5; y <= floorY + 4; y++)
                {
                    Tile tile = Framing.GetTileSafely(x, y);

                    // Remove existing cheap tiles and walls
                    tile.HasTile = false;
                    tile.WallType = WallID.None;
                    tile.LiquidAmount = 0;
                }
            }

            // Step 2: Foundation (Reinforced TrainingTile & Smooth Marble)
            for (int x = startX - 2; x <= endX + 2; x++)
            {
                for (int y = floorY; y <= floorY + 3; y++)
                {
                    Tile tile = Framing.GetTileSafely(x, y);
                    tile.HasTile = true;
                    tile.TileType = (ushort)(y == floorY ? smoothMarble : trainingTile);
                    tile.Slope = SlopeType.Solid;
                }
            }

            // Step 3: Outer Walls and Grand Arched Roof
            for (int x = startX; x <= endX; x++)
            {
                for (int y = roofY; y < floorY; y++)
                {
                    Tile tile = Framing.GetTileSafely(x, y);
                    bool isOuterWall = (x == startX || x == endX);
                    bool isRoof = (y == roofY);

                    if (isOuterWall)
                    {
                        tile.HasTile = true;
                        tile.TileType = marbleBlock;
                    }
                    else if (isRoof)
                    {
                        tile.HasTile = true;
                        tile.TileType = goldBrick;
                    }
                    else
                    {
                        // Background walls: Alternating marble with tall glass windows
                        int relativeX = Math.Abs(x - centerX);
                        if (relativeX % 10 is >= 3 and <= 7 && y > roofY + 6 && y < floorY - 3)
                        {
                            tile.WallType = glassWall;
                        }
                        else if (y < roofY + 4)
                        {
                            tile.WallType = goldWall;
                        }
                        else
                        {
                            tile.WallType = marbleWall;
                        }
                    }
                }
            }

            // Step 4: Multi-tier Floors (Grand Dueling Floor & Upper Suites)
            int middleFloorY = floorY - 14;
            for (int x = startX + 1; x < endX; x++)
            {
                // Leave a central open atrium for fencing acrobatics
                if (Math.Abs(x - centerX) > 12)
                {
                    Tile tile = Framing.GetTileSafely(x, middleFloorY);
                    tile.HasTile = true;
                    tile.TileType = smoothMarble;
                }
            }

            // Step 5: Marble Columns flanking the main hall
            int[] columnPositions = { centerX - 30, centerX - 15, centerX + 15, centerX + 30 };
            foreach (int colX in columnPositions)
            {
                for (int y = roofY + 1; y < floorY; y++)
                {
                    Tile tile = Framing.GetTileSafely(colX, y);
                    tile.HasTile = true;
                    tile.TileType = TileID.MarbleColumn;
                }
            }

            // Step 6: Furnishings and Lighting (Doors, Torches, Chandeliers, Dummies)
            WorldGen.PlaceTile(startX, floorY - 1, TileID.ClosedDoor, mute: true, forced: true, style: 24); // Marble Door
            WorldGen.PlaceTile(endX, floorY - 1, TileID.ClosedDoor, mute: true, forced: true, style: 24);

            // Chandeliers along the high ceiling
            WorldGen.PlaceTile(centerX, roofY + 1, TileID.Chandeliers, mute: true, forced: true, style: 15); // Marble Chandelier
            WorldGen.PlaceTile(centerX - 20, roofY + 1, TileID.Chandeliers, mute: true, forced: true, style: 15);
            WorldGen.PlaceTile(centerX + 20, roofY + 1, TileID.Chandeliers, mute: true, forced: true, style: 15);

            // Fencing Training Dummies in the central arena
            WorldGen.PlaceTile(centerX - 5, floorY - 1, TileID.TargetDummy, mute: true, forced: true);
            WorldGen.PlaceTile(centerX + 5, floorY - 1, TileID.TargetDummy, mute: true, forced: true);

            // Noble Tables & Chairs for Town NPC habitation
            WorldGen.PlaceTile(centerX - 24, middleFloorY - 1, TileID.Tables, mute: true, forced: true, style: 27);
            WorldGen.PlaceTile(centerX - 22, middleFloorY - 1, TileID.Chairs, mute: true, forced: true, style: 29);
            WorldGen.PlaceTile(centerX + 24, middleFloorY - 1, TileID.Tables, mute: true, forced: true, style: 27);
            WorldGen.PlaceTile(centerX + 26, middleFloorY - 1, TileID.Chairs, mute: true, forced: true, style: 29);

            // Step 7: Safely relocate any NPCs that were inside the construction area
            Rectangle palaceBounds = new Rectangle(startX * 16, roofY * 16, (endX - startX) * 16, (floorY - roofY) * 16);
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && npc.townNPC && palaceBounds.Contains(npc.Center.ToPoint()))
                {
                    npc.position = new Vector2(centerX * 16, (floorY - 4) * 16);
                    npc.netUpdate = true;
                }
            }

            // Sync tiles in multiplayer
            if (Main.netMode == NetmodeID.Server)
            {
                NetMessage.SendTileSquare(-1, startX - 2, roofY - 5, (halfWidth * 2) + 5, palaceHeight + 10);
            }
        }
    }
}
