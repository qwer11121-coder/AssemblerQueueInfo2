string LCDTag = "AInfo";  //Name of the LCD panel you want the info on

//-------- Don't change anything below this line :) ----------------

const long MONOSPACE_FONT = 1147350002;

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
        // By default there could be 32 charactors in a line
        int defaultScreenWidth = 32;
        long fontCode = panel.GetValue<long>("Font");

        // Monospace Font has only 25 charactors in a line
        if (fontCode == MONOSPACE_FONT)
        {
            defaultScreenWidth = 25;
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
                    string itemText = "";
                    switch (item.BlueprintId.SubtypeName)
                    {
                        case "ConstructionComponent":
                            itemText = "Construction Comp.";
                            break;
                        case "GravityGeneratorComponent":
                            itemText = "Gravity Comp.";
                            break;
                        case "DetectorComponent":
                            itemText = "Detector Comp.";
                            break;
                        case "MedicalComponent":
                            itemText = "Medical Comp.";
                            break;
                        case "RadioCommunicationComponent":
                            itemText = "Radio-comm Comp.";
                            break;
                        case "ReactorComponent":
                            itemText = "Reactor Comp.";
                            break;
                        case "ThrustComponent":
                            itemText = "Thruster Comp.";
                            break;
                        default:
                            itemText = item.BlueprintId.SubtypeName.Replace("Component", "");
                            break;
                    }
                    string itemAmount = item.Amount.ToString();
                    float dotCount = 0;
                    // Monospace is 
                    if (fontCode == MONOSPACE_FONT)
                    {
                        dotCount = screenWidth - itemText.Length - itemAmount.Length;
                    }
                    else
                    {
                        dotCount = ((float)screenWidth - CalculateStringWidth(itemText) - CalculateStringWidth(itemAmount)) * 2;
                    }
                    if (dotCount <= 2)
                        dotCount = 2;
                    string dots = new string('.', (int)dotCount);
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

// Calculate string width for Debug font, use "0" as standard width
float CalculateStringWidth(string text)
{
    float width = 0.0f;
    foreach (char c in text)
    {
        switch (c)
        {
            // Symbol
            case '.':
                width += 0.5f;
                break;
            case '_':
                width += 0.8f;
                break;
            case '-':
                width += 0.6f;
                break;
            case ' ':
                width += 0.45f;
                break;
            // Lower case letter
            case 'a':
            case 'b':
            case 'd':
            case 'e':
            case 'g':
            case 'h':
            case 'k':
            case 'n':
            case 'o':
            case 'p':
            case 'q':
            case 's':
            case 'u':
            case 'y':
                width += 0.9f;
                break;
            case 'c':
            case 'z':
                width += 0.85f;
                break;
            case 'f':
            case 'r':
            case 't':
                width += 0.5f;
                break;
            case 'i':
            case 'j':
            case 'l':
                width += 0.45f;
                break;
            case 'm':
            case 'w':
                width += 1.4f;
                break;
            case 'x':
            case 'v':
                width += 0.8f;
                break;
            // Upper case letter
            case 'A':
            case 'B':
            case 'D':
            case 'N':
            case 'O':
            case 'Q':
            case 'R':
            case 'S':
                width += 1.1f;
                break;
            case 'C':
            case 'X':
            case 'Z':
                width += 1.0f;
                break;
            case 'E':
                width += 0.95f;
                break;
            case 'F':
            case 'K':
            case 'T':
                width += 0.9f;
                break;
            case 'G':
            case 'H':
            case 'P':
            case 'U':
            case 'V':
            case 'Y':
                width += 1.05f;
                break;
            case 'I':
                width += 0.45f;
                break;
            case 'J':
                width += 0.85f;
                break;
            case 'L':
                width += 0.8f;
                break;
            case 'M':
                width += 1.35f;
                break;
            case 'W':
                width += 1.6f;
                break;
            // Number
            case '1':
                width += 0.5f;
                break;
            case '3':
                width += 0.9f;
                break;
            case '7':
                width += 0.85f;
                break;
            default:
                width += 1.0f;
                break;
        }
    }
    return width;
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
