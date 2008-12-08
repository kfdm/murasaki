using System.Drawing;
using SdlDotNet.Graphics;
using SdlDotNet.Input;
using Murasaki.Map;

namespace Murasaki.Actors {
	public class CActorPlayer : CActor {
		public override bool moveup { get { return m_moveup; } set { m_moveup = value; setDirection(); } }
		public override bool movedown { get { return m_movedown; } set { m_movedown = value; setDirection(); } }
		public override bool moveleft { get { return m_moveleft; } set { m_moveleft = value; setDirection(); } }
		public override bool moveright { get { return m_moveright; } set { m_moveright = value; setDirection(); } }

		public CActorPlayer(CTileMap map) {
			m_tileset = new Surface("Data/avatar.png");
			m_rect = new Rectangle(0, 0, m_tileset.Width / 3, m_tileset.Height / 4);
			m_tileset.Transparent = true;
			m_tileset.TransparentColor = Color.FromArgb(255, 0, 255);

			m_moveup = m_movedown = m_moveleft = m_moveright = false;
			m_movespeed = 2;
			m_map = map;
			m_collidable = true;
		}
		~CActorPlayer() {
			m_tileset.Dispose();
		}
		public void setDirection(KeyboardEventArgs key) {
			switch (key.Key) {
				case Key.UpArrow:
					m_direction = ActorDirection.Up;
					break;
				case Key.DownArrow:
					m_direction = ActorDirection.Down;
					break;
				case Key.LeftArrow:
					m_direction = ActorDirection.Left;
					break;
				case Key.RightArrow:
					m_direction = ActorDirection.Right;
					break;
			}
		}
		public void setDirection() {
			if (m_moveup)
				m_direction = ActorDirection.Up;
			if (m_moveleft)
				m_direction = ActorDirection.Left;
			if (m_movedown)
				m_direction = ActorDirection.Down;
			if (m_moveright)
				m_direction = ActorDirection.Right;
		}
		public override void GotHit(CActor hitby, System.Collections.Generic.List<CActor> toRemoveSelf, System.Collections.Generic.List<CActor> toRemoveOther) {
			System.Console.WriteLine("Got Hit!");
			toRemoveOther.Add(hitby);
		}
	}
}
