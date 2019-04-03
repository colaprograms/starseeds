using System.Collections;
using UnityEngine;
using Meta.Interop.Buttons;
using System.Collections.Generic;

namespace Meta.Buttons {
    public class buttonknower : BaseMetaButtonInteractionObject {
        bool button_pushed = false;
        
        private void Awake()
        {
            GameDad.headset_button_is_pushed = headset_button_is_pushed;
        }
        
        public bool headset_button_is_pushed() {
            return button_pushed;
        }
        
        public override void OnMetaButtonEvent(MetaButton button)
        {
            if(button.State == ButtonState.ButtonRelease)
                button_pushed = false;
            else if(button.State == ButtonState.ButtonShortPress || button.State == ButtonState.ButtonLongPress)
                button_pushed = true;
            else
                throw new System.Exception(System.String.Format("button in unknown state {0}", (int) button.State));
        }
    }
}
