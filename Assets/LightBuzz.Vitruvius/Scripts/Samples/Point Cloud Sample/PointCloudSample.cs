using UnityEngine;
using LightBuzz.Vitruvius;
using Windows.Kinect;

public class PointCloudSample : VitruviusSample
{
    #region Variables

    readonly int DOWNSAMPLE_SIZE = 4;
    readonly int DEPTH_WIDTH = Constants.DEFAULT_DEPTH_WIDTH;
    readonly int DEPTH_HEIGHT = Constants.DEFAULT_DEPTH_HEIGHT;

    ushort[] depthFrameData;
    Mesh pointCloudMesh;
    Vector3[] vertices;
    Vector2[] uv;
    int[] triangles;

    public float meshScale = 1;
    public float depthScale = 0.01f;
    public Transform pointCloudModel;

    #endregion

    #region Reserved methods // Awake - Update

    protected override void Awake()
    {
        base.Awake();

        depthFrameData = new ushort[DEPTH_WIDTH * DEPTH_HEIGHT];

        CreatePointCloudMesh(DEPTH_WIDTH / DOWNSAMPLE_SIZE, DEPTH_HEIGHT / DOWNSAMPLE_SIZE);
    }

    void Update()
    {
        UpdateFrame();
    }

    #endregion

    #region UpdateFrame

    void UpdateFrame()
    {
        if (colorFrameReader != null && depthFrameReader != null)
        {
            using (ColorFrame colorFrame = colorFrameReader.AcquireLatestFrame())
            {
                if (colorFrame != null)
                {
                    frameView.FrameTexture = colorFrame.ToBitmap();
                }
            }

            using (DepthFrame depthFrame = depthFrameReader.AcquireLatestFrame())
            {
                if (depthFrame != null)
                {
                    depthFrame.CopyFrameDataToArray(depthFrameData);

                    ColorSpacePoint[] colorSpace = new ColorSpacePoint[depthFrameData.Length];
                    KinectSensor.CoordinateMapper.MapDepthFrameToColorSpace(depthFrameData, colorSpace);

                    for (int y = 0; y < DEPTH_HEIGHT; y += DOWNSAMPLE_SIZE)
                    {
                        for (int x = 0; x < DEPTH_WIDTH; x += DOWNSAMPLE_SIZE)
                        {
                            int smallIndex = ((y / DOWNSAMPLE_SIZE) * (DEPTH_WIDTH / DOWNSAMPLE_SIZE)) + (x / DOWNSAMPLE_SIZE);

                            vertices[smallIndex].z = Average(x, y, DEPTH_WIDTH, DEPTH_HEIGHT) * depthScale;

                            ColorSpacePoint colorSpacePoint = colorSpace[(y * DEPTH_WIDTH) + x];
                            uv[smallIndex] = new Vector2(colorSpacePoint.X / 1920f, 1f - colorSpacePoint.Y / 1080f);
                        }
                    }

                    pointCloudMesh.vertices = vertices;
                    pointCloudMesh.uv = uv;
                    pointCloudMesh.triangles = triangles;
                    pointCloudMesh.RecalculateNormals();
                }
            }
        }
    }

    #endregion

    #region CreatePointCloudMesh

    void CreatePointCloudMesh(int width, int height)
    {
        pointCloudMesh = new Mesh();
        pointCloudModel.GetComponent<MeshFilter>().mesh = pointCloudMesh;

        Vector2 offset = new Vector2((float)width * meshScale * 0.5f, (float)height * meshScale * 0.5f);

        vertices = new Vector3[width * height];
        uv = new Vector2[width * height];
        triangles = new int[6 * ((width - 1) * (height - 1))];

        int triangleIndex = 0;
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int index = (y * width) + x;

                vertices[index] = new Vector3(x * meshScale - offset.x, height * meshScale - y * meshScale - offset.y, 0);
                uv[index] = new Vector2(((float)x / (float)width), (1f - (float)y / (float)height));

                if (x != (width - 1) && y != (height - 1))
                {
                    int topRight = index + 1;
                    int bottomLeft = index + width;

                    triangles[triangleIndex++] = index;
                    triangles[triangleIndex++] = topRight;
                    triangles[triangleIndex++] = bottomLeft;
                    triangles[triangleIndex++] = bottomLeft;
                    triangles[triangleIndex++] = topRight;
                    triangles[triangleIndex++] = bottomLeft + 1;
                }
            }
        }

        pointCloudMesh.vertices = vertices;
        pointCloudMesh.uv = uv;
        pointCloudMesh.triangles = triangles;
        pointCloudMesh.RecalculateNormals();
    }

    #endregion

    #region Average

    float Average(int x, int y, int width, int height)
    {
        double sum = 0.0;

        for (int y1 = y; y1 < y + 4; y1++)
        {
            for (int x1 = x; x1 < x + 4; x1++)
            {
                int fullIndex = (y1 * width) + x1;
                sum += depthFrameData[fullIndex] == 0 ? 4500d : depthFrameData[fullIndex];

            }
        }

        return (float)sum / 16f;
    }

    #endregion
}