namespace SimpleObjectPool{
    public class ObjectPoolParameter{
        internal readonly int preLoadCount;
        internal readonly int maxPoolCount;

        public ObjectPoolParameter(int preLoadCount, int maxPoolCount){
            this.preLoadCount = preLoadCount;
            this.maxPoolCount = maxPoolCount;
        }
    }
}
