﻿using System;
using System.Xml;
using System.Collections.Generic;
using System.Text;

namespace Murasaki {
    class CEntityMonsterSpawn : CEntity {
        public CEntityMonsterSpawn(CTileMap map, XmlAttributeCollection attributes, XmlNodeList properties) {
            Dictionary<string, string> props = new Dictionary<string, string>();
            foreach (XmlNode node in properties)
                props.Add(node.Attributes["name"].Value, node.InnerText.Trim());
            switch (props["MonsterType"]) {
                case "CActorCivilian":
                    int x = Convert.ToInt16(props["X"]);
                    int y = Convert.ToInt16(props["Y"]);
                    map.Actors.AddLast(new CActorCivilian("data/pink.png", x, y, 24));
                    break;
            }
        }
    }
}