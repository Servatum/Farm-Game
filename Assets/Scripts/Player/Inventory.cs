using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class Inventory : MonoBehaviour
{
    public ItemSlotUI[] uiSlots;
    public ItemSlot[] slots;

    public GameObject inventoryWindow;
    public Transform dropPosition;

    [Header("Selected Item")]
    private ItemSlot selectedItem;
    private int selectedItemIndex;
    public TextMeshProUGUI selectedItemName;
    public TextMeshProUGUI selectedItemDescription;
    public TextMeshProUGUI selectedItemStatName;
    public TextMeshProUGUI selectedItemStatValue;
    public GameObject useButton;
    public GameObject equipButton;
    public GameObject unequipButton;
    public GameObject dropButton;

    private int curEquipIndex;

    //components
    private PlayerController controller;
    private PlayerNeeds needs;

    [Header("Events")]
    public UnityEvent onOpenInventory;
    public UnityEvent onCloseInventory;

    //singeton
    public static Inventory instance;
    
    void Awake()
    {
        instance = this;
        controller = GetComponent<PlayerController>();
        needs = GetComponent<PlayerNeeds>();
    }

    void Start()
    {
        inventoryWindow.SetActive(false);
        slots = new ItemSlot[uiSlots.Length];

        //initialize the slots
        for(int x = 0; x < slots.Length; x++)
        {
            slots[x] = new ItemSlot();
            uiSlots[x].index = x;
            uiSlots[x].Clear();
        }

        ClearSelectedItemWindow();
    }

    public void OnInventoryButton(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Started)
        {
            Toggle();
        }
    }

    public void Toggle()
    {
        if(inventoryWindow.activeInHierarchy)
        {
            inventoryWindow.SetActive(false);
            onCloseInventory.Invoke();
            controller.ToggleCursor(false);
        }
        else
        {
            inventoryWindow.SetActive(true);
            onOpenInventory.Invoke();
            ClearSelectedItemWindow();
            controller.ToggleCursor(true);
        }
    }

    public bool IsOpen()
    {
        return inventoryWindow.activeInHierarchy;
    }

    //adds the requested item to the player's inventory
    public void AddItem(ItemData item)
    {
        //does this item have a stack it can be added to?
        if(item.canStack)
        {
            ItemSlot slotToStackTo = GetItemStack(item);

            if(slotToStackTo != null)
            {
                slotToStackTo.quantity++;
                UpdateUI();
                return;
            }
        }

        ItemSlot emptySlot = GetEmptySlot();

        //do we have an empty slot for the item?
        if(emptySlot != null)
        {
            emptySlot.item = item;
            emptySlot.quantity = 1;
            UpdateUI();
            return;
        }

        //if the item can't stack and there are no empty slots - throw it away
        ThrowItem(item);
    }

    //spawns the item in front of the player
    void ThrowItem(ItemData item)
    {
        Instantiate(item.dropPrefab, dropPosition.position, Quaternion.Euler(Vector3.one * Random.value * 360.0f));
    }

    //updates the UI slots
    void UpdateUI()
    {
        for(int x = 0; x < slots.Length; x++)
        {
            if(slots[x].item != null)
            {
                uiSlots[x].Set(slots[x]);
            }
            else
            {
                uiSlots[x].Clear();
            }
        }
    }

    //returns the item slot that the requested item can be stacked on
    //returns null if there is no stack available
    ItemSlot GetItemStack(ItemData item)
    {
        for(int x = 0; x < slots.Length; x++)
        {
            if(slots[x].item == item && slots[x].quantity < item.maxStackAmount)
            {
                return slots[x];
            }
        }

        return null;
    }

    //returns an empty slot in the inventory
    //if there are no empty slots - return null
    ItemSlot GetEmptySlot()
    {
        for(int x = 0; x < slots.Length; x++)
        {
            if(slots[x].item == null)
            {
                return slots[x];
            }
        }

        return null;
    }

    public void SelectItem(int index)
    {
        if(slots[index].item == null)
        {
            return;
        }

        selectedItem = slots[index];
        selectedItemIndex = index;

        selectedItemName.text = selectedItem.item.displayName;
        selectedItemDescription.text = selectedItem.item.description;

        //set stat values and stat names
        selectedItemStatName.text = string.Empty;
        selectedItemStatValue.text = string.Empty;

        for(int x = 0; x < selectedItem.item.consumables.Length; x++)
        {
            selectedItemStatName.text += selectedItem.item.consumables[x].type.ToString() + "\n";
            selectedItemStatValue.text += selectedItem.item.consumables[x].value.ToString() + "\n";
        }

        useButton.SetActive(selectedItem.item.type == ItemType.Consumable);
        equipButton.SetActive(selectedItem.item.type == ItemType.Equipable && !uiSlots[index].equipped);
        unequipButton.SetActive(selectedItem.item.type == ItemType.Equipable && uiSlots[index].equipped);
        dropButton.SetActive(true);
    }

    void ClearSelectedItemWindow()
    {
        //clear the text elements
        selectedItem = null;
        selectedItemName.text = string.Empty;
        selectedItemDescription.text = string.Empty;
        selectedItemStatName.text = string.Empty;
        selectedItemStatValue.text = string.Empty;

        //disable buttons
        useButton.SetActive(false);
        equipButton.SetActive(false);
        unequipButton.SetActive(false);
        dropButton.SetActive(false);
    }

    public void OnUseButton()
    {
        if(selectedItem.item.type == ItemType.Consumable)
        {
            for(int x = 0; x < selectedItem.item.consumables.Length; x++)
            {
                switch(selectedItem.item.consumables[x].type)
                {
                    case ConsumableType.Health: needs.Heal(selectedItem.item.consumables[x].value); break;
                    case ConsumableType.Hunger: needs.Eat(selectedItem.item.consumables[x].value); break;
                    case ConsumableType.Thirst: needs.Drink(selectedItem.item.consumables[x].value); break;
                    case ConsumableType.Sleep: needs.Sleep(selectedItem.item.consumables[x].value); break;

                }
            }
        }

        RemoveSelectedItem();
    }

    //called when the "Equip" button is pressed
    public void OnEquipButton()
    {
        if(uiSlots[curEquipIndex].equipped)
        {
            Unequip(curEquipIndex);
        }

        uiSlots[selectedItemIndex].equipped = true;
        curEquipIndex = selectedItemIndex;
        EquipManager.instance.EquipNew(selectedItem.item);
        UpdateUI();

        SelectItem(selectedItemIndex);
    }

    //unequips the requested item
    void Unequip(int index)
    {
        uiSlots[index].equipped = false;
        EquipManager.instance.UnEquip();
        UpdateUI();

        if(selectedItemIndex == index)
        {
            SelectItem(index);
        }
    }

    //unequips the requested item
    public void OnUnequipButton()
    {
        Unequip(selectedItemIndex);
    }


    public void OnDropButton()
    {
        ThrowItem(selectedItem.item);
        RemoveSelectedItem();
    }

    void RemoveSelectedItem()
    {
        selectedItem.quantity--;

        if(selectedItem.quantity == 0)
        {
            if(uiSlots[selectedItemIndex].equipped == true)
            {
                Unequip(selectedItemIndex);
            }

            selectedItem.item = null;
            ClearSelectedItemWindow();
        }

        UpdateUI();
    }

    public void RemoveItem(ItemData item)
    {
        for(int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == item)
            {
                slots[i].quantity--;

                if (slots[i].quantity == 0)
                {
                    if (uiSlots[i].equipped == true)
                    {
                        Unequip(i);
                    }

                    slots[i].item = null;
                    ClearSelectedItemWindow();
                }
            }
        }
    }

    // does the player have "quantity" amount of "item"s?
    public bool HasItems(ItemData item, int quantity)
    {
        int amount = 0;

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == item)
            {
                amount += slots[i].quantity;
            }

            if (amount >= quantity)
            {
                return true;
            }
        }
            return false;
    }
}

public class ItemSlot
{
    public ItemData item;
    public int quantity;
}
