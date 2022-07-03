using MalbersAnimations.Scriptables;


namespace MalbersAnimations.Controller.AI
{
    public class IsTargetInSetDecision : MAIDecision
    {
        public override string DisplayName => "General/Is Target in RuntimeSet?";
        public RuntimeGameObjects Set;

        public override bool Decide(MAnimalBrain brain, int Index)
        {
            if (brain.AIControl.Target != null)
            {
                return Set.Items.Exists(x => x.transform == brain.AIControl.Target);
            }
            return false;
        }

        void Reset() => Description = "Returns true if the Current AI Target is on a Runtime Set";
         
    }
}