using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace OctoConcurrency
{
	public class World
	{
		private List<Entity> entities;
		private List<Obstacle> obstacles;
		private List<Obstacle> obstaclesAndEntities;
		private Vector2 objective;
		Vector2 size;
		Texture2D entityTexture;
		Texture2D obstacleTexture;
		Texture2D objectiveTexture;

		public World (int xObjective, int yObjective, int width = 800, int height = 800, int nbEntities = 30)
		{
			entities = new List<Entity>();
			obstacles = new List<Obstacle>();
			obstaclesAndEntities = new List<Obstacle>();

			objective = new Vector2(xObjective, yObjective);
			size = new Vector2(width,height);

			Vector2 destination = new Vector2(xObjective, yObjective);

			Random r = new Random();

			//Add some obstacles

			Wall wall;
			Vector2 startWall, endWall;
			for(int i = 0; i < 4; ++i){
				startWall = new Vector2(r.Next((int)size.X), r.Next((int)size.Y));
				endWall = new Vector2(r.Next((int)size.X), r.Next((int)size.Y));
				wall = new Wall(startWall, endWall);
				obstacles.Add(wall);
			}

			obstaclesAndEntities.AddRange(obstacles);

			//Add some entities to the world

			Entity ent;
			Vector2 pos;
			for(int i = 0; i < nbEntities; ++i){

				pos = new Vector2(r.Next((int)size.X), r.Next((int)size.Y));
				ent = new Entity(destination, pos);
				while(isColliding(ent, ent.Position)){
					pos = new Vector2(r.Next((int)size.X), r.Next((int)size.Y));
					ent = new Entity(destination, pos, 10, 0.005f);
				}
				entities.Add(ent);
				obstaclesAndEntities.Add(ent);
			}



		}

		public Vector2 Objective {
			get { return objective;}
		}


		public void loadTextures(Texture2D entity, Texture2D wall, Texture2D objecti){
			entityTexture = entity;
			obstacleTexture = wall;
			objectiveTexture = objecti;
		}

		/**
		 * Update the game world by moving each of the entities toward their destination
		 **/
		public void updateWorld(float timeSinceLastUpdate) {

			float rotation;
			bool left;
			Vector2 nextPos;

			List<Entity> toRemove = new List<Entity>();
			int entCount = 0;
			foreach (Entity ent in entities){

				rotation = 0.0f;
				left = false;
				//Try to move in several directions, once right, once left, the further right...
				nextPos = ent.calculateNextPos(rotation, timeSinceLastUpdate);
				while(nextPos.Length() > 0 && isColliding(ent, nextPos)){
					rotation *= -1;
					if(left){
						rotation += 0.2f;
					}
					left = !left;
					nextPos = ent.calculateNextPos(rotation, timeSinceLastUpdate);
				}

				//if no move is possible, the entity stays where it is
				if(nextPos.Length() == 0){
					nextPos = ent.Position;
				}
				Console.Out.WriteLine("entity : " + entCount + " rotation : " + rotation);
				ent.move(nextPos);

				//Destination reached, remove the entity from the world
				if(ent.destinationReached()){
					toRemove.Add(ent);
				}

				++entCount;
			}

			foreach(Entity ent in toRemove){
				entities.Remove(ent);
				obstaclesAndEntities.Remove(ent);
			}
		}

		/**
		 * Check for collision against every obstacle (including other entities)
		 **/
		public bool isColliding(Entity ent, Vector2 nextPos){

			foreach(Obstacle obs in obstaclesAndEntities){
				if( ent!= obs && obs.collide(ent, nextPos)){
					return true;
				}
			}
			return false;
		}

		public List<Entity> Entities {
			get { return entities; }
		}

		public List<Obstacle> Obstacles {
			get { return obstacles; }
		}

		public void draw(SpriteBatch spritebatch){

			foreach (Obstacle obs in obstacles){
				obs.draw(spritebatch, obstacleTexture);
			}

			foreach (Entity ent in entities){
				ent.draw(spritebatch, entityTexture);
			}

			Vector2 adjustedObj = new Vector2(objective.X - objectiveTexture.Width/2, objective.Y - objectiveTexture.Height/2);
			spritebatch.Draw(objectiveTexture, adjustedObj, Color.White);
		}

	}
}

