using System;
using System.Reflection;
using SaintsField.Editor;
using SaintsField.Editor.UIToolkitElements;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.UIElements;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;
using Object = UnityEngine.Object;
using StateEditor = UnityEditor.Graphs.AnimationStateMachine.StateEditor;
using StateMachineBehaviorsEditor = UnityEditor.Graphs.AnimationStateMachine.StateMachineBehaviorsEditor;
using StateMachineInspector = UnityEditor.Graphs.AnimationStateMachine.StateMachineInspector;

namespace Editor.Playa.AnimatorEditorHijack
{
    internal class StateMachineBehaviorsEditorHijack: StateMachineBehaviorsEditor
    {
        private static readonly FieldInfo StateEditorBehavioursEditorField = typeof(StateEditor).GetField(
            "m_BehavioursEditor",
            BindingFlags.Instance | BindingFlags.NonPublic
        ) ?? throw new MissingFieldException(typeof(StateEditor).FullName, "m_BehavioursEditor");

        private static readonly FieldInfo StateMachineInspectorBehavioursEditorField =
            typeof(StateMachineInspector).GetField(
                "m_BehavioursEditor",
                BindingFlags.Instance | BindingFlags.NonPublic
            ) ?? throw new MissingFieldException(typeof(StateMachineInspector).FullName, "m_BehavioursEditor");

        private static readonly FieldInfo TargetField = typeof(StateMachineBehaviorsEditor).GetField(
            "m_Target",
            BindingFlags.Instance | BindingFlags.NonPublic
        ) ?? throw new MissingFieldException(typeof(StateMachineBehaviorsEditor).FullName, "m_Target");

        private static readonly FieldInfo SelectedObjectsContextField = typeof(StateMachineBehaviorsEditor).GetField(
            "m_SelectedObjectsContext",
            BindingFlags.Instance | BindingFlags.NonPublic
        ) ?? throw new MissingFieldException(typeof(StateMachineBehaviorsEditor).FullName,
            "m_SelectedObjectsContext");

        private static readonly FieldInfo BehaviourEditorsField = typeof(StateMachineBehaviorsEditor).GetField(
            "m_BehavioursEditor",
            BindingFlags.Instance | BindingFlags.NonPublic
        ) ?? throw new MissingFieldException(typeof(StateMachineBehaviorsEditor).FullName, "m_BehavioursEditor");

        private static readonly Type StateMachineBehaviourEditorType =
            typeof(StateEditor).Assembly.GetType(
                "UnityEditor.Graphs.AnimationStateMachine.StateMachineBehaviourEditor"
            ) ?? throw new TypeLoadException(
                "UnityEditor.Graphs.AnimationStateMachine.StateMachineBehaviourEditor"
            );

        private static readonly MethodInfo MoveUpMethod = GetBehaviourMenuMethod("MoveUp");
        private static readonly MethodInfo MoveDownMethod = GetBehaviourMenuMethod("MoveDown");
        private static readonly MethodInfo RemoveMethod = GetBehaviourMenuMethod("Remove");

        // ReSharper disable once MemberCanBePrivate.Global
        public StateMachineBehaviorsEditorHijack(AnimatorState state, UnityEditor.Editor host) : base(state, host)
        {
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public StateMachineBehaviorsEditorHijack(AnimatorStateMachine stateMachine, UnityEditor.Editor host) : base(stateMachine, host)
        {
        }

        public VisualElement CreateInspectorGUI()
        {
            VisualElement root = new VisualElement();
            UnityEditor.Editor[] displayedEditors = null;

            RefreshEditors();
            root.schedule.Execute(RefreshEditors).Every(150);
            // root.RegisterCallback<AttachToPanelEvent>(_ => UIToolkitUtils.LoopCheckOutOfScoopFoldout(root));
            return root;

            void RefreshEditors()
            {
                StateMachineBehaviour[] behaviours = effectiveBehaviours;
                if (behaviours == null)
                {
                    if (displayedEditors != null)
                    {
                        displayedEditors = null;
                        root.Clear();
                    }
                    return;
                }

                if (!IsEditorsValid(behaviours))
                {
                    BuildEditorList(behaviours);
                }

                UnityEditor.Editor[] editors =
                    (UnityEditor.Editor[])BehaviourEditorsField.GetValue(this);
                if (ReferenceEquals(displayedEditors, editors))
                {
                    return;
                }

                displayedEditors = editors;
                root.Clear();
                if (editors == null)
                {
                    return;
                }

                int totalLength = editors.Length;
                for (int index = 0; index < totalLength; index++)
                {
                    UnityEditor.Editor editor = editors[index];
                    if (editor == null || editor.target == null)
                    {
                        continue;
                    }

                    Object target = editor.target;

                    bool invalid = target.GetType().FullName ==
                                   "UnityEditor.Graphs.AnimationStateMachine.InvalidStateMachineBehaviour";
                    bool expanded = invalid || InternalEditorUtility.GetIsInspectorExpanded(target);

                    ComponentTitleMockElement foldout = new ComponentTitleMockElement(target)
                    {
                        value = expanded,
                    };

                    if (invalid)
                    {
                        foldout.RegisterValueChangedCallback(evt =>
                        {
                            if (!evt.newValue)
                            {
                                foldout.SetValueWithoutNotify(true);
                            }
                        });
                    }
                    else
                    {
                        foldout.RegisterValueChangedCallback(evt =>
                            InternalEditorUtility.SetIsInspectorExpanded(editor.target, evt.newValue));
                    }

                    foldout.ToggleActive.style.display = DisplayStyle.None;

                    Button contextMenuButton = foldout.ContextMenuButton;
                    int curIndex = index;
                    contextMenuButton.clicked += () =>
                    {
                        GenericDropdownMenu genericDropdownMenu = new GenericDropdownMenu();

                        MonoScript monoScript = SaintsEditor.GetMonoScript(target);
                        if (monoScript != null)
                        {
                            genericDropdownMenu.AddItem("Edit Script", false,
                                () => AssetDatabase.OpenAsset(monoScript));
                            genericDropdownMenu.AddItem("Reset", false, () => Unsupported.SmartReset(target));
                            genericDropdownMenu.AddSeparator("");
                        }

                        if (curIndex == 0)
                        {
                            genericDropdownMenu.AddDisabledItem("Move Up", false);
                        }
                        else
                        {
                            genericDropdownMenu.AddItem("Move Up", false, () => MoveUpMethod.Invoke(null, new object[] { new MenuCommand(target) }));
                        }

                        if (curIndex == totalLength - 1)
                        {
                            genericDropdownMenu.AddDisabledItem("Move Down", false);
                        }
                        else
                        {
                            genericDropdownMenu.AddItem("Move Down", false, () => MoveDownMethod.Invoke(null, new object[] { new MenuCommand(target) }));
                        }

                        genericDropdownMenu.AddItem("Remove", false, () => RemoveMethod.Invoke(null, new object[] { new MenuCommand(target) }));

                        genericDropdownMenu.DropDown(
                            contextMenuButton.worldBound,
                            contextMenuButton,
#if UNITY_6000_3_OR_NEWER
                            DropdownMenuSizeMode.Auto
#else
                            true
#endif
                        );
                    };

                    foldout.Add(new InspectorElement(editor));
                    root.Add(foldout);
                }
            }
        }

        private static MethodInfo GetBehaviourMenuMethod(string methodName)
        {
            return StateMachineBehaviourEditorType.GetMethod(
                methodName,
                BindingFlags.Static | BindingFlags.NonPublic,
                null,
                new[] { typeof(MenuCommand) },
                null
            ) ?? throw new MissingMethodException(StateMachineBehaviourEditorType.FullName, methodName);
        }

        public static StateMachineBehaviorsEditorHijack Wrap(StateEditor host)
        {
            return Wrap(host, StateEditorBehavioursEditorField);
        }

        public static StateMachineBehaviorsEditorHijack Wrap(StateMachineInspector host)
        {
            return Wrap(host, StateMachineInspectorBehavioursEditorField);
        }

        private static StateMachineBehaviorsEditorHijack Wrap(
            UnityEditor.Editor host,
            FieldInfo behavioursEditorField
        )
        {
            StateMachineBehaviorsEditor original =
                (StateMachineBehaviorsEditor)behavioursEditorField.GetValue(host);
            original?.OnDisable();
            original?.OnDestroy();

            StateMachineBehaviorsEditorHijack hijack = null;
            // ReSharper disable once ConvertSwitchStatementToSwitchExpression
            switch (host.target)
            {
                case AnimatorState asv:
                    hijack =
                        new StateMachineBehaviorsEditorHijack(asv, host);
                    break;
                case AnimatorStateMachine asm:
                    hijack =
                        new StateMachineBehaviorsEditorHijack(asm, host);
                    break;
            }

            behavioursEditorField.SetValue(host, hijack);
            hijack?.OnEnable();
            return hijack;
        }

        public void SuppressInspectorGUI(Action drawHostInspector)
        {
            object target = TargetField.GetValue(this);
            object selectedObjectsContext = SelectedObjectsContextField.GetValue(this);
            TargetField.SetValue(this, null);
            SelectedObjectsContextField.SetValue(this, null);
            try
            {
                drawHostInspector();
            }
            finally
            {
                TargetField.SetValue(this, target);
                SelectedObjectsContextField.SetValue(this, selectedObjectsContext);
            }
        }

        public void AddBehaviourButtonPublic() => AddBehaviourButton();
    }
}
