namespace EGFramework
{
    public class EGGenerate : EGModule
    {
        public override void Init()
        {

        }

        public T GenerateUI<T>(object data) where T : new()
        {
            T ui = new T();
            return ui;
        }
    }
}