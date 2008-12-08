using System;
using SdlDotNet.Graphics;
using System.Drawing;
using Murasaki.Actors;

namespace Murasaki.Map {
	class CMapLayer {
		private Surface m_surface;
		private int[,] m_layer;
		/// <summary>
		/// Size in Tile Units
		/// </summary>
		private Size m_size;
		private CTileSet m_tileset;
		private int m_tilesize;
		private bool m_collideonly;

		~CMapLayer() {
			if (!m_collideonly)
				m_surface.Dispose();
		}
		public CMapLayer(int[,] layer, int width, int height, int tilesize) {
			m_collideonly = true;
			m_layer = layer;
			m_size = new Size(width, height);
			m_tilesize = tilesize;
		}
		public CMapLayer(int[,] layer, int width, int height, CTileSet tileset) {
			m_collideonly = false;
			m_layer = layer;
			m_tileset = tileset;
			m_tilesize = m_tileset.TileSize;
			m_size = new Size(width, height);
			m_surface = new Surface(width * tileset.TileSize, height * tileset.TileSize);
			ReDraw();
		}
		public void MergeLayer(CMapLayer layer) {
			for (int y = 0; y < m_size.Height; y++) {
				for (int x = 0; x < m_size.Width; x++) {
					if (layer.m_layer[x, y] != 0)
						m_layer[x, y] = layer.m_layer[x, y];
				}
			}
		}
		public bool Collide(CActor actor) {
			int pointx, pointy;
			bool collide = false;

			//Top
			pointx = (actor.Center().X / m_tilesize);
			pointy = (actor.Top / m_tilesize);
			if (m_layer[pointx, pointy] != 0) {
				actor.ReverseMovement(ActorDirection.Up);
				collide = true;
			}

			//Bottom
			pointy = (actor.Bottom / m_tilesize);
			if (m_layer[pointx, pointy] != 0) {
				actor.ReverseMovement(ActorDirection.Down);
				collide = true;
			}

			//Left
			pointx = (actor.Left / m_tilesize);
			pointy = (actor.Center().Y / m_tilesize);
			if (m_layer[pointx, pointy] != 0) {
				actor.ReverseMovement(ActorDirection.Left);
				collide = true;
			}

			//Right
			pointx = (actor.Right / m_tilesize);
			if (m_layer[pointx, pointy] != 0) {
				actor.ReverseMovement(ActorDirection.Right);
				collide = true;
			}

			return collide;
		}
		public void Draw(Surface dest, Rectangle camera) {
			if (m_collideonly)
				throw new Exception("Tried to draw a collision layer");
			Rectangle destRect = new Rectangle(0, 0, camera.Width, camera.Height);
			dest.Blit(m_surface, destRect, camera);
		}
		public void ReDraw() {
			m_surface.Fill(Color.Purple);
			m_surface.Transparent = true;
			m_surface.TransparentColor = Color.Purple;

			Rectangle destRect, srcRect;
			for (int y = 0; y < m_size.Height; y++) {
				for (int x = 0; x < m_size.Width; x++) {
					if (m_layer[x, y] != 0) {
						srcRect = m_tileset.GetTile(m_layer[x, y]);
						destRect = new Rectangle(x * m_tilesize, y * m_tilesize, m_tilesize, m_tilesize);
						m_surface.Blit(m_tileset.Surface, destRect, srcRect);
					}
				}
			}
		}
	}
}
