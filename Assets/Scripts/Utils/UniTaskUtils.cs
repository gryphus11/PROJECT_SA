using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

public static class UniTaskUtils
{
    public static void CancelTokenSource(ref CancellationTokenSource cts)
    {
        if (cts == null)
            return;

        cts.Cancel();
        cts.Dispose();
        cts = null;
    }
}
