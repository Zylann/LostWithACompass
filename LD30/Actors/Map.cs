using Framework;

namespace LD30
{
    public class Map : Behaviour
    {
        public Array2D<int> cells;
        public MapRenderer mapRenderer;
        public MapCollider mapCollider;

        public override void OnCreate()
        {
            base.OnCreate();

            entity.AddTag("Map");

            cells = new Array2D<int>(128,128);

            mapCollider = entity.AddComponent<MapCollider>();
            mapCollider.cell2CollisionValue = new int[Cell.COUNT];
            mapCollider.cell2CollisionValue[Cell.VOID] = 1;
            mapCollider.cell2CollisionValue[Cell.GROUND] = 0;
            mapCollider.cell2CollisionValue[Cell.WALL] = 1;
            mapCollider.cell2CollisionValue[Cell.TRUNK] = 1;

            mapRenderer = entity.AddComponent<MapRenderer>();
            mapRenderer
                .SetTexture("terrain", RenderMode.BASE)
                .SetTexture("terrain_light_filter", RenderMode.LIGHT_FILTER)
                .SetTexture("black", RenderMode.LIGHT_MAP)
                .SetLayer(ViewLayers.TERRAIN);

            Tiler tiler = mapRenderer.tiler;
            tiler.AddPattern(new TilerPattern(Cell.GROUND, 0));
            tiler.AddPattern(new TilerPattern(Cell.WALL, 1));
            tiler.AddPattern(new TilerPattern(Cell.TRUNK, 2));

        }

        public bool IsInInnerRadius(int x, int y)
        {
            int r = cells.sizeX / 2 - 1;
            r = (r * 2) / 3; // Assume it's approximately 2/3 of the full radius
            int centerX = cells.sizeX / 2;
            int centerY = cells.sizeY / 2;
            float dsq = Mathf.Sq(x - centerX) + Mathf.Sq(y - centerY);
            return dsq < r * r;
        }

        public bool IsSpawnable(int x, int y)
        {
            return IsInInnerRadius(x, y) && cells[x, y] == Cell.GROUND;
        }

        public void Generate()
        {
            cells.Fill(Cell.GROUND);
            int r = cells.sizeX/2-1;
            int centerX = cells.sizeX / 2;
            int centerY = cells.sizeY / 2;

            for (int y = 0; y < cells.sizeY; ++y)
            {
                for (int x = 0; x < cells.sizeX; ++x)
                {
                    float dsq = Mathf.Sq(x - centerX) + Mathf.Sq(y - centerY);
                    float k = (float)dsq / (float)(r*r);
                    float trunkChance = 0.1f + k * k * k * k;
                    if (Random.Chance(trunkChance))
                    {
                        cells[x, y] = Cell.TRUNK;
                    }
                }
            }

            mapRenderer.RecalculateTiles(cells);
            mapCollider.RecalculateCollisionMap(cells);
        }
    }
}


