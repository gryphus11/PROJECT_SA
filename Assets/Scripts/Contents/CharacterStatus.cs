public class CharacterStatus
{
    public Data.CreatureData Data;
    public int DataId { get; set; } = 1;
    public int Level { get; set; } = 1;
    public int MaxHp { get; set; } = 1;
    public int Atk { get; set; } = 1;
    public int Def { get; set; } = 1;
    public int TotalExp { get; set; } = 1;
    public float MoveSpeed { get; set; } = 1;

    public void SetInfo(int key)
    {
        DataId = key;
        Data = Managers.Data.CreatureDic[key];
        MaxHp = (int)((Data.maxHp + Level * Data.upMaxHp) * Data.hpRate);
        Atk = (int)(Data.atk + (Level * Data.upAtk) * Data.atkRate);
        Def = (int)Data.def;
        MoveSpeed = Data.moveSpeed * Data.moveSpeedRate;
    }
}