﻿using UnityEngine;
using System.Collections.Generic;

namespace Bronk
{
	public class GameWorld
	{
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

		private List<CubeData> _data;

		public GameWorld ()
		{
			_data = new List<CubeData> (SIZE_X * SIZE_Z);
		}

		public void init ()
		{
			_data.Clear ();

			Random.seed = "L33T HAXXOR".GetHashCode ();

			int startPosIndex = Random.Range (0, SIZE_X * SIZE_Z);

			int startAreaSizeX = 8;
			int startAreaSizeZ = 8;

			int startX = startPosIndex % SIZE_X;
			int startZ = startPosIndex / SIZE_Z;
			int endX = Mathf.Clamp (startX + startAreaSizeX, 0, SIZE_X - 1);
			int endZ = Mathf.Clamp (startZ + startAreaSizeZ, 0, SIZE_Z - 1);
			startX += (startAreaSizeX - (endX - startX));
			startZ += (startAreaSizeZ - (endZ - startZ));

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

		public Vector3 getCubePosition (int index)
		{
			return new Vector3 (index % SIZE_X, 0, (int)(index / SIZE_Z));
		}

		public CubeData getCubeData (int index)
		{
			return _data [index];
		}
	}
}
