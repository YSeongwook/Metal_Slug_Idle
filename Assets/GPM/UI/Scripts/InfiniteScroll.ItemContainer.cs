namespace Gpm.Ui
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public partial class InfiniteScroll
    {
        [Header("Scroll Item", order = 2)]
        public int needItemCount = 0; // 필요한 아이템 수

        public InfiniteScrollItem itemPrefab = null; // 아이템 프리팹

        public bool dynamicItemSize = false; // 동적 아이템 크기 사용 여부

        private const float NEED_MORE_ITEM_RATE = 2; // 추가 아이템 비율

        private Vector2 defaultItemPrefabSize = Vector2.zero; // 기본 아이템 프리팹 크기

        private List<InfiniteScrollItem> itemObjectList = new List<InfiniteScrollItem>(); // 아이템 오브젝트 리스트

        // 주어진 인덱스의 아이템 크기 반환
        public float GetItemSize(int itemIndex)
        {
            float size = 0;

            if (dynamicItemSize == true)
            {
                if (itemIndex < itemCount)
                {
                    DataContext context = GetContextFromItem(itemIndex);
                    if (context != null)
                    {
                        size = context.GetItemSize();
                    }
                }
            }
            else
            {
                size = layout.GetMainSize(defaultItemPrefabSize);
            }

            return size;
        }

        // 동적 아이템 크기 사용 여부 반환
        public bool IsDynamicItemSize()
        {
            return dynamicItemSize;
        }

        // 새로운 아이템 생성
        private InfiniteScrollItem CreateItem()
        {
            InfiniteScrollItem itemObject = Instantiate(itemPrefab, content, false);

            itemObject.Initalize(this, itemObjectList.Count);
            itemObject.SetActive(false, false);

            itemObject.SetAxis(cachedData.anchorMin, cachedData.anchorMax, cachedData.itemPivot);

            itemObject.AddSelectCallback(OnSelectItem);
            
            RectTransform itemTransform = itemObject.rectTransform;
            itemTransform.sizeDelta = layout.GetAxisVector(layout.GetMainSize(itemTransform.sizeDelta));

            itemObjectList.Add(itemObject);

            return itemObject;
        }

        // 필요한 아이템 크기 반환
        private float GetNeedSize()
        {
            return layout.GetMainSize(viewport) * NEED_MORE_ITEM_RATE;
        }

        // 필요한 만큼의 아이템 생성
        private void CreateNeedItem()
        {
            for (int itemNumber = itemObjectList.Count; itemNumber < needItemCount; itemNumber++)
            {
                CreateItem();
            }
        }

        // 모든 아이템 데이터 초기화
        private void ClearItemsData()
        {
            for (int index = 0; index < itemObjectList.Count; ++index)
            {
                itemObjectList[index].ClearData(false);
            }
        }

        // 모든 아이템 제거
        private void ClearItems()
        {
            ClearItemsData();
            for (int index = 0; index < itemObjectList.Count; ++index)
            {
                itemObjectList[index].Clear();
                GameObject.Destroy(itemObjectList[index].gameObject);
            }
            itemObjectList.Clear();
        }

        // 주어진 컨텍스트에 해당하는 아이템 반환
        private InfiniteScrollItem PullItem(DataContext context)
        {
            InfiniteScrollItem item = context.itemObject;

            if (item == null || item.GetDataIndex() != context.index)
            {
                context.itemObject = null;
                int itemObjectIndex = GetItemIndexFromDataIndex(context.index, true);
                if (itemObjectIndex == -1)
                {
                    item = CreateItem();
                }
                else
                {
                    item = itemObjectList[itemObjectIndex];
                }
            }

            return item;
        }

        // 주어진 데이터 인덱스에서 아이템 인덱스를 찾음 (빈 인덱스 찾기 옵션 포함)
        private int GetItemIndexFromDataIndex(int dataIndex, bool findEmptyIndex = false)
        {
            int emptyIndex = -1;
            for (int index = 0; index < itemObjectList.Count; ++index)
            {
                if (itemObjectList[index].GetDataIndex() == dataIndex)
                {
                    return index;
                }

                if (findEmptyIndex == true)
                {
                    if (emptyIndex == -1 && itemObjectList[index].IsActive() == false)
                    {
                        emptyIndex = index;
                    }
                }
            }

            return emptyIndex;
        }
        
        // 아이템 크기 업데이트 시 호출
        internal void OnUpdateItemSize(DataContext context)
        {
            if (dynamicItemSize == true)
            {
                if (context.itemObject != null)
                {
                    UpdateAllData(false);
                }

                needReBuildLayout = true;
            }
        }
    }
}
