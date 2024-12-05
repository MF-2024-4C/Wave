using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Wave.Result
{
    public class ProgressBar : MonoBehaviour
    {
        [SerializeField] private GameObject _parent;
        [SerializeField] private Sprite _unCheckIcon;
        [SerializeField] private Sprite _checkIcon;
        [SerializeField] private Sprite _nowIcon;
        [SerializeField] private GameObject _iconObjectPrefab;
        [SerializeField] private Vector3 _defaultIconObjectScale = new Vector3(0.5f, 0.5f, 0.5f);
        [SerializeField] private Vector3 _selectIconObjectScale = new Vector3(1, 1, 1);

        private List<Image> _iconObjects = new List<Image>();
        private int _nowIndex = 0;

        public void CreateProgressBar(int max)
        {
            for (int i = 0; i < max; i++)
            {
                GameObject iconObject = Instantiate(_iconObjectPrefab, _parent.transform);
                iconObject.GetComponent<RectTransform>().localScale = _defaultIconObjectScale;
                Image image = iconObject.GetComponent<Image>();
                image.sprite = _unCheckIcon;
                _iconObjects.Add(image);
            }

            _nowIndex = 0;
        }

        /// <summary>
        /// プログレスバーの進捗を設定する
        /// </summary>
        /// <param name="index"></param>
        public void SetProgressBar(int index)
        {
            if (index < 0 || index >= _iconObjects.Count)
            {
                return;
            }

            _iconObjects[_nowIndex].sprite = _checkIcon;
            _iconObjects[_nowIndex].GetComponent<RectTransform>().localScale = _defaultIconObjectScale;
            _iconObjects[index].sprite = _nowIcon;
            _iconObjects[index].GetComponent<RectTransform>().localScale = _selectIconObjectScale;
            _nowIndex = index;
        }
        
        public void ResetProgressBar()
        {
            //オブジェクトを削除
            int count = _iconObjects.Count;
            for (int i = count - 1; i >= 0; i--)
            {
                Destroy(_iconObjects[i].gameObject);
                _iconObjects[i] = null;
            }
        }
    }
}
