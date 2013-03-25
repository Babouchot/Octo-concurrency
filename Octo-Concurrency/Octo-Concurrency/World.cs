using System;
using System.Collections.Generic;

namespace OctoConcurrency
{
	public class World
	{
		private List<Entity> entities;
		private List<Obstacle> obstacles;

		public World ()
		{
			entities = new List<Entity>();
			obstacles = new List<Obstacle>();
		}
	}
}

