#region Using Statements
using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Input;

#endregion


namespace OctoConcurrency
{
	/// <summary>
	/// This is the main type for your game
	/// </summary>
	public class Game1 : Game
	{
		GraphicsDeviceManager graphics;

		//Simulation variables
		static List<Entity> entities;
		static List<Obstacle> obstacles;
		Vector2 objective;
		const int XOBJ = 400;
		const int YOBJ = 400;

		//Drawing variables
		SpriteBatch spriteBatch;
		Texture2D entityTexture;
		Texture2D objectiveTexture;

		//interface variables
		private bool paused;

		public Game1 ()
		{
			graphics = new GraphicsDeviceManager (this);
			Content.RootDirectory = "Content";	            
			graphics.IsFullScreen = true;
			objective = new Vector2(XOBJ, YOBJ);
			entities = new List<Entity>();
			obstacles = new List<Obstacle>();
			paused = true;
		}

		public static List<Entity> Entities {
			get { return entities; }
		}

		public static List<Obstacle> Obstacles {
			get { return obstacles; }
		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize ()
		{
			// TODO: Add your initialization logic here
			base.Initialize ();
			for (int i = 0; i < 10; ++i) {
				for (int j = 0; j < 10; ++j)
					entities.Add(new Entity(new Vector2(i*entityTexture.Width, 
					                                    j*entityTexture.Height), 
					                        objective,
					                        entityTexture.Width,
					                        entityTexture.Height));
			}
			Console.WriteLine("Simulation is paused : press 'P' to resume (and to pause again)");
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent ()
		{
			// Create a new SpriteBatch, which can be used to draw textures.
			spriteBatch = new SpriteBatch (GraphicsDevice);
			entityTexture = Content.Load<Texture2D>("entity");
			objectiveTexture = Content.Load<Texture2D>("objectif");
			//TODO: use this.Content to load your game content here 
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update (GameTime gameTime)
		{
			if (Keyboard.GetState().IsKeyDown(Keys.Escape)) {
				Exit ();
			}
			//TODO trouver une alternative au IsKeyDown type IsKeyPressed
			if (Keyboard.GetState().IsKeyDown(Keys.P)) {
				paused = !paused;
			}

			//if (!paused) {
				if (entities.Count == 0) {
					Console.WriteLine("Simulation ended successfully");
					Exit();
				}

				List<int> toRemove = new List<int>();
				foreach (Entity e in entities) {
					if (e.Reach)
						toRemove.Add(entities.IndexOf(e));
					else
						e.Populate();
				}
				int offset = 0;
				foreach (int i in toRemove) {
					entities.RemoveAt(i-offset++);
				}
				Console.WriteLine("nb entity : " + entities.Count);
			//}

			// TODO: Add your update logic here			
			base.Update (gameTime);
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw (GameTime gameTime)
		{
			graphics.GraphicsDevice.Clear (Color.Black);
			//TODO: Add your drawing code here
			base.Draw (gameTime);
			spriteBatch.Begin();
			/*if (entities.Count == 0) {
				spriteBatch.DrawString(new SpriteFont(),"Simulation Successful",
				                       new Vector2(400, 400),
				                       Color.White);
			}*/
			spriteBatch.Draw(objectiveTexture, objective, Color.White);
			foreach (Entity e in entities) {
				spriteBatch.Draw(entityTexture, e.Position, Color.White);
			}
			spriteBatch.End();
		}
	}
}

