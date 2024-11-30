#pragma warning disable 649

using UnityEngine;

namespace Watermelon
{
    [System.Serializable]
    public class PropSave
    {
        [SerializeField] private Item prop;
        [SerializeField] private Vector3 position;
        [SerializeField] private Vector3 rotation;

        public Item Prop { get => prop; }
        public Vector3 Position { get => position; }
        public Vector3 Rotation { get => rotation; }

        public PropSave(Item prop, Vector3 position, Vector3 rotation)
        {
            this.prop = prop;
            this.position = position;
            this.rotation = rotation;
        }
    }
}