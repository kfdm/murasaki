using System;
using System.Xml;
using Murasaki.Map;

namespace Murasaki.Entities {
	class CEntityFactory {
		public static void Factory(CMap map, XmlNode entity) {
			XmlAttributeCollection attr = entity.Attributes;
			XmlNodeList properties = entity.FirstChild.ChildNodes;
			switch (attr["type"].Value) {
				case "CEntityTransition":
					new CEntityTransition(map, attr, properties);
					break;
				case "CEntityMonsterSpawn":
					new CEntityMonsterSpawn(map, attr, properties);
					break;
				default:
					Console.WriteLine("Unknown Entity Type {0} in LoadMapObjectgroup()", attr["type"].Value);
					break;
			}
		}
	}
}
