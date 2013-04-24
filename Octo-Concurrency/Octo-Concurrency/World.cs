using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace OctoConcurrency
{
	/**
	 * The class representing the simulation world <br>
	 * Contains all the entities and obstacles and hold the objective location <br>
	 * Handle all the game update calculation and the game logic
	 **/
	public class World
	{
		private List<Entity> entities;
		private List<Obstacle> obstacles;
		private Vector2 objective;
		private Vector2 size;
		private Texture2D entityTexture;
		private Texture2D obstacleTexture;
		private Texture2D objectiveTexture;

		private PathFinder pathFinder;

		public World (int xObjective, int yObjective, int width = 800, int height = 800, int nbEntities = 30)
		{
			entities = new List<Entity>();
			obstacles = new List<Obstacle>();

			objective = new Vector2(xObjective, yObjective);
			size = new Vector2(width,height);

			Node destination = new Node(new Vector2(xObjective, yObjective));

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

			//Create the pathFinder here
			pathFinder = new PathFinder(this);

			//Add some entities to the world

			Entity ent;
			Vector2 pos;
			for(int i = 0; i < nbEntities; ++i){

				pos = new Vector2(r.Next((int)size.X), r.Next((int)size.Y));
				ent = new Entity(destination, pos);

				while(isCollidingWithObstacle(ent.Position, ent.Position) 
				      || isCollidingWithEntities(ent, ent.Position)){

					pos = new Vector2(r.Next((int)size.X), r.Next((int)size.Y));
					ent = new Entity(destination, pos);
				}
				entities.Add(ent);
				//find the first goal of each entities
				ent.Destination = pathFinder.findClosestSubGoal(ent.Position, this);
			}



		}

		public Vector2 Objective {
			get { return objective;}
		}

		/**
		 * Load the texture the world will use to draw itelf
		 **/
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

			foreach (Entity ent in entities){

				//If stuck relauch the pathfinding to find another way
				if(ent.checkIfStuck()){
					ent.Destination = pathFinder.findClosestSubGoal(ent.Position, this, ent.Destination);
				}

				rotation = 0.0f;
				left = false;
				//Try to move in several directions, once right, once left, the further right...

				nextPos = ent.calculateNextPos(rotation, timeSinceLastUpdate);

				while(nextPos.Length() > 0 && (isCollidingWithObstacle(ent.Position, nextPos)
				               || isCollidingWithEntities(ent, nextPos))){
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

				ent.move(nextPos);

				//Destination reached, remove the entity from the world
				if(ent.destinationReached()){
					//if final objective reached
					if(ent.Destination.OutNodes.Count == 0){
						toRemove.Add(ent);
					} else {
						ent.Destination = pathFinder.findNextNode(ent.Destination);
					}

				}
			}

			foreach(Entity ent in toRemove){
				entities.Remove(ent);
			}
		}

		/**
		 * Check for collision against every obstacle (including other entities)
		 **/
		public bool isCollidingWithObstacle(Vector2 oldPos, Vector2 nextPos){

			foreach(Obstacle obs in obstacles){
				if(obs.collide(oldPos, nextPos)){
					return true;
				}
			}
			return false;
		}

		public bool isCollidingWithEntities(Entity ent, Vector2 nextPos){

			foreach(Entity e in entities){
				if(ent != e && e.collide(ent.Position, nextPos)){
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

		public Vector2 Size {
			get { return size; }
		}

		/**
		 * Draw all the world elements
		 **/
		public void draw(SpriteBatch spritebatch){

			foreach (Obstacle obs in obstacles){
				obs.draw(spritebatch, obstacleTexture);
			}

			foreach (Entity ent in entities){
				ent.draw(spritebatch, entityTexture);
				//debugDraw for the stuck problem
				if(ent.checkIfStuck()){
					ent.debugDrawDestination(spritebatch);
				}

			}

			Vector2 adjustedObj = new Vector2(objective.X - objectiveTexture.Width/2, objective.Y - objectiveTexture.Height/2);
			spritebatch.Draw(objectiveTexture, adjustedObj, Color.White);

			pathFinder.debugDraw(spritebatch, obstacleTexture);
		}

	}
}

