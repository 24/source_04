using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace pb.Windows.Forms
{
    // KeyEventArgs :
    //   KeyCode et KeyValue sont identique
    //   KeyData = KeyCode & Modifiers
    //   Keys :
    //     KeyCode valeur de 1 à 254
    //     KeyCode mask 0xFFFF (65535)
    //     Modifiers : Shift = 0x00010000 (65536), Control = 0x00020000 (131072), Alt = 0x00040000 (262144)
    //     Back = 8, Tab = 9, Enter = 13, ShiftKey = 16, ControlKey = 17, Menu (alt) = 18, Pause = 19, CapsLock = 20, Escape = 27,
    //     Space = 32, PageUp = 33, PageDown = 34, PrintScreen = 44, A = 65, Z = 90, LWin = 91, RWin = 92, Apps (context menu) = 93, NumPad0 = 96, NumPad9 = 105, F1 = 112, F24 = 135,
    //     NumLock = 144, Scroll = 145
    //     alt-gr = ControlKey + Menu (alt)
    //   Keyboard keys :
    //     [escape] = Escape (27)  -  [F1] = F1 (112)  -  [F12] = F12 (123)  -  [print screen] (no key)  -  [scroll lock] = Scroll (145)  -  [ctrl-scroll lock] = Cancel (3)  -  [pause] = Pause (19)  -  [ctrl-pause] = Cancel (3)
    //     [²] = Oem7 (222)  -  [1] = D1 (49)  -  [9] = D9 (57)  - [0] = D0 (48)  -  [)] = OemOpenBrackets (219)  -  [=] = Oemplus (187)  -  [Backspace] = Back (8)
    //     [Tab] = Tab (9)  -  [A] = A (65)  -  [P] = P (80)  -  [^] = Oem6 (221)  -  [$] = Oem1 (186)  -  [enter] = Return (13)
    //     [Caps lock] = Capital (20)  -  [Q] = Q (81)  -  [L] = L (76)  -  [M] = M (77)  -  [ù] = Oemtilde (192)  -  [*] = Oem5 (220)
    //     [shift] = ShiftKey (16)  -  [<] = OemBackslash (226)  -  [W] = W (87)  -  [N] = N (78)  -  [,] = Oemcomma (188)  -  [;] = OemPeriod (190)  -  [:] = OemQuestion (191)  -  [!] = Oem8 (223)  -  [shift] = ShiftKey (16)
    //     [ctrl] = ControlKey (17)  -  [LWin] = LWin (91)  -  [alt] = Menu (18)  -  [space] = Space (32)  -  [alt-gr] = ControlKey + Menu (17 + 18)  -  [RWin] = RWin (92)  -  [apps] = Apps (93)  -  [ctrl] = ControlKey (17)
    //     [ins] = Insert (45)  -  [home] = Home (36)  -  [page-up] = PageUp (33)
    //     [del] = Delete (46)  -  [end] = End (35)  -  [page-down] = Next (34)
    //     [numlock] = NumLock (144)  -  [/] = Divide (111)  -  [*] = Multiply (106)  -  [-] = Subtract (109)
    //     [7] = NumPad7 (103)  -  [8] = NumPad8 (104)  -  [9] = NumPad9 (105)  -  [+] = Add (107)
    //     [4] = NumPad4 (100)  -  [5] = NumPad5 (101)  -  [6] = NumPad6 (102)
    //     [1] = NumPad1 (97)  -  [2] = NumPad2 (98)  -  [3] = NumPad3 (99)  -  [enter] = Return (13)
    //     [0] = NumPad0 (96)  -  [.] = Decimal (110)
    //
    //   examples :
    //     crtl              : KeyCode = ControlKey    KeyData = ControlKey, Control
    //     crtl-N            : KeyCode = ControlKey    KeyData = ControlKey, Control          (send multiple times)
    //                         KeyCode = N             KeyData = N, Control
    //     shift-crtl-N      : KeyCode = ShiftKey      KeyData = ShiftKey, Shift              (send multiple times)
    //                         KeyCode = ControlKey    KeyData = ControlKey, Shift, Control   (send multiple times)
    //                         KeyCode = N             KeyData = N, Shift, Control
    //     shift-crtl-alt-N  : KeyCode = ControlKey    KeyData = ControlKey, Control
    //                         KeyCode = ShiftKey      KeyData = ShiftKey, Shift, Control
    //                         KeyCode = Menu          KeyData = Menu, Shift, Control, Alt
    //                         KeyCode = N             KeyData = N, Shift, Control, Alt


    // manage multiple key like ctrl-K + ctrl-N
    // one KeyDefinition manage all multiple key who last key is ctrl-N, ex : ctrl-N, ctrl-K + ctrl-N, ctrl-M + ctrl-N
    public class KeyDefinition
    {
        // simple key                  : SimpleKey = true,  FirstKey = false, Multiple = false                     ctrl-O
        // first key                   : SimpleKey = false, FirstKey = true,  Multiple = false                     ctrl-K
        // multiple key                : SimpleKey = false, FirstKey = false, Multiple = true                      ctrl-K ctrl-N
        // first key and multiple key  : SimpleKey = false, FirstKey = true,  Multiple = true                      ctrl-K ctrl-K
        // simple key and multiple key : SimpleKey = true,  FirstKey = false, Multiple = true                      ctrl-N and ctrl-K ctrl-N
        public bool SimpleKey = false;
        public bool FirstKey = false;
        public bool Multiple = false;
        public Action Action = null;                         // Action of simple key (ctrl-N),  or Action of first key (ctrl-K)
        public Dictionary<int, Action> Actions = null;       // Action of multiple keys : ctrl-K + ctrl-N, ctrl-K is key in dictionary
    }

    public class FormKeyboard
    {
        private static bool __trace = false;
        private Keys _firstKey = Keys.None;
        private Dictionary<int, KeyDefinition> _keys = new Dictionary<int, KeyDefinition>();
        private int _minKeyValue = 256;
        private int _maxKeyValue = 0;

        public FormKeyboard(Form form)
        {
            form.KeyPreview = true;
            form.KeyDown += Form_KeyDown;
        }

        public static bool Trace { get { return __trace; } set { __trace = value; } }

        public void SetSimpleKey(Keys key, Action action)
        {
            if (action == null)
                throw new PBException("action can't be null");

            //int keyValue = (int)(key & Keys.KeyCode);
            //if (keyValue < (int)Keys.Space)
            //    throw new PBException("bad key {0}", key.ToString());
            //if (keyValue < _minKeyValue)
            //    _minKeyValue = keyValue;
            //if (keyValue > _maxKeyValue)
            //    _maxKeyValue = keyValue;
            ControlKey(key);

            KeyDefinition keyDefinition;
            if (_keys.TryGetValue((int)key, out keyDefinition))
            {
                if (keyDefinition.FirstKey)
                    throw new PBException("key {0} is already defined as first key", key.ToString());
                //if (keyDefinition.Multiple)
                //    throw new PBException("key {0} is already defined as multiple key", key.ToString());
                keyDefinition.Action = action;
            }
            else
                _keys.Add((int)key, new KeyDefinition { SimpleKey = true, Action = action });
        }

        public void SetFirstKey(Keys key, Action action = null)
        {
            //int keyValue = (int)(key & Keys.KeyCode);
            //if (keyValue < (int)Keys.Space)
            //    throw new PBException("bad key {0}", key.ToString());
            //if (keyValue < _minKeyValue)
            //    _minKeyValue = keyValue;
            //if (keyValue > _maxKeyValue)
            //    _maxKeyValue = keyValue;
            ControlKey(key);

            KeyDefinition keyDefinition;
            if (_keys.TryGetValue((int)key, out keyDefinition))
            {
                //if (!keyDefinition.FirstKey && !keyDefinition.Multiple)
                if (keyDefinition.SimpleKey)
                    throw new PBException("key {0} is already defined as a simple key", key.ToString());
                keyDefinition.FirstKey = true;
                if (action != null)
                    keyDefinition.Action = action;
            }
            else
                _keys.Add((int)key, new KeyDefinition { FirstKey = true, Action = action });
        }

        public void SetMultipleKey(Keys key1, Keys key2, Action action)
        {
            if (action == null)
                throw new PBException("action can't be null");

            //int keyValue = (int)(key1 & Keys.KeyCode);
            //if (keyValue < (int)Keys.Space)
            //    throw new PBException("bad key {0}", key1.ToString());
            //if (keyValue < _minKeyValue)
            //    _minKeyValue = keyValue;
            //if (keyValue > _maxKeyValue)
            //    _maxKeyValue = keyValue;

            //KeyDefinition keyDefinition;
            //if (_keys.TryGetValue((int)key1, out keyDefinition))
            //{
            //    //if (!keyDefinition.FirstKey)
            //    if (!keyDefinition.FirstKey && !keyDefinition.Multiple)
            //        //throw new PBException("key {0} is already defined and is'nt a first key", key1.ToString());
            //        throw new PBException("key {0} is already defined as a simple key", key1.ToString());
            //}
            //else
            //    _keys.Add((int)key1, new KeyDefinition { FirstKey = true });
            SetFirstKey(key1);

            //int keyValue = (int)(key2 & Keys.KeyCode);
            //if (keyValue < (int)Keys.Space)
            //    throw new PBException("bad key {0}", key2.ToString());
            //if (keyValue < _minKeyValue)
            //    _minKeyValue = keyValue;
            //if (keyValue > _maxKeyValue)
            //    _maxKeyValue = keyValue;
            ControlKey(key2);

            KeyDefinition keyDefinition;
            if (_keys.TryGetValue((int)key2, out keyDefinition))
            {
                // c'est possible ex : ctrl-K + ctrl-K
                //if (keyDefinition.FirstKey)
                //    throw new PBException("key {0} is already defined as first key", key2.ToString());
                //if (!keyDefinition.FirstKey && !keyDefinition.Multiple)
                //if (keyDefinition.SimpleKey)
                //    throw new PBException("key {0} is already defined as a simple key", key2.ToString());
                keyDefinition.Multiple = true;
                if (keyDefinition.Actions == null)
                    keyDefinition.Actions = new Dictionary<int, Action>();
                if (keyDefinition.Actions.ContainsKey((int)key1))
                {
                    keyDefinition.Actions[(int)key1] = action;
                }
                else
                    keyDefinition.Actions.Add((int)key1, action);
            }
            else
                _keys.Add((int)key2, new KeyDefinition { Multiple = true, Actions = new Dictionary<int, Action> { { (int)key1, action } } });
        }

        // control key value and update _minKeyValue and _maxKeyValue
        private void ControlKey(Keys key)
        {
            int keyValue = (int)(key & Keys.KeyCode);
            //if (keyValue < (int)Keys.Space)
            if (keyValue < (int)Keys.Escape)
                throw new PBException("bad key {0}", key.ToString());
            if (keyValue < _minKeyValue)
                _minKeyValue = keyValue;
            if (keyValue > _maxKeyValue)
                _maxKeyValue = keyValue;
        }

        private void Form_KeyDown(object sender, KeyEventArgs e)
        {
            //if (__trace)
            //    pb.Trace.WriteLine("FormKeyboard : KeyCode {0} KeyValue {1} {2} KeyData {3} Modifiers {4} {5} KeyCode {6,-30} KeyData {7}",
            //        ((int)e.KeyCode).zToHex(), e.KeyValue.zToHex(), (int)e.KeyCode == e.KeyValue ? "ok" : "NOT OK",
            //        ((int)e.KeyData).zToHex(), ((int)e.Modifiers).zToHex(), e.KeyData == (e.KeyCode | e.Modifiers) ? "ok" : "NOT OK", e.KeyCode.ToString(), e.KeyData.ToString());

            bool foundFirstKey = false;
            int keyValue = e.KeyValue;
            if (keyValue >= _minKeyValue && keyValue <= _maxKeyValue)
            {
                int keyData = (int)e.KeyData;
                KeyDefinition keyDefinition;
                if (_keys.TryGetValue(keyData, out keyDefinition))
                {
                    Action action;
                    if (keyDefinition.Multiple && _firstKey != Keys.None && keyDefinition.Actions.TryGetValue((int)_firstKey, out action))
                    {
                        if (__trace)
                            pb.Trace.WriteLine("FormKeyboard : call action for multiple key {0} + {1}", _firstKey.ToString(), e.KeyData.ToString());
                        action();
                    }
                    else if (keyDefinition.FirstKey)
                    {
                        _firstKey = e.KeyData;
                        foundFirstKey = true;
                        if (__trace)
                            pb.Trace.WriteLine("FormKeyboard : set first key {0}", e.KeyData.ToString());
                        if (keyDefinition.Action != null)
                        {
                            if (__trace)
                                pb.Trace.WriteLine("FormKeyboard : call first key action {0}", e.KeyData.ToString());
                            keyDefinition.Action();
                        }
                    }
                    else if (keyDefinition.SimpleKey)
                    {
                        if (__trace)
                            pb.Trace.WriteLine("FormKeyboard : call action for simple key {0}", e.KeyData.ToString());
                        keyDefinition.Action();
                    }
                }
            }
            if (!foundFirstKey && keyValue >= 32)
                _firstKey = Keys.None;

            //bool foundFirstKey = false;
            //int keyValue = e.KeyValue;
            //if (keyValue >= _minKeyValue && keyValue <= _maxKeyValue)
            //{
            //    int keyData = (int)e.KeyData;
            //    KeyDefinition keyDefinition;
            //    if (_keys.TryGetValue(keyData, out keyDefinition))
            //    {
            //        bool found = false;
            //        if (keyDefinition.Multiple)
            //        {
            //            if (_firstKey != Keys.None)
            //            {
            //                Action action;
            //                if (keyDefinition.Actions.TryGetValue((int)_firstKey, out action))
            //                {
            //                    if (__trace)
            //                        pb.Trace.WriteLine("FormKeyboard : call action for multiple key {0} + {1}", _firstKey.ToString(), e.KeyData.ToString());
            //                    found = true;
            //                    action();
            //                }
            //                else if (__trace)
            //                    pb.Trace.WriteLine("FormKeyboard : no definition for multiple key {0} + {1}", _firstKey.ToString(), e.KeyData.ToString());
            //            }
            //        }
            //        if (!found && keyDefinition.FirstKey)
            //        {
            //            _firstKey = e.KeyData;
            //            foundFirstKey = true;
            //            found = true;
            //            if (__trace)
            //                pb.Trace.WriteLine("FormKeyboard : set first key {0}", e.KeyData.ToString());
            //            if (keyDefinition.Action != null)
            //            {
            //                if (__trace)
            //                    pb.Trace.WriteLine("FormKeyboard : call first key action {0}", e.KeyData.ToString());
            //                keyDefinition.Action();
            //            }
            //        }
            //        if (!found && keyDefinition.SimpleKey)
            //        {
            //            if (__trace)
            //                pb.Trace.WriteLine("FormKeyboard : call action for simple key {0}", e.KeyData.ToString());
            //            keyDefinition.Action();
            //        }
            //    }
            //}
            //if (!foundFirstKey && keyValue >= 32)
            //    _firstKey = Keys.None;
        }
    }
}
