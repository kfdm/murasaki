using SdlDotNet.Graphics;
using SdlDotNet.Graphics.Sprites;
using System.Drawing;

namespace Murasaki.Map {
    public class CTileSet {
        private int m_numx, m_numy,m_tilesize;
        private Sprite m_tilemap;

        public Surface Surface { get { return m_tilemap.Surface; } }
        public int TileSize { get { return m_tilesize; } }

        public CTileSet(string filename) : this(filename, 24, Color.FromArgb(255, 0, 255)) { }
        public CTileSet(string filename, int tilesize) : this(filename, tilesize, Color.FromArgb(255, 0, 255)) { }
        public CTileSet(string filename,int tilesize,Color transparent) {
            m_tilemap   = new Sprite(filename);
            m_tilemap.Transparent = true;
            m_tilemap.TransparentColor = transparent;
            m_tilesize  = tilesize;
            m_numx      = m_tilemap.Width / m_tilesize;
            m_numy      = m_tilemap.Height / m_tilesize;
        }
        ~CTileSet() {
            m_tilemap.Dispose();
        }
        public Rectangle GetTile(int tile) {
            int tilex = ((tile % m_numx)-1) * m_tilesize;
            int tiley = ((tile / m_numx)) * m_tilesize;
            return new Rectangle(tilex, tiley, m_tilesize, m_tilesize);
        }
        public Surface GetSurface() {
            return m_tilemap.Surface;
        }
    }
}
