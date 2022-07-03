using UnityEngine;
using MalbersAnimations.Events;
using UnityEngine.UI;

namespace MalbersAnimations.UI
{
    /// <summary>makes an UI Object to follow a World Object</summary>
    [AddComponentMenu("Malbers/UI/UI Follow Transform")]
    public class UIFollowTransform : MonoBehaviour
    {
        public Camera MainCamera;
        public Transform WorldTransform;
        public Text TextUI;
        [Tooltip("Reset the World Transform to Null when this component is Disable")]
        public bool ResetOnDisable = false;
        [Tooltip("Clear the text component string value when this component is disabled")]
        public bool ClearText = false;

        public Behaviour HideOffScreen;
        public Vector3 ScreenCenter { get; set; }
        public Vector3 DefaultScreenCenter { get; set; }

 

        void Awake()
        {
            MainCamera = MTools.FindMainCamera();
           if (TextUI == null) TextUI = GetComponent<Text>();
            ScreenCenter = transform.position;
            DefaultScreenCenter = transform.position;
        }

        private void OnEnable()
        {
            MainCamera = MTools.FindMainCamera();

            if (WorldTransform)
                Align();
            else
                Clear();
        }

        private void OnDisable()
        {
            if (ResetOnDisable)
            {
                Clear();
                if (TextUI != null) TextUI.text = string.Empty;
            }
        }

        public virtual void Clear()
        {
            WorldTransform = null;
            transform.position = ScreenCenter;
            transform.gameObject.SetActive(true);
            enabled = false;
            if (HideOffScreen) HideOffScreen.enabled = true;
        }

        public void SetTransform(Transform newTarget)
        {
            WorldTransform = newTarget;
            Align();
            enabled = true;
        }
        public void SetScreenCenter(Vector3 newScreenCenter)
        {
            ScreenCenter = newScreenCenter;
            enabled = true;
        }

        void Update()
        {
            Align();
        }

        public void Align()
        {
            if (MainCamera == null || WorldTransform == null) { enabled = false; return; }
            var pos = MainCamera.WorldToScreenPoint(WorldTransform.position);
            transform.position = pos;
            if (HideOffScreen) HideOffScreen.enabled = (DoHideOffScreen(pos));
        }



        private bool DoHideOffScreen(Vector3 position)
        {
            if (position.x < 0 || position.x > Screen.width) return false;
            if (position.y < 0 || position.y > Screen.height) return false;
            if (position.z < 0) return false;

            return true;
        }


#if UNITY_EDITOR

        void Reset()
        {
            MEventListener MeventL = GetComponent<MEventListener>();

            if (MeventL == null)
            {
                MeventL = gameObject.AddComponent<MEventListener>();
            }

            MeventL.Events = new System.Collections.Generic.List<MEventItemListener>(1) { new MEventItemListener() };

            var listener = MeventL.Events[0];

            listener.useTransform = true;
            listener.useVector3 = true;
            listener.useVoid = false;

            listener.Event = MTools.GetInstance<MEvent>("Follow UI Transform");

            if (listener.Event != null)
            {
                UnityEditor.Events.UnityEventTools.AddPersistentListener(listener.ResponseTransform, SetTransform);
                UnityEditor.Events.UnityEventTools.AddPersistentListener(listener.ResponseVector3, SetScreenCenter);
            }
        }
#endif
    }
}