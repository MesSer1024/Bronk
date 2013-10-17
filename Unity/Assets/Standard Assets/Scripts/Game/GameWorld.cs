using UnityEngine;
using System.Collections.Generic;

namespace Bronk {
    class GameWorld {
        //  How cubes are stored in memory and how to access them (given a size of x=5,z=4,y=3)
        //    													
        // 	    x	0	1	2	3	4		0	1	2	3	4		0	1	2	3	4
        // z
        // 0	    0	1	2	3	4		20	21	22	23	24		40	41	42	43	44
        // 1	    5	6	7	8	9		25	26	27	28	29		45	46	47	48	49
        // 2	    10	11	12	13	14		30	31	32	33	34		50	51	52	53	54
        // 3	    15	16	17	18	19		35	36	37	38	39		55	56	57	58	59
        // -
        //      y    0	0	0	0	0		1	1	1	1	1		2	2	2	2	2

        private const int SIZE_X = 128;
        private const int SIZE_Y = 64;
        private const int SIZE_Z = 120;
        private const int SIZE_LAYER = SIZE_X * SIZE_Z;

        public enum BlockType {
            None,
            Unknown,
            Dirt,
            Stone
        }

        public List<CubeData> Cubes { get { return _data; } }
        private List<CubeData> _data;


        public GameWorld() {
            _data = new List<CubeData>(SIZE_X * SIZE_Y * SIZE_Z);
        }

        public void init() {
            _data.Clear();

            //reset randomizer with a seed (to make sure that no other values are taken prior to this)
            Random.seed = "I am LEEEET! (1337)".GetHashCode();

            for (int i = 0; i < SIZE_X * SIZE_Y * SIZE_Z; ++i) {
                var rnd = Random.value;
                BlockType t;
                if (rnd < 0.075) {
                    t = BlockType.None;
                } else if (rnd < 0.75) {
                    t = BlockType.Dirt;
                } else if (rnd < 0.96) {
                    t = BlockType.Stone;
                } else {
                    t = BlockType.Unknown;
                }

                var cube = new CubeData(i, t);
                _data.Add(cube);
            }
        }

        public Vector3 getPositionOfCube(int index) {
            return new Vector3(index % SIZE_X, (int)((index % SIZE_LAYER) / SIZE_X), (int)(index / SIZE_LAYER));
        }

        public CubeData getCubeFromPosition(ref Vector3 v) {
            return _data[getCubeIndexFromPosition(ref v)];
        }

        public int getCubeIndexFromPosition(ref Vector3 v) {
            return (int)(v.x) + (int)(v.z) * SIZE_X + (int)(v.y) * SIZE_LAYER;
        }
    }
}
