using UnityEngine;
using System.Collections.Generic;

namespace Bronk
{
	public class GameWorld
	{
        public WorldGameObject ViewComponent { get; set; }
        public StockpileComp StockpileComponent { get; set; }

		public const int SIZE_X = 11;
		public const int SIZE_Z = 11;

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
			GameCamera gameCam = Camera.main.GetComponent<GameCamera> ();
			if (gameCam != null)
				gameCam.SetPosition (startAreaPos);

			var blockArray = new BlockData[SIZE_X * SIZE_Z];

			int lowResSizeX = (SIZE_X / 5);
			int lowResSizeZ = (SIZE_Z / 5);
			float[] lowResValues = new float[lowResSizeX * lowResSizeZ ];
			for (int i = 0; i < lowResValues.Length; i++) {
				lowResValues [i] = Random.Range (0, 1f);
			}

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

					float value1 = lowResValues [(int)lowResX + (int)lowResZ * lowResSizeZ];
					float value2 = lowResValues [indexX + indexZ * lowResSizeZ];
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
				blockArray [i] = block;
			}
			Blocks.init (blockArray, SIZE_X, SIZE_Z);
            StockpileComponent.init();
            PlaceArtifact();
		}

        public void PlaceArtifact()
        {
            Vector2 artifactPosition = StartArea.center + new Vector2(3,0);
            ArtifactObject artifact = new ArtifactObject(Blocks.getBlockIDByPosition(artifactPosition), AIMain.GenerateUniqueId());

            //item.View check required since this message is sent both when selected/deselected and when the block is really changed...
            if (artifact != null && artifact.View == null)
            {
                artifact.View = ViewComponent.InstantiateCarryItemView(artifact);
            }
        }

		static void GenerateStartArea (out int startX, out int startZ, out int endX, out int endZ)
		{
            //int startPosIndex = Random.Range (0, SIZE_X * SIZE_Z);
            //int startAreaSizeX = 8;
            //int startAreaSizeZ = 8;
            //startX = startPosIndex % SIZE_X;
            //startZ = startPosIndex / SIZE_Z;
            //endX = Mathf.Clamp (startX + startAreaSizeX, 0, SIZE_X - 1);
            //endZ = Mathf.Clamp (startZ + startAreaSizeZ, 0, SIZE_Z - 1);
            //startX += (startAreaSizeX - (endX - startX));
            //startZ += (startAreaSizeZ - (endZ - startZ));

            int offsetFromMapBorder = 1;
            int startAreaSize = 8;
            startX = Random.Range(offsetFromMapBorder, SIZE_X - startAreaSize - offsetFromMapBorder - 1);
            startZ = Random.Range(offsetFromMapBorder, SIZE_Z - startAreaSize - offsetFromMapBorder - 1);
            endX = startX + startAreaSize;
            endZ = startZ + startAreaSize;
		}

		public Vector2 getCubePosition (int blockID)
		{
			return Blocks.getBlockPosition (blockID);
		}
	}
}
