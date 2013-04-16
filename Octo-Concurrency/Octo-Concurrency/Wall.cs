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

		public bool collide(Vector2 oldPos, Vector2 newPos){
			Vector2 intersection;
			//Radius not taken into account here, it should be, have to figure out a way to do that...
			return GeometryTools.Intersects2D(start, end, oldPos, newPos, out intersection);
		}

		public void draw(SpriteBatch spritebatch, Texture2D texture){

			Texture2D wallText = new Texture2D(spritebatch.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
			wallText.SetData(new[]{Color.White});
			GeometryTools.DrawLine(spritebatch, wallText, radius, Color.Red, start, end);
		}
		

	}
}