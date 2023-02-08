namespace Reaper1121.SharpToolbox;

public interface IObservable<in T> where T : IObserver {

    void Subscribe(T Arg_Observer);
    void Unsubscribe(T Arg_Observer);

}

public interface IObservable {

    void Subscribe(IObserver Arg_Observer);
    void Unsubscribe(IObserver Arg_Observer);

}

public interface IWeakObservable<in T> where T : IObserver {

    void Subscribe(T Arg_Observer);
    void Unsubscribe(T Arg_Observer);

}

public interface IWeakObservable {

    void Subscribe(IObserver Arg_Observer);
    void Unsubscribe(IObserver Arg_Observer);

}