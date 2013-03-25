using System;
using Microsoft.Xna.Framework;

namespace OctoConcurrency
{
	public interface Obstacle
	{

		bool collide(Entity e, Vector2 newPos);
	}
}

