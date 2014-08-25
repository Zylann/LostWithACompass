using SFML.Graphics;
using SFML.Window;
using System.Collections.Generic;

namespace Framework
{
	public class Particle
	{
		public Vector2f position;
		public Vector2f velocity;
		public float time;
		public Color color;
	}

	public class ParticleSimulator
	{
		public int maxParticles = 1000;
		public float particleLifeTime = 1f;
		public float emissionRate = 10;
		public FloatRect spawnArea;
		public Vector2f initialVelocityMin = new Vector2f(-0.5f, 0);
		public Vector2f initialVelocityMax = new Vector2f(-2f, 0);
		public Color initialColor = new Color(255, 255, 255, 255);

		private List<Particle> _particles = new List<Particle>();
		private Stack<Particle> _pool = new Stack<Particle>();

		private float _timeBeforeSpawn;

		public IEnumerable<Particle> particles
		{
			get { return _particles; }
		}

		public void Update(float dt)
		{
			if(emissionRate < 0.01f)
				return;

			float emissionInterval = 1f / emissionRate;
			_timeBeforeSpawn -= dt;
			while(_timeBeforeSpawn <= 0 && _particles.Count < maxParticles)
			{
				_timeBeforeSpawn += emissionInterval;
				Particle p = SpawnParticle();
			}

			for (int i = 0; i < _particles.Count; ++i)
			{
				Particle p = _particles[i];

				UpdateParticle(p, dt);

				if(p.time > particleLifeTime)
				{
					// Kill particle

					// Return it to pool
					_pool.Push(p);

					// Swap last with current and remove last
					int last = _particles.Count - 1;
					_particles[i] = _particles[last];
					_particles.RemoveAt(last);
					--i;
				}
			}
		}

		private void UpdateParticle(Particle p, float dt)
		{
			p.position += p.velocity * dt;
			p.time += dt;
			float k = p.time / particleLifeTime;
			p.color.A = (byte)(255f * (1f - Mathf.Sq(2f * k - 1f)));
		}

		private Particle SpawnParticle()
		{
			Particle p = CreateParticle();

			p.time = 0;

			p.position = new Vector2f(
				Random.Range(spawnArea.Left, spawnArea.Left + spawnArea.Width),
				Random.Range(spawnArea.Top, spawnArea.Top + spawnArea.Height)
			);

			p.velocity = new Vector2f(
				Random.Range(initialVelocityMin.X, initialVelocityMax.X),
				Random.Range(initialVelocityMin.Y, initialVelocityMax.Y)
			);

			p.color = initialColor;

			_particles.Add(p);

			return p;
		}

		private Particle CreateParticle()
		{
			Particle p = null;
			if(_pool.Count > 0)
			{
				p = _pool.Pop();
			}
			else
			{
				p = new Particle();
			}
			return p;
		}

	}
}

