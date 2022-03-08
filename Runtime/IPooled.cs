namespace SimpleObjectPool
{
    public interface IPooled
    {
        void OnRelease();
        void OnTake();
        void OnClear(); 
    }
}
