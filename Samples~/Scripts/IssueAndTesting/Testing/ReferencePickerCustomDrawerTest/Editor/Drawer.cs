using SaintsField.Samples.Scripts.IssueAndTesting.Testing.ReferencePickerCustomDrawerTest;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Samples.Scripts.IssueAndTesting.Testing.ReferencePickerCustomDrawerTest.Editor
{
    [CustomPropertyDrawer(typeof(ReferencePickerCustomDrawer.StructImpl))]
    public class Drawer : PropertyDrawer
    {
#if UNITY_2021_3_OR_NEWER && !SAINTSFIELD_UI_TOOLKIT_DISABLE
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new VisualElement
            {
                style =
                {
                    flexDirection = FlexDirection.Row,
                },
            };
            HelpBox info = new HelpBox(
                "Custom Drawer!", HelpBoxMessageType.Info)
            {
                style =
                {
                    flexGrow = 1,
                    flexShrink = 1,
                    minWidth = 0,
                },
            };
            root.Add(info);
            root.Add(new PropertyField(property.FindPropertyRelative(nameof(ReferencePickerCustomDrawer.StructImpl.myStruct)))
            {
                style =
                {
                    flexGrow = 1,
                    flexShrink = 1,
                    minWidth = 0,
                },
            });
            return root;
        }
#endif
    }
}
