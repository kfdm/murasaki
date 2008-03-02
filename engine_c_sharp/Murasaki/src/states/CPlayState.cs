﻿using System;
using SdlDotNet.Core;
using SdlDotNet.Graphics;
using SdlDotNet.Input;
using System.Drawing;

using Murasaki.Actors;
using Murasaki.Map;

namespace Murasaki.State {
    class CPlayState : CGameState {
        const string START_MAP = "test.tmx";
        private static CPlayState m_PlayState;
        private static CTileMap m_CurrentMap;
        private Random m_rand;
        public override void OnKeyboardDown(CGameEngine game, KeyboardEventArgs e) {
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
                case Key.F5:
                    Video.Screen.SaveBmp("screenshot.bmp");
                    break;
                case Key.Backspace:
                    for (int i = 0; i < 10; i++)
                        m_CurrentMap.Actors.AddLast(new CActorCivilian("data/pink.png", 26, 13, 24, m_rand));
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
                    m_CurrentMap.Weapons.AddLast(tmp);
                    break;
            }
        }
        public override void OnKeyboardUp(CGameEngine game, KeyboardEventArgs e) {
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
            m_rand = new Random(DateTime.Now.Millisecond);
        }
        public void ChangeMap(CTileMap map) {
            m_CurrentMap = map;
            GC.Collect();
            GC.WaitForPendingFinalizers();
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
