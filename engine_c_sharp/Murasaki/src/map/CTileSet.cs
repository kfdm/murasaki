using System;
using System.Collections.Generic;
using System.Text;
using SdlDotNet.Core;
using SdlDotNet.Graphics;
using SdlDotNet.Graphics.Sprites;
using System.Drawing;

namespace Murasaki {
    class CTileSet {
        private const int TILE_SIZE = 48;
        private int m_numx, m_numy;
        public Sprite tilemap;
        public CTileSet(string filename) {
            tilemap = new Sprite(filename);
            m_numx = tilemap.Width / TILE_SIZE;
            m_numy = tilemap.Height / TILE_SIZE;
        }
        public Rectangle GetTile(int tile) {
            int tilex = (tile % m_numx)*TILE_SIZE;
            int tiley = (tile / m_numy)*TILE_SIZE;
            return new Rectangle(tilex, tiley, TILE_SIZE, TILE_SIZE);
        }
    }
}
