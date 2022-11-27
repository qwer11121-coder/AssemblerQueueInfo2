string LCDTag = "AInfo";  //Name of the LCD panel you want the info on

//-------- Don't change anything below this line :) ----------------

List<IMyAssembler> assemblers = new List<IMyAssembler>();
List<MyProductionItem> singleQueue = new List<MyProductionItem>();

Dictionary<string, List<MyProductionItem>> outputDict = new Dictionary<string, List<MyProductionItem>>();

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
            outputDict[name] = new List<MyProductionItem>();
            if (singleQueue.Count > 0)
            {
                for (int j = 0; j < singleQueue.Count; j++)
                {
                    outputDict[name].Add(singleQueue[j]);
                }
            }
        }
    }

    foreach (IMyTextPanel panel in Panels)
    {
        // By default there could be 38 charactors in a line
        int defaultScreenWidth = 38;
        char dot = '_';
        long fontCode = panel.GetValue<long>("Font");

        // Monospace Font has only 25 charactors in a line
        if (fontCode == 1147350002)
        {
            defaultScreenWidth = 25;
            dot = '.';
        }

        float fontSize = panel.FontSize;
        int screenWidth = (int)(defaultScreenWidth / fontSize);

        StringBuilder text = new StringBuilder();
        string[] assemblerName = panel.CustomData.Split('\n');
        foreach (string name in assemblerName)
        {
            if (outputDict.ContainsKey(name))
            {
                // output data in outputDict as Text
                text.Append($"<<{name}>>\n").Append(new string('-', defaultScreenWidth)).Append("\n");
                foreach (var item in outputDict[name])
                {
                    string itemText = item.BlueprintId.SubtypeName.Replace("Component", "");
                    string itemAmount = item.Amount.ToString();
                    int dotCount = screenWidth - itemText.Length - itemAmount.Length;
                    if (dotCount <= 2)
                        dotCount = 2;
                    string dots = new string(dot, dotCount);
                    text.Append(itemText)
                    .Append(dots)
                    .Append(itemAmount)
                    .Append("\n");
                }
                text.Append("\n");
            }
        }
        panel.WriteText(text.ToString());
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
