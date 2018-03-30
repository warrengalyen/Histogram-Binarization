using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace Binarization
{
    public unsafe partial class FrmTest : Form
    {
        private Bitmap SrcBmp;
        private Bitmap DestBmp;
        private Bitmap HistBmp;
        private Bitmap SmoothHistBmp;
        private int[] Histogram = new int[256];
        private int[] HistGramS = new int[256];
        private int Thr;
        bool Init = false;
        public FrmTest()
        {
            InitializeComponent();
        }


        private void CmdOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.RestoreDirectory = true;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                Bitmap Temp = (Bitmap)Bitmap.FromFile(openFileDialog.FileName);
                if (IsGrayBitmap(Temp) == true)
                    SrcBmp = Temp;
                else
                {
                    SrcBmp = ConvertToGrayBitmap(Temp);
                    Temp.Dispose();
                }
                DestBmp = CreateGrayBitmap(SrcBmp.Width, SrcBmp.Height);
                GetHistGram(SrcBmp, Histogram);
                SrcPic.Image = SrcBmp;
                DestPic.Image = DestBmp;
                Update();
              
            }
            openFileDialog.Dispose();
        }


        private void FrmTest_Load(object sender, EventArgs e)
        {

            CmbMethod.Items.Add("Mean");
            CmbMethod.Items.Add("Huang Fuzzy");
            CmbMethod.Items.Add("Minimum");
            CmbMethod.Items.Add("Intermodes");
            CmbMethod.Items.Add("PTile");
            CmbMethod.Items.Add("Iterative Best");
            CmbMethod.Items.Add("OTSU");
            CmbMethod.Items.Add("1D Max Entropy");
            CmbMethod.Items.Add("Moment Preserving");
            CmbMethod.Items.Add("Kittler Minimum Error");
            CmbMethod.Items.Add("ISODATA");
            CmbMethod.Items.Add("Shanbhag");
            CmbMethod.Items.Add("Yen"); 
            CmbMethod.SelectedIndex = 2;
            SrcBmp = global::Binarization.Properties.Resources.Lena;
            DestBmp = CreateGrayBitmap(SrcBmp.Width, SrcBmp.Height);
            GetHistGram(SrcBmp, Histogram);
            SrcPic.Image = SrcBmp;
            DestPic.Image = DestBmp;
            HistBmp = CreateGrayBitmap(256, 100);
            SmoothHistBmp = CreateGrayBitmap(256, 100);
            PicHist.Image = HistBmp;
            PicSmoothHist.Image = SmoothHistBmp;
            Update();
            Init = true;
        }

        private Bitmap CreateGrayBitmap(int Width, int Height)
        {
            Bitmap Bmp = new Bitmap(Width, Height, PixelFormat.Format8bppIndexed);
            ColorPalette Pal = Bmp.Palette;
            for (int Y = 0; Y < Pal.Entries.Length; Y++) Pal.Entries[Y] = Color.FromArgb(255, Y, Y, Y);
            Bmp.Palette = Pal;
            return Bmp;
        }

        private bool IsGrayBitmap(Bitmap Bmp)
        {
            if (Bmp.PixelFormat != PixelFormat.Format8bppIndexed) return false;
            if (Bmp.Palette.Entries.Length != 256) return false;
            for (int Y = 0; Y < Bmp.Palette.Entries.Length; Y++)
                if (Bmp.Palette.Entries[Y] != Color.FromArgb(255, Y, Y, Y)) return false;
            return true;
        }

        private Bitmap ConvertToGrayBitmap(Bitmap Src)
        {
            Bitmap Dest = CreateGrayBitmap(Src.Width, Src.Height);
            BitmapData SrcData = Src.LockBits(new Rectangle(0, 0, Src.Width, Src.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            BitmapData DestData = Dest.LockBits(new Rectangle(0, 0, Dest.Width, Dest.Height), ImageLockMode.ReadWrite, Dest.PixelFormat);
            int Width = SrcData.Width, Height = SrcData.Height;
            int SrcStride = SrcData.Stride, DestStride = DestData.Stride;
            byte* SrcP, DestP;
            for (int Y = 0; Y < Height; Y++)
            {
                SrcP = (byte*)SrcData.Scan0 + Y * SrcStride;         // The unsafe function must be turned on somewhere. In fact, unsafe in C# is very safe and it is awesome.           
                DestP = (byte*)DestData.Scan0 + Y * DestStride;
                for (int X = 0; X < Width; X++)
                {
                    *DestP = (byte)((*SrcP + (*(SrcP + 1) << 1) + *(SrcP + 2)) >> 2);
                    SrcP += 3;
                    DestP++;
                }
            }
            Src.UnlockBits(SrcData);
            Dest.UnlockBits(DestData);
            return Dest;
        }

        private void GetHistGram(Bitmap Src, int[] Histogram)
        {
            BitmapData SrcData = Src.LockBits(new Rectangle(0, 0, Src.Width, Src.Height), ImageLockMode.ReadWrite, Src.PixelFormat);
            int Width = SrcData.Width, Height = SrcData.Height, SrcStride = SrcData.Stride;
            byte* SrcP;
            for (int Y = 0; Y < 256; Y++) Histogram[Y] = 0;
            for (int Y = 0; Y < Height; Y++)
            {
                SrcP = (byte*)SrcData.Scan0 + Y * SrcStride;
                for (int X = 0; X < Width; X++, SrcP++) Histogram[*SrcP]++;
            }
            Src.UnlockBits(SrcData);
        }

        private void DoBinarization(Bitmap Src, Bitmap Dest, int Threshold)
        {
            if (Threshold == -1)
            {
                MessageBox.Show("Illegal threshold variable selected.");
                return;
            }
            BitmapData SrcData = Src.LockBits(new Rectangle(0, 0, Src.Width, Src.Height), ImageLockMode.ReadWrite, Src.PixelFormat);
            BitmapData DestData = Dest.LockBits(new Rectangle(0, 0, Dest.Width, Dest.Height), ImageLockMode.ReadWrite, Dest.PixelFormat);
            int Width = SrcData.Width, Height = SrcData.Height;
            int SrcStride = SrcData.Stride, DestStride = DestData.Stride;
            byte* SrcP, DestP;
            for (int Y = 0; Y < Height; Y++)
            {
                SrcP = (byte*)SrcData.Scan0 + Y * SrcStride;            
                DestP = (byte*)DestData.Scan0 + Y * DestStride;
                for (int X = 0; X < Width; X++, SrcP++, DestP++)
                    *DestP = *SrcP > Threshold ? byte.MaxValue : byte.MinValue;  
            }
            Src.UnlockBits(SrcData);
            Dest.UnlockBits(DestData);
            DestPic.Invalidate();
            LblThreshold.Text = Threshold.ToString();
        }

        private int GetThreshold()
        {
            switch (CmbMethod.SelectedItem.ToString())
            {
                case "Mean":
                    return Threshold.GetMeanThreshold(Histogram);
                case "Huang Fuzzy":
                    return Threshold.GetHuangFuzzyThreshold(Histogram);
                case "Minimum":
                    return Threshold.GetMinimumThreshold(Histogram,  HistGramS);
                case "Intermodes":
                    return Threshold.GetIntermodesThreshold(Histogram,  HistGramS);
                case "PTile":
                    return Threshold.GetPTileThreshold(Histogram);
                case "Iterative Best":
                    return Threshold.GetIterativeBestThreshold(Histogram);
                case "OTSU":
                    return Threshold.GetOSTUThreshold(Histogram);
                case "1D Max Entropy":
                    return Threshold.Get1DMaxEntropyThreshold(Histogram);
                case "Moment Preserving":
                    return Threshold.GetMomentPreservingThreshold(Histogram);
                case "Kittler Minimum Error":
                    return Threshold.GetKittlerMinError(Histogram);
                case "ISODATA":
                    return Threshold.GetIsoDataThreshold(Histogram);
                case "Shanbhag":
                    return Threshold.GetShanbhagThreshold(Histogram);
                case "Yen":
                    return Threshold.GetYenThreshold(Histogram);
                default:
                    break;
            }
            return -1;
        }

        public void DrawHistGram(Bitmap SrcBmp,int []Histgram)
        {
            BitmapData HistData = SrcBmp.LockBits(new Rectangle(0, 0, SrcBmp.Width, SrcBmp.Height), ImageLockMode.ReadWrite, SrcBmp.PixelFormat);
            int X, Y, Max = 0;
            byte* P;
            for (Y = 0; Y < 256; Y++) if (Max < Histgram[Y]) Max = Histgram[Y];
            for (X = 0; X < 256; X++)
            {
                P = (byte*)HistData.Scan0 + X;
                for (Y = 0; Y < 100; Y++)
                {
                    if ((100 - Y) > Histgram[X] * 100 / Max)
                        *P = 220;
                    else
                        *P = 0;
                    P += HistData.Stride;
                }
            }

            P = (byte*)HistData.Scan0 + Thr;
            for (Y = 0; Y < 100; Y++)
            {
                *P = 255;
                P += HistData.Stride;
            }
            SrcBmp.UnlockBits(HistData);
        }

        private void CmbMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Init == true) Update();
        }

        private void Update()
        {
            Thr = GetThreshold();
            DoBinarization(SrcBmp, DestBmp, Thr);
            DrawHistGram(HistBmp, Histogram);
            PicHist.Invalidate();
            if (CmbMethod.SelectedItem.ToString() == "Minimum" || CmbMethod.SelectedItem.ToString() == "Intermodes")
            {
                DrawHistGram(SmoothHistBmp,HistGramS);
                PicSmoothHist.Invalidate();
            }
        }

        private void CmdSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Bitmap files (*.Bitmap)|*.Bmp|Jpeg files (*.jpg)|*.jpg|Png files (*.png)|*.png";
            saveFileDialog.FilterIndex = 4;
            saveFileDialog.RestoreDirectory = true;
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                DestPic.Image.Save(saveFileDialog.FileName);

            }
        }


    }
}
