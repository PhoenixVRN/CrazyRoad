using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Watermelon.VehicleFactory
{
    public class WallSkinBehavior : MonoBehaviour
    {
        [SerializeField] Material material;
        public Material Material => material;
    }
}