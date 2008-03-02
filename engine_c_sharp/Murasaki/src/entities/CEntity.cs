using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using SdlDotNet.Core;
using SdlDotNet.Graphics;
using SdlDotNet.Input;

namespace Murasaki.Entitys {
    public class CEntity {
        public virtual Rectangle Rectangle { get { return m_rect; } }

        protected Surface m_tileset;
        protected Rectangle m_rect;

        public virtual void Draw() {}
        public virtual void CollidePlayer() {}
        public virtual void Update() {}
    }
}
