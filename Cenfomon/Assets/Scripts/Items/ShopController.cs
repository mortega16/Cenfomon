using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ShopState { Menu, Buying, Selling, Busy}

public class ShopController : MonoBehaviour
{
    [SerializeField] InventoryUI inventoryUI;
    [SerializeField] ShopUI shopUI;
    [SerializeField] WalletUI walletUI;
    [SerializeField] CountSelectorUI countSelectorUI;

    public event Action OnStart;
    public event Action OnFinish;
    
    ShopState state;

    Merchant merchant;

    public static ShopController i {  get; private set; }
    private void Awake()
    {
        i = this;
    }

    Inventory inventory;
    private void Start()
    {
        inventory = Inventory.GetInventory();
    }

    public IEnumerator StartTrading(Merchant merchant)
    {
        this.merchant = merchant;

        OnStart?.Invoke();
        yield return StartMenuState();
    }

    IEnumerator StartMenuState()
    {
        state = ShopState.Menu;

        int selectedChoice = 0;
        yield return DialogManager.Instance.ShowDialogText("¿En qué puedo ayudarte?",
            waitForInput: false,
            choices: new List<string>() { "Comprar", "Salir" },
            onChoiceSelected: choiceIndex => selectedChoice = choiceIndex);

        if (selectedChoice == 0)
        {
            //Comprar
            state = ShopState.Buying;
            walletUI.Show();
            shopUI.Show(merchant.AvailableItems, (item) => StartCoroutine(BuyItem(item)),
                OnBackFromBuying);
        }
        else if (selectedChoice == 1)
        {
            //Salir
            OnFinish?.Invoke();
            yield break;
        }
    }

    public void HandleUpdate()
    {
        if(state == ShopState.Buying)
        {
            shopUI.HandleUpdate();
        }
    }

    IEnumerator BuyItem(ItemBase item)
    {
        state = ShopState.Busy;

        yield return DialogManager.Instance.ShowDialogText("¿Qué te gustaría comprar?",
            waitForInput: false, autoClose: false);

        int countToBuy = 1;
        yield return countSelectorUI.ShowSelector(100, item.Price,
            (selectedCount) => countToBuy = selectedCount);

        DialogManager.Instance.CloseDialog();

        float totalPrice = item.Price * countToBuy;

        if (Wallet.i.HasMoney(totalPrice))
        {
            int selectedChoice = 0;
            yield return DialogManager.Instance.ShowDialogText($"Sería ${totalPrice}",
                waitForInput: false,
                choices: new List<string>() { "Sí", "No" },
                onChoiceSelected: choiceIndex => selectedChoice = choiceIndex);

            if(selectedChoice == 0)
            {
                //sí
                inventory.AddItem(item, countToBuy);
                Wallet.i.TakeMoney(totalPrice);
                yield return DialogManager.Instance.ShowDialogText("¡Gracias, vuelva pronto!");
            }
        }
        else
        {
            yield return DialogManager.Instance.ShowDialogText("Uy mano, no te alcanza");
        }

        state = ShopState.Buying;
    }

    void OnBackFromBuying()
    {
        shopUI.Close();
        walletUI.Close();
        StartCoroutine(StartMenuState());
    }
}
