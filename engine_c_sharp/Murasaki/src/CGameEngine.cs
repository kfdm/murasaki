using System;
using System.Collections;
using SdlDotNet.Core;
using SdlDotNet.Graphics;
using SdlDotNet.Input;
using Murasaki.State;

namespace Murasaki  {
    public class CGameEngine {
        /// <summary>
        /// Stack of states
        /// </summary>
        private Stack states = new Stack();
        /// <summary>
        /// Main drawing screen
        /// </summary>
        public Surface screen;
        /// <summary>
        /// Initilize the Engine
        /// </summary>
        /// <param name="title">Title Bar Text</param>
        public void Init(string title) {
            this.Init(title,800,600,0,60,false);
        }
        /// <summary>
        /// Initilize the Engine
        /// </summary>
        /// <param name="title">Title Bar Text</param>
        /// <param name="width">Window Width</param>
        /// <param name="height">Window Height</param>
        /// <param name="bpp">Bits per Pixel</param>
        /// <param name="fps">Frames per second</param>
        /// <param name="fullscreen">Full Screen</param>
        public void Init(string title, int width, int height, int bpp, int fps, bool fullscreen) {
            Video.Initialize();
            Video.WindowCaption = title;
            Video.SetVideoMode(width, height, bpp);
            Video.Screen.AlphaBlending = true;

            Events.TargetFps = fps;
            Events.KeyboardDown += new EventHandler<KeyboardEventArgs>(this.OnKeyboardDown);
            Events.KeyboardUp += new EventHandler<KeyboardEventArgs>(this.OnKeyboardUp);
            Events.MouseMotion += new EventHandler<MouseMotionEventArgs>(this.OnMouseMotion);
            Events.MouseButtonDown += new EventHandler<MouseButtonEventArgs>(this.OnMouseButtonDown);
            Events.Tick += new EventHandler<TickEventArgs>(this.OnTick);
            Events.Quit += new EventHandler<QuitEventArgs>(this.OnQuit);
        }
        private void OnKeyboardDown(object sender, KeyboardEventArgs e) {
            ((CGameState)states.Peek()).OnKeyboardDown(this, e);
        }
        private void OnKeyboardUp(object sender, KeyboardEventArgs e) {
            ((CGameState)states.Peek()).OnKeyboardUp(this, e);
        }
        private void OnMouseMotion(object sender, MouseMotionEventArgs e) {
            ((CGameState)states.Peek()).OnMouseMotion(this, e);
        }
        private void OnMouseButtonDown(object sender, MouseButtonEventArgs e) {
            ((CGameState)states.Peek()).OnMouseButtonDown(this, e);
        }
        /// <summary>
        /// Main Engine Loop
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTick(object sender, TickEventArgs e) {
            ((CGameState)states.Peek()).Update(this);
            ((CGameState)states.Peek()).Draw(this);
            Video.WindowCaption = Events.Fps.ToString() + " fps";
            Video.Screen.Update();
        }
        private void OnQuit(object sender, QuitEventArgs e) {
            Events.Remove();
            Events.QuitApplication();
        }
        public void Cleanup() {
            while (states.Count > 0) {
                ((CGameState)states.Peek()).Cleanup();
                states.Pop();
            }
        }
        public void ChangeState(CGameState state) {
            if (states.Count > 0) {
                ((CGameState)states.Peek()).Cleanup();
                states.Pop();
            }
            states.Push(state);
            ((CGameState)states.Peek()).Init();
        }
        public void PushState(CGameState state) {
            if (states.Count > 0) {
                ((CGameState)states.Peek()).Pause();
            }
            states.Push(state);
            ((CGameState)states.Peek()).Init();
        }
        public void PopState() {
            if (states.Count > 0) {
                ((CGameState)states.Peek()).Cleanup();
                states.Pop();
            }
            if (states.Count > 0) {
                ((CGameState)states.Peek()).Resume();
            }
        }
        public void Run() {
            Events.Run();
        }
        public static void Main() {
            CGameEngine game = new CGameEngine();
            game.Init("Engine Test");
            //game.ChangeState(CIntroState.Instance());
            game.ChangeState(CPlayState.Instance());
            game.Run();
        }
    }
}