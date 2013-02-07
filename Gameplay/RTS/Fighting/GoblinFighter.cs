namespace MHGameWork.TheWizards.RTS.Fighting
{
    class GoblinFighter
    {
        private Goblin goblin;
        private bool fighting;

        public GoblinFighter(Goblin goblin)
        {
            this.goblin = goblin;
        }
        public void Update()
        {
            if (!fighting)
                return;
        }
    }
}
