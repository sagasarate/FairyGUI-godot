using System;
using System.Collections.Generic;
using Godot;

namespace FairyGUI
{
    /// <summary>
    /// 
    /// </summary>
    public class FontManager
    {
        public static Dictionary<string, Font> sFontFactory = new Dictionary<string, Font>();
        static public void RegisterFont(Font font, string alias = null)
        {
            sFontFactory[font.GetFontName()] = font;
            if (alias != null)
                sFontFactory[alias] = font;
        }
        static public void UnregisterFont(Font font)
        {
            List<string> toDelete = new List<string>();
            foreach (KeyValuePair<string, Font> kv in sFontFactory)
            {
                if (kv.Value == font)
                    toDelete.Add(kv.Key);
            }

            foreach (string key in toDelete)
                sFontFactory.Remove(key);
        }
        static public Font GetFont(string fontPath)
        {
            if (string.IsNullOrEmpty(fontPath))
            {
                return fallbackFont;
            }
            Font font;
            if (fontPath.StartsWith(UIPackage.URL_PREFIX))
            {
                font = UIPackage.GetItemAssetByURL(fontPath) as Font;
                if (font != null)
                    return font;
            }

            if (sFontFactory.TryGetValue(fontPath, out font))
                return font;
            if (fontPath.StartsWith("res://"))
            {
                font = ResourceLoader.Load<Font>(fontPath);
                if (font == null)
                {
                    GD.PushWarning($"font {fontPath} not found.");
                    return fallbackFont;
                }
            }
            else
            {
                SystemFont sysFont = new SystemFont();
                sysFont.FontNames = new string[] { fontPath };
                font = sysFont;
            }
            RegisterFont(font, fontPath);
            return font;
        }
        public static Font fallbackFont
        {
            get
            {
                Font font;
                if (!sFontFactory.TryGetValue("$default_font", out font))
                {
                    font = ThemeDB.FallbackFont;
                    RegisterFont(font, "$default_font");
                }
                return font;
            }
        }
        static public void Clear()
        {
            foreach (KeyValuePair<string, Font> kv in sFontFactory)
                kv.Value.Dispose();
            sFontFactory.Clear();
        }
    }
}
