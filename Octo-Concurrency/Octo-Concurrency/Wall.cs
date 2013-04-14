using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace OctoConcurrency
{
	public class Wall : Obstacle
	{
		private Vector2 start;
		private Vector2 end;
		private float radius;

		public Wall(Vector2 startPoint, Vector2 endPoint, float rad){
			start = startPoint;
			end = endPoint;
			radius = rad;
		}

		public bool collide(Entity ent, Vector2 newPos){
			Vector2 intersection;
			//Radius not taken into account here, it should be, have to figure out a way to do that...
			return GeometryTools.Intersects2D(start, end, ent.Position, newPos, out intersection);
		}

		public void draw(SpriteBatch spritebatch, Texture2D texture){
			Console.Out.WriteLine("Wall colinding test, rad = " + radius);
		}
		

	}
}