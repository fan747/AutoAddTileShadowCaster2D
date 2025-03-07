using System.Linq;
using System.Numerics;
using System.Reflection;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using static UnityEngine.UI.Image;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;


#if UNITY_EDITOR

[RequireComponent(typeof(CompositeCollider2D))]
public class ShadowCaster2DCreator : MonoBehaviour
{
	[SerializeField]
	private bool _selfShadows = true;

    [SerializeField] private float _extrudeDistance;

	private CompositeCollider2D tilemapCollider;

	static readonly FieldInfo meshField = typeof(ShadowCaster2D).GetField("m_Mesh", BindingFlags.NonPublic | BindingFlags.Instance);
	static readonly FieldInfo shapePathField = typeof(ShadowCaster2D).GetField("m_ShapePath", BindingFlags.NonPublic | BindingFlags.Instance);
	static readonly FieldInfo shapePathHashField = typeof(ShadowCaster2D).GetField("m_ShapePathHash", BindingFlags.NonPublic | BindingFlags.Instance);
	static readonly MethodInfo generateShadowMeshMethod = typeof(ShadowCaster2D)
									.Assembly
									.GetType("UnityEngine.Rendering.Universal.ShadowUtility")
									.GetMethod("GenerateShadowMesh", BindingFlags.Public | BindingFlags.Static);

	public void Create()
	{
		DestroyOldShadowCasters();
		tilemapCollider = GetComponent<CompositeCollider2D>();

		for (int i = 0; i < tilemapCollider.pathCount; i++)
		{
			Vector2[] pathVertices = new Vector2[tilemapCollider.GetPathPointCount(i)];
			tilemapCollider.GetPath(i, pathVertices);

            pathVertices = OffsetVerticesInsert(pathVertices);

            GameObject shadowCaster = new GameObject("shadow_caster_" + i);
			shadowCaster.transform.parent = gameObject.transform;
			shadowCaster.transform.localPosition = Vector3.zero;
			ShadowCaster2D shadowCasterComponent = shadowCaster.AddComponent<ShadowCaster2D>();
			shadowCasterComponent.selfShadows = this._selfShadows;

			Vector3[] testPath = new Vector3[pathVertices.Length];
			for (int j = 0; j < pathVertices.Length; j++)
			{
				testPath[j] = pathVertices[j];
            }

			shapePathField.SetValue(shadowCasterComponent, testPath);
			shapePathHashField.SetValue(shadowCasterComponent, Random.Range(int.MinValue, int.MaxValue));
			meshField.SetValue(shadowCasterComponent, new Mesh());
			generateShadowMeshMethod.Invoke(shadowCasterComponent,
			new object[] { meshField.GetValue(shadowCasterComponent), shapePathField.GetValue(shadowCasterComponent) });
		}
	}

    private Vector2[] OffsetVerticesInsert(Vector2[] vertices)
    {
        if (_extrudeDistance == 0) return vertices;

        Vector2[] insertedVertices = new Vector2[vertices.Length];

        for (int i = 0; i < vertices.Length; i++)
        {
            int prevIndex = (i == 0) ? vertices.Length - 1 : i - 1;
            int nextIndex = (i + 1) % vertices.Length;

            Vector2 normal = ComputeEdgeNormal(
                vertices[prevIndex],
                vertices[i],
                vertices[nextIndex]
            );

            insertedVertices[i] = vertices[i] - normal * _extrudeDistance;
        }

        return insertedVertices;
    }

    public Vector2 ComputeEdgeNormal(Vector2 previousVertex, Vector2 currentVertex, Vector2 nextVertex)
    {
        Vector2 edge1 = currentVertex - previousVertex;
        Vector2 edge2 = nextVertex - currentVertex;

        Vector2 normal1 = new Vector2(edge1.y, -edge1.x).normalized;
        Vector2 normal2 = new Vector2(edge2.y, -edge2.x).normalized;

        Vector2 averageNormal = (normal1 + normal2).normalized;

        return averageNormal;
    }
    private bool IsOnZero(float x) => x > -0.1 && x < 0.1;

    private Vector2 ComputeCenter(Vector2[] pathVertices)
    {
		Vector2 sum = Vector2.zero;
        foreach (var pathVertex in pathVertices)
        {
            sum += pathVertex;
        }
		return sum / pathVertices.Length;
    }

	public void DestroyOldShadowCasters()
	{
        var tempList = transform.Cast<Transform>().ToList();
		foreach (var child in tempList)
		{
			DestroyImmediate(child.gameObject);
		}
	}
}

[CustomEditor(typeof(ShadowCaster2DCreator))]
public class ShadowCaster2DTileMapEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("Create"))
		{
			var creator = (ShadowCaster2DCreator)target;
			creator.Create();
		}

		if (GUILayout.Button("Remove Shadows"))
		{
			var creator = (ShadowCaster2DCreator)target;
			creator.DestroyOldShadowCasters();
		}
		EditorGUILayout.EndHorizontal();
	}

}

#endif
