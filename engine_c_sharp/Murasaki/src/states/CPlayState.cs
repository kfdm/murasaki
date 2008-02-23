using System;
using SdlDotNet.Core;
using SdlDotNet.Graphics;
using SdlDotNet.Input;

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
                case Key.DownArrow:
                case Key.LeftArrow:
                case Key.RightArrow:
                    m_CurrentMap.OnKeyboardDown(e);
                    break;
            }
        }
        public override void OnKeyboardUp(CGameEngine game, KeyboardEventArgs e) {
            Console.WriteLine("CPlayState OnKeyboardUp");
            switch (e.Key) {
                case Key.UpArrow:
                case Key.DownArrow:
                case Key.LeftArrow:
                case Key.RightArrow:
                    m_CurrentMap.OnKeyboardUp(e);
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
            m_CurrentMap.Draw(Video.Screen);
        }
    }
}
