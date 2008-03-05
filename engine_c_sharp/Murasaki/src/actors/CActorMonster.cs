using System;
using System.Collections.Generic;
using System.Drawing;
using SdlDotNet.Graphics;
using SdlDotNet.Input;
using Murasaki.Map;

namespace Murasaki.Actors {
    class CActorMonster : CActor {
        public override bool moveup { get { return m_moveup; } set { m_moveup = value; m_direction = Key.UpArrow; } }
        public override bool movedown { get { return m_movedown; } set { m_movedown = value; m_direction = Key.DownArrow; } }
        public override bool moveleft { get { return m_moveleft; } set { m_moveleft = value; m_direction = Key.LeftArrow; } }
        public override bool moveright { get { return m_moveright; } set { m_moveright = value; m_direction = Key.RightArrow; } }

        private CActorPlayer m_player;
        private int ticks = 0;
        /// <summary>
        /// Create a new monster
        /// </summary>
        /// <param name="map">Reference to the map (to be able to load certain values)</param>
        /// <param name="x">Starting X Coord in Tile units</param>
        /// <param name="y">Starting Y Coord in Tile units</param>
        public CActorMonster(CTileMap map, int x, int y) {
            m_tileset = new Surface("Data/white.png");
            m_rect = new Rectangle(x * map.TileSize, y * map.TileSize, m_tileset.Width / 3, m_tileset.Height / 4);
            movespeed = 1;
            m_tileset.Transparent = true;
            m_tileset.TransparentColor = Color.FromArgb(255, 0, 255);
            m_player = map.Avatar;
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
        public override void GotHit(CActor hitby, List<CActor> toRemove, List<CActor> toRemoveWeapon) {
            Console.WriteLine("Hit Monster");
            toRemove.Add(this);
            toRemoveWeapon.Add(hitby);
        }
    }
}
