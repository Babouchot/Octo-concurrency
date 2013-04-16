using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace OctoConcurrency
{
	public class Entity : OctoConcurrency.Obstacle
	{
		private Vector2 position;
		private Node destination;
		private float radius;
		private float maxSpeed;

		public Entity (Node dest)
		{
			position = new Vector2();
			radius = 1;
			destination = dest;
		}

		public Entity(Node dest, Vector2 position, int rad = 20, float maxiSpeed = 0.1f) {
			this.position = position;
			radius = rad;
			this.destination = dest;
			maxSpeed = maxiSpeed;
		}


		public float Radius {
			get { return radius; }
			set { radius = value; }
		}

		public Vector2 Position {
			get { return position; }
			set { position = value; }
		}

		public Node Destination {
			get { return destination; }
			set { destination = value; }
		}

		public bool destinationReached(){
			return (destination.Position - position).Length() < radius;
		}

		/**
		 * Calculate the next position
		 * The calculated move will be redirected according to the rotationOffset ( 0 = none 1 = half turn)
		 * Rotation Offset should be between 0 and 2
		 */
		public Vector2 calculateNextPos(float rotationOffset, float timeSinceLastUpdate){

			if(rotationOffset > 1 || rotationOffset < -1){
				Console.Out.WriteLine("rotation out of bound");
				return new Vector2(0,0);
			}

			//Calculate and move down to maxSpeed if needed
			Vector2 tempMove = destination.Position - position;
			//Console.Out.WriteLine("the line right after this might not work as intended");
			if(tempMove.Length() > maxSpeed * timeSinceLastUpdate) {
				tempMove.Normalize();
				tempMove *= maxSpeed * timeSinceLastUpdate;
			}

			//Rotate the move vector by rotationOffset * Pi
			tempMove = GeometryTools.RotateVector2(tempMove, rotationOffset * Math.PI);

			return tempMove + position;
		}

		//Move the entity to its new position
		public void move(Vector2 newPos){
			position = newPos;
		}

		//Check if the moving entity will collide with this
		public bool collide(Vector2 oldPos, Vector2 newPos) {
			if (Vector2.Distance(position, newPos) < radius){
				return true;
			} else if (Vector2.Distance(position, oldPos) < radius && Vector2.Distance(position, newPos) < radius){
				return true;
			}
			return false;
		}

		public void draw(SpriteBatch spritebatch, Texture2D texture){
			Vector2 adjustedPos = new Vector2(position.X - texture.Width/2, position.Y - texture.Height/2);
			spritebatch.Draw (texture, adjustedPos, Color.White);
		}
	}
}

