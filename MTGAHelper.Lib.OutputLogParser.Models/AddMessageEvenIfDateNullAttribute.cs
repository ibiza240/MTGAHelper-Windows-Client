using System;

namespace MTGAHelper.Lib.OutputLogParser.Models
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class AddMessageEvenIfDateNullAttribute : Attribute
    {
        public AddMessageEvenIfDateNullAttribute()
        {
        }
    }
}
