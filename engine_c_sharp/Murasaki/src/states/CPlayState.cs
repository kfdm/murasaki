using System;
using SdlDotNet.Core;
using SdlDotNet.Graphics;
using SdlDotNet.Input;
using System.Drawing;

namespace Murasaki {
    class CPlayState : CGameState {
        const string START_MAP = "Data/Start.map";
        private static CPlayState m_PlayState;
        private static CTileMap m_CurrentMap;
        public override void OnKeyboardDown(CGameEngine game, KeyboardEventArgs e) {
            Console.WriteLine("CPlayState OnKeyboardDown");
            switch (e.Key) {
                case Key.Escape:
                case Key.Q:
                    Events.QuitApplication();
                    break;
                case Key.UpArrow:
                    m_CurrentMap.Avatar.moveup = true;
                    break;
                case Key.DownArrow:
                    m_CurrentMap.Avatar.movedown = true;
                    break;
                case Key.LeftArrow:
                    m_CurrentMap.Avatar.moveleft = true;
                    break;
                case Key.RightArrow:
                    m_CurrentMap.Avatar.moveright = true;
                    break;
                case Key.Space:
                    CActorBullet tmp = new CActorBullet("Data/bullet.png");
                    tmp.Top = m_CurrentMap.Avatar.Top;
                    tmp.Left = m_CurrentMap.Avatar.Left;
                    switch (m_CurrentMap.Avatar.direction) {
                        case Key.UpArrow:
                            tmp.moveup = true;
                            break;
                        case Key.DownArrow:
                            tmp.movedown = true;
                            break;
                        case Key.LeftArrow:
                            tmp.moveleft = true;
                            break;
                        case Key.RightArrow:
                            tmp.moveright = true;
                            break;
                    }
                    m_CurrentMap.Actors.AddLast(tmp);
                    break;
            }
        }
        public override void OnKeyboardUp(CGameEngine game, KeyboardEventArgs e) {
            Console.WriteLine("CPlayState OnKeyboardUp");
            switch (e.Key) {
                case Key.UpArrow:
                    m_CurrentMap.Avatar.moveup = false;
                    break;
                case Key.DownArrow:
                    m_CurrentMap.Avatar.movedown = false;
                    break;
                case Key.LeftArrow:
                    m_CurrentMap.Avatar.moveleft = false;
                    break;
                case Key.RightArrow:
                    m_CurrentMap.Avatar.moveright = false;
                    break;
            }
        }
        public static CPlayState Instance() {
            if (m_PlayState == null)
                m_PlayState = new CPlayState();
            return m_PlayState;
        }
        public override void Init() {
            m_CurrentMap = new CTileMap(START_MAP);
        }
        public override void Draw(CGameEngine game) {
            Video.Screen.Fill(Color.White);
            m_CurrentMap.Draw(Video.Screen,Video.Screen.Rectangle);
        }
        public override void Update(CGameEngine game) {
            m_CurrentMap.Update();
        }
    }
}
