using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace OctoConcurrency
{
	public class PathFinder
	{
		List<Node> nodes;
		Node objective;

		/**
		 * Initialize the pathfinder and the navigation graph
		 *
		 **/
		public PathFinder (World world, int nbNodesPerSide){

			nodes = new List<Node>();
			List<Node> remainingNodes = new List<Node>();// = ALL
			List<Node> toProcess = new List<Node>();
			List<Node> toProcessLater = new List<Node>();
			List<Node> toRemove = new List<Node>();


			// , les mettres dans toProcessLater et ajouter n à l.outNodes

			Vector2 size = world.Size;

			objective = new Node(world.Objective);

			for(int x = 0; x < size.X; x += (int)size.X/nbNodesPerSide){
				for(int y = 0; y < size.Y; y += (int)size.Y/nbNodesPerSide){
					remainingNodes.Add(new Node(new Vector2(x, y)));
				}
			}

			toProcess.Add(objective); // la premiere node a traiter est la destination finale

			//Tant qu'il reste des nodes à traiter
			while(toProcess.Count != 0){

				//pour chaque nodes n de toProcess,
				//on doit chercher les nodes accessible l dans remainingNode
				foreach( Node outNode in toProcess ){
					Console.Out.WriteLine("toProcess : " + toProcess.Count);
					foreach( Node inNode in remainingNodes ){
						Console.Out.WriteLine("remaining : "+remainingNodes.Count);
						if(!world.isCollidingWithObstacle(outNode.Position, inNode.Position)){
							//Chaque node accessible est mise de coté pour le prochain traitement
							if(toProcessLater.IndexOf(inNode) < 0){
								toProcessLater.Add(inNode);
								toRemove.Add(inNode);
							}
							//Chaque node accessible se voit ajouté outNode a sa liste de nodes sortantes
							inNode.OutNodes.Add(outNode);
							//On se prépare a retirer la node trouvée de remainingNode
						}
					}
					nodes.Add(outNode);
				}

				//Retrait des nodes trouvées de remainingNodes
				foreach( Node nod in toRemove ){
					remainingNodes.Remove(nod);
				}

				toProcess.Clear();
				toProcess.AddRange(toProcessLater);
				toProcessLater.Clear();

				Console.Out.WriteLine("count : " + toProcess.Count);
			}
		}

		/**
		 * Find the closest reachable node from the position
		 **/
		public Node findClosestSubGoal(Vector2 pos, World world, Node lastGoal=null){
			Node closest = objective;

			foreach( Node nod in nodes){
				if(nod != lastGoal && !world.isCollidingWithObstacle(pos, nod.Position) 
				   && (nod.Position - pos).Length() < (closest.Position - pos).Length()){
					closest = nod;
				}
			}

			return closest;
		}

		/**
		 * Find the next destination after reaching the current node
		 **/
		public Node findNextNode(Node current){

			List<Node> outNodes = current.OutNodes;

			//That shouldn't happen...really
			if(outNodes.Count == 0){
				Console.Error.WriteLine("Error in during findNextNode, destination is already reached");
				return null;
			}

			Node closestToEnd = outNodes[0];

			//Look for the most direct way
			foreach(Node nod in outNodes){
				if((nod.Position - objective.Position).Length() < (closestToEnd.Position - objective.Position).Length()){
					closestToEnd = nod;
				}
			}

			return closestToEnd;
		}


		public void debugDraw(SpriteBatch spritebatch, Texture2D texture){
			foreach (Node nod in nodes){
				Vector2 adjustedPos = new Vector2(nod.Position.X - texture.Width/2, nod.Position.Y - texture.Height/2);
				spritebatch.Draw (texture, adjustedPos, Color.White);
			}
		}
	}
}

