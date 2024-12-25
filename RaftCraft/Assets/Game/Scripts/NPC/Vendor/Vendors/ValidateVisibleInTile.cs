using System;
using Game.Scripts.Raft.BuildSystem;
using UnityEngine;

namespace Game.Scripts.NPC.Vendor.Vendors
{
    public class ValidateVisibleInTile : MonoBehaviour
    {
        [SerializeField] private TileBuild _parentBuild;

        private void Start()
        {
            transform.localScale = Vector3.zero;
            if (_parentBuild != null)
            {
                _parentBuild.OnBuyTile += OnBuyTile;
            }
        }

        private void OnBuyTile()
        {
            transform.localScale = Vector3.one;
        }

        private void OnDestroy()
        {
            if (_parentBuild != null)
            {
                _parentBuild.OnBuyTile -= OnBuyTile;
            }
        }
    }
}
