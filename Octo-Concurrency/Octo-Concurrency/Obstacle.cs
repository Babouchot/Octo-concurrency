using System;
using Microsoft.Xna.Framework;

namespace OctoConcurrency
{
	public interface Obstacle
	{
		//Bastien est un gros con !
		bool collide(Entity e, Vector2 newPos);
	}
}

