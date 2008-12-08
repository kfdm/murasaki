using System.Collections.Generic;
using System.Drawing;
using SdlDotNet.Graphics;
using Murasaki.Map;

namespace Murasaki.Actors {
	class CActorBullet : CActor {
		public CActorBullet(CMap map, int x, int y, ActorDirection direction)
			: this(map, x, y) {
			switch (direction) {
				case ActorDirection.Up:
					moveup = true;
					break;
				case ActorDirection.Down:
					movedown = true;
					break;
				case ActorDirection.Left:
					moveleft = true;
					break;
				case ActorDirection.Right:
					moveright = true;
					break;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="map"></param>
		/// <param name="x">X Coord in World Units</param>
		/// <param name="y">Y Coord in World Units</param>
		public CActorBullet(CMap map, int x, int y) {
			m_tileset = new Surface("Data/bullet.png");
			m_rect = new Rectangle(x, y, m_tileset.Width / 3, m_tileset.Height / 4);
			m_tileset.Transparent = true;
			m_tileset.TransparentColor = Color.FromArgb(255, 0, 255);
			m_moveup = m_movedown = m_moveleft = m_moveright = false;
			m_movespeed = 5;
			m_map = map;
		}
		~CActorBullet() {
			m_tileset.Dispose();
		}
		public override void CollideWall(List<CActor> toRemove) {
			toRemove.Add(this);
		}
		public override void Draw(Surface dest, Rectangle Camera) {
			Camera.X = m_rect.X - Camera.X;
			Camera.Y = m_rect.Y - Camera.Y;
			Camera.Width = m_rect.Width;
			Camera.Height = m_rect.Height;

			dest.Blit(m_tileset, Camera);
		}
	}
}
