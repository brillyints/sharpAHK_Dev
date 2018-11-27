using sharpAHK;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
//using System.Web;
//using System.Configuration;
using sharpAHK_Dev;
using System.Net.Mail;
using Telerik.WinControls.UI;
using System.Collections;
using Telerik.WinControls;
using TelerikExpressions;
using AHKExpressions;
using Telerik.WinControls.UI.Docking;
using System.Reflection;

namespace sharpAHK_Dev
{
    public partial class _TelerikLib
    {
        class _RadImageEditor
        {

            public void Resize(RadImageEditor Editor, int Width = 100, int Height = 100)
            {
                Editor.ImageEditorElement.Resize(Width, Height);
                Editor.ImageEditorElement.SaveState();
            }

            public void CanvasResize(RadImageEditor Editor, Color color, int Width = 100, int Height = 100, ContentAlignment Align = ContentAlignment.BottomCenter)
            {
                Editor.ImageEditorElement.ResizeCanvas(Width, Height, ContentAlignment.BottomRight, color);
                Editor.ImageEditorElement.SaveState();
            }

            public void Rotate(RadImageEditor Editor, RotateFlipType rotate = RotateFlipType.Rotate90FlipNone)
            {
                Editor.ImageEditorElement.RotateFlip(rotate);
                Editor.ImageEditorElement.SaveState();
            }

            public void Flip(RadImageEditor Editor, RotateFlipType rotate = RotateFlipType.Rotate90FlipXY)
            {
                Editor.ImageEditorElement.RotateFlip(RotateFlipType.RotateNoneFlipX);
                Editor.ImageEditorElement.SaveState();
            }

            public void RoundCorners(RadImageEditor Editor)
            {
                Editor.ImageEditorElement.RoundCorners(100, Color.Red, 2, Color.Green);
                Editor.ImageEditorElement.SaveState();
            }

            public void Crop(RadImageEditor Editor)
            {
                Editor.ImageEditorElement.Crop(new Rectangle(0, 0, 20, 20));
                Editor.ImageEditorElement.SaveState();
            }

            public void DrawText(RadImageEditor Editor)
            {
                Editor.ImageEditorElement.DrawString("Test", 12, Color.Red, 200, 200, 120);
                Editor.ImageEditorElement.SaveState();
            }

            public void Undo(RadImageEditor Editor)
            {
                Editor.ImageEditorElement.Undo();
            }

            public void Redo(RadImageEditor Editor)
            {
                Editor.ImageEditorElement.Redo();
            }

            public void Hue(RadImageEditor Editor, int hue)
            {
                Editor.ImageEditorElement.SetHue(hue);
                Editor.ImageEditorElement.SaveState();
            }

            public void Saturation(RadImageEditor Editor, int saturation)
            {
                Editor.ImageEditorElement.SetSaturation(saturation);
                Editor.ImageEditorElement.SaveState();
            }

            public void ContrastBrightness(RadImageEditor Editor, int contrast, int brightness)
            {
                Editor.ImageEditorElement.SetContrastAndBrightness(contrast, brightness);
                Editor.ImageEditorElement.SaveState();
            }

            public void InvertColors(RadImageEditor Editor)
            {
                Editor.ImageEditorElement.InvertColors();
                Editor.ImageEditorElement.SaveState();
            }

            public void Zoom(RadImageEditor Editor, SizeF size)
            {
                Editor.ZoomFactor = size;
            }

            public void Open(RadImageEditor Editor, string filePath)
            {
                Editor.OpenImage(filePath);
            }

            public void SaveImage(RadImageEditor Editor, bool SavePrompt = false)
            {
                if (!SavePrompt) { Editor.SaveImage(); }
                else { Editor.SaveImageAs(); }
            }

        }
    }
}
