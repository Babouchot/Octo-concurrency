using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Threading;

namespace OctoConcurrency
{
	/**
	 * Entity class, represthis a moving, living Entity from the game
	 **/
	public class Entity : OctoConcurrency.Obstacle
	{
		private Vector2 position;
		private Node destination;
		//The radius of the Entity, used to know how mush space the Entity occupies
		private double radius;
		private float maxSpeed;
		private float lastUpdateTime;
		private int nbLastMoves = 20;
		private volatile bool toRemove;
		private List<Vector2> lastMoves;
		private int zoneWidth;
		private int zoneHeight;


		/**
		 * Create a new Entity with the given parameters
		 * The default radius and maxiSpeed may change
		 **/
		public Entity(Node dest, Vector2 position, int zoneWi, int zoneHei, int rad = 20, float maxiSpeed = 0.07f) {
			this.position = position;
			radius = rad;
			this.destination = dest;
			maxSpeed = maxiSpeed;
			lastUpdateTime = 0;
			lastMoves = new List<Vector2>(nbLastMoves);
			toRemove = false;
			zoneHeight = zoneHei;
			zoneWidth = zoneWi;
		}


		public double Radius {
			get { return radius; }
			set { radius = value; }
		}

		public Vector2 Position {
			get { return position; }
			set { position = value; }
		}

		public Node Destination {
			get { return destination; }
			//If the destination changes, reset the stuck checking List to avoid a false stuck detection
			set { destination = value; lastMoves.Clear(); }
		}

		/**
		 * Check if the Entity is on its destination <br>
		 * Return true if the destination has been reached
		 **/
		public bool destinationReached(){

			if(destination == null){
				return false;
			}

			if(destination.OutNodes.Count==0){

				return (destination.Position - position).Length() < radius;

			} else {

				return (destination.Position - position).Length() < radius/10;

			}

		}

		/**
		 * Calculate the next position <br>
		 * The calculated move will be redirected according to the rotationOffset ( 0 = none 1 = half turn) <br>
		 * Rotation Offset should be between 0 and 2
		 */
		public Vector2 calculateNextPos(float rotationOffset, double timeSinceLastUpdate){

			if(rotationOffset > 1 || rotationOffset < -1){
				Console.Out.WriteLine("rotation out of bound");
				return new Vector2(0,0);
			}

			//Calculate and reduce to maxSpeed if needed
			Vector2 tempMove = destination.Position - position;

			tempMove *= (float)timeSinceLastUpdate;
			//Rotate the move vector by rotationOffset * Pi
			tempMove = GeometryTools.RotateVector2(tempMove, rotationOffset * Math.PI);

			if(tempMove.Length() > maxSpeed){

				tempMove.Normalize();
				tempMove *= (float)timeSinceLastUpdate * maxSpeed;
			}

			return tempMove + position;
		}


		/**
		 * Check if the Entity is stuck by calculating the total motion ober the last X move <br>
		 * And by cheking if it is to small
		 **/
		public bool checkIfStuck(){

			Vector2 globalMove = Vector2.Zero;
			foreach( Vector2 move in lastMoves ){
				globalMove += move;
			}
			return lastMoves.Count >= nbLastMoves/2 && globalMove.Length() < radius / 2;
		}

		/**
		 * Move the Entity to its new position
		 **/
		public void move(Vector2 newPos){

			Vector2 move = newPos - position;
			if(lastMoves.Count >= nbLastMoves){
				lastMoves.RemoveAt(0);
			}
			lastMoves.Add(move);
			position = newPos;

		}

		/*
		 * Check if the moving Entity will collide with this
		 **/
		public bool collide(Vector2 oldPos, Vector2 newPos) {
			if (Vector2.Distance(position, newPos) < radius){
				return true;
			} else if (Vector2.Distance(position, oldPos) < radius && Vector2.Distance(position, newPos) < radius){
				return true;
			}
			return false;
		}

		/**
		 * Draw the Entity at its position with the given texture
		 **/
		public void draw(SpriteBatch spritebatch, Texture2D texture){
			//spritebatch.Begin();
			if(!toRemove){
				Vector2 adjustedPos = new Vector2(position.X - texture.Width/2, position.Y - texture.Height/2);
				spritebatch.Draw (texture, adjustedPos, Color.White);
			}

			//spritebatch.End();
		}

		/*public void debugDrawDestination(SpriteBatch spritebatch){
			//debug draw for the pathfinding
			
			Texture2D wallText = new Texture2D(spritebatch.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
			wallText.SetData(new[]{Color.White});
			GeometryTools.DrawLine(spritebatch, wallText, 1, Color.Blue, position, destination.Position);
		}*/


		/**
		 * Update the entity
		 **/
		private void updateEntity(){

			if(!Game1.paused){
				
				float rotation;
				bool left;
				Vector2 nextPos;
				if(lastUpdateTime == 0){
					lastUpdateTime = Game1.currentTime -10;
				}
				float timeSinceLastUpdate = Game1.currentTime - lastUpdateTime;
				lastUpdateTime = Game1.currentTime;
				
				
				//If stuck relauch the pathfinding to find another way
				if(this.checkIfStuck()){
					this.Destination = Game1.world.Pathfinder.findClosestSubGoal(this.Position, Game1.world, this.Destination);
				}
				
				rotation = 0.0f;
				left = false;
				//Try to move in several directions, once right, once left, the further right...
				
				nextPos = this.calculateNextPos(rotation, timeSinceLastUpdate);
				
				while(nextPos.Length() > 0 && (Game1.world.isCollidingWithObstacle(this.Position, nextPos)
				                               || Game1.world.isCollidingWithEntities(this, nextPos))){
					rotation *= -1;
					if(left){
						rotation += 0.2f;
					}
					left = !left;
					nextPos = this.calculateNextPos(rotation, timeSinceLastUpdate);
				}
				
				//if no move is possible, the Entity stays where it is
				if(nextPos.Length() == 0){
					nextPos = this.Position;
				}
				
				this.move(nextPos);
				
				//Destination reached, remove the Entity from the world
				if(this.destinationReached()){
					//if final objective reached
					if(this.Destination.OutNodes.Count == 0){
						toRemove = true;
					} else {
						this.Destination = Game1.world.Pathfinder.findNextNode(this.Destination);
					}
					
				}
				
				if(toRemove){
					//remove Entity from the list (in a secure fashion) and stop Thread
					//int index = Game1.world.Entities.IndexOf(this);
					//Game1.world.entitiesToRemove.Add(this);
					//Game1.world.Threads.RemoveAt(index);
				}
				
				//this.draw(Game1.spriteBatch, Game1.world.EntityTexture);
				
			}

		}

		/**
		 * The running thread method for an entity
		 * <br> Repeatedly update the entity
		 **/
		public void autonomousUpdate(){

			while(!toRemove){

				//Lock everything
				lockSurroundingZonesAndUpdate();
				Thread.Sleep(50);
			}
		}

		/**
		 * Return the current zone of the entity
		 **/
		public Vector2 findCurrentZone(){

			Vector2 posZone = new Vector2(0);
			posZone.X = position.X/zoneWidth;
			posZone.Y = position.Y/zoneHeight;
 
			posZone.X = (float)Math.Truncate((double)posZone.X);
			posZone.Y = (float)Math.Truncate((double)posZone.Y);

			return posZone;
		}


		/**
		 * Lock all the zones surrounding the current zone of the entity
		 **/
		private void lockSurroundingZonesAndUpdate(){
			
			Vector2 findCurrentZone /*= findCurrentZone()*/;
			int zoneX = (int) findCurrentZone.X;
			int zoneY = (int) findCurrentZone.Y;
			
			/*List<List<Object> > zones = Game1.world.lockZones;
			
			int xFirst = Math.Max(0, zoneX - 1);
			int yFirst = Math.Max(0, zoneY - 1);
			int xLast = Math.Min(world.NbZonesPerSide-1, zoneX + 1);
			int yLast = Math.Min(world.NbZonesPerSide-1, zoneY + 1);
			
			for(int x = xFirst; x <= xLast; ++x){

				for(int y = yFirst; y <= yLast; ++y){
					lock(zones[x][y]){

					}
				}
			}*/

			lockXm1Ym1(zoneX, zoneY);
			
		}

		private void lockXm1Ym1(int x, int y){

			List<List<Object> > zones = Game1.world.lockZones;
			
			if(x - 1 >= 0 && y - 1 >= 0){
				lock(zones[x - 1][y - 1]){
					lockXm1Y(x, y);
				}
			} else {
				lockXm1Y(x, y);
			}

		}

		private void lockXm1Y(int x, int y){
			
			List<List<Object> > zones = Game1.world.lockZones;
			
			if(x - 1 >= 0){
				lock(zones[x - 1][y]){
					lockXm1Yp1(x, y);
				}
			} else {
				lockXm1Yp1(x, y);
			}

		}

		private void lockXm1Yp1(int x, int y){

			World world = Game1.world;
			
			List<List<Object> > zones = Game1.world.lockZones;
			
			if(x - 1 >= 0 && y + 1 < world.NbZonesPerSide){
				lock(zones[x - 1][y + 1]){
					lockXYm1(x, y);
				}
			} else {
				lockXYm1(x, y);
			}

		}

		private void lockXYm1(int x, int y){
			
			List<List<Object> > zones = Game1.world.lockZones;
			
			if(y - 1 >= 0){
				lock(zones[x][y - 1]){
					lockXY(x, y);
				}
			} else {
				lockXY(x, y);
			}

		}

		private void lockXY(int x, int y){
			
			List<List<Object> > zones = Game1.world.lockZones;

			lock(zones[x][y]){
				lockXYp1(x, y);
			}

		}

		private void lockXYp1(int x, int y){

			World world = Game1.world;
			
			List<List<Object> > zones = Game1.world.lockZones;
			
			if(y + 1 < world.NbZonesPerSide){
				lock(zones[x][y + 1]){
					lockXp1Ym1(x, y);
				}
			} else {
				lockXp1Ym1(x, y);
			}

		}

		private void lockXp1Ym1(int x, int y){

			World world = Game1.world;
			
			List<List<Object> > zones = Game1.world.lockZones;
			
			if(x + 1 < world.NbZonesPerSide && x - 1 >= 0){
				lock(zones[x + 1][y - 1]){
					lockXp1Y(x, y);
				}
			} else {
				lockXp1Y(x, y);
			}

		}

		private void lockXp1Y(int x, int y){

			World world = Game1.world;
			
			List<List<Object> > zones = Game1.world.lockZones;
			
			if(x + 1 < world.NbZonesPerSide){
				lock(zones[x + 1][y]){
					lockXp1Yp1(x, y);
				}
			} else {
				lockXp1Yp1(x, y);
			}

		}

		private void lockXp1Yp1(int x, int y){

			World world = Game1.world;

			List<List<Object> > zones = Game1.world.lockZones;
			
			if(x + 1 < world.NbZonesPerSide && y + 1 < world.NbZonesPerSide){
				lock(zones[x + 1][y + 1]){
					updateEntity();
				}
			} else {
				updateEntity();
			}

		}


		/**
		 * Tell if the entity is still active or if it already reached the final goal
		 **/
		public bool active(){
			return !toRemove;
		}

		/**
		 * Request the entity to stop moving and cause the thread to finish
		 **/
		public void requestStop(){
			toRemove = true;
		}
	}
}

