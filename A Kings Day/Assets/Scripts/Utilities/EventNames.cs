using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities
{
    public class EventNames
    {
        public const string PLAYER_PAUSE = "PLAYER_PAUSE";
        public const string PLAYER_RESUME = "PLAYER_RESUME";

        // TRANSITION
        public const string SAVE_KINGDOM_DATA = "SAVE_KINGDOM_DATA";
        public const string BEFORE_LOAD_SCENE = "BEFORE_LOAD_SCENE";
        // TOOLTIP
        public const string SHOW_TOOLTIP_MESG = "SHOW_TOOLTIP_MESG";
        public const string HIDE_TOOLTIP_MESG = "HIDE_TOOLTIP_MESG";
        public const string DISABLE_TOOLTIP_MESG = "DISABLE_TOOLTIP_MESG";
        public const string ENABLE_TOOLTIP_MESG = "ENABLE_TOOLTIP_MESG";

        // IN GAME INTERACTION
        public const string DISABLE_IN_GAME_INTERACTION = "DISABLE_IN_GAME_INTERACTION";
        public const string ENABLE_IN_GAME_INTERACTION = "ENABLE_IN_GAME_INTERACTION";

        // IN-GAME TABS
        public const string ENABLE_TAB_COVER = "ENABLE_TAB_COVER";
        public const string DISABLE_TAB_COVER = "DISABLE_TAB_COVER";
        public const string OPEN_MAP_TAB = "OPEN_MAP_TAB";
        public const string CLOSE_MAP_TAB = "CLOSE_MAP_TAB";

        // Resource UIs
        public const string SHOW_RESOURCES = "SHOW_RESOURCES";
        public const string HIDE_RESOURCES = "HIDE_RESOURCES";

        // SAVE PLAYER DATA
        public const string SAVE_PLAYER_DATA = "SAVE_PLAYER_DATA";

        // WEEKLY PGROESS
        public const string WEEKLY_UPDATE = "WEEKLY_UPDATE";
    }
}
