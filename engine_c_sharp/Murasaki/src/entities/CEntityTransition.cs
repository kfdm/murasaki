using System;
using System.Xml;
using System.Drawing;
using Murasaki.State;
using Murasaki.Map;

namespace Murasaki.Entitys {
	public class CEntityTransition : CEntity {
		CMap m_map;
		String newmap;
		int newx, newy;
		public CEntityTransition(CMap map, XmlAttributeCollection attributes, XmlNodeList properties) {
			m_map = map;

			int l = Convert.ToInt16(attributes["x"].Value);
			int t = Convert.ToInt16(attributes["y"].Value);
			int w = Convert.ToInt16(attributes["width"].Value);
			int h = Convert.ToInt16(attributes["height"].Value);
			m_rect = new Rectangle(l, t, w, h);

			foreach (XmlNode node in properties) {
				switch (node.Attributes["name"].Value) {
					case "Map":
						newmap = node.InnerText.Trim();
						break;
					case "X":
						newx = Convert.ToInt16(node.InnerText.Trim());
						break;
					case "Y":
						newy = Convert.ToInt16(node.InnerText.Trim());
						break;
				}
			}
		}
		public override void CollidePlayer() {
			Console.WriteLine("Triggered World Transition");
			CMap map = new CMap(newmap, newx, newy);
			CPlayState.Instance().ChangeMap(map);
		}
	}
}
