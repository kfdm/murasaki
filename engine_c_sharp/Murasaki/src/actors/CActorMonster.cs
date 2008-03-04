using System;
using System.Collections.Generic;
using System.Drawing;
using SdlDotNet.Graphics;
using SdlDotNet.Input;

namespace Murasaki.Actors {
    class CActorMonster : CActor {
        public override bool moveup { get { return m_moveup; } set { m_moveup = value; m_direction = Key.UpArrow; } }
        public override bool movedown { get { return m_movedown; } set { m_movedown = value; m_direction = Key.DownArrow; } }
        public override bool moveleft { get { return m_moveleft; } set { m_moveleft = value; m_direction = Key.LeftArrow; } }
        public override bool moveright { get { return m_moveright; } set { m_moveright = value; m_direction = Key.RightArrow; } }

        private CActorPlayer m_player;
        private int ticks = 0;
        public CActorMonster(string filename, int x, int y, int tilewidth,CActorPlayer player) {
            m_tileset = new Surface("Data/white.png");
            m_rect = new Rectangle(x * tilewidth, y * tilewidth, m_tileset.Width / 3, m_tileset.Height / 4);
            movespeed = 1;
            m_tileset.Transparent = true;
            m_tileset.TransparentColor = Color.FromArgb(255, 0, 255);
            m_player = player;
        }
        ~CActorMonster() {
            m_tileset.Dispose();
        }
        public override void Update() {
            ticks++;
            if (ticks % 60 == 0)
                CheckDirection();
            if (m_moveup)
                Top -= movespeed;
            if (m_movedown)
                Top += movespeed;
            if (m_moveleft)
                Left -= movespeed;
            if (m_moveright)
                Left += movespeed;
            //Animate
            if ((m_direction == Key.UpArrow) ||
                (m_direction == Key.DownArrow) ||
                (m_direction == Key.LeftArrow) ||
                (m_direction == Key.RightArrow)) {
                if (m_walkdelay > 10) {
                    m_walkdelay = 0;
                    m_walkframe = (m_walkframe + 1) % 3;
                }
                m_walkdelay++;
            }
        }
        private void CheckDirection() {
            ticks = 0;
            m_moveright = m_moveleft = m_movedown = m_moveup = false;
            if (m_player.Top > m_rect.Top)
                movedown = true;
            if (m_player.Top < m_rect.Top)
                moveup = true;
            if (m_player.Left > m_rect.Left)
                moveright = true;
            if (m_player.Left < m_rect.Left)
                moveleft = true;
        }
        public override void CollideWall(List<CActor> toRemove) {
            CheckDirection();
        }
    }
}
