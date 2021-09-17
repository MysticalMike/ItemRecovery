using System.Linq;
using ItemRecovery.Util;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace ItemRecovery.Events
{
    public class ShopEvents
    {
        private IModHelper helper;
        private IMonitor monitor;
        private double CostMultiplier;

        public ShopEvents(IModHelper helper, IMonitor monitor, double CostMultiplier)
        {
            this.helper = helper;
            this.monitor = monitor;
            this.CostMultiplier = CostMultiplier;
            
            helper.Events.Display.MenuChanged += OnMenuChanged;
        }
        
        private void OnMenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (e.NewMenu is ShopMenu newMenu)
            {
                if (newMenu.portraitPerson == null || newMenu.portraitPerson.Name != "Marlon")
                    return;
                
                bool same = ListHelper.SortListsThenCompareItemNames(newMenu.itemPriceAndStock.Keys.ToList(), Utility.getAdventureShopStock().Keys.ToList());
                if (!same)
                {
                    helper.Events.Input.ButtonPressed += OnButtonPressed;
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
                
                bool same = ListHelper.SortListsThenCompareItemNames(oldMenu.itemPriceAndStock.Keys.ToList(), Utility.getAdventureShopStock().Keys.ToList());
                if (!same)
                {
                    helper.Events.Input.ButtonPressed -= OnButtonPressed;
                    
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
            
            EmulatedShopMenu.receiveLeftClick(menu, Game1.getMouseX(true), Game1.getMouseY(true), true);
        }
    }
}