using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Bronk;

public class BlockDecorators
{
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
		public Material[] PossibleMaterials;
		public Mesh[] PossibleMeshes;
	}

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
	private static Dictionary<GameWorld.BlockType, DecoratorRenderingData> _MaterialsByBlockType = new Dictionary<GameWorld.BlockType, DecoratorRenderingData> ();

	public static void Initialize ()
	{
		if (_Initialized == false) {
			_Initialized = true;
			Vector3[] vertices = new Vector3[4] {
				new Vector3 (-1, 0, -1), new Vector3 (1, 0, -1), new Vector3 (1, 0, 1), new Vector3 (-1, 0, 1)
			};
			Vector2[] uvs = new Vector2[4]{ new Vector2 (0, 0), new Vector2 (0, 1), new Vector2 (1, 1), new Vector2 (1, 0) };
			int[] triangles = new int[]{ 2, 1, 0, 3, 2, 0 }; 
			Color[] colorsBuffer = new Color[4];

			_DecoratorMaterial = Resources.Load<Material> (MaterialPath);

			for (int i = 0; i < _DecoratorData.Length; i++) {
				var data = _DecoratorData [i];
				Object[] textureObjects = Resources.LoadAll (data.PossibleDecoratorTexturesPath, typeof(Texture2D));
				Texture2D[] textures = new Texture2D[textureObjects.Length];
				for (int j = 0; j < textureObjects.Length; j++) {
					textures [j] = textureObjects [j] as Texture2D;
				}
				Material[] materials = new Material[textures.Length];

				for (int textureIndex = 0; textureIndex < textures.Length; textureIndex++) {
					var material = new Material (_DecoratorMaterial);
					material.mainTexture = textures [textureIndex];
					materials [textureIndex] = material;
				}
				Mesh[] meshes = new Mesh[data.PossibleDecoratorColors.Length];
				for (int colorIndex = 0; colorIndex < meshes.Length; colorIndex++) {
					for (int j = 0; j < colorsBuffer.Length; j++) {
						colorsBuffer [j] = data.PossibleDecoratorColors [colorIndex];
					}

					var mesh = new Mesh (); 
					mesh.vertices = vertices;
					mesh.triangles = triangles;
					mesh.uv = uvs;
					mesh.colors = colorsBuffer;
					mesh.RecalculateNormals ();
					meshes [colorIndex] = mesh;
				}

				_MaterialsByBlockType.Add (data.BlockType, new DecoratorRenderingData () {
					PossibleMeshes = meshes,
					PossibleMaterials = materials,
				});
			}
		}
	}

	public static GameObject GetDecorator (GameWorld.BlockType type, Vector3 tilePos, float maxSize)
	{
		DecoratorRenderingData renderingData;
		if (_MaterialsByBlockType.TryGetValue (type, out renderingData)) {

			float scale = UnityEngine.Random.Range (0.15f, 1f);
			GameObject obj = new GameObject (string.Empty, typeof(MeshFilter), typeof(MeshRenderer));
			obj.hideFlags = HideFlags.HideAndDontSave;
			obj.transform.position = tilePos;
			obj.transform.localScale = maxSize * Vector3.one * scale;
			obj.transform.rotation = Quaternion.Euler (0, Random.Range (0, 360), 0);
			int meshIndex = Random.Range (0, renderingData.PossibleMeshes.Length);
			int materialIndex = Random.Range (0, renderingData.PossibleMaterials.Length);
			obj.GetComponent<MeshFilter> ().mesh = renderingData.PossibleMeshes [meshIndex];
			obj.GetComponent<MeshRenderer> ().sharedMaterial = renderingData.PossibleMaterials [materialIndex];
			return obj;
		}
		return null;
	}
}
