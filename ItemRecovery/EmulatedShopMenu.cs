using System;
using System.Collections.Generic;
using System.Linq;
using Netcode;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace ItemRecovery
{
    public class EmulatedShopMenu
    {
        private IModHelper helper;
        private IMonitor monitor;
        private IReflectionHelper reflection;

        private double CostMultiplier;

        public EmulatedShopMenu(IModHelper helper, IMonitor monitor, double CostMultiplier)
        {
            this.helper = helper;
            this.monitor = monitor;
            reflection = helper.Reflection;
            this.CostMultiplier = CostMultiplier;
            helper.Events.Display.MenuChanged += this.OnMenuChanged;
        }
        
        private bool StocksAreEqual(Dictionary<ISalable, int[]> stock1, Dictionary<ISalable, int[]> stock2)
        {
            if (stock1.Count != stock2.Count)
                return false;
            
            List<ISalable> list1 = stock1.Keys.ToList();
            List<ISalable> list2 = stock1.Keys.ToList();
            
            for (int i = 0; i < list1.Count; i++)
            {
                if (!list1[i].Name.Equals(list2[i].Name))
                    return false;
            }
            return true;
        }

        private void OnMenuChanged(object sender, MenuChangedEventArgs e)
        {
            Farmer farmer = (Farmer)sender;
            
            if (e.NewMenu is ShopMenu newMenu)
            {
                if (newMenu.portraitPerson == null || newMenu.portraitPerson.Name != "Marlon")
                    return;
                
                bool same = StocksAreEqual(newMenu.itemPriceAndStock, Utility.getAdventureShopStock());
                if (!same)
                {
                    helper.Events.Input.ButtonPressed += this.OnButtonPressed;
                    string text = "Test text right here!";
                    newMenu.potraitPersonDialogue = text;
                    
                    foreach (ISalable salable in newMenu.itemPriceAndStock.Keys)
                    {
                        newMenu.itemPriceAndStock[salable][0] = (int)(newMenu.itemPriceAndStock[salable][0] * CostMultiplier);
                        if (((Item)salable).isLostItem)
                            ((Item)salable).isLostItem = false;
                    }
                }
            }
            else if (e.OldMenu is ShopMenu oldMenu)
            {
                if (oldMenu.portraitPerson == null || oldMenu.portraitPerson.Name != "Marlon")
                    return;
                
                bool same = StocksAreEqual(oldMenu.itemPriceAndStock, Utility.getAdventureShopStock());
                if (!same)
                {
                    helper.Events.Input.ButtonPressed -= this.OnButtonPressed;
                    
                    foreach (ISalable salable in oldMenu.itemPriceAndStock.Keys)
                    {
                        if (((Item)salable).isLostItem)
                            ((Item)salable).isLostItem = true;
                    }
                }
            }
        }

        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!(Game1.activeClickableMenu is ShopMenu menu))
                return;

            if (e.Button != SButton.MouseLeft && e.Button != SButton.MouseRight)
                return;
            
            Item item = (Item)menu.hoveredItem;
            if (item != null)
            {
                helper.Input.Suppress(e.Button);
            }
            else return;
            
            receiveLeftClick(menu, Game1.getMouseX(true), Game1.getMouseY(true), true);
        }
        
        private void receiveLeftClick(ShopMenu menu, int x, int y, bool playSound = true)
        {
            for (int index = 0; index < menu.forSaleButtons.Count; ++index)
            {
                if (menu.currentItemIndex + index < menu.forSale.Count &&
                    menu.forSaleButtons[index].containsPoint(x, y))
                {
                    int num = menu.currentItemIndex + index;
                    if (menu.forSale[num] != null)
                    {
                        bool isShiftPressed = helper.Input.IsDown(SButton.LeftShift) || helper.Input.IsDown(SButton.RightShift);
                        int val1 = menu.forSale[num].Stack;
                        int numberToBuy = Math.Min(val1, menu.forSale[num].maximumStackSize());
                        if (numberToBuy == -1)
                            numberToBuy = 1;
                        if (menu.canPurchaseCheck != null && !menu.canPurchaseCheck(num))
                            return;
                        
                        if (numberToBuy > 0 && tryToPurchaseItem(menu, menu.forSale[num], menu.heldItem, numberToBuy, x, y, num))
                        {
                            menu.itemPriceAndStock.Remove(menu.forSale[num]);
                            menu.forSale.RemoveAt(num);
                        }
                        else if (numberToBuy <= 0)
                        {
                            if (menu.itemPriceAndStock[menu.forSale[num]].Length != 0 &&
                                menu.itemPriceAndStock[menu.forSale[num]][0] > 0)
                                Game1.dayTimeMoneyBox.moneyShakeTimer = 1000;
                            Game1.playSound("cancel");
                        }

                        IReflectedField<bool> feild = reflection.GetField<bool>(menu, "_isStorageShop", true);
                        if (menu.heldItem != null &&
                            ((feild.GetValue() || Game1.options.SnappyMenus ||
                              isShiftPressed && menu.heldItem.maximumStackSize() == 1) &&
                            (Game1.activeClickableMenu != null && Game1.activeClickableMenu is ShopMenu &&
                             Game1.player.addItemToInventoryBool(menu.heldItem as Item))))
                        {
                            menu.heldItem = (ISalable)null;
                            DelayedAction.playSoundAfterDelay("coin", 100);
                        }
                    }

                    menu.currentItemIndex = Math.Max(0, Math.Min(menu.forSale.Count - 4, menu.currentItemIndex));
                    menu.updateSaleButtonNeighbors();

                    IReflectedMethod setScrollBarToCurrectIndexMethod = reflection.GetMethod(menu, "setScrollBarToCurrentIndex", true);
                    setScrollBarToCurrectIndexMethod.Invoke();
                    return;
                }
            }
        }


        private bool tryToPurchaseItem(
            ShopMenu menu,
            ISalable item,
            ISalable held_item,
            int numberToBuy,
            int x,
            int y,
            int indexInForSaleList)
        {
            if (menu.readOnly)
                return false;
            if (held_item == null)
            {
                if (menu.itemPriceAndStock[item][1] == 0)
                {
                    menu.hoveredItem = (ISalable)null;
                    return true;
                }

                if (item.GetSalableInstance().maximumStackSize() < numberToBuy)
                    numberToBuy = Math.Max(1, item.GetSalableInstance().maximumStackSize());
                int amount = menu.itemPriceAndStock[item][0];
                int num1 = -1;
                int num2 = 5;
                if (menu.itemPriceAndStock[item].Length > 2)
                {
                    num1 = menu.itemPriceAndStock[item][2];
                    if (menu.itemPriceAndStock[item].Length > 3)
                        num2 = menu.itemPriceAndStock[item][3];
                    num2 *= numberToBuy;
                }

                if (ShopMenu.getPlayerCurrencyAmount(Game1.player, menu.currency) >= amount &&
                    (num1 == -1 || Game1.player.hasItemInInventory(num1, num2)))
                {
                    menu.heldItem = item.GetSalableInstance();
                    menu.heldItem.Stack = numberToBuy;
                    if (menu.storeContext == "QiGemShop" || menu.storeContext == "StardewFair")
                        menu.heldItem.Stack *= item.Stack;
                    else if (menu.itemPriceAndStock[item][1] == int.MaxValue && item.Stack != int.MaxValue)
                        menu.heldItem.Stack *= item.Stack;
                    if (!menu.heldItem.CanBuyItem(Game1.player) && !item.IsInfiniteStock() &&
                        (!(menu.heldItem is StardewValley.Object) ||
                         !(bool)(NetFieldBase<bool, NetBool>)(menu.heldItem as StardewValley.Object).isRecipe))
                    {
                        Game1.playSound("smallSelect");
                        menu.heldItem = (ISalable)null;
                        return false;
                    }

                    if (menu.itemPriceAndStock[item][1] != int.MaxValue && !item.IsInfiniteStock())
                    {
                        menu.itemPriceAndStock[item][1] -= numberToBuy;
                        menu.forSale[indexInForSaleList].Stack -= numberToBuy;
                    }

                    ShopMenu.chargePlayer(Game1.player, menu.currency, amount);
                    if (num1 != -1)
                        Game1.player.removeItemsFromInventory(num1, num2);
                    IReflectedField<bool> _isStorageShop = reflection.GetField<bool>(menu, "_isStorageShop", true);
                    if (!_isStorageShop.GetValue() && item.actionWhenPurchased())
                    {
                        if (menu.heldItem is StardewValley.Object &&
                            (bool)(NetFieldBase<bool, NetBool>)(menu.heldItem as StardewValley.Object).isRecipe)
                        {
                            string key = menu.heldItem.Name.Substring(0, menu.heldItem.Name.IndexOf("Recipe") - 1);
                            try
                            {
                                if ((menu.heldItem as StardewValley.Object).Category == -7)
                                    Game1.player.cookingRecipes.Add(key, 0);
                                else
                                    Game1.player.craftingRecipes.Add(key, 0);
                                Game1.playSound("newRecipe");
                            }
                            catch (Exception ex)
                            {
                            }
                        }

                        held_item = (ISalable)null;
                        menu.heldItem = (ISalable)null;
                    }
                    else
                    {
                        if (menu.heldItem != null && menu.heldItem is StardewValley.Object &&
                            (menu.heldItem as StardewValley.Object).ParentSheetIndex == 858)
                        {
                            Game1.player.team.addQiGemsToTeam.Fire(menu.heldItem.Stack);
                            menu.heldItem = (ISalable)null;
                        }

                        if (Game1.mouseClickPolling > 300)
                        {
                            if (menu.purchaseRepeatSound != null)
                                Game1.playSound(menu.purchaseRepeatSound);
                        }
                        else if (menu.purchaseSound != null)
                            Game1.playSound(menu.purchaseSound);
                    }

                    if (menu.onPurchase != null && menu.onPurchase(item, Game1.player, numberToBuy))
                        menu.exitThisMenu();
                }
                else
                {
                    if (amount > 0)
                        Game1.dayTimeMoneyBox.moneyShakeTimer = 1000;
                    Game1.playSound("cancel");
                }
            }
            else if (held_item.canStackWith(item))
            {
                numberToBuy = Math.Min(numberToBuy, held_item.maximumStackSize() - held_item.Stack);
                if (numberToBuy > 0)
                {
                    int amount = menu.itemPriceAndStock[item][0] * numberToBuy;
                    int num1 = -1;
                    int num2 = 5;
                    if (menu.itemPriceAndStock[item].Length > 2)
                    {
                        num1 = menu.itemPriceAndStock[item][2];
                        if (menu.itemPriceAndStock[item].Length > 3)
                            num2 = menu.itemPriceAndStock[item][3];
                        num2 *= numberToBuy;
                    }

                    int stack = item.Stack;
                    item.Stack = numberToBuy + menu.heldItem.Stack;
                    if (!item.CanBuyItem(Game1.player))
                    {
                        item.Stack = stack;
                        Game1.playSound("cancel");
                        return false;
                    }

                    item.Stack = stack;
                    if (ShopMenu.getPlayerCurrencyAmount(Game1.player, menu.currency) >= amount &&
                        (num1 == -1 || Game1.player.hasItemInInventory(num1, num2)))
                    {
                        int num3 = numberToBuy;
                        if (menu.itemPriceAndStock[item][1] == int.MaxValue && item.Stack != int.MaxValue)
                            num3 *= item.Stack;
                        menu.heldItem.Stack += num3;
                        if (menu.itemPriceAndStock[item][1] != int.MaxValue && !item.IsInfiniteStock())
                        {
                            menu.itemPriceAndStock[item][1] -= numberToBuy;
                            menu.forSale[indexInForSaleList].Stack -= numberToBuy;
                        }

                        ShopMenu.chargePlayer(Game1.player, menu.currency, amount);
                        if (Game1.mouseClickPolling > 300)
                        {
                            if (menu.purchaseRepeatSound != null)
                                Game1.playSound(menu.purchaseRepeatSound);
                        }
                        else if (menu.purchaseSound != null)
                            Game1.playSound(menu.purchaseSound);

                        if (num1 != -1)
                            Game1.player.removeItemsFromInventory(num1, num2);
                        IReflectedField<bool> _isStorageShop = reflection.GetField<bool>(menu, "_isStorageShop", true);
                        if (!_isStorageShop.GetValue() && item.actionWhenPurchased())
                            menu.heldItem = (ISalable)null;
                        if (menu.onPurchase != null && menu.onPurchase(item, Game1.player, numberToBuy))
                            menu.exitThisMenu();
                    }
                    else
                    {
                        if (amount > 0)
                            Game1.dayTimeMoneyBox.moneyShakeTimer = 1000;
                        Game1.playSound("cancel");
                    }
                }
            }

            if (menu.itemPriceAndStock[item][1] > 0)
                return false;
            Game1.player.itemsLostLastDeath.Remove((Item)item);
            item = (ISalable)null;
            return true;
        }
    }
}