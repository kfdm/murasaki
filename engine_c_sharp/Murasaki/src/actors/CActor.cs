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
        public abstract void Draw(Surface dest, Rectangle World, Rectangle Camera);
        public virtual void CollideWall() { }
        public abstract void Update();
    }
}
