using System.Collections.Generic;
using System.Drawing;
using SdlDotNet.Graphics;
using Murasaki.Map;

namespace Murasaki.Actors {
	public enum ActorDirection { Up, Down, Left, Right, Stop }

	public abstract class CActor : Murasaki.Entities.CEntity {
		#region Properties
		/// <summary>
		/// Set if the actor is moving up
		/// </summary>
		public virtual bool moveup { get { return m_moveup; } set { m_moveup = value; m_direction = ActorDirection.Up; } }
		/// <summary>
		/// Set if the actor is moving up
		/// </summary>
		public virtual bool movedown { get { return m_movedown; } set { m_movedown = value; m_direction = ActorDirection.Down; } }
		/// <summary>
		/// Set if the Actor is moving left
		/// </summary>
		public virtual bool moveleft { get { return m_moveleft; } set { m_moveleft = value; m_direction = ActorDirection.Left; } }
		/// <summary>
		/// Set if the Actor is moving right
		/// </summary>
		public virtual bool moveright { get { return m_moveright; } set { m_moveright = value; m_direction = ActorDirection.Right; } }
		/// <summary>
		/// Actor's moveing speed
		/// </summary>
		public virtual int MoveSpeed { get { return m_movespeed; } set { m_movespeed = value; } }
		/// <summary>
		/// Direction the actor is moving
		/// </summary>
		public virtual ActorDirection Direction { get { return m_direction; } }
		public bool Collideable { get { return m_collidable; } set { m_collidable = value; } }

		protected Surface m_tileset;
		protected bool m_moveup, m_movedown, m_moveright, m_moveleft;
		protected int m_movespeed;
		protected ActorDirection m_direction = ActorDirection.Up;
		protected int m_walkframe = 0, m_walkdelay = 0;
		protected bool m_collidable;
		protected CMap m_map;
		#endregion

		public virtual void Draw(Surface dest, Rectangle Camera) {
			Rectangle srcRect = new Rectangle(0, m_rect.Height * 2, m_rect.Width, m_rect.Height);

			switch (m_direction) {
				case ActorDirection.Up:
					srcRect.Y = 0;
					break;
				case ActorDirection.Down:
					srcRect.Y = m_rect.Height * 2;
					break;
				case ActorDirection.Left:
					srcRect.Y = m_rect.Height * 3;
					break;
				case ActorDirection.Right:
					srcRect.Y = m_rect.Height;
					break;
			}
			srcRect.X = m_rect.Width * m_walkframe;

			Camera.X = m_rect.X - Camera.X;
			Camera.Y = m_rect.Y - Camera.Y;
			Camera.Width = m_rect.Width;
			Camera.Height = m_rect.Height;

			dest.Blit(m_tileset, Camera, srcRect);
		}
		/// <summary>
		/// Called when an actor is hit by another actor
		/// </summary>
		/// <param name="hitby">Actor that hit the current actor</param>
		/// <param name="toRemoveSelf">List to remove the current actor</param>
		/// <param name="toRemoveOther">List to remove the other actor</param>
		public virtual void GotHit(CActor hitby, List<CActor> toRemoveSelf, List<CActor> toRemoveOther) { }
		public virtual void CollideWall(List<CActor> toRemove) { }
		public virtual void ReverseMovement() {
			if (m_moveup)
				m_rect.Y += m_movespeed;
			if (m_movedown)
				m_rect.Y -= m_movespeed;
			if (m_moveleft)
				m_rect.X += m_movespeed;
			if (m_moveright)
				m_rect.X -= m_movespeed;
		}
		public virtual void ReverseMovement(ActorDirection direction) {
			switch (direction) {
				case ActorDirection.Up:
					m_rect.Y += m_movespeed;
					break;
				case ActorDirection.Down:
					m_rect.Y -= m_movespeed;
					break;
				case ActorDirection.Left:
					m_rect.X += m_movespeed;
					break;
				case ActorDirection.Right:
					m_rect.X -= m_movespeed;
					break;
			}
		}
		public override void Update() {
			//Move Actors
			if (m_moveup)
				m_rect.Y -= m_movespeed;
			if (m_movedown)
				m_rect.Y += m_movespeed;
			if (m_moveleft)
				m_rect.X -= m_movespeed;
			if (m_moveright)
				m_rect.X += m_movespeed;
			//Update animation
			if (m_moveup || m_movedown || m_moveleft || m_moveright) {
				if (m_walkdelay > 10) {
					m_walkdelay = 0;
					m_walkframe = (m_walkframe + 1) % 3;
				}
				m_walkdelay++;
			}
			if (m_collidable) {
				//Check to see if actors collided
				foreach (CActor actor in m_map.Actors) {
					if (actor != this) {
						if (actor.IntersectsWith(m_rect)) {
							ReverseMovement();
						}
					}
				}
				//Colide with Avatar
				if (!(this is CActorPlayer))
					if (m_map.Avatar.IntersectsWith(m_rect)) {
						ReverseMovement();
					}
			}
		}
	}
}
