using System;
using Microsoft.Xna.Framework;

namespace OctoConcurrency
{
	public class Entity : OctoConcurrency.Obstacle
	{
		private Vector2 position;
		private Vector2 destination;
		private float radius;
		private const float maxSpeed = 1;

		private bool reached;

		//private World world; 
		//WHY ?

		public Entity () : base(new Vector2(), 0,0)
		{
			position = new Vector2();
			destination = new Vector2();
			radius = 1;
			//world = new World();
		}

		public Entity(Vector2 position, Vector2 destination, int width, int height) : 
			base(position,
			     width,
		         height) {
			
			this.position = position;
			this.destination = destination;
		}


		public float Radius {
			get { return radius; }
			set { radius = value; }
		}

		public Vector2 Position{
			get { return position; }
			set { position = value; }
		}

		public Vector2 Destination{
			get { return destination; }
			set { Console.Out.WriteLine("Deprecated fuction ! " +
				"Bad boy ! (check with Gwen if you don't understand why");
				destination = value; }
		}

		public bool Reach {
			get { return reached; }
		}

		public Vector2 calculateNextPos(){
			return calculateNextPos(0);
		}

		/**
		 * Calculate the next position
		 * The calculated move will be redirected according to the rotationOffset ( 0 = none 1 = full turn)
		 * Rotation Offset should be between -1 and 1
		 */
		public Vector2 calculateNextPos(float rotationOffset){

			if(rotationOffset > 1 || rotationOffset < -1){
				//throw ArgumentException;
			}

			//Calculate and move down to maxSpeed if needed
			Vector2 tempMove = destination - position;
			//Console.Out.WriteLine("the line right after this might not work as intended");
			if(tempMove.Length() > maxSpeed) {
				tempMove.Normalize();
				tempMove *= maxSpeed;
			}

			//Rotate the move vector by rotationOffset * Pi
			tempMove = RotateVector2(tempMove, rotationOffset * Math.PI);

			foreach (Obstacle o in Game1.Obstacles) {
				if (o.collide(this, tempMove)) {
					//TODO
				}
			}
			return tempMove;
		}

		private static Vector2 RotateVector2(Vector2 point, double radians)
		{
			float cosRadians = (float)Math.Cos(radians);
			float sinRadians = (float)Math.Sin(radians);
			
			return new Vector2(
				point.X * cosRadians - point.Y * sinRadians,
				point.X * sinRadians + point.Y * cosRadians);
		}


		public Vector2 Populate() {
			//TODO here is the update of each entity position
			//Console.WriteLine("Old Position "+Position);
			position += calculateNextPos();
			//Console.WriteLine("New position : "+Position);
			if (position == destination)
				reached = true;
			return position;

		}

		/*public bool collide(Entity e, Vector2 newPos) {

		}*/

	}
}

