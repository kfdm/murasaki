using System.Drawing;
using SdlDotNet.Graphics;
using SdlDotNet.Input;

namespace Murasaki.Actors {
    class CActorBullet : CActor{
        public CActorBullet(string filename) {
            m_tileset = new Surface(filename);
            m_rect = new Rectangle(0, 0, m_tileset.Width / 3, m_tileset.Height / 4);
            m_tileset.Transparent = true;
            m_tileset.TransparentColor = Color.FromArgb(255, 0, 255);

            m_moveup = m_movedown = m_moveleft = m_moveright = false;
            m_movespeed = 5;
        }
        ~CActorBullet() {
            m_tileset.Dispose();
        }
        public override void CollideWall() {
            
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
