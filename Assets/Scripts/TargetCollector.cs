using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TargetCollector : MonoBehaviour
{
    public IReadOnlyList<Transform> Targets => _targets;

    private List<Transform> _targets = new();

    public void PutTarget(Transform target)
    {
        _targets.Add(target);
    }

    public bool TryGetTarget(out Transform target)
    {
        target = null;

        if (_targets.Count > 0)
        {
            target = _targets.FirstOrDefault();

            if (target != null)
            {
                _targets.Remove(target);

                return true;
            }

        }

        return false;
    }
}
