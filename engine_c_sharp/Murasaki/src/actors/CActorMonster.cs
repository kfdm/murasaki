using System;
using System.Collections.Generic;
using System.Drawing;
using SdlDotNet.Graphics;
using Murasaki.Map;

namespace Murasaki.Actors {
    class CActorMonster : CActor {
        public override bool moveup { get { return m_moveup; } set { m_moveup = value; m_direction = ActorDirection.Up; } }
        public override bool movedown { get { return m_movedown; } set { m_movedown = value; m_direction = ActorDirection.Down; } }
        public override bool moveleft { get { return m_moveleft; } set { m_moveleft = value; m_direction = ActorDirection.Left; } }
        public override bool moveright { get { return m_moveright; } set { m_moveright = value; m_direction = ActorDirection.Right; } }

        private CActorPlayer m_player;
        private int ticks = 0;
        private const int tickdelay = 60;
        private const int tickfiredelay = 80;
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
            m_map = map;
            m_collidable = true;
        }
        ~CActorMonster() {
            m_tileset.Dispose();
        }
        /// <summary>
        /// Fire a weapon at the avatar
        /// </summary>
        private void Fire() {
            CActorBullet tmp = new CActorBullet(m_map, m_rect.X, m_rect.Y, m_direction);
            m_map.EnemyWeapons.AddLast(tmp);
        }

        public override void Update() {
            ticks++;
            if (ticks % tickdelay == 0)
                CheckDirection();
            if (ticks % tickfiredelay == 0)
                if (m_map.Camera.IntersectsWith(m_rect))
                    Fire();
            base.Update();
        }
        /// <summary>
        /// If the player is on the screen, check the direction we want to go
        /// </summary>
        private void CheckDirection() {
            m_direction = ActorDirection.Stop;
            m_moveright = m_moveleft = m_movedown = m_moveup = false;
            if (m_map.Camera.IntersectsWith(m_rect)) {
                if (m_player.Top > m_rect.Top)
                    movedown = true;
                if (m_player.Top < m_rect.Top)
                    moveup = true;
                if (m_player.Left > m_rect.Left)
                    moveright = true;
                if (m_player.Left < m_rect.Left)
                    moveleft = true;
            }
        }
        /// <summary>
        /// If the monster hits a wall, check our bearings
        /// </summary>
        /// <param name="toRemove">Add to list to remove from world</param>
        public override void CollideWall(List<CActor> toRemove) {
            CheckDirection();
        }
        /// <summary>
        /// Actor was hit
        /// </summary>
        /// <param name="hitby">Actor was hit by this</param>
        /// <param name="toRemove">List to remove actor</param>
        /// <param name="toRemoveWeapon">List to remove hitby</param>
        public override void GotHit(CActor hitby, List<CActor> toRemove, List<CActor> toRemoveWeapon) {
            Console.WriteLine("Hit Monster");
            toRemove.Add(this);
            toRemoveWeapon.Add(hitby);
        }
    }
}
