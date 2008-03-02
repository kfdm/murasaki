using System;
using SdlDotNet.Core;
using SdlDotNet.Graphics;
using SdlDotNet.Input;

namespace Murasaki.State {
    public abstract class CGameState {
        public virtual void Init() { }
        public virtual void Cleanup() { }
        public virtual void Pause() { }
        public virtual void Resume() { }
        public virtual void OnKeyboardDown(CGameEngine game, KeyboardEventArgs e) { }
        public virtual void OnKeyboardUp(CGameEngine game, KeyboardEventArgs e) { }
        public virtual void OnMouseMotion(CGameEngine game, MouseMotionEventArgs e) { }
        public virtual void OnMouseButtonDown(CGameEngine game, MouseButtonEventArgs e) { }
        public virtual void Update(CGameEngine game) { }
        public virtual void Draw(CGameEngine game) { }
    }
}
