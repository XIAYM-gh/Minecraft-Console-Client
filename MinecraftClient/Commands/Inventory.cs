using System;
using System.Collections.Generic;
using System.Linq;
using MinecraftClient.Inventory;

namespace MinecraftClient.Commands
{
    class Inventory : Command
    {
        public override string CMDName { get { return "inventory"; } }
        public override string CMDDesc { get { return GetCommandDesc(); } }

        public override string Run(McClient handler, string command, Dictionary<string, object> localVars)
        {
            if (handler.GetInventoryEnabled())
            {
                string[] args = getArgs(command);
                if (args.Length >= 1)
                {
                    try
                    {
                        int inventoryId;
                        if (args[0].ToLower() == "creativegive")
                        {
                            if (args.Length >= 4)
                            {
                                int slot = int.Parse(args[1]);
                                ItemType itemType = ItemType.Stone;
                                if (Enum.TryParse(args[2], true, out itemType))
                                {
                                    if (handler.GetGamemode() == 1)
                                    {
                                        int count = int.Parse(args[3]);
                                        if (handler.DoCreativeGive(slot, itemType, count, null))
                                            return "已要求 " + itemType + " x" + count + " 在物品槽 #" + slot;
                                        else return "未能获取物品!";
                                    }
                                    else return "你必须处于创造模式";
                                }
                                else
                                {
                                    return CMDDesc;
                                }
                            }
                            else return CMDDesc;
                        }
                        else if (args[0].ToLower().StartsWith("p"))
                        {
                            // player inventory is always ID 0
                            inventoryId = 0;
                        }
                        else if (args[0].ToLower().StartsWith("c"))
                        {
                            List<int> availableIds = handler.GetInventories().Keys.ToList();
                            availableIds.Remove(0); // remove player inventory ID from list
                            if (availableIds.Count == 1)
                                inventoryId = availableIds[0]; // one container, use it
                            else return "未找到容器 请用明确的ID重试";
                        }
                        else if (args[0].ToLower() == "help")
                        {
                            if (args.Length >= 2)
                            {
                                return GetSubCommandHelp(args[1]);
                            }
                            else return GetHelp();
                        }
                        else inventoryId = int.Parse(args[0]);
                        string action = args.Length > 1
                            ? args[1].ToLower()
                            : "list";
                        switch (action)
                        {
                            case "close":
                                if (handler.CloseInventory(inventoryId))
                                    return "正在关闭物品栏 #" + inventoryId;
                                else return "关闭物品栏 #" + inventoryId + "失败";
                            case "list":
                                Container inventory = handler.GetInventory(inventoryId);
                                if(inventory==null)
                                    return "物品栏 #" + inventoryId + " 未退出";
                                List<string> response = new List<string>();
                                response.Add("Inventory #" + inventoryId + " - " + inventory.Title + "§8");
                                foreach (KeyValuePair<int, Item> item in inventory.Items)
                                {
                                    string displayName = item.Value.DisplayName;
                                    if (String.IsNullOrEmpty(displayName))
                                    {
                                        if (item.Value.Damage != 0)
                                            response.Add(String.Format(" #{0}: {1} x{2} | 已消耗耐久: {3}", item.Key, item.Value.Type, item.Value.Count, item.Value.Damage));
                                        else
                                            response.Add(String.Format(" #{0}: {1} x{2}", item.Key, item.Value.Type, item.Value.Count));
                                    }
                                    else
                                    {
                                        if (item.Value.Damage != 0)
                                            response.Add(String.Format(" #{0}: {1} x{2} - {3}§8 | 已消耗耐久: {4}", item.Key, item.Value.Type, item.Value.Count, displayName, item.Value.Damage));
                                        else
                                            response.Add(String.Format(" #{0}: {1} x{2} - {3}§8", item.Key, item.Value.Type, item.Value.Count, displayName));
                                    }
                                }
                                if (inventoryId == 0) response.Add("您选择的物品栏位置是 " + (handler.GetCurrentSlot() + 1));
                                return String.Join("\n", response.ToArray());
                            case "click":
                                if (args.Length >= 3)
                                {
                                    int slot = int.Parse(args[2]);
                                    WindowActionType actionType = WindowActionType.LeftClick;
                                    string keyName = "左键";
                                    if (args.Length >= 4)
                                    {
                                        string b = args[3];
                                        if (b.ToLower()[0] == 'r')
                                        {
                                            actionType = WindowActionType.RightClick;
                                            keyName = "右键";
                                        }
                                        if (b.ToLower()[0] == 'm')
                                        {
                                            actionType = WindowActionType.MiddleClick;
                                            keyName = "中键";
                                        }
                                    }
                                    handler.DoWindowAction(inventoryId, slot, actionType);
                                    return "正在使用 " + keyName + " 点击物品槽 " + slot + " 在窗口 #" + inventoryId;
                                }
                                else return CMDDesc;
                            case "drop":
                                if (args.Length >= 3)
                                {
                                    int slot = int.Parse(args[2]);
                                    // check item exist
                                    if (!handler.GetInventory(inventoryId).Items.ContainsKey(slot))
                                        return "物品槽 #" + slot + "没有物品";
                                    WindowActionType actionType = WindowActionType.DropItem;
                                    if (args.Length >= 4)
                                    {
                                        if (args[3].ToLower() == "all")
                                        {
                                            actionType = WindowActionType.DropItemStack;
                                        }
                                    }
                                    if (handler.DoWindowAction(inventoryId, slot, actionType))
                                    {
                                        if (actionType == WindowActionType.DropItemStack)
                                            return "从槽中删除整个项目堆栈 #" + slot;
                                        else return "从物品槽 #" + slot + "丢出一个物品";
                                    }
                                    else
                                    {
                                        return "失败";
                                    }
                                }
                                else return CMDDesc;
                            default:
                                return CMDDesc;
                        }
                    }
                    catch (FormatException) { return CMDDesc; }
                }
                else
                {
                    Dictionary<int, Container> inventories = handler.GetInventories();
                    List<string> response = new List<string>();
                    response.Add("库存:");
                    foreach (KeyValuePair<int, Container> inventory in inventories)
                    {
                        response.Add(String.Format(" #{0}: {1}", inventory.Key, inventory.Value.Title + "§8"));
                    }
                    response.Add(CMDDesc);
                    return String.Join("\n", response);
                }
            }
            else return "请从配置文件中开启 inventoryhandling 来使用该指令";
        }

        #region Methods for commands help
        private string GetCommandDesc()
        {
            return GetBasicUsage() + " 使用 \"/inventory help\" 获取更多帮助";
        }

        private string GetAvailableActions()
        {
            return "可用的命令: list, close, click, drop.";
        }

        private string GetBasicUsage()
        {
            return "基础语法: /inventory <player|container|<id>> <action>.";
        }

        private string GetHelp()
        {
            return GetBasicUsage()
                + "\n " + GetAvailableActions() + " 使用 \"/inventory help <action>\" 来获取命令帮助"
                + "\n 创造模式物品获取: " + GetCreativeGiveHelp()
                + "\n \"player\" 和 \"container\" 可以缩写为 \"p\" and \"c\""
                + "\n 注意， \"[]\" 中的参数是可选的";
        }

        private string GetCreativeGiveHelp()
        {
            return "语法: /inventory creativegive <物品槽> <物品类型> <数量>";
        }

        private string GetSubCommandHelp(string cmd)
        {
            switch (cmd)
            {
                case "list":
                    return "列出你物品栏中的物品 语法: /inventory <player|container|<id>> list";
                case "close":
                    return "关闭一个已打开的物品栏 语法: /inventory <player|container|<id>> close";
                case "click":
                    return "点击一个物品 语法: /inventory <player|container|<id>> click <物品槽> [left|right|middle]. \n默认为左键物品";
                case "drop":
                    return "从你的背包中丢出物品 语法: /inventory <player|container|<id>> drop <物品槽> [all]. \n“all”即代表丢出所有的物品";
                case "help":
                    return GetHelp();
                default:
                    return "未知命令 " + GetAvailableActions();
            }
        }
        #endregion
    }
}
