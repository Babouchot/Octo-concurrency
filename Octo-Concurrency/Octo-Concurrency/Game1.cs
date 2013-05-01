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

		//Static variables
		public static SpriteBatch spriteBatch;
		public static volatile World world = null;
		public static float currentTime;
		public static bool paused;

		//interface variables
		private bool isDown;
		private bool launched;

		public Game1 ()
		{
			graphics = new GraphicsDeviceManager (this);
			Content.RootDirectory = "Content";	
			graphics.PreferredBackBufferWidth = 600;
			graphics.PreferredBackBufferHeight = 400;
			graphics.PreferMultiSampling = false;
			graphics.IsFullScreen = false;

			graphics.ApplyChanges();

			separatedInitialization();

		}

		/**
		 * Initialize or reinitialize the game
		 **/
		private void separatedInitialization(){

			if(world != null){
				world.stopThreads();
			}
			paused = true;
			world = new World(300, 200, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight, 60);
			currentTime = 0;
			isDown = false;
			launched = false;
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
			Console.WriteLine("Simulation is paused : press 'P' to resume (and to pause again)");
			//world.startThreads();
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent ()
		{
			// Create a new SpriteBatch, which can be used to draw textures.
			spriteBatch = new SpriteBatch (GraphicsDevice);
			Texture2D entityTexture = Content.Load<Texture2D>("entity");
			Texture2D objectiveTexture = Content.Load<Texture2D>("objectif");
			Texture2D obstacleTexture = Content.Load<Texture2D>("obstacle");

			world.loadTextures(entityTexture, obstacleTexture, objectiveTexture);
			//TODO: use this.Content to load your game content here 
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update (GameTime gameTime)
		{

			if(!launched && Keyboard.GetState().IsKeyDown(Keys.L)){
				world.startThreads();
				launched = true;

			}

			if (Keyboard.GetState().IsKeyDown(Keys.Escape)) {
				Exit ();
			}
			//TODO trouver une alternative au IsKeyDown type IsKeyPressed
			if (Keyboard.GetState().IsKeyDown(Keys.P)) {
				isDown = true;
			}
			if(isDown && !Keyboard.GetState().IsKeyDown(Keys.P)) {
				isDown = false;
				paused = !paused;
			}

			// Reset l'état de la simulation
			if(Keyboard.GetState().IsKeyDown(Keys.R)){
				separatedInitialization();
				LoadContent();
			}
			if(!paused){
				currentTime += gameTime.ElapsedGameTime.Milliseconds;
				world.updateWorld(gameTime.ElapsedGameTime.Milliseconds);
				//I moved that one up here hopping it will stop the gameTime incrementation when paused
				//Console.Out.WriteLine("FPS : " + (1.0f/gameTime.ElapsedGameTime.Milliseconds)*1000);
				base.Update (gameTime);
			}

		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw (GameTime gameTime)
		{
			graphics.GraphicsDevice.Clear (Color.White);
			//TODO: Add your drawing code here
			base.Draw (gameTime);
			spriteBatch.Begin();
			world.draw(spriteBatch);

			if (world.Entities.Count == 0) {
				/*spriteBatch.DrawString(new SpriteFont(),"Simulation Successful",
				                       new Vector2(400, 400),
				                       Color.White);*/
			}

			foreach(Entity ent in world.Entities){
				if(ent.active()){
					ent.draw(spriteBatch, world.EntityTexture);
				}
			}
			spriteBatch.End();
		}
	}
}

