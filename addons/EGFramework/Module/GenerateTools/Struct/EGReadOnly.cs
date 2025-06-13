namespace EGFramework{
    public interface IEGReadOnlyString{
        public string GetString();
    }
    public struct EGReadOnlyString : IEGReadOnlyString{
        public string Value { get; private set; }
        public EGReadOnlyString(string value)
        {
            Value = value;
        }

        public string GetString()
        {
            return Value;
        }
    }
}