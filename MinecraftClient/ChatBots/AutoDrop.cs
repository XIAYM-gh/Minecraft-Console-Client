using MinecraftClient.Inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MinecraftClient.ChatBots
{
    class AutoDrop : ChatBot
    {
        private enum Mode
        {
            Include,    // Items in list will be dropped
            Exclude,    // Items in list will be kept
            Everything  // Everything will be dropped
        }
        private Mode dropMode = Mode.Include;
        private bool enable = true;

        private int updateDebounce = 0;
        private int updateDebounceValue = 2;
        private int inventoryUpdated = -1;

        private List<ItemType> itemList = new List<ItemType>();

        public AutoDrop(string mode, string itemList)
        {
            if (!Enum.TryParse(mode, true, out dropMode))
            {
                LogToConsole("无法从配置中读取拖放模式, 使用 Include 模式");
            }
            if (dropMode != Mode.Everything)
                this.itemList = ItemListParser(itemList).ToList();
        }

        /// <summary>
        /// Convert an item type string to item type array
        /// </summary>
        /// <param name="itemList">String to convert</param>
        /// <returns>Item type array</returns>
        private ItemType[] ItemListParser(string itemList)
        {
            string trimed = new string(itemList.Where(c => !char.IsWhiteSpace(c)).ToArray());
            string[] list = trimed.Split(',');
            List<ItemType> result = new List<ItemType>();
            foreach (string t in list)
            {
                ItemType item;
                if (Enum.TryParse(t, true, out item))
                {
                    result.Add(item);
                }
            }
            return result.ToArray();
        }

        public string CommandHandler(string cmd, string[] args)
        {
            if (args.Length > 0)
            {
                switch (args[0].ToLower())
                {
                    case "on":
                        enable = true;
                        inventoryUpdated = 0;
                        OnUpdateFinish();
                        return "AutoDrop 开启";
                    case "off":
                        enable = false;
                        return "AutoDrop 关闭";
                    case "add":
                        if (args.Length >= 2)
                        {
                            ItemType item;
                            if (Enum.TryParse(args[1], true, out item))
                            {
                                itemList.Add(item);
                                return "增加物品 " + item.ToString();
                            }
                            else
                            {
                                return "错误的物品的名称 " + args[1] + ". 请重试";
                            }
                        }
                        else
                        {
                            return "语法: add <物品名称>";
                        }
                    case "remove":
                        if (args.Length >= 2)
                        {
                            ItemType item;
                            if (Enum.TryParse(args[1], true, out item))
                            {
                                if (itemList.Contains(item))
                                {
                                    itemList.Remove(item);
                                    return "移除物品 " + item.ToString();
                                }
                                else
                                {
                                    return "列表中没有的项目";
                                }
                            }
                            else
                            {
                                return "错误的物品的名称 " + args[1] + ". 请重试";
                            }
                        }
                        else
                        {
                            return "语法: remove <物品名称>";
                        }
                    case "list":
                        if (itemList.Count > 0)
                        {
                            return "总计 " + itemList.Count + " 在列表:\n" + string.Join("\n", itemList);
                        }
                        else
                        {
                            return "列表中没有的项目";
                        }
                    default:
                        return GetHelp();
                }
            }
            else
            {
                return GetHelp();
            }
        }

        private string GetHelp()
        {
            return "AutoDrop ChatBot 命令. 可用命令: on, off, add, remove, list";
        }

        public override void Initialize()
        {
            if (!GetInventoryEnabled())
            {
                LogToConsole("库存处理被禁用. 取消加载...");
                UnloadBot();
                return;
            }
            RegisterChatBotCommand("autodrop", "AutoDrop ChatBot 命令", CommandHandler);
            RegisterChatBotCommand("ad", "AutoDrop ChatBot 命令别名", CommandHandler);
        }

        public override void Update()
        {
            if (updateDebounce > 0)
            {
                updateDebounce--;
                if (updateDebounce <= 0)
                {
                    OnUpdateFinish();
                }
            }
        }

        public override void OnInventoryUpdate(int inventoryId)
        {
            if (enable)
            {
                updateDebounce = updateDebounceValue;
                inventoryUpdated = inventoryId;
            }
        }

        private void OnUpdateFinish()
        {
            if (inventoryUpdated != -1)
            {
                var inventory = GetInventories()[inventoryUpdated];
                var items = inventory.Items.ToDictionary(entry => entry.Key, entry => entry.Value);
                if (dropMode == Mode.Include)
                {
                    foreach (var item in items)
                    {
                        if (itemList.Contains(item.Value.Type))
                        {
                            // Drop it !!
                            WindowAction(inventoryUpdated, item.Key, WindowActionType.DropItemStack);
                        }
                    }
                }
                else if (dropMode == Mode.Exclude)
                {
                    foreach (var item in items)
                    {
                        if (!itemList.Contains(item.Value.Type))
                        {
                            // Drop it !!
                            WindowAction(inventoryUpdated, item.Key, WindowActionType.DropItemStack);
                        }
                    }
                }
                else
                {
                    foreach (var item in items)
                    {
                        // Drop it !!
                        WindowAction(inventoryUpdated, item.Key, WindowActionType.DropItemStack);
                    }
                }
                inventoryUpdated = -1;
            }
        }
    }
}
