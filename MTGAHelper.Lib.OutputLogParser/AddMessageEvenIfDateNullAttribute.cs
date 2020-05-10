using System;

namespace MTGAHelper.Lib.OutputLogParser
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    internal sealed class AddMessageEvenIfDateNullAttribute : Attribute
    {
        public AddMessageEvenIfDateNullAttribute()
        {
        }
    }
}
