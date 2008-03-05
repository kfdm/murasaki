using System.Collections.Generic;
using System.Drawing;
using SdlDotNet.Graphics;
using SdlDotNet.Input;
using Murasaki.Map;

namespace Murasaki.Actors {
    class CActorBullet : CActor{
        public CActorBullet(CTileMap map, int x, int y, Key direction)
            : this(map, x, y) {
            switch (direction) {
                case Key.UpArrow:
                    moveup = true;
                    break;
                case Key.DownArrow:
                    movedown = true;
                    break;
                case Key.LeftArrow:
                    moveleft = true;
                    break;
                case Key.RightArrow:
                    moveright = true;
                    break;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="map"></param>
        /// <param name="x">X Coord in Tile Units</param>
        /// <param name="y">Y Coord in Tile Units</param>
        public CActorBullet(CTileMap map,int x, int y) {
            m_tileset = new Surface("Data/bullet.png");
            m_rect = new Rectangle(x * map.TileSize, y * map.TileSize, m_tileset.Width / 3, m_tileset.Height / 4);
            m_tileset.Transparent = true;
            m_tileset.TransparentColor = Color.FromArgb(255, 0, 255);
            m_moveup = m_movedown = m_moveleft = m_moveright = false;
            m_movespeed = 5;
        }
        ~CActorBullet() {
            m_tileset.Dispose();
        }
        public override void CollideWall(List<CActor> toRemove) {
            toRemove.Add(this);
        }
        public override void Draw(Surface dest, Rectangle Camera) {
            Camera.X = m_rect.X - Camera.X;
            Camera.Y = m_rect.Y - Camera.Y;
            Camera.Width = m_rect.Width;
            Camera.Height = m_rect.Height;

            dest.Blit(m_tileset, Camera);
        }
        public override void Update() {
            switch (m_direction) {
                case Key.UpArrow:
                    m_rect.Y -= m_movespeed;
                    break;
                case Key.DownArrow:
                    m_rect.Y += m_movespeed;
                    break;
                case Key.LeftArrow:
                    m_rect.X -= m_movespeed;
                    break;
                case Key.RightArrow:
                    m_rect.X += m_movespeed;
                    break;
            }
        }
    }
}
