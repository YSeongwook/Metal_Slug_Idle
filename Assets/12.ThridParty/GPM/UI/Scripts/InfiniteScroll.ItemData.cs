namespace Gpm.Ui
{
    using System;
    using System.Collections.Generic;
    using UnityEngine.Events;

    public partial class InfiniteScroll
    {
        public class DataContext
        {
            public DataContext(InfiniteScrollData data, int index)
            {
                this.index = index;
                this.data = data;
            }

            internal InfiniteScrollData data; // 데이터 객체
            internal int index = -1; // 데이터 인덱스
            internal int itemIndex = -1; // 아이템 인덱스
            internal float offset = 0; // 오프셋
            internal bool needUpdateItemData = true; // 아이템 데이터 업데이트 필요 여부
            internal float scrollItemSize = 0; // 스크롤 아이템 크기
            internal InfiniteScrollItem itemObject; // 연결된 아이템 객체

            // 아이템 데이터 업데이트 필요 여부 반환
            public bool IsNeedUpdateItemData()
            {
                return needUpdateItemData;
            }

            // 아이템 연결 해제
            public void UnlinkItem(bool notifyEvent = false)
            {
                if (itemObject != null)
                {
                    itemObject.ClearData(notifyEvent);
                    itemObject = null;
                }

                itemIndex = -1;
            }

            // 데이터 업데이트
            public void UpdateData(InfiniteScrollData data)
            {
                this.data = data;
                needUpdateItemData = true;
            }

            // 아이템 크기 반환
            public float GetItemSize()
            {
                return scrollItemSize;
            }

            // 아이템 크기 설정
            public void SetItemSize(float value)
            {
                scrollItemSize = value;
            }
        }

        protected List<DataContext> dataList = new List<DataContext>(); // 데이터 컨텍스트 리스트
        protected int itemCount = 0; // 아이템 개수

        protected bool needUpdateItemList = true; // 아이템 리스트 업데이트 필요 여부

        protected int selectDataIndex = -1; // 선택된 데이터 인덱스
        protected Action<InfiniteScrollData> selectCallback = null; // 선택 콜백

        // 데이터의 인덱스 반환
        public int GetDataIndex(InfiniteScrollData data)
        {
            if (isInitialize == false)
            {
                Initialize();
            }

            return dataList.FindIndex((context) =>
            {
                return context.data.Equals(data);
            });
        }

        // 데이터 개수 반환
        public int GetDataCount()
        {
            return dataList.Count;
        }

        // 인덱스로 데이터 반환
        public InfiniteScrollData GetData(int index)
        {
            return dataList[index].data;
        }

        // 데이터 리스트 반환
        public List<InfiniteScrollData> GetDataList()
        {
            List<InfiniteScrollData> list = new List<InfiniteScrollData>();

            for (int index = 0; index < dataList.Count; index++)
            {
                list.Add(dataList[index].data);
            }
            return list;
        }

        // 활성화된 아이템 리스트 반환
        public List<InfiniteScrollData> GetItemList()
        {
            List<InfiniteScrollData> list = new List<InfiniteScrollData>();

            for (int index = 0; index < dataList.Count; index++)
            {
                if (dataList[index].itemIndex != -1)
                {
                    list.Add(dataList[index].data);
                }
            }
            return list;
        }

        // 아이템 개수 반환
        public int GetItemCount()
        {
            return itemCount;
        }

        // 데이터로부터 아이템 인덱스 반환
        public int GetItemIndex(InfiniteScrollData data)
        {
            var context = GetDataContext(data);
            return context.itemIndex;
        }

        // 선택 콜백 추가
        public void AddSelectCallback(Action<InfiniteScrollData> callback)
        {
            if (isInitialize == false)
            {
                Initialize();
            }

            selectCallback += callback;
        }

        // 선택 콜백 제거
        public void RemoveSelectCallback(Action<InfiniteScrollData> callback)
        {
            if (isInitialize == false)
            {
                Initialize();
            }

            selectCallback -= callback;
        }

        // 아이템 활성화 상태 변경 시 호출
        public void OnChangeActiveItem(int dataIndex, bool active)
        {
            onChangeActiveItem.Invoke(dataIndex, active);
        }

        // 데이터로부터 데이터 컨텍스트 반환
        protected DataContext GetDataContext(InfiniteScrollData data)
        {
            if (isInitialize == false)
            {
                Initialize();
            }

            return dataList.Find((context) =>
            {
                return context.data.Equals(data);
            });
        }

        // 아이템 인덱스로부터 데이터 컨텍스트 반환
        protected DataContext GetContextFromItem(int itemIndex)
        {
            if (isInitialize == false)
            {
                Initialize();
            }

            if (IsValidItemIndex(itemIndex) == true)
            {
                return GetItem(itemIndex);
            }
            else
            {
                return null;
            }
        }

        // 데이터 추가
        protected void AddData(InfiniteScrollData data)
        {
            DataContext addData = new DataContext(data, dataList.Count);
            InitFitContext(addData);

            dataList.Add(addData);

            CheckItemAfterAddData(addData);
        }

        // 데이터 추가 후 아이템 상태 확인
        private bool CheckItemAfterAddData(DataContext addData)
        {
            if (onFilter != null && onFilter(addData.data) == true)
            {
                return false;
            }

            int itemIndex = 0;
            if (itemCount > 0)
            {
                for (int dataIndex = addData.index - 1; dataIndex >= 0; dataIndex--)
                {
                    if (dataList[dataIndex].itemIndex != -1)
                    {
                        itemIndex = dataList[dataIndex].itemIndex + 1;
                        break;
                    }
                }
            }

            addData.itemIndex = itemIndex;
            itemCount++;

            for (int dataIndex = addData.index + 1; dataIndex < dataList.Count; dataIndex++)
            {
                if (dataList[dataIndex].itemIndex != -1)
                {
                    dataList[dataIndex].itemIndex++;
                }
            }
            
            needReBuildLayout = true;

            return true;
        }

        // 데이터 삽입
        protected void InsertData(InfiniteScrollData data, int insertIndex)
        {
            if (insertIndex < 0 || insertIndex > dataList.Count)
            {
                throw new ArgumentOutOfRangeException();
            }

            if (insertIndex < dataList.Count)
            {
                DataContext addData = new DataContext(data, insertIndex);
                InitFitContext(addData);
                
                for (int dataIndex = insertIndex; dataIndex < dataList.Count; dataIndex++)
                {
                    dataList[dataIndex].index++;
                }

                dataList.Insert(insertIndex, addData);

                CheckItemAfterAddData(addData);
            }
            else
            {
                AddData(data);
            }
        }

        // 데이터 컨텍스트 초기화
        protected void InitFitContext(DataContext context)
        {
            float size = layout.GetMainSize(defaultItemPrefabSize);
            if (dynamicItemSize == true)
            {
                float ItemSize = context.GetItemSize();
                if (ItemSize != 0)
                {
                    size = ItemSize;
                }
            }

            context.SetItemSize(size);
        }

        // 유효한 데이터 인덱스인지 확인
        protected bool IsValidDataIndex(int index)
        {
            return (index >= 0 && index < dataList.Count);
        }

        // 유효한 아이템 인덱스인지 확인
        protected bool IsValidItemIndex(int index)
        {
            return (index >= 0 && index < itemCount);
        }

        // 아이템 리스트 빌드
        protected void BuildItemList()
        {
            itemCount = 0;
            for (int i = 0; i < dataList.Count; i++)
            {
                DataContext context = dataList[i];

                if (onFilter != null && onFilter(context.data) == true)
                {
                    context.UnlinkItem(false);
                    continue;
                }
                context.itemIndex = itemCount;
                itemCount++;
            }

            needReBuildLayout = true;
        }
        
        // 아이템 선택 시 호출
        private void OnSelectItem(InfiniteScrollData data)
        {
            int dataIndex = GetDataIndex(data);
            if (IsValidDataIndex(dataIndex))
            {
                selectDataIndex = dataIndex;

                if (selectCallback != null)
                {
                    selectCallback(data);
                }
            }
        }
    }
}
