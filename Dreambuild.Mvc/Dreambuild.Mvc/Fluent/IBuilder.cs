namespace Dreambuild.Mvc
{
    public interface IBuilder<T>
    {
        T Object { get; set; }
    }
}
