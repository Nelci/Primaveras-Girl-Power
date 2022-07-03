using MalbersAnimations.Scriptables;
using UnityEngine;

namespace MalbersAnimations.Utilities
{
    [AddComponentMenu("Malbers/Utilities/Humanoid Parent")]
    public class HumanoidParent : MonoBehaviour
    {
        public Animator animator;
        [SearcheableEnum]
        [Tooltip("Which bone will be the parent of this gameobject")]
        public HumanBodyBones parent = HumanBodyBones.Spine;
        [Tooltip("Reset the Local Position of this gameobject when parented")]
        public BoolReference LocalPos;
        [Tooltip("Reset the Local Rotation of this gameobject when parented")]
        public BoolReference LocalRot;
        [Tooltip("Additional Local Position Offset to add after the gameobject is parented")]
        public Vector3Reference PosOffset;
        [Tooltip("Additional Local Rotation Offset to add after the gameobject is parented")]
        public Vector3Reference RotOffset;

        private void Awake()
        {
            if (animator != null)
            {
                var boneParent = animator.GetBoneTransform(parent);

                if (boneParent != null && transform.parent != boneParent)
                {
                    transform.parent = boneParent;

                    if (LocalPos.Value) transform.localPosition = Vector3.zero;
                    if (LocalRot.Value) transform.localRotation = Quaternion.identity;

                    transform.localPosition += PosOffset;
                    transform.localRotation *= Quaternion.Euler(RotOffset);
                }
            }
        }

        private void OnValidate()
        {
            if (animator == null) animator = gameObject.FindComponent<Animator>();
        }
    }
}
