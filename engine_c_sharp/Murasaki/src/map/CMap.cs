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
        private int m_MapHeight, m_MapWidth;
        private CTileSet m_TileSet;
        private int m_TileSize;
        private int[,] m_MapBase,m_MapDetail,m_MapColide;

        private Rectangle m_camera;
        private int m_camera_width,m_camera_halfwidth;

        private Surface m_surface;
        private CActorPlayer m_avatar;

        public CTileMap(string filename) : this(filename, 10, 10) { }
        public CTileMap(string filename, int playerx, int playery) {
            m_surface = new Surface(20 * 24, 20 * 24);

            LoadAvatar("Data/avatar.png");
            LoadMap("Data/test.tmx");
            movePlayer(playerx, playery);

            m_camera_width = 20;
            m_camera_halfwidth = m_camera_width / 2;
            m_camera = new Rectangle(0, 0, m_camera_width * m_TileSize, m_camera_width * m_TileSize);
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

            UpdatePlayerColide();
        }
        private void UpdatePlayerColide() {
            int top, bottom, left, right;
            top = m_avatar.Top / m_TileSize;
            bottom = m_avatar.Bottom / m_TileSize;
            left = m_avatar.Left / m_TileSize;
            right = m_avatar.Right / m_TileSize;

            if (m_avatar.moveup) {
                if (m_MapColide[left, top] != 0 || m_MapColide[right, top] != 0) {
                    m_avatar.Top = (top + 1) * m_TileSize;
                    top = m_avatar.Top / m_TileSize;
                    bottom = m_avatar.Bottom / m_TileSize;
                }
            }
            if (m_avatar.movedown) {
                 if (m_MapColide[left, bottom] != 0 || m_MapColide[right, bottom] != 0) {
                    m_avatar.Bottom = (bottom * m_TileSize) - 1;
                    top = m_avatar.Top / m_TileSize;
                    bottom = m_avatar.Bottom / m_TileSize;
                }
            }

            if (m_avatar.moveleft) {
                if (m_MapColide[left, top] != 0 || m_MapColide[left, bottom] != 0) {
                    m_avatar.Left = (left + 1) * m_TileSize;
                    left = m_avatar.Left / m_TileSize;
                    right = m_avatar.Right / m_TileSize;
                }
            }
            if (m_avatar.moveright) {
                if (m_MapColide[right, top] != 0 || m_MapColide[right, bottom] != 0) {
                    m_avatar.Right = (right * m_TileSize) - 1;
                    left = m_avatar.Left / m_TileSize;
                    right = m_avatar.Right / m_TileSize;
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
        public void DrawActors() {
            Rectangle destRect = new Rectangle();
            destRect.Height = m_avatar.Height;
            destRect.Width = m_avatar.Width;
            destRect.X = (m_camera_halfwidth * m_TileSize) - m_avatar.Width / 2;
            destRect.Y = (m_camera_halfwidth * m_TileSize) - m_avatar.Height / 2;
            m_avatar.Draw(m_surface, destRect);
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

            Console.WriteLine("{0},{1} {2},{3}", m_camera.X, m_camera.Y, Math.Abs( m_camera.X - m_camera.Width), Math.Abs(m_camera.Y - m_camera.Height));

            DrawLayer(m_MapBase, camx, camy);
            DrawLayer(m_MapColide, camx, camy);
            DrawActors();
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
