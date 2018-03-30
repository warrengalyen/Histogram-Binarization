using System;

namespace Binarization
{
    public static class Threshold
    {
        /// <summary>
        /// Threshold based on gray average
        /// </summary>
        /// <param name="Histogram">Histogram of grayscale image</param>
        /// <returns></returns>
        public static int GetMeanThreshold(int[] Histogram)
        {
            int Sum = 0, Amount = 0;
            for (int Y = 0; Y < 256; Y++)
            {
                Amount += Histogram[Y];
                Sum += Y * Histogram[Y];
            }
            return Sum / Amount;
        }
        /// <summary>
        /// 基于模糊集的黄式阈值算法
        /// http://www.ktl.elf.stuba.sk/study/vacso/Zadania-Cvicenia/Cvicenie_3/TimA2/Huang_E016529624.pdf
        /// </summary>
        /// <param name="Histogram">Histogram of grayscale image</param>
        /// <returns></returns>

        public static int GetHuangFuzzyThreshold(int[] Histogram)
        {
            int X, Y;
            int First, Last;
            int Threshold = -1;
            double BestEntropy = Double.MaxValue, Entropy;
            //   Find the first and last non-zero level values
            for (First = 0; First < Histogram.Length && Histogram[First] == 0; First++) ;
            for (Last = Histogram.Length - 1; Last > First && Histogram[Last] == 0; Last--) ;
            if (First == Last) return First;                // There is only one color in the image
            if (First + 1 == Last) return First;            // There are only two colors in the image

            // Calculate the cumulative histogram and the corresponding cumulative histogram with weights
            int[] S = new int[Last + 1];
            int[] W = new int[Last + 1];            // 对于特大图，此数组的保存数据可能会超出int的表示范围，可以考虑用long类型来代替
            S[0] = Histogram[0];
            for (Y = First > 1 ? First : 1; Y <= Last; Y++)
            {
                S[Y] = S[Y - 1] + Histogram[Y];
                W[Y] = W[Y - 1] + Y * Histogram[Y];
            }

            // Establish the lookup tables used in equations (4) and (6)
            double[] Sum = new double[Last + 1 - First];
            for (Y = 1; Y < Sum.Length; Y++)
            {
                double mu = 1 / (1 + (double)Y / (Last - First));               // Eq.（4）
                Sum[Y] = -mu * Math.Log(mu) - (1 - mu) * Math.Log(1 - mu);      // Eq.（6）
            }

            // Iterative calculation of the optimal threshold
            for (Y = First; Y <= Last; Y++)
            {
                Entropy = 0;
                int mu = (int)Math.Round((double)W[Y] / S[Y]);             // 公式17
                for (X = First; X <= Y; X++)
                    Entropy += Sum[Math.Abs(X - mu)] * Histogram[X];
                mu = (int)Math.Round((double)(W[Last] - W[Y]) / (S[Last] - S[Y]));  // 公式18       
                for (X = Y + 1; X <= Last; X++)
                    Entropy += Sum[Math.Abs(X - mu)] * Histogram[X];       // 公式8
                if (BestEntropy > Entropy)
                {
                    BestEntropy = Entropy;      // Take the minimum entropy as the best threshold
                    Threshold = Y;
                }
            }
            return Threshold;
        }


        /// <summary>
        /// Threshold based on the valley bottom
        /// This method is applied to an image with a clear bimodal histogram, which 
        /// looks for a double peak valley as a threshold.
        /// References: 
        /// J. M. S. Prewitt and M. L. Mendelsohn, "The analysis of cell images," in
        /// nnals of the New York Academy of Sciences, vol. 128, pp. 1035-1053, 1966.
        /// C. A. Glasbey, "An analysis of histogram-based thresholding algorithms,"
        /// CVGIP: Graphical Models and Image Processing, vol. 55, pp. 532-537, 1993.
        /// </summary>
        /// <param name="Histogram">Histogram of grayscale image</param>
        /// <param name="HistGramS">Return smoothed histogram</param>
        /// <returns></returns>
        public static int GetMinimumThreshold(int[] Histogram, int[] HistogramS)
        {
            int Y, Iter = 0;
            double[] HistGramC = new double[256];           // We must use floating-point for accuracy, otherwise we can't get the correct result
            double[] HistGramCC = new double[256];          // The process of averaging will destroy the original data, so we copy it
            for (Y = 0; Y < 256; Y++)
            {
                HistGramC[Y] = Histogram[Y];
                HistGramCC[Y] = Histogram[Y];
            }

            // Smooth the histogram by averaging three points
            while (IsDimodal(HistGramCC) == false)                                        // To determine if it is already a bimodal image      
            {
                HistGramCC[0] = (HistGramC[0] + HistGramC[0] + HistGramC[1]) / 3;                 // The first point
                for (Y = 1; Y < 255; Y++)
                    HistGramCC[Y] = (HistGramC[Y - 1] + HistGramC[Y] + HistGramC[Y + 1]) / 3;     // The middle point
                HistGramCC[255] = (HistGramC[254] + HistGramC[255] + HistGramC[255]) / 3;         // the last point
                System.Buffer.BlockCopy(HistGramCC, 0, HistGramC, 0, 256 * sizeof(double));
                Iter++;
                if (Iter >= 1000) return -1;                                                   // Histogram cannot smooth bimodal, return error code
            }
            for (Y = 0; Y < 256; Y++) HistogramS[Y] = (int)HistGramCC[Y];
            // The threshold is extremely low between the two peaks 
            bool Peakfound = false;
            for (Y = 1; Y < 255; Y++)
            {
                if (HistGramCC[Y - 1] < HistGramCC[Y] && HistGramCC[Y + 1] < HistGramCC[Y]) Peakfound = true;
                if (Peakfound == true && HistGramCC[Y - 1] >= HistGramCC[Y] && HistGramCC[Y + 1] >= HistGramCC[Y])
                    return Y - 1;
            }
            return -1;
        }

        /// <summary>
        /// Threshold based on bimodal averages
        /// This method is applied to an image with a clear bimodal histogram, which
        /// looks for a double peak valley as a threshold.
        /// References: 
        /// J. M. S. Prewitt and M. L. Mendelsohn, "The analysis of cell images," in
        /// nnals of the New York Academy of Sciences, vol. 128, pp. 1035-1053, 1966.
        /// C. A. Glasbey, "An analysis of histogram-based thresholding algorithms,"
        /// CVGIP: Graphical Models and Image Processing, vol. 55, pp. 532-537, 1993.
        /// </summary>
        /// <param name="Histogram">Histogram of grayscale image</param>
        /// <param name="HistogramS">Return smoothed histogram</param>
        /// <returns></returns>
        public static int GetIntermodesThreshold(int[] Histogram, int[] HistGramS)
        {
            int Y, Iter = 0, Index;
            double[] HistGramC = new double[256];           // We must use floating-point for accuracy, otherwise we can't get the correct result
            double[] HistGramCC = new double[256];          // The process of averaging will destroy the original data, so we copy it
            for (Y = 0; Y < 256; Y++)
            {
                HistGramC[Y] = Histogram[Y];
                HistGramCC[Y] = Histogram[Y];
            }
            // Smooth the histogram by averaging three points
            while (IsDimodal(HistGramCC) == false)                                                  // To determine if it is already a bimodal image      
            {
                HistGramCC[0] = (HistGramC[0] + HistGramC[0] + HistGramC[1]) / 3;                   // The first point
                for (Y = 1; Y < 255; Y++)
                    HistGramCC[Y] = (HistGramC[Y - 1] + HistGramC[Y] + HistGramC[Y + 1]) / 3;       // The middle point
                HistGramCC[255] = (HistGramC[254] + HistGramC[255] + HistGramC[255]) / 3;           // the last point
                System.Buffer.BlockCopy(HistGramCC, 0, HistGramC, 0, 256 * sizeof(double));         // Back up data to prepare for the next iteration
                Iter++;
                if (Iter >= 10000) return -1;                                                       // Histogram cannot smooth bimodal, return error code
            }
            for (Y = 0; Y < 256; Y++) HistGramS[Y] = (int)HistGramCC[Y];
            // The threshold is the average of two peaks
            int[] Peak = new int[2];
            for (Y = 1, Index = 0; Y < 255; Y++)
                if (HistGramCC[Y - 1] < HistGramCC[Y] && HistGramCC[Y + 1] < HistGramCC[Y]) Peak[Index++] = Y - 1;
            return ((Peak[0] + Peak[1]) / 2);
        }

        /// <summary>
        /// Percentage threshold
        /// </summary>
        /// <param name="Histogram">Histogram of grayscale image</param>
        /// <param name="Tile">The percentage of the background area in the image</param>
        /// <returns></returns>
        public static int GetPTileThreshold(int[] Histogram, int Tile = 50)
        {
            int Y, Amount = 0, Sum = 0;
            for (Y = 0; Y < 256; Y++) Amount += Histogram[Y];        //  Total number of pixels
            for (Y = 0; Y < 256; Y++)
            {
                Sum = Sum + Histogram[Y];
                if (Sum >= Amount * Tile / 100) return Y;
            }
            return -1;
        }

        /// <summary>
        /// Iterative method to obtain the threshold
        /// </summary>
        /// <param name="Histogram">Histogram of grayscale image</param>
        /// <returns></returns>
        public static int GetIterativeBestThreshold(int[] Histogram)
        {
            int X, Iter = 0;
            int MeanValueOne, MeanValueTwo, SumOne, SumTwo, SumIntegralOne, SumIntegralTwo;
            int MinValue, MaxValue;
            int Threshold, NewThreshold;

            for (MinValue = 0; MinValue < 256 && Histogram[MinValue] == 0; MinValue++) ;
            for (MaxValue = 255; MaxValue > MinValue && Histogram[MinValue] == 0; MaxValue--) ;

            if (MaxValue == MinValue) return MaxValue;          // There is only one color in the image             
            if (MinValue + 1 == MaxValue) return MinValue;      // There are only two colors in the image

            Threshold = MinValue;
            NewThreshold = (MaxValue + MinValue) >> 1;
            while (Threshold != NewThreshold)    // When the last two iterations have the same threshold, the iteration ends.   
            {
                SumOne = 0; SumIntegralOne = 0;
                SumTwo = 0; SumIntegralTwo = 0;
                Threshold = NewThreshold;
                for (X = MinValue; X <= Threshold; X++)         // According to the threshold, the image is divided into two parts: target and background, and the average gray value of the two parts is obtained.    
                {
                    SumIntegralOne += Histogram[X] * X;
                    SumOne += Histogram[X];
                }
                MeanValueOne = SumIntegralOne / SumOne;
                for (X = Threshold + 1; X <= MaxValue; X++)
                {
                    SumIntegralTwo += Histogram[X] * X;
                    SumTwo += Histogram[X];
                }
                MeanValueTwo = SumIntegralTwo / SumTwo;
                NewThreshold = (MeanValueOne + MeanValueTwo) >> 1;       // Find new threshold
                Iter++;
                if (Iter >= 1000) return -1;
            }
            return Threshold;
        }

        /// <summary>
        /// Otsu's threshold algorithm
        /// Reference:
        /// M. Emre Celebi 6.15.2007, Fourier Library https://sourceforge.net/projects/fourier-ipal/
        /// </summary>
        /// <param name="Histogram">Histogram of grayscale image</param>
        /// <returns></returns>
        public static int GetOSTUThreshold(int[] Histogram)
        {
            int X, Y, Amount = 0;
            int PixelBack = 0, PixelFore = 0, PixelIntegralBack = 0, PixelIntegralFore = 0, PixelIntegral = 0;
            double OmegaBack, OmegaFore, MicroBack, MicroFore, SigmaB, Sigma;              // Variance between classes
            int MinValue, MaxValue;
            int Threshold = 0;

            for (MinValue = 0; MinValue < 256 && Histogram[MinValue] == 0; MinValue++) ;
            for (MaxValue = 255; MaxValue > MinValue && Histogram[MinValue] == 0; MaxValue--) ;
            if (MaxValue == MinValue) return MaxValue;          // There is only one color in the image             
            if (MinValue + 1 == MaxValue) return MinValue;      // There are only two colors in the image

            for (Y = MinValue; Y <= MaxValue; Y++) Amount += Histogram[Y];        //  Total number of pixels

            PixelIntegral = 0;
            for (Y = MinValue; Y <= MaxValue; Y++) PixelIntegral += Histogram[Y] * Y;
            SigmaB = -1;
            for (Y = MinValue; Y < MaxValue; Y++)
            {
                PixelBack = PixelBack + Histogram[Y];
                PixelFore = Amount - PixelBack;
                OmegaBack = (double)PixelBack / Amount;
                OmegaFore = (double)PixelFore / Amount;
                PixelIntegralBack += Histogram[Y] * Y;
                PixelIntegralFore = PixelIntegral - PixelIntegralBack;
                MicroBack = (double)PixelIntegralBack / PixelBack;
                MicroFore = (double)PixelIntegralFore / PixelFore;
                Sigma = OmegaBack * OmegaFore * (MicroBack - MicroFore) * (MicroBack - MicroFore);
                if (Sigma > SigmaB)
                {
                    SigmaB = Sigma;
                    Threshold = Y;
                }
            }
            return Threshold;
        }

        /// <summary>
        /// Max Entropy thresholding method
        /// Reference:
        /// Kapur J.N., Sahoo P.K., and Wong A.K.C. (1985) "A New Method for
        /// Gray-Level Picture Thresholding Using the Entropy of the Histogram"
        /// Graphical Models and Image Processing, 29(3): 273-285
        /// M. Emre Celebi 06.15.2007
        /// </summary>
        /// <param name="Histogram">Histogram of grayscale image</param>
        /// <returns></returns>
        public static int Get1DMaxEntropyThreshold(int[] Histogram)
        {
            int X, Y, Amount = 0;
            double[] HistGramD = new double[256];
            double SumIntegral, EntropyBack, EntropyFore, MaxEntropy;
            int MinValue = 255, MaxValue = 0;
            int Threshold = 0;

            for (MinValue = 0; MinValue < 256 && Histogram[MinValue] == 0; MinValue++) ;
            for (MaxValue = 255; MaxValue > MinValue && Histogram[MinValue] == 0; MaxValue--) ;
            if (MaxValue == MinValue) return MaxValue;          // There is only one color in the image             
            if (MinValue + 1 == MaxValue) return MinValue;      // There are only two colors in the image

            for (Y = MinValue; Y <= MaxValue; Y++) Amount += Histogram[Y];        //  Total number of pixels

            for (Y = MinValue; Y <= MaxValue; Y++) HistGramD[Y] = (double)Histogram[Y] / Amount + 1e-17;

            MaxEntropy = double.MinValue; ;
            for (Y = MinValue + 1; Y < MaxValue; Y++)
            {
                SumIntegral = 0;
                for (X = MinValue; X <= Y; X++) SumIntegral += HistGramD[X];
                EntropyBack = 0;
                for (X = MinValue; X <= Y; X++) EntropyBack += (-HistGramD[X] / SumIntegral * Math.Log(HistGramD[X] / SumIntegral));
                EntropyFore = 0;
                for (X = Y + 1; X <= MaxValue; X++) EntropyFore += (-HistGramD[X] / (1 - SumIntegral) * Math.Log(HistGramD[X] / (1 - SumIntegral)));
                if (MaxEntropy < EntropyBack + EntropyFore)
                {
                    Threshold = Y;
                    MaxEntropy = EntropyBack + EntropyFore;
                }
            }
            return Threshold;
        }

        /// <summary>
        /// Moment preserving threshold method
        /// References:
        /// http://fiji.sc/wiki/index.php/Auto_Threshold#Huang
        ///   W. Tsai, "Moment-preserving thresholding: a new approach," Computer
        ///   Vision, Graphics, and Image Processing, vol. 29, pp. 377-393, 1985.
        ///
        ///  C. A. Glasbey, "An analysis of histogram-based thresholding algorithms,"
        ///  CVGIP: Graphical Models and Image Processing, vol. 55, pp. 532-537, 1993.
        /// </summary>
        /// <param name="Histogram">Histogram of grayscale image</param>
        /// <returns></returns>
        public static byte GetMomentPreservingThreshold(int[] Histogram)
        {
            int X, Y, Index = 0, Amount = 0;
            double[] Avec = new double[256];
            double X2, X1, X0, Min;

            for (Y = 0; Y <= 255; Y++) Amount += Histogram[Y];        //  Total number of pixels
            for (Y = 0; Y < 256; Y++) Avec[Y] = (double)A(Histogram, Y) / Amount;       // The threshold is chosen such that A(y,t)/A(y,n) is closest to x0.

            // The following finds x0.

            X2 = (double)(B(Histogram, 255) * C(Histogram, 255) - A(Histogram, 255) * D(Histogram, 255)) / (double)(A(Histogram, 255) * C(Histogram, 255) - B(Histogram, 255) * B(Histogram, 255));
            X1 = (double)(B(Histogram, 255) * D(Histogram, 255) - C(Histogram, 255) * C(Histogram, 255)) / (double)(A(Histogram, 255) * C(Histogram, 255) - B(Histogram, 255) * B(Histogram, 255));
            X0 = 0.5 - (B(Histogram, 255) / A(Histogram, 255) + X2 / 2) / Math.Sqrt(X2 * X2 - 4 * X1);

            for (Y = 0, Min = double.MaxValue; Y < 256; Y++)
            {
                if (Math.Abs(Avec[Y] - X0) < Min)
                {
                    Min = Math.Abs(Avec[Y] - X0);
                    Index = Y;
                }
            }
            return (byte)Index;
        }

        /// <summary>
        /// Minimum Error thresholding method
        /// References:
        /// Kittler and J. Illingworth, "Minimum error thresholding," Pattern Recognition, vol. 19, pp. 41-47, 1986.
        /// C. A. Glasbey, "An analysis of histogram-based thresholding algorithms," CVGIP: Graphical Models and Image Processing, vol. 55, pp. 532-537, 1993.
        /// </summary>
        /// <param name="Histogram">Histogram of grayscale image</param>
        /// <returns></returns>
        public static int GetKittlerMinError(int[] Histogram)
        {
            int X, Y;
            int MinValue, MaxValue;
            int Threshold;
            int PixelBack, PixelFore;
            double OmegaBack, OmegaFore, MinSigma, Sigma, SigmaBack, SigmaFore;
            for (MinValue = 0; MinValue < 256 && Histogram[MinValue] == 0; MinValue++) ;
            for (MaxValue = 255; MaxValue > MinValue && Histogram[MinValue] == 0; MaxValue--) ;
            if (MaxValue == MinValue) return MaxValue;          // There is only one color in the image             
            if (MinValue + 1 == MaxValue) return MinValue;      // There are only two colors in the image
            Threshold = -1;
            MinSigma = 1E+20;
            for (Y = MinValue; Y < MaxValue; Y++)
            {
                PixelBack = 0; PixelFore = 0;
                OmegaBack = 0; OmegaFore = 0;
                for (X = MinValue; X <= Y; X++)
                {
                    PixelBack += Histogram[X];
                    OmegaBack = OmegaBack + X * Histogram[X];
                }
                for (X = Y + 1; X <= MaxValue; X++)
                {
                    PixelFore += Histogram[X];
                    OmegaFore = OmegaFore + X * Histogram[X];
                }
                OmegaBack = OmegaBack / PixelBack;
                OmegaFore = OmegaFore / PixelFore;
                SigmaBack = 0; SigmaFore = 0;
                for (X = MinValue; X <= Y; X++) SigmaBack = SigmaBack + (X - OmegaBack) * (X - OmegaBack) * Histogram[X];
                for (X = Y + 1; X <= MaxValue; X++) SigmaFore = SigmaFore + (X - OmegaFore) * (X - OmegaFore) * Histogram[X];
                if (SigmaBack == 0 || SigmaFore == 0)
                {
                    if (Threshold == -1)
                        Threshold = Y;
                }
                else
                {
                    SigmaBack = Math.Sqrt(SigmaBack / PixelBack);
                    SigmaFore = Math.Sqrt(SigmaFore / PixelFore);
                    Sigma = 1 + 2 * (PixelBack * Math.Log(SigmaBack / PixelBack) + PixelFore * Math.Log(SigmaFore / PixelFore));
                    if (Sigma < MinSigma)
                    {
                        MinSigma = Sigma;
                        Threshold = Y;
                    }
                }
            }
            return Threshold;
        }

        // Also called intermeans
        // Iterative procedure based on the isodata algorithm [T.W. Ridler, S. Calvard, Picture 
        // thresholding using an iterative selection method, IEEE Trans. System, Man and 
        // Cybernetics, SMC-8 (1978) 630-632.] 
        // The procedure divides the image into objects and background by taking an initial threshold,
        // then the averages of the pixels at or below the threshold and pixels above are computed. 
        // The averages of those two values are computed, the threshold is incremented and the 
        // process is repeated until the threshold is larger than the composite average. That is,
        //  threshold = (average background + average objects)/2
 
        public static int GetIsoDataThreshold(int[] Histogram)
        {
            int i, l, toth, totl, h, g = 0;
            for (i = 1; i < Histogram.Length; i++)
            {
                if (Histogram[i] > 0)
                {
                    g = i + 1;
                    break;
                }
            }
            while (true)
            {
                l = 0;
                totl = 0;
                for (i = 0; i < g; i++)
                {
                    totl = totl + Histogram[i];
                    l = l + (Histogram[i] * i);
                }
                h = 0;
                toth = 0;
                for (i = g + 1; i < Histogram.Length; i++)
                {
                    toth += Histogram[i];
                    h += (Histogram[i] * i);
                }
                if (totl > 0 && toth > 0)
                {
                    l /= totl;
                    h /= toth;
                    if (g == (int)Math.Round((l + h) / 2.0))
                        break;
                }
                g++;
                if (g > Histogram.Length - 2)
                {
                    return 0;
                }
            }
            return g;
        }

        // Shanhbag A.G. (1994) "Utilization of Information Measure as a Means of
        //  Image Thresholding" Graphical Models and Image Processing, 56(5): 414-419
        // Ported to ImageJ plugin by G.Landini from E Celebi's fourier_0.8 routines
        public static int GetShanbhagThreshold(int[] Histogram)
        {
            int threshold;
            int ih, it;
            int first_bin;
            int last_bin;
            double term;
            double tot_ent;  /* total entropy */
            double min_ent;  /* max entropy */
            double ent_back; /* entropy of the background pixels at a given threshold */
            double ent_obj;  /* entropy of the object pixels at a given threshold */
            double[] norm_histo = new double[Histogram.Length]; /* normalized histogram */
            double[] P1 = new double[Histogram.Length]; /* cumulative normalized histogram */
            double[] P2 = new double[Histogram.Length];

            int total = 0;
            for (ih = 0; ih < Histogram.Length; ih++)
                total += Histogram[ih];

            for (ih = 0; ih < Histogram.Length; ih++)
                norm_histo[ih] = (double)Histogram[ih] / total;

            P1[0] = norm_histo[0];
            P2[0] = 1.0 - P1[0];
            for (ih = 1; ih < Histogram.Length; ih++)
            {
                P1[ih] = P1[ih - 1] + norm_histo[ih];
                P2[ih] = 1.0 - P1[ih];
            }

            /* Determine the first non-zero bin */
            first_bin = 0;
            for (ih = 0; ih < Histogram.Length; ih++)
            {
                if (!(Math.Abs(P1[ih]) < 2.220446049250313E-16))
                {
                    first_bin = ih;
                    break;
                }
            }

            /* Determine the last non-zero bin */
            last_bin = Histogram.Length - 1;
            for (ih = Histogram.Length - 1; ih >= first_bin; ih--)
            {
                if (!(Math.Abs(P2[ih]) < 2.220446049250313E-16))
                {
                    last_bin = ih;
                    break;
                }
            }

            // Calculate the total entropy each gray-level
            // and find the threshold that maximizes it 
            threshold = -1;
            min_ent = Double.MaxValue;

            for (it = first_bin; it <= last_bin; it++)
            {
                /* Entropy of the background pixels */
                ent_back = 0.0;
                term = 0.5 / P1[it];
                for (ih = 1; ih <= it; ih++)
                { //0+1?
                    ent_back -= norm_histo[ih] * Math.Log(1.0 - term * P1[ih - 1]);
                }
                ent_back *= term;

                /* Entropy of the object pixels */
                ent_obj = 0.0;
                term = 0.5 / P2[it];
                for (ih = it + 1; ih < Histogram.Length; ih++)
                {
                    ent_obj -= norm_histo[ih] * Math.Log(1.0 - term * P2[ih]);
                }
                ent_obj *= term;

                /* Total entropy */
                tot_ent = Math.Abs(ent_back - ent_obj);

                if (tot_ent < min_ent)
                {
                    min_ent = tot_ent;
                    threshold = it;
                }
            }
            return threshold;
        }

        // M. Emre Celebi
        // 06.15.2007
        // Ported to ImageJ plugin by G.Landini from E Celebi's fourier_0.8 routines
        public static int GetYenThreshold(int[] Histogram)
        {
            int threshold;
            int ih, it;
            double crit;
            double max_crit;
            double[] norm_histo = new double[Histogram.Length]; /* normalized histogram */
            double[] P1 = new double[Histogram.Length]; /* cumulative normalized histogram */
            double[] P1_sq = new double[Histogram.Length];
            double[] P2_sq = new double[Histogram.Length];

            int total = 0;
            for (ih = 0; ih < Histogram.Length; ih++)
                total += Histogram[ih];

            for (ih = 0; ih < Histogram.Length; ih++)
                norm_histo[ih] = (double)Histogram[ih] / total;

            P1[0] = norm_histo[0];
            for (ih = 1; ih < Histogram.Length; ih++)
                P1[ih] = P1[ih - 1] + norm_histo[ih];

            P1_sq[0] = norm_histo[0] * norm_histo[0];
            for (ih = 1; ih < Histogram.Length; ih++)
                P1_sq[ih] = P1_sq[ih - 1] + norm_histo[ih] * norm_histo[ih];

            P2_sq[Histogram.Length - 1] = 0.0;
            for (ih = Histogram.Length - 2; ih >= 0; ih--)
                P2_sq[ih] = P2_sq[ih + 1] + norm_histo[ih + 1] * norm_histo[ih + 1];

            /* Find the threshold that maximizes the criterion */
            threshold = -1;
            max_crit = Double.MinValue;
            for (it = 0; it < Histogram.Length; it++)
            {
                crit = -1.0 * ((P1_sq[it] * P2_sq[it]) > 0.0 ? Math.Log(P1_sq[it] * P2_sq[it]) : 0.0) + 2 * ((P1[it] * (1.0 - P1[it])) > 0.0 ? Math.Log(P1[it] * (1.0 - P1[it])) : 0.0);
                if (crit > max_crit)
                {
                    max_crit = crit;
                    threshold = it;
                }
            }
            return threshold;
        }

        private static double A(int[] Histogram, int Index)
        {
            double Sum = 0;
            for (int Y = 0; Y <= Index; Y++)
                Sum += Histogram[Y];
            return Sum;
        }

        private static double B(int[] Histogram, int Index)
        {
            double Sum = 0;
            for (int Y = 0; Y <= Index; Y++)
                Sum += (double)Y * Histogram[Y];
            return Sum;
        }

        private static double C(int[] Histogram, int Index)
        {
            double Sum = 0;
            for (int Y = 0; Y <= Index; Y++)
                Sum += (double)Y * Y * Histogram[Y];
            return Sum;
        }

        private static double D(int[] Histogram, int Index)
        {
            double Sum = 0;
            for (int Y = 0; Y <= Index; Y++)
                Sum += (double)Y * Y * Y * Histogram[Y];
            return Sum;
        }


        private static bool IsDimodal(double[] Histogram)       // Check whether the histogram is bimodal
        {
            // Count the peaks of the histogram, only peak number 2 is double-peaked
            int Count = 0;
            for (int Y = 1; Y < 255; Y++)
            {
                if (Histogram[Y - 1] < Histogram[Y] && Histogram[Y + 1] < Histogram[Y])
                {
                    Count++;
                    if (Count > 2) return false;
                }
            }
            if (Count == 2)
                return true;
            else
                return false;
        }

    }
}
