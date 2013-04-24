using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace OctoConcurrency
{
	/**
	 * Entity class, represent a moving, living entity from the game
	 **/
	public class Entity : OctoConcurrency.Obstacle
	{
		private Vector2 position;
		private Node destination;
		//The radius of the entity, used to know how mush space the entity occupies
		private double radius;
		private float maxSpeed;
	
		private int nbLastMoves = 20;
		private List<Vector2> lastMoves;


		/**
		 * Create a new entity with the given parameters
		 * The default radius and maxiSpeed may change
		 **/
		public Entity(Node dest, Vector2 position, int rad = 20, float maxiSpeed = 0.1f) {
			this.position = position;
			radius = rad;
			this.destination = dest;
			maxSpeed = maxiSpeed;

			lastMoves = new List<Vector2>(nbLastMoves);
		}


		public double Radius {
			get { return radius; }
			set { radius = value; }
		}

		public Vector2 Position {
			get { return position; }
			set { position = value; }
		}

		public Node Destination {
			get { return destination; }
			//If the destination changes, reset the stuck checking List to avoid a false stuck detection
			set { destination = value; lastMoves.Clear(); }
		}

		/**
		 * Check if the entity is on its destination <br>
		 * Return true if the destination has been reached
		 **/
		public bool destinationReached(){

			if(destination == null){
				return false;
			}

			if(destination.OutNodes.Count==0){

				return (destination.Position - position).Length() < radius;

			} else {

				return (destination.Position - position).Length() < radius/10;

			}

		}

		/**
		 * Calculate the next position <br>
		 * The calculated move will be redirected according to the rotationOffset ( 0 = none 1 = half turn) <br>
		 * Rotation Offset should be between 0 and 2
		 */
		public Vector2 calculateNextPos(float rotationOffset, double timeSinceLastUpdate){

			if(rotationOffset > 1 || rotationOffset < -1){
				Console.Out.WriteLine("rotation out of bound");
				return new Vector2(0,0);
			}

			//Calculate and reduce to maxSpeed if needed
			Vector2 tempMove = destination.Position - position;

			tempMove *= (float)timeSinceLastUpdate;
			//Rotate the move vector by rotationOffset * Pi
			tempMove = GeometryTools.RotateVector2(tempMove, rotationOffset * Math.PI);

			if(tempMove.Length() > maxSpeed){

				tempMove.Normalize();
				tempMove *= (float)timeSinceLastUpdate * maxSpeed;
			}

			return tempMove + position;
		}


		/**
		 * Check if the entity is stuck by calculating the total motion ober the last X move <br>
		 * And by cheking if it is to small
		 **/
		public bool checkIfStuck(){

			Vector2 globalMove;
			foreach( Vector2 move in lastMoves ){
				globalMove += move;
			}
			return lastMoves.Count >= nbLastMoves/2 && globalMove.Length() < radius / 2;
		}

		/**
		 * Move the entity to its new position
		 **/
		public void move(Vector2 newPos){

			Vector2 move = newPos - position;
			if(lastMoves.Count >= nbLastMoves){
				lastMoves.RemoveAt(0);
			}
			lastMoves.Add(move);
			position = newPos;

		}

		/*
		 * Check if the moving entity will collide with this
		 **/
		public bool collide(Vector2 oldPos, Vector2 newPos) {
			if (Vector2.Distance(position, newPos) < radius){
				return true;
			} else if (Vector2.Distance(position, oldPos) < radius && Vector2.Distance(position, newPos) < radius){
				return true;
			}
			return false;
		}

		/**
		 * Draw the entity at its position with the given texture
		 **/
		public void draw(SpriteBatch spritebatch, Texture2D texture){
			Vector2 adjustedPos = new Vector2(position.X - texture.Width/2, position.Y - texture.Height/2);
			spritebatch.Draw (texture, adjustedPos, Color.White);
		}

		public void debugDrawDestination(SpriteBatch spritebatch){
			//debug draw for the pathfinding
			
			Texture2D wallText = new Texture2D(spritebatch.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
			wallText.SetData(new[]{Color.White});
			GeometryTools.DrawLine(spritebatch, wallText, 1, Color.Blue, position, destination.Position);
		}
	}
}

