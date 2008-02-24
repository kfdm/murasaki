using System;
using System.Collections.Generic;
using System.Text;
using SdlDotNet.Core;
using SdlDotNet.Graphics;
using SdlDotNet.Graphics.Sprites;
using System.Drawing;

namespace Murasaki {
    class CTileSet {
        private int m_numx, m_numy,m_tilesize;
        public Sprite m_tilemap;
        public CTileSet(string filename) : this(filename, 24) { }
        public CTileSet(string filename,int tilesize) {
            m_tilemap   = new Sprite(filename);
            m_tilesize  = tilesize;
            m_numx      = m_tilemap.Width / m_tilesize;
            m_numy      = m_tilemap.Height / m_tilesize;
        }
        public Rectangle GetTile(int tile) {
            int tilex = ((tile % m_numx)-1) * m_tilesize;
            int tiley = ((tile / m_numx)) * m_tilesize;
            return new Rectangle(tilex, tiley, m_tilesize, m_tilesize);
        }
    }
}
