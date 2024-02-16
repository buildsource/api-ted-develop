namespace TED.API.Annotations;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
sealed class IncludeInSwaggerAttribute : Attribute
{
    public IncludeInSwaggerAttribute()
    {
    }
}