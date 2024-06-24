using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemScope : MonoBehaviour
{
    List<Item> ItemsInScope;

    private void Awake()
    {
        ItemsInScope = new List<Item>();
    }

    private void Update()
    {
        //Debug.Log(ItemsInScope.Count);
        if (ItemsInScope.Count == 0)
            return;

        if (Input.GetKeyDown(KeyCode.F))
        {
            PickUpAllItem();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Item>())
        {
            ItemsInScope.Add(other.GetComponent<Item>());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Item>())
        {
            ItemsInScope.Remove(other.GetComponent<Item>());
        }
    }

    public void PickUpAllItem()
    {
        List<Item> ItemsToRemove = new List<Item>(ItemsInScope);
        foreach (Item item in ItemsToRemove)
        {
            ItemEnum rand = GetRandomItem();
            GameManager.Instance.PickUpItem(rand);
            ItemsInScope.Remove(item);
            Destroy(item.gameObject);
        }
    }

    ItemEnum GetRandomItem()
    {
        ItemEnum[] itemValues = (ItemEnum[])System.Enum.GetValues(typeof(ItemEnum));
        int randomIndex = Random.Range(1, itemValues.Length);

        return itemValues[randomIndex];
    }
}
