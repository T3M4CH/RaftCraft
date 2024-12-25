public interface IStartableElement
{
    int Priority { get; }
    void Execute();
}
