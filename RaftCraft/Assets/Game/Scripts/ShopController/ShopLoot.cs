using System;
using System.Collections;
using Game.Scripts.ResourceController.Interfaces;
using Game.Scripts.Saves;
using UnityEngine;

namespace Game.Scripts.ShopController
{
    public class ShopLoot
    {
        [System.Serializable]
        public class LootData
        {
            public int Count { get; set; }
            public float Time { get; set; }
            public int CountOutput { get; set; }
        }

        public event Action OnChange; 

        public float TimeGlobal
        {
            get
            {
                var result = (Mathf.Clamp(_loot.Count - 1, 0, int.MaxValue)) * _item.DelayCell;
                if (_loot.Count > 0)
                {
                    result += CurrentTime;
                }
                return result;
            }
        }

        public int CountOutput => _loot.CountOutput * _item.CountOutput;

        private float _currentTime;
        
        private float CurrentTime
        {
            get => _currentTime;
            set
            {
                _currentTime = Mathf.Clamp(value, 0f, float.MaxValue);
                UpdateView();
            }
        }
        
        private readonly PriceItem _item;
        private LootData _loot;
        private ShopCell _cell;
        private IResourceService _resourceService;
        private GameSave _gameSave;
        public ShopLoot(PriceItem priceItem, ShopCell cell, IResourceService resourceService, GameSave gameSave)
        {
            _gameSave = gameSave;
            _resourceService = resourceService;
            _item = priceItem;
            _cell = cell;
        }

        private void UpdateView()
        {
            if (_cell == null)
            {
                return;
            }
            
            _cell.SetProgress(1f - (CurrentTime/_item.DelayCell));
            _cell.SetTotalTime(TimeGlobal);
            _cell.SetCountOutput(CountOutput);
            _cell.SetCurrentCount(_loot.Count);
            _loot.Time = CurrentTime;
            _gameSave.SetData($"LootData_{_item.Id}", _loot);
        }

        public void Add()
        {
            _resourceService.TryRemove(_item.Input, 1);
            _loot.Count++;
            if (CurrentTime == 0f)
            {
                CurrentTime = _item.DelayCell;
            }
            
            UpdateView();
        }

        public void Claim()
        {
            _resourceService.Add(_item.Output, CountOutput);
            _loot.CountOutput = 0;
            OnChange?.Invoke();
            UpdateView();
        }

        public void Update()
        {
            if (_loot.Count > 0)
            {
                CurrentTime -= Time.smoothDeltaTime;
                _cell.StartAnimation();
                if (CurrentTime <= 0f)
                {
                    _loot.Count--;
                    _loot.CountOutput++;
                    OnChange?.Invoke();
                    CurrentTime = _item.DelayCell;
                }
            }
        }

        public void Load()
        {
            _loot = _gameSave.GetData($"LootData_{_item.Id}", new LootData()
            {
                Count = 0,
                Time = _item.DelayCell,
                CountOutput = 0
            });
            CurrentTime = _loot.Time;
            _cell.OnClickEvent += Add;
            UpdateView();
            OnChange?.Invoke();
        }
        

        public void Save()
        {
            _loot.Time = CurrentTime;
            _gameSave.SetData($"LootData_{_item.Id}", _loot);
            _cell.OnClickEvent -= Add;
        }
    }
}