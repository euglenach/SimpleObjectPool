namespace SimpleObjectPool
{
    public class ObjectPoolParameter
    {
        internal int preLoadCount;
        internal int maxPoolCount;

        public ObjectPoolParameter(int preLoadCount, int maxPoolCount)
        {
            this.preLoadCount = preLoadCount;
            this.maxPoolCount = maxPoolCount;
        }
    }
}
