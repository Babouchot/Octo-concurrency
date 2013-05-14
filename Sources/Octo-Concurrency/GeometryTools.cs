using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace OctoConcurrency
{
	/**
	 * Here are stuff from the internet allowing to do some Vector calculation
	 * 
	 **/
	class GeometryTools
	{
		// Returns true if the lines intersect, otherwise false. If the lines
		// intersect, intersectionPoint holds the intersection point.
		public static bool Intersects2D(Vector2 line1Start, Vector2 line1End, Vector2 line2Start, Vector2 line2End, out Vector2 intersectionPoint)
		{
			float firstLineSlopeX, firstLineSlopeY, secondLineSlopeX, secondLineSlopeY;
			
			firstLineSlopeX = line2End.X - line2Start.X;
			firstLineSlopeY = line2End.Y - line2Start.Y;
			
			secondLineSlopeX = line1End.X - line1Start.X;
			secondLineSlopeY = line1End.Y - line1Start.Y;
			
			float s, t;
			s = (-firstLineSlopeY * (line2Start.X - line1Start.X) + firstLineSlopeX * (line2Start.Y - line1Start.Y)) / (-secondLineSlopeX * firstLineSlopeY + firstLineSlopeX * secondLineSlopeY);
			t = (secondLineSlopeX * (line2Start.Y - line1Start.Y) - secondLineSlopeY * (line2Start.X - line1Start.X)) / (-secondLineSlopeX * firstLineSlopeY + firstLineSlopeX * secondLineSlopeY);
			
			if (s >= 0 && s <= 1 && t >= 0 && t <= 1)
			{
				float intersectionPointX = line2Start.X + (t * firstLineSlopeX);
				float intersectionPointY = line2Start.Y + (t * firstLineSlopeY);
				
				// Collision detected
				intersectionPoint = new Vector2(intersectionPointX, intersectionPointY);
				
				return true;
			}
			
			intersectionPoint = Vector2.Zero;
			return false; // No collision
		}


		/**
		 * Return vect rotated by rad radians 
		 **/
		public static Vector2 RotateVector2(Vector2 vect, double rad)
		{
			float cosRadians = (float)Math.Cos(rad);
			float sinRadians = (float)Math.Sin(rad);
			
			return new Vector2(
				vect.X * cosRadians - vect.Y * sinRadians,
				vect.X * sinRadians + vect.Y * cosRadians);
		}

		public static void DrawLine(SpriteBatch batch, Texture2D blank,
		              float width, Color color, Vector2 point1, Vector2 point2)
		{
			float angle = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);
			float length = Vector2.Distance(point1, point2);
			
			batch.Draw(blank, point1, null, color,
			           angle, Vector2.Zero, new Vector2(length, width),
			           SpriteEffects.None, 0);
		}

	}
}
