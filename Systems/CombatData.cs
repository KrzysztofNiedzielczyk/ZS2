public class CombatData : Singleton<CombatData>
{
    private int enemyCount;

    public void SetCombatData(int enemies)
    {
        enemyCount = enemies;
    }

    public int GetEnemyCount()
    {
        return enemyCount;
    }
}