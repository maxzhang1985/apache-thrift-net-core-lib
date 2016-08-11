// ReSharper disable once CheckNamespace
// Dirty) hack to avoid problems with generated files and absent Serializable attribute
namespace System.Runtime.Serialization
{
    /// <summary>
    /// Fake attribute for Serializable to avoid errors
    /// </summary>
    public class Serializable : Attribute
    {
    }
}