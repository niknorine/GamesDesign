using UnityEngine;
using System.Collections.Generic;

namespace DeepSky.Haze
{
    [ExecuteInEditMode, AddComponentMenu("DeepSky Haze/Controller", 51)]
    public class DS_HazeCore : MonoBehaviour
    {
        public enum HeightFalloffType { Exponential, None };

        private static readonly int[] kBayerMatrix = { 1, 49, 13, 61, 4, 52, 16, 64,
                                33, 17, 45, 29, 36, 20, 48, 32,
                                9, 57, 5, 53, 12, 60, 8, 56,
                                41, 25, 39, 21, 44, 28, 40, 24,
                                3, 51, 15, 63, 2, 50, 14, 62,
                                35, 19, 47, 31, 34, 18, 46, 30,
                                11, 59, 7, 55, 10, 58, 6, 54,
                                43, 27, 39, 23, 42, 26, 38, 22 };

        public static readonly Vector2[] kOffsetSequence = new Vector2[64];

        private static DS_HazeCore instance;
        public static DS_HazeCore Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<DS_HazeCore>();
                }

                return instance;
            }
        }

        #region FIELDS
        [SerializeField, Range(0, 1), Tooltip("The time at which Zones will evaluate their settings. Animate this or set in code to create time-of-day transitions.")]
        private float m_Time = 0.0f;
        [SerializeField, Tooltip("The height falloff method to use globally (default Exponential).")]
        private HeightFalloffType m_HeightFalloff = HeightFalloffType.Exponential;
        [SerializeField, HideInInspector]
        private List<DS_HazeZone> m_Zones = new List<DS_HazeZone>();

        // Volumetric lights set.
        private HashSet<DS_HazeLightVolume> m_LightVolumes = new HashSet<DS_HazeLightVolume>();
        private Texture2D m_InterleavedSampleTexture;

        public float Time
        {
            get { return m_Time; }
            set { m_Time = Mathf.Clamp01(value); }
        }

        public Texture2D InterleavedSampleTexture
        {
            get { return m_InterleavedSampleTexture; }
        }


        public HeightFalloffType HeightFalloff
        {
            get { return m_HeightFalloff; }
            set {
                m_HeightFalloff = value;
                SetGlobalHeightFalloff();
            }
        }
        #endregion

        private void SetGlobalHeightFalloff()
        {
            switch (m_HeightFalloff)
            {
                case HeightFalloffType.Exponential:
                    Shader.DisableKeyword("DS_HAZE_HEIGHT_FALLOFF_NONE");
                    break;
                case HeightFalloffType.None:
                    Shader.EnableKeyword("DS_HAZE_HEIGHT_FALLOFF_NONE");
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Rebuild the list whenever the children change. This doesn't happen very often
        /// so just clear the old structure and remake.
        /// Inactive zones are included, they will be ignored later during blending.
        /// </summary>
        void OnTransformChildrenChanged()
        {
            Debug.Log("Repopulating zones: " + m_Zones.Count);

            m_Zones.Clear();
            DS_HazeZone[] zones = GetComponentsInChildren<DS_HazeZone>(true);
            m_Zones.AddRange(zones);
        }

        private int GetHaltonCoord(float index, float dim)
        {
            float res = 0;
            float ff = 1;
            float ii = index;

            while (ii > 0)
            {
                ff /= dim;
                res += ff * (ii % dim);
                ii = Mathf.Floor(ii / dim);
            }
            return Mathf.FloorToInt(res * dim);
        }

        /// <summary>
        /// Create the texture for the sample offets. This represents an 8x8 Bayer matrix.
        /// </summary>
        private void CreateInterleavedSampleTextureResource()
        {
            Color[] pixels = new Color[kBayerMatrix.Length];
            float step = 1.0f / kBayerMatrix.Length;
            for (int of = 0; of < kBayerMatrix.Length; of++)
            {
                pixels[of].r = kBayerMatrix[of] * step;
            }

            m_InterleavedSampleTexture = new Texture2D(8, 8, TextureFormat.RGB24, false, true);
            m_InterleavedSampleTexture.name = "DS_HazeSampleOffset";
            m_InterleavedSampleTexture.hideFlags = HideFlags.HideAndDontSave;
            m_InterleavedSampleTexture.filterMode = FilterMode.Point;
            m_InterleavedSampleTexture.SetPixels(pixels);
            m_InterleavedSampleTexture.Apply();

            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    kOffsetSequence[y * 8 + x].x = GetHaltonCoord(x+1, 8);
                    kOffsetSequence[y * 8 + x].y = GetHaltonCoord(y+1, 8);
                }
            }

            Shader.SetGlobalTexture("_SamplingOffsets", m_InterleavedSampleTexture);
        }

        /// <summary>
        /// On Awake we need to check there is only one Haze Controller in the scene and
        /// set up the singleton reference to it.
        /// </summary>
        void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
            {
                Debug.LogError("DeepSky::DS_HazeCore:Awake - There is more than one Haze Controller in this scene! Disabling " + name);
                enabled = false;
            }
        }

        void OnEnable()
        {
            SetGlobalHeightFalloff();

            if (m_InterleavedSampleTexture == null)
            {
                CreateInterleavedSampleTextureResource();
            }
            Shader.SetGlobalTexture("_SamplingOffsets", m_InterleavedSampleTexture);
        }

        void OnDestroy()
        {
            DestroyImmediate(m_InterleavedSampleTexture);
        }

        /// <summary>
        /// Add a volumetric light component to render this frame.
        /// </summary>
        /// <param name="lightVolume"></param>
        public void AddLightVolume(DS_HazeLightVolume lightVolume)
        {
            RemoveLightVolume(lightVolume);
            m_LightVolumes.Add(lightVolume);
        }

        /// <summary>
        /// Remove a volumetric light component from rendering this frame.
        /// </summary>
        /// <param name="lightVolume"></param>
        public void RemoveLightVolume(DS_HazeLightVolume lightVolume)
        {
            m_LightVolumes.Remove(lightVolume);
        }

        /// <summary>
        /// Populate the passed in lists with all the light volumes that will be rendered from
        /// the given camera position (split depending on whether cast shadows or not).
        /// </summary>
        /// <param name="cameraPosition"></param>
        /// <param name="lightVolumes"></param>
        public void GetRenderLightVolumes(Vector3 cameraPosition, List<DS_HazeLightVolume> lightVolumes, List<DS_HazeLightVolume> shadowVolumes)
        {
            foreach (DS_HazeLightVolume lv in m_LightVolumes)
            {
                if (lv.WillRender(cameraPosition))
                {
                    if (lv.CastShadows)
                    {
                        shadowVolumes.Add(lv);
                    }
                    else
                    {
                        lightVolumes.Add(lv);
                    }
                }
            }
        }

        /// <summary>
        /// Find the zones containing the given position and blend between them.
        /// The render context will be null if the position is not within any zones.
        /// If there is only one zone, that will provide the context as-is.
        /// </summary>
        public DS_HazeContextItem GetRenderContextAtPosition(Vector3 position)
        {
            List<DS_HazeZone> blendZones = new List<DS_HazeZone>();
            for (int zi = 0; zi < m_Zones.Count; zi++)
            {
                if (m_Zones[zi].Contains(position) && m_Zones[zi].enabled)
                {
                    blendZones.Add(m_Zones[zi]);
                }
            }

            if (blendZones.Count == 0) { return null; }

            if (blendZones.Count == 1)
            {
                return blendZones[0].Context.GetContextItemBlended(m_Time);
            }

            blendZones.Sort(delegate (DS_HazeZone z1, DS_HazeZone z2)
            {
                if (z1 < z2) return -1;
                else return 1;
            });

            // With the list in priority order (lowest to highest), iterate through and blend from first to last.
            DS_HazeContextItem renderContext = blendZones[0].Context.GetContextItemBlended(m_Time);
            float weight = 0;
            for (int ci = 1; ci < blendZones.Count; ci++)
            {
                weight = blendZones[ci].GetBlendWeight(position);
                renderContext.Lerp(blendZones[ci].Context.GetContextItemBlended(m_Time), weight);
            }

            return renderContext;
        }
    }
}
