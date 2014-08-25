using Framework;
using SFML.Window;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD30
{
    public class LDWorld : World
    {
        public Map map;
        public int dimension = 0;

        public LDWorld() : base()
        {
            map = new Map();
            TS = Game.TS;
        }

        public void SetDimension(int d)
        {
            int deltaIndex = d - dimension;
            int offset = deltaIndex * 8;
            dimension = d;

            Log.Debug("Setting dimension " + d + " (offset: "+ offset+")");

            foreach(TilerPattern pattern in map.mapRenderer.tiler.patterns)
            {
                if (pattern == null)
                    continue;
                if(pattern.tiles != null)
                {
                    for (int i = 0; i < pattern.tiles.Length; ++i)
                    {
                        pattern.tiles[i] += offset;
                    }
                }
                for (int i = 0; i < pattern.defaultOutput.Length; ++i)
                {
                    pattern.defaultOutput[i] += offset;
                }
            }

            IEnumerable<Entity> daemons = taggedEntities["Daemon"];
            foreach(Entity e in daemons)
            {
                ParallelActor a = e.GetComponent<ParallelActor>();
                a.dimension = dimension;
                a.OnDimensionChange(dimension);
            }

            map.mapRenderer.RecalculateTiles(map.cells);

        }

        public Vector2i FindSpawnableZone(System.Predicate<Vector2i> additionnalConditions = null)
        {
            int attempts = 0;
            int maxAttempts = 1000;
            Vector2i pos = new Vector2i();
            for (attempts = 0; attempts < maxAttempts; ++attempts)
            {
                pos = new Vector2i(
                    Random.Range(0, map.cells.sizeX - 1),
                    Random.Range(0, map.cells.sizeY - 1)
                );
                if (map.IsSpawnable(pos.X, pos.Y) && (additionnalConditions!=null? additionnalConditions(pos) : true))
                {
                    break;
                }
            }
            if (attempts == maxAttempts)
            {
                Log.Error("Failed to find spawnable zone! I'm really sorry :'D");
            }
            return pos;
        }
    }
}


