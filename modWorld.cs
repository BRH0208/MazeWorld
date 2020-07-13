using System.IO;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.World.Generation;
using Microsoft.Xna.Framework;
using Terraria.GameContent.Generation;
using System.Linq;

namespace MazeWorld
{
	// Toolbox
    /*
    Main.maxTilesX -- Maximum X
    Main.maxTilesY -- Maximum Y
	WorldGen.clearWorld(); --Clear the whole world
    WorldGen.genRand.Next --> Random gen
    WorldGen.PlaceTile(x, y, TileType); --Place Block
    WorldGen.PlaceWall(X, Y, type); --Place Background
    WorldGen.KillWall(X, Y); --Destroy wall(Doesn't seem to actually work)
    WorldGen.KillTile(X, Y);[bool fail = false],[bool effectOnly = false],[bool noItem = false] --Destroy Block
    mod.Logger.Debug(String);

    https://github.com/tModLoader/tModLoader/wiki/Vanilla-Tile-IDs
    or
    https://terraria.gamepedia.com/Tile_IDs

	-- Make Tree --
    WorldGen.PlaceTile(X, Y, TileID.Dirt);
    WorldGen.PlaceObject(X, Y - 1, mod.TileType("MagicSapling));
    WorldGen.GrowTree(X, Y - 1);
    */
	
    public class MazeGeneration : ModWorld
    {
		private int mazeTile;
		private void wallV(int x, int startY, int endY){
			bool dir = WorldGen.genRand.NextBool();
			int possibleY = WorldGen.genRand.Next(startY,endY);
			while(Main.tile[x-1, possibleY].active() || Main.tile[x-1, possibleY+1].active() || Main.tile[x-1, possibleY-1].active() || Main.tile[x+1, possibleY].active() || Main.tile[x+1, possibleY+1].active() || Main.tile[x+1, possibleY-1].active()){
				if(possibleY <= startY){
					dir = true;
				}
				if(possibleY >= endY){
					dir = false;
				}
				if(dir){
					possibleY++;
				}else{
					possibleY--;
				}
			}
			WorldGen.KillTile(x, possibleY, false,false,true);
			WorldGen.KillTile(x, possibleY-1, false,false,true);
			WorldGen.KillTile(x, possibleY+1, false,false,true);
			if(!Main.tile[x-1, possibleY+2].active() && !Main.tile[x+1, possibleY+2].active()){
				WorldGen.KillTile(x, possibleY+2, false,false,true);
			}
			if(!Main.tile[x-1, possibleY-2].active() && !Main.tile[x+1, possibleY-2].active()){
				WorldGen.KillTile(x, possibleY-2, false,false,true);
			}
		}
		private void wallH(int y, int startX, int endX){
			bool dir = WorldGen.genRand.NextBool();
			int possibleX = WorldGen.genRand.Next(startX,endX);
			while(Main.tile[possibleX, y-1].active() || Main.tile[possibleX+1, y-1].active() || Main.tile[possibleX, y+1].active() || Main.tile[possibleX+1, y+1].active()){
				if(possibleX <= startX){
					dir = true;
				}
				if(possibleX >= endX){
					dir = false;
				}
				if(dir){
					possibleX++;
				}else{
					possibleX--;
				}
			}
			WorldGen.KillTile(possibleX, y, false,false,true);
			WorldGen.KillTile(possibleX+1, y, false,false,true);
		}
		private void makeMaze(int startX, int startY, int endX,int endY, int terminator){
			
			if(terminator <= 0){
				return;
			}
			if(endX - startX <= endY - startY){
				if(endY - startY < 7){
					//mod.Logger.Debug("Cannot Slice: Too smol");
				}else{
					//mod.Logger.Debug("Slice ("+endY +"-" +startY+")");
					for(int x1 = startX; x1 < endX; x1++){
						WorldGen.PlaceTile(x1, (endY + startY)/2,mazeTile);
					}
					makeMaze(startX,startY,endX,(endY + startY)/2,terminator-1);
					makeMaze(startX,(endY + startY)/2,endX,endY,terminator-1);
					wallH((endY + startY)/2,startX,endX);
				}
			}
			else if(endX - startX >= endY - startY){
				if(endX - startX < 6){
					//mod.Logger.Debug("Cannot Dice: Too smol");
				}else{
					//mod.Logger.Debug("Dice ("+endX +"-" +startX+")");
					for(int y1 = startY; y1 < endY; y1++){
						WorldGen.PlaceTile((endX + startX)/2, y1, mazeTile);
					
					}
					makeMaze(startX,startY,(endX + startX)/2,endY,terminator-1);
					makeMaze((endX + startX)/2,startY,endX,endY,terminator-1);
					wallV((endX + startX)/2,startY,endY);
				}
			}
		}
		
		private void clear(int startX, int startY, int endX,int endY){
			for(int x1 = startX; x1 < endX; x1++){
				for(int y1 = startX; y1 < endX; y1++){
					WorldGen.KillTile(x1, y1,false, false, true);
				}
			}
		}
		
		
        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref float totalWeight)
        {
			mazeTile = mod.TileType("MazeBlock");
			tasks.RemoveRange(1, tasks.Count - 1);
            int genIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Micro Biomes"));
			//int spawnIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Spawn Point"));
			//tasks.RemoveRange(1, tasks.Count);
            tasks.Add(new PassLegacy("Spawn Point", delegate {
				Main.spawnTileX = 44;
				Main.spawnTileY = 45;
			}));
			tasks.Insert(genIndex + 1, new PassLegacy("Maze Generation", delegate (GenerationProgress progress)
            {
                progress.Message = "Making a maze";
				makeMaze(41,41,Main.maxTilesX-41,Main.maxTilesY-41,50);
            }));
			tasks.Insert(genIndex + 1, new PassLegacy("Maze Generation", delegate (GenerationProgress progress)
            {
                progress.Message = "Undoing all of that good work making a world";
				WorldGen.clearWorld();
            }));
			
        }
    }
}