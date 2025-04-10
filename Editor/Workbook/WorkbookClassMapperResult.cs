﻿#if BIG_WORKBOOK || UNITY_EDITOR

// Copyright (c) 2025, Big Ice Games
// All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;

namespace BIG.Editor.Workbook
{
    public class WorkbookClassMapperResult : IEnumerable<KeyValuePair<Type, IList>>
    {
        private readonly Dictionary<Type, IList> _result;

        public IList this[Type t] => _result[t];

        public WorkbookClassMapperResult(Dictionary<Type, IList> result)
        {
            _result = result;
        }

        public IEnumerator<KeyValuePair<Type, IList>> GetEnumerator()
        {
            foreach (KeyValuePair<Type, IList> keyValuePair in _result)
            {
                yield return keyValuePair;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
#endif