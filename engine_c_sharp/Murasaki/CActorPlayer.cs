using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using SdlDotNet.Core;
using SdlDotNet.Graphics;
using SdlDotNet.Input;

namespace Murasaki {
    class CActorPlayer : CActor {
        public override bool moveup { get { return m_moveup; } set { m_moveup = value; setDirection(); } }
        public override bool movedown { get { return m_movedown; } set { m_movedown = value; setDirection(); } }
        public override bool moveleft { get { return m_moveleft; } set { m_moveleft = value; setDirection(); } }
        public override bool moveright { get { return m_moveright; } set { m_moveright = value; setDirection(); } }

        public CActorPlayer(string filename) {
            m_tileset = new Surface(filename);
            m_rect = new Rectangle(0, 0, m_tileset.Width / 3, m_tileset.Height / 4);
            m_tileset.Transparent = true;
            m_tileset.TransparentColor = Color.FromArgb(255, 0, 255);

            m_moveup = m_movedown = m_moveleft = m_moveright = false;
            m_movespeed = 2;
        }
        ~CActorPlayer() {
            m_tileset.Dispose();
        }
        public void setDirection(KeyboardEventArgs key) {
            m_direction = key.Key;
        }
        public void setDirection() {
            if (moveup)
                m_direction = Key.UpArrow;
            if (moveleft)
                m_direction = Key.LeftArrow;
            if (movedown)
                m_direction = Key.DownArrow;
            if (moveright)
                m_direction = Key.RightArrow;
        }
        public override void Draw(Surface dest, Rectangle world, Rectangle camera) {
            Console.WriteLine(world.ToString() + " " + camera.ToString());
            //Center over camera
            camera.X = (camera.Width / 2) - m_rect.Width / 2;
            camera.Y = (camera.Height / 2) - m_rect.Height / 2;
            camera.Height = m_rect.Height;
            camera.Width = m_rect.Width;

            Rectangle srcRect = new Rectangle(0, 0, m_rect.Width, m_rect.Height);
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

            dest.Blit(m_tileset, camera,srcRect);
        }
        public override void Update() {
            if (moveup)
                Top -= movespeed;
            if (movedown)
                Top += movespeed;
            if (moveleft)
                Left -= movespeed;
            if (moveright)
                Left += movespeed;
            if (moveup || movedown || moveleft || moveright) {
                if (m_walkanim2 > 10) {
                    m_walkanim2 = 0;
                    m_walkanim = (m_walkanim + 1) % 3;
                }
                m_walkanim2++;
            }
        }
    }
}
