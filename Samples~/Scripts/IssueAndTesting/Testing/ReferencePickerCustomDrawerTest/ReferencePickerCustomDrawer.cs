using System;
using UnityEngine;

namespace SaintsField.Samples.Scripts.IssueAndTesting.Testing.ReferencePickerCustomDrawerTest
{
    public class ReferencePickerCustomDrawer : MonoBehaviour
    {
        public interface IRefInterface
        {
            public int TheInt { get; }
        }

        [Serializable]
        public struct StructImpl : IRefInterface
        {
            [field: SerializeField]
            public int TheInt { get; set; }
            public string myStruct;
        }

        [Serializable]
        public class ClassDirect: IRefInterface
        {
            [field: SerializeField, Range(0, 10)]
            public int TheInt { get; set; }
        }

        [SerializeReference, ReferencePicker]
        public IRefInterface myInterface;

    }
}
