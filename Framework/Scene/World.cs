using SFML.Graphics;
using System.Collections.Generic;
using SFML.Window;
using Framework;
using System;

namespace Framework
{
	public class World
	{
		#region time

		/// <summary>
		/// Total elapsed time since the scene was created
		/// </summary>
		public int timeMs;

		/// <summary>
		/// Update time delta in milliseconds
		/// </summary>
		public int deltaMs;

		/// <summary>
		/// Update time delta in seconds
		/// </summary>
		public float delta;

		#endregion

		#region globals

		/// <summary>
		/// Tile size in pixels (pixels to world units).
		/// Mainly useful for tile-based games.
		/// </summary>
		public int TS = 1;

		/// <summary>
		/// Main character of the game. 
		/// Can be null if there is no such thing in the scene.
		/// </summary>
		public Entity avatar;

		/// <summary>
		/// Main camera of the game, the one that sees the game world (not the GUI for instance).
		/// (there is always at least one camera in a scene)
		/// </summary>
		public Camera mainCamera;

		public Vector2i spawnPosition;

		#endregion

		// Entities

		private EntityList<Entity> _entities = new EntityList<Entity>();
		public Dictionary<string, HashSet<Entity>> taggedEntities = new Dictionary<string, HashSet<Entity>>();

		// Component systems

		public LightSystem lightSystem;
		public PhysicsSystem physics;
		public RenderSystem graphics;
		public List<Component> behaviours = new List<Component>();

		// Debug

		/// <summary>
		/// Use this to draw lines over the scene for debug purpose.
		/// </summary>
		public LineBatch debugLines;

		public World()
		{
			physics = new PhysicsSystem(this);
			graphics = new RenderSystem(this);
			debugLines = new LineBatch();
		}

		public void ClearEntities()
		{
			_entities.Clear();
		}

		public void Update(int deltaMs)
		{
			// Update time
			timeMs += deltaMs;
			this.deltaMs = deltaMs;
			this.delta = (float)deltaMs / 1000f;

            List<Component> bcopy = new List<Component>();
            foreach(Component cmp in behaviours)
            {
                bcopy.Add(cmp);
            }
			// Update behaviours
            foreach (Component cmp in bcopy)
			{
				cmp.OnUpdate();
			}

			// Update entities
			_entities.UpdateAll();

			// Update bodies
			physics.Update();

			// Update lights
			if(lightSystem != null)
				lightSystem.Update();
		}

		public Entity SpawnEntity()
		{
			return Spawn(new Entity());
		}
       
        public Entity Spawn(Entity e, Vector2f physicalPosition)
		{
			Spawn(e);
			if (e.body != null)
				e.body.position = physicalPosition;
			else if (e.sprite != null)
				e.sprite.position = TS * physicalPosition;
			return e;
		}

		public Entity Spawn(Entity e)
		{
			if(e.world != null)
			{
				Log.Error("Entity already added to the world!");
				return null;
			}

			e.world = this;
			e.OnCreate();
			_entities.Add(e);
			return e;
		}

		public T Spawn<T>() where T:Entity,new()
		{
			T e = new T();
			if (Spawn(e) != null)
				return e;
			else
				return null;
		}

		public void Render(RenderTarget rt)
		{
			graphics.Render(rt);
			debugLines.Display(rt);
			debugLines.Flush();
		}

		public void OnScreenResized(Vector2u newSize)
		{
			graphics.OnScreenResized(newSize);
			if(lightSystem != null)
				lightSystem.OnScreenResized(newSize);
		}

		public void OnEntityTagged(string tag, Entity e)
		{
			HashSet<Entity> tagged = null;
			if(!taggedEntities.TryGetValue(tag, out tagged))
			{
				tagged = new HashSet<Entity>();
				taggedEntities.Add(tag, tagged);
			}
			tagged.Add(e);
		}

		public void OnEntityUntagged(string tag, Entity e)
		{
			taggedEntities[tag].Remove(e);
		}

		public List<Entity> GetTaggedEntities(string tagName)
		{
			IEnumerable<Entity> entities = taggedEntities[tagName];
			List<Entity> lcopy = new List<Entity>();
			lcopy.AddRange(entities);
			return lcopy;
		}

		/// <summary>
		/// Get the first encountered entity having the given tag.
		/// </summary>
		/// <param name="tag"></param>
		/// <returns>One of the entities having the tag, or null if none have it</returns>
		public Entity GetEntityByTag(string tag)
		{
			try
			{
				HashSet<Entity> entities = taggedEntities[tag];
				if(entities.Count > 0)
				{
					return entities.GetEnumerator().Current;
				}
			}
			catch(KeyNotFoundException)
			{
			}

			return null;
		}
	}
}

