using System.Drawing;
using SdlDotNet.Graphics;
using Murasaki.Actors;
namespace Murasaki.Entitys {
	/// <summary>
	/// Base Game Entity
	/// </summary>
	public abstract class CEntity {
		#region Properties
		//public virtual Rectangle Rectangle { get { return m_rect; } }
		public int Top { get { return m_rect.Top; } set { m_rect.Y = value; } }
		public int Bottom { get { return m_rect.Bottom; } set { m_rect.Y = (value - m_rect.Height); } }
		public int Left { get { return m_rect.Left; } set { m_rect.X = value; } }
		public int Right { get { return m_rect.Right; } set { m_rect.X = (value - m_rect.Width); } }
		public int Height { get { return m_rect.Height; } }
		public int Width { get { return m_rect.Width; } }
		#endregion

		#region Fields
		protected Rectangle m_rect;
		#endregion

		#region Methods
		public virtual void Draw() { }
		public virtual void CollidePlayer() { }
		public virtual void Update() { }
		public bool IntersectsWith(CEntity entity) {
			return m_rect.IntersectsWith(entity.m_rect);

		}
		public bool IntersectsWith(Rectangle rect) {
			return m_rect.IntersectsWith(rect);
		}

		public Point Center() {
			return new Point(m_rect.X + m_rect.Width / 2, m_rect.Y + m_rect.Height / 2);
		}
		#endregion
	}
}
