using SaintsField.Editor.Playa;
using UnityEditor;
using UnityEngine;

namespace Editor.Playa.AnimatorEditorHijack
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(StateMachineBehaviour), true)]
    public class SaintsStateMachineBehaviourEditor : ApplySaintsEditorBase
    {
        // UnityEditor.Graphs.AnimationStateMachine.StateEditor
    }
}
