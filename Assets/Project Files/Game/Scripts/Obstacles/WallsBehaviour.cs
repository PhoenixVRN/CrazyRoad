using System;
using System.Collections.Generic;
using UnityEngine;
using Watermelon;
using Watermelon.SkinStore;

namespace Watermelon.VehicleFactory
{
    public class WallsBehaviour : MonoBehaviour
    {
        [SerializeField] List<Renderer> wallRenderers;

        private static WallsBehaviour instance;

        private void Awake()
        {
            instance = this;
        }

        private void OnEnable()
        {
            SkinsController.SkinSelected += OnNewProductSelected;
        }

        private void OnDisable()
        {
            SkinsController.SkinSelected -= OnNewProductSelected;
        }

        private void OnNewProductSelected(ISkinData skinData)
        {
            if (skinData is EnvironmentSkinData)
            {
                OnWallSkinSelected((EnvironmentSkinData)skinData);
            }
        }

        public static void Init()
        {
            EnvironmentSkinData environmentSkinData = (EnvironmentSkinData)SkinsController.Instance.GetSelectedSkin<EnvironmentSkinsDatabase>();

            instance.OnWallSkinSelected(environmentSkinData);
        }

        private void OnWallSkinSelected(EnvironmentSkinData skinData)
        {
            for (int i = 0; i < wallRenderers.Count; i++)
            {
                wallRenderers[i].material = skinData.Material;
            }
        }
    }
}