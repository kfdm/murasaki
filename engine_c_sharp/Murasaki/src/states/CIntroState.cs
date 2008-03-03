using SdlDotNet.Core;
using SdlDotNet.Graphics;
using SdlDotNet.Input;

namespace Murasaki.State {
    class CIntroState : CGameState {
        private static CIntroState m_IntroState;
        private Surface bg;
        private Surface fader;
        public override void  Init() {
            Surface temp = new Surface("Data/intro.bmp");
            bg = temp.CreateResizedSurface(Video.Screen.Size);
            fader = new Surface(bg.Width, bg.Height,
                                bg.BitsPerPixel, bg.RedMask,
                                bg.GreenMask, bg.BlueMask,
                                bg.AlphaMask);
            fader.Alpha = 255;
            fader.Fill(System.Drawing.Color.Black);
            fader.AlphaBlending = true;
        }
        public override void OnKeyboardDown(CGameEngine game, KeyboardEventArgs e) {
            switch (e.Key) {
                case Key.Escape:
                case Key.Q:
                    Events.QuitApplication();
                    break;
                case Key.Space:
                    game.ChangeState(CPlayState.Instance());
                    break;
            }
        }
        public override void Update(CGameEngine game) {
            if (fader.Alpha > 0) {
                fader.Alpha = (byte)(fader.Alpha - 1);
                fader.AlphaBlending = true;
            }
        }
        public override void Draw(CGameEngine game) {
            Video.Screen.Blit(bg);
            if (fader.Alpha != 0)
                Video.Screen.Blit(fader);
        }
        public static CIntroState Instance() {
            if (m_IntroState == null)
                m_IntroState = new CIntroState();
            return m_IntroState;
        }
    }
}
