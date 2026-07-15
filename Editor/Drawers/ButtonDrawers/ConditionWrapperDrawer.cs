using SaintsField.Editor.Core;
using UnityEditor;

namespace SaintsField.Editor.Drawers.ButtonDrawers
{
    // this is a workaround... this ensures the decorated attribute's order won't get above these
    // this is a hack of the GetAttributes cache that secretly changed SaintsField attribute to top
    [CustomPropertyDrawer(typeof(DecButtonShowIfAttribute), true)]
    [CustomPropertyDrawer(typeof(AboveButtonShowIfAttribute), true)]
    [CustomPropertyDrawer(typeof(AboveButtonHideIfAttribute), true)]
    [CustomPropertyDrawer(typeof(BelowButtonShowIfAttribute), true)]
    [CustomPropertyDrawer(typeof(BelowButtonHideIfAttribute), true)]
    [CustomPropertyDrawer(typeof(PostFieldButtonShowIfAttribute), true)]
    [CustomPropertyDrawer(typeof(PostFieldButtonHideIfAttribute), true)]
    public class ConditionWrapperDrawer: SaintsPropertyDrawer
    {

    }
}
