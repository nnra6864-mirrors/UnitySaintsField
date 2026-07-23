using SaintsField.Editor.Utils;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using HeaderGUI = SaintsField.Editor.HeaderGUI;

namespace Editor.Playa.AnimatorEditorHijack
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(UnityEditor.Animations.AnimatorState), true)]
    internal class SaintsAnimatorStateEditor: UnityEditor.Graphs.AnimationStateMachine.StateEditor
    {
        private StateMachineBehaviorsEditorHijack _behavioursEditor;

        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = new VisualElement();
            // root.Add(new Label("HI"));

            root.Add(new IMGUIContainer(OnInspectorGUI));

            // Now, _behavioursEditor
            _behavioursEditor ??= StateMachineBehaviorsEditorHijack.Wrap(this);

            root.Add(_behavioursEditor.CreateInspectorGUI());

            root.Add(new IMGUIContainer(() => _behavioursEditor?.AddBehaviourButtonPublic()));

            UIToolkitUtils.OnAttachToPanelOnce(root, _ =>
            {
                root.schedule.Execute(() => HeaderGUI.DrawHeaderGUI.EnsureInitLoad()).StartingIn(500);
            });
            return root;
        }

        public new void OnEnable()
        {
            base.OnEnable();
            _behavioursEditor = StateMachineBehaviorsEditorHijack.Wrap(this);
        }

        public override void OnInspectorGUI()
        {
            if (_behavioursEditor == null)
            {
                base.OnInspectorGUI();
                return;
            }

            _behavioursEditor.SuppressInspectorGUI(() => base.OnInspectorGUI());
        }
        public new void OnDisable()
        {
            base.OnDisable();
        }

        public new void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}
