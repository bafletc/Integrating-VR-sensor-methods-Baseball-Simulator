using UnityEngine;
using LightBuzz.Vitruvius;
using Windows.Kinect;

public class PaperPlanesSample : VitruviusSample
{
    #region Variables

    Body body;
    
    public FrameView cutoutFrameView;

    byte[] cutoutImagePixels;
    byte[] colorImagePixels;
    Texture2D cutoutTexture;
    Texture2D colorTexture;

    public int minMaxPixelOffset = 10;
    int beginX = 0;
    int endX = 0;

    #endregion

    #region Reserved methods // Awake - Start - Update

    protected override void Awake()
    {
        base.Awake();
        
        cutoutTexture = new Texture2D(Constants.DEFAULT_COLOR_WIDTH, Constants.DEFAULT_COLOR_HEIGHT, TextureFormat.RGBA32, false);
        colorTexture = new Texture2D(Constants.DEFAULT_COLOR_WIDTH, Constants.DEFAULT_COLOR_HEIGHT, TextureFormat.RGBA32, false);
    }

    void Start()
    {
        frameView.FrameTexture = colorTexture;
        cutoutFrameView.FrameTexture = cutoutTexture;
    }

    void Update()
    {
        UpdateFrame();
    }

    #endregion

    #region UpdateFrame

    void UpdateFrame()
    {
        UpdateBodyFrame();

        if (colorFrameReader != null && depthFrameReader != null && bodyIndexFrameReader != null)
        {
            using (ColorFrame colorFrame = colorFrameReader.AcquireLatestFrame())
            using (DepthFrame depthFrame = depthFrameReader.AcquireLatestFrame())
            using (BodyIndexFrame bodyIndexFrame = bodyIndexFrameReader.AcquireLatestFrame())
            {
                if (colorFrame != null && depthFrame != null && bodyIndexFrame != null)
                {
                    colorFrame.GreenScreenHD(depthFrame, bodyIndexFrame, out cutoutImagePixels, out colorImagePixels, beginX, endX);

                    cutoutTexture.LoadRawTextureData(cutoutImagePixels);
                    cutoutTexture.Apply();

                    colorTexture.LoadRawTextureData(colorImagePixels);
                    colorTexture.Apply();
                }
            }
        }
    }

    #endregion

    #region OnBodyFrameReceived

    protected override void OnBodyFrameReceived(BodyFrame frame)
    {
        body = frame.Bodies().Closest();

        GetMinMaxXFromJoint(out beginX, out endX);
    }

    #endregion

    #region GetMinMaxXFromJoint

    void GetMinMaxXFromJoint(out int minX, out int maxX)
    {
        minX = int.MaxValue;
        maxX = int.MinValue;

        for (int i = 0, count = (int)JointType.ThumbLeft; i < count; i++)
        {
            int x = (int)body.Joints[(JointType)i].Position.ToPoint(Visualization.Color, KinectSensor.CoordinateMapper).x;

            if (x > maxX)
            {
                maxX = x;
            }

            if (x < minX)
            {
                minX = x;
            }
        }

        minX = Mathf.Clamp(minX - minMaxPixelOffset, 0, Constants.DEFAULT_COLOR_WIDTH);
        maxX = Mathf.Clamp(maxX + minMaxPixelOffset + 1, 0, Constants.DEFAULT_COLOR_WIDTH);
    }

    #endregion
}