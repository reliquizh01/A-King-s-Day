using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections.ObjectModel;

namespace Utilities
{
    public class UtilitiesControlActionNames
    {
        public static readonly IList<string> actionNames = new ReadOnlyCollection<string>
         (new List<string> {
            INCREASE_COUNT_INCREMENT
        });

        public const string INCREASE_COUNT_INCREMENT = "INCREASE_COUNT_INCREMENT";
    }
}
