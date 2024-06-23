using System;
using System.Dynamic;

namespace Quantum.Wave.Item
{
    public unsafe partial class ItemSystem : SystemMainThreadFilter<ItemSystem.Filter> , ISignalOnInteractCall, ISignalOnReleaseCall
    {
        public struct Filter
        {
            public EntityRef Entity;
            public Transform3D* Transform;
            public Interactor* Interactor;
            public ItemData* ItemData;
        }

        public override void Update(Frame f, ref Filter filter)
        {
        }

        public void OnInteractCall(Frame f, EntityRef interacter, EntityRef player)
        {
            InteractItem(f, interacter, player);
        }
        
        public void OnReleaseCall(Frame f, EntityRef interactor, EntityRef player)
        {
            ReleaseItem(f, interactor, player);
        }

        protected virtual void InteractItem(Frame f, EntityRef item, EntityRef player)
        {
            if (!CheckItemInteract(f, item, player)) return;
            
            if (!GetItemConfig(f, item, out BaseItemConfig config)) return;
            config.Execute(f, item, player);
            
            Log.Info($"player[{player.Index}] has interact Item[{item.Index}]");    
        }

        protected virtual void ReleaseItem(Frame f, EntityRef item, EntityRef player)
        {
            if(!CheckItemInteract(f, item, player)) return;
            if (!GetItemConfig(f, item, out BaseItemConfig config)) return;
            config.Release(f, item, player);
            
            Log.Info($"player[{player.Index}] has release Item[{item.Index}]");
        }

        private bool CheckItemInteract(Frame f, EntityRef item, EntityRef player)
        {
            //if(_entityRef != item) return false;
            if (!f.Unsafe.TryGetPointer<Interactor>(item, out var interacter))
            {
                return false;
            }

            interacter->OnInteract = true;

            return true;
        }
        
        private bool GetItemConfig<T>(Frame f, EntityRef itemRef, out T config) where T : AssetObject
        {
            config = null;
            if (!f.Unsafe.TryGetPointer<ItemData>(itemRef, out ItemData* itemData)) return false;
            var asset = f.FindAsset<T>(itemData->Config.Id);
            if (asset == null) return false;
            config = asset as T;
            return true;
        }
    }
}
