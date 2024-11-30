#pragma warning disable 649
using UnityEngine;

namespace Watermelon
{
    public class SavableItem : MonoBehaviour
    {
        [SerializeField] Item item;

        public Item Item { get => item; }
    }
}