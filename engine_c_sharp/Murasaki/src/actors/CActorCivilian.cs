using System;
using System.Collections.Generic;
using System.Drawing;
using SdlDotNet.Graphics;
using SdlDotNet.Input;

namespace Murasaki.Actors {
    class CActorCivilian : CActor{
        public override bool moveup { get { return m_moveup; } set { m_moveup = value; m_direction = Key.UpArrow; } }
        public override bool movedown { get { return m_movedown; } set { m_movedown = value; m_direction = Key.DownArrow; } }
        public override bool moveleft { get { return m_moveleft; } set { m_moveleft = value; m_direction = Key.LeftArrow; } }
        public override bool moveright { get { return m_moveright; } set { m_moveright = value; m_direction = Key.RightArrow; } }
        private int ticks;
        private Random m_rand;
        public CActorCivilian(string filename, int tilex, int tiley, int tilewidth, Random rand) : this(filename,tilex,tiley,tilewidth) {
            m_rand = rand;
        }
        public CActorCivilian(string filename,int tilex, int tiley,int tilewidth) {
            m_tileset = new Surface(filename);
            m_rect = new Rectangle(tilex * tilewidth, tiley * tilewidth, m_tileset.Width / 3, m_tileset.Height / 4);

            movedown = true;
            movespeed = 2;

            m_tileset.Transparent = true;
            m_tileset.TransparentColor = Color.FromArgb(255, 0, 255);

            m_rand = new Random(DateTime.Now.Millisecond);
        }
        ~CActorCivilian() {
            m_tileset.Dispose();
        }
        private void RandomDirection() {
            switch (m_rand.Next(4)) {
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
                if (m_walkdelay > 10) {
                    m_walkdelay = 0;
                    m_walkframe = (m_walkframe + 1) % 3;
                }
                m_walkdelay++;
            }
        }
        public override void GotHit(CActor hitby, List<CActor> toRemove, List<CActor> toRemoveWeapon) {
            Console.WriteLine("Hit Civilian");
            toRemove.Add(this);
            toRemoveWeapon.Add(hitby);
        }
        public override void CollideWall(List<CActor> toRemove) {
            RandomDirection();
        }
    }
}
