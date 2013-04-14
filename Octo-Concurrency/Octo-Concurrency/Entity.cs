using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace OctoConcurrency
{
	public class Entity : OctoConcurrency.Obstacle
	{
		private Vector2 position;
		private Vector2 destination;
		private float radius;
		private float maxSpeed;

		public Entity (Vector2 dest)
		{
			position = new Vector2();
			radius = 1;
			destination = dest;
		}

		public Entity(Vector2 dest, Vector2 position, int rad = 20, float maxiSpeed = 0.1f) {
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

		public Vector2 Destination {
			get { return destination; }
		}

		public bool destinationReached(){
			return (destination - position).Length() < radius;
		}

		/**
		 * Calculate the next position
		 * The calculated move will be redirected according to the rotationOffset ( 0 = none 1 = half turn)
		 * Rotation Offset should be between 0 and 2
		 */
		public Vector2 calculateNextPos(float rotationOffset, float timeSinceLastUpdate){

			if(rotationOffset > 1 || rotationOffset < -1){
				return new Vector2(0,0);
			}

			//Calculate and move down to maxSpeed if needed
			Vector2 tempMove = destination - position;
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
			this.Position = newPos;
		}

		//Check if the moving entity will collide with this
		public bool collide(Entity e, Vector2 newPos) {
			if (Vector2.Distance(position, newPos) < radius){
				return true;
			} else if (Vector2.Distance(position, e.Position) < radius && Vector2.Distance(position, newPos) < radius){
				return true;
			}
			return false;
		}

		public void draw(SpriteBatch spritebatch, Texture2D texture){
			spritebatch.Draw (texture, position, Color.White);
		}
	}
}

