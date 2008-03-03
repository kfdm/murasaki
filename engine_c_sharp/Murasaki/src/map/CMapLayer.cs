using System;
using System.Collections.Generic;
using System.Text;
using SdlDotNet.Core;
using SdlDotNet.Input;
using SdlDotNet.Graphics;
using SdlDotNet.Graphics.Sprites;
using System.Drawing;

namespace Murasaki.Map {
    class CMapLayer {
        private Surface m_surface;
        private int[,] m_layer;
        private Rectangle m_size;
        private CTileSet m_tileset;
        private int m_tilesize;
        private bool m_collideonly;

        public Rectangle Rectangle { get { return m_size; } }
        public int Height { get { return m_size.Height; } }
        public int Width { get { return m_size.Width; } }

        ~CMapLayer() {
            if(!m_collideonly)
                m_surface.Dispose();
        }
        public CMapLayer(int[,] layer, int width, int height) {
            m_collideonly = true;
            m_layer = layer;
            m_size = new Rectangle(0, 0, width, height);
        }
        public CMapLayer(int[,] layer, int width, int height, CTileSet tileset) {
            m_collideonly = false;
            m_layer = layer;
            m_tileset = tileset;
            m_tilesize = m_tileset.TileSize;
            m_size = new Rectangle(0, 0, width, height);
            m_surface = new Surface(width * tileset.TileSize, height * tileset.TileSize);
            ReDraw();
        }
        public bool Collide(int x, int y) {
            if (m_layer[x, y] == 0)
                return false;
            return true;
        }
        public void Draw(Surface dest, Rectangle camera) {
            if (m_collideonly)
                throw new Exception("Tried to draw a collision layer");
            Rectangle destRect = new Rectangle(0, 0, camera.Width, camera.Height);
            dest.Blit(m_surface, destRect, camera);
        }
        public void ReDraw() {
            m_surface.Fill(Color.Purple);
            m_surface.Transparent = true;
            m_surface.TransparentColor = Color.Purple;

            Rectangle destRect, srcRect;
            for (int y = 0; y < m_size.Height; y++) {
                for (int x = 0; x < m_size.Width; x++) {
                    if (m_layer[x, y] != 0) {
                        srcRect = m_tileset.GetTile(m_layer[x, y]);
                        destRect = new Rectangle(x * m_tilesize, y * m_tilesize, m_tilesize, m_tilesize);
                        m_surface.Blit(m_tileset.Surface, destRect, srcRect);
                    }
                }
            }
        }
    }
}
