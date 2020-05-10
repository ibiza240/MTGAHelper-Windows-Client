namespace MTGAHelper.Lib.OutputLogParser.InMatchTracking
{
    public enum ZoneSimpleEnum
    {
        Unknown,
        ZoneType_Battlefield,
        ZoneType_Library,
        ZoneType_Graveyard,
        ZoneType_Exile,
        ZoneType_Hand,
        ZoneType_Stack,
        ZoneType_Command,
        ZoneType_Limbo,
        ZoneType_Sideboard,
        ZoneType_Revealed,
        ZoneType_Pending,
        ZoneType_PhasedOut,
    }

    public enum GameStateType
    {
        Unknown,
        GameStateType_Full,
        GameStateType_Diff,
        GameStateType_Binary,
    }

    public enum GameObjectType
    {
        Unknown,
        GameObjectType_Card,
        GameObjectType_Token,
        GameObjectType_Ability,
        GameObjectType_Emblem,
        GameObjectType_SplitCard,
        GameObjectType_SplitLeft,
        GameObjectType_SplitRight,
        GameObjectType_RevealedCard,
    }

    public enum Visibility
    {
        Unknown,
        Visibility_Public,
        Visibility_Private,
        Visibility_Hidden,
    }

    public enum SubZoneType
    {
        Unknown,
        SubZoneType_Top,
        SubZoneType_Bottom,
    }

    public enum GroupType
    {
        Unknown,
        GroupType_Ordered,
        GroupType_Arbitrary,
    }

    public enum GroupingContext
    {
        GroupingContext_None,
        GroupingContext_Scry,
        GroupingContext_Surveil
    }

    public enum OptionType
    {
        OptionType_None,
        OptionType_Modal,
        OptionType_Splice,
        OptionType_AlternativeCost,
        OptionType_Numeric,
        OptionType_VariableCost,
        OptionType_ManaType,
        OptionType_Order,
        OptionType_Search,
        OptionType_Group,
        OptionType_Select,
        OptionType_SelectGroup,
        OptionType_Distribution,
        OptionType_OptionalAction,
        OptionType_ActionsAvailable,
        OptionType_SelectFromGroups,
        OptionType_SearchFromGroups,
        OptionType_Gathering
    }

    public enum IdType
    {
        IdType_None,
        IdType_InstanceId,
        IdType_PromptParameterIndex
    }

    public enum AllowCancel
    {
        AllowCancel_None,
        AllowCancel_Continue,
        AllowCancel_Abort,
        AllowCancel_No
    }

    public enum AnnotationType
    {
        Unknown,
        AnnotationType_ZoneTransfer,
        AnnotationType_LossOfGame,
        AnnotationType_DamageDealt,
        AnnotationType_TappedUntappedPermanent,
        AnnotationType_ModifiedPower,
        AnnotationType_ModifiedToughness,
        AnnotationType_ModifiedColor,
        AnnotationType_PhaseOrStepModified,
        AnnotationType_AddAbility,
        AnnotationType_ModifiedLife,
        AnnotationType_CreateAttachment,
        AnnotationType_RemoveAttachment,
        AnnotationType_ObjectIdChanged,
        AnnotationType_Counter,
        AnnotationType_ControllerChanged,
        AnnotationType_CounterAdded,
        AnnotationType_CounterRemoved,
        AnnotationType_LayeredEffectCreated,
        AnnotationType_LayeredEffectDestroyed,
        AnnotationType_Attachment,
        AnnotationType_Haunt,
        AnnotationType_CopiedObject,
        AnnotationType_RemoveAbility,
        AnnotationType_WinTheGame,
        AnnotationType_ModifiedType,
        AnnotationType_TargetSpec,
        AnnotationType_TextChange,
        AnnotationType_FaceDown,
        AnnotationType_TurnPermanent,
        AnnotationType_DynamicAbility,
        AnnotationType_ObjectsSelected,
        AnnotationType_TriggeringObject,
        AnnotationType_DamageSource,
        AnnotationType_ManaPaid,
        AnnotationType_TokenCreated,
        AnnotationType_AbilityInstanceCreated,
        AnnotationType_AbilityInstanceDeleted,
        AnnotationType_DisplayCardUnderCard,
        AnnotationType_AbilityWordActive,
        AnnotationType_LinkInfo,
        AnnotationType_TokenDeleted,
        AnnotationType_Qualification,
        AnnotationType_ResolutionStart,
        AnnotationType_ResolutionComplete,
        AnnotationType_Designation,
        AnnotationType_GainDesignation,
        AnnotationType_CardRevealed,
        AnnotationType_NewTurnStarted,
        AnnotationType_ManaDetails,
        AnnotationType_DisqualifiedEffect,
        AnnotationType_LayeredEffect,
        AnnotationType_PendingEffect,
        AnnotationType_ShouldntPlay,
        AnnotationType_UseOrCostsManaCost,
        AnnotationType_RemainingSelections,
        AnnotationType_Shuffle,
        AnnotationType_CoinFlip,
        AnnotationType_ChooseRandom,
        AnnotationType_RevealedCardCreated,
        AnnotationType_RevealedCardDeleted,
        AnnotationType_SuspendLike,
        AnnotationType_ReplacementEffect,
        AnnotationType_EnteredZoneThisTurn,
        AnnotationType_CastingTimeOption,
        AnnotationType_Scry,
        AnnotationType_PredictedDirectDamage,
        AnnotationType_SwitchPowerToughness,
        AnnotationType_SupplementalText,
    }
}
