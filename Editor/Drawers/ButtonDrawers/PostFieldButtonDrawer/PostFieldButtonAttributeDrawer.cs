using System;
using System.Collections.Generic;
using SaintsField.Editor.Drawers.ButtonDrawers.DecButtonDrawer;
using SaintsField.Interfaces;
using UnityEditor;
using UnityEngine;

namespace SaintsField.Editor.Drawers.ButtonDrawers.PostFieldButtonDrawer
{
#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.Editor.DrawerPriority(Sirenix.OdinInspector.Editor.DrawerPriorityLevel.WrapperPriority)]
#endif
    [CustomPropertyDrawer(typeof(PostFieldButtonAttribute), true)]
    public partial class PostFieldButtonAttributeDrawer : DecButtonAttributeDrawer
    {
        protected override IReadOnlyList<DecButtonShowIfAttribute> GetCurrentShowHide(IReadOnlyList<PropertyAttribute> attributes, ISaintsAttribute currentAttribute)
        {
            List<DecButtonShowIfAttribute> showControlAttributes = new List<DecButtonShowIfAttribute>(attributes.Count);

            foreach (PropertyAttribute attr in attributes)
            {
                if (ReferenceEquals(attr, currentAttribute))
                {
                    return showControlAttributes;
                }

                if (attr is DecButtonAttribute)
                {
                    showControlAttributes.Clear();
                    continue;
                }

                if (attr is PostFieldButtonShowIfAttribute showOrHide)  // this includes the HideIf (inherent)
                {
                    showControlAttributes.Add(showOrHide);
                }
            }
            // not found the decorated, skip all
            return Array.Empty<DecButtonShowIfAttribute>();
        }
    }
}
