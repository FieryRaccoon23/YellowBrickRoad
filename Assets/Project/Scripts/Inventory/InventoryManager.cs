using UnityEngine;

namespace BluMarble.Inventory
{
    public class InventoryManager : BluMarble.Singleton.Singleton<InventoryManager>
    {
        private InventoryStorage m_InventoryStorage;

        public override void PerformInit()
        {
            m_InventoryStorage = GetComponent<InventoryStorage>();
            m_InventoryStorage.PerformInit();
        }
    }
}
