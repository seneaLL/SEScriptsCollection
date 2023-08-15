public class Ticks
{
    public int Update1 { get; set; }
    public int Update10 { get; set; }
    public int Update100 { get; set; }

    public Ticks()
    {
        Update1 = 0;
        Update10 = 0;
        Update100 = 0;
    }

    public override string ToString()
    {
        return $"Update1: {Update1}\nUpdate10: {Update10}\nUpdate100: {Update100}";
    }
}

Ticks ticks = new Ticks();

public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Update1 | UpdateFrequency.Update10 | UpdateFrequency.Update100;
}

public void Main(string args, UpdateType updateSource)
{
    if ((updateSource & UpdateType.Update1) != 0) Frequency1Tick();
    if ((updateSource & UpdateType.Update10) != 0) Frequency10Tick();
    if ((updateSource & UpdateType.Update100) != 0) Frequency100Tick();

    WriteTextOnSurface(ticks.ToString());
}

private void Frequency1Tick()
{
    ticks.Update1 += 1;
}

private void Frequency10Tick()
{
    ticks.Update10 += 1;
    AirLock();
}

private void Frequency100Tick()
{
    ticks.Update100 += 1;
}

private void WriteTextOnSurface(string text)
{
    IMyTextSurface surface = Me.GetSurface(0);
    surface.ContentType = ContentType.TEXT_AND_IMAGE;
    surface.Alignment = TextAlignment.CENTER;
    surface.TextPadding = 18.0f;
    surface.FontSize = 1.5f;
    surface.FontColor = new Color(100, 100, 100);
    surface.WriteText(Me.CustomName + "\n" + text);
}

private void AirLock()
{
    List<IMyBlockGroup> airlockGroups = new List<IMyBlockGroup>();
    GridTerminalSystem.GetBlockGroups(airlockGroups);

    foreach (IMyBlockGroup group in airlockGroups)
    {
        if (group.Name.Contains("AirLock"))
        {
            CheckAndLockDoorsInGroup(group);
        }
    }
}

private void CheckAndLockDoorsInGroup(IMyBlockGroup group)
{
    List<IMyTerminalBlock> groupBlocks = new List<IMyTerminalBlock>();
    group.GetBlocks(groupBlocks);

    bool allDoorsClosed = true;

    foreach (IMyTerminalBlock block in groupBlocks)
    {
        if (block is IMyDoor)
        {
            IMyDoor door = block as IMyDoor;
            if (door.Status == DoorStatus.Closed)
            {
                door.ApplyAction("OnOff_Off");
            }
            else
            {
                allDoorsClosed = false;
            }
        }
    }

    if (allDoorsClosed)
    {
        foreach (IMyTerminalBlock block in groupBlocks)
        {
            if (block is IMyDoor)
            {
                IMyDoor door = block as IMyDoor;
                door.ApplyAction("OnOff_On");
            }
        }
    }
}