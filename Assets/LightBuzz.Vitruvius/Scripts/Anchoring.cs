using UnityEngine;

namespace LightBuzz.Vitruvius
{
    #region Enums

    public enum AnchorAlignment { TopLeft, TopCenter, TopRight, LeftCenter, Center, RightCenter, BottomLeft, BottomCenter, BottomRight }

    public enum AnchorOffset { Percentage, Unit }

    #endregion

    [DisallowMultipleComponent, ExecuteInEditMode]
    public sealed class Anchoring : MonoBehaviour
    {
        #region Variables

        Vector2 anchoringBounds;

        #endregion

        #region Variables with properties

        [SerializeField]
        Camera anchoringBody = null;
        [HideInInspector, SerializeField]
        Camera prevAnchoringBody = null;
        public Camera AnchoringBody
        {
            get
            {
                return anchoringBody;
            }
            set
            {
                if (prevAnchoringBody != value)
                {
                    anchoringBody = value;

                    prevAnchoringBody = value;

                    RefreshAnchor();
                }
            }
        }

        [SerializeField]
        AnchorOffset anchorOffset = AnchorOffset.Percentage;
        [HideInInspector, SerializeField]
        AnchorOffset prevAnchorOffset = AnchorOffset.Percentage;
        public AnchorOffset AnchorOffset
        {
            get
            {
                return anchorOffset;
            }
            set
            {
                if (prevAnchorOffset != value)
                {
                    anchorOffset = value;

                    prevAnchorOffset = value;

                    RefreshAnchor();
                }
            }
        }

        [SerializeField]
        Vector2 percentageOffset = Vector2.zero;
        [HideInInspector, SerializeField]
        Vector2 prevPercentageOffset = Vector2.zero;
        public Vector2 PercentageOffset
        {
            get
            {
                return percentageOffset;
            }
            set
            {
                if (prevPercentageOffset != value)
                {
                    percentageOffset = value;

                    prevPercentageOffset = value;

                    RefreshAnchor();
                }
            }
        }

        [SerializeField]
        Vector2 unitOffset = Vector2.zero;
        [HideInInspector, SerializeField]
        Vector2 prevUnitOffset = Vector2.zero;
        public Vector2 UnitOffset
        {
            get
            {
                return unitOffset;
            }
            set
            {
                if (prevUnitOffset != value)
                {
                    unitOffset = value;

                    prevUnitOffset = value;

                    RefreshAnchor();
                }
            }
        }

        [SerializeField]
        AnchorAlignment anchor = AnchorAlignment.Center;
        [HideInInspector, SerializeField]
        AnchorAlignment prevAnchor = AnchorAlignment.Center;
        public AnchorAlignment Anchor
        {
            get
            {
                return anchor;
            }
            set
            {
                if (prevAnchor != value)
                {
                    anchor = value;

                    prevAnchor = value;

                    RefreshAnchor();
                }
            }
        }

        [SerializeField]
        SpriteAlignment pivot = SpriteAlignment.Center;
        [HideInInspector, SerializeField]
        SpriteAlignment prevPivot = SpriteAlignment.Center;
        public SpriteAlignment Pivot
        {
            get
            {
                return pivot;
            }
            set
            {
                if (prevPivot != value)
                {
                    pivot = value;

                    prevPivot = value;

                    switch (pivot)
                    {
                        case SpriteAlignment.TopLeft:
                            PivotPoint = new Vector2(0, 1);
                            break;
                        case SpriteAlignment.TopCenter:
                            PivotPoint = new Vector2(0.5f, 1);
                            break;
                        case SpriteAlignment.TopRight:
                            PivotPoint = new Vector2(1, 1);
                            break;
                        case SpriteAlignment.LeftCenter:
                            PivotPoint = new Vector2(0, 0.5f);
                            break;
                        case SpriteAlignment.Center:
                            PivotPoint = new Vector2(0.5f, 0.5f);
                            break;
                        case SpriteAlignment.RightCenter:
                            PivotPoint = new Vector2(1, 0.5f);
                            break;
                        case SpriteAlignment.BottomLeft:
                            PivotPoint = new Vector2(0, 0);
                            break;
                        case SpriteAlignment.BottomCenter:
                            PivotPoint = new Vector2(0.5f, 0);
                            break;
                        case SpriteAlignment.BottomRight:
                            PivotPoint = new Vector2(1, 0);
                            break;
                    }
                }
            }
        }

        [SerializeField]
        Vector2 pivotPoint = new Vector2(0.5f, 0.5f);
        [HideInInspector, SerializeField]
        Vector2 prevPivotPoint = new Vector2(0.5f, 0.5f);
        public Vector2 PivotPoint
        {
            get
            {
                return pivotPoint;
            }
            set
            {
                if (prevPivotPoint != value)
                {
                    pivotPoint = value;

                    prevPivotPoint = value;

                    RefreshAnchor();
                }
            }
        }

        public Vector3 PivotOffset
        {
            get
            {
                if (GetComponent<MeshRenderer>() == null)
                {
                    return Vector3.zero;
                }

                Vector3 point = GetComponent<MeshRenderer>().bounds.size;

                point.Set(point.x * (pivotPoint.x - 0.5f), point.y * (pivotPoint.y - 0.5f), 0);

                return point;
            }
        }

        #endregion

        #region Properties

        public Vector2 Min
        {
            get
            {
                return new Vector2(-anchoringBounds.x * 0.5f,
                    anchoringBounds.y * 0.5f);
            }
        }

        public Vector2 Max
        {
            get
            {
                return new Vector2(anchoringBounds.x * 0.5f,
                   -anchoringBounds.y * 0.5f);
            }
        }

        #endregion

        #region Reserved methods // OnEnable - Update

        void OnEnable()
        {
            RefreshAnchor();
        }

        void Update()
        {
            if (anchoringBody != prevAnchoringBody || anchorOffset != prevAnchorOffset ||
                percentageOffset.x != prevPercentageOffset.x || percentageOffset.y != prevPercentageOffset.y ||
                unitOffset.x != prevUnitOffset.x || unitOffset.y != prevUnitOffset.y ||
                anchor != prevAnchor || pivot != prevPivot ||
                pivotPoint.x != prevPivotPoint.x || pivotPoint.y != prevPivotPoint.y)
            {
                AnchoringBody = anchoringBody;
                AnchorOffset = anchorOffset;
                PercentageOffset = percentageOffset;
                UnitOffset = unitOffset;
                Anchor = anchor;
                Pivot = pivot;
                PivotPoint = pivotPoint;
            }
        }

        #endregion

        #region Refresh Anchor

        public void RefreshAnchor()
        {
            if (anchoringBody == null)
            {
                return;
            }

            anchoringBounds = anchoringBody.Size();

            Vector3 position;

            Vector2 min = Min;
            Vector2 max = Max;
            Vector2 size = new Vector2(Mathf.Abs(max.x - min.x), Mathf.Abs(max.y - min.y));

            position = min;

            if (anchorOffset == AnchorOffset.Unit)
            {
                switch (anchor)
                {
                    case AnchorAlignment.TopLeft:
                        position.x += unitOffset.x;
                        position.y -= unitOffset.y;
                        break;
                    case AnchorAlignment.TopCenter:
                        position.x += size.x * 0.5f + unitOffset.x;
                        position.y -= unitOffset.y;
                        break;
                    case AnchorAlignment.TopRight:
                        position.x += size.x - unitOffset.x;
                        position.y -= unitOffset.y;
                        break;
                    case AnchorAlignment.LeftCenter:
                        position.x += unitOffset.x;
                        position.y = max.y + size.y * 0.5f + unitOffset.y;
                        break;
                    case AnchorAlignment.Center:
                        position.x += size.x * 0.5f + unitOffset.x;
                        position.y = max.y + size.y * 0.5f + unitOffset.y;
                        break;
                    case AnchorAlignment.RightCenter:
                        position.x += size.x - unitOffset.x;
                        position.y = max.y + size.y * 0.5f + unitOffset.y;
                        break;
                    case AnchorAlignment.BottomLeft:
                        position.x += unitOffset.x;
                        position.y = max.y + unitOffset.y;
                        break;
                    case AnchorAlignment.BottomCenter:
                        position.x += size.x * 0.5f + unitOffset.x;
                        position.y = max.y + unitOffset.y;
                        break;
                    case AnchorAlignment.BottomRight:
                        position.x += size.x - unitOffset.x;
                        position.y = max.y + unitOffset.y;
                        break;
                }
            }
            else
            {
                switch (anchor)
                {
                    case AnchorAlignment.TopLeft:
                        position.x += size.x * percentageOffset.x;
                        position.y -= size.y * percentageOffset.y;
                        break;
                    case AnchorAlignment.TopCenter:
                        position.x += size.x * 0.5f + size.x * percentageOffset.x;
                        position.y -= size.y * percentageOffset.y;
                        break;
                    case AnchorAlignment.TopRight:
                        position.x += size.x - size.x * percentageOffset.x;
                        position.y -= size.y * percentageOffset.y;
                        break;
                    case AnchorAlignment.LeftCenter:
                        position.x += size.x * percentageOffset.x;
                        position.y += max.y + size.y * percentageOffset.y;
                        break;
                    case AnchorAlignment.Center:
                        position.x += size.x * 0.5f + size.x * percentageOffset.x;
                        position.y += max.y + size.y * percentageOffset.y;
                        break;
                    case AnchorAlignment.RightCenter:
                        position.x += size.x - size.x * percentageOffset.x;
                        position.y += max.y + size.y * percentageOffset.y;
                        break;
                    case AnchorAlignment.BottomLeft:
                        position.x += size.x * percentageOffset.x;
                        position.y = max.y + size.y * percentageOffset.y;
                        break;
                    case AnchorAlignment.BottomCenter:
                        position.x += size.x * 0.5f + size.x * percentageOffset.x;
                        position.y = max.y + size.y * percentageOffset.y;
                        break;
                    case AnchorAlignment.BottomRight:
                        position.x += size.x - size.x * percentageOffset.x;
                        position.y = max.y + size.y * percentageOffset.y;
                        break;
                }
            }

            position += anchoringBody.transform.position - PivotOffset;
            position.z = transform.position.z;

            if (float.IsNaN(position.x) || float.IsNaN(position.y) || float.IsInfinity(position.x) || float.IsInfinity(position.y))
            {
                return;
            }

            transform.position = position;
        }

        #endregion
    }
}