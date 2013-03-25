using System;
using Microsoft.Xna.Framework;

namespace OctoConcurrency
{
	public class Entity
	{
		private Vector2 position;
		private Vector2 destination;
		private int taille;
		private World world;

		public Entity ()
		{
			position = new Vector2();
			destination = new Vector2();
			taille = 0;
			world = new World();
		}
	}
}

