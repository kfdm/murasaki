using System;
using System.Xml;
using System.Collections.Generic;
using System.Text;
using System.IO;
using SdlDotNet.Core;
using SdlDotNet.Input;
using SdlDotNet.Graphics;
using SdlDotNet.Graphics.Sprites;
using System.Drawing;

namespace Murasaki {
    public class CTileMap {
        public CActorPlayer Avatar { get { return m_avatar; } }
        public LinkedList<CActor> Weapons { get { return m_avatar_weapons; } }
        public LinkedList<CActor> Actors { get { return m_actors; } }
        public Dictionary<String,CActor> Entities { get { return m_entities; } } 

        private CTileSet m_TileSet;
        private int m_MapHeight, m_MapWidth;
        private int m_TileSize;
        private int[,] m_MapBase,m_MapDetail,m_MapColide;

        private Rectangle m_camera;
        private const int m_camera_width = 25;
        private const int m_camera_height = 20;
        private const int m_camera_halfwidth = m_camera_width / 2;
        private const int m_camera_halfheight = m_camera_height / 2;

        private Surface m_surface;

        private CActorPlayer m_avatar;
        private LinkedList<CActor> m_actors, m_actor_weapons, m_avatar_weapons;
        private Dictionary<String,CActor> m_entities;

        public CTileMap(string filename,int playerx, int playery) : this(filename) {
            movePlayer(playerx, playery);
        }
        public CTileMap(string filename) {
            LoadAvatar("Data/avatar.png");
            LoadMap("Data/test.tmx");

            m_actors=new LinkedList<CActor>();
            m_actor_weapons = new LinkedList<CActor>();
            m_avatar_weapons = new LinkedList<CActor>();
            m_actors.AddLast( new CActorCivilian("data/pink.png",26,13,m_TileSize));

            m_entities = new Dictionary<String,CActor>();

            m_surface = new Surface(m_camera_width * m_TileSize, m_camera_height * m_TileSize);
            m_camera = new Rectangle(0, 0, m_camera_width * m_TileSize, m_camera_height * m_TileSize);
            Video.SetVideoMode(m_camera_width * m_TileSize, m_camera_height * m_TileSize);
        }
        ~CTileMap() {
            m_surface.Dispose();
        }
        private void LoadAvatar(string filename) {
            m_avatar = new CActorPlayer(filename);
        }
        private void LoadMap(string filename) {
            XmlDocument xml;
            XmlNodeList nodes;

            xml = new XmlDocument();
            xml.Load(filename);
            nodes = xml.SelectNodes("map");
            foreach (XmlNode map in nodes)
                LoadMapInfo(map);
            nodes = xml.SelectNodes("map/properties");
            foreach (XmlNode properties in nodes)
                LoadMapProperties(properties);
            nodes = xml.SelectNodes("map/layer");
            foreach (XmlNode layer in nodes)
                LoadMapLayer(layer);
            nodes = xml.SelectNodes("map/tileset");
            foreach (XmlNode tileset in nodes)
                LoadMapTileset(tileset);
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
                    currbyte+=4;
                }
            }

            //Save layer
            switch (layername) {
                case "Base":
                    m_MapBase = currentlayer;
                    break;
                case "Colide":
                    m_MapColide = currentlayer;
                    break;
                case "Detail":
                    m_MapDetail = currentlayer;
                    break;
            }
        }
        private void LoadMapTileset(XmlNode tileset) {
            m_TileSet = new CTileSet("Data/sewer_tileset.png");
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
                switch (attr["type"].Value) {
                    default:
                        Console.WriteLine("Unknown Entity Type {0} in LoadMapObjectgroup()", attr["type"].Value);
                        break;
                }
            }
        }
        public void Update() {
            List<CActor> toRemoveWeapons = new List<CActor>();
            List<CActor> toRemoveActors = new List<CActor>();

            //Update Avatar
            m_avatar.Update();
            UpdateActorColide(m_avatar, toRemoveWeapons);

            //Update Avatar Weapons
            foreach (CActor weapon in m_avatar_weapons) {
                weapon.Update();
                UpdateActorColide(weapon, toRemoveWeapons);
                foreach(CActor actor in m_actors) {
                    if (weapon.Rectangle.IntersectsWith(actor.Rectangle)) {
                        Console.WriteLine("Hit Actor");
                        toRemoveWeapons.Add(weapon);
                        toRemoveActors.Add(actor);
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
                UpdateActorColide(actor,toRemoveWeapons);
            }


            
        }
        private void UpdateActorColide(CActor actor,List<CActor> toRemove) {
            int top, bottom, left, right;
            bool collide = false;
            top = actor.Top / m_TileSize;
            bottom = actor.Bottom / m_TileSize;
            left = actor.Left / m_TileSize;
            right = actor.Right / m_TileSize;

            if (actor.moveup || actor.movedown || actor.moveleft || actor.moveright) {
                //Top
                if (m_MapColide[left, top] != 0 && m_MapColide[right, top] != 0) {
                    actor.Top = (top + 1) * m_TileSize;
                    top = actor.Top / m_TileSize;
                    bottom = actor.Bottom / m_TileSize;
                    collide = true;
                }
                //Bottom
                if (m_MapColide[left, bottom] != 0 && m_MapColide[right, bottom] != 0) {
                    actor.Bottom = (bottom * m_TileSize) - 1;
                    top = actor.Top / m_TileSize;
                    bottom = actor.Bottom / m_TileSize;
                    collide = true;
                }
                //Left
                if (m_MapColide[left, top] != 0 && m_MapColide[left, bottom] != 0) {
                    actor.Left = (left + 1) * m_TileSize;
                    left = actor.Left / m_TileSize;
                    right = actor.Right / m_TileSize;
                    collide = true;
                }
                //Right
                if (m_MapColide[right, top] != 0 && m_MapColide[right, bottom] != 0) {
                    actor.Right = (right * m_TileSize) - 1;
                    left = actor.Left / m_TileSize;
                    right = actor.Right / m_TileSize;
                    collide = true;
                }
            }
            if (collide) {
                if (actor is CActorBullet)
                    toRemove.Add(actor);
                actor.CollideWall();
            }
        }
        private void DrawLayer(int[,] layer,int CameraLeft,int CameraTop) {
            Surface tmp = new Surface(m_TileSize * (m_camera_width + 2), m_TileSize * (m_camera_width + 2));
            tmp.Transparent = true;
            Rectangle destRect,srcRect;
            for (int y = 0; y < m_camera_width+2; y++) {
                for (int x = 0; x < m_camera_width+2; x++) {
                    if (layer[x+CameraLeft,y+CameraTop] != 0) {
                        srcRect = m_TileSet.GetTile(layer[x + CameraLeft, y + CameraTop]);
                        destRect = new Rectangle(x * m_TileSize, y * m_TileSize, m_TileSize, m_TileSize);
                        tmp.Blit(m_TileSet.m_tilemap.Surface, destRect, srcRect);
                    }
                }
            }
            m_surface.Blit(tmp, m_surface.Rectangle, m_camera);
            tmp.Dispose();
        }
        private void DrawActors(int CamLeft, int CamTop) {
            int left, top;
            Rectangle m_world = new Rectangle(CamLeft * m_TileSize, CamTop * m_TileSize, m_camera_width * m_TileSize, m_camera_height * m_TileSize);

            //Draw Avatar
            m_avatar.Draw(m_surface, m_world, m_camera);

            //Draw Avatar Weapons
            foreach (CActor actor in m_avatar_weapons) {
                left = actor.Left / m_TileSize;
                top = actor.Top / m_TileSize;
                if (left >= CamLeft && left < CamLeft + m_camera_width &&
                    top >= CamTop && top < CamTop + m_camera_width) {
                    actor.Draw(m_surface, m_world, m_camera);
                }
            }

            //Draw Actors
            foreach (CActor actor in m_actors) {
                left = actor.Left / m_TileSize;
                top = actor.Top / m_TileSize;
                if (left >= CamLeft && left < CamLeft + m_camera_width &&
                    top >= CamTop && top < CamTop + m_camera_width) {
                    actor.Draw(m_surface,m_world, m_camera);
                }
            }

            //Draw Actors Weapons
            foreach (CActor actor in m_actor_weapons) {
                left = actor.Left / m_TileSize;
                top = actor.Top / m_TileSize;
                if (left >= CamLeft && left < CamLeft + m_camera_width &&
                    top >= CamTop && top < CamTop + m_camera_width) {
                    actor.Draw(m_surface, m_world, m_camera);
                }
            }
        }
        public void Draw(Surface dest,Rectangle size) {
            int camx, camy,playx, playy;

            playx = (m_avatar.Left / m_TileSize);
            playy = (m_avatar.Top / m_TileSize);
            camy = playy - m_camera_halfwidth;
            camx = playx - m_camera_halfwidth;

            if (camx < 0)
                camx = 0;
            if (camy < 0)
                camy = 0;

            m_camera.X = (m_avatar.Left + (m_avatar.Width / 2) - (m_camera_halfwidth * m_TileSize)) - (camx * m_TileSize);
            m_camera.Y = (m_avatar.Top + (m_avatar.Height / 2) - (m_camera_halfheight * m_TileSize)) - (camy * m_TileSize);

            m_surface.Fill(Color.Black);
            DrawLayer(m_MapBase, camx, camy);
            DrawLayer(m_MapColide, camx, camy);
            DrawActors(camx,camy);
            DrawLayer(m_MapDetail, camx, camy);
            dest.Blit(m_surface, size);
        }
        private void movePlayer(int x, int  y) {
            if (x > 0 && x < m_MapWidth && y > 0 && y < m_MapHeight) {
                m_avatar.Left = x * m_TileSize;
                m_avatar.Top = y * m_TileSize;
            } else {
                Console.WriteLine("Error: can't move player to %d,%d", x, y);
            }
        }
    }
}
