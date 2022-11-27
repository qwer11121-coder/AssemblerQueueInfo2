        string LCDTag = "AInfo";  //Name of the LCD panel you want the info on

        //-------- Don't change anything below this line :) ----------------

        List<IMyAssembler> assemblers = new List<IMyAssembler>();
        List<MyProductionItem> singleQueue = new List<MyProductionItem>();

        public struct QueueItem
        {
            public string Text { get; }
            public int Number { get; }

            public override string ToString() => $"{Number} x {Text}";
        }

        Dictionary<string, List<MyProductionItem>> queueDict = new Dictionary<string, List<MyProductionItem>>();
        Dictionary<string, string> outputDict = new Dictionary<string, string>();

        List<IMyTextPanel> Panels = new List<IMyTextPanel>();

        public Program()
        {
            Runtime.UpdateFrequency = UpdateFrequency.Update10 | UpdateFrequency.Update100;
            GridTerminalSystem.GetBlocksOfType(assemblers);
            UpdatePanels();
        }

        public void Main(string argument, UpdateType updateSource)
        {
            if ((updateSource & UpdateType.Update100) != 0)
            {
                UpdatePanels();
                assemblers.Clear();
                GridTerminalSystem.GetBlocksOfType(assemblers);
            }

            for (int i = 0; i < assemblers.Count; i++)
            {
                if (!assemblers[i].IsQueueEmpty)
                {
                    string name = assemblers[i].CustomName;
                    assemblers[i].GetQueue(singleQueue);
                    outputDict[name] = name + "\'s queue:\n";
                    if (singleQueue.Count > 0)
                    {
                        for (int j = 0; j < singleQueue.Count; j++)
                        {
                            outputDict[name] += "\n" + singleQueue[j].Amount + " x " +  singleQueue[j].BlueprintId.SubtypeName.Replace("Component","");
                        }
                    }
                    else
                    {
                        outputDict[name] += "\n No items in queue";
                    }
                }                
            }    

            foreach (IMyTextPanel panel in Panels)
            {
                string text = "---------------------\n";
                string[] assemblerName = panel.CustomData.Split('\n');
                foreach (string name in assemblerName)
                {
                    if (outputDict.ContainsKey(name))
                    {
                        text += outputDict[name];
                        text += "\n---------------------\n\n";
                    }                    
                }
                panel.WriteText(text);
            }
        }

        void UpdatePanels()
        {
            List<IMyTextPanel> LCDs = new List<IMyTextPanel>();
            Panels.Clear();
            GridTerminalSystem.GetBlocksOfType(LCDs);
            foreach (IMyTextPanel Panel in LCDs)
            {
                if (Panel.CustomName.Contains("[" + LCDTag + "]"))
                {
                    Panel.ContentType = ContentType.TEXT_AND_IMAGE;
                    Panels.Add(Panel);
                }
            }
        }
