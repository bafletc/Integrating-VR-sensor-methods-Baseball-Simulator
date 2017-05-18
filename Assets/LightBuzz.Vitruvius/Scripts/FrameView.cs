using UnityEngine;
using System.Collections;
using LightBuzz.Vitruvius;

[ExecuteInEditMode]
public class FrameView : MonoBehaviour
{
    #region Constants

    const float COLOR_X_RATIO = 0.5625f;
    const float COLOR_Y_RATIO = 1.777778f;

    #endregion

    #region Variables and Properties

    int currentScreenWidth;
    int currentScreenHeight;

    [SerializeField]
    bool mirroredView = true;
#if UNITY_EDITOR
    [HideInInspector, SerializeField]
    bool prevMirroredView = true;
#endif
    public bool MirroredView
    {
        get
        {
            return mirroredView;
        }
        set
        {
            if (mirroredView != value)
            {
                mirroredView = value;

#if UNITY_EDITOR
                prevMirroredView = value;
#endif

                RefreshMirrorView();
                RefreshView();
            }
        }
    }

    [SerializeField]
    float viewScale = 1;
#if UNITY_EDITOR
    [HideInInspector, SerializeField]
    float prevViewScale = 1;
#endif
    public float ViewScale
    {
        get
        {
            return viewScale;
        }
        set
        {
            if (viewScale != value)
            {
                viewScale = value;

#if UNITY_EDITOR
                prevViewScale = viewScale;
#endif

                RefreshView();
            }
        }
    }
    
#if UNITY_EDITOR
    [HideInInspector, SerializeField]
    float prevXRatio = 0;
#endif
    public float XRatio
    {
        get;
        private set;
    }

#if UNITY_EDITOR
    [HideInInspector, SerializeField]
    float prevYRatio = 0;
#endif
    public float YRatio
    {
        get;
        private set;
    }

    [SerializeField]
#if UNITY_EDITOR
    new
#endif
    Renderer renderer;

    Material material;
    public Material Material
    {
        get
        {
            return material;
        }
        set
        {
            if (material != value)
            {
                material = value;

                RefreshView();
            }
        }
    }

    [SerializeField]
    bool keepScreenRatio = true;
#if UNITY_EDITOR
    bool prevKeepScreenRatio = true;
#endif
    public bool KeepScreenRatio
    {
        get
        {
            return keepScreenRatio;
        }
        set
        {
            if (keepScreenRatio != value)
            {
                keepScreenRatio = value;

#if UNITY_EDITOR
                prevKeepScreenRatio = keepScreenRatio;
#endif

                if (!keepScreenRatio)
                {
                    TextureRatioLessThanScreen = false;
                }

                RefreshView();
            }
        }
    }
    public float ScreenRatio
    {
        get;
        private set;
    }

    public bool TextureRatioLessThanScreen
    {
        get;
        private set;
    }

    public Texture2D FrameTexture
    {
        get
        {
            return (Texture2D)Material.mainTexture;
        }
        set
        {
            if (value == null && Material.mainTexture != null)
            {
                Destroy(Material.mainTexture);
            }

            Material.mainTexture = value;

            RefreshMirrorView();

            if (value != null)
            {
                float xRatio = (float)value.height / (float)value.width;
                float yRatio = (float)value.width / (float)value.height;

                bool changed = xRatio != XRatio || yRatio != YRatio;

                XRatio = xRatio;
                YRatio = yRatio;

                if (changed)
                {
                    RefreshView();
                }
            }
            else
            {
                if (XRatio != COLOR_X_RATIO)
                {
                    XRatio = COLOR_X_RATIO;
                    YRatio = COLOR_Y_RATIO;

                    RefreshView();
                }
            }
        }
    }

    #endregion

    #region Reserved methods // Awake - OnDestroy - Update

    void Awake()
    {
        if (material == null)
        {
            material = renderer.sharedMaterial;
            material.SetTextureScale("_MainTex", new Vector2(mirroredView ? 1 : -1, -1));
        }

        RefreshView();
    }

    void OnDestroy()
    {
        if (Material != null)
        {
            Material.mainTextureOffset = new Vector2(0, 0);
            FrameTexture = null;
        }
    }
    
    void Update()
    {
        bool refreshView = false;

        if (Screen.width != currentScreenWidth || Screen.height != currentScreenHeight)
        {
            currentScreenWidth = Screen.width;
            currentScreenHeight = Screen.height;

            refreshView = true;
        }

#if UNITY_EDITOR

        if (prevViewScale != viewScale)
        {
            prevViewScale = viewScale;
            refreshView = true;
        }

        if (prevXRatio != XRatio)
        {
            prevXRatio = XRatio;
            refreshView = true;
        }

        if (prevYRatio != YRatio)
        {
            prevYRatio = YRatio;
            refreshView = true;
        }

        if (prevKeepScreenRatio != keepScreenRatio)
        {
            prevKeepScreenRatio = keepScreenRatio;
            refreshView = true;
        }

        if (prevMirroredView != mirroredView)
        {
            prevMirroredView = mirroredView;
            RefreshMirrorView();
            refreshView = true;
        }
#endif

        if (refreshView)
        {
            RefreshView();
        }
    }

    #endregion

    #region RefreshView

    public void RefreshView()
    {
        if (!float.IsNaN(viewScale) && !float.IsNaN(XRatio) && !float.IsNaN(YRatio))
        {
            if (keepScreenRatio)
            {
                float screenRatio = (float)Screen.width / (float)Screen.height;

                ScreenRatio = screenRatio / YRatio;

                TextureRatioLessThanScreen = screenRatio > YRatio;

                if (TextureRatioLessThanScreen)
                {
                    if (YRatio == 0)
                    {
                        return;
                    }

                    transform.localScale = new Vector3(viewScale * screenRatio, viewScale * (screenRatio / YRatio), 1);
                }
                else
                {
                    transform.localScale = new Vector3(viewScale * YRatio, viewScale, 1);
                }
            }
            else
            {
                transform.localScale = new Vector3(viewScale * YRatio, viewScale, 1);
            }
        }
    }

    #endregion

    #region RefreshMirrorView

    void RefreshMirrorView()
    {
        Material.mainTextureScale = new Vector2(mirroredView ? 1 : -1, -1);

        float offsetX = Mathf.Abs(Material.mainTextureOffset.x);
        Material.mainTextureOffset = new Vector2(mirroredView ? -offsetX : offsetX, 0);
    }

    #endregion

    #region SetPositionOnFrame

    public void SetPositionOnFrame(ref Vector2 position)
    {
        if (FrameTexture == null)
        {
            return;
        }

        Vector2 textureSize = new Vector2(FrameTexture.width, FrameTexture.height);

        if (textureSize.x != Constants.DEFAULT_DEPTH_WIDTH)
        {
            textureSize.Set(Constants.DEFAULT_COLOR_WIDTH, Constants.DEFAULT_COLOR_HEIGHT);
        }

        Vector2 frameScale = transform.localScale;
        Vector2 framePosition = (Vector2)transform.position + new Vector2(
            Material.mainTextureOffset.x * -frameScale.x,
            Material.mainTextureOffset.y * frameScale.y);

        position.Set(
            (position.x / textureSize.x * frameScale.x - frameScale.x * 0.5f + framePosition.x) * Material.mainTextureScale.x,
            (position.y / -textureSize.y * frameScale.y + frameScale.y * 0.5f + framePosition.y) * -Material.mainTextureScale.y);
    }

    public void SetPositionOnFrame(ref Vector3 position)
    {
        if (FrameTexture == null)
        {
            return;
        }

        Vector2 textureSize = new Vector2(FrameTexture.width, FrameTexture.height);

        if (textureSize.x != Constants.DEFAULT_DEPTH_WIDTH)
        {
            textureSize.Set(Constants.DEFAULT_COLOR_WIDTH, Constants.DEFAULT_COLOR_HEIGHT);
        }

        Vector2 frameScale = transform.localScale;
        Vector2 framePosition = (Vector2)transform.position + new Vector2(
            Material.mainTextureOffset.x * -frameScale.x,
            Material.mainTextureOffset.y * frameScale.y);

        position.Set(
            (position.x / textureSize.x * frameScale.x - frameScale.x * 0.5f + framePosition.x) * Material.mainTextureScale.x,
            (position.y / -textureSize.y * frameScale.y + frameScale.y * 0.5f + framePosition.y) * -Material.mainTextureScale.y,
            0);
    }

    #endregion

    #region SetOffset

    public void SetOffset(Vector2 offset)
    {
        Material.mainTextureOffset = offset;
    }

    #endregion

    #region SetMaterialFullscreen

    public void SetMaterialFullscreen(bool fullscreen)
    {
        if (Material != null)
        {
            if (Material.HasProperty("_Rect"))
            {
                Vector4 rect = Material.GetVector("_Rect");

                rect.x = fullscreen ? -rect.y : 0;

                Material.SetVector("_Rect", rect);
            }
        }
    }

    #endregion
}