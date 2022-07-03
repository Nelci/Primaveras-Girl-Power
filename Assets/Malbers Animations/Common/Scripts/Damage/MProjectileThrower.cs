using MalbersAnimations.Scriptables;
using MalbersAnimations.Utilities;
using UnityEngine;

namespace MalbersAnimations.Weapons
{
    [AddComponentMenu("Malbers/Damage/Projectile Thrower")]

    public class MProjectileThrower : MonoBehaviour, IThrower
    {
        /// <summary> Is Used to calculate the Trajectory and Display it as a LineRenderer </summary>
        public System.Action<bool> Predict { get; set; }

        [Header("Projectile")]
        [SerializeField, Tooltip("What projectile will be instantiated")]
        private GameObjectReference m_Projectile = new GameObjectReference();
        [Tooltip("The projectile will be fired on start")]
        public BoolReference FireOnStart;


        [Header("Layer Interaction")]
        [SerializeField] private LayerReference hitLayer = new LayerReference(-1);
        [SerializeField] private QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore;

        [Header("References")]
        [SerializeField, Tooltip("When this parameter is set it will Aim Directly to the Target")]
        private TransformReference m_Target;
        [SerializeField, Tooltip("Transform Reference for to calculate the Thrower Aim Origin Position")]
        private Transform m_AimOrigin;
        [SerializeField, Tooltip("Owner of the Thrower Component. By default it should be the Root GameObject")] 
        private GameObjectReference m_Owner;
        [Tooltip("Reference for the Aimer Component")]
        public Aim Aimer;

        [Header("Physics Values")]
        [SerializeField, Tooltip("Launch force for the Projectile")]
        private float m_power = 50f;
        
        [Range(0, 90)]
        [SerializeField, Tooltip("Angle of the Projectile when a Target is assigned")]
        private float m_angle = 45f;
        [SerializeField, Tooltip("Gravity to apply to the Projectile. By default is set to Physics.gravity")]
        private Vector3Reference gravity = new Vector3Reference(Physics.gravity);

        
        
        public Vector3 Gravity { get => gravity.Value; set => gravity.Value = value; }
        public LayerMask Layer { get => hitLayer.Value; set => hitLayer.Value = value; }
        public QueryTriggerInteraction TriggerInteraction { get => triggerInteraction; set => triggerInteraction = value; }
        public Vector3 AimOriginPos => m_AimOrigin.position;
        public Transform Target { get => m_Target.Value; set => m_Target.Value = value; }

        /// <summary> Owner of the  </summary>
        public GameObject Owner { get => m_Owner.Value; set => m_Owner.Value = value; }
        public GameObject Projectile { get => m_Projectile.Value; set => m_Projectile.Value = value; }

        /// <summary> Projectile Launch Velocity(v3) </summary>
        public Vector3 Velocity { get; set; }

        /// <summary>Force to Apply to the Projectile</summary>
        public float Power { get => m_power; set => m_power = value; }

        public Transform AimOrigin => m_AimOrigin;

        private void Start()
        {
            if (Owner == null) Owner = transform.root.gameObject;
            if (m_AimOrigin == null) m_AimOrigin = transform;
            if (Aimer) m_AimOrigin = Aimer.AimOrigin; //Set the Aim origin from the Aimer

            if (FireOnStart) Fire();
        }

        /// <summary> Fire Proyectile </summary>
        public virtual void Fire()
        {
            if (!enabled) return;
            if (Projectile == null) return;

            CalculateVelocity();
            var Inst_Projectile = Instantiate(Projectile, AimOriginPos, Quaternion.identity);

            Prepare_Projectile(Inst_Projectile);

            Predict?.Invoke(false);
        }


        public void SetTarget(Transform target) => Target = target;
        public void ClearTarget() => Target = null;
        public void SetTarget(GameObject target) => Target = target.transform;

        void Prepare_Projectile(GameObject p)
        {
            var projectile = p.GetComponent<MProjectile>();

            if (projectile != null) //Means its a Malbers Projectile ^^
            {
                projectile.Prepare(Owner, Gravity, Velocity,  Layer, TriggerInteraction);
                projectile.Fire();
            }
            else //Fire without the Projectile Component
            {
                var rb = p.GetComponent<Rigidbody>();
                rb?.AddForce(Velocity, ForceMode.VelocityChange);
            }
        }


        public virtual void CalculateVelocity()
        {
            if (Target)
            {
                var target_Direction = (Target.position - AimOriginPos).normalized;

                float TargetAngle = 90 - Vector3.Angle(target_Direction, -Gravity) + 0.1f; //in case the angle is smaller than the target height

                if (TargetAngle < m_angle)
                    TargetAngle = m_angle;

                Power = MTools.PowerFromAngle(AimOriginPos, Target.position, TargetAngle);
                Velocity = MTools.VelocityFromPower(AimOriginPos, Power, TargetAngle, Target.position);
            }
            else 
            if (Aimer)
            {
                Velocity = Aimer.AimDirection.normalized * Power;
            }
            else
            {
                Velocity = transform.forward.normalized * Power;
            }

            Predict?.Invoke(true);
        }

        void Reset()
        {
            Owner = transform.root.gameObject;
            m_AimOrigin = transform;
        }


#if UNITY_EDITOR

        void OnDrawGizmos()
        {
            UnityEditor.Handles.color = Color.blue;
            UnityEditor.Handles.ArrowHandleCap(0, transform.position, transform.rotation, Power*0.01f, EventType.Repaint);
        }
#endif
    }
}