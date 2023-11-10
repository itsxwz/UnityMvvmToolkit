using System;

namespace UnityMvvmToolkit.Core.Attributes
{
    /// <summary>
    ///   <para>PreserveAttribute prevents byte code stripping from removing a class, method, field, or property.</para>
    /// </summary>
    [AttributeUsage(
        AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum |
        AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field |
        AttributeTargets.Event | AttributeTargets.Interface | AttributeTargets.Delegate, Inherited = false)]
    public class PreserveAttribute : Attribute
    {
        // 防止字节码剥离删除类、方法、字段、属性
    }
}