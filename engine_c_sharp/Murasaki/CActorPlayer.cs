using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using SdlDotNet.Core;
using SdlDotNet.Graphics;
using SdlDotNet.Input;

namespace Murasaki {
    class CActorPlayer : CActor {
        public bool moveup { get { return m_moveup; } set { m_moveup = value; } }
        public bool movedown { get { return m_movedown; } set { m_movedown = value; } }
        public bool moveleft { get { return m_moveleft; } set { m_moveleft = value; } }
        public bool moveright { get { return m_moveright; } set { m_moveright = value; } }
        public int movespeed { get { return m_movespeed; } set { m_movespeed = value; } }

        private bool m_moveup,m_movedown,m_moveright,m_moveleft;
        private int m_movespeed;
        public CActorPlayer(string filename) {
            m_tileset = new Surface(filename);
            m_rect = new Rectangle(0, 0, m_tileset.Width, m_tileset.Height);
            m_tileset.Transparent = true;
            m_tileset.TransparentColor = Color.FromArgb(255, 0, 255);

            m_moveup = m_movedown = m_moveleft = m_moveright = false;
            m_movespeed = 2;
        }
        public override void Draw(Surface dest, Rectangle destRect) {
            dest.Blit(m_tileset, destRect);
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
        }
    }
}
