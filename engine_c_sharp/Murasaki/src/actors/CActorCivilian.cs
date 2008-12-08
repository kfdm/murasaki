using System;
using System.Collections.Generic;
using System.Drawing;
using SdlDotNet.Graphics;
using Murasaki.Map;

namespace Murasaki.Actors {
	class CActorCivilian : CActor {
		public override bool moveup { get { return m_moveup; } set { m_moveup = value; m_direction = ActorDirection.Up; } }
		public override bool movedown { get { return m_movedown; } set { m_movedown = value; m_direction = ActorDirection.Down; } }
		public override bool moveleft { get { return m_moveleft; } set { m_moveleft = value; m_direction = ActorDirection.Left; } }
		public override bool moveright { get { return m_moveright; } set { m_moveright = value; m_direction = ActorDirection.Right; } }
		private int ticks;
		private Random m_rand;
		/// <summary>
		/// 
		/// </summary>
		/// <param name="map"></param>
		/// <param name="x">X Coord in Tile Units</param>
		/// <param name="y">Y Coord in Tile Units</param>
		public CActorCivilian(CTileMap map, int x, int y) {
			m_tileset = new Surface("data/pink.png");
			m_tileset.Transparent = true;
			m_tileset.TransparentColor = Color.FromArgb(255, 0, 255);
			m_rect = new Rectangle(x * map.TileSize, y * map.TileSize, m_tileset.Width / 3, m_tileset.Height / 4);
			movedown = true;
			MoveSpeed = 2;
			m_rand = map.RandomGenerator;
			m_map = map;
			m_collidable = true;
		}
		~CActorCivilian() {
			m_tileset.Dispose();
		}
		private void RandomDirection() {
			moveup = movedown = moveleft = moveright = false;
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
			base.Update();
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
