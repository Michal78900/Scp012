namespace Scp012
{
    using Exiled.API.Interfaces;

    public class Translation : ITranslation
    {
        public string IHaveTo { get; private set; } = "I have to... I have to finish it.";
        public string IDontThink { get; private set; } = "I don't... think... I can do this.";
        public string IMust { get; private set; } = "I... I... must... do it.";
        public string NoChoice { get; private set; } = "I-I... have... no... ch-choice!";
        public string NoSense { get; private set; } = "This....this makes...no sense!";
        public string IsImpossible { get; private set; } = "No... this... this is... impossible!";
        public string CantBeCompleted { get; private set; } = "It can't... It can't be completed!";
    }
}
