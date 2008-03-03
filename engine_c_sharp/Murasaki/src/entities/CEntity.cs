using System.Drawing;
using SdlDotNet.Graphics;

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
