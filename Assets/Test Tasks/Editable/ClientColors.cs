using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TestTask.Editable
{
    public class ClientColors : MonoBehaviour
    {
        [SerializeField]
        private GameObject colorPrefab;
        [SerializeField]
        private Transform container;

        public void UpdateColors(List<Color> newColors)
        {
            while (container.childCount > 0)
            {
                DestroyImmediate(container.GetChild(0).gameObject);
            }

            foreach (Color color in newColors)
            {
                GameObject colorObject = Instantiate(colorPrefab, container);
                if (colorObject.TryGetComponent<Image>(out var image))
                {
                    image.color = color;
                }
            }
        }

        public void OnRequestColorsClicked()
        {
            ClientPacketsHandler.SendColorRequest();
        }

    }
}
