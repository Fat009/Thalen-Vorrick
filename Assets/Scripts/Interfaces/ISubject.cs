namespace CardGame
{
    public interface ISubject
    {
        void Attach(IObserver observer);
        void Detach(IObserver observer);
        void Notify(Card card, CardEvent cardEvent);
    }
}
