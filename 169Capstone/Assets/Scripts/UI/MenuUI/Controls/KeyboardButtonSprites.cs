using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Controls/KeyboardButtonSprites")]
public class KeyboardButtonSprites : DeviceButtonSpritesObject
{
    [Header("Letter Keys")]
    [SerializeField] private Sprite keyA;
    [SerializeField] private Sprite keyB;
    [SerializeField] private Sprite keyC;
    [SerializeField] private Sprite keyD;
    [SerializeField] private Sprite keyE;
    [SerializeField] private Sprite keyF;
    [SerializeField] private Sprite keyG;
    [SerializeField] private Sprite keyH;
    [SerializeField] private Sprite keyI;
    [SerializeField] private Sprite keyJ;
    [SerializeField] private Sprite keyK;
    [SerializeField] private Sprite keyL;
    [SerializeField] private Sprite keyM;
    [SerializeField] private Sprite keyN;
    [SerializeField] private Sprite keyO;
    [SerializeField] private Sprite keyP;
    [SerializeField] private Sprite keyQ;
    [SerializeField] private Sprite keyR;
    [SerializeField] private Sprite keyS;
    [SerializeField] private Sprite keyT;
    [SerializeField] private Sprite keyU;
    [SerializeField] private Sprite keyV;
    [SerializeField] private Sprite keyW;
    [SerializeField] private Sprite keyX;
    [SerializeField] private Sprite keyY;
    [SerializeField] private Sprite keyZ;

    // Return for both normal keys as well as numpad
    [Header("Number Keys")]
    [SerializeField] private Sprite key0;
    [SerializeField] private Sprite key1;
    [SerializeField] private Sprite key2;
    [SerializeField] private Sprite key3;
    [SerializeField] private Sprite key4;
    [SerializeField] private Sprite key5;
    [SerializeField] private Sprite key6;
    [SerializeField] private Sprite key7;
    [SerializeField] private Sprite key8;
    [SerializeField] private Sprite key9;

    [Header("Arrow Keys")]
    [SerializeField] private Sprite arrowLeft;
    [SerializeField] private Sprite arrowRight;
    [SerializeField] private Sprite arrowUp;
    [SerializeField] private Sprite arrowDown;

    [Header("Function Keys")]
    [SerializeField] private Sprite f1;
    [SerializeField] private Sprite f2;
    [SerializeField] private Sprite f3;
    [SerializeField] private Sprite f4;
    [SerializeField] private Sprite f5;
    [SerializeField] private Sprite f6;
    [SerializeField] private Sprite f7;
    [SerializeField] private Sprite f8;
    [SerializeField] private Sprite f9;
    [SerializeField] private Sprite f10;
    [SerializeField] private Sprite f11;
    [SerializeField] private Sprite f12;

    [Header("Symbols")]
    [SerializeField] private Sprite leftBracket;
    [SerializeField] private Sprite rightBracket;
    [SerializeField] private Sprite plusSign;
    [SerializeField] private Sprite plusSignTall;
    [SerializeField] private Sprite slash;
    [SerializeField] private Sprite tilda;
    [SerializeField] private Sprite asteriskNumpad;
    [SerializeField] private Sprite minus;
    [SerializeField] private Sprite leftMark;
    [SerializeField] private Sprite rightMark;
    [SerializeField] private Sprite quote;
    [SerializeField] private Sprite semiColon;

    [Header("Other Keys")]
    [SerializeField] private Sprite spacebar;
    [SerializeField] private Sprite capsLock;
    [SerializeField] private Sprite shift;
    [SerializeField] private Sprite shiftLong;
    [SerializeField] private Sprite tab;
    [SerializeField] private Sprite alt;
    [SerializeField] private Sprite control;
    [SerializeField] private Sprite backspace;
    [SerializeField] private Sprite insert;
    [SerializeField] private Sprite home;
    [SerializeField] private Sprite pageUp;
    [SerializeField] private Sprite pageDown;
    [SerializeField] private Sprite delete;
    [SerializeField] private Sprite end;
    [SerializeField] private Sprite printScreen;
    [SerializeField] private Sprite numLock;
    [SerializeField] private Sprite enter;
    [SerializeField] private Sprite enterTall;
    [SerializeField] private Sprite windows;
    [SerializeField] private Sprite escape;

    [Header("Mouse Input")]
    [SerializeField] private Sprite mouse;
    [SerializeField] private Sprite leftMouseButton;
    [SerializeField] private Sprite rightMouseButton;
    [SerializeField] private Sprite middleMouseButton;

    public override Sprite GetSprite(string controlPath)
    {
        // From the input system, we get the path of the control on device. So we can just
        // map from that to the sprites we have for gamepads.
        switch (controlPath)
        {
            // Letter Keys (might have to be lowercase?)
            case "A": return keyA;
            case "B": return keyB;
            case "C": return keyC;
            case "D": return keyD;
            case "E": return keyE;
            case "F": return keyF;
            case "G": return keyG;
            case "H": return keyH;
            case "I": return keyI;
            case "J": return keyJ;
            case "K": return keyK;
            case "L": return keyL;
            case "M": return keyM;
            case "N": return keyN;
            case "O": return keyO;
            case "P": return keyP;
            case "Q": return keyQ;
            case "R": return keyR;
            case "S": return keyS;
            case "T": return keyT;
            case "U": return keyU;
            case "V": return keyV;
            case "W": return keyW;
            case "X": return keyX;
            case "Y": return keyY;
            case "Z": return keyZ;

            // Number Keys
            case "0": return key0;
            case "Numpad 0": return key0;
            case "1": return key1;
            case "Numpad 1": return key1;
            case "2": return key2;
            case "Numpad 2": return key2;
            case "3": return key3;
            case "Numpad 3": return key3;
            case "4": return key4;
            case "Numpad 4": return key4;
            case "5": return key5;
            case "Numpad 5": return key5;
            case "6": return key6;
            case "Numpad 6": return key6;
            case "7": return key7;
            case "Numpad 7": return key7;
            case "8": return key8;
            case "Numpad 8": return key8;
            case "9": return key9;
            case "Numpad 9": return key9;

            // Arrow Keys
            case "Left Arrow": return arrowLeft;
            case "Right Arrow": return arrowRight;
            case "Up Arrow": return arrowUp;
            case "Down Arrow": return arrowDown;

            // Function Keys
            case "F1": return f1;
            case "F2": return f2;
            case "F3": return f3;
            case "F4": return f4;
            case "F5": return f5;
            case "F6": return f6;
            case "F7": return f7;
            case "F8": return f8;
            case "F9": return f9;
            case "F10": return f10;
            case "F11": return f11;
            case "F12": return f12;

            // Symbols
            case "[": return leftBracket;
            case "]": return rightBracket;
            case "=": return plusSign;
            case "Numpad +": return plusSignTall;
            case "/": return slash;
            case "Numpad /": return slash;
            case "`": return tilda;
            case "Numpad *": return asteriskNumpad;
            case "-": return minus;
            case "Numpad -": return minus;
            case ",": return leftMark;
            case ".": return rightMark;
            case "'": return quote;
            case ";": return semiColon;

            // Other Keys
            case "Space": return spacebar;
            case "Caps Lock": return capsLock;
            case "Shift": return shift;
            case "Left Shift": return shift;
            case "Right Shift": return shiftLong;
            case "Tab": return tab;
            case "Alt": return alt;
            case "Right Alt": return alt;
            case "Left Alt": return alt;
            case "Control": return control;
            case "Right Control": return control;
            case "Left Control": return control;
            case "Backspace": return backspace;
            case "Insert": return insert;
            case "Home": return home;
            case "Page Up": return pageUp;
            case "Page Down": return pageDown;
            case "Delete": return delete;
            case "End": return end;
            case "Print Screen": return printScreen;
            case "Num Lock": return numLock;
            case "Enter": return enter;
            case "Numpad Enter": return enterTall;
            case "Left System": return windows;
            // TODO check for an escape button
            case "Escape": return escape;

            // Mouse Input
            case "Right Button": return rightMouseButton;
            case "Left Button": return leftMouseButton;
            case "Middle Button": return middleMouseButton;
            case "Mouse": return mouse; // Don't know what to put for this one
        }
        Debug.LogWarning("No icon found for control path: " + controlPath);
        return null;
    }
}
