using System.Collections.Generic;
using Core.Logic;
using Core.Logic.Grid;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(AbstractGrid))]
    public class GridEditor : UnityEditor.Editor
    {
    }
}
