#if SAINTSFIELD_SAINTS_EDITOR_SCRIPTABLE_RENDERER_APPLY
using UnityEditor;
using UnityEngine.Rendering.Universal;

namespace SaintsField.Editor.Playa.ScriptableRenderer
{
    [CustomEditor(typeof(ScriptableRendererData), true)]
    public class ApplySaintsScriptableRendererDataEditor: SaintsScriptableRendererDataEditor
    {
    }

    [CustomEditor(typeof(ScriptableRendererFeature), true)]
    public class ApplySaintsScriptableRendererFeatureEditor: SaintsScriptableRendererFeatureEditor
    {
    }
}
#endif
