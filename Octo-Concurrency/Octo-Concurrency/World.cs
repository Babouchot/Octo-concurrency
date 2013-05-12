using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Threading;

namespace OctoConcurrency
{
	/**
	 * The class representing the simulation world <br>
	 * Contains all the entities and obstacles and hold the objective location <br>
	 * Handle all the game update calculation and the game logic
	 **/
	public class World
	{
		private volatile List<Entity> entities;
		private List<Obstacle> obstacles;
		private Vector2 objective;
		private Vector2 size;
		private Texture2D entityTexture;
		private Texture2D obstacleTexture;
		private Texture2D objectiveTexture;
		private List<Thread> threads;

		//zones to lock before update
		private volatile List<List<Mutex>> lockZones;
		private int zoneWidth;
		private int zoneHeight;
		private int nbZonesPerSide = 10;

		private PathFinder pathFinder;

		public World (int xObjective, int yObjective, int width = 800, int height = 800,
		              int nbEntities = 30, int nbZonesPath = 10)
		{

			nbZonesPerSide = nbZonesPath;
			lockZones = new List<List<Mutex>>();

			for(int i = 0; i < nbZonesPerSide; ++i){
				lockZones.Add(new List<Mutex>());
				for(int j = 0; j < nbZonesPerSide; ++j){

					lockZones[i].Add(new Mutex());
				}
			}

			zoneWidth = width/nbZonesPerSide;
			zoneHeight = height/nbZonesPerSide;

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
			pathFinder = new PathFinder(this, nbZonesPerSide);

			//Add some entities to the world

			Entity ent;
			Vector2 pos;
			for(int i = 0; i < nbEntities; ++i){

				pos = new Vector2(r.Next((int)size.X), r.Next((int)size.Y));
				ent = new Entity(destination, pos, zoneWidth, zoneHeight);

				while(isCollidingWithObstacle(ent.Position, ent.Position) 
				      || isCollidingWithEntities(ent, ent.Position)){

					pos = new Vector2(r.Next((int)size.X), r.Next((int)size.Y));
					ent = new Entity(destination, pos, zoneWidth, zoneHeight);
				}
				entities.Add(ent);
				//find the first goal of each entities
				ent.Destination = pathFinder.findClosestSubGoal(ent.Position, this);
			}

			threads = new List<Thread>();
			foreach(Entity enti in entities){

				Thread t = new Thread(enti.autonomousUpdate);
				threads.Add(t);

			}

		}

		public void startThreads(){
			foreach(Thread thread in threads){
				thread.Start();
			}
			Console.WriteLine("Threads started");
		}

		public void stopThreads(){
			foreach(Entity ent in entities){
				ent.requestStop();
			}
	
			//Rends la main aux threads pour qu'ils se stoppent.
			Thread.Sleep(100);

			Console.WriteLine("remaining threads will be killed");

			foreach(Thread th in threads){
				th.Abort();
			}

			threads.Clear();
			entities.Clear();

			Console.WriteLine("Threads stopped");
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
			//Old update method, should not be used
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

				if(e.active() && Math.Abs(e.findCurrentZone().X - ent.findCurrentZone().X) <=1 
				   && Math.Abs(e.findCurrentZone().Y - ent.findCurrentZone().Y) <=1
				   && ent != e && e.collide(ent.Position, nextPos)){

					return true;
				}
			}
			return false;
		}

		public List<Entity> Entities {
			get { return entities; }
			set { entities = value ; }
		}

		public List<Obstacle> Obstacles {
			get { return obstacles; }
		}

		public Vector2 Size {
			get { return size; }
		}

		public PathFinder Pathfinder {
			get { return pathFinder; }
		}

		public Texture2D EntityTexture {
			get { return entityTexture; }
		}

		public List<Thread> Threads {
			get { return threads; }
		}

		public int NbZonesPerSide {
			get { return nbZonesPerSide; }
		}

		public List<List<Mutex>> LockZones {
			get {
				return lockZones;
			}
		}

		/**
		 * Draw all the world elements
		 **/
		public void draw(SpriteBatch spritebatch){

			foreach (Obstacle obs in obstacles){
				obs.draw(spritebatch, obstacleTexture);
			}

			Vector2 adjustedObj = new Vector2(objective.X - objectiveTexture.Width/2, objective.Y - objectiveTexture.Height/2);
			spritebatch.Draw(objectiveTexture, adjustedObj, Color.White);

			pathFinder.debugDraw(spritebatch, obstacleTexture);
		}

	}
}

