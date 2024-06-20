using UnityEngine;
using Meta.XR.BuildingBlocks;


    //[RequireComponent(typeof(CustomSpatialAnchorSpawnerBuildingBlock))]
    public class CustomSpatialAnchorSpawner : MonoBehaviour
    {
        public GameObject AnchorPrefab
        {
            get => _anchorPrefab;
            set
            {
                _anchorPrefab = value;
                if (_anchorPrefabTransform)  Destroy(_anchorPrefabTransform.gameObject);
                _anchorPrefabTransform = Instantiate(AnchorPrefab).transform;
                FollowHand = _followHand;
            }
        }

        public bool FollowHand
        {
            get => _followHand;
            set
            {
                _followHand = value;
                if (_followHand)
                {
                    _initialPosition = _anchorPrefabTransform.position;
                    _initialRotation = _anchorPrefabTransform.rotation;
                    _anchorPrefabTransform.parent = _cameraRig.rightControllerAnchor;
                    _anchorPrefabTransform.localPosition = Vector3.zero;
                    _anchorPrefabTransform.localRotation = Quaternion.identity;
                }
                else
                {
                    _anchorPrefabTransform.parent = null;
                    _anchorPrefabTransform.SetPositionAndRotation(_initialPosition, _initialRotation);
                }
            }
        }

        [Tooltip("A placeholder object to place in the anchor's position.")]
        [SerializeField]
        private GameObject _anchorPrefab;

        [Tooltip("Anchor prefab GameObject will follow the user's right hand.")]
        [SerializeField] private bool _followHand = true;

        private SpatialAnchorCoreBuildingBlock _spatialAnchorCore;
        private OVRCameraRig _cameraRig;
        private Transform _anchorPrefabTransform;
        private Vector3 _initialPosition;
        private Quaternion _initialRotation;

        private void Awake()
        {
            _spatialAnchorCore = FindAnyObjectByType<SpatialAnchorCoreBuildingBlock>();
            if (!_spatialAnchorCore)
            {
                Debug.Log($"[{nameof(SpatialAnchorCoreBuildingBlock)}] component is missing.");
                return;
            }
            _cameraRig = FindAnyObjectByType<OVRCameraRig>();
            AnchorPrefab = _anchorPrefab;
            FollowHand = _followHand;
            _spatialAnchorCore.OnAnchorsEraseAllCompleted.AddListener(delegate { AnchorPrefab = _anchorPrefab; });
        }

        /// <summary>
        /// Spawn a spatial anchor.
        /// </summary>
        /// <param name="position">Position for the new anchor.</param>
        /// <param name="rotation">Orientation of the new anchor</param>
        public void SpawnSpatialAnchor(Vector3 position, Quaternion rotation)
        {
            _spatialAnchorCore.InstantiateSpatialAnchor(AnchorPrefab, position, rotation);
            Destroy(_anchorPrefabTransform.gameObject);
        }

        public void SpawnSpatialAnchor()
        {
            if (!_anchorPrefabTransform) return;
            if (!FollowHand)
                SpawnSpatialAnchor(AnchorPrefab.transform.position, AnchorPrefab.transform.rotation);
            else
                SpawnSpatialAnchor(_anchorPrefabTransform.position, _anchorPrefabTransform.rotation);
        }
    }
