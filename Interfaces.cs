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
        public void Intro();

        public void Update();

        public void Outro();
    }

}
