using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using SdlDotNet.Core;
using SdlDotNet.Graphics;
using SdlDotNet.Input;

namespace Murasaki {
    abstract class CActor {
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

        public abstract void Draw(Surface dest, Rectangle destRect);
        public abstract void Update();
    }
}
