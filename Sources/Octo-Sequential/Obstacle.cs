using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OctoConcurrency
{
	/**
	 * Interface for any Obstacle (which can be collided with) object
	 **/
	public interface Obstacle
	{
		bool collide(Vector2 oldPos, Vector2 newPos);
		void draw(SpriteBatch spritebatch, Texture2D texture);
	}
}

