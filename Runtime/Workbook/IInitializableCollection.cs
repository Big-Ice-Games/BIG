#if BIG_WORKBOOK

// Copyright (c) 2025, Big Ice Games
// All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;

namespace BIG.Workbook
{
  public interface IInitializableCollection
  {
    void Initialize(KeyValuePair<Type, IList> data);
    string OutputPath { get; }
  }
}

#endif