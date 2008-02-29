using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using SdlDotNet.Core;
using SdlDotNet.Graphics;
using SdlDotNet.Input;

namespace Murasaki {
    class CActorCivilian : CActor{
        public override bool moveup { get { return m_moveup; } set { m_moveup = value; m_direction = Key.UpArrow; } }
        public override bool movedown { get { return m_movedown; } set { m_movedown = value; m_direction = Key.DownArrow; } }
        public override bool moveleft { get { return m_moveleft; } set { m_moveleft = value; m_direction = Key.LeftArrow; } }
        public override bool moveright { get { return m_moveright; } set { m_moveright = value; m_direction = Key.RightArrow; } }
        private int ticks;
        public CActorCivilian(string filename,int tilex, int tiley,int tilewidth) {
            m_tileset = new Surface(filename);
            m_rect = new Rectangle(tilex * tilewidth, tiley * tilewidth, m_tileset.Width / 3, m_tileset.Height / 4);

            movedown = true;
            movespeed = 2;

            m_tileset.Transparent = true;
            m_tileset.TransparentColor = Color.FromArgb(255, 0, 255);
        }
        ~CActorCivilian() {
            m_tileset.Dispose();
        }
        private void RandomDirection() {
            Random rand = new Random();
            switch (rand.Next(4)) {
                case 0:
                    moveup = true;
                    break;
                case 1:
                    movedown = true;
                    break;
                case 2:
                    moveleft = true;
                    break;
                case 3:
                    moveright = true;
                    break;
            }
        }
        public override void Update() {
            ticks++;
            if (ticks % 60 == 0)
                RandomDirection();

            if (m_direction==Key.UpArrow)
                Top -= movespeed;
            if (m_direction==Key.DownArrow)
                Top += movespeed;
            if (m_direction==Key.LeftArrow)
                Left -= movespeed;
            if (m_direction==Key.RightArrow)
                Left += movespeed;
            if ((m_direction==Key.UpArrow) ||
                (m_direction==Key.DownArrow) ||
                (m_direction==Key.LeftArrow) ||
                (m_direction==Key.RightArrow)) {
                if (m_walkanim2 > 10) {
                    m_walkanim2 = 0;
                    m_walkanim = (m_walkanim + 1) % 3;
                }
                m_walkanim2++;
            }
        }
        public override void CollideWall() {
            RandomDirection();
        }
        public override void Draw(Surface dest, Rectangle world, Rectangle Camera) {
            Rectangle srcRect = new Rectangle(0, m_rect.Height * 2, m_rect.Width, m_rect.Height);
            switch (m_direction) {
                case Key.UpArrow:
                    srcRect.Y = 0;
                    break;
                case Key.DownArrow:
                    srcRect.Y = m_rect.Height * 2;
                    break;
                case Key.LeftArrow:
                    srcRect.Y = m_rect.Height * 3;
                    break;
                case Key.RightArrow:
                    srcRect.Y = m_rect.Height;
                    break;
            }
            srcRect.X = m_rect.Width * m_walkanim;

            Camera.X = m_rect.X - world.X - Camera.X;
            Camera.Y = m_rect.Y - world.Y - Camera.Y;
            Camera.Width = m_rect.Width;
            Camera.Height = m_rect.Height;

            dest.Blit(m_tileset, Camera, srcRect);
        }
    }
}
