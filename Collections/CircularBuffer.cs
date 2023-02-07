using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Reaper1121.SharpToolbox.Collections;

public interface ICircularBuffer<T> : IEnumerable<T>, ICollection {

    int Capacity { get; }

    void Push(T Arg_Item);
    bool TryPush(T Arg_Item, int Arg_Timeout);

    T Pop(int Arg_Timeout, CancellationToken Arg_CancellationToken);
    bool TryPop(out T Arg_Item, int Arg_Timeout, CancellationToken Arg_CancellationToken);

    T Peek(int Arg_Timeout, CancellationToken Arg_CancellationToken);
    bool TryPeek(out T Arg_Item, int Arg_Timeout, CancellationToken Arg_CancellationToken);

    void Clear();

}
