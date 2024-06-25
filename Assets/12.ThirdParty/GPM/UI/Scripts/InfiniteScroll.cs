namespace Gpm.Ui
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.Events;

    public partial class InfiniteScroll : MonoBehaviour
    {
        protected bool isInitialize = false; // 초기화 여부를 나타내는 플래그

        protected RectTransform content = null; // 스크롤뷰의 콘텐츠 영역

        private bool changeValue = false; // 값 변경 여부 플래그

        [Header("Event", order = 4)]
        public ChangeValueEvent onChangeValue = new ChangeValueEvent(); // 값이 변경될 때 발생하는 이벤트
        public ItemActiveEvent onChangeActiveItem = new ItemActiveEvent(); // 아이템 활성화 상태가 변경될 때 발생하는 이벤트
        public StateChangeEvent onStartLine = new StateChangeEvent(); // 스크롤뷰의 시작 상태 이벤트
        public StateChangeEvent onEndLine = new StateChangeEvent(); // 스크롤뷰의 끝 상태 이벤트

        private Predicate<InfiniteScrollData> onFilter = null; // 필터링 조건을 설정하는 델리게이트

        private void Awake()
        {
            PublicInitialize();
        }

        public void PublicInitialize()
        {
            Initialize();
        }
        
        private float GetSecondChildHeight()
        {
            RectTransform secondChildRect = transform.GetChild(1) as RectTransform;
            if (secondChildRect != null)
            {
                return secondChildRect.rect.height;
            }

            return 0;
        }

        protected void Initialize()
        {
            if (isInitialize == false)
            {
                scrollRect = GetComponent<ScrollRect>(); // ScrollRect 컴포넌트 가져오기
                content = scrollRect.content; // ScrollRect의 content 가져오기
                viewport = scrollRect.viewport; // ScrollRect의 viewport 가져오기

                CheckScrollAxis(); // 스크롤 축 확인
                ClearScrollContent(); // 스크롤 콘텐츠 초기화

                RectTransform itemTransform = (RectTransform)itemPrefab.transform;
                RectTransform chatTransform = (RectTransform)itemTransform.transform.GetChild(1);
                defaultItemPrefabSize = itemTransform.sizeDelta; // 기본 아이템 프리팹 크기 설정

                itemObjectList.Clear(); // 아이템 오브젝트 리스트 초기화
                dataList.Clear(); // 데이터 리스트 초기화

                scrollRect.onValueChanged.AddListener(OnValueChanged); // 스크롤 값 변경 리스너 추가

                CreateNeedItem(); // 필요한 아이템 생성

                CheckScrollData(); // 스크롤 데이터 확인

                isInitialize = true; // 초기화 완료 플래그 설정

                needReBuildLayout = true; // 레이아웃 재구성 필요 플래그 설정
            }
        }

        public void InsertData(InfiniteScrollData data, bool immediately = false)
        {
            if (isInitialize == false)
            {
                Initialize();
            }

            AddData(data);

            UpdateAllData(immediately);

            // 새로운 데이터 추가 시 자동으로 스크롤
            if (scrollRect != null)
            {
                Canvas.ForceUpdateCanvases();
                scrollRect.verticalNormalizedPosition = 0f;
            }
        }

        public void InsertData(InfiniteScrollData data, int insertIndex, bool immediately = false)
        {
            if (insertIndex < 0 || insertIndex > dataList.Count)
            {
                throw new ArgumentOutOfRangeException();
            }

            if (isInitialize == false)
            {
                Initialize();
            }

            InsertData(data, insertIndex);

            UpdateAllData(immediately);

            // 새로운 데이터 추가 시 자동으로 스크롤
            if (scrollRect != null)
            {
                Canvas.ForceUpdateCanvases();
                scrollRect.verticalNormalizedPosition = 0f;
            }
        }

        public void InsertData(InfiniteScrollData[] datas, bool immediately = false)
        {
            if (isInitialize == false)
            {
                Initialize();
            }

            foreach (InfiniteScrollData data in datas)
            {
                AddData(data);
            }

            UpdateAllData(immediately);

            // 새로운 데이터 추가 시 자동으로 스크롤
            if (scrollRect != null)
            {
                Canvas.ForceUpdateCanvases();
                scrollRect.verticalNormalizedPosition = 0f;
            }
        }

        public void InsertData(InfiniteScrollData[] datas, int insertIndex, bool immediately = false)
        {
            if (insertIndex < 0 || insertIndex > dataList.Count)
            {
                throw new ArgumentOutOfRangeException();
            }

            if (isInitialize == false)
            {
                Initialize();
            }

            foreach (InfiniteScrollData data in datas)
            {
                InsertData(data, insertIndex++);
            }

            UpdateAllData(immediately);

            // 새로운 데이터 추가 시 자동으로 스크롤
            if (scrollRect != null)
            {
                Canvas.ForceUpdateCanvases();
                scrollRect.verticalNormalizedPosition = 0f;
            }
        }

        public void RemoveData(InfiniteScrollData data, bool immediately = false)
        {
            if (isInitialize == false)
            {
                Initialize();
            }

            int dataIndex = GetDataIndex(data);

            RemoveData(dataIndex, immediately);
        }

        public void RemoveData(int dataIndex, bool immediately = false)
        {
            if (isInitialize == false)
            {
                Initialize();
            }

            if (IsValidDataIndex(dataIndex) == true)
            {
                selectDataIndex = -1;

                int removeShowIndex = -1;
                
                if(dataList[dataIndex].itemIndex != -1)
                {
                    removeShowIndex = dataList[dataIndex].itemIndex;
                }
                dataList[dataIndex].UnlinkItem(true);
                dataList.RemoveAt(dataIndex);
                for(int i= dataIndex; i< dataList.Count;i++)
                {
                    dataList[i].index--;

                    if(removeShowIndex != -1)
                    {
                        if (dataList[i].itemIndex != -1)
                        {
                            dataList[i].itemIndex--;
                        }
                    }
                }

                if (removeShowIndex != -1)
                {
                    if (removeShowIndex < firstItemIndex)
                    {
                        firstItemIndex--;
                    }
                    if (removeShowIndex < lastItemIndex)
                    {
                        lastItemIndex--;
                    }

                    itemCount--;
                }

                needReBuildLayout = true;

                UpdateAllData(immediately);
            }
        }

        public void ClearData(bool immediately = false)
        {
            if (isInitialize == false)
            {
                Initialize();
            }

            itemCount = 0;
            selectDataIndex = -1;

            dataList.Clear();
            lineLayout.Clear();
            layoutSize = 0;
            lineCount = 0;

            ClearItemsData();

            lastItemIndex = 0;
            firstItemIndex = 0;

            showLineIndex = 0;
            showLineCount = 0;

            isStartLine = false;
            isEndLine = false;

            needUpdateItemList = true;
            needReBuildLayout = true;
            isUpdateArea = true;

            onFilter = null;

            ClearScrollContent();

            cachedData.Clear();

            UpdateAllData(immediately);
        }

        public void Clear()
        {
            if (isInitialize == false)
            {
                Initialize();
            }

            itemCount = 0;
            selectDataIndex = -1;
            dataList.Clear();
            lineLayout.Clear();
            layoutSize = 0;
            lineCount = 0;

            ClearItems();

            lastItemIndex = 0;
            firstItemIndex = 0;

            showLineIndex = 0;
            showLineCount = 0;

            isStartLine = false;
            isEndLine = false;

            needUpdateItemList = true;
            needReBuildLayout = true;
            isUpdateArea = true;

            onFilter = null;

            cachedData.Clear();

            ClearScrollContent();
        }

        public void UpdateData(InfiniteScrollData data)
        {
            if (isInitialize == false)
            {
                Initialize();
            }

            var context = GetDataContext(data);
            if (context != null)
            {
                context.UpdateData(data);

                needReBuildLayout = true;
            }
        }

        public void UpdateAllData(bool immediately = true)
        {
            if (isInitialize == false)
            {
                Initialize();
            }

            needReBuildLayout = true;
            isUpdateArea = true;

            CreateNeedItem();

            if (immediately == true)
            {
                UpdateShowItem(true);
            }
        }

        public void SetFilter(Predicate<InfiniteScrollData> onFilter)
        {
            this.onFilter = onFilter;
            needUpdateItemList = true;
        }

        public float GetViewportSize()
        {
            return layout.GetMainSize(viewport);
        }

        public float GetContentSize()
        {
            UpdateContentSize();

            return layout.GetMainSize(content);
        }

        public float GetContentPosition()
        {
            return layout.GetAxisPosition(content);
        }

        public void ResizeScrollView()
        {
            if (isInitialize == false)
            {
                Initialize();
            }

            UpdateContentSize();
        }
        
        public float GetItemPosition(int itemIndex)
        {
            float distance = GetItemDistance(itemIndex);

            return -layout.GetAxisPostionFromOffset(distance);
        }

        public void RefreshScroll()
        {
            if (isInitialize == false)
            {
                Initialize();
            }

            if (needUpdateItemList == true)
            {
                BuildItemList();

                needUpdateItemList = false;
            }
            if (NeedUpdateItem() == true)
            {
                UpdateShowItem();
            }
        }

        protected float GetCrossSize()
        {
            return layout.GetCrossSize(content.rect);
        }

        protected void ResizeContent()
        {
            cachedData.contentSize = GetItemTotalSize();
            content.sizeDelta = layout.GetAxisVector(-layout.padding, cachedData.contentSize);
        }

        protected void UpdateContentSize()
        {
            if (needReBuildLayout == true)
            {
                BuildLayout();
                needReBuildLayout = false;
            }
        }

        protected bool NeedUpdateItem()
        {
            CheckScrollData();

            if (needReBuildLayout == true ||
                isRebuildLayout == true ||
                isUpdateArea == true)
            {
                return true;
            }

            return false;
        }

        protected bool IsShowBeforePosition(float position, float contentPosition)
        {
            float viewPosition = position - contentPosition;
            if (viewPosition < 0)
            {
                return true;
            }

            return false;
        }

        protected bool IsShowAfterPosition(float position, float contentPosition, float viewportSize)
        {
            float viewPosition = position - contentPosition;
            if (viewPosition >= viewportSize)
            {
                return true;
            }

            return false;
        }

        private void Update()
        {
            if (isInitialize == true)
            {
                RefreshScroll();
            }
        }

        private void OnValidate()
        {
            layout.SetDefaults();
        }


        [Serializable]
        public class ChangeValueEvent : UnityEvent<int, int, bool, bool>
        {
            public ChangeValueEvent()
            {
            }
        }

        [Serializable]
        public class ItemActiveEvent : UnityEvent<int, bool>
        {
            public ItemActiveEvent()
            {
            }
        }

        [Serializable]
        public class StateChangeEvent : UnityEvent<bool>
        {
            public StateChangeEvent()
            {
            }
        }
    }
}
