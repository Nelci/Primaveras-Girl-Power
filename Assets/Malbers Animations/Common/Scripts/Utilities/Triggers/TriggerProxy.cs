using UnityEngine;
using MalbersAnimations.Events;
using MalbersAnimations.Scriptables;
using System.Collections.Generic;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MalbersAnimations.Utilities
{
    /// <summary>
    /// This is used when the collider is in a different gameObject and you need to check the Collider Events
    /// Create this component at runtime and subscribe to the UnityEvents
    /// </summary>
    [AddComponentMenu("Malbers/Utilities/Colliders/Trigger Proxy")]
    public class TriggerProxy : MonoBehaviour, IMLayer
    {
        //[Tooltip("Proxy ID, can be used to Identify which is the Proxy Trigger used")]
        //[SerializeField] private IntReference m_ID = new IntReference(0);
        [Tooltip("Hit Layer for the Trigger Proxy")]
        [SerializeField] private LayerReference hitLayer = new LayerReference(-1);
        [SerializeField] private QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore;
        [Tooltip("Search only Tags")]
        public Tag[] Tags;

        public ColliderEvent OnTrigger_Enter = new ColliderEvent();
        public ColliderEvent OnTrigger_Exit = new ColliderEvent();
        public ColliderEvent OnTrigger_Stay = new ColliderEvent();

        public GameObjectEvent OnGameObjectEnter = new GameObjectEvent();
        public GameObjectEvent OnGameObjectExit = new GameObjectEvent();
        public GameObjectEvent OnGameObjectStay = new GameObjectEvent();

        [SerializeField] private bool m_debug = false;

        public BoolReference useOnTriggerStay = new BoolReference();

        internal List<Collider> m_colliders = new List<Collider>();


        ///// <summary>All the Gameobjects using the Proxy</summary>
        //internal List<Type> AllowedTypes = new List<Type>();

        public Type ForcedType;



        /// <summary>All the Gameobjects using the Proxy</summary>
        internal List<GameObject> EnteringGameObjects = new List<GameObject>();

        public Action<GameObject, Collider> EnterTriggerInteraction = delegate { };
        public Action<GameObject, Collider> ExitTriggerInteraction = delegate { };

        public bool Active { get => enabled; set => enabled = value; }

        //public int ID { get => m_ID.Value; set => m_ID.Value = value; }
        public LayerMask Layer { get => hitLayer.Value; set => hitLayer.Value = value; }
        public QueryTriggerInteraction TriggerInteraction { get => triggerInteraction; set => triggerInteraction = value; }

        /// <summary> Collider Component used for the Trigger Proxy </summary>
        [RequiredField] public Collider trigger;
        public Transform Owner { get; set; } 

        public bool TrueConditions(Collider other)
        {
            if (!Active) return false;

            if (Tags != null && Tags.Length > 0)
            {
                if (!other.gameObject.HasMalbersTagInParent(Tags)) return false;
            }

            ////Check for Forced Type
            //if (ForcedType != null)
            //{
            //    if (!other.GetComponentInParent(ForcedType)) { return false; }
            //}
                      
            if (trigger == null) return false; // you are 
            if (triggerInteraction == QueryTriggerInteraction.Ignore && other.isTrigger) return false; // Check Trigger Interactions 
            if (!MTools.Layer_in_LayerMask(other.gameObject.layer, Layer)) return false;
           
            if (transform.IsChildOf(other.transform)) return false;             // Do not Interact with yourself
            if (other.transform.IsChildOf(Owner)) return false;             // Do not Interact with yourself

            return true;
        }
        

        void OnTriggerEnter(Collider other)
        {
            if (TrueConditions(other))
            {
                var newGo = other.transform.root.gameObject;                              //Get the animal on the entering collider

                OnTrigger_Enter.Invoke(other); //Invoke when a Collider enters the Trigger
                if (m_debug) Debug.Log($"<b>{name}</b> [Entering Collider] -> [{other.name}]",this);


                //Check Recently destroyed Colliders (Strange bug)
                CheckMissingColliders();

                if (m_colliders.Find(coll => coll == other) == null)                               //if the entering collider is not already on the list add it
                    m_colliders.Add(other);

                if (EnteringGameObjects.Contains(newGo))
                {
                    return;
                }
                else
                {
                    EnteringGameObjects.Add(newGo);
                   
                    EnterTriggerInteraction(newGo, other);
                    
                    OnGameObjectEnter.Invoke(newGo);

                    if (m_debug) Debug.Log($"<b>{name}</b> [Entering GameObject] -> [{newGo.name}]", this);
                }
            }
        }


        /// <summary>Check Recently destroyed Colliders (Strange bug)</summary>
        private void CheckMissingColliders()
        {
            for (var i = m_colliders.Count - 1; i > -1; i--)
            {
                if (m_colliders[i] == null) m_colliders.RemoveAt(i);
            }

            if (m_colliders.Count == 0) EnteringGameObjects = new List<GameObject>();
        }

        void OnTriggerExit(Collider other)
        {
            if (TrueConditions(other))
            {
                OnTrigger_Exit.Invoke(other);
                m_colliders.Remove(other);

                if (m_debug) Debug.Log($"<b>{name}</b> [Exit Collider] -> [{other.name}]", this);

                var newGo = other.transform.root.gameObject;         //Get the gameObject on the entering collider

                if (EnteringGameObjects.Contains(newGo))             //Means that the Entering GameObject still exist
                {
                    if (!m_colliders.Exists(c => c != null && c.transform.root.gameObject == newGo)) //Means that all that root colliders are out
                    {
                        EnteringGameObjects.Remove(newGo);
                        OnGameObjectExit.Invoke(newGo);

                        ExitTriggerInteraction(newGo, other);

                        if (m_debug) Debug.Log($"<b>{name}</b> - [Leaving Gameobject] -> [{newGo.name}]", this);
                    }
                }
              CheckMissingColliders();
            }
        }

        public void ResetTrigger()
        {
            m_colliders = new List<Collider>();
            EnteringGameObjects = new List<GameObject>();
        }

        private void OnDisable()
        {
            if (m_colliders.Count > 0)
            {
                foreach (var c in m_colliders)
                {
                   if (c) OnTrigger_Exit.Invoke(c); //the colliders may be destroyed
                }
            }

            if (EnteringGameObjects.Count > 0)
            {
                foreach (var c in EnteringGameObjects)
                {
                    if (c) OnGameObjectExit.Invoke(c);  //the gameobjects  may be destroyed
                }
            }
            ResetTrigger();
        }

        private void OnEnable() => ResetTrigger();

        private void Awake()
        {
          if (trigger == null)  trigger = GetComponent<Collider>();

            if (trigger) trigger.isTrigger = true;
            else
                Debug.LogWarning("This Script requires a Collider, please add any type of collider", this);

            if (Owner == null) Owner = transform; 

            ResetTrigger();
        }


        private void Update()
        {
            CheckOntriggerStay();
        }
        void CheckOntriggerStay()
        {
            if (useOnTriggerStay.Value)
            {
                foreach (var gos in EnteringGameObjects)
                {
                    OnGameObjectStay.Invoke(gos);
                }

                foreach (var col in m_colliders)
                {
                    OnTrigger_Stay.Invoke(col);
                }
            }
        }

        public void SetLayer(LayerMask mask, QueryTriggerInteraction triggerInteraction, Transform Owner, Tag[] tags = null)
        {
            TriggerInteraction = triggerInteraction;
            Tags = tags;
            Layer = mask;
            this.Owner = Owner;
        }
    }

    #region Inspector


#if UNITY_EDITOR
    [CanEditMultipleObjects, CustomEditor(typeof(TriggerProxy))]
    public class TriggerProxyEditor : Editor
    {
        SerializedProperty debug, OnTrigger_Enter, OnTrigger_Exit, useOnTriggerStay, OnTrigger_Stay, 
            triggerInteraction, hitLayer, OnGameObjectEnter, OnGameObjectExit, OnGameObjectStay, Tags;

        TriggerProxy m;

        private void OnEnable()
        {
            m = (TriggerProxy)target;
            triggerInteraction = serializedObject.FindProperty("triggerInteraction");
            useOnTriggerStay = serializedObject.FindProperty("useOnTriggerStay");
            hitLayer = serializedObject.FindProperty("hitLayer");
            debug = serializedObject.FindProperty("m_debug");
            OnTrigger_Enter = serializedObject.FindProperty("OnTrigger_Enter");
            OnTrigger_Exit = serializedObject.FindProperty("OnTrigger_Exit");
            OnGameObjectEnter = serializedObject.FindProperty("OnGameObjectEnter");
            OnGameObjectExit = serializedObject.FindProperty("OnGameObjectExit");
            Tags = serializedObject.FindProperty("Tags");
            OnGameObjectStay = serializedObject.FindProperty("OnGameObjectStay");
            OnTrigger_Stay = serializedObject.FindProperty("OnTrigger_Stay");
        }


        public override void OnInspectorGUI()
        {
            MalbersEditor.DrawDescription("Use this component to do quick OnTrigger Enter/Exit logics");

            serializedObject.Update();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.BeginHorizontal();
            //EditorGUILayout.PropertyField(trigger, new GUIContent("Trigger"));
            EditorGUILayout.PropertyField(hitLayer, new GUIContent("Layer"));
            MalbersEditor.DrawDebugIcon(debug);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.PropertyField(triggerInteraction);
            EditorGUILayout.PropertyField(useOnTriggerStay);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(Tags,true);
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);


            OnTrigger_Enter.isExpanded = MalbersEditor.Foldout(OnTrigger_Enter.isExpanded, "Events");

            if (OnTrigger_Enter.isExpanded)
            {
                EditorGUILayout.PropertyField(OnTrigger_Enter, new GUIContent("On Trigger Enter"));
                EditorGUILayout.PropertyField(OnTrigger_Exit, new GUIContent("On Trigger Exit"));
                if (m.useOnTriggerStay.Value)
                    EditorGUILayout.PropertyField(OnTrigger_Stay, new GUIContent("On Trigger Stay"));
                
                
                EditorGUILayout.PropertyField(OnGameObjectEnter, new GUIContent("On GameObject Enter "));
                EditorGUILayout.PropertyField(OnGameObjectExit, new GUIContent("On GameObject Exit"));
                if (m.useOnTriggerStay.Value)
                EditorGUILayout.PropertyField(OnGameObjectStay, new GUIContent("On GameObject Stay"));
            }

            EditorGUILayout.EndVertical();

            if (Application.isPlaying && debug.boolValue)
            {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                EditorGUILayout.LabelField("Debug", EditorStyles.boldLabel);

             //   EditorGUILayout.ObjectField("Own Collider", m.trigger, typeof(Collider), false);

                EditorGUILayout.LabelField("Entering GameObjects (" + m.EnteringGameObjects.Count + ")", EditorStyles.boldLabel);
                foreach (var item in m.EnteringGameObjects)
                {
                    EditorGUILayout.ObjectField(item.name, item, typeof(GameObject), false);
                }

                EditorGUILayout.LabelField("Entering Colliders (" + m.m_colliders.Count + ")", EditorStyles.boldLabel);
               
                foreach (var item in m.m_colliders)
                {
                    if (item != null)
                    EditorGUILayout.ObjectField(item.name, item, typeof(Collider), false);
                }

                EditorGUILayout.EndVertical();
                Repaint();
                EditorGUI.EndDisabledGroup();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
    #endregion
}