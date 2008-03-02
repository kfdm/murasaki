using System;
using System.Xml;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using SdlDotNet.Core;
using SdlDotNet.Graphics;
using SdlDotNet.Input;

using Murasaki.State;
using Murasaki.Map;

namespace Murasaki.Entitys {
    public class CEntityTransition : CEntity {
        CTileMap m_map;
        String newmap;
        int newx, newy;
        public CEntityTransition(CTileMap map,XmlAttributeCollection attributes, XmlNodeList properties) {
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
            CTileMap map = new CTileMap(newmap, newx, newy);
            CPlayState.Instance().ChangeMap(map);
        }
    }
}
