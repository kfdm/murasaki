using System;
using System.Xml;
using System.Collections.Generic;
using Murasaki.Actors;
using Murasaki.Map;

namespace Murasaki.Entitys {
    class CEntityMonsterSpawn : CEntity {
        public CEntityMonsterSpawn(CTileMap map, XmlAttributeCollection attributes, XmlNodeList properties) {
            int x, y;
            Dictionary<string, string> props = new Dictionary<string, string>();
            foreach (XmlNode node in properties)
                props.Add(node.Attributes["name"].Value, node.InnerText.Trim());
            switch (props["MonsterType"]) {
                case "CActorCivilian":
                    x = Convert.ToInt16(props["X"]);
                    y = Convert.ToInt16(props["Y"]);
                    map.Actors.AddLast(new CActorCivilian(map, x, y));
                    break;
                case "CActorMonster":
                    x = Convert.ToInt16(props["X"]);
                    y = Convert.ToInt16(props["Y"]);
                    map.Actors.AddLast(new CActorMonster(map, x, y));
                    break;
            }
        }
    }
}
