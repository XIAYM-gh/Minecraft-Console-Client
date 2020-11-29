using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using MinecraftClient.Inventory;
using MinecraftClient.Mapping;

namespace MinecraftClient.ChatBots
{
    class AutoCraft : ChatBot
    {
        private bool waitingForMaterials = false;
        private bool waitingForUpdate = false;
        private bool waitingForTable = false;
        private bool craftingFailed = false;
        private int inventoryInUse = -2;
        private int index = 0;
        private Recipe recipeInUse;
        private List<ActionStep> actionSteps = new List<ActionStep>();

        private Location tableLocation = new Location();
        private bool abortOnFailure = true;
        private int updateDebounceValue = 2;
        private int updateDebounce = 0;
        private int updateTimeoutValue = 10;
        private int updateTimeout = 0;
        private string timeoutAction = "未定义";

        private string configPath = @"autocraft\config.ini";
        private string lastRecipe = ""; // Used in parsing recipe config

        private Dictionary<string, Recipe> recipes = new Dictionary<string, Recipe>();

        private void resetVar()
        {
            craftingFailed = false;
            waitingForTable = false;
            waitingForUpdate = false;
            waitingForMaterials = false;
            inventoryInUse = -2;
            index = 0;
            recipeInUse = null;
            actionSteps.Clear();
        }

        private enum ActionType
        {
            LeftClick,
            ShiftClick,
            WaitForUpdate,
            ResetCraftArea,
            Repeat
        }

        /// <summary>
        /// Represent a single action step of the whole crafting process
        /// </summary>
        private class ActionStep
        {
            /// <summary>
            /// The action type of this action step
            /// </summary>
            public ActionType ActionType;

            /// <summary>
            /// For storing data needed for processing
            /// </summary>
            /// <remarks>-2 mean not used</remarks>
            public int Slot = -2;

            /// <summary>
            /// For storing data needed for processing
            /// </summary>
            /// <remarks>-2 mean not used</remarks>
            public int InventoryID = -2;

            /// <summary>
            /// For storing data needed for processing
            /// </summary>
            public ItemType ItemType;

            public ActionStep(ActionType actionType)
            {
                ActionType = actionType;
            }
            public ActionStep(ActionType actionType, int inventoryID)
            {
                ActionType = actionType;
                InventoryID = inventoryID;
            }
            public ActionStep(ActionType actionType, int inventoryID, int slot)
            {
                ActionType = actionType;
                Slot = slot;
                InventoryID = inventoryID;
            }
            public ActionStep(ActionType actionType, int inventoryID, ItemType itemType)
            {
                ActionType = actionType;
                InventoryID = inventoryID;
                ItemType = itemType;
            }
        }

        /// <summary>
        /// Represent a crafting recipe
        /// </summary>
        private class Recipe
        {
            /// <summary>
            /// The results item of this recipe
            /// </summary>
            public ItemType ResultItem;

            /// <summary>
            /// Crafting table required for this recipe, playerInventory or Crafting
            /// </summary>
            public ContainerType CraftingAreaType;

            /// <summary>
            /// Materials needed and their position
            /// </summary>
            /// <remarks>position start with 1, from left to right, top to bottom</remarks>
            public Dictionary<int, ItemType> Materials;

            public Recipe() { }
            public Recipe(Dictionary<int, ItemType> materials, ItemType resultItem, ContainerType type)
            {
                Materials = materials;
                ResultItem = resultItem;
                CraftingAreaType = type;
            }

            /// <summary>
            /// Convert the position of a defined recipe from playerInventory to Crafting
            /// </summary>
            /// <param name="recipe"></param>
            /// <returns>Converted recipe</returns>
            /// <remarks>so that it can be used in crafting table</remarks>
            public static Recipe ConvertToCraftingTable(Recipe recipe)
            {
                if (recipe.CraftingAreaType == ContainerType.PlayerInventory)
                {
                    if (recipe.Materials.ContainsKey(4))
                    {
                        recipe.Materials[5] = recipe.Materials[4];
                        recipe.Materials.Remove(4);
                    }
                    if (recipe.Materials.ContainsKey(3))
                    {
                        recipe.Materials[4] = recipe.Materials[3];
                        recipe.Materials.Remove(3);
                    }
                }
                return recipe;
            }
        }

        public AutoCraft(string configPath = @"autocraft\config.ini")
        {
            this.configPath = configPath;
        }

        public override void Initialize()
        {
            if (!GetInventoryEnabled())
            {
                LogToConsole("Inventory handling 已禁用,自动合成将无法使用!");
                UnloadBot();
                return;
            }
            RegisterChatBotCommand("autocraft", "自动合成机器人命令", CommandHandler);
            RegisterChatBotCommand("ac", "自动合成命令别名", CommandHandler);
            LoadConfig();
        }

        public string CommandHandler(string cmd, string[] args)
        {
            if (args.Length > 0)
            {
                switch (args[0])
                {
                    case "load":
                        LoadConfig();
                        return "";
                    case "list":
                        string names = string.Join(", ", recipes.Keys.ToList());
                        return String.Format("总共有 {0} 个配方加载 : {1}", recipes.Count, names);
                    case "reload":
                        recipes.Clear();
                        LoadConfig();
                        return "";
                    case "resetcfg":
                        WriteDefaultConfig();
                        return "已设置配置为默认.";
                    case "start":
                        if (args.Length >= 2)
                        {
                            string name = args[1];
                            if (recipes.ContainsKey(name))
                            {
                                resetVar();
                                PrepareCrafting(recipes[name]);
                                return "";
                            }
                            else return "未指定配方名称,请检查配置文件!";
                        }
                        else return "请指定您要合成的配方名称!";
                    case "stop":
                        StopCrafting();
                        return "AutoCraft 已停止.";
                    case "help":
                        return GetCommandHelp(args.Length >= 2 ? args[1] : "");
                    default:
                        return GetHelp();
                }
            }
            else return GetHelp();
        }

        private string GetHelp()
        {
            return "可用子命令: load, list, reload, resetcfg, start, stop, help. 使用 /autocraft help <cmd name> 获取帮助. 可以用/ac代替/autocraft.";
        }

        private string GetCommandHelp(string cmd)
        {
            switch (cmd.ToLower())
            {
                case "load":
                    return "加载配置文件";
                case "list":
                    return "列出已加载合成方案";
                case "reload":
                    return "重新加载配置文件";
                case "resetcfg":
                    return "将默认示例配置写入默认位置";
                case "start":
                    return "开始合成, 使用: /autocraft start <合成方案>";
                case "stop":
                    return "停止当前正在运行的制造过程";
                case "help":
                    return "获取命令描述, 使用: /autocraft help <命令名>";
                default:
                    return GetHelp();
            }
        }

        #region Config handling

        public void LoadConfig()
        {
            if (!File.Exists(configPath))
            {
                if (!Directory.Exists(configPath))
                {
                    Directory.CreateDirectory(@"autocraft");
                }
                WriteDefaultConfig();
                LogDebugToConsole("没有找到配置, 创建一个新的");
            }
            try
            {
                ParseConfig();
                LogToConsole("成功加载");
            }
            catch (Exception e)
            {
                LogToConsole("解析配置时出错: \n" + e.Message);
            }
        }

        private void WriteDefaultConfig()
        {
            string[] content =
            {
                "[AutoCraft]",
                "# 一个有效的自动合成文件必须以 [AutoCraft] 开头.",
                "",
                "tablelocation=0,65,0   # 如果你想使用工作台的话, Terrain and movements必须启用, 在这里填写工作台的坐标, 格式: x,y,z",
                "onfailure=abort        # 若合成失败, 终止(abort) 还是 等待(wait)",
                "",
                "# 您可以在一个配置文件中定义多个合成方案",
                "# 这是一个如何定义合成方案的示例",
                "[Recipe]",
                "name=whatever          # 你想叫什么名字都行, 必须首先定义此字段（用于 Start）",
                "type=player            # 合成方式, 玩家(player) 还是 工作台(table)",
                "result=StoneButton     # the resulting item",
                "",
                "# 定义 槽 与它们应得的项目",
                "slot1=Stone            # 插槽(slot) 从1开始，从左到右，从上到下计数",
                "# 有关项目名称，请参阅：",
                "# https://github.com/ORelio/Minecraft-Console-Client/blob/master/MinecraftClient/Inventory/ItemType.cs"
							
				 
			 
			 
			 
			  
		   
		   
            };
            File.WriteAllLines(configPath, content);
        }

        private void ParseConfig()
        {
            string[] content = File.ReadAllLines(configPath);
            if (content.Length <= 0)
            {
                throw new Exception("空的配置文件: " + configPath);
            }
            if (content[0].ToLower() != "[autocraft]")
            {
                throw new Exception("无效的配置文件: " + configPath);
            }

            // local variable for use in parsing config
            string section = "";
            Dictionary<string, Recipe> recipes = new Dictionary<string, Recipe>();

            foreach (string l in content)
            {
                // ignore comment start with #
                if (l.StartsWith("#"))
                    continue;
                string line = l.Split('#')[0].Trim();
                if (line.Length <= 0)
                    continue;

                if (line[0] == '[' && line[line.Length - 1] == ']')
                {
                    section = line.Substring(1, line.Length - 2).ToLower();
                    continue;
                }

                string key = line.Split('=')[0].ToLower();
                if (!(line.Length > (key.Length + 1)))
                    continue;
                string value = line.Substring(key.Length + 1);
                switch (section)
                {
                    case "recipe": parseRecipe(key, value); break;
                    case "autocraft": parseMain(key, value); break;
                }
            }

            // check and save recipe
            foreach (var pair in recipes)
            {
                if ((pair.Value.CraftingAreaType == ContainerType.PlayerInventory
                    || pair.Value.CraftingAreaType == ContainerType.Crafting)
                    && (pair.Value.Materials != null
                    && pair.Value.Materials.Count > 0)
                    && pair.Value.ResultItem != ItemType.Air)
                {
                    // checking pass
                    this.recipes.Add(pair.Key, pair.Value);
                }
                else
                {
                    throw new Exception("合成方案中遗漏的项目: " + pair.Key);
                }
            }

            
        }

        #region Method for parsing different section of config

        private void parseMain(string key, string value)
        {
            switch (key)
            {
                case "tablelocation":
                    string[] values = value.Split(',');
                    if (values.Length == 3)
                    {
                        tableLocation.X = Convert.ToInt32(values[0]);
                        tableLocation.Y = Convert.ToInt32(values[1]);
                        tableLocation.Z = Convert.ToInt32(values[2]);
                    }
                    else throw new Exception("合成台位置(tablelocation) 格式无效: " + key);
                    break;
                case "onfailure":
                    abortOnFailure = value.ToLower() == "abort" ? true : false;
                    break;
                case "updatedebounce":
                    updateDebounceValue = Convert.ToInt32(value);
                    break;
            }
        }

        private void parseRecipe(string key, string value)
        {
            if (key.StartsWith("slot"))
            {
                int slot = Convert.ToInt32(key[key.Length - 1].ToString());
                if (slot > 0 && slot < 10)
                {
                    if (recipes.ContainsKey(lastRecipe))
                    {
                        ItemType itemType;
                        if (Enum.TryParse(value, true, out itemType))
                        {
                            if (recipes[lastRecipe].Materials != null && recipes[lastRecipe].Materials.Count > 0)
                            {
                                recipes[lastRecipe].Materials.Add(slot, itemType);
                            }
                            else
                            {
                                recipes[lastRecipe].Materials = new Dictionary<int, ItemType>()
                                    {
                                        { slot, itemType }
                                    };
                            }
                            return;
                        }
                        else
                        {
                            throw new Exception("无效的物品名称 " + lastRecipe + " 在 " + key);
                        }
                    }
                    else
                    {
                        throw new Exception("解析合成方案时缺少合成方案名称");
                    }
                }
                else
                {
                    throw new Exception("无效的物品槽在合成方案: " + key);
                }
            }
            else
            {
                switch (key)
                {
                    case "name":
                        if (!recipes.ContainsKey(value))
                        {
                            recipes.Add(value, new Recipe());
                            lastRecipe = value;
                        }
                        else
                        {
                            throw new Exception("指定的复制配方名称: " + value);
                        }
                        break;
                    case "type":
                        if (recipes.ContainsKey(lastRecipe))
                        {
                            recipes[lastRecipe].CraftingAreaType = value.ToLower() == "player" ? ContainerType.PlayerInventory : ContainerType.Crafting;
                        }
                        break;
                    case "result":
                        if (recipes.ContainsKey(lastRecipe))
                        {
                            ItemType itemType;
                            if (Enum.TryParse(value, true, out itemType))
                            {
                                recipes[lastRecipe].ResultItem = itemType;
                            }
                        }
                        break;
                }
            }
        }
        #endregion

        #endregion

        #region Core part of auto-crafting

        public override void OnInventoryUpdate(int inventoryId)
        {
            if ((waitingForUpdate && inventoryInUse == inventoryId) || (waitingForMaterials && inventoryInUse == inventoryId))
            {
                // Because server might send us a LOT of update at once, even there is only a single slot updated.
                // Using this to make sure we don't do things before inventory update finish
                updateDebounce = updateDebounceValue;
            }
        }

        public override void OnInventoryOpen(int inventoryId)
        {
            if (waitingForTable)
            {
                if (GetInventories()[inventoryId].Type == ContainerType.Crafting)
                {
                    waitingForTable = false;
                    ClearTimeout();
                    // After table opened, we need to wait for server to update table inventory items
                    waitingForUpdate = true;
                    inventoryInUse = inventoryId;
                    PrepareCrafting(recipeInUse);
                }
            }
        }

        public override void Update()
        {
            if (updateDebounce > 0)
            {
                updateDebounce--;
                if (updateDebounce <= 0)
                    InventoryUpdateFinished();
            }
            if (updateTimeout > 0)
            {
                updateTimeout--;
                if (updateTimeout <= 0)
                    HandleUpdateTimeout();
            }
        }

        private void InventoryUpdateFinished()
        {
            if (waitingForUpdate || waitingForMaterials)
            {
                if (waitingForUpdate)
                    waitingForUpdate = false;
                if (waitingForMaterials)
                {
                    waitingForMaterials = false;
                    craftingFailed = false;
                }
                HandleNextStep();
            }
        }

        private void OpenTable(Location location)
        {
            SendPlaceBlock(location, Direction.Up);
        }

        /// <summary>
        /// Prepare the crafting action steps by the given recipe name and start crafting
        /// </summary>
        /// <param name="recipe">Name of the recipe to craft</param>
        private void PrepareCrafting(string name)
        {
            PrepareCrafting(recipes[name]);
        }

        /// <summary>
        /// Prepare the crafting action steps by the given recipe and start crafting
        /// </summary>
        /// <param name="recipe">Recipe to craft</param>
        private void PrepareCrafting(Recipe recipe)
        {
            recipeInUse = recipe;
            if (recipeInUse.CraftingAreaType == ContainerType.PlayerInventory)
                inventoryInUse = 0;
            else
            {
                var inventories = GetInventories();
                foreach (var inventory in inventories)
                    if (inventory.Value.Type == ContainerType.Crafting)
                        inventoryInUse = inventory.Key;
                if (inventoryInUse == -2)
                {
                    // table required but not found. Try to open one
                    OpenTable(tableLocation);
                    waitingForTable = true;
                    SetTimeout("未找到工作台");
                    return;
                }
            }

            foreach (KeyValuePair<int, ItemType> slot in recipe.Materials)
            {
                // Steps for moving items from inventory to crafting area
                actionSteps.Add(new ActionStep(ActionType.LeftClick, inventoryInUse, slot.Value));
                actionSteps.Add(new ActionStep(ActionType.LeftClick, inventoryInUse, slot.Key));
            }
            if (actionSteps.Count > 0)
            {
                // Wait for server to send us the crafting result
                actionSteps.Add(new ActionStep(ActionType.WaitForUpdate, inventoryInUse, 0));
                // Put item back to inventory. (Using shift-click can take all item at once)
                actionSteps.Add(new ActionStep(ActionType.ShiftClick, inventoryInUse, 0));
                // We need to wait for server to update us after taking item from crafting result
                actionSteps.Add(new ActionStep(ActionType.WaitForUpdate, inventoryInUse));
                // Repeat the whole process again
                actionSteps.Add(new ActionStep(ActionType.Repeat));
                // Start crafting
                ConsoleIO.WriteLogLine("正在启动 AutoCraft: " + recipe.ResultItem);
                HandleNextStep();
            }
            else ConsoleIO.WriteLogLine("AutoCraft 启动失败, 检查你可用的制作材料 " + recipe.ResultItem);
        }

        /// <summary>
        /// Stop the crafting process by clearing crafting action steps and close the inventory
        /// </summary>
        private void StopCrafting()
        {
            actionSteps.Clear();
            // Closing inventory can make server to update our inventory
            // Useful when
            // - There are some items left in the crafting area
            // - Resynchronize player inventory if using crafting table
            if (GetInventories().ContainsKey(inventoryInUse))
            {
                CloseInventory(inventoryInUse);
                ConsoleIO.WriteLogLine("物品栏 #" + inventoryInUse + " 已关闭于 AutoCraft");
            }
        }

        /// <summary>
        /// Handle next crafting action step
        /// </summary>
        private void HandleNextStep()
        {
            while (actionSteps.Count > 0)
            {
                if (waitingForUpdate || waitingForMaterials || craftingFailed) break;
                ActionStep step = actionSteps[index];
                index++;
                switch (step.ActionType)
                {
                    case ActionType.LeftClick:
                        if (step.Slot != -2)
                        {
                            WindowAction(step.InventoryID, step.Slot, WindowActionType.LeftClick);
                        }
                        else
                        {
                            int[] slots = GetInventories()[step.InventoryID].SearchItem(step.ItemType);
                            if (slots.Count() > 0)
                            {
                                int ignoredSlot;
                                if (recipeInUse.CraftingAreaType == ContainerType.PlayerInventory)
                                    ignoredSlot = 9;
                                else
                                    ignoredSlot = 10;
                                slots = slots.Where(slot => slot >= ignoredSlot).ToArray();
                                if (slots.Count() > 0)
                                    WindowAction(step.InventoryID, slots[0], WindowActionType.LeftClick);
                                else
                                    craftingFailed = true;
                            }
                            else craftingFailed = true;
                        }
                        break;

                    case ActionType.ShiftClick:
                        if (step.Slot == 0)
                        {
                            WindowAction(step.InventoryID, step.Slot, WindowActionType.ShiftClick);
                        }
                        else craftingFailed = true;
                        break;

                    case ActionType.WaitForUpdate:
                        if (step.InventoryID != -2)
                        {
                            waitingForUpdate = true;
                        }
                        else craftingFailed = true;
                        break;

                    case ActionType.ResetCraftArea:
                        if (step.InventoryID != -2)
                            CloseInventory(step.InventoryID);
                        else
                            craftingFailed = true;
                        break;

                    case ActionType.Repeat:
                        index = 0;
                        break;
                }
                HandleError();
            }
            
        }

        /// <summary>
        /// Handle any crafting error after a step was processed
        /// </summary>
        private void HandleError()
        {
            if (craftingFailed)
            {
                if (actionSteps[index - 1].ActionType == ActionType.LeftClick && actionSteps[index - 1].ItemType != ItemType.Air)
                {
                    // Inform user the missing meterial name
                    ConsoleIO.WriteLogLine("缺少材料: " + actionSteps[index - 1].ItemType.ToString());
                }
                if (abortOnFailure)
                {
                    StopCrafting();
                    ConsoleIO.WriteLogLine("制作失败! 请检查你的可用材料");
                }
                else
                {
                    waitingForMaterials = true;
                    // Even though crafting failed, action step index will still increase
                    // we want to do that failed step again so decrease index by 1
                    index--;
                    ConsoleIO.WriteLogLine("制作失败! 等待更多材料");
                }
            }
        }

        private void HandleUpdateTimeout()
        {
            ConsoleIO.WriteLogLine("操作超时! 原因: " + timeoutAction);
        }

        /// <summary>
        /// Set the timeout. Used to detect the failure of open crafting table
        /// </summary>
        /// <param name="reason">The reason to display if timeout</param>
        private void SetTimeout(string reason = "未定义")
        {
            updateTimeout = updateTimeoutValue;
            timeoutAction = reason;
        }

        /// <summary>
        /// Clear the timeout
        /// </summary>
        private void ClearTimeout()
        {
            updateTimeout = 0;
        }

        #endregion
    }
}
