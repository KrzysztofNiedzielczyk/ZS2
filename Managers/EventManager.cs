using UnityEngine;
using UnityEngine.Events;

public static class EventManager
{
    public static event UnityAction AbilityOneCalled;
    public static event UnityAction AbilityTwoCalled;
    public static event UnityAction AbilityThreeCalled;
    public static event UnityAction AbilityFourCalled;
    public static event UnityAction DayNightCycleToggled;
    public static event UnityAction<GameObject> EnemyDied;

    public static void OnAbilityOneCalled() => AbilityOneCalled?.Invoke();
    public static void OnAbilityTwoCalled() => AbilityTwoCalled?.Invoke();
    public static void OnAbilityThreeCalled() => AbilityThreeCalled?.Invoke();
    public static void OnAbilityFourCalled() => AbilityFourCalled?.Invoke();
    public static void OnDayNightCycleToggled() => DayNightCycleToggled?.Invoke();
    public static void OnEnemyDied(GameObject _enemy) => EnemyDied?.Invoke(_enemy);
}
