using UnityEngine;

namespace EnumTypes
{
    public enum LookDirection { Up, Down, Left, Right }
    public enum PlayerState { Stand, Move, Run, Hold, Cook }
    public enum Layers
    {
        Default,
        TransparentFX,
        IgnoreRaycast,
        Reserved1,
        Water,
        UI,
        Reserved2,
        Reserved3,
        Player,
        Enemy,
    }
    
    public enum HeroEvents
    {
        LeaderAttackStarted,
        LeaderAttackStopped,
        LeaderDirectionChanged,
    }
    
    public enum FormationEvents
    {
        OnChangeLeaderMode,
        SetLeader,
    }

    public enum UIEvents
    {
        OnClickSignInGoogle,
        OnClickStart,
        OnClickManualGPGSSignIn,
        OnClickEmailSignIn,
        StartLoading,
        EndLoading,
        OnTouchStartJoystick,
        OnTouchEndJoystick,
        OnClickAutoButton,
        OnClickShowOnlyOwnedButton,
        OnClickSortListAttackButton,
        OnClickHeroTabButton,
        OnClickFormationTabButton,
    }

    public enum DataEvents
    {
        OnUserDataSave,
        OnUserDataLoad,
        OnUserDataReset,
        HeroCollectionUpdated,
    }

    public enum GachaEvents
    {
        GachaSingle,
        GachaTen,
        GachaThirty
    }

    public enum GoogleEvents
    {
        GPGSSignIn,
        ManualGPGSSignIn,
    }

    public enum FirebaseEvents
    {
        FirebaseInitialized,
        FirebaseDatabaseInitialized,
        FirebaseSignIn,
        EmailSignIn,
    }

    public class EnumTypes : MonoBehaviour { }
}