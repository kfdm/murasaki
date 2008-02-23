using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using SdlDotNet.Core;
using SdlDotNet.Input;
using SdlDotNet.Graphics;
using System.Drawing;

namespace Murasaki {
    public class CTileMap {
        private const int TILE_SIZE = 48;

        private int m_MapHeight, m_MapWidth;
        private int m_PlayerX, m_PlayerY;
        private CTileSet m_TileSet;
        private int[,] m_MapBase,m_MapTrans,m_MapDetail,m_MapColide;
        private Surface m_surface;

        public CTileMap(string filename) : this(filename, 0, 0) { }
        public CTileMap(string filename, int playerx, int playery): base() {
            Console.WriteLine("CMap {0} {1} {2}", filename, playerx, playery);
            m_surface = new Surface(Video.Screen.Rectangle);
            m_PlayerX = playerx;
            m_PlayerY = playery;
            StreamReader srReader;
            try {
                srReader = File.OpenText(filename);
                int mapversion = Convert.ToInt32(srReader.ReadLine());
                m_MapHeight = Convert.ToInt32(srReader.ReadLine());
                m_MapWidth = Convert.ToInt32(srReader.ReadLine());
                m_TileSet = new CTileSet(srReader.ReadLine());

                m_MapBase = new int[m_MapHeight, m_MapWidth];
                m_MapTrans = new int[m_MapHeight, m_MapWidth];
                m_MapDetail = new int[m_MapHeight, m_MapWidth];
                m_MapColide = new int[m_MapHeight, m_MapWidth];
                for (int y = 0; y < m_MapHeight; y++) {
                    for (int x = 0; x < m_MapWidth; x++) {
                        m_MapBase[y, x] = Convert.ToInt32(srReader.ReadLine());
                        m_MapTrans[y, x] = Convert.ToInt32(srReader.ReadLine());
                        m_MapDetail[y, x] = Convert.ToInt32(srReader.ReadLine());
                        m_MapColide[y, x] = Convert.ToInt32(srReader.ReadLine());
                    }
                }
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
            }
        }
        public void Update() {

        }
        public void Draw(Surface dest) {
            Rectangle destRect;
            Rectangle srcRect;
            for (int y = 0; y < m_MapHeight; y++) {
                for (int x = 0; x < m_MapWidth; x++) {
                    srcRect = m_TileSet.GetTile(m_MapBase[y, x]);
                    destRect = new Rectangle(x * TILE_SIZE, y * TILE_SIZE, TILE_SIZE, TILE_SIZE);
                    m_surface.Blit(m_TileSet.tilemap.Surface , destRect, srcRect);
                }
            }
            dest.Blit(m_surface);
        }
        public void movePlayer(int x, int  y) {
            if (x > 0 && x < m_MapWidth && y > 0 && y < m_MapHeight) {
                m_PlayerX = x;
                m_PlayerY = y;
            } else {
                Console.WriteLine("Error: can't move player to %d,%d", x, y);
            }
        }
        public void OnKeyboardDown(KeyboardEventArgs e) {

        }
        public void OnKeyboardUp(KeyboardEventArgs e) {

        }
    }
}
