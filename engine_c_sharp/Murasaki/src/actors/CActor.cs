using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using SdlDotNet.Core;
using SdlDotNet.Graphics;
using SdlDotNet.Input;

namespace Murasaki {
    public abstract class CActor {
        public virtual bool moveup { get { return m_moveup; } set { m_moveup = value; m_direction = Key.UpArrow; } }
        public virtual bool movedown { get { return m_movedown; } set { m_movedown = value; m_direction = Key.DownArrow; } }
        public virtual bool moveleft { get { return m_moveleft; } set { m_moveleft = value; m_direction = Key.LeftArrow; } }
        public virtual bool moveright { get { return m_moveright; } set { m_moveright = value; m_direction = Key.RightArrow; } }
        public virtual Rectangle Rectangle { get { return m_rect; } }
        public virtual int movespeed { get { return m_movespeed; } set { m_movespeed = value; } }
        public virtual Key direction { get { return m_direction; } }

        protected bool m_moveup, m_movedown, m_moveright, m_moveleft;
        protected int m_movespeed;
        protected Key m_direction = Key.UpArrow;
        protected int m_walkanim = 0, m_walkanim2 = 0;

        protected Surface m_tileset;
        protected Rectangle m_rect;
        public int Top {
            get { return m_rect.Top; }
            set { m_rect.Y = value; }
        }
        public int Bottom {
            get { return m_rect.Bottom; }
            set { m_rect.Y = (value - m_rect.Height); }
        }
        public int Left {
            get { return m_rect.Left; }
            set { m_rect.X = value; }
        }
        public int Right {
            get { return m_rect.Right; }
            set { m_rect.X = (value - m_rect.Width); }
        }
        public int Height {
            get { return m_rect.Height; }
        }
        public int Width {
            get { return m_rect.Width; }
        }
        public virtual void Draw(Surface dest, Rectangle Camera) {
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

            Camera.X = m_rect.X - Camera.X;
            Camera.Y = m_rect.Y - Camera.Y;
            Camera.Width = m_rect.Width;
            Camera.Height = m_rect.Height;

            dest.Blit(m_tileset, Camera, srcRect);
        }
        public virtual void CollideWall() { }
        public virtual void Update() {
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
