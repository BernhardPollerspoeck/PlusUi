using System;

namespace PlusUi.SourceGenerators
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class GenerateGenericWrapperAttribute : Attribute
    {
        // Simple marker attribute for generator
    }
}