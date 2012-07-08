﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace FreePIE.GUI.CodeCompletion
{
    public abstract class KeyAction : IPopupAction
    {
        protected KeyAction(IEnumerable<Key> modifiers, Key key)
        {
            this.Modifiers = modifiers;
            this.Key = key;
        }

        protected KeyAction()
        {
            this.Modifiers = new Caliburn.Micro.BindableCollection<Key>();
        }

        private bool IsTriggered(EventType type, object args)
        {
            if (type != EventType.KeyPress)
                return false;

            var keyArgs = args as KeyEventArgs;

            return Key == keyArgs.Key && Modifiers.All(keyArgs.KeyboardDevice.IsKeyDown);
        }

        protected abstract void DoAct(CompletionPopupView view, KeyEventArgs args);

        public void Act(EventType type, CompletionPopupView view, object args)
        {
            if (!IsTriggered(type, args))
                return;

            var keyArgs = args as KeyEventArgs;

            DoAct(view, keyArgs);

            if (ShouldSwallow)
                keyArgs.Handled = true;
        }

        public Key Key { get; set; }
        public bool ShouldSwallow { get; set; }

        [TypeConverter(typeof (KeyActionListConverter))]
        public IEnumerable<Key> Modifiers { get; set; }
    }

    public class KeyActionListConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof (string))
                return true;

            return false;
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture,
                                           object value)
        {
            string val = value as string;

            if (val == null)
                throw new InvalidOperationException("Can only convert from strings.");

            var values = val.Split(' ');

            return values.Select(modifier => (Key) Enum.Parse(typeof (Key), modifier));
        }
    }
}
