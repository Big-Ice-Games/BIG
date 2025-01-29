namespace BIG
{
    public interface IComponent
    {
        ulong ComponentType { get; }
    }

    public static class ComponentsUtils
    {
        public static ulong ToFlag(this IComponent[] components)
        {
            ulong flag = 0;

            for (int i = 0; i < components.Length; i++)
            {
                flag |= components[i].ComponentType;
            }

            return flag;
        }
    }
}