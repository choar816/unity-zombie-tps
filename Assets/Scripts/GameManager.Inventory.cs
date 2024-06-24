using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public enum ItemEnum
{
    None,
    Ammo,
    Potion,
}

public class ItemInfo
{
    public ItemEnum type;
    public Sprite sprite;
    public Action action;

    public ItemInfo(ItemEnum _type, Sprite _sprite, Action _action)
    {
        type = _type;
        sprite = _sprite;
        action = _action;
    }
}

public partial class GameManager : MonoBehaviour
{
    public Transform ItemSlotContainer;
    public GameObject ItemSlotPrefab;
    public Transform ItemSlotStartPos;
    public GameObject ItemContextMenu;
    public Button ItemUseButton;
    public Button ItemDumpButton;

    public Dictionary<ItemEnum, ItemInfo> ItemData;
    List<ItemInfo> ItemInfoList;
    List<GameObject> ItemObjectList;
    GameObject SelectedItem;
    const int ITEM_COUNT = 8;

    void InitInventoryUI()
    {
        ItemObjectList = new List<GameObject>();
        ItemContextMenu.SetActive(false);
        SelectedItem = null;

        Vector3 startPos = ItemSlotStartPos.position;
        int diff = 120;
        for (int i = 0; i < ITEM_COUNT; ++i)
        {
            Vector3 nextPos = startPos + (Vector3.right * (i % 4) + Vector3.down * (i / 4)) * diff;
            ItemObjectList.Add(Instantiate(ItemSlotPrefab, nextPos, Quaternion.identity, ItemSlotContainer));
        }
    }

    void InitItemInfoList()
    {
        ItemInfoList = new List<ItemInfo>();
        for (int i = 0; i < ITEM_COUNT; ++i)
        {
            ItemInfoList.Add(ItemData[ItemEnum.None]);
        }
    }

    void UpdateInventoryUI()
    {
        for (int i = 0; i < ITEM_COUNT; ++i)
        {
            GameObject go = ItemObjectList[i];
            go.GetComponentInChildren<ItemImage>().SetImage(ItemInfoList[i].sprite);
        }
    }

    public void PickUpItem(ItemEnum itemEnum)
    {
        for (int i = 0; i < ITEM_COUNT; ++i)
        {
            // find the first empty slot
            if (ItemInfoList[i].type == ItemEnum.None)
            {
                ItemInfoList[i] = ItemData[itemEnum];
                UpdateInventoryUI();
                return;
            }
        }
    }

    public void SetSelectedItem(GameObject go)
    {
        SelectedItem = go;
    }

    void UseItem()
    {
        if (SelectedItem == null)
        {
            Debug.Log("UseItem: no selected item");
            return;
        }
        int itemIndex = ItemObjectList.IndexOf(SelectedItem);
        ItemInfoList[itemIndex].action();
        RemoveItem();
    }

    void RemoveItem()
    {
        if (SelectedItem == null)
        {
            Debug.Log("RemoveItem: no selected item");
            return;
        }
        ItemInfoList[ItemObjectList.IndexOf(SelectedItem)] = ItemData[ItemEnum.None];
        HideContextMenu();
        UpdateInventoryUI();
    }

    public void ShowContextMenu(Vector2 pressPosition)
    {
        ItemContextMenu.transform.position = pressPosition;
        ItemContextMenu.SetActive(true);
    }

    public void HideContextMenu()
    {
        ItemContextMenu.SetActive(false);
    }

    void InitItemData()
    {
        ItemData = new Dictionary<ItemEnum, ItemInfo>
        {
            { ItemEnum.None, new ItemInfo(ItemEnum.None, null, () => Debug.Log("used empty item")) },
            { ItemEnum.Ammo, new ItemInfo(ItemEnum.Ammo, GetSprite("Sprites/ammo"), Player.Instance.Reload) },
            { ItemEnum.Potion, new ItemInfo(ItemEnum.Potion, GetSprite("Sprites/potion"), () => Player.Instance.GainHP(10)) },
        };
    }

    Sprite GetSprite(string spritePath)
    {
        if (spritePath == string.Empty || spritePath == null)
            return null;

        Sprite sprite = Resources.Load<Sprite>(spritePath);
        if (sprite == null)
        {
            Debug.Log("File not found at: " + spritePath);
            return null;
        }
        return sprite;
    }
}

