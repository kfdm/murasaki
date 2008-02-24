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
        private int m_PlayerX, m_PlayerY;
        private CTileSet m_TileSet;
        private int m_TileSize;
        private int[,] m_MapBase,m_MapDetail,m_MapColide;
        private Surface m_surface;
        private Sprite m_avatar;

        public CTileMap(string filename){
            m_surface = new Surface(Video.Screen.Rectangle);
            LoadAvatar("Data/avatar.png");
            LoadMap("Data/test.tmx");
            movePlayer(m_MapHeight / 2, m_MapWidth / 2);
        }
        public CTileMap(string filename, int playerx, int playery) {
            m_surface = new Surface(Video.Screen.Rectangle);
            LoadAvatar("Data/avatar.png");
            LoadMap("Data/test.tmx");
            movePlayer(playerx, playery);
        }
        private void LoadAvatar(string filename) {
            m_avatar = new Sprite(filename);
            m_avatar.TransparentColor = System.Drawing.Color.Magenta;
            m_avatar.Transparent = true;
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
                    map[y, x] = tile;
                    Console.Write(" {0} ",tile);
                }
                Console.WriteLine();
            }
            xml.Read();  //Read in Data Tag
            xml.MoveToContent();
        }
        public void Update() {

        }
        public void DrawLayer(int[,] layer) {
            Rectangle destRect;
            Rectangle srcRect;
            for (int y = 0; y < m_MapHeight; y++) {
                for (int x = 0; x < m_MapWidth; x++) {
                    if (layer[y, x] != 0) {
                        srcRect = m_TileSet.GetTile(layer[y, x]);
                        destRect = new Rectangle(x * m_TileSize, y * m_TileSize, m_TileSize, m_TileSize);
                        m_surface.Blit(m_TileSet.m_tilemap.Surface, destRect, srcRect);
                    }
                }
            }
        }
        public void DrawAvatar() {
            Rectangle destRect = new Rectangle(m_PlayerX, m_PlayerY, m_TileSize, m_TileSize);
            m_surface.Blit(m_avatar.Surface, destRect);
        }
        public void Draw(Surface dest) {
            DrawLayer(m_MapBase);
            DrawLayer(m_MapColide);
            DrawAvatar();
            DrawLayer(m_MapDetail);
            dest.Blit(m_surface);
        }
        public void movePlayer(int x, int  y) {
            if (x > 0 && x < m_MapWidth && y > 0 && y < m_MapHeight) {
                m_PlayerX = x * m_TileSize;
                m_PlayerY = y * m_TileSize;
            } else {
                Console.WriteLine("Error: can't move player to %d,%d", x, y);
            }
        }
        public void OnKeyboardDown(KeyboardEventArgs e) {
            Console.WriteLine(e.Key.ToString());
            switch (e.Key) {
                case Key.DownArrow:
                    m_PlayerY += 16;
                    break;
                case Key.UpArrow:
                    m_PlayerY -= 16;
                    break;
                case Key.LeftArrow:
                    m_PlayerX -= 16;
                    break;
                case Key.RightArrow:
                    m_PlayerX += 16;
                    break;
            }
        }
        public void OnKeyboardUp(KeyboardEventArgs e) {

        }
    }
}
