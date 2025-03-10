using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class UIInventory : MonoBehaviour
{
    public ItemSlot[] slots;

    public Transform slotPanel;
    public Transform dropPosition;

    [Header("Selected Item")]
    private ItemSlot selectedItem;
    private int selectedItemIndex;
    public TextMeshProUGUI selectedItemText;
    public TextMeshProUGUI selectedItemStat;

    private bool canUse;
    private bool canEquip;
    private bool canUnEquip;
    private bool canDrop;

    private int curEquipIndex;
    private int selectedIndex = 0;

    private PlayerController controller;
    private PlayerCondition condition;

    void Start()
    {
        controller = CharacterManager.Instance.Player.controller;
        condition = CharacterManager.Instance.Player.condition;
        dropPosition = CharacterManager.Instance.Player.dropPosition;

        CharacterManager.Instance.Player.addItem += AddItem;

        slots = new ItemSlot[slotPanel.childCount - 1];

        for (int i = 0; i < slots.Length; i++)
        {
            slots[i] = slotPanel.GetChild(i + 1).GetComponent<ItemSlot>();
            slots[i].index = i;
            slots[i].inventory = this;
            slots[i].Clear();
        }

        ClearSelectedItemWindow();

        SelectItem(0);
    }

    void ClearSelectedItemWindow()
    {
        selectedItem = null;

        selectedItemText.transform.parent.gameObject.SetActive(false);
        selectedItemStat.transform.parent.gameObject.SetActive(false);

        selectedItemText.text = string.Empty;
        selectedItemStat.text = string.Empty;
    }

    void Update()
    {
        if (Keyboard.current.digit1Key.wasPressedThisFrame) SelectItem(0);
        if (Keyboard.current.digit2Key.wasPressedThisFrame) SelectItem(1);
        if (Keyboard.current.digit3Key.wasPressedThisFrame) SelectItem(2);
        if (Keyboard.current.digit4Key.wasPressedThisFrame) SelectItem(3);
        if (Keyboard.current.digit5Key.wasPressedThisFrame) SelectItem(4);
    }

    public void OnSelectItem(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        int key = (int)context.ReadValue<float>() - 1;
        if (key >= 0 && key < slots.Length)
        {
            SelectItem(key);
        }
    }

    // PlayerController 먼저 수정

    public void AddItem()
    {
        ItemData data = CharacterManager.Instance.Player.itemData;

        if (data.canStack)
        {
            ItemSlot slot = GetItemStack(data);
            if (slot != null)
            {
                slot.quantity++;
                UpdateUI();
                CharacterManager.Instance.Player.itemData = null;

                SelectItem(selectedIndex);
                return;
            }
        }

        ItemSlot emptySlot = GetEmptySlot();

        if (emptySlot != null)
        {
            emptySlot.item = data;
            emptySlot.quantity = 1;
            UpdateUI();
            CharacterManager.Instance.Player.itemData = null;

            SelectItem(selectedIndex);
            return;
        }

        ThrowItem(data);
        CharacterManager.Instance.Player.itemData = null;
    }

    public void UpdateUI()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item != null)
            {
                slots[i].Set();
            }
            else
            {
                slots[i].Clear();
            }
        }
    }

    ItemSlot GetItemStack(ItemData data)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == data && slots[i].quantity < data.maxStackAmount)
            {
                return slots[i];
            }
        }
        return null;
    }

    ItemSlot GetEmptySlot()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == null)
            {
                return slots[i];
            }
        }
        return null;
    }

    // Player 스크립트 먼저 수정
    public void ThrowItem(ItemData data)
    {
        Instantiate(data.dropPrefab, dropPosition.position, Quaternion.Euler(Vector3.one * Random.value * 360));
    }


    // ItemSlot 스크립트 먼저 수정
    public void SelectItem(int index)
    {
        selectedIndex = index;
        
        foreach (ItemSlot slot in slots)
        {
            slot.outline.enabled = false;
        }
        slots[selectedIndex].outline.enabled = true;

        if (slots[index].item == null) return;

        selectedItem = slots[index];
        selectedItemIndex = index;

        selectedItemText.transform.parent.gameObject.SetActive(true);
        selectedItemText.text = $"{selectedItem.item.displayName} : <color=#fffa><font=\"NEXON Lv1 Gothic OTF SDF\">{selectedItem.item.description}";

        selectedItemStat.text = string.Empty;

        selectedItemStat.transform.parent.gameObject.SetActive(true);

        for (int i = 0; i < selectedItem.item.consumables.Length; i++)
        {
            selectedItemStat.text += $"{selectedItem.item.consumables[i].type}<color=#fffa><font=\"NEXON Lv1 Gothic OTF SDF\"> +{selectedItem.item.consumables[i].value}</font></color>" + "\n";
        }

        canUse = selectedItem.item.type == ItemType.Consumable;
        canEquip = selectedItem.item.type == ItemType.Equipable && !slots[index].equipped;
        canUnEquip = selectedItem.item.type == ItemType.Equipable && slots[index].equipped;
        canDrop = true;
    }

    public void OnUseInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            // 소비 아이템일 경우
            if (canUse)
            {
                for (int i = 0; i < selectedItem.item.consumables.Length; i++)
                {
                    switch (selectedItem.item.consumables[i].type)
                    {
                        case ConsumableType.Health:
                            condition.Heal(selectedItem.item.consumables[i].value); break;
                        case ConsumableType.JumpBoost:
                            condition.BoostJumpPower(selectedItem.item.consumables[i].value, 10f); break;
                    }
                }
                RemoveSelctedItem();
            }
            else if (canEquip)
            {
                // 장착 로직 구현
            }
            else if (!canUnEquip)
            {
                // 장착 해제 로직 구현
            }
        }
    }

    public void OnDropInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed && canDrop)
        {
            SelectItem(selectedIndex);

            if (selectedItem.item == null) return;

            ThrowItem(selectedItem.item);
            RemoveSelctedItem();
        }
    }

    void RemoveSelctedItem()
    {
        selectedItem.quantity--;

        if (selectedItem.quantity <= 0)
        {
            if (slots[selectedItemIndex].equipped)
            {
                // UnEquip(selectedItemIndex);
            }

            selectedItem.item = null;
            ClearSelectedItemWindow();
        }

        UpdateUI();
    }

    public bool HasItem(ItemData item, int quantity)
    {
        return false;
    }
}