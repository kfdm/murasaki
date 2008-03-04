using System;
using System.Collections.Generic;
using SdlDotNet.Graphics;
using System.Drawing;
using Murasaki.Actors;

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
        public int[,] Layer { get { return m_layer; } }

        ~CMapLayer() {
            if(!m_collideonly)
                m_surface.Dispose();
        }
        public CMapLayer(int[,] layer, int width, int height, int tilesize) {
            m_collideonly = true;
            m_layer = layer;
            m_size = new Rectangle(0, 0, width, height);
            m_tilesize = tilesize;
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
        public void MergeLayer(int[,] layer, int width, int height) {
            for (int y = 0; y < height; y++) {
                for (int x = 0; x < width; x++) {
                    if (layer[x, y] != 0)
                        m_layer[x, y] = layer[x, y];
                }
            }
        }
        public bool Collide(int x, int y) {
            if (m_layer[x, y] == 0)
                return false;
            return true;
        }
        public void Collide(CActor actor,List<CActor> remove) {
            int top, bottom, left, right;
            bool collide = false;
            top = actor.Top / m_tilesize;
            bottom = actor.Bottom / m_tilesize;
            left = actor.Left / m_tilesize;
            right = actor.Right / m_tilesize;

            //Top
            if ((m_layer[left, top]!=0) && (m_layer[right, top]!=0)) {
                actor.Top = (top + 1) * m_tilesize;
                top = actor.Top / m_tilesize;
                bottom = actor.Bottom / m_tilesize;
                collide = true;
            }
            //Bottom
            if ((m_layer[left, bottom]!=0) && (m_layer[right, bottom]!=0)) {
                actor.Bottom = (bottom * m_tilesize) - 1;
                top = actor.Top / m_tilesize;
                bottom = actor.Bottom / m_tilesize;
                collide = true;
            }
            //Left
            if ((m_layer[left, top]!=0) && (m_layer[left, bottom]!=0)) {
                actor.Left = (left + 1) * m_tilesize;
                left = actor.Left / m_tilesize;
                right = actor.Right / m_tilesize;
                collide = true;
            }
            //Right
            if ((m_layer[right, top] != 0) && (m_layer[right, bottom] != 0)) {
                actor.Right = (right * m_tilesize) - 1;
                left = actor.Left / m_tilesize;
                right = actor.Right / m_tilesize;
                collide = true;
            }
            if (collide)
                actor.CollideWall(remove);
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
