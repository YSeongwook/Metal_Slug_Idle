using EnumTypes;
using EventLibrary;
using UnityEngine;

public class LeaderIconManager : MonoBehaviour
{
    public GameObject[] icons;
    
    private void Awake()
    {
        EventManager<FormationEvents>.StartListening(FormationEvents.SetLeader, ToggleLeaderIcon);
    }

    private void OnDestroy()
    {
        EventManager<FormationEvents>.StopListening(FormationEvents.SetLeader, ToggleLeaderIcon);
    }

    private void ToggleLeaderIcon()
    {
        foreach (var icon in icons)
        {
            if(icon.activeSelf) icon.SetActive(false);
        }
    }
}
