using MalbersAnimations.Scriptables;
using UnityEngine;

namespace MalbersAnimations.Controller
{
    /// <summary>Force the Animal Current State to exit to another state</summary>
    [AddComponentMenu("Malbers/Animal Controller/AllowExit")]
    public class AllowExitBehaviour : StateMachineBehaviour
    {
        private MAnimal animal;
        [Tooltip("State the Animal will exit to, when the time as passed. If null it will not force the next state")]
        public StateID NextState;
        public IntReference ExitStatus = new IntReference();
        [Range(0, 1)]
        public float m_time = 0.8f;
        private bool isOn;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (animal == null) animal = animator.FindComponent<MAnimal>();
            isOn = false;
        }

        public override void OnStateMove(Animator animator, AnimatorStateInfo state, int layerIndex)
        {
            if (animal && !isOn && (state.normalizedTime % 1 >= m_time))
            {
                isOn = true;
                animal.State_Allow_Exit(NextState.ID, ExitStatus);
            }
        }

        void Reset()
        {
            
        }
    }
}