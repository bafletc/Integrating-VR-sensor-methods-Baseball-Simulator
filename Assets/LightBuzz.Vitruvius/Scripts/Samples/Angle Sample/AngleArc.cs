using UnityEngine;

[ExecuteInEditMode]
public class AngleArc : MonoBehaviour
{
    #region Variables with properties

    [Range(5, 100), SerializeField]
    int quality = 15;
#if UNITY_EDITOR
    int prevQuality = 15;
#endif
    public int Quality
    {
        get
        {
            return quality;
        }
        set
        {
            if (quality != value)
            {
                quality = value;

#if UNITY_EDITOR
                prevQuality = value;
#endif

                RefreshArc();
            }
        }
    }

    [SerializeField]
    float angle = 90;
#if UNITY_EDITOR
    float prevAngle = 90;
#endif
    public float Angle
    {
        get
        {
            return angle;
        }
        set
        {
            if (angle != value)
            {
                angle = value % 360;

#if UNITY_EDITOR
                prevAngle = angle;
#endif

                RefreshArc();
            }
        }
    }

    [SerializeField]
    float minDist = 0;
#if UNITY_EDITOR
    float prevMinDist = 0;
#endif
    public float MinDistance
    {
        get
        {
            return minDist;
        }
        set
        {
            if (minDist != value)
            {
                minDist = value;

#if UNITY_EDITOR
                prevMinDist = value;
#endif

                RefreshArc();
            }
        }
    }

    [SerializeField]
    float maxDist = 0.5f;
#if UNITY_EDITOR
    float prevMaxDist = 0.5f;
#endif
    public float MaxDistance
    {
        get
        {
            return maxDist;
        }
        set
        {
            if (maxDist != value)
            {
                maxDist = value;

#if UNITY_EDITOR
                prevMaxDist = value;
#endif

                RefreshArc();
            }
        }
    }

    #endregion

    #region Variables

    public MeshFilter meshFilter;
    Mesh mesh;

    #endregion

    #region Reserved methods // Awake - OnDestroy - Update(Editor) - Reset

    void Awake()
    {
        if (meshFilter == null)
        {
            meshFilter = GetComponent<MeshFilter>();
        }

        if (meshFilter == null)
            return;

        if (mesh == null)
        {
            mesh = new Mesh();
        }
        else
        {
            mesh.Clear();
        }

        mesh.vertices = new Vector3[4 * quality];
        mesh.triangles = new int[3 * 2 * quality];

        Vector3[] normals = new Vector3[4 * quality];
        Vector2[] uv = new Vector2[4 * quality];

        for (int i = 0; i < uv.Length; i++)
            uv[i] = new Vector2(0, 0);
        for (int i = 0; i < normals.Length; i++)
            normals[i] = new Vector3(0, 0, 1);

        mesh.uv = uv;
        mesh.normals = normals;

        meshFilter.sharedMesh = mesh;
    }

    void OnDestroy()
    {
        if (mesh != null)
        {
#if UNITY_EDITOR
            DestroyImmediate(mesh);
#else
                Destroy(mesh);
#endif
        }
    }

    #region Update(Editor)

#if UNITY_EDITOR

    void Update()
    {
        if (prevQuality != quality)
        {
            prevQuality = quality;

            RefreshArc();
        }

        if (prevAngle != angle)
        {
            angle = angle % 360;
            prevAngle = angle;

            RefreshArc();
        }


        if (prevMinDist != minDist)
        {
            prevMinDist = minDist;

            RefreshArc();
        }

        if (prevMaxDist != maxDist)
        {
            prevMaxDist = maxDist;

            RefreshArc();
        }
    }

#endif

    #endregion

    void Reset()
    {
        Awake();
    }

    #endregion

    #region Refresh arch method

    void RefreshArc()
    {
        if (mesh == null)
            return;

        float angle_delta = -angle / quality;

        float angle_curr = angle;
        float angle_next = angle_delta + angle;

        Vector3 pos_curr_min = Vector3.zero;
        Vector3 pos_curr_max = Vector3.zero;

        Vector3 pos_next_min = Vector3.zero;
        Vector3 pos_next_max = Vector3.zero;

        Vector3[] vertices = new Vector3[4 * quality];
        int[] triangles = new int[3 * 2 * quality];

        for (int i = 0; i < quality; i++)
        {
            Vector3 sphere_curr = new Vector3(
            Mathf.Sin(Mathf.Deg2Rad * (angle_curr)),
            Mathf.Cos(Mathf.Deg2Rad * (angle_curr)),
            0);

            Vector3 sphere_next = new Vector3(
            Mathf.Sin(Mathf.Deg2Rad * (angle_next)),
            Mathf.Cos(Mathf.Deg2Rad * (angle_next)),
            0);

            pos_curr_min = sphere_curr * minDist;
            pos_curr_max = sphere_curr * maxDist;

            pos_next_min = sphere_next * minDist;
            pos_next_max = sphere_next * maxDist;

            int a = 4 * i;
            int b = 4 * i + 1;
            int c = 4 * i + 2;
            int d = 4 * i + 3;

            vertices[a] = pos_curr_min;
            vertices[b] = pos_curr_max;
            vertices[c] = pos_next_max;
            vertices[d] = pos_next_min;

            triangles[6 * i] = a;
            triangles[6 * i + 1] = b;
            triangles[6 * i + 2] = c;
            triangles[6 * i + 3] = c;
            triangles[6 * i + 4] = d;
            triangles[6 * i + 5] = a;

            angle_curr += angle_delta;
            angle_next += angle_delta;
        }

        if (vertices.Length != triangles.Length)
        {
            mesh.Clear();
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
    }

    #endregion

    #region Arch visibility methods

    public void Show()
    {
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }
    }

    public void Hide()
    {
        if (gameObject.activeSelf)
        {
            gameObject.SetActive(false);
        }
    }

    #endregion
}