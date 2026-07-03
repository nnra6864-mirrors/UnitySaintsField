#if UNITY_2021_3_OR_NEWER
using SaintsField.Editor.Utils;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace SaintsField.Editor.Playa.Renderer.Table
{
    public class TableCellFoldableElement: BindableElement, INotifyValueChanged<bool>
    {
        private class NoBubbleContainer : VisualElement, IBindable
        {
            public NoBubbleContainer()
            {
                RegisterCallback<ChangeEvent<bool>>(StopBoolChangeEvent);
            }

            private static void StopBoolChangeEvent(ChangeEvent<bool> evt)
            {
                evt.StopPropagation();
            }

// #if UNITY_2023_2_OR_NEWER
//             private static bool IsBoolEvent(EventBase evt)
//             {
//                 return $"{evt}" == "UnityEngine.UIElements.ChangeEvent`1[System.Boolean]";
//             }
//
//             // [EventInterest(new System.Type[] {typeof (SerializedPropertyBindEvent)})]
//             protected override void HandleEventBubbleUp(EventBase evt)
//             {
//                 if (IsBoolEvent(evt))
//                 {
//                     evt.StopPropagation();
//                     return;
//                 }
//                 base.HandleEventBubbleUp(evt);
//             }
// #else
//             public NoBubbleContainer()
//             {
//                 RegisterCallback<ChangeEvent<bool>>(StopBoolChangeEvent);
//             }
//
//             private static void StopBoolChangeEvent(ChangeEvent<bool> evt)
//             {
//                 evt.StopPropagation();
//             }
//
//             // // [EventInterest(new System.Type[] {typeof (SerializedPropertyBindEvent)})]
//             // protected override void ExecuteDefaultActionAtTarget(EventBase evt)
//             // {
//             //     if (IsBoolEvent(evt))
//             //     {
//             //         evt.StopPropagation();
//             //         return;
//             //     }
//             //     base.ExecuteDefaultActionAtTarget(evt);
//             // }
// #endif

            public IBinding binding { get; set; }
            public string bindingPath { get; set; }
        }

        public override VisualElement contentContainer { get; }

        private readonly Foldout _foldout;

        public TableCellFoldableElement()
        {
            hierarchy.Add(_foldout = new Foldout
            {
                value = true,
                style =
                {
                    display = DisplayStyle.None,
                    flexGrow = 0,
                    flexShrink = 0,
                    width = 2,
                },
            });
            hierarchy.Add(contentContainer = new NoBubbleContainer
            {
                style =
                {
                    flexGrow = 1,
                    flexShrink = 1,
                },
            });
            style.flexDirection = FlexDirection.Row;
        }

        public void ToggleDisplay(bool on)
        {
            UIToolkitUtils.SetDisplayStyle(_foldout, on? DisplayStyle.Flex: DisplayStyle.None);
        }

        private string _viewKeyId;

        public void SetViewKey(string id)
        {
            _viewKeyId = id;
            bool newValue = SessionState.GetBool(id, true);
            FoldoutAndHeightUpdate(newValue);
        }

        public void SetValueWithoutNotify(bool newValue)
        {
            FoldoutAndHeightUpdate(newValue);
            if (!string.IsNullOrEmpty(_viewKeyId))
            {
                SessionState.SetBool(_viewKeyId, newValue);
            }
        }

        private void FoldoutAndHeightUpdate(bool newValue)
        {
            _foldout.SetValueWithoutNotify(newValue);
            style.maxHeight = newValue ? StyleKeyword.Initial : EditorGUIUtility.singleLineHeight + 2;
        }

        public bool value
        {
            get => _foldout.value;
            set => _foldout.value = value;
        }
    }
}
#endif
