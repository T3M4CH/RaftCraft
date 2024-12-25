using System;
using System.Linq;
using Game.Prefabs.NPC.Vendors;
using Game.Scripts.ShopVendor;
using UnityEngine;

namespace Game.Scripts.Tutorial.TutorialBuildVendor
{
    public abstract class TutorialBuildVendorBase : TutorialBuildVendor
    {

        [SerializeField] private ShopVendorController _shop;

        private void ShopOnOnBuyEvent()
        {
            OnComplete?.Invoke(this);
        }

        public override int Cost()
        {
            if (_shop == null)
            {
                return int.MaxValue;
            }

            return _shop.CountResource;
        }

        private void Start()
        {
            GetShop();
        }

        protected virtual void GetShop()
        {
            _shop = FindObjectsOfType<ShopVendorController>().Where(tile => tile.name == NameShop()).OrderBy(x => x.gameObject.activeSelf).First();
        }
        protected abstract string NameShop();

        public override void StartTutorial()
        {
            _shop.OnBuyEvent += ShopOnOnBuyEvent;
        }

        private void OnDestroy()
        {
            if (_shop != null)
            {
                _shop.OnBuyEvent -= ShopOnOnBuyEvent;
            }
        }

        public override Transform Target()
        {
            return _shop.PointAim;
        }
    }
}
