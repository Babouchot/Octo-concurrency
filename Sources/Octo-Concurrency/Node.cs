using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace OctoConcurrency
{
	/**
	 * The node class, a node is a pathfinding entity composed of a position and a list of outgoing nodes
	 **/
	public class Node
	{

		List<Node> outNodes;
		Vector2 position;

		public Node (Vector2 pos){
			position = pos;
			outNodes = new List<Node>();
		}

		public Vector2 Position {
			get { return position; }
		}

		public List<Node> OutNodes {
			get { return outNodes; }
			set { outNodes = value; }
		}
	}
}

