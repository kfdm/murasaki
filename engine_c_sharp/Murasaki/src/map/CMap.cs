using System;
using System.Xml;
using System.Collections.Generic;
using SdlDotNet.Graphics;
using System.Drawing;
using Murasaki.Actors;
using Murasaki.Entitys;

namespace Murasaki.Map {
	public class CTileMap {
		#region Properties
		public CActorPlayer Avatar { get { return m_avatar; } }
		public LinkedList<CActor> Weapons { get { return m_avatar_weapons; } }
		public LinkedList<CActor> EnemyWeapons { get { return m_actor_weapons; } }
		public LinkedList<CActor> Actors { get { return m_actors; } }
		public LinkedList<CEntity> Entities { get { return m_entities; } }
		public int TileSize { get { return m_TileSize; } }
		public Random RandomGenerator { get { return m_rand; } }
		public Rectangle Camera { get { return m_camera; } }
		#endregion

		#region Private
		private CTileSet m_TileSet;
		private int m_MapHeight, m_MapWidth, m_TileSize;
		private CMapLayer m_MapBase, m_MapDetail, m_MapCollide, m_MapClip;
		private Random m_rand;

		private Rectangle m_camera;
		private const int m_camera_width = 25;
		private const int m_camera_height = 20;
		private const int m_camera_halfwidth = m_camera_width / 2;
		private const int m_camera_halfheight = m_camera_height / 2;

		private Surface m_surface;

		private CActorPlayer m_avatar;
		private LinkedList<CActor> m_actors, m_actor_weapons, m_avatar_weapons;
		private LinkedList<CEntity> m_entities;
		#endregion

		#region Constuctors/Destructors
		public CTileMap(string filename, int playerx, int playery)
			: this(filename) {
			movePlayer(playerx, playery);
		}
		public CTileMap(string filename) {
			m_actors = new LinkedList<CActor>();
			m_actor_weapons = new LinkedList<CActor>();
			m_avatar_weapons = new LinkedList<CActor>();
			m_entities = new LinkedList<CEntity>();

			m_avatar = new CActorPlayer(this);
			LoadMap("Data/" + filename);

			m_camera = new Rectangle(0, 0, m_camera_width * m_TileSize, m_camera_height * m_TileSize);
			m_surface = new Surface(m_camera);
			Video.SetVideoMode(m_camera_width * m_TileSize, m_camera_height * m_TileSize);

			m_rand = new Random(DateTime.Now.Millisecond);
		}
		~CTileMap() {
			m_surface.Dispose();
		}
		#endregion

		#region Loader
		private void LoadMap(string filename) {
			XmlDocument xml;
			XmlNodeList nodes;

			xml = new XmlDocument();
			xml.Load(filename);
			nodes = xml.SelectNodes("map");
			foreach (XmlNode map in nodes)
				LoadMapInfo(map);

			//Load Map Properties
			nodes = xml.SelectNodes("map/properties");
			foreach (XmlNode properties in nodes)
				LoadMapProperties(properties);

			//Load Tilesets
			nodes = xml.SelectNodes("map/tileset");
			foreach (XmlNode tileset in nodes)
				LoadMapTileset(tileset);

			//Load Map Layers
			nodes = xml.SelectNodes("map/layer");
			foreach (XmlNode layer in nodes)
				LoadMapLayer(layer);
			m_MapClip.MergeLayer(m_MapCollide.Layer, m_MapWidth, m_MapHeight);

			//Load Entities
			nodes = xml.SelectNodes("map/objectgroup");
			foreach (XmlNode objectgroup in nodes)
				LoadMapObjectgroup(objectgroup);
			return;
		}
		private void LoadMapLayer(XmlNode layer) {
			int[,] currentlayer;
			string layername = "";

			//Read Layer Attributes
			foreach (XmlAttribute attr in layer.Attributes) {
				switch (attr.Name) {
					case "name":
						layername = attr.Value;
						break;
					default:
						Console.WriteLine("Unknown attr {0}={1} in LoadMapLayer()", attr.Name, attr.Value);
						break;
				}
			}
			//Read in Layer
			currentlayer = new int[m_MapHeight, m_MapWidth];
			//Move to the first tile node
			layer = layer.FirstChild;
			String text = layer.InnerText.Trim();
			byte[] bytes = Convert.FromBase64String(text);
			int currbyte = 0;
			for (int y = 0; y < m_MapHeight; y++) {
				for (int x = 0; x < m_MapWidth; x++) {
					currentlayer[x, y] = bytes[currbyte];
					currbyte += 4;
				}
			}

			//Save layer
			switch (layername) {
				case "Base":
					m_MapBase = new CMapLayer(currentlayer, m_MapWidth, m_MapHeight, m_TileSet);
					break;
				case "Collide":
					m_MapCollide = new CMapLayer(currentlayer, m_MapWidth, m_MapHeight, m_TileSet);
					break;
				case "Clip":
					m_MapClip = new CMapLayer(currentlayer, m_MapWidth, m_MapHeight, m_TileSize);
					break;
				case "Detail":
					m_MapDetail = new CMapLayer(currentlayer, m_MapWidth, m_MapHeight, m_TileSet);
					break;
			}
		}
		private void LoadMapTileset(XmlNode tileset) {
			tileset = tileset.FirstChild;
			String filename = "Data/" + tileset.Attributes["source"].Value;
			String colorstr = tileset.Attributes["trans"].Value;
			Color trans = Color.FromArgb(255, 0, 255);
			m_TileSet = new CTileSet(filename, m_TileSize, trans);
		}
		private void LoadMapInfo(XmlNode map) {
			foreach (XmlAttribute attr in map.Attributes) {
				switch (attr.Name) {
					case "width":
						m_MapWidth = Convert.ToInt16(attr.Value);
						break;
					case "height":
						m_MapHeight = Convert.ToInt16(attr.Value);
						break;
					case "tilewidth":
						m_TileSize = Convert.ToInt16(attr.Value);
						break;
					default:
						Console.WriteLine("Unknown attr {0}={1} in LoadMapInfo()", attr.Name, attr.Value);
						break;
				}
			}
		}
		private void LoadMapProperties(XmlNode properties) {
			int startx = 0, starty = 0;

			foreach (XmlNode tmp in properties.SelectNodes("property")) {
				XmlAttributeCollection attr = tmp.Attributes;
				switch (attr["name"].Value) {
					case "startx":
						startx = Convert.ToInt16(attr["value"].Value);
						break;
					case "starty":
						starty = Convert.ToInt16(attr["value"].Value);
						break;
					default:
						Console.WriteLine("Unknown attr {0}={1} in LoadMapInfo()", attr["name"].Value, attr["value"].Value);
						break;
				}
			}

			movePlayer(startx, starty);
		}
		private void LoadMapObjectgroup(XmlNode objectgroup) {
			foreach (XmlNode entity in objectgroup.ChildNodes) {
				XmlAttributeCollection attr = entity.Attributes;
				XmlNodeList properties = entity.FirstChild.ChildNodes;
				switch (attr["type"].Value) {
					case "CEntityTransition":
						m_entities.AddLast(new CEntityTransition(this, attr, properties));
						break;
					case "CEntityMonsterSpawn":
						CEntityMonsterSpawn tmp = new CEntityMonsterSpawn(this, attr, properties);
						break;
					default:
						Console.WriteLine("Unknown Entity Type {0} in LoadMapObjectgroup()", attr["type"].Value);
						break;
				}
			}
		}
		#endregion

		#region UpdateLoop
		public void Update() {
			List<CActor> toRemoveWeapons = new List<CActor>();
			List<CActor> toRemoveActors = new List<CActor>();

			//Update Avatar
			m_avatar.Update();
			m_MapClip.Collide(m_avatar, toRemoveActors);

			//Update Avatar Weapons
			foreach (CActor weapon in m_avatar_weapons) {
				weapon.Update();
				m_MapCollide.Collide(weapon, toRemoveWeapons);
				foreach (CActor actor in m_actors) {
					if (weapon.IntersectsWith(actor)) {
						actor.GotHit(weapon, toRemoveActors, toRemoveWeapons);
					}
				}
			}
			//Remove Dead Weapons and Actors
			foreach (CActor actor in toRemoveWeapons)
				m_avatar_weapons.Remove(actor);
			foreach (CActor actor in toRemoveActors)
				m_actors.Remove(actor);


			//Update Actors
			foreach (CActor actor in m_actors) {
				actor.Update();
				m_MapClip.Collide(actor, toRemoveActors);
			}
			//Update Actor Weapons
			foreach (CActor weapon in m_actor_weapons) {
				weapon.Update();
				m_MapCollide.Collide(weapon, toRemoveWeapons);
				if (weapon.IntersectsWith(m_avatar)) {
					m_avatar.GotHit(weapon, toRemoveActors, toRemoveWeapons);
				}
			}
			foreach (CActor weapon in toRemoveWeapons)
				m_actor_weapons.Remove(weapon);

			//Update Entities
			foreach (CEntity entity in m_entities) {
				if (entity.IntersectsWith(m_avatar))
					entity.CollidePlayer();
			}
			foreach (CActor actor in toRemoveActors)
				m_actors.Remove(actor);
		}
		#endregion

		#region DrawLoop
		private void DrawActors() {
			int CamLeft = m_camera.X / m_TileSize;
			int CamTop = m_camera.Y / m_TileSize;
			int left, top;
			//Draw Avatar
			m_avatar.Draw(m_surface, m_camera);

			//Draw Avatar Weapons
			foreach (CActor actor in m_avatar_weapons) {
				left = actor.Left / m_TileSize;
				top = actor.Top / m_TileSize;
				if (left >= CamLeft && left < CamLeft + m_camera_width &&
					top >= CamTop && top < CamTop + m_camera_width) {
					actor.Draw(m_surface, m_camera);
				}
			}

			//Draw Actors
			foreach (CActor actor in m_actors) {
				left = actor.Left / m_TileSize;
				top = actor.Top / m_TileSize;
				if (left >= CamLeft && left < CamLeft + m_camera_width &&
					top >= CamTop && top < CamTop + m_camera_width) {
					actor.Draw(m_surface, m_camera);
				}
			}

			//Draw Actors Weapons
			foreach (CActor actor in m_actor_weapons) {
				left = actor.Left / m_TileSize;
				top = actor.Top / m_TileSize;
				if (left >= CamLeft && left < CamLeft + m_camera_width &&
					top >= CamTop && top < CamTop + m_camera_width) {
					actor.Draw(m_surface, m_camera);
				}
			}
		}
		public void Draw(Surface dest, Rectangle size) {
			m_camera.X = m_avatar.Left + (m_avatar.Width / 2) - (m_camera_halfwidth * m_TileSize);
			m_camera.Y = m_avatar.Top + (m_avatar.Height / 2) - (m_camera_halfheight * m_TileSize);

			m_surface.Fill(Color.Black);

			m_MapBase.Draw(m_surface, m_camera);
			DrawActors();
			m_MapCollide.Draw(m_surface, m_camera);
			m_MapDetail.Draw(m_surface, m_camera);

			dest.Blit(m_surface, size);
		}
		#endregion

		#region Misc
		private void movePlayer(int x, int y) {
			if (x > 0 && x < m_MapWidth && y > 0 && y < m_MapHeight) {
				m_avatar.Left = x * m_TileSize;
				m_avatar.Top = y * m_TileSize;
			} else {
				Console.WriteLine("Error: can't move player to %d,%d", x, y);
			}
		}
		#endregion
	}
}
