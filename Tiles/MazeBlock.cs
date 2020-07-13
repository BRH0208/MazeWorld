using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace MazeWorld.Tiles
{
    public class MazeBlock : ModTile
    {
        public override void SetDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
			drop = 2;
            AddMapEntry(new Color(79, 79, 79));

            minPick = 30000;

        }
		public override bool CanExplode(int x, int y){return false;}
		public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b) {
			r = 0.5f;
			g = 0.5f;
			b = 0.5f;
		}
    }
}