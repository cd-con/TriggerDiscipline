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

        public void FixedUpdate();
    }
}
