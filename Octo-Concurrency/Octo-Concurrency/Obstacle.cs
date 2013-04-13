using System;
using Microsoft.Xna.Framework;

namespace OctoConcurrency
{
	public class Obstacle
	{

		private int width;
		private int height;
		private Vector2 coordinates;

		public Obstacle(Vector2 coordinates, int width, int height) {
			this.coordinates = coordinates;
			this.width = width;
			this.height = height;
		}

		public int Width {
			get { return width; }
		}

		public int Height {
			get { return height; }
		}

		public Vector2 Coordinates {
			get { return coordinates; }
			set { coordinates = value; }
		}

		public bool collide(Entity e, Vector2 newPos) {
			//TODO
			return false;
		}
	}
}

