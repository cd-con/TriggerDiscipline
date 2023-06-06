namespace TriggerDiscipline
{
    public interface IObject
    {
        public enum ObjectState
        {
            INTRO,
            LOOP,
            OUTRO
        }
        public void Compute();

        public void Update();
    }

    public class UIGeneric : IObject
    {
        public void Compute()
        {
            throw new System.NotImplementedException();
        }

        public void Update()
        {
            throw new System.NotImplementedException();
        }
    }
}
