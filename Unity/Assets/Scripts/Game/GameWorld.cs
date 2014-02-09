using UnityEngine;
using System.Collections.Generic;

namespace Bronk
{
	public class GameWorld
	{
        public WorldGameObject ViewComponent { get; set; }
        public StockpileComp StockpileComponent { get; set; }

		public const int SIZE_X = 100;
		public const int SIZE_Z = 100;

		public enum BlockType
		{
			Unknown,
			DirtGround,
			Dirt,
			Stone,
			Food,
			Gold,
		}

		public GameWorldData Blocks { get; private set;}
        public Rect StartArea { get; private set; }


		public GameWorld ()
		{
			Blocks = new GameWorldData ();
		}

		public void init ()
		{

			Random.seed = "L33T HAXXOR".GetHashCode ();

			int startX;
			int startZ;
			int endX;
			int endZ;

            

			GenerateStartArea (out startX, out startZ, out endX, out endZ);

            StartArea = new Rect(startX, startZ, endX - startX, endZ - startZ);

			Vector2 startAreaPos = new Vector2((startX + endX) / 2, (startZ + endZ) / 2);

			var blockArray = new BlockData[SIZE_X * SIZE_Z];

			int lowResSizeX = (SIZE_X / 5);
			int lowResSizeZ = (SIZE_Z / 5);
			float[] lowResValues = new float[lowResSizeX * lowResSizeZ ];
			for (int i = 0; i < lowResValues.Length; i++) {
				lowResValues [i] = Random.Range (0, 1f);
			}
			IntRect discoveredBoundingBox = new IntRect (int.MaxValue, int.MinValue, int.MaxValue, int.MinValue);
			for (int i = 0; i < SIZE_X * SIZE_Z; ++i) {
				int x = i % SIZE_X;
				int z = i / SIZE_Z;

				BlockType t;
				if (x >= startX // Start area
				    && x <= endX
				    && z >= startZ
				    && z <= endZ) {
					t = BlockType.DirtGround;
				} else {

					float normalizedX = (float)x / SIZE_X;
					float normalizedZ = (float)z / SIZE_Z;
					float lowResX = normalizedX * lowResSizeX;
					float lowResZ = normalizedZ * lowResSizeZ;


					float noiseValueX = SimplexNoise.Noise.Generate (x, z) * 1f;
					float noiseValueZ = SimplexNoise.Noise.Generate (z, x) * 1f;
					int indexX = Mathf.Clamp ((int)((lowResZ + noiseValueZ) * lowResSizeZ), 0, lowResSizeX - 1);
					int indexZ = Mathf.Clamp ((int)(lowResX + noiseValueX), 0, lowResSizeZ - 1);

					float value1 = lowResValues [(int)lowResX + (int)lowResZ * lowResSizeX];
					float value2 = lowResValues [indexX + indexZ * lowResSizeX];
					float value = (SimplexNoise.Noise.Generate (x, z) < 0f) ? value1 : value2;
					if (value < 0.075) {
						t = BlockType.DirtGround;
					} else if (value < 0.75) {
						t = BlockType.Dirt;
					} else if (value < 0.90) {
						t = BlockType.Gold;
					} else {
						t = BlockType.Food;
					}
				}
				BlockData block;
				block.Selected = false;
				block.Type = t; 
				block.Discovered = (x >= startX - 1// Start area
					&& x <= endX + 1
					&& z >= startZ - 1
					&& z <= endZ + 1); 
				if (block.Discovered) {
					discoveredBoundingBox.xMin = Mathf.Min (x, discoveredBoundingBox.xMin);
					discoveredBoundingBox.xMax = Mathf.Max (x, discoveredBoundingBox.xMax);
					discoveredBoundingBox.yMin = Mathf.Min (z, discoveredBoundingBox.yMin);
					discoveredBoundingBox.yMax = Mathf.Max(z, discoveredBoundingBox.yMax);
				}
				blockArray [i] = block;
			}
			Blocks.init (blockArray, SIZE_X, SIZE_Z, discoveredBoundingBox);
			StockpileComponent.init();
			GameCamera gameCam = Camera.main.GetComponent<GameCamera> ();
			if (gameCam != null)
				gameCam.SetPosition (startAreaPos);
		}

		static void GenerateStartArea (out int startX, out int startZ, out int endX, out int endZ)
		{
			int startPosIndex = Random.Range (0, SIZE_X * SIZE_Z);
			int startAreaSizeX = 8;
			int startAreaSizeZ = 8;
			startX = startPosIndex % SIZE_X;
			startZ = startPosIndex / SIZE_Z;
			endX = Mathf.Clamp (startX + startAreaSizeX, 0, SIZE_X - 1);
			endZ = Mathf.Clamp (startZ + startAreaSizeZ, 0, SIZE_Z - 1);
			startX += (startAreaSizeX - (endX - startX));
			startZ += (startAreaSizeZ - (endZ - startZ));
		}

		public Vector2 getCubePosition (int blockID)
		{
			return Blocks.getBlockPosition (blockID);
		}
	}
}
