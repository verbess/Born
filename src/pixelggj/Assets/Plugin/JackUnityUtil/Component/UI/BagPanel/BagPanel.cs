using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace JackUtil {

    public class BagPanel : PanelBase {

        [HideInInspector]
        public SlotGoBase[] itemGoArray;

        public GameObject itemBD;
        public SlotGoBase itemPrefab;

        public Button closeButton;

        SlotGoBase itemInDraging;
        SlotGoBase itemDragTo;

        public event Action<int, int> DragItemTo;

        protected override GameObject defaultSelectedGo => itemGoArray?[0].gameObject;

        protected override void Awake() {

            base.Awake();

            closeButton.onClick.AddListener(() => {
                Close();
            });

        }

        public void Init(int _maxSlot) {

            itemGoArray = new SlotGoBase[_maxSlot];

            for (int i = 0; i < itemGoArray.Length; i += 1) {

                SlotGoBase _itemGo = Instantiate(itemPrefab, itemBD.transform);
                _itemGo.RenderItem(i, null, i.ToString());
                _itemGo.enterAction += OnEnterItem;
                _itemGo.exitAction += OnExitItem;
                _itemGo.endDragAction += OnEndDrag;

                itemGoArray[i] = _itemGo;

            }

        }

        void OnEnterItem(SlotGoBase _itemGo) {

            itemDragTo = _itemGo;

        }

        void OnExitItem() {

            itemDragTo = null;

        }

        void OnEndDrag(SlotGoBase _itemGo) {

            itemInDraging = _itemGo;

            if (itemDragTo == null) {

                itemInDraging.PutDefaultPosition();
                return;

            }

            int _currentIndex = itemInDraging.index;
            int _targetIndex = itemDragTo.index;

            // 默认调换位置 方法一:
            // itemInDraging.transform.localPosition = itemDragTo.defalutPos;
            // itemDragTo.transform.localPosition = itemInDraging.defalutPos;

            // 默认调换位置 方法二:
            if (_currentIndex > _targetIndex) {
                itemInDraging.transform.SetSiblingIndex(itemDragTo.index + 1);
                itemDragTo.transform.SetSiblingIndex(itemInDraging.index);
            } else {
                itemDragTo.transform.SetSiblingIndex(itemInDraging.index + 1);
                itemInDraging.transform.SetSiblingIndex(itemDragTo.index);
            }
            
            itemInDraging.index = _targetIndex;
            itemDragTo.index = _currentIndex;

            DragItemTo?.Invoke(_currentIndex, _targetIndex);

            // 调换位置 or 叠加物品 or 回到原位

        }


    }

}