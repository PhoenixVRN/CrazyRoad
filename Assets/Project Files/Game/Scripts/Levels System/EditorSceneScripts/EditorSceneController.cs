#pragma warning disable 649

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Watermelon
{
    public class EditorSceneController : MonoBehaviour
    {

#if UNITY_EDITOR
        private static EditorSceneController instance;
        public static EditorSceneController Instance { get => instance; }

        public Vector3 FinishPosition { get => finish.transform.position; set => finish.transform.position = value; }

        [SerializeField] private GameObject container;
        [SerializeField] private GameObject finish;
        [SerializeField] private GameObject spawnerTool;

        public EditorSceneController()
        {
            instance = this;
        }

        //used when user spawns objects by clicking on object name in level editor
        public void Spawn(GameObject prefab,Vector3 defaultPosition)
        {
            GameObject gameObject = Instantiate(prefab, defaultPosition, Quaternion.identity, container.transform);
            gameObject.name = prefab.name + " ( Child # " + container.transform.childCount + ")";
            SelectGameObject(gameObject);
        }

        //used when level loads in level editor
        public void Spawn(PropSave tempItemSave, GameObject prefab)
        {
            GameObject gameObject = Instantiate(prefab, tempItemSave.Position, Quaternion.Euler(tempItemSave.Rotation), container.transform);
            gameObject.name = prefab.name + "(el # " + container.transform.childCount + ")";
            SelectGameObject(gameObject);
        }

        public void SelectGameObject(GameObject selectedGameObject)
        {
            Selection.activeGameObject = selectedGameObject;
        }

        public void SelectFinishGameObject()
        {
            Selection.activeGameObject = finish;
        }

        public void SelectSpawnerTool()
        {
            Selection.activeGameObject = spawnerTool;
        }

        public void Clear()
        {
            for (int i = container.transform.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(container.transform.GetChild(i).gameObject);
            }
        }

        public PropSave[] GetProps()
        {
            SavableItem[] savableItems = FindObjectsOfType<SavableItem>();
            List<PropSave> result = new List<PropSave>();

            for (int i = 0; i < savableItems.Length; i++)
            {
                result.Add(HandleParse(savableItems[i]));
            }

            return result.ToArray();
        }

        private PropSave HandleParse(SavableItem savableItem)
        {
            return new PropSave(savableItem.Item, savableItem.gameObject.transform.position, savableItem.gameObject.transform.rotation.eulerAngles);
        }

#endif
    }
}
