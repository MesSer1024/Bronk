using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Bronk;

public class BlockDecorators
{
	public struct DecoratorObject
	{
		public DecoratorMaterialBatchData BatchData;
		public int Index;
	}

	struct BlockDecoratorData
	{
		public GameWorld.BlockType BlockType;
		public Color[] PossibleDecoratorColors;
		public string PossibleDecoratorTexturesPath;

		public BlockDecoratorData (GameWorld.BlockType type, Color[] colors, string texturesPath)
		{
			BlockType = type;
			PossibleDecoratorColors = colors;
			PossibleDecoratorTexturesPath = texturesPath;
		}
	}

	struct DecoratorRenderingData
	{
		public DecoratorMaterialBatchData[] PossibleMaterials;
		public Color[] PossibleColors;
	}

	public class BatchMeshData
	{
		public Mesh Mesh;
		public Vector3[] Vertices;
		public int[] Triangles;
		public Color[] Colors;
		public bool Dirty;
	}

	public class DecoratorMaterialBatchData
	{
		public Material Material;
		public Sprite Sprite;
		public List<BatchMeshData> BatchingMeshes;
		public List<int> FreeMeshIndices;
		public int NextMeshIndex;
	}

	const int QUADS_PER_BUFFER = 60;
	private static bool _Initialized = false;
	private static readonly string MaterialPath = "Terrain/Decorators/DecoratorMaterial";
	private static Material _DecoratorMaterial;
	private static BlockDecoratorData[] _DecoratorData = new BlockDecoratorData[] {
		new BlockDecoratorData (GameWorld.BlockType.DirtGround, new Color[] {
			new Color32 (68, 80, 94, 255),
			new Color32 (77, 91, 104, 255),
		}, "Terrain/Decorators/Textures"),
		new BlockDecoratorData (GameWorld.BlockType.Dirt, new Color[] {
			new Color32 (21, 30, 39, 255),
			new Color32 (13, 17, 26, 255),
		}, "Terrain/Decorators/Textures"),
	};
	private static Dictionary<int, DecoratorRenderingData> _MaterialsByBlockType = new Dictionary<int, DecoratorRenderingData> ();
	private static Vector3[] _QuadVertices = new Vector3[4] {
		new Vector3 (-1, 0, -1), new Vector3 (1, 0, -1), new Vector3 (1, 0, 1), new Vector3 (-1, 0, 1)
	};
	private static Vector2[] _QuadUVs = new Vector2[4] {
		new Vector2 (0, 0),
		new Vector2 (0, 1),
		new Vector2 (1, 1),
		new Vector2 (1, 0)
	};
	private static int[] _Triangles = new int[]{ 2, 1, 0, 3, 2, 0 };
	private static Vector2[] _UVBuffer = new Vector2[QUADS_PER_BUFFER * 4];
	private static List<BatchMeshData> _DirtyMeshes = new List<BatchMeshData>();

	public static void Initialize ()
	{
		if (_Initialized == false) {
			_Initialized = true;

			_DecoratorMaterial = Resources.Load<Material> (MaterialPath);

			for (int i = 0; i < _DecoratorData.Length; i++) {
				var data = _DecoratorData [i];
				Object[] textureObjects = Resources.LoadAll (data.PossibleDecoratorTexturesPath, typeof(Texture2D));
				Object[] spriteObjects = Resources.LoadAll (data.PossibleDecoratorTexturesPath, typeof(Sprite));
				Texture2D[] textures = new Texture2D[textureObjects.Length];
				Sprite[] sprites = new Sprite[textureObjects.Length];
				for (int j = 0; j < textureObjects.Length; j++) {
					textures [j] = textureObjects [j] as Texture2D;
				}
				for (int j = 0; j < spriteObjects.Length; j++) {
					sprites [j] = spriteObjects [j] as Sprite;
				}
				DecoratorMaterialBatchData[] materialData = new DecoratorMaterialBatchData[textures.Length];

				for (int textureIndex = 0; textureIndex < textures.Length; textureIndex++) {
					var material = new Material (_DecoratorMaterial);
					material.mainTexture = textures [textureIndex];
					materialData [textureIndex] = new DecoratorMaterialBatchData ();
					materialData [textureIndex].Material = material;
					materialData [textureIndex].Sprite = sprites [textureIndex];
					materialData [textureIndex].BatchingMeshes = new List<BatchMeshData> ();
					materialData [textureIndex].FreeMeshIndices = new List<int> ();
				}
				_MaterialsByBlockType.Add ((int)data.BlockType, new DecoratorRenderingData () {
					PossibleMaterials = materialData,
					PossibleColors = data.PossibleDecoratorColors,
				});
			}
		}
	}

	public static void Update()
	{
		for (int i = 0; i < _DirtyMeshes.Count; i++) {
			UpdateMesh (_DirtyMeshes [i]);
			_DirtyMeshes [i].Dirty = false;
		}
		_DirtyMeshes.Clear ();
	}

	public static DecoratorObject GetDecorator (GameWorld.BlockType type, Vector3 tilePos, float maxSize)
	{
		DecoratorRenderingData renderingData;
		if (_MaterialsByBlockType.TryGetValue ((int)type, out renderingData)) {

			int materialIndex = Random.Range (0, renderingData.PossibleMaterials.Length);
			int colorIndex = Random.Range (0, renderingData.PossibleColors.Length);
			Color color = renderingData.PossibleColors [colorIndex];
			DecoratorMaterialBatchData batchData = renderingData.PossibleMaterials [materialIndex];
			int index; 
			if (batchData.FreeMeshIndices.Count > 0) {
				index = batchData.FreeMeshIndices [batchData.FreeMeshIndices.Count - 1];
				batchData.FreeMeshIndices.RemoveAt (batchData.FreeMeshIndices.Count - 1);
			} else {
				index = batchData.NextMeshIndex;
				batchData.NextMeshIndex++;
			}
			float scale = UnityEngine.Random.Range (0.15f, 1f) * maxSize * 0.5f;
			/*GameObject spriteObj = new GameObject ("sprite", typeof(SpriteRenderer));
			SpriteRenderer spriteRenderer = spriteObj.GetComponent<SpriteRenderer> ();
			spriteRenderer.sprite = batchData.Sprite;
			spriteRenderer.material = batchData.Material;
			spriteRenderer.color = color;
			spriteObj.transform.position = tilePos;
			spriteObj.transform.rotation = Quaternion.Euler (90, 0, 0);
			spriteObj.transform.localScale = Vector3.one * scale;
*/

			int meshObjectIndex = index / QUADS_PER_BUFFER;
			if (batchData.BatchingMeshes.Count <= meshObjectIndex) {
				GameObject obj = new GameObject ("decomesh" + meshObjectIndex, typeof(MeshFilter), typeof(MeshRenderer));
				//obj.hideFlags = HideFlags.HideAndDontSave;
				BatchMeshData meshData = new BatchMeshData ();
				meshData.Mesh = new Mesh ();
				meshData.Mesh.MarkDynamic ();
				meshData.Vertices = new Vector3[QUADS_PER_BUFFER * 4];
				meshData.Colors = new Color[QUADS_PER_BUFFER * 4];
				meshData.Triangles = new int[QUADS_PER_BUFFER * 6];
				for (int i = 0; i < QUADS_PER_BUFFER; i++) {
					for (int j = 0; j < 4; j++) {
						meshData.Vertices [j + i * 4] = Vector3.zero;
						_UVBuffer [j + i * 4] = _QuadUVs [j];
					}
					for (int k = 6 - 1; k >= 0; k--) {
						meshData.Triangles [k + i * 6] = 0;
					}
				}
				meshData.Mesh.vertices = meshData.Vertices;
				meshData.Mesh.uv = _UVBuffer;

				obj.GetComponent<MeshFilter> ().mesh = meshData.Mesh;
				obj.GetComponent<MeshRenderer> ().sharedMaterial = renderingData.PossibleMaterials [materialIndex].Material;
				batchData.BatchingMeshes.Add (meshData);
			}
			int meshIndex = index - meshObjectIndex * QUADS_PER_BUFFER;
			BatchMeshData batchMeshData = batchData.BatchingMeshes [meshObjectIndex];

			for (int j = 0; j < 4; j++) {
				batchMeshData.Vertices [j + meshIndex * 4] = _QuadVertices [j] * scale + tilePos;
				batchMeshData.Colors [j + meshIndex * 4] = color;
			}
			for (int k = 0; k < 6; k++) {
				batchMeshData.Triangles [k + meshIndex * 6] = _Triangles [k] + meshIndex * 4;
			}
			if (batchMeshData.Dirty == false) {
				batchMeshData.Dirty = true;
				_DirtyMeshes.Add (batchMeshData);
			}
			DecoratorObject decoratorObj;
			decoratorObj.BatchData = batchData;
			decoratorObj.Index = index;
			return decoratorObj;
		}
		return default(DecoratorObject);
	}

	static void UpdateMesh (BatchMeshData batchMeshData)
	{
		batchMeshData.Mesh.Clear (true);
		batchMeshData.Mesh.vertices = batchMeshData.Vertices;
		batchMeshData.Mesh.colors = batchMeshData.Colors;
		batchMeshData.Mesh.uv = _UVBuffer;
		batchMeshData.Mesh.triangles = batchMeshData.Triangles;
	}

	public static void FreeDecorator (ref DecoratorObject obj)
	{
		var batchData = obj.BatchData;
		if (batchData != null) {
			int index = obj.Index;	
			batchData.FreeMeshIndices.Add (index);

			int meshObjectIndex = index / QUADS_PER_BUFFER;
			int meshIndex = index - meshObjectIndex * QUADS_PER_BUFFER;
			BatchMeshData batchMeshData = batchData.BatchingMeshes [meshObjectIndex];

			for (int j = 0; j < 4; j++) {
				batchMeshData.Vertices [j + meshIndex * 4] = Vector3.zero;
			}
			for (int k = 0; k < 6; k++) {
				batchMeshData.Triangles [k + meshIndex * 6] = 0;
			}
			UpdateMesh (batchMeshData);
		}
	}
}
