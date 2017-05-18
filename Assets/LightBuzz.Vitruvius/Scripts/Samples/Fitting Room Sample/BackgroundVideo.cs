using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class BackgroundVideo : MonoBehaviour
{
    #region Variables and Properties

    public Material videoMaterial;
    public MovieTexture video;
    public bool playOnAwake = true;

    [SerializeField]
    bool loop = true;
#if UNITY_EDITOR
    [HideInInspector, SerializeField]
    bool prevLoop = true;
#endif
    public bool Loop
    {
        get
        {
            return loop;
        }
        set
        {
            if (loop != value)
            {
                loop = value;

#if UNITY_EDITOR
                prevLoop = value;
#endif
                video.loop = value;
            }
        }
    }

    public Transform frameView;

    #endregion

    #region Reserved methods // Awake - Update

    void Awake()
    {
        video.loop = loop;

        if (videoMaterial != null)
        {
            videoMaterial.mainTexture = video;
        }

#if UNITY_EDITOR
        if (Application.isPlaying)
#endif
            if (playOnAwake)
            {
                video.Play();
            }

        Update();
    }

    void Update()
    {
#if UNITY_EDITOR
        if (prevLoop != loop)
        {
            prevLoop = loop;
            video.loop = loop;
        }

        if (frameView == null)
        {
            return;
        }
#endif

        if (frameView.localScale != transform.localScale)
        {
            transform.localScale = frameView.localScale;
        }
    }

    #endregion
}