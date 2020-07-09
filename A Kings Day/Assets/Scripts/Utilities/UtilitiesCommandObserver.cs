using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Utilities
{
    public enum InputType
    {
        hold,
        down,
        up,
    }
    [System.Serializable]
    public class InputCommand
    {
        public string actionName;
        public KeyCode key;
        public bool isToggled;
        public InputType inputType;
    }

    public class UtilitiesCommandObserver : MonoBehaviour
    {
        #region Singleton
        private static UtilitiesCommandObserver instance;
        public static UtilitiesCommandObserver GetInstance
        {
            get
            {
                return instance;
            }
        }
        public void Awake()
        {
            if (UtilitiesCommandObserver.GetInstance == null)
            {
                DontDestroyOnLoad(this.gameObject);
                instance = this;
            }
            else
            {
                Destroy(this.gameObject);
            }
        }
        #endregion

        [Tooltip("List of Special Inputs that is always needs to be Checked")]
        public List<InputCommand> KeyInputs;

        public void Update()
        {
            for (int i = 0; i < KeyInputs.Count; i++)
            {
                switch (KeyInputs[i].inputType)
                {
                    case InputType.hold:
                        KeyInputs[i].isToggled = Input.GetKey(KeyInputs[i].key);
                        break;
                    case InputType.down:
                        KeyInputs[i].isToggled = Input.GetKeyDown(KeyInputs[i].key);
                        break;
                    case InputType.up:
                        KeyInputs[i].isToggled = Input.GetKeyUp(KeyInputs[i].key);
                        break;
                    default:
                        break;
                }
            }
        }

        public KeyCode GetKey(string actionName)
        {
            return KeyInputs.Find(x => x.actionName == actionName).key;
        }
        public bool isKeyToggled(KeyCode thisKey)
        {
            bool toggled = false;

            if(KeyInputs.Find(x => x.key == thisKey) != null)
            {
                toggled = KeyInputs.Find(x => x.key == thisKey).isToggled;
            }
            else
            {
                Debug.LogWarning("Trying to Check Key Input ["+ thisKey.ToString() +"] but not in ObserverList!");
            }

            return toggled;
        }
    }

}
