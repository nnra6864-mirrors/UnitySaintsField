using System;
using UnityEngine;

namespace SaintsField.Samples.Scripts.IssueAndTesting.Issue
{
    public class Issue413ListAddSameRef : SaintsMonoBehaviour
    {
        [Serializable]
        public enum MyEnum
        {
            OptionA,
            OptionB
        }

        public interface MyInterface
        {

        }

        [Serializable]
        public class MyClass: MyInterface
        {
            public MyEnum MyEnum;
            public int Number;

            public MyClass()
            {

            }

            public MyClass(MyEnum myEnum, int number)
            {
                MyEnum= myEnum;
                Number= number;
            }

            // Other irrelevant code here.
        }

        [FieldDefaultExpand]
        public SaintsInterface<MyInterface>[] interfaces;
    }
}
