using UnityEngine;
using System.Collections.Generic;

namespace Bronk {
    class GameWorld {
        private const int SIZE_X = 100;
        private const int SIZE_Z = 100;

        public enum BlockType {
            None,
            Unknown,
            Dirt,
            Stone
        }

        public List<CubeData> Cubes { get { return _data; } }
        private List<CubeData> _data;


        public GameWorld() {
            _data = new List<CubeData>(SIZE_X * SIZE_Z);
        }

        public void init() {
            _data.Clear();

            //reset randomizer with a seed (to make sure that no other values are taken prior to this)
            Random.seed = "I am LEEEET! (1337)".GetHashCode();

            for (int i = 0; i < SIZE_X * SIZE_Z; ++i) {
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
    }
}
