using UnityEngine;
using System.Collections.Generic;

namespace Bronk
{
	public class GameWorld
	{
        public WorldGameObject ViewComponent { get; set; }

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

		public List<CubeData> Cubes { get { return _data; } }
        public Rect StartArea { get; private set; }

		private List<CubeData> _data;

		public GameWorld ()
		{
			_data = new List<CubeData> (SIZE_X * SIZE_Z);
		}

		public void init ()
		{
			_data.Clear ();

			Random.seed = "L33T HAXXOR".GetHashCode ();

			int startX;
			int startZ;
			int endX;
			int endZ;

            

			GenerateStartArea (out startX, out startZ, out endX, out endZ);

            StartArea = new Rect(startX, startZ, endX - startX, endZ - startZ);

			Vector2 startAreaPos = new Vector2((startX + endX) / 2, (startZ + endZ) / 2);
			GameCamera gameCam = Camera.mainCamera.GetComponent<GameCamera> ();
			if (gameCam != null)
				gameCam.SetPosition (startAreaPos);



			int lowResSizeX = (SIZE_X / 5);
			int lowResSizeZ = (SIZE_Z / 5);
			float[] lowResValues = new float[lowResSizeX * lowResSizeZ ];
			for (int i = 0; i < lowResValues.Length; i++) {
				int x = i % lowResSizeX;
				int y = i / lowResSizeZ;
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

					float normalizedTileX = lowResX - Mathf.Floor (lowResX);
					float normalizedTileZ = lowResZ = Mathf.Floor (lowResZ);
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

				var cube = new CubeData (i, t);
				_data.Add (cube);
			}


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

        public CubeData getCubeData(int index)
        {
            return _data[index];
        }

		public Vector3 getCubePosition (int index)
		{
			return new Vector3 (index % SIZE_X, 0, (int)(index / SIZE_Z));
		}

        public CubeData getCubeByPosition(Vector3 pos)
        {
            int index = (int)(pos.x) + (int)(pos.z) * SIZE_X;
            if(index >= _data.Count || index < 0) 
                throw new System.Exception("No cube found on " + pos);
            return _data[index];
        }

        public CubeData getRightCube(CubeData node)
        {
            int tarId = node.Index + 1;
            if (tarId % SIZE_X > 0)
            {
                return _data[tarId];
            }
            return null;
        }

        public CubeData getLeftCube(CubeData node)
        {
            var tarId = (node.Index % SIZE_X) - 1;
            if (tarId < 0)
            {
                return null;
            }
            return _data[node.Index - 1];
        }

        public CubeData getTopCube(CubeData node)
        {
            var tarId = node.Index - SIZE_X;
            if (tarId >= 0)
            {
                return _data[tarId];
            }
            return null;
        }

        public CubeData getBottomCube(CubeData node)
        {
            var tarId = node.Index + SIZE_X;
            if (tarId < _data.Count)
            {
                return _data[tarId];
            }
            return null;
        }
	}
}
