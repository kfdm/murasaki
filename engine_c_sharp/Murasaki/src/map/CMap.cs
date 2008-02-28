﻿using System;
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
        private int m_MapHeight, m_MapWidth;
        private CTileSet m_TileSet;
        private int m_TileSize;
        private int[,] m_MapBase,m_MapDetail,m_MapColide;

        private Rectangle m_camera;
        private int m_camera_width,m_camera_halfwidth;

        private Surface m_surface;
        private CActorPlayer m_avatar;

        private LinkedList<CActor> m_actors;

        public CTileMap(string filename,int playerx, int playery) : this(filename) {
            movePlayer(playerx, playery);
        }
        public CTileMap(string filename) {
            m_surface = new Surface(20 * 24, 20 * 24);

            LoadAvatar("Data/avatar.png");
            LoadMap("Data/test.tmx");

            m_actors=new LinkedList<CActor>();
            m_actors.AddLast( new CActorCivilian("data/pink.png",26,13,m_TileSize));

            m_camera_width = 20;
            m_camera_halfwidth = m_camera_width / 2;
            m_camera = new Rectangle(0, 0, m_camera_width * m_TileSize, m_camera_width * m_TileSize);
        }
        ~CTileMap() {
            m_surface.Dispose();
        }
        private void LoadAvatar(string filename) {
            m_avatar = new CActorPlayer(filename);
        }
        private void LoadMap(string filename) {
            System.Xml.XmlTextReader reader = new System.Xml.XmlTextReader(filename);
            while (reader.Read()) {
                reader.MoveToContent();
                if (reader.IsStartElement()) {
                    switch (reader.Name) {
                        case "map":
                            Console.WriteLine("Found Map!");
                            m_TileSize = Convert.ToInt16(reader.GetAttribute("tilewidth"));
                            m_MapWidth = Convert.ToInt16(reader.GetAttribute("width"));
                            m_MapHeight = Convert.ToInt16(reader.GetAttribute("height"));
                            
                            m_MapBase = new int[m_MapHeight, m_MapWidth];
                            m_MapColide = new int[m_MapHeight, m_MapWidth];
                            m_MapDetail = new int[m_MapHeight, m_MapWidth];
                            break;
                        case "properties":
                            Console.WriteLine("Found Properties");
                            LoadMapProperties(reader);
                            break;
                        case "tileset":
                            Console.WriteLine("Found Tileset");
                            Console.WriteLine(reader.GetAttribute("name"));
                            LoadMapTileset(reader);
                            break;
                        case "layer":
                            String name = reader.GetAttribute("name");
                            Console.WriteLine("Found Layer:"+name);
                            int width = Convert.ToInt32(reader.GetAttribute("width"));
                            int height = Convert.ToInt32(reader.GetAttribute("height"));
                            switch (name) {
                                case "Base":
                                    LoadMapLayer(reader, width, height, m_MapBase);
                                    break;
                                case "Colide":
                                    LoadMapLayer(reader, width, height, m_MapColide);

                                    break;
                                case "Detail":
                                    LoadMapLayer(reader, width, height, m_MapDetail);
                                    break;
                            }
                            break;
                    }
                }
            }
        }
        private void LoadMapProperties(XmlReader xml) {
            int startx = 15, starty = 15;
            while (xml.Read()) {
                xml.MoveToContent();
                //Exit if we find the </properties> tag
                if (xml.Name == "properties") {
                    movePlayer(startx, starty);
                    return;
                }
                switch (xml.GetAttribute("name")) {
                    case "startx":
                        startx = Convert.ToInt32(xml.GetAttribute("value"));
                        break;
                    case "starty":
                        starty = Convert.ToInt32(xml.GetAttribute("value"));
                        break;
                }
            }
            
        }
        private void LoadMapTileset(XmlReader xml) {
            m_TileSet = new CTileSet("Data/sewer_tileset.png");
        }
        private void LoadMapLayer(XmlReader xml, int width, int height, int[,] map) {
            Console.WriteLine("{0} {1}", width, height);
            xml.Read();  //Read in Data Tag
            xml.MoveToContent();
            for (int y = 0; y < m_MapHeight; y++) {
                for (int x = 0; x < m_MapWidth; x++) {
                    xml.Read();
                    xml.MoveToContent();
                    int tile = Convert.ToInt16(xml.GetAttribute("gid"));
                    map[x,y] = tile;
                    Console.Write(" {0} ",tile);
                }
                Console.WriteLine();
            }
            xml.Read();  //Read in Data Tag
            xml.MoveToContent();
        }
        public void Update() {
            m_avatar.Update();

            foreach (CActor actor in m_actors) {
                actor.Update();
                UpdateActorColide(actor);
            }

            UpdateActorColide(m_avatar);
        }
        private void UpdateActorColide(CActor actor) {
            int top, bottom, left, right;
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
                    actor.CollideWall();
                }
                //Bottom
                if (m_MapColide[left, bottom] != 0 && m_MapColide[right, bottom] != 0) {
                    actor.Bottom = (bottom * m_TileSize) - 1;
                    top = actor.Top / m_TileSize;
                    bottom = actor.Bottom / m_TileSize;
                    actor.CollideWall();
                }
                //Left
                if (m_MapColide[left, top] != 0 && m_MapColide[left, bottom] != 0) {
                    actor.Left = (left + 1) * m_TileSize;
                    left = actor.Left / m_TileSize;
                    right = actor.Right / m_TileSize;
                    actor.CollideWall();
                }
                //Right
                if (m_MapColide[right, top] != 0 && m_MapColide[right, bottom] != 0) {
                    actor.Right = (right * m_TileSize) - 1;
                    left = actor.Left / m_TileSize;
                    right = actor.Right / m_TileSize;
                    actor.CollideWall();
                }
            }            
        }
        public void DrawLayer(int[,] layer,int CameraLeft,int CameraTop) {
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
        public void DrawActors(int CamLeft, int CamTop) {
            Rectangle m_world = new Rectangle(CamLeft * m_TileSize, CamTop * m_TileSize, m_camera_width * m_TileSize, m_camera_width * m_TileSize);
            m_avatar.Draw(m_surface, m_world, m_camera);
            foreach (CActor actor in m_actors) {
                int left = actor.Left / m_TileSize;
                int top = actor.Top / m_TileSize;
                if (left >= CamLeft && left < CamLeft + m_camera_width &&
                    top >= CamTop && top < CamTop + m_camera_width) {
                    actor.Draw(m_surface,m_world, m_camera);
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
            m_camera.Y = (m_avatar.Top + (m_avatar.Height / 2) - (m_camera_halfwidth * m_TileSize)) - (camy * m_TileSize);

            m_surface.Fill(Color.Black);
            DrawLayer(m_MapBase, camx, camy);
            DrawLayer(m_MapColide, camx, camy);
            DrawActors(camx,camy);
            DrawLayer(m_MapDetail, camx, camy);
            dest.Blit(m_surface, size);
        }
        public void movePlayer(int x, int  y) {
            if (x > 0 && x < m_MapWidth && y > 0 && y < m_MapHeight) {
                m_avatar.Left = x * m_TileSize;
                m_avatar.Top = y * m_TileSize;
            } else {
                Console.WriteLine("Error: can't move player to %d,%d", x, y);
            }
        }
        public void OnKeyboardDown(KeyboardEventArgs e) {
            Console.WriteLine(e.Key.ToString());
            switch (e.Key) {
                case Key.DownArrow:
                    m_avatar.movedown = true;
                    break;
                case Key.UpArrow:
                    m_avatar.moveup = true;
                    break;
                case Key.LeftArrow:
                    m_avatar.moveleft = true;
                    break;
                case Key.RightArrow:
                    m_avatar.moveright = true;
                    break;
            }
        }
        public void OnKeyboardUp(KeyboardEventArgs e) {
            Console.WriteLine(e.Key.ToString());
            switch (e.Key) {
                case Key.DownArrow:
                    m_avatar.movedown = false;
                    break;
                case Key.UpArrow:
                    m_avatar.moveup = false;
                    break;
                case Key.LeftArrow:
                    m_avatar.moveleft = false;
                    break;
                case Key.RightArrow:
                    m_avatar.moveright = false;
                    break;
            }
        }
    }
}
