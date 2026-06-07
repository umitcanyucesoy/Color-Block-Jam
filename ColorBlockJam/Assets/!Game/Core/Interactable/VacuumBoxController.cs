using System.Collections.Generic;
using _Game.Enums;
using UnityEngine;

namespace _Game.Core.Interactable
{
    public class VacuumBoxController : MonoBehaviour, IVacuumBoxController
    {
        private readonly struct Entry
        {
            public readonly VacuumBox Box;
            public readonly ShapeColor Color;

            public Entry(VacuumBox box, ShapeColor color)
            {
                Box = box;
                Color = color;
            }
        }
        
        private static readonly Vector2Int[] Neighbors =
        {
            Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right
        };

        private readonly Dictionary<Vector2Int, Entry> _boxes = new();
        private readonly Dictionary<Vector2Int, List<VacuumBox>> _groups = new();
        private readonly Queue<Vector2Int> _frontier = new();

        public void Register(Vector2Int vacuumCell, VacuumBox box, ShapeColor color) => _boxes[vacuumCell] = new Entry(box, color);

        public void BuildGroups()
        {
            _groups.Clear();

            foreach (var pair in _boxes)
            {
                if (_groups.ContainsKey(pair.Key))
                    continue;

                var color = pair.Value.Color;
                var group = new List<VacuumBox>();

                _frontier.Clear();
                _frontier.Enqueue(pair.Key);
                _groups[pair.Key] = group;

                while (_frontier.Count > 0)
                {
                    var cell = _frontier.Dequeue();
                    if (_boxes[cell].Box)
                        group.Add(_boxes[cell].Box);

                    for (int i = 0; i < Neighbors.Length; i++)
                    {
                        var next = cell + Neighbors[i];
                        if (_groups.ContainsKey(next)) continue;

                        if (_boxes.TryGetValue(next, out var neighbor) && neighbor.Color == color)
                        {
                            _groups[next] = group;
                            _frontier.Enqueue(next);
                        }
                    }
                }
            }
        }

        public void PlaySwallow(Vector2Int vacuumCell)
        {
            if (!_groups.TryGetValue(vacuumCell, out var group))
                return;

            for (int i = 0; i < group.Count; i++)
                if (group[i])
                    group[i].AnimateSwallow();
        }

        public void Clear()
        {
            _boxes.Clear();
            _groups.Clear();
        }
    }
}
