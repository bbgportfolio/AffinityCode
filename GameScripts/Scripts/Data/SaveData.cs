using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Data
{
    [Serializable]
    public class SaveData : ISerializationCallbackReceiver
    {
        #region Runtime Only Variables

        [NonSerialized] public int Current = 1;

        #endregion

        #region Serialized Variables

        [SerializeField] private int _current;

        #endregion

        public void OnBeforeSerialize()
        {
            _current = Current;
        }

        public void OnAfterDeserialize()
        {
            if (_current == null) _current = 1;
            Current = _current;
        }
    }
}