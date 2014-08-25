using Framework;
using System.Collections.Generic;

namespace Framework
{
	public class Entity
	{
		public World world; // Reference to the world
		public string name;
        public bool destroyLaterFlag = false;

		#region shortcuts
		public BasicBody body; // Physical representation (includes position and bounds)
		public AudioEmitter audio;
		public SpriteRenderer sprite; // Main sprite
		public Camera camera;
		#endregion

		private bool _destroyed;
		private List<Component> _components = new List<Component>();
		private HashSet<string> _tags = new HashSet<string>();

		public T AddComponent<T>() where T : Component, new()
		{
			T cmp = new T();

			// Shortcuts

			if (this.audio == null && cmp is AudioEmitter)
				this.audio = (AudioEmitter)(Component)cmp;
            
            if (this.body == null && cmp is BasicBody)
				this.body = (BasicBody)(Component)cmp;

            if (this.sprite == null && cmp is SpriteRenderer)
				this.sprite = (SpriteRenderer)(Component)cmp;

            if (this.camera == null && cmp is Camera)
				this.camera = (Camera)(Component)cmp;

			cmp.entity = this;
			cmp.OnCreate();

			_components.Add(cmp);

			return cmp;
		}

		public T GetComponent<T>() where T : Component
		{
			foreach(Component cmp in _components)
			{
				if(cmp is T)
				{
					return (T)cmp;
				}
			}
			return null;
		}

        public IEnumerable<Component> components
        {
            get { return _components; }
        }

		public Entity AddTag(string tag)
		{
			if(_tags.Add(tag))
			{
				world.OnEntityTagged(tag, this);
			}
			return this;
		}

		public bool HasTag(string tag)
		{
			return _tags.Contains(tag);
		}

		public void RemoveTag(string tag)
		{
			if(_tags.Remove(tag))
			{
				world.OnEntityUntagged(tag, this);
			}
		}

		public bool Destroyed
		{
			get { return _destroyed; }
		}

        public void DestroyLater()
        {
            if(!destroyLaterFlag)
            {
                destroyLaterFlag = true;
            }
        }

		public void DestroyNow()
		{
			if (!_destroyed)
			{
				OnDestroy();
                HashSet<string> tagsCopy = new HashSet<string>();
                foreach (string t in _tags)
                    tagsCopy.Add(t);
				foreach(string tag in tagsCopy)
				{
					RemoveTag(tag);
				}
				foreach(Component cmp in _components)
				{
					cmp.OnDestroy();
				}
			}
			_components.Clear();
			_destroyed = true;
		}

		public void Update()
		{
			OnUpdate();

			if (sprite != null)
			{
				if (body != null)
				{
					sprite.position = body.pixelPosition;
				}
			}
		}

		public virtual void OnCreate() { }

		protected virtual void OnUpdate() { }

		protected virtual void OnDestroy() { }
	}
}

