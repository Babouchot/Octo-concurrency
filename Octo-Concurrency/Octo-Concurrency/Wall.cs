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

		public Wall(Vector2 startPoint, Vector2 endPoint, float rad = 1){
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
			Texture2D blank = new Texture2D(spritebatch.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
			blank.SetData(new[]{Color.White});

			GeometryTools.DrawLine(spritebatch, blank, radius, Color.Red, start, end);
		}
		

	}
}