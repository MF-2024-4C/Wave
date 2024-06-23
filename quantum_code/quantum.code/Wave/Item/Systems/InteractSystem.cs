namespace Quantum.Wave.Interact
{
    public unsafe class InteractSystem : SystemMainThreadFilter<InteractSystem.Filter> , ISignalOnReleaseInteractor
    {
        public struct Filter
        {
            public EntityRef Entity;
            public Interactor* Interactor;
        }

        public override void Update(Frame f, ref Filter filter)
        {
            Interactor.CheckElapsedTime(f, filter.Entity, filter.Interactor);
            Interactor.CheckInteractCoolTime(f, filter.Interactor);
        }

        public void OnReleaseInteractor(Frame f, EntityRef interactor, EntityRef player)
        {
            if (!f.Unsafe.TryGetPointer<Interactor>(interactor, out var interactorPtr)) return;
            Interactor.Release(f, player, interactorPtr);
        }
    }
}