using System.Collections.Generic;

namespace MyNrf.这里写仿真程序
{
    class SmartProcess
    {
        #region 系统参数及外部函数定义
        public byte[] P;//存储当前行像素
        public byte[,] Pixels;//存储当前场所有像素的二维指针数组Pixels[0][1]
        public int Image_Height;//图像高度;
        public int Image_Width;//图像宽度;
        public int FieldCount;//记录场数
        public List<LCR> Lcr = new List<LCR>();
        byte black = 0, white = 255;
        byte x, y;

        private Form1 mf;

        byte[][] BinPixels = new byte[70][]
        {
            new byte[186], new byte[186],  new byte[186], new byte[186], new byte[186], new byte[186], new byte[186], new byte[186], new byte[186], new byte[186],
            new byte[186], new byte[186],  new byte[186], new byte[186], new byte[186], new byte[186], new byte[186], new byte[186], new byte[186], new byte[186],
            new byte[186], new byte[186],  new byte[186], new byte[186], new byte[186], new byte[186], new byte[186], new byte[186], new byte[186], new byte[186],
            new byte[186], new byte[186],  new byte[186], new byte[186], new byte[186], new byte[186], new byte[186], new byte[186], new byte[186], new byte[186],
            new byte[186], new byte[186],  new byte[186], new byte[186], new byte[186], new byte[186], new byte[186], new byte[186], new byte[186], new byte[186],
            new byte[186], new byte[186],  new byte[186], new byte[186], new byte[186], new byte[186], new byte[186], new byte[186], new byte[186], new byte[186],
            new byte[186], new byte[186],  new byte[186], new byte[186], new byte[186], new byte[186], new byte[186], new byte[186], new byte[186], new byte[186],
        };

        public void ClearFlag()
        {
            flag_left_round_type = 0;
            flag_right_round_type = 0;
            jishu_type1 = 0;
            jishu_type2 = 0;
            jishu_type3 = 0;
            jishu_type4 = 0;
            jishu_type5 = 0;
            jishu_type6 = 0;
            jishu_type7 = 0;
            flag_stopcar = 0;
            flag_stopcar_people = 0;
        }
        public void LeftBlock()
        {

        }
        public void RightBlock()
        {

        }
        public void BrkRoadAddLine()
        {

        }
        public void BrkRoadNotAddLine()
        {

        }
        public void Anulus0()
        {

        }
        public void Anulus1()
        {

        }
        public void Anulus2()
        {

        }
        public void Anulus3()
        {

        }
        public void StopCross()
        {

        }

        public SmartProcess(Form1 arg)
        {
            mf = arg;
        }

        #endregion

        #region 横向扫点记录
        byte[] L_black = new byte[70];
        byte[] R_black = new byte[70];
        byte[] L_Start = new byte[70];
        byte[] R_Start = new byte[70];
        byte[] LCenter = new byte[70];
        #endregion
        #region 自定义
        #region[函数]
        byte fabss(byte a1, byte a2) //绝对值
        {
            if (a1 > a2)
                return (byte)(a1 - a2);
            else
                return (byte)(a2 - a1);
        }
        int abs(int a)
        {
            if (a < 0)
            {
                return -a;
            }
            else
            {
                return a;
            }
        }
        byte MinNum(byte Value1, byte Value2)
        {
            if (Value1 >= Value2)
                return Value2;
            else
                return Value1;
        }
        double My_double_abs(double x)
        {
            if (x < 0)
                return (double)(-x);
            else
                return (double)x;
        }
        byte AvoidOverflow(int x)//避免溢出
        {
            if (x < 0)
                return 0;
            else if (x > 185)
                return 185;
            else
                return (byte)x;
        }

        byte AvoidOverflow_Row(int x)//避免溢出
        {
            if (x < 0)
                return 0;
            else if (x > 69)
                return 69;
            else
                return (byte)x;
        }

        byte Compare_abs(int a, int b)
        {
            SetText("滑行判断" + "   " + abs(a - b));
            if (abs(a - b) <= 4)
            {
                return 1;
            }
            else
                return 0;
        }

        float float_abs(float x)
        {
            if (x < 0)
                return (-x);
            else
                return x;
        }
        static double double_abs(double x)
        {
            if (x < 0)
                return (double)(-x);
            else
                return (double)x;
        }
        int My_Max(int x, int y)
        {
            if (x > y)
                return x;
            else
                return y;
        }
        int My_Min(int x, int y)
        {
            if (x > y)
                return y;
            else
                return x;
        }
        int MySlopeJudge(int ax, int ay, int bx, int by)
        {
            float k;
            if (bx == ax || by == ay)
                return 0;
            k = (float)(bx - ax) / (by - ay);
            if (k > 0)
                return 1;
            else
                return -1;
        }

        int ByteSubtractByte(byte a, byte b)
        {
            if (a > b)
                return a - b;
            else
                return b - a;
        }

        int JudgeTurn(byte y1, byte y2, byte y3)
        {
            if ((y1 < y2 && y3 > y2) || (y1 > y2 && y3 < y2))
                return 1;
            else
                return 0;
        }
        double My_atan(double z)
        {
            return z - (z * z * z / 3) + (z * z * z * z * z / 5) - (z * z * z * z * z * z * z) / 7;
        }
        byte my_sqrt(byte x)
        {
            byte ans = 0, p = 0x80;
            while (p != 0)
            {
                ans += p;
                if (ans * ans > x)
                {
                    ans -= p;
                }
                p = (byte)(p / 2);
            }
            return (ans);
        }
        float process_curvity(byte x1, byte y1, byte x2, byte y2, byte x3, byte y3)
        {
            float K;
            int S_of_ABC = ((x2 - x1) * (y3 - y1) - (x3 - x1) * (y2 - y1)) / 2;
            //面积的符号表示方向
            byte q1 = (byte)((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1));
            byte AB = my_sqrt(q1);
            q1 = (byte)((x3 - x2) * (x3 - x2) + (y3 - y2) * (y3 - y2));
            byte BC = my_sqrt(q1);
            q1 = (byte)((x3 - x1) * (x3 - x1) + (y3 - y1) * (y3 - y1));
            byte AC = my_sqrt(q1);
            K = (float)4 * S_of_ABC / (AB * BC * AC);
            return K;
        }
        #endregion

        #region[自定义变量]
        static public float[] MyDataRead =new float [50];
        int IMAGE_W = 186;
        int IMAGE_H = 70;
        int CENTER_POINT;
        int IMG_WHITE = 255;
        int IMG_BLACK = 0;
        public byte[] forecast_LCenter = new byte[71];          // 最后一个元素用来记录转向点对应的行数
        public byte[] leftFindFlag = new byte[71];
        public byte[] rightFindFlag = new byte[71];
        public byte[] centerLine = new byte[71];
        public int[] leftLine = new int[71];
        public int[] rightLine = new int[71];
        uint8_t[] fixValue = new uint8_t[65]
        {
            185,185,185,183,178,174,172,168,164,161,
            158,154,150,146,143,140,136,132,128,124,
            120,117,113,109,105,102,99,95,91,88,
            84,80,76,73,69,65,60,56,51,45,
            41,36,32,28,23,19,0,0,0,0,
            0,0,0,0,0,0,0,0,0,0,
            0,0,0,0,0
        };
        int middle_point = 93;
        int findLine_shizi = 0;
        int cumulants1;
        int cumulants2;
        int cumulants3;
        int cumulants4;
        int cumulants5;
        int lostleft_flag;
        int lostright_flag;
        int j;
        uint8_t flag_enter_shizi = 0;
        byte flag_enter_left;
        byte flag_enter_right;
        uint8_t flag_small_S = 0;
        uint8_t flag_middle_S = 0;
        uint8_t flag_changzhidao_jishu = 0;
        uint8_t flag_chuqu = 0;
        uint8_t flag_right_jiasu_panduan;
        uint8_t flag_left_jiasu_panduan;
        uint8_t flag_judge_wenhaowan;
        int flag_shizi_jinru;
        int flag_shizi_right_zhijie;
        int flag_shizi_left_zhijie;
        uint8_t refindLine = 0;
        //环岛处理(左)
        uint8_t left_round_jishu = 0;
        uint8_t left_round_jishu_buchong = 0;
        uint8_t flag_left_round = 0;
        uint8_t flag_left_round_type = 0;
        uint8_t flag_left_round_jishu = 0;
        uint8_t flag_left_round_up = 0;  //上拐点
        uint8_t flag_left_round_in = 0;
        uint8_t flag_left_round_type6 = 0;

        //环岛处理(右)
        uint8_t right_round_jishu = 0;
        uint8_t right_round_jishu_buchong = 0;
        uint8_t flag_right_round = 0;
        uint8_t flag_right_round_type = 0;
        uint8_t flag_right_round_jishu = 0;
        uint8_t flag_right_round_up = 0;  //上拐点
        uint8_t flag_right_round_in = 0;
        uint8_t flag_right_round_type6 = 0;

        uint8_t jishu_type1 = 0;
        uint8_t jishu_type2 = 0;
        uint8_t jishu_type3 = 0;
        uint8_t jishu_type4 = 0;
        uint8_t jishu_type5 = 0;
        uint8_t jishu_type6 = 0;
        uint8_t jishu_type7 = 0;


        byte round_jishu = 0;
        uint8_t Round_jishu=0;
        uint8_t Long_str_jishu = 0;
        uint8_t round_overflow_point = 0;

        uint8_t flag_jinwan_left = 0;
        uint8_t flag_jinwan_right = 0;
        uint8_t flag_jinwan_qita = 0;

        uint8_t jishu_jinwan_left = 0;
        uint8_t jishu_jinwan_right = 0;
        uint8_t jishu_zuizhong_left = 0;
        uint8_t jishu_zuizhong_right = 0;
        int jishu_zuizhong_qita = 0;
        int jishu_jinwan_qita;

        int center_max_min_delta;
        uint8_t left_delta;
        uint8_t right_delta;

        float k_0_25;

        byte flag_ramp = 0;
        byte flag_ramp_jishu = 0;
        byte flag_ramp_out_jishu = 0;
        byte flag_enter_changzhidao;
        byte flag_enter_changzhidao_jishu = 0;
        byte flag_middle_S_jishu = 0;
        byte flag_middle_S_out = 0;
        byte flag_doube_shizi = 0;
        int flag_jiansu_changzhidao_jishu;

        byte flag_enter_shortstr;
        byte flag_enter_shortstr_jishu = 0;
        uint8_t flag_huaxing_short = 0;
        uint8_t flag_huaxing_long = 0;
        int flag_jiansu_shortstr_jishu;
        byte flag_shizi_left = 0;
        byte flag_shizi_right = 0;

        byte parkingFee_jishu_25 = 0;
        byte parkingFee_jishu_29 = 0;
        byte flag_stopcar = 0;
        byte flag_stopcar_people = 0;
        byte startDirect = 1;

        public float curve_a;
        public float curve_b;
        public float parameterA;
        public float parameterB;

        int[] left_turn_down_wandao = new int[2];
        int[] right_turn_down_wandao = new int[2];
        int[] right_turn_up = new int[2];
        int[] right_turn_down = new int[2];
        int[] left_turn_up = new int[2];
        int[] left_turn_down = new int[2];                                //记录两边都丢线的行
        int[] second_right_turn_down = new int[2];
        int[] second_left_turn_down = new int[2];
        int[] centre_right_point = new int[2];
        int[] centre_left_point = new int[2];

        int[] left_round_point = new int[2];        //记录环岛进入时的拐点（左）
        int[] left_round_up = new int[2];
        int[] right_round_point = new int[2];        //记录环岛进入时的拐点（右）
        int[] right_round_up = new int[2];

        byte[][] L_black_visual = new byte[3][];
        byte[][] R_black_visual = new byte[3][];
        #region[用最小二乘法拟合二元多次曲线]
        ///用最小二乘法拟合二元多次曲线
        ///</summary>
        ///<param name="arrX">已知点的x坐标集合</param>
        ///<param name="arrY">已知点的y坐标集合</param>
        ///<param name="length">已知点的个数</param>
        ///<param name="dimension">方程的最高次数</param>
        /**********/
        public static double My_Pow(double a, int b)
        {
            int i = 0;
            double result = a;
            for (i = 1; i <= b; i++)
            {
                result *= a;
            }
            return result;
        }
        public static double[] MultiLine(double[] arrX, double[] arrY, int length, int dimension)//二元多次线性方程拟合曲线
        {
            int n = dimension + 1;                 //dimension次方程需要求 dimension+1个 系数
            double[,] Guass = new double[n, n + 1];     //高斯矩阵 例如：y=a0+a1*x+a2*x*x
            for (int i = 0; i < n; i++)
            {
                int j;
                for (j = 0; j < n; j++)
                {
                    Guass[i, j] = SumArr(arrX, j + i, length);
                }
                Guass[i, j] = SumArr(arrX, i, arrY, 1, length);
            }
            return ComputGauss(Guass, n);
        }
        public static double SumArr(double[] arr, int n, int length) //求数组的元素的n次方的和
        {
            double s = 0;
            for (int i = 0; i < length; i++)
            {
                if (arr[i] != 0 || n != 0)
                    s = s + My_Pow(arr[i], n);
                else
                    s = s + 1;
            }
            return s;
        }
        public static double SumArr(double[] arr1, int n1, double[] arr2, int n2, int length)
        {
            double s = 0;
            for (int i = 0; i < length; i++)
            {
                if ((arr1[i] != 0 || n1 != 0) && (arr2[i] != 0 || n2 != 0))
                    s = s + My_Pow(arr1[i], n1) * My_Pow(arr2[i], n2);
                else
                    s = s + 1;
            }
            return s;

        }
        public static double[] ComputGauss(double[,] Guass, int n)
        {
            int i, j;
            int k, m;
            double temp;
            double max;
            double s;
            double[] x = new double[n]; for (i = 0; i < n; i++) x[i] = 0.0;//初始化
            for (j = 0; j < n; j++)
            {
                max = 0; k = j;
                for (i = j; i < n; i++)
                {
                    if (double_abs(Guass[i, j]) > max)
                    {
                        max = Guass[i, j];
                        k = i;
                    }
                }

                if (k != j)
                {
                    for (m = j; m < n + 1; m++)
                    {
                        temp = Guass[j, m];
                        Guass[j, m] = Guass[k, m];
                        Guass[k, m] = temp;
                    }
                }
                if (0 == max)
                {
                    // "此线性方程为奇异线性方程"                return x;
                }
                for (i = j + 1; i < n; i++)
                {
                    s = Guass[i, j];
                    for (m = j; m < n + 1; m++)
                    {
                        Guass[i, m] = Guass[i, m] - Guass[j, m] * s / (Guass[j, j]);
                    }
                }
            }//结束for (j=0;j<n;j++)       
            for (i = n - 1; i >= 0; i--)
            {
                s = 0;
                for (j = i + 1; j < n; j++)
                {
                    s = s + Guass[i, j] * x[j];
                }
                x[i] = (Guass[i, n] - s) / Guass[i, i];
            }
            return x;
        }//返回值是函数的系数例如：y=a0+a1*x 返回值则为a0 a1例如：y=a0+a1*x+a2*x*x 返回值则为a0 a1 a2 剩下的就不用写了吧
        #endregion


        //最小二乘法拟合形如y=a^2+b的二次曲线
        void LeastSquareCalc_Curve(uint8_t StartLine, uint8_t EndLine, uint8_t type)
        {
            uint8_t i = 0;
            float Sum_X2 = 0, Sum_Y = 0, Sum_X4 = 0, Sum_YX2 = 0, Average_X2 = 0, Average_Y = 0, Average_X4 = 0, Average_YX2 = 0, Sum = 0;

            Sum = EndLine - StartLine;
            if (type == 1)
            {
                for (i = StartLine; i < EndLine; i++)
                {
                    Sum_X2 += i * i;
                    Sum_Y += L_black[i];
                    Sum_X4 += i * i * i * i;
                    Sum_YX2 += L_black[i] * i * i;
                }
            }
            else if (type == 2)
            {
                for (i = StartLine; i < EndLine; i++)
                {
                    Sum_X2 += i * i;
                    Sum_Y += R_black[i];
                    Sum_X4 += i * i * i * i;
                    Sum_YX2 += R_black[i] * i * i;
                }
            }
            Average_X2 = Sum_X2 / Sum;
            Average_Y = Sum_Y / Sum;
            Average_X4 = Sum_X4 / Sum;
            Average_YX2 = Sum_YX2 / Sum;
            curve_a = (Average_YX2 - Average_Y * Average_X2) / (Average_X4 - Average_X2 * Average_X2);
            curve_b = Average_Y - curve_a * Average_X2;
        }

        void regression(int type, int startline, int endline)
        {
            int i = 0;
            int sumlines = endline - startline;
            int sumX = 0;
            int sumY = 0;
            float averageX = 0;
            float averageY = 0;
            float sumUp = 0;
            float sumDown = 0;
            if (type == 0)      //拟合中线
            {
                for (i = startline; i < endline; i++)
                {
                    sumX += i;
                    sumY += LCenter[i];
                }
                averageX = sumX / sumlines;     //x的平均值
                averageY = sumY / sumlines;     //y的平均值
                for (i = startline; i < endline; i++)
                {
                    sumUp += (LCenter[i] - averageY) * (i - averageX);
                    sumDown += (i - averageX) * (i - averageX);
                }
                if (sumDown == 0) parameterB = 0;
                else parameterB = sumUp / sumDown;
                parameterA = averageY - parameterB * averageX;
            }
            else if (type == 1)//拟合左线
            {
                for (i = startline; i < endline; i++)
                {
                    sumX += i;
                    sumY += L_black[i];
                }
                averageX = sumX / sumlines;     //x的平均值
                averageY = sumY / sumlines;     //y的平均值
                for (i = startline; i < endline; i++)
                {
                    sumUp += (L_black[i] - averageY) * (i - averageX);
                    sumDown += (i - averageX) * (i - averageX);
                }
                if (sumDown == 0) parameterB = 0;
                else parameterB = sumUp / sumDown;
                parameterA = averageY - parameterB * averageX;
            }
            else if (type == 2)//拟合右线
            {
                for (i = startline; i < endline; i++)
                {
                    sumX += i;
                    sumY += R_black[i];
                }
                averageX = sumX / sumlines;     //x的平均值
                averageY = sumY / sumlines;     //y的平均值
                for (i = startline; i < endline; i++)
                {
                    sumUp += (R_black[i] - averageY) * (i - averageX);
                    sumDown += (i - averageX) * (i - averageX);
                }
                if (sumDown == 0) parameterB = 0;
                else parameterB = sumUp / sumDown;
                parameterA = averageY - parameterB * averageX;
            }
        }

        byte juge_if_same_fuhao(float a, float b)
        {
            if ((a < 0 && b < 0) || (a > 0 && b > 0))            return 0;
            else if (float_abs(a) > 25 || float_abs(b) > 25)     return 0;
            else return 1;
        }              //从上往下拉线
        void advanced_regression(int type, int startline1, int endline1, int startline2, int endline2)
        {
            int i = 0;
            int sumlines1 = endline1 - startline1;
            int sumlines2 = endline2 - startline2;
            int sumX = 0;
            int sumY = 0;
            float averageX = 0;
            float averageY = 0;
            float sumUp = 0;
            float sumDown = 0;
            if (type == 0)  //拟合中线
            {
                /**计算sumX sumY**/
                for (i = startline1; i < endline1; i++)
                {
                    sumX += i;
                    sumY += LCenter[i];
                }
                for (i = startline2; i < endline2; i++)
                {
                    sumX += i;
                    sumY += LCenter[i];
                }
                averageX = sumX / (sumlines1 + sumlines2);     //x的平均值
                averageY = sumY / (sumlines1 + sumlines2);     //y的平均值
                for (i = startline1; i < endline1; i++)
                {
                    sumUp += (LCenter[i] - averageY) * (i - averageX);
                    sumDown += (i - averageX) * (i - averageX);
                }
                for (i = startline2; i < endline2; i++)
                {
                    sumUp += (LCenter[i] - averageY) * (i - averageX);
                    sumDown += (i - averageX) * (i - averageX);
                }
                if (sumDown == 0) parameterB = 0;
                else parameterB = sumUp / sumDown;
                parameterA = averageY - parameterB * averageX;

            }
            else if (type == 1)     //拟合左线
            {
                /**计算sumX sumY**/
                for (i = startline1; i < endline1; i++)
                {
                    sumX += i;
                    sumY += L_black[i];
                }
                for (i = startline2; i < endline2; i++)
                {
                    sumX += i;
                    sumY += L_black[i];
                }
                averageX = sumX / (sumlines1 + sumlines2);     //x的平均值
                averageY = sumY / (sumlines1 + sumlines2);     //y的平均值
                for (i = startline1; i < endline1; i++)
                {
                    sumUp += (L_black[i] - averageY) * (i - averageX);
                    sumDown += (i - averageX) * (i - averageX);
                }
                for (i = startline2; i < endline2; i++)
                {
                    sumUp += (L_black[i] - averageY) * (i - averageX);
                    sumDown += (i - averageX) * (i - averageX);
                }
                if (sumDown == 0) parameterB = 0;
                else parameterB = sumUp / sumDown;
                parameterA = averageY - parameterB * averageX;
            }
            else if (type == 2)         //拟合右线
            {
                /**计算sumX sumY**/
                for (i = startline1; i < endline1; i++)
                {
                    sumX += i;
                    sumY += R_black[i];
                }
                for (i = startline2; i < endline2; i++)
                {
                    sumX += i;
                    sumY += R_black[i];
                }
                averageX = sumX / (sumlines1 + sumlines2);     //x的平均值
                averageY = sumY / (sumlines1 + sumlines2);     //y的平均值
                for (i = startline1; i < endline1; i++)
                {
                    sumUp += (R_black[i] - averageY) * (i - averageX);
                    sumDown += (i - averageX) * (i - averageX);
                }
                for (i = startline2; i < endline2; i++)
                {
                    sumUp += (R_black[i] - averageY) * (i - averageX);
                    sumDown += (i - averageX) * (i - averageX);
                }
                if (sumDown == 0) parameterB = 0;
                else parameterB = sumUp / sumDown;
                parameterA = averageY - parameterB * averageX;
            }

        }         //上下拉线
        uint8_t findLine;
        

        #endregion
        void ImageProcess()//图像行处理
        {
            #region[复制]
            int temp = 0;
            flag_chuqu = 0;
            flag_changzhidao_jishu = 0;
            flag_enter_shizi = 0;
            flag_left_round_up = 0;
            flag_right_round_up = 0;
            flag_left_round_in = 0;
            flag_right_round_in = 0;
            left_round_jishu = 0;
            left_round_jishu_buchong = 0;
            right_round_jishu_buchong = 0;
            right_round_jishu = 0;

            parkingFee_jishu_25 = 0;  //停车线计数
            parkingFee_jishu_29 = 0;  //停车线计数

            double k_left_double = 0;
            double b_left_double = 0;
            double k_right_double = 0;
            double b_right_double = 0;
            int left_point_double = 0;
            int right_point_double = 0;
            byte left_pointDouble_flag = 0;
            byte right_pointDouble_flag = 0;

            // 前十行从中间往两边查找
            for (int i = 0; i <= 185; i++)
            {
                if (Pixels[3, i] == IMG_BLACK)
                {
                    flag_chuqu += 1;
                }
            }
            ////(flag_chuqu);
            //[第一次扫线]
            for (findLine = 0; findLine < 55; findLine++)
            {
                if (flag_shizi_jinru != 1)
                    LCenter[findLine] = 93;
                R_black[findLine] = 0;
                L_black[findLine] = 185;
                if (findLine == 0)
                    CENTER_POINT = middle_point;
                else
                    CENTER_POINT = LCenter[findLine - 1];
                if (CENTER_POINT < 3)
                {
                    CENTER_POINT = 3;
                }
                else if (CENTER_POINT > 183)
                {
                    CENTER_POINT = 183;
                }
                leftFindFlag[findLine] = 0;
                rightFindFlag[findLine] = 0;

                if (flag_left_round_type == 3)    //左环岛三状态重扫
                {

                    for (temp = (byte)170; temp > 1; temp--)
                    {
                        if (Pixels[findLine, temp - 1] == IMG_BLACK && Pixels[findLine, temp] == IMG_WHITE)
                        {
                            rightFindFlag[findLine] = 1;
                            R_black[findLine] = (byte)(temp - 1);
                            break;
                        }
                    }

                    if (flag_left_round_up == 0 && findLine > 1 && abs(R_black[findLine] - R_black[findLine - 1]) > 25 && R_black[findLine] != 0 && R_black[findLine] != 185)
                    {
                        left_round_up[0] = findLine + 1;//数组里面没有第0行
                        left_round_up[1] = R_black[findLine];
                        flag_left_round_up = 1;
                        SetText("leftround_uppoint" + left_round_up[0] + "  " + left_round_up[1]);
                        //if (left_round_up[0]<30&& findLine< left_round_up[0]+6)
                        //{
                        //    for (temp = 0; temp < (CENTER_POINT - 2); temp++)
                        //    {
                        //        // 寻找右黑线 
                        //        if (rightFindFlag[findLine] == 0
                        //        && Pixels[findLine, (CENTER_POINT - temp - 1)] == IMG_BLACK       //小的
                        //        && Pixels[findLine, (CENTER_POINT - temp)] == IMG_WHITE
                        //        && Pixels[findLine, (CENTER_POINT - temp + 1)] == IMG_WHITE
                        //        && Pixels[findLine, (CENTER_POINT - temp + 2)] == IMG_WHITE)
                        //        {
                        //            R_black[findLine] = (byte)(CENTER_POINT - temp - 1);
                        //            //if (findLine != 0 && abs(R_black[findLine] - R_black[findLine - 1]) > 40&& flag_shizi_max!=1)
                        //            //{
                        //            //    R_black[findLine] = 0;
                        //            //    break;
                        //            //}
                        //            rightFindFlag[findLine] = 1;
                        //            break;
                        //        }
                        //    }
                        //}
                        //else
                        break;
                    }
                    for (temp = 0; temp < (183 - CENTER_POINT); temp++)
                    {
                        // 寻找左黑线
                        if (leftFindFlag[findLine] == 0
                        && Pixels[findLine, (CENTER_POINT + temp)] == IMG_BLACK         //大的
                        && Pixels[findLine, (CENTER_POINT + temp - 1)] == IMG_WHITE
                        && Pixels[findLine, (CENTER_POINT + temp - 2)] == IMG_WHITE
                        && Pixels[findLine, (CENTER_POINT + temp - 3)] == IMG_WHITE)
                        {
                            L_black[findLine] = (byte)(CENTER_POINT + temp);
                            //if (findLine != 0 && abs(L_black[findLine] - L_black[findLine - 1]) > 40 && flag_shizi_max != 1)
                            //{
                            //    L_black[findLine] = 185;
                            //    break;
                            //}
                            leftFindFlag[findLine] = 1;
                            break;
                        }

                    }

                }
                else if (flag_left_round_type == 2)     //左环岛二状态重扫
                {
                    for (temp = 0; temp < (183 - 88); temp++)
                    {
                        // 寻找左黑线
                        if (leftFindFlag[findLine] == 0
                        && Pixels[findLine, (88 + temp)] == IMG_BLACK         //大的
                        && Pixels[findLine, (88 + temp - 1)] == IMG_WHITE
                        && Pixels[findLine, (88 + temp - 2)] == IMG_WHITE
                        && Pixels[findLine, (88 + temp - 3)] == IMG_WHITE)
                        {
                            L_black[findLine] = (byte)(88 + temp);
                            //if (findLine != 0 && abs(L_black[findLine] - L_black[findLine - 1]) > 40 && flag_shizi_max != 1)
                            //{
                            //    L_black[findLine] = 185;
                            //    break;
                            //}
                            leftFindFlag[findLine] = 1;
                            break;
                        }

                    }
                    for (temp = 0; temp < (88 - 2); temp++)
                    {
                        // 寻找右黑线 
                        if (rightFindFlag[findLine] == 0
                        && Pixels[findLine, (88 - temp - 1)] == IMG_BLACK       //小的
                        && Pixels[findLine, (88 - temp)] == IMG_WHITE
                        && Pixels[findLine, (88 - temp + 1)] == IMG_WHITE
                        && Pixels[findLine, (88 - temp + 2)] == IMG_WHITE)
                        {
                            R_black[findLine] = (byte)(88 - temp - 1);
                            //if (findLine != 0 && abs(R_black[findLine] - R_black[findLine - 1]) > 40&& flag_shizi_max!=1)
                            //{
                            //    R_black[findLine] = 0;
                            //    break;
                            //}
                            rightFindFlag[findLine] = 1;
                            break;
                        }
                    }

                }
                else if (flag_left_round_type == 6)     //左环岛六状态重扫
                {
                    for (temp = (byte)170; temp > 1; temp--)
                    {
                        if (Pixels[findLine, temp - 1] == IMG_BLACK && Pixels[findLine, temp] == IMG_WHITE)
                        {
                            rightFindFlag[findLine] = 1;
                            R_black[findLine] = (byte)(temp - 1);
                            if (flag_left_round_type == 6 && flag_left_round_type6 == 1 && findLine == 0)
                            {
                                right_turn_down[0] = R_black[findLine];
                                right_turn_down[1] = 0;
                            }
                            break;
                        }
                    }
                    //if (flag_left_round_type == 6 && flag_round_type6 == 1 && findLine == 0)
                    //{
                    //    for (temp = (byte)170; temp > 1; temp--)
                    //    {
                    //        if(Pixels[findLine, temp - 1]== IMG_BLACK && Pixels[findLine, temp] == IMG_WHITE)
                    //    }
                    //}
                    for (temp = 0; temp < (183 - CENTER_POINT); temp++)
                    {
                        // 寻找左黑线
                        if (leftFindFlag[findLine] == 0
                        && Pixels[findLine, (CENTER_POINT + temp)] == IMG_BLACK         //大的
                        && Pixels[findLine, (CENTER_POINT + temp - 1)] == IMG_WHITE
                        && Pixels[findLine, (CENTER_POINT + temp - 2)] == IMG_WHITE
                        && Pixels[findLine, (CENTER_POINT + temp - 3)] == IMG_WHITE)
                        {
                            L_black[findLine] = (byte)(CENTER_POINT + temp);
                            //if (findLine != 0 && abs(L_black[findLine] - L_black[findLine - 1]) > 40 && flag_shizi_max != 1)
                            //{
                            //    L_black[findLine] = 185;
                            //    break;
                            //}
                            leftFindFlag[findLine] = 1;
                            break;
                        }

                    }

                }
                else if (flag_right_round_type == 3)    //右环岛三状态重扫
                {
                    for (temp = 10; temp < 184; temp++)
                    {

                        if (leftFindFlag[findLine] == 0
                            && Pixels[findLine, temp - 1] == IMG_WHITE && Pixels[findLine, temp] == IMG_BLACK)
                        {
                            leftFindFlag[findLine] = 1;
                            L_black[findLine] = (byte)(temp);
                            break;
                        }
                    }

                    if (flag_right_round_up == 0 && findLine > 1 && abs(L_black[findLine - 1] - L_black[findLine]) > 25
                        && L_black[findLine] != 0 && L_black[findLine] != 185)
                    {
                        right_round_up[0] = findLine + 1;//数组里面没有第0行
                        right_round_up[1] = L_black[findLine];
                        flag_right_round_up = 1;
                        SetText("rightround_uppoint" + right_round_up[0] + "  " + right_round_up[1]);
                        break;
                    }
                    for (temp = 0; temp < (93 - 2); temp++)
                    {
                        // 寻找右黑线 
                        if (rightFindFlag[findLine] == 0
                        && Pixels[findLine, (93 - temp - 1)] == IMG_BLACK       //小的
                        && Pixels[findLine, (93 - temp)] == IMG_WHITE
                        && Pixels[findLine, (93 - temp + 1)] == IMG_WHITE
                        && Pixels[findLine, (93 - temp + 2)] == IMG_WHITE)
                        {
                            R_black[findLine] = (byte)(93 - temp - 1);
                            rightFindFlag[findLine] = 1;
                            break;
                        }
                    }

                }
                else if (flag_right_round_type == 2)     //右环岛二状态重扫
                {
                    for (temp = 0; temp < (183 - 98); temp++)
                    {
                        // 寻找左黑线
                        if (leftFindFlag[findLine] == 0
                        && Pixels[findLine, (98 + temp)] == IMG_BLACK         //大的
                        && Pixels[findLine, (98 + temp - 1)] == IMG_WHITE
                        && Pixels[findLine, (98 + temp - 2)] == IMG_WHITE
                        && Pixels[findLine, (98 + temp - 3)] == IMG_WHITE)
                        {
                            L_black[findLine] = (byte)(98 + temp);
                            //if (findLine != 0 && abs(L_black[findLine] - L_black[findLine - 1]) > 40 && flag_shizi_max != 1)
                            //{
                            //    L_black[findLine] = 185;
                            //    break;
                            //}
                            leftFindFlag[findLine] = 1;
                            break;
                        }

                    }
                    for (temp = 0; temp < (110 - 2); temp++)
                    {
                        // 寻找右黑线 
                        if (rightFindFlag[findLine] == 0
                        && Pixels[findLine, (110 - temp - 1)] == IMG_BLACK       //小的
                        && Pixels[findLine, (110 - temp)] == IMG_WHITE
                        && Pixels[findLine, (110 - temp + 1)] == IMG_WHITE
                        && Pixels[findLine, (110 - temp + 2)] == IMG_WHITE)
                        {
                            R_black[findLine] = (byte)(110 - temp - 1);
                            //if (findLine != 0 && abs(R_black[findLine] - R_black[findLine - 1]) > 40&& flag_shizi_max!=1)
                            //{
                            //    R_black[findLine] = 0;
                            //    break;
                            //}
                            rightFindFlag[findLine] = 1;
                            break;
                        }
                    }

                }
                else if (flag_right_round_type == 6)    //右环岛六状态重扫
                {
                    for (temp = 1; temp < 100; temp++)
                    {

                        if (leftFindFlag[findLine] == 0
                            && Pixels[findLine, temp - 1] == IMG_WHITE && Pixels[findLine, temp] == IMG_BLACK)
                        {
                            leftFindFlag[findLine] = 1;
                            L_black[findLine] = (byte)(temp);
                            if (flag_right_round_type == 6 && flag_right_round_type6 == 1 && findLine == 0)
                            {
                                left_turn_down[0] = L_black[findLine];
                                left_turn_down[1] = 0;
                            }
                            break;
                        }
                    }
                    for (temp = 0; temp < (CENTER_POINT - 2); temp++)
                    {
                        // 寻找右黑线 
                        if (rightFindFlag[findLine] == 0
                        && Pixels[findLine, (CENTER_POINT - temp - 1)] == IMG_BLACK       //小的
                        && Pixels[findLine, (CENTER_POINT - temp)] == IMG_WHITE
                        && Pixels[findLine, (CENTER_POINT - temp + 1)] == IMG_WHITE
                        && Pixels[findLine, (CENTER_POINT - temp + 2)] == IMG_WHITE)
                        {
                            R_black[findLine] = (byte)(CENTER_POINT - temp - 1);
                            //if (findLine != 0 && abs(R_black[findLine] - R_black[findLine - 1]) > 40&& flag_shizi_max!=1)
                            //{
                            //    R_black[findLine] = 0;
                            //    break;
                            //}
                            rightFindFlag[findLine] = 1;
                            break;
                        }
                    }
                }
                else if (flag_shizi_jinru == 1 && findLine < refindLine)
                {
                    for (temp = 0; temp < (183 - centerLine[findLine]); temp++)
                    {
                        // 寻找左黑线
                        if (leftFindFlag[findLine] == 0
                        && Pixels[findLine, (centerLine[findLine] + temp)] == IMG_BLACK         //大的
                        && Pixels[findLine, (centerLine[findLine] + temp - 1)] == IMG_WHITE
                        && Pixels[findLine, (centerLine[findLine] + temp - 2)] == IMG_WHITE
                        && Pixels[findLine, (centerLine[findLine] + temp - 3)] == IMG_WHITE)
                        {
                            L_black[findLine] = (byte)(centerLine[findLine] + temp);
                            //if (findLine != 0 && abs(L_black[findLine] - L_black[findLine - 1]) > 40 && flag_shizi_max != 1)
                            //{
                            //    L_black[findLine] = 185;
                            //    break;
                            //}
                            leftFindFlag[findLine] = 1;
                            break;
                        }

                    }
                    for (temp = 0; temp < (centerLine[findLine] - 2); temp++)
                    {
                        // 寻找右黑线 
                        if (rightFindFlag[findLine] == 0
                        && Pixels[findLine, (centerLine[findLine] - temp - 1)] == IMG_BLACK       //小的
                        && Pixels[findLine, (centerLine[findLine] - temp)] == IMG_WHITE
                        && Pixels[findLine, (centerLine[findLine] - temp + 1)] == IMG_WHITE
                        && Pixels[findLine, (centerLine[findLine] - temp + 2)] == IMG_WHITE)
                        {
                            R_black[findLine] = (byte)(centerLine[findLine] - temp - 1);
                            //if (findLine != 0 && abs(R_black[findLine] - R_black[findLine - 1]) > 40&& flag_shizi_max!=1)
                            //{
                            //    R_black[findLine] = 0;
                            //    break;
                            //}
                            rightFindFlag[findLine] = 1;
                            break;
                        }
                    }
                }
                else
                {

                    for (temp = 0; temp < (183 - CENTER_POINT); temp++)
                    {
                        // 寻找左黑线
                        if (leftFindFlag[findLine] == 0
                        && Pixels[findLine, (CENTER_POINT + temp)] == IMG_BLACK         //大的
                        && Pixels[findLine, (CENTER_POINT + temp - 1)] == IMG_WHITE
                        && Pixels[findLine, (CENTER_POINT + temp - 2)] == IMG_WHITE
                        && Pixels[findLine, (CENTER_POINT + temp - 3)] == IMG_WHITE)
                        {
                            L_black[findLine] = (byte)(CENTER_POINT + temp);
                            //if (findLine != 0 && abs(L_black[findLine] - L_black[findLine - 1]) > 40 && flag_shizi_max != 1)
                            //{
                            //    L_black[findLine] = 185;
                            //    break;
                            //}
                            leftFindFlag[findLine] = 1;
                            break;
                        }

                    }
                    for (temp = 0; temp < (CENTER_POINT - 2); temp++)
                    {
                        // 寻找右黑线 
                        if (rightFindFlag[findLine] == 0
                        && Pixels[findLine, (CENTER_POINT - temp - 1)] == IMG_BLACK       //小的
                        && Pixels[findLine, (CENTER_POINT - temp)] == IMG_WHITE
                        && Pixels[findLine, (CENTER_POINT - temp + 1)] == IMG_WHITE
                        && Pixels[findLine, (CENTER_POINT - temp + 2)] == IMG_WHITE)
                        {
                            R_black[findLine] = (byte)(CENTER_POINT - temp - 1);
                            //if (findLine != 0 && abs(R_black[findLine] - R_black[findLine - 1]) > 40&& flag_shizi_max!=1)
                            //{
                            //    R_black[findLine] = 0;
                            //    break;
                            //}
                            rightFindFlag[findLine] = 1;
                            break;
                        }
                    }


                }


                lostleft_flag = 0;
                lostright_flag = 0;
                LCenter[findLine] = (byte)((L_black[findLine] + R_black[findLine]) / 2);
                if (leftFindFlag[findLine] == 0)
                {
                    L_black[findLine] = 185;
                    lostleft_flag++;
                }
                if (rightFindFlag[findLine] == 0)
                {
                    R_black[findLine] = 0;
                    lostright_flag++;
                }
                if (findLine > 5)
                {
                    //if (flag_left_round_type == 3)
                    //{
                    //    if (R_black[findLine] > 10 && R_black[findLine] < 60)
                    //        break;
                    //}
                    ////else if (flag_right_round_type == 3)
                    ////{

                    ////}
                    if (flag_stopcar == 1)
                    {
                        
                    }
                    else if ((Pixels[findLine, LCenter[findLine]]) == IMG_BLACK || abs(L_black[findLine] - L_black[findLine - 1]) > 130 || abs(R_black[findLine] - R_black[findLine - 1]) > 130)
                        break;
                }

                /**************弯道补线***********/
                if (rightFindFlag[findLine] == 0 && leftFindFlag[findLine] == 1)
                {

                    R_black[findLine] = 0;
                }
                if (leftFindFlag[findLine] == 0 && rightFindFlag[findLine] == 1)
                {

                    L_black[findLine] = 185;
                }

                /********  最最简单的十字判定**********/

                if (leftFindFlag[findLine] == 0 && rightFindFlag[findLine] == 0 && L_black[1] > 93 && R_black[1] < 93 && findLine < 35 && L_black[13] > 93 && R_black[13] < 93 /*&& LCenter[22] < 130 && LCenter[22] > 60*/)
                {
                    L_black[findLine] = 185;
                    R_black[findLine] = 0;
                    if (findLine > 0 && findLine < 15)
                        flag_enter_shizi += 1;
                }
                else if (leftFindFlag[findLine] == 0 && rightFindFlag[findLine] == 0)
                {
                    L_black[findLine] = 185;
                    R_black[findLine] = 0;
                }


            }
            
            //if ((rightFindFlag[0] == 0 && rightFindFlag[1] == 0 && rightFindFlag[2] == 0 && rightFindFlag[3] == 0
            //    && leftFindFlag[15] == 0 && leftFindFlag[16] == 0 && leftFindFlag[17] == 0 && leftFindFlag[18] == 0)
            //    || (rightFindFlag[15] == 0 && rightFindFlag[16] == 0 && rightFindFlag[17] == 0 && rightFindFlag[18] == 0
            //    && leftFindFlag[0] == 0 && leftFindFlag[1] == 0 && leftFindFlag[2] == 0 && leftFindFlag[3] == 0))
            //{
            //    flag_shizi_jinru = 1;
            //}
            //else
            //    flag_shizi_jinru = 0;
            //SetText("斜入十字"+flag_shizi_jinru);

            //
            SetText("1              "+ flag_enter_shizi);
            /***********  判断弯道类型*******************/
            if (findLine > 29)
            {
                if ((LCenter[25] - LCenter[4]) > 8 && findLine < 50)
                {
                    flag_enter_left = 1;
                }
                else
                {
                    flag_enter_left = 0;
                }


                if ((LCenter[25] - LCenter[4]) < -8 && findLine < 50)
                {
                    flag_enter_right = 1;
                }
                else
                {

                    flag_enter_right = 0;
                }

            }
            else
            {
                if ((LCenter[findLine - 4] - LCenter[4]) > 5)
                {
                    flag_enter_left = 1;
                }
                else if ((LCenter[findLine - 4] - LCenter[4]) < -5)
                {
                    flag_enter_right = 1;
                }
                else
                {
                    flag_enter_left = 0;
                    flag_enter_right = 0;
                }




            }
            for (int i = findLine; i < 65; i++)
            {
                if (flag_enter_left == 1 || flag_left_round == 1)
                {
                    R_black[i] = 185;
                    L_black[i] = 185;
                    LCenter[i] = 185;
                }
                else if (flag_enter_right == 1 || flag_right_round == 1)
                {
                    R_black[i] = 0;
                    L_black[i] = 0;
                    LCenter[i] = 0;
                }
                else
                {
                    R_black[i] = 0;
                    L_black[i] = 0;
                    LCenter[i] = 0;
                }
            }
            regression(0, 0, 25);
            k_0_25 = parameterB;
            byte flag_left_down_point = 0;       //判断是否找到拐点
            byte flag_right_down_point = 0;
            byte flag_left_up_point = 0;       //判断是否找到拐点
            byte flag_right_up_point = 0;
            #region[十字]
            if (flag_left_round == 0 && flag_right_round == 0 && flag_stopcar == 0)
            {
                byte flag_left_down_point_wandao = 0;       //判断是否找到拐点
                byte flag_right_down_point_wandao = 0;
                byte flag_second_right_down_point = 0;     //判断是否找到第二个下拐点
                byte flag_second_left_down_point = 0;
                byte flag_centre_right_point = 0;       //判断是否找到中线拐点
                byte flag_centre_left_point = 0;
                if (flag_ramp == 0)
                {
                    for (j = 2; j <= 49; j++)
                    {

                        if (flag_changzhidao_jishu >= 45) break;
                        if (j > 2 && j <= 36)
                        {

                            if (flag_left_down_point == 0 && abs(L_black[j - 1] - L_black[j - 2]) <= 3
                                && abs(L_black[j] - L_black[j - 1]) <= 3 && (L_black[j + 1] - L_black[j] >= 1)
                                && (L_black[j + 3] - L_black[j] >= 3)
                                && leftFindFlag[j - 2] == 1 && leftFindFlag[j - 1] == 1
                                && leftFindFlag[j] == 1)
                            {   //左下

                                left_turn_down[0] = j + 1;//数组里面没有第0行
                                left_turn_down[1] = L_black[j];
                                flag_left_down_point = 1;
                            }
                            if (flag_right_down_point == 0
                                && abs(R_black[j - 1] - R_black[j - 2]) <= 3
                                && abs(R_black[j] - R_black[j - 1]) <= 3
                                && (R_black[j + 1] - R_black[j] <= -1) && (R_black[j + 3] - R_black[j] <= -3)
                                && rightFindFlag[j - 2] == 1 && rightFindFlag[j - 1] == 1 && rightFindFlag[j] == 1)
                            {   //右下
                                right_turn_down[0] = j + 1;
                                right_turn_down[1] = R_black[j];
                                flag_right_down_point = 1;
                            }
                        }
                        //入十字用的上拐点
                        if (j < 43)
                        {
                            //左上拐点
                            if (flag_left_up_point == 0 && (L_black[j] - L_black[j - 2]) <= -7
                                && (L_black[j] - L_black[j - 1]) <= -3 && abs(L_black[j + 2] - L_black[j + 1]) <= 4 && abs(L_black[j + 1] - L_black[j]) <= 5
                                && leftFindFlag[j + 2] == 1 && leftFindFlag[j] == 1 && leftFindFlag[j + 1] == 1)
                            {

                                left_turn_up[0] = j + 1;//数组里面没有第0行
                                left_turn_up[1] = L_black[j];
                                if ((L_black[j] - L_black[j + 1]) >= 4)
                                {
                                    left_turn_up[0] = j + 2;//数组里面没有第0行
                                    left_turn_up[1] = L_black[j + 1];
                                }
                                flag_left_up_point = 1;
                            }
                            if (flag_enter_shizi > 2 && flag_second_left_down_point == 0 && flag_left_up_point == 1 && (abs(L_black[j - 1] - L_black[j - 2]) <= 5 || abs(L_black[j] - L_black[j - 1]) <= 5)
                                && ((L_black[j + 1] - L_black[j] >= 10) || (L_black[j + 2] - L_black[j] >= 15))
                                && leftFindFlag[j - 2] == 1 && leftFindFlag[j - 1] == 1 && leftFindFlag[j] == 1)
                            //下一个下拐点
                            {
                                second_left_turn_down[0] = j + 1;//数组里面没有第0行
                                second_left_turn_down[1] = L_black[j];
                                flag_second_left_down_point = 1;
                            }

                            //右上拐点
                            if (flag_right_up_point == 0 && (R_black[j] - R_black[j - 2]) >= 7
                                && (R_black[j] - R_black[j - 1]) >= 3 && abs(R_black[j + 2] - R_black[j + 1]) <= 4
                                && abs(R_black[j + 1] - R_black[j]) <= 5
                                && rightFindFlag[j + 2] == 1 && rightFindFlag[j] == 1 && rightFindFlag[j + 1] == 1)
                            {
                                right_turn_up[0] = j + 1;
                                right_turn_up[1] = R_black[j];
                                if ((R_black[j] - R_black[j + 1]) <= -4)
                                {
                                    right_turn_up[0] = j + 2;
                                    right_turn_up[1] = R_black[j + 1];
                                }
                                flag_right_up_point = 1;
                            }
                            if (flag_enter_shizi > 2 && flag_second_right_down_point == 0 && flag_right_up_point == 1 && (abs(R_black[j - 1] - R_black[j - 2]) <= 5 || abs(R_black[j] - R_black[j - 1]) <= 5)
                            && ((R_black[j + 1] - R_black[j] <= -10) || (R_black[j + 2] - R_black[j] <= -15))
                            && rightFindFlag[j - 2] == 1 && rightFindFlag[j - 1] == 1 && rightFindFlag[j] == 1)
                            //下一个拐点
                            {
                                second_right_turn_down[0] = j + 1;//数组里面没有第0行
                                second_right_turn_down[1] = R_black[j];
                                flag_second_right_down_point = 1;
                            }
                        }
                        /**************************弯道拐点判断（更加严苛）*****************************/
                        if (j < 42)
                        {

                            if (flag_left_down_point_wandao == 0 && abs(L_black[j - 1] - L_black[j - 2]) <= 3 && abs(L_black[j] - L_black[j - 1]) <= 3
                                && (L_black[j + 1] - L_black[j] >= 0) && (L_black[j + 2] - L_black[j] >= 1)
                                && leftFindFlag[j - 2] == 1 && leftFindFlag[j - 1] == 1 && leftFindFlag[j] == 1)
                            {

                                left_turn_down_wandao[0] = j + 1;//数组里面没有第0行
                                left_turn_down_wandao[1] = L_black[j];
                                flag_left_down_point_wandao = 1;
                            }
                            if (flag_right_down_point_wandao == 0 && abs(R_black[j - 1] - R_black[j - 2]) <= 3 && abs(R_black[j] - R_black[j - 1]) <= 3
                                && (R_black[j + 1] - R_black[j] <= 0) && (R_black[j + 2] - R_black[j] <= -1)
                                && rightFindFlag[j - 2] == 1 && rightFindFlag[j - 1] == 1 && rightFindFlag[j] == 1)
                            {
                                right_turn_down_wandao[0] = j + 1;
                                right_turn_down_wandao[1] = R_black[j];
                                flag_right_down_point_wandao = 1;
                            }
                        }
                        /**************************去噪下拐点判断（更加严苛）*****************************/
                        if (j > 25 && j < 36)
                        {
                            if (flag_left_down_point == 0 && L_black[j] - L_black[j - 3] >= 4 && L_black[j] - L_black[j - 2] >= 3 && L_black[j] - L_black[j - 1] >= 0
                                && (L_black[j] - L_black[j + 3] >= 4) && (L_black[j] - L_black[j + 2] >= 3) && (L_black[j] - L_black[j + 1] >= 0)
                                && leftFindFlag[j - 2] == 1 && leftFindFlag[j - 1] == 1 && leftFindFlag[j] == 1 && leftFindFlag[0] == 1)
                            {

                                left_turn_down[0] = j + 1;//数组里面没有第0行
                                left_turn_down[1] = L_black[j];
                                flag_left_down_point = 1;
                            }
                            if (flag_right_down_point == 0 && R_black[j] - R_black[j - 3] >= 4 && R_black[j] - R_black[j - 2] >= 3 && R_black[j] - R_black[j - 1] >= 0
                                && (R_black[j] - R_black[j + 1] >= 0) && (R_black[j] - R_black[j + 2] >= 3) && (R_black[j] - R_black[j + 3] >= 4)
                                && rightFindFlag[j - 2] == 1 && rightFindFlag[j - 1] == 1 && rightFindFlag[j] == 1 && rightFindFlag[0] == 1)
                            {
                                right_turn_down[0] = j + 1;
                                right_turn_down[1] = R_black[j];
                                flag_right_down_point = 1;
                            }
                        }
                        if (j < 40 && j > 10)
                        {
                            if (flag_left_round_in == 0 && (L_black[j] > R_black[j] + fixValue[j] + 15))    //判断进入左环岛
                            {
                                left_round_jishu += 1;
                                left_round_jishu_buchong += 1;
                            }
                            else if (flag_left_round_in == 0 && leftFindFlag[j] == 0)
                            {
                                left_round_jishu += 1;
                            }
                            else if (left_round_jishu != 0)
                            {
                                flag_left_round_in = 1;
                            }

                            if (flag_right_round_in == 0 && (R_black[j] < L_black[j] - fixValue[j] - 15))     //判断进入右环岛
                            {

                                right_round_jishu += 1;
                                right_round_jishu_buchong += 1;
                            }
                            else if (flag_right_round_in == 0 && rightFindFlag[j] == 0)
                            {
                                right_round_jishu += 1;
                            }
                            else if (right_round_jishu != 0)
                            {
                                flag_right_round_in = 1;
                            }

                        }
                        if ((flag_enter_shizi <= 2 && ((flag_right_up_point == 1 && flag_left_up_point == 1 && flag_left_down_point == 1 && flag_right_down_point == 1) || (left_turn_down[0] > 10 && right_turn_down[0] > 10)))
                        || (flag_second_right_down_point == 1 && flag_second_left_down_point == 1))
                        {
                            break;
                        }

                    }
                    SetText("1482              " );
                    if (flag_stopcar == 0)
                    {
                        for (int v = 20; v < 165; v++)
                        {
                            if (Pixels[25, v] == IMG_BLACK && Pixels[25, v - 1] == IMG_BLACK && Pixels[25, v + 1] == IMG_WHITE && Pixels[25, v + 2] == IMG_WHITE)      //7
                            {
                                parkingFee_jishu_25++;
                            }
                            if (Pixels[29, v] == IMG_BLACK && Pixels[29, v - 1] == IMG_BLACK && Pixels[29, v + 1] == IMG_WHITE && Pixels[29, v + 2] == IMG_WHITE)
                            {
                                parkingFee_jishu_29++;
                            }
                        }
                        SetText("parkingFee_jishu" + parkingFee_jishu_25 + "     " + parkingFee_jishu_29);
                        if ((parkingFee_jishu_25 >= 7&& parkingFee_jishu_25 <= 10) || (parkingFee_jishu_29 >= 7&& parkingFee_jishu_29 <= 10))
                        {
                            flag_stopcar = 1;
                        }
                    }
                    SetText("1510              ");
                }
                uint8_t delta;
                SetText("左环岛判断" + left_round_jishu + "  " + flag_left_round + "   " + flag_right_up_point + "   " + right_turn_up[0] + "  " + right_turn_up[1]);
                /********************左环岛判断*******************/
                if (flag_ramp == 0 && left_round_jishu_buchong >= 3 && left_round_jishu > 7 && flag_left_round == 0 && flag_right_up_point == 0)
                {
                    regression(2, 10, 35);
                    delta = (byte)abs((R_black[39] + R_black[5]) / 2 - R_black[22]);
                    SetText("right_k_30" + "  " + delta + "  " + parameterB + "   " + abs(R_black[30] - R_black[1]) + "   " + lostright_flag);
                    if (float_abs(parameterB - 1.9f) <= 0.3f && abs(R_black[30] - R_black[1]) < 67 && delta <= 5)
                    {
                        flag_left_round_jishu += 1;

                    }
                    else
                    {
                        flag_left_round_jishu = 0;
                    }
                }
                else
                    flag_left_round_jishu = 0;
                SetText("右环岛判断" + right_round_jishu + "  " + flag_right_round + "   " + flag_left_up_point + "   " + left_turn_up[0] + "  " + left_turn_up[1] + "  " + flag_right_round_jishu);
                /********************右环岛判断*******************/
                if (flag_ramp == 0 && right_round_jishu_buchong >= 3 && right_round_jishu > 7 && flag_right_round == 0 && flag_left_up_point == 0)
                {
                    regression(1, 0, 40);
                    delta = (byte)abs((L_black[39] + L_black[5]) / 2 - L_black[22]);
                    SetText("left_k_30" + "   " + delta + "  " + parameterB + "   " + abs(L_black[30] - L_black[1]));
                    if (float_abs(-parameterB - 2.0f) <= 0.3f && abs(L_black[30] - L_black[1]) < 67 && delta <= 5)
                    {
                        flag_right_round_jishu += 1;

                    }
                    else
                    {
                        flag_right_round_jishu = 0;
                    }
                }
                else
                {
                    flag_right_round_jishu = 0;
                }


                if (flag_left_round_jishu == 2 && flag_left_round_type == 0 && flag_right_round == 0)
                {
                    Round_jishu += 1;
                    flag_left_round_type = 1;
                    flag_left_round_jishu = 0;
                    flag_left_round = 1;

                }
                else if (flag_right_round_jishu == 2 && flag_right_round_type == 0 && flag_left_round == 0)
                {
                    Round_jishu += 1;
                    flag_right_round_type = 1;
                    flag_right_round_jishu = 0;
                    flag_right_round = 1;
                }
                if (flag_enter_shizi <= 6)
                {
                    float trend_of_left = 0;
                    float trend_of_right = 0;
                    float point_down_k = 0;
                    float point_up_k = 0;
                    byte twolines_trend = juge_if_same_fuhao(trend_of_left, trend_of_right);
                    if (left_turn_down[0] != 0 && right_turn_down[0] == 0)//左下拐点存在而右下拐点不存在
                    {
                        regression(1, left_turn_down[0] - 2, left_turn_down[0] + 3);
                        trend_of_left = parameterB;
                        regression(2, left_turn_down[0] - 2, left_turn_down[0] + 3);
                        trend_of_right = parameterB;
                        SetText("haodshi" + trend_of_left + "   " + trend_of_right);
                        twolines_trend = juge_if_same_fuhao(trend_of_left, (float)(trend_of_right - 1.9f));
                        if (twolines_trend == 0 && left_turn_down[0] > 26)
                        {
                            regression(1, left_turn_down[0] - 25, left_turn_down[0] - 10);
                            point_down_k = parameterB;
                            regression(1, left_turn_down[0] + 2, left_turn_down[0] + 10);
                            point_up_k = parameterB;
                            SetText("斜十字补充" + point_down_k + "   " + point_up_k);
                            if (point_down_k < -1.8f && point_down_k > -2.6f && left_turn_down[1] <= 100
                                && point_up_k < 7 && point_up_k > 6)
                                twolines_trend = 1;
                        }
                    }
                    else if (right_turn_down[0] != 0 && left_turn_down[0] == 0)    //右下拐点存在而左下拐点不存在
                    {
                        regression(1, right_turn_down[0] - 2, right_turn_down[0] + 3);
                        trend_of_left = parameterB;
                        regression(2, right_turn_down[0] - 2, right_turn_down[0] + 3);
                        trend_of_right = parameterB;
                        twolines_trend = juge_if_same_fuhao((float)(trend_of_left + 1.9f), trend_of_right);
                        if (twolines_trend == 0 && right_turn_down[0] > 26)
                        {
                            regression(2, right_turn_down[0] - 25, right_turn_down[0] - 10);
                            point_down_k = parameterB;
                            regression(2, right_turn_down[0] + 2, right_turn_down[0] + 10);
                            point_up_k = parameterB;
                            SetText("斜十字补充   " + point_down_k + "   " + point_up_k);
                            if (point_down_k > 1.8f && point_down_k < 2.6f && right_turn_down[1] >= 86
                                && point_up_k > -7 && point_up_k < -6)
                                twolines_trend = 1;
                        }
                    }
                    else if (left_turn_down[0] != 0 && right_turn_down[0] != 0)   //左右拐点均存在
                    {
                        regression(1, left_turn_down[0] - 2, left_turn_down[0] + 3);
                        trend_of_left = parameterB;
                        regression(2, right_turn_down[0] - 2, right_turn_down[0] + 3);
                        trend_of_right = parameterB;
                        twolines_trend = juge_if_same_fuhao(trend_of_left, trend_of_right);
                    }
                    SetText("十字趋势判断" + trend_of_left + "  " + trend_of_right);
                    ////("shia"+ trend_of_left+"   "+ trend_of_right);
                    //经过限制后的下拐点   首先是存在拐点，并且两条线趋势相异，或上相异的线丢失了
                    byte findrightdownguai = 0;
                    byte findleftdownguai = 0;
                    byte findrightupguai = 0;
                    byte findleftupguai = 0;
                    if ((left_turn_down[0] != 0 && twolines_trend == 1) || (lostright_flag >= 3 && left_turn_down[0] != 0))
                    {
                        findleftdownguai = 1;   //表示找到左下拐点了                
                    }
                    if ((right_turn_down[0] != 0 && twolines_trend == 1) || (lostleft_flag >= 3 && right_turn_down[0] != 0))
                    {
                        findrightdownguai = 1;//表示找到右下拐点了                
                    }
                    byte second_scan_flag = 0;
                    int start = 0;
                    int end = 0;
                    if (findrightdownguai == 1 || findleftdownguai == 1)
                    {


                        if (findrightdownguai == 1 && findleftdownguai == 0)//左斜入十字，仅有右下拐点，取右下拐点下的中线行
                        {
                            start = right_turn_down[0] - 20;
                            end = right_turn_down[0] - 3;
                            if (start <= 0) start = 0;
                            if (end <= 1) end = 1;
                            regression(0, start, end);
                        }
                        else if (findrightdownguai == 0 && findleftdownguai == 1) //右斜入十字，仅有左下拐点，取左下拐点下的中线行
                        {
                            start = left_turn_down[0] - 20;
                            end = left_turn_down[0] - 3;
                            if (start <= 0) start = 0;
                            if (end <= 1) end = 1;
                            regression(0, start, end);
                        }
                        else if (findrightdownguai == 1 && findleftdownguai == 1) //正入十字：用两个下拐点中最小行下的中线行，拟合出k，b，进而拟合出预测中线
                        {
                            start = My_Min(left_turn_down[0], right_turn_down[0]) - 10;
                            end = My_Min(left_turn_down[0], right_turn_down[0]) - 3;
                            if (start <= 0) start = 0;
                            if (end <= 1) end = 1;
                            regression(0, start, end);       //start是0，end是左下拐点和右下拐点的平均行数
                        }
                        //这样得出了拟合出的k和b
                        //预测新的中线值
                        //这时候forecast_LCenter
                        for (int i = 0; i < 70; i++)
                        {
                            //forecast_LCenter[i] = (byte)(parameterA * i + parameterB);
                            forecast_LCenter[i] = (byte)(parameterB * i + parameterA);
                            L_Start[i] = forecast_LCenter[i];
                            ////(forecast_LCenter[i]);
                        }
                        second_scan_flag = 1;
                    }
                    /********************第二次扫线（顺着预测出来的点往两边扫）********************/
                    uint8_t flag_first_turnpoint_left = 0;
                    uint8_t flag_first_turnpoint_right = 0;
                    if (second_scan_flag == 1)
                    {
                        if (flag_shizi_jinru == 0)
                        {
                            SetText("十字重扫" + start + "  " + end);
                            for (findLine = 0; findLine < 65; findLine++)
                            {
                                leftFindFlag[findLine] = 0;
                                rightFindFlag[findLine] = 0;
                                for (int i = (byte)forecast_LCenter[findLine]; i >= 0 && i < 184; i++)
                                {
                                    if (Pixels[findLine, i + 1] == 0 && Pixels[findLine, i] == 0)
                                    {
                                        leftFindFlag[findLine] = 1;
                                        L_black[findLine] = (byte)(i + 1);
                                        break;
                                    }
                                }
                                for (int i = (byte)forecast_LCenter[findLine]; i <= 185 && i > 1; i--)
                                {
                                    if (Pixels[findLine, i - 1] == 0 && Pixels[findLine, i] == 0)
                                    {
                                        rightFindFlag[findLine] = 1;
                                        R_black[findLine] = (byte)(i - 1);
                                        break;
                                    }
                                }
                                if (leftFindFlag[findLine] == 0 && rightFindFlag[findLine] != 0) L_black[findLine] = 185;
                                if (rightFindFlag[findLine] == 0 && leftFindFlag[findLine] != 0) R_black[findLine] = 0;
                                LCenter[findLine] = (byte)((L_black[findLine] + R_black[findLine]) / 2);
                                if (findLine > 5)
                                {
                                    if ((Pixels[findLine, LCenter[findLine]]) == IMG_BLACK || abs(L_black[findLine] - L_black[findLine - 1]) > 130 || abs(R_black[findLine] - R_black[findLine - 1]) > 130)
                                        break;
                                }
                            }
                        }
                        if (findleftdownguai == 1 && left_turn_down[0] > 10)
                        {
                            for (j = left_turn_down[0]; j <= My_Min((left_turn_down[0] + 37), 64); j++)
                            {
                                //左上拐点
                                if (L_black[j] != 185 && L_black[j] != 0 && flag_first_turnpoint_left == 0
                                    && (L_black[j] - L_black[j - 1]) <= -3 && (L_black[j] - L_black[j - 2]) <= -6
                                    && (L_black[j] - L_black[j - 2]) <= -8
                                    && abs(L_black[j + 2] - L_black[j + 1]) <= 4 && abs(L_black[j + 1] - L_black[j]) <= 3
                                    && leftFindFlag[j + 2] == 1 && leftFindFlag[j] == 1 && leftFindFlag[j + 1] == 1)
                                {

                                    left_turn_up[0] = j + 1;//数组里面没有第0行
                                    left_turn_up[1] = L_black[j];
                                    flag_first_turnpoint_left = 1;

                                }
                                if (L_black[j] != 185 && L_black[j] != 0 && flag_first_turnpoint_left == 1 && (abs(L_black[j - 1] - L_black[j - 2]) <= 5 || abs(L_black[j] - L_black[j - 1]) <= 5)
                                    && ((L_black[j + 1] - L_black[j] >= 10) || (L_black[j + 2] - L_black[j] >= 15))
                                    && leftFindFlag[j - 2] == 1 && leftFindFlag[j - 1] == 1 && leftFindFlag[j] == 1)
                                //下一个下拐点
                                {
                                    second_left_turn_down[0] = j + 1;//数组里面没有第0行
                                    second_left_turn_down[1] = L_black[j];
                                    break;
                                }
                            }
                        }
                        else
                        {
                            for (j = 10; j <= 43; j++)
                            {
                                //左上拐点
                                if (L_black[j] != 185 && L_black[j] != 0 && flag_first_turnpoint_left == 0
                                    && (L_black[j] - L_black[j - 1]) <= -3 && (L_black[j] - L_black[j - 2]) <= -6
                                    && abs(L_black[j + 2] - L_black[j + 1]) <= 4 && abs(L_black[j + 1] - L_black[j]) <= 3
                                    && leftFindFlag[j + 2] == 1 && leftFindFlag[j] == 1 && leftFindFlag[j + 1] == 1)
                                {

                                    left_turn_up[0] = j + 1;//数组里面没有第0行
                                    left_turn_up[1] = L_black[j];
                                    flag_first_turnpoint_left = 1;
                                }
                                if (L_black[j] != 185 && L_black[j] != 0 && flag_first_turnpoint_left == 1 && (abs(L_black[j - 1] - L_black[j - 2]) <= 5 || abs(L_black[j] - L_black[j - 1]) <= 5)
                                    && ((L_black[j + 1] - L_black[j] >= 10) || (L_black[j + 2] - L_black[j] >= 15))
                                    && leftFindFlag[j - 2] == 1 && leftFindFlag[j - 1] == 1 && leftFindFlag[j] == 1)
                                //下一个下拐点
                                {
                                    second_left_turn_down[0] = j + 1;//数组里面没有第0行
                                    second_left_turn_down[1] = L_black[j];
                                    break;
                                }
                            }
                        }

                        //SetText("日你妈妈皮"+ findrightdownguai +"  "+ right_turn_down[0] );
                        if (findrightdownguai == 1 && right_turn_down[0] > 10)
                        {

                            for (j = right_turn_down[0]; j <= My_Min((right_turn_down[0] + 37), 64); j++)
                            {
                                //右上拐点
                                if (R_black[j] != 0 && R_black[j] != 185 && flag_first_turnpoint_right == 0
                                    && (R_black[j] - R_black[j - 1]) >= 4 && (R_black[j] - R_black[j - 2]) >= 7
                                    && (R_black[j] - R_black[j - 3]) >= 10
                                    && abs(R_black[j + 2] - R_black[j + 1]) <= 4 && abs(R_black[j + 1] - R_black[j]) <= 3
                                    && rightFindFlag[j + 2] == 1 && rightFindFlag[j] == 1 && rightFindFlag[j + 1] == 1)
                                {
                                    right_turn_up[0] = j + 1;
                                    right_turn_up[1] = R_black[j];
                                    flag_first_turnpoint_right = 1;
                                }
                                if (R_black[j] != 0 && R_black[j] != 185 && flag_first_turnpoint_right == 1 && (abs(R_black[j - 1] - R_black[j - 2]) <= 5 || abs(R_black[j] - R_black[j - 1]) <= 5)
                                && ((R_black[j + 1] - R_black[j] <= -10) || (R_black[j + 2] - R_black[j] <= -15))
                                && rightFindFlag[j - 2] == 1 && rightFindFlag[j - 1] == 1 && rightFindFlag[j] == 1)
                                {
                                    second_right_turn_down[0] = j + 1;//数组里面没有第0行
                                    second_right_turn_down[1] = R_black[j];
                                    break;
                                }
                            }
                        }
                        else
                        {
                            for (j = 10; j <= 43; j++)
                            {
                                //右上拐点
                                if (R_black[j] != 0 && R_black[j] != 185 && R_black[j] != 185 && flag_first_turnpoint_right == 0 && (R_black[j] - R_black[j - 1]) >= 4 && (R_black[j] - R_black[j - 2]) >= 5
                                    && abs(R_black[j + 2] - R_black[j + 1]) <= 4 && abs(R_black[j + 1] - R_black[j]) <= 3
                                    && rightFindFlag[j + 2] == 1 && rightFindFlag[j] == 1 && rightFindFlag[j + 1] == 1)
                                {
                                    right_turn_up[0] = j + 1;
                                    right_turn_up[1] = R_black[j];
                                    flag_first_turnpoint_right = 1;
                                }
                                if (R_black[j] != 0 && R_black[j] != 185 && flag_first_turnpoint_right == 1 && (abs(R_black[j - 1] - R_black[j - 2]) <= 5 || abs(R_black[j] - R_black[j - 1]) <= 5)
                                && ((R_black[j + 1] - R_black[j] <= -10) || (R_black[j + 2] - R_black[j] <= -15))
                                && rightFindFlag[j - 2] == 1 && rightFindFlag[j - 1] == 1 && rightFindFlag[j] == 1)
                                {
                                    second_right_turn_down[0] = j + 1;//数组里面没有第0行
                                    second_right_turn_down[1] = R_black[j];
                                    break;
                                }
                            }
                        }
                        SetText("gan" + k_0_25);
                        if (right_turn_down[1] > 110 && flag_left_down_point == 0 && (right_turn_up[0] < right_turn_down[0]) && k_0_25 > 0.8f)
                        {
                            SetText("左转去噪");
                            if (right_turn_down[1] > 155)
                            {
                                for (int c = right_turn_down[0]; c < findLine; c++)
                                {
                                    L_black[c] = 185;
                                    R_black[c] = 185;
                                    LCenter[c] = 185;
                                }
                            }
                            else
                            {
                                regression(2, right_turn_down[0] - 7, right_turn_down[0] - 3);
                                for (int j = right_turn_down[0] - 3; j < findLine - 1; j++)
                                {
                                    if ((parameterB * j + parameterA) <= 185)
                                    {
                                        R_black[j] = (byte)(parameterB * j + parameterA);
                                        L_black[j] = 185;
                                        LCenter[j] = (byte)((L_black[j] + R_black[j]) / 2);
                                    }
                                    else if ((parameterB * j + parameterA) > 185)
                                    {
                                        R_black[j] = 185;
                                        L_black[j] = 185;
                                    }
                                }
                            }
                            findLine = (uint8_t)right_turn_down[0];
                        }
                        else if (left_turn_down[1] < 75 && flag_right_down_point == 0 && (left_turn_up[0] < left_turn_down[0]) && k_0_25 < -0.8f)
                        {
                            SetText("右转去噪");
                            if (left_turn_down[1] < 30)
                            {
                                for (int c = left_turn_down[0]; c < findLine; c++)
                                {
                                    L_black[c] = 0;
                                    R_black[c] = 0;
                                    LCenter[c] = 0;
                                }
                            }
                            else
                            {
                                regression(1, left_turn_down[0] - 7, left_turn_down[0] - 3);
                                for (int j = left_turn_down[0] - 3; j < findLine - 1; j++)
                                {
                                    if ((parameterB * j + parameterA) <= 185)
                                    {
                                        L_black[j] = (byte)(parameterB * j + parameterA);
                                        R_black[j] = 0;
                                    }
                                    else if ((parameterB * j + parameterA) > 185)
                                    {
                                        L_black[j] = 185;
                                        R_black[j] = 0;
                                        LCenter[j] = (byte)((L_black[j] + R_black[j]) / 2);
                                    }
                                }
                            }
                            findLine = (uint8_t)left_turn_down[0];
                        }
                        if (findleftdownguai == 1 && left_turn_down[0] >= 10)
                        {
                            SetText("左下" + left_turn_down[0] + "    " + left_turn_down[1]);
                        }
                        if (findrightdownguai == 1 && right_turn_down[0] >= 10)
                        {
                            SetText("右下" + right_turn_down[0] + "    " + right_turn_down[1]);
                        }
                        if (right_turn_up[0] > right_turn_down[0] && right_turn_up[0] != 0)
                        {
                            SetText("右上" + right_turn_up[0] + "    " + right_turn_up[1]);
                            findrightupguai = 1;//表示找到右上拐点了                
                        }
                        if (left_turn_up[0] > left_turn_down[0] && left_turn_up[0] != 0)
                        {
                            SetText("左上" + left_turn_up[0] + "    " + left_turn_up[1]);
                            findleftupguai = 1;//表示找到左上拐点了                
                        }
                    }
                    /*********开始补线(找到左下拐点和左上拐点  或 找到右下拐点和右上拐点)*********/
                    int start1_left;
                    int end1_left;
                    int start2_left;
                    int end2_left;
                    int start1_right;
                    int end1_right;
                    int start2_right;
                    int end2_right;
                    if (flag_small_S != 1 && flag_middle_S != 1)
                    {
                        if (findleftupguai == 1 && (findleftdownguai == 1 && left_turn_down[0] >= 12))        //找到左下拐点和左上拐点,拟合所需的点是下拐点下面三个点和上拐点上面三个点
                        {
                            start1_left = left_turn_down[0] - 3;
                            if (start1_left <= 0) start1_left = 0;
                            end1_left = left_turn_down[0] - 1;
                            start2_left = left_turn_up[0];
                            if (start2_left <= 0) start2_left = 0;
                            if (left_turn_up[0] < 30)
                                end2_left = left_turn_up[0] + 5;
                            else if (left_turn_up[0] < 35)
                                end2_left = left_turn_up[0] + 3;
                            else
                                end2_left = left_turn_up[0] + 2;
                            advanced_regression(1, start1_left, end1_left, start2_left, end2_left);
                            SetText("左边上下拉" + parameterB + "   " + parameterA);
                            /***********需要补的是上下两个拐点之间的点*******************************/
                            if (float_abs(parameterB) < 4)
                            {
                                flag_shizi_left = 1;
                                for (j = (byte)end1_left; j < (byte)start2_left; j++)
                                {
                                    if ((parameterB * j + parameterA) <= 185)
                                        L_black[j] = (byte)(parameterB * j + parameterA);
                                    else
                                        L_black[j] = 185;
                                }
                                flag_shizi_left_zhijie = 1;
                            }
                            else
                                flag_shizi_left_zhijie = 0;

                        }
                        else
                            flag_shizi_left = 0;
                        //并列关系
                        if (findrightupguai == 1 && (findrightdownguai == 1 && right_turn_down[0] >= 12))        //找到右下拐点和右上拐点,拟合所需的点是下拐点下面三个点和上拐点上面三个点
                        {
                            start1_right = right_turn_down[0] - 3;
                            if (start1_right <= 0) start1_right = 0;
                            end1_right = right_turn_down[0] - 1;
                            start2_right = right_turn_up[0];
                            if (start2_right <= 0) start2_right = 0;
                            if (right_turn_up[0] < 30)
                                end2_right = right_turn_up[0] + 5;
                            else if (right_turn_up[0] < 35)
                                end2_right = right_turn_up[0] + 3;
                            else
                                end2_right = right_turn_up[0] + 2;
                            advanced_regression(2, start1_right, end1_right, start2_right, end2_right);
                            //regression(2, start2_right, end2_right);
                            SetText("右边上下拉" + parameterB + "   " + parameterA + "   " + start2_right + "   " + end2_right);
                            /***********需要补的是上下两个拐点之间的点*******************************/
                            if (float_abs(parameterB) < 4)
                            {
                                flag_shizi_right = 1;
                                for (j = (byte)end1_right; j < (byte)start2_right; j++)
                                {
                                    if ((parameterB * j + parameterA) >= 0 && (parameterB * j + parameterA) <= 185)
                                        R_black[j] = (byte)(parameterB * j + parameterA);
                                    else if ((parameterB * j + parameterA) < 0)
                                        R_black[j] = 0;
                                    else if ((parameterB * j + parameterA) > 0)
                                        R_black[j] = 185;
                                }
                                flag_shizi_right_zhijie = 1;
                            }
                            else
                                flag_shizi_right_zhijie = 0;

                        }
                        else
                            flag_shizi_right = 0;

                        if (flag_shizi_left_zhijie == 1 && flag_shizi_right_zhijie == 1 && right_turn_up[0] < 40
                            && left_turn_up[0] < 40 && left_turn_up[0] > 0 && right_turn_up[0] > 0)
                        {
                            flag_shizi_jinru = 1;

                        }

                        /****************入十字了，用上面两个点拉下来**************************/
                        regression(0, 0, 25);
                        //(findrightupguai + "  " + findrightdownguai + "  " + right_turn_down[0] + "   " + right_turn_up[0] + "   " + right_turn_up[1] + "  " + left_turn_down[1] + "  " + findleftdownguai + "  " + parameterB);
                        if (((findrightupguai == 1 && (findrightdownguai == 0 || right_turn_down[0] < 12) && right_turn_up[0] < 43) && ((right_turn_up[1] < left_turn_down[1]) || findleftdownguai == 0)) && parameterB > -1.55f)
                        {

                            start1_right = right_turn_up[0] + 1;
                            if (start1_right <= 0) start1_right = 0;
                            if (right_turn_up[0] <= 20)
                                end1_right = right_turn_up[0] + 7;
                            else if (right_turn_up[0] <= 25)
                                end1_right = right_turn_up[0] + 6;
                            else if (right_turn_up[0] <= 30)
                                end1_right = right_turn_up[0] + 5;
                            else
                                end1_right = right_turn_up[0] + 4;
                            regression(2, start1_right, end1_right);
                            SetText("右边直接拉" + parameterB + "   " + parameterA);
                            if (float_abs(parameterB) < 4)
                            {
                                for (j = 0; j < (byte)start1_right; j++)
                                {
                                    if ((parameterB * j + parameterA) >= 0)
                                        R_black[j] = (byte)(parameterB * j + parameterA);
                                    else if ((parameterB * j + parameterA) < 0)
                                        R_black[j] = 0;

                                }
                            }
                        }
                        regression(0, 0, 25);
                        if (((findleftupguai == 1 && (findleftdownguai == 0 || left_turn_down[0] < 12) && left_turn_up[0] < 43) && ((left_turn_up[1] > right_turn_down[1]) || findrightdownguai == 0)) && parameterB < 1.55f)
                        {
                            start1_left = left_turn_up[0] + 1;
                            if (start1_left <= 0) start1_left = 0;
                            if (left_turn_up[0] <= 20)
                                end1_left = left_turn_up[0] + 7;
                            else if (left_turn_up[0] <= 25)
                                end1_left = left_turn_up[0] + 6;
                            else if (left_turn_up[0] <= 30)
                                end1_left = left_turn_up[0] + 5;
                            else
                                end1_left = left_turn_up[0] + 4;
                            regression(1, start1_left, end1_left);
                            SetText("左边直接拉" + parameterB + "   " + parameterA);
                            if (float_abs(parameterB) < 4)
                            {
                                for (j = 0; j < (byte)start1_left; j++)
                                {
                                    if ((parameterB * j + parameterA) <= 185)
                                        L_black[j] = (byte)(parameterB * j + parameterA);
                                    else
                                        L_black[j] = 185;
                                }
                            }

                        }
                    }
                }
                else if (flag_shizi_jinru == 1 || flag_enter_shizi > 6)
                {
                    int start1 = right_turn_up[0];
                    int end1 = right_turn_up[0] + 6;
                    if (abs(right_turn_up[0] - left_turn_up[0]) >= 15 && My_Max(left_turn_up[0], right_turn_up[0]) > 30)//若两个上拐点相差过大，说明肯定上面的那个上拐点是误判，所以可以直接隔绝掉这个点
                    {
                        if (right_turn_up[0] > left_turn_up[0])       //右上拐点是假的
                        {
                            start1 = left_turn_up[0];
                            if (start1 <= 0) start1 = 0;
                            end1 = left_turn_up[0] + 10;
                            regression(1, start1, end1);

                            /***********需要补的是上下两个拐点之间的点*******************************/
                            if (float_abs(parameterB) < 8)
                            {
                                SetText("左边过十字拉" + parameterB + "   " + parameterA + "   " + left_turn_up[0]);
                                for (j = 0; j < (byte)start1; j++)
                                {
                                    if ((parameterB * j + parameterA) <= 185)
                                        L_black[j] = (byte)(parameterB * j + parameterA);
                                    else
                                        L_black[j] = 185;
                                }
                            }
                        }
                        else                                     //左上拐点是假定
                        {
                            start1 = right_turn_up[0];
                            if (start1 <= 0) start1 = 0;
                            end1 = right_turn_up[0] + 6;
                            regression(2, start1, end1);

                            /***********需要补的是上下两个拐点之间的点*******************************/
                            if (float_abs(parameterB) < 4)
                            {
                                SetText("右边过十字拉" + parameterB + "   " + parameterA + "   " + right_turn_up[0]);
                                for (j = 0; j < (byte)start1; j++)
                                {
                                    if ((parameterB * j + parameterA) >= 0)
                                        R_black[j] = (byte)(parameterB * j + parameterA);
                                    else if ((parameterB * j + parameterA) < 0)
                                        R_black[j] = 0;
                                    else if ((parameterB * j + parameterA) > 0)
                                        R_black[j] = 185;
                                }
                            }
                        }
                    }
                    else                                   //都是真的
                    {
                        start1 = right_turn_up[0];
                        if (start1 <= 0) start1 = 0;
                        end1 = right_turn_up[0] + 6;
                        regression(2, start1, end1);

                        /***********需要补的是上下两个拐点之间的点*******************************/
                        if (float_abs(parameterB) < 4)
                        {
                            SetText("右边过十字拉" + parameterB + "   " + parameterA + "   " + right_turn_up[0]);
                            for (j = 0; j < (byte)start1; j++)
                            {
                                if ((parameterB * j + parameterA) >= 0)
                                    R_black[j] = (byte)(parameterB * j + parameterA);
                                else if ((parameterB * j + parameterA) < 0)
                                    R_black[j] = 0;
                                else if ((parameterB * j + parameterA) > 0)
                                    R_black[j] = 185;
                            }
                        }
                        start1 = left_turn_up[0];
                        if (start1 <= 0) start1 = 0;
                        end1 = left_turn_up[0] + 6;
                        regression(1, start1, end1);

                        /***********需要补的是上下两个拐点之间的点*******************************/
                        if (float_abs(parameterB) < 8)
                        {
                            SetText("左边过十字拉" + parameterB + "   " + parameterA + "   " + left_turn_up[0]);
                            for (j = 0; j < (byte)start1; j++)
                            {
                                if ((parameterB * j + parameterA) <= 185)
                                    L_black[j] = (byte)(parameterB * j + parameterA);
                                else
                                    L_black[j] = 185;
                            }
                        }
                    }
                    //start1 = right_turn_up[0];
                    //if (start1 <= 0) start1 = 0;
                    //end1 = right_turn_up[0] + 8;
                    //regression(2, start1, end1);

                    ///***********需要补的是上下两个拐点之间的点*******************************/
                    //if (float_abs(parameterB) < 4)
                    //{
                    //    SetText("右边过十字拉" + parameterB + "   " + parameterA+"   "+ right_turn_up[0]);
                    //    for (j = 0; j < (byte)start1; j++)
                    //    {
                    //        if ((parameterB * j + parameterA) >= 0)
                    //            R_black[j] = (byte)(parameterB * j + parameterA);
                    //        else if ((parameterB * j + parameterA) < 0)
                    //            R_black[j] = 0;
                    //        else if ((parameterB * j + parameterA) > 0)
                    //            R_black[j] = 185;
                    //    }
                    //}
                    //start1 = left_turn_up[0];
                    //if (start1 <= 0) start1 = 0;
                    //end1 = left_turn_up[0] + 10;
                    //regression(1, start1, end1);

                    ///***********需要补的是上下两个拐点之间的点*******************************/
                    //if (float_abs(parameterB) < 8)
                    //{
                    //    SetText("左边过十字拉" + parameterB + "   " + parameterA+"   "+ left_turn_up[0]);
                    //    for (j = 0; j < (byte)start1; j++)
                    //    {
                    //        if ((parameterB * j + parameterA) <= 185)
                    //            L_black[j] = (byte)(parameterB * j + parameterA);
                    //        else
                    //            L_black[j] = 185;
                    //    }
                    //}

                }
                if (flag_shizi_jinru == 1 && (right_turn_up[0] < 10 || left_turn_up[0] < 10))
                    flag_shizi_jinru = 0;

                byte blank_jishu_left = 0;
                byte blank_jishu_right = 0;
                int max = 0;
                int min = 185;
                
                
                for (byte i = 2; i <= 30; i++)
                {
                    if (L_black[i] == 185 && L_black[i+1] != 185&& left_pointDouble_flag==0)
                    {
                        SetText("左二重补线       "+ i + "      "+(L_black[i] + L_black[i + 12] - 2 * L_black[i + 6]));
                        if (abs(L_black[i] + L_black[i + 12] - 2 * L_black[i + 6]) < 4)
                        {
                            regression(1, i + 1, i + 10);
                            k_left_double = parameterB;
                            b_left_double = parameterA;
                            left_point_double = i;
                            left_pointDouble_flag = 1;
                        }
                    }
                    if (R_black[i] == 0 && R_black[i + 1] != 0&& right_pointDouble_flag==0)
                    {
                        SetText("右二重补线       " + i + "      " + (R_black[i] + R_black[i + 12] - 2 * R_black[i + 6]));
                        if (abs(R_black[i] + R_black[i + 12] - 2 * R_black[i + 6]) < 4)
                        {
                            regression(2, i + 1, i + 10);
                            k_right_double = parameterB;
                            b_right_double = parameterA;
                            right_point_double = i;
                            right_pointDouble_flag = 1;
                        }
                    }
                }
                /******弯道补线*******/
                float k_niheLCenter = 0;
                if (findLine > 4)
                {
                    regression(0, 0, findLine - 3);
                    k_niheLCenter = parameterB;
                }
                else
                {
                    k_niheLCenter = 0;
                }
                for (int i = 0; i <= findLine; i++)
                {
                    if (right_pointDouble_flag == 1 || left_pointDouble_flag == 1)
                    {
                        if (left_pointDouble_flag == 1 && right_pointDouble_flag == 0)
                        {
                            if (i < left_point_double)
                                LCenter[i] = (byte)((k_left_double * i + b_left_double + R_black[i]) / 2);
                            else
                                LCenter[i] = (byte)((L_black[i] + R_black[i]) / 2);
                        }
                        else if (left_pointDouble_flag == 0 && right_pointDouble_flag == 1)
                        {
                            if(i< right_point_double)
                                LCenter[i] = (byte)((L_black[i] + k_right_double * i + b_right_double) / 2);
                            else
                                LCenter[i] = (byte)((L_black[i] + R_black[i]) / 2);
                        }
                        else if (left_pointDouble_flag == 1 && right_pointDouble_flag == 1)
                        {
                            if(i<My_Min(left_point_double, right_point_double))
                                LCenter[i] = (byte)((k_left_double * i + b_left_double + k_right_double * i + b_right_double) / 2);
                            else if(i < My_Max(left_point_double, right_point_double))
                            {
                                if(left_point_double> right_point_double)
                                    LCenter[i] = (byte)((k_left_double * i + b_left_double + R_black[i]) / 2);
                                else
                                    LCenter[i] = (byte)((L_black[i] + k_right_double * i + b_right_double) / 2);
                            }
                            else
                                LCenter[i] = (byte)((L_black[i] + R_black[i]) / 2);
                        }


                    }
                    else
                        LCenter[i] = (byte)((L_black[i] + R_black[i]) / 2);
                    centerLine[i] = LCenter[i];
                    if (flag_enter_right == 1)
                    {
                        if (rightFindFlag[i] == 0)
                        {
                            blank_jishu_right += 1;
                        }
                        else if (R_black[i + 1] > 0 && R_black[i] > 0 && R_black[i + 1] <= 10 && R_black[i] <= 6 && i != 0)
                        {
                            if (i == blank_jishu_right)
                            {
                                flag_enter_right = 0;
                                flag_right_jiasu_panduan = 1;
                            }
                        }

                    }

                    if (flag_enter_left == 1)
                    {
                        if (leftFindFlag[i] == 0)
                        {
                            blank_jishu_left += 1;
                        }
                        else if (L_black[i + 1] < 185 && L_black[i] < 185 && L_black[i + 1] >= 175 && L_black[i] >= 179 && i != 0)
                        {
                            if (i == blank_jishu_left)
                            {
                                flag_enter_left = 0;
                                flag_left_jiasu_panduan = 1;
                            }
                        }
                    }
                    if (LCenter[i] >= max && i < findLine - 5) max = LCenter[i];
                    if (LCenter[i] <= min && i < findLine - 5) min = LCenter[i];
                    if (i == 0)  //累差
                    {
                        cumulants1 = 0;
                        cumulants2 = 0;
                        cumulants3 = 0;
                        cumulants4 = 0;
                        cumulants5 = 0;
                    }
                    else
                    {
                        if (i < 25)
                        {
                            cumulants3 += ((LCenter[i] - 93));                  //与中线的偏移量
                        }
                        if (i <= 25 && i > 5)
                            cumulants2 += abs((LCenter[i] - LCenter[i - 1]));   //倾斜程度
                        if (i <= 40)
                        {
                            cumulants1 += abs((LCenter[i] - LCenter[i - 1]));  //用于判断40行以下部分是否连续
                        }
                        if (i < (findLine - 3))                                //连续性
                        {

                            cumulants4 += abs(LCenter[i] - (int)(parameterB * i + parameterA));
                        }
                        if (i < 35)                                           //中线累差
                            cumulants5 += abs((LCenter[i] - LCenter[i - 1]));
                    }

                    /*********************  中线拐点判断*******************/
                    //中线右拐点
                    if (i > 9 && flag_centre_right_point == 0
                    && (LCenter[i - 5] - LCenter[i - 4]) >= 0
                    && (LCenter[i - 6] - LCenter[i - 4]) >= 0
                    && (LCenter[i - 7] - LCenter[i - 4]) >= 0
                    && (LCenter[i - 8] - LCenter[i - 4]) >= 0
                    && (LCenter[i - 8] - LCenter[i - 4]) <= 20                        //判断j-4是否为中线拐点，拐点在左边的那个
                    && (LCenter[i - 3] - LCenter[i - 4]) >= 0
                    && (LCenter[i - 2] - LCenter[i - 4]) >= 0
                    && (LCenter[i - 2] - LCenter[i - 4]) <= 20
                    && (LCenter[i - 1] - LCenter[i - 4]) >= 1
                    && (LCenter[i] - LCenter[i - 4]) >= 1
                    && LCenter[i - 4] >= 19 && LCenter[i - 4] <= 165)
                    {
                        centre_right_point[0] = i - 3;
                        centre_right_point[1] = LCenter[i - 4];
                        flag_centre_right_point = 1;
                    }
                    //中线左拐点
                    if (i > 9 && flag_centre_left_point == 0
                    && (LCenter[i - 5] - LCenter[i - 4]) <= 0
                    && (LCenter[i - 6] - LCenter[i - 4]) <= 0
                    && (LCenter[i - 7] - LCenter[i - 4]) <= 0
                    && (LCenter[i - 8] - LCenter[i - 4]) <= 0
                    && (LCenter[i - 8] - LCenter[i - 4]) >= -20       //判断j-4是否为中线拐点，拐点在左边的那个
                    && (LCenter[i - 3] - LCenter[i - 4]) <= 0
                    && (LCenter[i - 2] - LCenter[i - 4]) <= 0
                    && (LCenter[i - 2] - LCenter[i - 4]) >= -20
                    && (LCenter[i - 1] - LCenter[i - 4]) <= -1
                    && (LCenter[i] - LCenter[i - 4]) <= -1
                    && LCenter[i - 4] >= 19 && LCenter[i - 4] <= 165)
                    {
                        centre_left_point[0] = i - 3;
                        centre_left_point[1] = LCenter[i - 4];
                        flag_centre_left_point = 1;
                    }
                }
                center_max_min_delta = max - min;
                /*********************  弯道加速判断*******************/
                if (blank_jishu_left >= findLine - 2 && findLine <= 36)
                {
                    if (flag_jinwan_qita == 1)
                        jishu_zuizhong_qita = jishu_jinwan_qita;
                    flag_jinwan_left = 1;
                    jishu_jinwan_left += 1;
                    jishu_jinwan_qita = 0;
                    flag_jinwan_qita = 0;
                }
                else if (blank_jishu_right >= findLine - 2 && findLine <= 36)
                {
                    if (flag_jinwan_qita == 1)
                        jishu_zuizhong_qita = jishu_jinwan_qita;
                    flag_jinwan_right = 1;
                    jishu_jinwan_right += 1;
                    jishu_jinwan_qita = 0;
                    flag_jinwan_qita = 0;
                }
                else
                {
                    if (flag_jinwan_left == 1)
                    {
                        jishu_zuizhong_left = jishu_jinwan_left;
                        flag_jinwan_left = 0;
                        jishu_jinwan_left = 0;
                    }
                    else if (flag_jinwan_right == 1)
                    {
                        jishu_zuizhong_right = jishu_jinwan_right;
                        flag_jinwan_right = 0;
                        jishu_jinwan_right = 0;
                    }
                    flag_jinwan_qita = 1;
                    jishu_jinwan_qita += 1;
                }
                //("****************************?>?>?>?>?>?>" + jishu_jinwan_left + "   " + jishu_jinwan_right + "    " + jishu_jinwan_qita);
                //("   " + jishu_zuizhong_left + "  " + jishu_zuizhong_right + "    " + jishu_zuizhong_qita);



                //问号弯暂时注释
                //if (flag_small_S != 1 && flag_middle_S != 1 && right_turn_down_wandao[0] >= 20 && (((LCenter[22 - 1] + LCenter[22] + LCenter[22 + 1]) / 3 - LCenter[4]) > 10 || center_max_min_delta > 30))
                //{
                //    regression(0, 0, right_turn_down_wandao[0]);
                //    SetText("右弯道准备判断" + right_turn_down_wandao[0] + "    " + parameterB + "    " + k_niheLCenter);
                //    if (parameterB > k_niheLCenter && parameterB > 0.2f && k_niheLCenter <= 1
                //        && LCenter[40] < 120 && R_black[right_turn_down_wandao[0]] < 120)
                //    {
                //        SetText("**************问号弯");
                //        flag_judge_wenhaowan = 1;
                //    }
                //}
                //else if (flag_small_S != 1 && flag_middle_S != 1 && left_turn_down_wandao[0] >= 20 && (((LCenter[22 - 1] + LCenter[22] + LCenter[22 + 1]) / 3 - LCenter[4]) < -10 || center_max_min_delta > 30))
                //{
                //    regression(0, 0, left_turn_down_wandao[0]);
                //    SetText("左弯道准备判断" + left_turn_down_wandao[0] + "    " + parameterB + "    " + k_niheLCenter);
                //    if (parameterB < k_niheLCenter && parameterB < -0.2f && k_niheLCenter >= -1
                //        && LCenter[40] > 120 && L_black[left_turn_down_wandao[0]] > 60)
                //    {
                //        SetText("******************问号弯");
                //        flag_judge_wenhaowan = 1;
                //    }
                //}
                //else
                //    flag_judge_wenhaowan = 0;

                /***********  弯道补线*******************/

                //if (flag_small_S == 0 && flag_middle_S != 1 && flag_judge_wenhaowan == 0)
                //{
                //    for (int i = 0; i <= findLine; i++)
                //    {
                //        if (flag_enter_right == 1)
                //        {
                //            if (L_black[i] != 185)
                //            {
                //                LCenter[i] = AvoidOverflow(L_black[i] - fixValue[i] / 2);
                //                centerLine[i] = LCenter[i];
                //            }
                //        }
                //        else if (flag_enter_left == 1)
                //        {
                //            if (R_black[i] != 0)
                //            {
                //                LCenter[i] = AvoidOverflow(R_black[i] + fixValue[i] / 2);
                //                centerLine[i] = LCenter[i];
                //            }
                //        }

                //    }
                //}



                /***********  补线结束*******************/
                /***********  判断坡道*******************/

                left_delta = (uint8_t)((L_black[0] - L_black[35]) / 3);
                right_delta = (uint8_t)((R_black[35] - R_black[0]) / 3);
                regression(1, 20, 40);
                float left_k_20_40 = parameterB;
                regression(2, 20, 40);
                float right_k_20_40 = parameterB;
                SetText(left_k_20_40 + "     " + right_k_20_40 + "    " + (L_black[50] - R_black[50]) + "  " + fixValue[50]);
                
                if (abs(L_black[11] - (L_black[0] - left_delta)) <= 7
                    && abs(L_black[23] - (L_black[11] - left_delta)) <= 7
                    && abs(R_black[11] - (R_black[0] + left_delta)) <= 7
                    && abs(R_black[23] - (R_black[11] + left_delta)) <= 7
                    && left_k_20_40 < 1.1f
                    && right_k_20_40 < 0.9f
                    && (L_black[50] - R_black[50] - fixValue[50]) > 30)
                {
                    flag_ramp_jishu += 1;
                    flag_doube_shizi = 0;
                    flag_enter_shortstr_jishu = 0;
                    flag_enter_changzhidao_jishu = 0;
                    flag_small_S = 0;
                    flag_middle_S_jishu = 0;
                    SetText("坡道");
                }
                /***********  判断长直道  *******************/
                //else if (abs(second_left_turn_down[0] - second_right_turn_down[0]) < 5
                //    && (second_left_turn_down[1] - second_right_turn_down[1]) > 40
                //    && second_left_turn_down[1] <= 155 && second_left_turn_down[1] >= 100
                //    && second_right_turn_down[1] <= 105 && second_right_turn_down[1] >= 50
                //    && second_right_turn_down[0] != 0 && second_left_turn_down[0] != 0)   //左右拐点均存在
                //{
                //    float trend_of_left1 = 0;
                //    float trend_of_right1 = 0;
                //    regression(1, second_left_turn_down[0] - 1, second_left_turn_down[0] + 1);
                //    trend_of_left1 = parameterB;
                //    regression(2, second_right_turn_down[0] - 1, second_right_turn_down[0] + 1);
                //    trend_of_right1 = parameterB;
                //    if (juge_if_same_fuhao(trend_of_left1, trend_of_right1) == 1)
                //    {
                //        flag_doube_shizi += 1;
                //        flag_enter_changzhidao_jishu = 0;
                //        flag_small_S = 0;
                //        flag_enter_shortstr_jishu = 0;
                //        flag_middle_S_jishu = 0;
                //        flag_ramp_jishu = 0;
                //    }
                //}          //双十字冲
                else if (flag_enter_changzhidao == 0 &&
                    flag_enter_shortstr == 0 &&
                    cumulants2 <= 12 && abs(cumulants1) <= 45 &&
                    
                    LCenter[42] > 53 && LCenter[42] < 143 &&
                    (L_black[40] - R_black[40]) >= 25 && (L_black[35] - R_black[35]) <= 68 &&
                    findLine >= 40)
                {
                    flag_doube_shizi = 0;
                    flag_enter_changzhidao_jishu += 1;
                    flag_small_S = 0;
                    flag_enter_shortstr_jishu = 0;
                    flag_middle_S_jishu = 0;
                    flag_ramp_jishu = 0;
                }
                /***********  判断短直道*******************/
                else if (flag_enter_shortstr == 0 && flag_small_S == 0 &&
                    cumulants2 <= 12 && abs(cumulants1) <= 120 &&
                    cumulants5 < 20 &&
                    abs(cumulants3) < 500 &&
                    (L_black[30] - R_black[30]) >= 65 && (L_black[25] - R_black[25]) <= 150 &&
                    findLine > 35 && findLine <= 45)
                {
                    flag_doube_shizi = 0;
                    flag_enter_shortstr_jishu += 1;
                    flag_enter_changzhidao_jishu = 0;
                    flag_small_S = 0;
                    flag_middle_S_jishu = 0;
                    flag_ramp_jishu = 0;
                }
                /***********  判断中S(替换函数)*******************/
                else if (flag_middle_S == 0 && abs(centre_right_point[0] - centre_left_point[0]) >= 10 && abs(centre_right_point[0] - centre_left_point[0]) <= 35
                    && abs(centre_right_point[1] - centre_left_point[1]) >= 30 && abs(centre_right_point[1] - centre_left_point[1]) <= 72
                    && centre_right_point[0] != 0 && centre_left_point[0] != 0
                    && findLine >= 47 && cumulants2 <= 40
                    )
                {
                    flag_middle_S_jishu += 1;
                    flag_doube_shizi = 0;
                    flag_small_S = 0;
                    flag_enter_changzhidao_jishu = 0;
                    flag_enter_shortstr_jishu = 0;
                    flag_enter_shortstr = 0;
                    flag_enter_changzhidao = 0;
                    flag_ramp_jishu = 0;
                }
                /***********  判断小S(替换函数)*******************/
                else if ((flag_shizi_left == 0 && flag_shizi_right == 0)
                    && abs(centre_right_point[0] - centre_left_point[0]) >= 8 && abs(centre_right_point[0] - centre_left_point[0]) <= 28
                    && abs(centre_right_point[1] - centre_left_point[1]) >= 10 && abs(centre_right_point[1] - centre_left_point[1]) <= 41
                    && centre_right_point[0] != 0 && centre_left_point[0] != 0
                    && findLine >= 43 && cumulants2 >= 5
                    && (centre_right_point[1] + centre_left_point[1]) > 90 && (centre_right_point[1] + centre_left_point[1]) < 282
                    )
                {
                    flag_doube_shizi = 0;
                    flag_small_S = 1;
                    flag_enter_changzhidao_jishu = 0;
                    flag_enter_shortstr_jishu = 0;
                    flag_middle_S_jishu = 0;
                    flag_enter_shortstr = 0;
                    flag_enter_changzhidao = 0;
                    flag_ramp_jishu = 0;
                }
                else
                {
                    flag_doube_shizi = 0;
                    flag_enter_changzhidao_jishu = 0;
                    flag_small_S = 0;
                    flag_enter_shortstr_jishu = 0;
                    flag_middle_S_jishu = 0;
                    flag_ramp_jishu = 0;

                }
            }
            #endregion
            #region[停车]
            else if (flag_stopcar == 1&& startDirect!=0)
            {
                if (startDirect == 1)
                {
                    #region[左停车]

                    //if (flag_stopcar_type1 == 0)
                    //{
                    //    SetText("向左停车");
                    //    for (j = 2; j <= 49; j++)
                    //    {

                    //        if (j > 2 && j <= 34)
                    //        {

                    //            if (flag_left_down_point == 0 && abs(L_black[j - 1] - L_black[j - 2]) <= 3
                    //                && abs(L_black[j] - L_black[j - 1]) <= 3 && (L_black[j + 1] - L_black[j] >= 1)
                    //                && (L_black[j + 3] - L_black[j] >= 3)
                    //                && leftFindFlag[j - 2] == 1 && leftFindFlag[j - 1] == 1
                    //                && L_black[j] > 130
                    //                && leftFindFlag[j] == 1)
                    //            {   //左下

                    //                left_turn_down[0] = j + 1;//数组里面没有第0行
                    //                left_turn_down[1] = L_black[j];
                    //                flag_left_down_point = 1;
                    //            }
                    //            if (flag_right_down_point == 0)                       //找断掉的点
                    //            {
                    //                if ((R_black[j + 1] - R_black[j]) > 40 && abs(R_black[j - 1] - R_black[j - 2]) <= 3
                    //                    && abs(R_black[j] - R_black[j - 1]) <= 3)
                    //                {
                    //                    right_turn_down[0] = j + 1;
                    //                    right_turn_down[1] = R_black[j];
                    //                    flag_right_down_point = 1;
                    //                }
                    //            }
                    //        }
                    //        //入十字用的上拐点
                    //        if (flag_right_down_point == 1)
                    //        {
                    //            //左上拐点
                    //            if (flag_left_up_point == 0
                    //            && (L_black[j] - L_black[j - 2]) <= -8 && (L_black[j] - L_black[j - 1]) <= -8
                    //            && abs(L_black[j + 2] - L_black[j + 1]) <= 3 && abs(L_black[j + 1] - L_black[j]) <= 4
                    //            && leftFindFlag[j + 1] == 1)
                    //            {

                    //                left_turn_up[0] = j + 1;//数组里面没有第0行
                    //                left_turn_up[1] = L_black[j];
                    //                flag_left_up_point = 1;
                    //                if (right_turn_down[0] <= 22)
                    //                {
                    //                    flag_stopcar_type1 = 1;
                    //                    break;
                    //                }

                    //            }


                    //        }


                    //        if ((flag_enter_shizi <= 2 && (flag_right_up_point == 1 && flag_left_up_point == 1 && flag_left_down_point == 1 && flag_right_down_point == 1)))
                    //        {
                    //            break;
                    //        }

                    //    }
                    //}
                    //else if (flag_stopcar_type1 == 1 && flag_stopcar_type2 == 0)
                    //{
                    //    for (int i = 35; i > 2; i--)
                    //    {
                    //        if (flag_left_up_point == 0
                    //        && (L_black[i] - L_black[i - 2]) <= -8 && (L_black[i] - L_black[i - 1]) <= -8
                    //        && abs(L_black[i + 3] - L_black[i + 2]) <= 3 && abs(L_black[i + 2] - L_black[i + 1]) <= 3 && abs(L_black[i + 1] - L_black[i]) <= 4
                    //        && leftFindFlag[i + 1] == 1)
                    //        {

                    //            left_turn_up[0] = i + 1;//数组里面没有第0行
                    //            left_turn_up[1] = L_black[i];
                    //            flag_left_up_point = 1;
                    //            if (left_turn_up[0] < 17)
                    //            {
                    //                flag_stopcar_type2 = 1;
                    //            }
                    //            break;
                    //        }
                    //    }
                    //}
                    //if (flag_stopcar_type2 == 1)
                    //{
                    //    for (int i = 35; i > 2; i--)
                    //    {
                    //        if (flag_right_up_point == 0
                    //        && (R_black[i] - R_black[i - 2]) >= 10 && (R_black[i] - R_black[i - 1]) >= 10
                    //        && abs(R_black[i] + R_black[i + 4] - 2 * R_black[i + 2]) <= 3)
                    //        {

                    //            right_turn_up[0] = i + 1;//数组里面没有第0行
                    //            right_turn_up[1] = L_black[i];
                    //            flag_right_up_point = 1;
                    //            //if (right_turn_up[0] < 10 && right_turn_up[0] > 2)
                    //            //{
                    //            //    flag_stopcar_type2 = 0;
                    //            //}
                    //            break;
                    //        }
                    //    }
                    //}
                    //double k5_left;
                    //double b5_left;
                    //if (flag_left_up_point == 1 && flag_stopcar_type2 == 0)
                    //{

                    //    k5_left = (left_turn_up[1]) / (left_turn_up[0]+1);
                    //    b5_left = 0;
                    //    for (int i = 0; i <= left_turn_up[0]; i++)
                    //    {
                    //        R_black[i] = (byte)(k5_left * i + b5_left);
                    //        L_black[i] = 185;
                    //        LCenter[i] = (byte)((R_black[i] + L_black[i])/2);
                    //    }

                    //}
                    //else if (flag_stopcar_type2 == 1)
                    //{
                    //    regression(2, right_turn_up[0], right_turn_up[0] + 5);
                    //    for (j = 0; j < (byte)right_turn_up[0] + 20; j++)
                    //    {
                    //        if ((parameterB * j + parameterA) >= 0)
                    //            R_black[j] = (byte)(parameterB * j + parameterA);
                    //        else if ((parameterB * j + parameterA) < 0)
                    //            R_black[j] = 0;
                    //        else if ((parameterB * j + parameterA) > 0)
                    //            R_black[j] = 185;
                    //        LCenter[j] = (byte)(R_black[j] + fixValue[j] / 2);

                    //    }
                    //}
                    //SetText("起跑线右下" + right_turn_down[0] + "    " + right_turn_down[1]);
                    //SetText("起跑线左上" + left_turn_up[0] + "    " + left_turn_up[1]);
                    //SetText("起跑线左下" + left_turn_down[0] + "     " + left_turn_down[1]);
                    //SetText("起跑线右上" + right_turn_up[0] + "    " + right_turn_up[1]);
                    //SetText("状态1      " + flag_stopcar_type1);
                    //SetText("状态2      " + flag_stopcar_type2);
                    #endregion
                    
                    for (int i = 0; i < 10; i++)
                    {
                        if (Pixels[i, 185] == IMG_WHITE)
                        {
                            flag_stopcar_people = 1;
                        }
                    }
                }
                else if(startDirect == 2)
                {
                    #region[右停车]
                    //if (flag_stopcar_type1 == 0)
                    //{
                    //    SetText("向右停车");
                    //    for (j = 2; j <= 49; j++)
                    //    {

                    //        if (j > 2 && j <= 34)
                    //        {

                    //            if (flag_left_down_point == 0 && abs(L_black[j - 1] - L_black[j - 2]) <= 3
                    //                && abs(L_black[j] - L_black[j - 1]) <= 3 && (L_black[j + 1] - L_black[j] >= 1)
                    //                && (L_black[j + 3] - L_black[j] >= 3)
                    //                && leftFindFlag[j - 2] == 1 && leftFindFlag[j - 1] == 1
                    //                && L_black[j] > 130
                    //                && leftFindFlag[j] == 1)
                    //            {   //左下

                    //                left_turn_down[0] = j + 1;//数组里面没有第0行
                    //                left_turn_down[1] = L_black[j];
                    //                flag_left_down_point = 1;
                    //            }
                    //            if (flag_right_down_point == 0)
                    //            {
                    //                if ((R_black[j + 1] - R_black[j]) > 40 && abs(R_black[j - 1] - R_black[j - 2]) <= 3
                    //                    && abs(R_black[j] - R_black[j - 1]) <= 3)
                    //                {
                    //                    right_turn_down[0] = j + 1;
                    //                    right_turn_down[1] = R_black[j];
                    //                    flag_right_down_point = 1;
                    //                }
                    //            }
                    //        }
                    //        //入十字用的上拐点
                    //        if (flag_right_down_point == 1)
                    //        {
                    //            //左上拐点
                    //            if (flag_left_up_point == 0
                    //            && (L_black[j] - L_black[j - 2]) <= -8 && (L_black[j] - L_black[j - 1]) <= -8
                    //            && abs(L_black[j + 2] - L_black[j + 1]) <= 3 && abs(L_black[j + 1] - L_black[j]) <= 4
                    //            && leftFindFlag[j + 1] == 1)
                    //            {

                    //                left_turn_up[0] = j + 1;//数组里面没有第0行
                    //                left_turn_up[1] = L_black[j];
                    //                flag_left_up_point = 1;
                    //                if (right_turn_down[0] <= 22)
                    //                {
                    //                    flag_stopcar_type1 = 1;
                    //                    break;
                    //                }

                    //            }


                    //        }


                    //        if ((flag_enter_shizi <= 2 && (flag_right_up_point == 1 && flag_left_up_point == 1 && flag_left_down_point == 1 && flag_right_down_point == 1)))
                    //        {
                    //            break;
                    //        }

                    //    }
                    //}
                    //else if (flag_stopcar_type1 == 1 && flag_stopcar_type2 == 0)
                    //{
                    //    for (int i = 35; i > 2; i--)
                    //    {
                    //        if (flag_left_up_point == 0
                    //        && (L_black[i] - L_black[i - 2]) <= -8 && (L_black[i] - L_black[i - 1]) <= -8
                    //        && abs(L_black[i + 3] - L_black[i + 2]) <= 3 && abs(L_black[i + 2] - L_black[i + 1]) <= 3 && abs(L_black[i + 1] - L_black[i]) <= 4
                    //        && leftFindFlag[i + 1] == 1)
                    //        {

                    //            left_turn_up[0] = i + 1;//数组里面没有第0行
                    //            left_turn_up[1] = L_black[i];
                    //            flag_left_up_point = 1;
                    //            if (left_turn_up[0] < 17)
                    //            {
                    //                flag_stopcar_type2 = 1;
                    //            }
                    //            break;
                    //        }
                    //    }
                    //}
                    //if (flag_stopcar_type2 == 1)
                    //{
                    //    for (int i = 35; i > 2; i--)
                    //    {
                    //        if (flag_right_up_point == 0
                    //        && (R_black[i] - R_black[i - 2]) >= 10 && (R_black[i] - R_black[i - 1]) >= 10
                    //        && abs(R_black[i] + R_black[i + 4] - 2 * R_black[i + 2]) <= 3)
                    //        {

                    //            right_turn_up[0] = i + 1;//数组里面没有第0行
                    //            right_turn_up[1] = L_black[i];
                    //            flag_right_up_point = 1;
                    //            //if (right_turn_up[0] < 10 && right_turn_up[0] > 2)
                    //            //{
                    //            //    flag_stopcar_type2 = 0;
                    //            //}
                    //            break;
                    //        }
                    //    }
                    //}
                    //double k5_left;
                    //double b5_left;
                    //if (flag_left_up_point == 1 && flag_stopcar_type2 == 0)
                    //{
                    //    if (left_turn_down[0] < 15)
                    //    {
                    //        k5_left = (left_turn_up[1]) / (left_turn_up[0]);
                    //        b5_left = 0;
                    //        for (int i = 0; i <= left_turn_up[0]; i++)
                    //        {
                    //            R_black[i] = (byte)(k5_left * i + b5_left);
                    //        }
                    //        LeastSquareCalc_Curve(0, (byte)(left_turn_up[0]), 2);
                    //        for (int j = 0; j < left_turn_up[0]; j++)
                    //        {
                    //            if ((j + 1) * (j + 1) * (curve_a) + curve_b > 185)
                    //            {
                    //                break;
                    //            }
                    //            R_black[j] = (byte)((j + 1) * (j + 1) * (curve_a) + curve_b);
                    //            L_black[j] = 185;
                    //            LCenter[j] = (byte)((L_black[j] + R_black[j]) / 2);

                    //        }
                    //    }
                    //    else
                    //    {
                    //        k5_left = (left_turn_up[1] - R_black[left_turn_down[0]]) / (left_turn_up[0] - left_turn_down[0]);
                    //        b5_left = R_black[left_turn_down[0]] - k5_left * left_turn_down[0];
                    //        for (int i = left_turn_down[0]; i <= left_turn_up[0]; i++)
                    //        {
                    //            R_black[i] = (byte)(k5_left * i + b5_left);
                    //        }
                    //        LeastSquareCalc_Curve(0, (byte)(left_turn_up[0]), 2);
                    //        for (int j = 0; j < left_turn_up[0]; j++)
                    //        {
                    //            if ((j + 1) * (j + 1) * (curve_a) + curve_b > 185)
                    //            {
                    //                break;
                    //            }
                    //            R_black[j] = (byte)((j + 1) * (j + 1) * (curve_a) + curve_b);
                    //            LCenter[j] = (byte)(R_black[j] + fixValue[j] / 2);

                    //        }
                    //    }
                    //}
                    //else if (flag_stopcar_type2 == 1)
                    //{
                    //    regression(2, right_turn_up[0], right_turn_up[0] + 5);
                    //    for (j = 0; j < (byte)right_turn_up[0] + 20; j++)
                    //    {
                    //        if ((parameterB * j + parameterA) >= 0)
                    //            R_black[j] = (byte)(parameterB * j + parameterA);
                    //        else if ((parameterB * j + parameterA) < 0)
                    //            R_black[j] = 0;
                    //        else if ((parameterB * j + parameterA) > 0)
                    //            R_black[j] = 185;
                    //        LCenter[j] = (byte)(R_black[j] + fixValue[j] / 2);

                    //    }
                    //}
                    //SetText("起跑线右下" + right_turn_down[0] + "    " + right_turn_down[1]);
                    //SetText("起跑线左上" + left_turn_up[0] + "    " + left_turn_up[1]);
                    //SetText("起跑线左下" + left_turn_down[0] + "     " + left_turn_down[1]);
                    //SetText("起跑线右上" + right_turn_up[0] + "    " + right_turn_up[1]);
                    //SetText("状态1      " + flag_stopcar_type1);
                    //SetText("状态2      " + flag_stopcar_type2);
                    #endregion
                    for (int i = 0; i < 10; i++)
                    {
                        if (Pixels[i, 0] == IMG_WHITE)
                        {
                            flag_stopcar_people = 1;
                        }
                    }
                }
            }
            #endregion
            #region[左环岛]
            byte flag_left_round_point = 0;        //左环岛进入时的拐点
            byte flag_left_round_out_up_point = 0;
            byte left_round_out_up_point = 0;
            flag_left_round_up = 0;
            if (flag_left_round==1)
            {
                for (j = 2; j <= 49; j++)
                {
                    if (flag_changzhidao_jishu >= 45) break;
                    if (flag_left_round_type == 1 && j > 23 && j < 50)
                    {
                        if (flag_left_round_point == 0
                           && L_black[j - 4] - L_black[j] >= 1 && L_black[j - 3] - L_black[j] >= 0
                           && L_black[j - 2] - L_black[j] >= 0 && L_black[j - 1] - L_black[j] >= 0
                           && (L_black[j + 2] - L_black[j] >= 0) && (L_black[j + 1] - L_black[j] >= 0)
                           && (L_black[j + 4] - L_black[j] >= 1) && (L_black[j + 3] - L_black[j] >= 0)
                           && leftFindFlag[j - 2] == 1 && leftFindFlag[j - 1] == 1 && leftFindFlag[j] == 1
                           && leftFindFlag[j + 4] == 1 && leftFindFlag[j + 3] == 1 && leftFindFlag[j + 1] == 1)
                        {
                            left_round_point[0] = j + 1;//数组里面没有第0行
                            left_round_point[1] = L_black[j];
                            flag_left_round_point = 1;
                        }
                    }
                    //左环岛上拐点

                    if (flag_left_round_type == 2
                        && flag_left_round_up == 0
                        && j > 15
                        && (L_black[j] - L_black[j + 1]) <= 6
                        && (L_black[j - 2] - L_black[j]) >= 20 && (L_black[j - 1] - L_black[j]) >= 20)
                    {
                        SetText("好");
                        left_round_up[0] = j + 1;//数组里面没有第0行
                        left_round_up[1] = L_black[j];
                        flag_left_round_up = 1;
                    }

                    if ((flag_left_round_type == 4 || flag_left_round_type == 5) && j > 5 && j <= 40 &&
                        flag_right_down_point == 0
                        && (R_black[j] - R_black[j + 1] >= 0) && (R_black[j] - R_black[j + 2] >= 0) && (R_black[j] - R_black[j + 3] >= 0)
                        && (R_black[j] - R_black[j - 1] >= 1) && (R_black[j] - R_black[j - 2] >= 1) && (R_black[j] - R_black[j - 3] >= 2)
                        && rightFindFlag[j - 2] == 1 && rightFindFlag[j - 1] == 1 && rightFindFlag[j] == 1
                         && R_black[j] != 185)
                    {
                        right_turn_down[0] = j + 1;
                        right_turn_down[1] = R_black[j];
                        flag_right_down_point = 1;
                    }
                    if ((flag_left_round_type == 4 || flag_left_round_type == 5 || flag_left_round_type == 6) && j > 20 && j < 50)
                    {
                        if (flag_left_round_out_up_point == 0
                            && Pixels[j - 1, 185] == IMG_WHITE && Pixels[j, 185] == IMG_WHITE
                            && Pixels[j + 1, 185] == IMG_BLACK && Pixels[j + 2, 185] == IMG_BLACK)
                        {
                            left_round_out_up_point = (byte)(j + 1);
                            round_overflow_point = left_round_out_up_point;
                            flag_left_round_out_up_point = 1;
                        }
                    }
                    if (flag_left_round_type == 6)
                    {
                        if (j < 10&& flag_left_round_type6==0
                        && flag_right_down_point == 0
                        && (R_black[j] - R_black[j + 1] >= 0) && (R_black[j] - R_black[j + 2] >= 0)
                        && rightFindFlag[j] == 1)
                        {
                            right_turn_down[0] = j + 1;
                            right_turn_down[1] = R_black[j];
                            flag_right_down_point = 1;
                            
                        }
                        if (right_turn_down[0] < 5)
                            flag_left_round_type6 = 1;
                    }
                    if ((flag_left_round_type == 7 || flag_left_round_type == 6)  && j >= 3)
                    {
                        //完全出左环岛的上拐点
                        if (flag_left_up_point == 0 && (L_black[j] - L_black[j - 2]) <= -5
                            && (L_black[j] - L_black[j - 1]) <= -3 && abs(L_black[j + 2] - L_black[j + 1]) <= 3 && abs(L_black[j + 1] - L_black[j]) <= 3
                            && leftFindFlag[j + 2] == 1 && leftFindFlag[j] == 1 && leftFindFlag[j + 1] == 1)
                        {

                            left_turn_up[0] = j + 1;//数组里面没有第0行
                            left_turn_up[1] = L_black[j];
                            flag_left_up_point = 1;
                        }
                    }
                    

                }
                
                SetText("type1 point" + left_round_point[0] + "  " + left_round_point[1]);
                SetText("diao" + left_round_up[0] + "  " + left_round_up[1]);
                SetText("type4  " + right_turn_down[0] + "  " + right_turn_down[1]);
                SetText("out point" + left_round_out_up_point);
                SetText("left round out point" + left_turn_up[0]);
                //左环岛补线准备
                double k1_left = 0;
                double b1_left = 0;
                if (flag_left_round_type == 2 || flag_left_round_type == 3)
                {
                    //if (left_round_up[0] > 28)
                    //{
                        k1_left = (left_round_up[1] - R_black[0]) * 1.0 / ((left_round_up[0] - 0) * 1.0);
                        b1_left = R_black[0];
                    //}
                    //else
                    //{
                    //    regression(2, left_round_up[0], left_round_up[0]+4);
                    //    k1_left = parameterB;
                    //    b1_left = parameterA;
                    //    SetText("zty");
                    //}
                }
                //if (right_turn_down[1] > 0)
                //{
                //    right_turn_down[1] = 0;
                //}
                double k2_left = (right_turn_down[1] * 1.0 - 185) / (right_turn_down[0] - left_round_out_up_point * 1.0);
                double b2_left = 185 - k2_left * left_round_out_up_point * 1.0f;
                byte start3_left = (byte)(left_turn_up[0] + 1);
                if (start3_left <= 0) start3_left = 0;
                byte end3_left = (byte)(left_turn_up[0] + 5);
                regression(1, start3_left, end3_left);
                double k3_left = parameterB;
                double b3_left = parameterA;
                double k4_left = 0;
                double b4_left = 0;
                if (flag_left_round_type == 4 && right_turn_down[0] > 5)
                {
                    regression(2, right_turn_down[0] - 5, right_turn_down[0] - 1);
                    k4_left = parameterB;
                    b4_left = parameterA;
                }
                for (int i = 0; i <= findLine; i++)
                {
                    /*************************** 左边进环岛补线***************************/
                    if (flag_left_round_type == 1)
                    {
                        if (i < left_round_point[0])
                        {
                            L_black[i] = AvoidOverflow(R_black[i] + fixValue[i]);
                            LCenter[i] = (byte)((L_black[i] + R_black[i]) / 2);
                        }
                    }
                    else if (flag_left_round_type == 2 && left_round_up[0] != 0)
                    {

                        if (i < left_round_up[0])
                        {
                            R_black[i] = (byte)(k1_left * i + b1_left);
                        }

                    }
                    else if (flag_left_round_type == 3)
                    {
                        if (i < left_round_up[0])
                        {
                            R_black[i] = (byte)(k1_left * i + b1_left);
                        }
                    }
                    else if (flag_left_round_type == 4 && right_turn_down[0] != 0)
                    {
                        if (i > right_turn_down[0] && i < left_round_out_up_point)
                        {
                            R_black[i] = (byte)(k4_left * i + b4_left);
                            LCenter[i] = (byte)((L_black[i] + R_black[i]) / 2);
                        }
                    }
                    else if (flag_left_round_type == 5 || flag_left_round_type == 6)
                    {
                        if (i < left_round_out_up_point && i > right_turn_down[0])
                        {

                            R_black[i] = (byte)(k2_left * i + b2_left);
                            LCenter[i] = (byte)((L_black[i] + R_black[i]) / 2);
                            centerLine[i] = LCenter[i];
                            if ((R_black[i] >= 160 || (R_black[i] < R_black[i - 1])) && i > 20)
                            {
                                round_overflow_point = (byte)(i + 1);
                                SetText("tpye6"+ right_turn_down[0] + right_turn_down[1]);
                                break;
                            }

                        }
                    }
                    else if (flag_left_round_type == 7)
                    {
                        if (i < left_turn_up[0] + 3)
                        {
                            L_black[i] = AvoidOverflow((int)(k3_left * i + b3_left));
                            LCenter[i] = (byte)((L_black[i] + R_black[i]) / 2);
                        }
                    }
                    
                }
                if ((flag_left_round_type == 2 || flag_left_round_type == 3)&& left_round_up[0]!=0)
                {
                    LeastSquareCalc_Curve(0, (byte)(left_round_up[0]), 2);
                    SetText(curve_a + " 12345 " + curve_b);
                    for (int j = 0; j < 50; j++)
                    {
                        if ((j + 1) * (j + 1) * (curve_a + 0.01f) + curve_b > 185)
                        {
                            break;
                        }
                        R_black[j] = (byte)((j + 1) * (j + 1) * (curve_a + 0.01f) + curve_b);
                        LCenter[j] = (byte)((L_black[j] + R_black[j]) / 2);

                    }
                }

                for (byte i = 2; i <= 20; i++)
                {
                    if (L_black[i] == 185 && L_black[i + 1] != 185 && left_pointDouble_flag == 0)
                    {
                        SetText("左二重补线       " + i + "      " + (L_black[i] + L_black[i + 12] - 2 * L_black[i + 6]));
                        if (abs(L_black[i] + L_black[i + 12] - 2 * L_black[i + 6]) < 4)
                        {
                            regression(1, i + 1, i + 10);
                            k_left_double = parameterB;
                            b_left_double = parameterA;
                            left_point_double = i;
                            left_pointDouble_flag = 1;
                        }
                    }
                    if (R_black[i] == 0 && R_black[i + 1] != 0 && right_pointDouble_flag == 0)
                    {
                        SetText("右二重补线       " + i + "      " + (R_black[i] + R_black[i + 12] - 2 * R_black[i + 6]));
                        if (abs(R_black[i] + R_black[i + 12] - 2 * R_black[i + 6]) < 4)
                        {
                            regression(2, i + 1, i + 10);
                            k_right_double = parameterB;
                            b_right_double = parameterA;
                            right_point_double = i;
                            right_pointDouble_flag = 1;
                        }
                    }
                }
                if (right_pointDouble_flag == 1 || left_pointDouble_flag == 1)
                {
                    if (right_point_double > left_point_double)
                    {
                        for (int i = 0; i <= right_point_double; i++)
                        {
                            LCenter[i] = (byte)((L_black[i] + k_right_double * i + b_right_double) / 2);
                        }
                    }
                    else if (right_point_double < left_point_double)
                    {
                        for (int i = 0; i <= left_point_double; i++)
                        {
                            LCenter[i] = (byte)((k_left_double * i + b_left_double + R_black[i]) / 2);
                        }
                    }
                }
                /***********  弯道补线*******************/
                //if (flag_left_round_type == 2 || flag_left_round_type == 3)
                //{
                //    for (int i = 0; i <= findLine; i++)
                //    {
                //        if (flag_right_round_type == 2 || flag_right_round_type == 3)
                //        {
                //            if (L_black[i] != 185)
                //                LCenter[i] = AvoidOverflow(L_black[i] - fixValue[i] / 2);
                //        }
                //        else if (flag_left_round_type == 2 || flag_left_round_type == 3)
                //        {
                //            if (R_black[i] != 0)
                //                LCenter[i] = AvoidOverflow(R_black[i] + fixValue[i] / 2);
                //        }

                //    }
                //}
                //else if (flag_left_round_point == 1)
                //{
                //    for (int i = 0; i <= findLine; i++)
                //    {
                //        if (flag_enter_right == 1)
                //        {
                //            if (L_black[i] != 185)
                //                LCenter[i] = AvoidOverflow(L_black[i] - fixValue[i] / 2);
                //        }
                //        else if (flag_enter_left == 1)
                //        {
                //            if (R_black[i] != 0)
                //                LCenter[i] = AvoidOverflow(R_black[i] + fixValue[i] / 2);
                //        }

                //    }
                //}
                regression(0, 0, 30);
                SetText("middle_k_30" + parameterB);
                /*********************************左转状态转化*********************************/
                SetText("一状态" + flag_left_round_jishu);
                
                if (jishu_type1 > 1 && flag_left_round_type == 1 && left_round_point[0] <= 31 && left_round_point[0] >= 20 && leftFindFlag[15] == 1)
                {
                    flag_left_round_type = 2;
                    jishu_type1 = 0;
                }
                else if (jishu_type2 > 1 && flag_left_round_type == 2 && ((left_round_up[0] <= 38 && left_round_up[0] > 23)))
                {
                    flag_left_round_type = 3;
                    jishu_type2 = 0;
                }
                else if (jishu_type3 > 1 && flag_left_round_type == 3 && left_round_up[0] <= 21 && left_round_up[0] > 10)
                {
                    flag_left_round_type = 4;
                    jishu_type3 = 0;
                }
                else if (jishu_type4 > 1 && flag_left_round_type == 4 && right_turn_down[0] < 35 && right_turn_down[0] > 20)
                {
                    flag_left_round_type = 5;
                    jishu_type4 = 0;
                }
                else if (jishu_type5 > 1 && flag_left_round_type == 5 && right_turn_down[0] <= 16 && right_turn_down[0] != 0)
                {
                    flag_left_round_type = 6;
                    flag_left_round_type6 = 0;
                    jishu_type5 = 0;
                }
                else if (jishu_type6 > 5 && flag_left_round_type == 6 && ((Pixels[4, 185] == IMG_BLACK && Pixels[5, 185] == IMG_BLACK) || (float_abs(parameterB) <= 2.3f && (left_round_out_up_point >= 38 || left_round_out_up_point == 0))))
                {
                    flag_left_round_type = 7;
                    jishu_type6 = 0;
                }

                else if (jishu_type7 > 7 && flag_left_round_type == 7 && (Pixels[25, 175] == IMG_BLACK) && ((left_turn_up[0] <= 10 && left_turn_up[0] >= 3) || (leftFindFlag[10] == 1 && left_turn_up[0]<20)))
                {
                    flag_left_round_type = 0;
                    jishu_type7 = 0;
                }

                if (flag_left_round_type != 0)
                {
                    flag_left_round = 1;
                }
                else
                {
                    flag_left_round = 0;
                }
            }
            #endregion
            #region[右环岛]
            byte flag_right_round_point = 0;        //右环岛进入时的拐点
            byte flag_right_round_out_up_point = 0;
            byte right_round_out_up_point = 0;
            flag_right_round_up = 0;
            if (flag_right_round == 1)
            {
                for (j = 2; j <= 49; j++)
                {
                    if (flag_changzhidao_jishu >= 45) break;
                    if (flag_right_round_type == 1 && j > 23 && j < 50)
                    {
                        if (flag_right_round_point == 0
                           && R_black[j] - R_black[j - 4] >= 1 && R_black[j] - R_black[j - 3] >= 0
                           && R_black[j] - R_black[j - 2] >= 0 && R_black[j] - R_black[j - 1] >= 0
                           && (R_black[j] - R_black[j + 2] >= 0) && (R_black[j] - R_black[j + 1] >= 0)
                           && (R_black[j] - R_black[j + 4] >= 1) && (R_black[j] - R_black[j + 3] >= 0)
                           && rightFindFlag[j - 2] == 1 && rightFindFlag[j - 1] == 1 && rightFindFlag[j] == 1
                           && rightFindFlag[j + 2] == 1 && rightFindFlag[j + 1] == 1 && rightFindFlag[j + 4] == 1)
                        {
                            right_round_point[0] = j + 1;//数组里面没有第0行
                            right_round_point[1] = R_black[j];
                            flag_right_round_point = 1;
                        }
                    }
                    //右环岛上拐点

                    if (flag_right_round_type == 2
                        && flag_right_round_up == 0
                        && j > 15
                        && (R_black[j + 1] - R_black[j]) <= 6
                        && (R_black[j] - R_black[j - 2]) >= 20 && (R_black[j] - R_black[j - 1]) >= 20)
                    {
                        SetText("好");
                        right_round_up[0] = j + 1;//数组里面没有第0行
                        right_round_up[1] = R_black[j];
                        flag_right_round_up = 1;
                    }


                    if ((flag_right_round_type == 4 || flag_right_round_type == 5) && j > 5 && j < 40 &&
                        flag_left_down_point == 0
                        && (L_black[j + 1] - L_black[j] >= 0) && (L_black[j + 2] - L_black[j] >= 0) && (L_black[j + 3] - L_black[j] >= 0)
                        && (L_black[j - 1] - L_black[j] >= 1) && (L_black[j - 2] - L_black[j] >= 1) && (L_black[j - 3] - L_black[j] >= 2)
                        && leftFindFlag[j - 2] == 1 && leftFindFlag[j - 1] == 1 && leftFindFlag[j] == 1
                        && L_black[j] != 0)
                    {
                        left_turn_down[0] = j + 1;
                        left_turn_down[1] = L_black[j];
                        flag_left_down_point = 1;
                    }


                    if ((flag_right_round_type == 4 || flag_right_round_type == 5 || flag_right_round_type == 6) && j > 20 && j < 50)
                    {
                        if (flag_right_round_out_up_point == 0
                            && Pixels[j - 1, 0] == IMG_WHITE && Pixels[j,0]==IMG_WHITE 
                            && Pixels[j + 1, 0] == IMG_BLACK && Pixels[j + 2, 0] == IMG_BLACK)
                        {
                            right_round_out_up_point = (byte)(j + 1);
                            round_overflow_point = right_round_out_up_point;
                            flag_right_round_out_up_point = 1;
                        }
                    }
                    if (flag_right_round_type == 6)
                    {
                        if (flag_right_round_type6 == 0 && j < 10
                            && flag_left_down_point == 0
                            && (L_black[j + 1] - L_black[j] >= 0) && (L_black[j + 2] - L_black[j] >= 0)
                            && leftFindFlag[j] == 1)
                        {
                            left_turn_down[0] = j + 1;
                            left_turn_down[1] = L_black[j];
                            flag_left_down_point = 1;
                        }
                        if (left_turn_down[0] < 5)
                            flag_right_round_type6 = 1;
                    }
                    //if (flag_left_round_type == 6)
                    //{
                    //    if (j < 10 && flag_left_round_type6 == 0
                    //    && flag_right_down_point == 0
                    //    && (R_black[j] - R_black[j + 1] >= 0) && (R_black[j] - R_black[j + 2] >= 0)
                    //    && rightFindFlag[j] == 1)
                    //    {
                    //        right_turn_down[0] = j + 1;
                    //        right_turn_down[1] = R_black[j];
                    //        flag_right_down_point = 1;
                    //        if (right_turn_down[0] < 5)
                    //            flag_left_round_type6 = 1;
                    //    }
                    //}
                    if ((flag_right_round_type == 7 || flag_right_round_type == 6) && j >= 3)
                    {
                        //完全出右环岛的上拐点
                        if (flag_right_up_point == 0 && (R_black[j] - R_black[j - 2]) >= 5
                            && (R_black[j] - R_black[j - 1]) >= 3 && abs(R_black[j + 2] - R_black[j + 1]) <= 3 && abs(R_black[j + 1] - R_black[j]) <= 3
                            && rightFindFlag[j + 2] == 1 && rightFindFlag[j] == 1 && rightFindFlag[j + 1] == 1)
                        {

                            right_turn_up[0] = j + 1;//数组里面没有第0行
                            right_turn_up[1] = R_black[j];
                            flag_right_up_point = 1;
                        }
                    }
                }

               
                SetText("bigdiao" + right_round_point[0] + "  " + right_round_point[1]);
                SetText("diao" + right_round_up[0] + "  " + right_round_up[1]);
                SetText("out down point" + left_turn_down[0] + "  " + left_turn_down[1]);
                SetText("out point" + right_round_out_up_point + "   " + L_black[right_round_out_up_point]);
                SetText("right round out point" + right_turn_up[0]);
                //右环岛补线准备
                double k1_right = (right_round_up[1] - L_black[0]) * 1.0 / ((right_round_up[0] - 0) * 1.0);
                double b1_right = L_black[0];
                double k2_right = (left_turn_down[1] * 1.0 - 0) / (left_turn_down[0] - right_round_out_up_point * 1.0);
                double b2_right = 0 - k2_right * right_round_out_up_point * 1.0f;
                byte start3_right = (byte)(right_turn_up[0] + 1);
                if (start3_right <= 0) start3_right = 0;
                byte end3_right = (byte)(right_turn_up[0] + 5);
                regression(2, start3_right, end3_right);
                double k3_right = parameterB;
                double b3_right = parameterA;
                double k4_right = 0;
                double b4_right = 0;
                if (flag_right_round_type == 4 && left_turn_down[0] > 5)
                {
                    regression(1, left_turn_down[0] - 5, left_turn_down[0] - 1);
                    k4_right = parameterB;
                    b4_right = parameterA;
                }
                for (int i = 0; i <= findLine; i++)
                {
                    /*************************** 右边进环岛补线***************************/
                    if (flag_right_round_type == 1)
                    {
                        if (i < right_round_point[0])
                        {
                            R_black[i] = AvoidOverflow(L_black[i] - fixValue[i]);
                            LCenter[i] = (byte)((L_black[i] + R_black[i]) / 2);
                        }
                    }
                    else if (flag_right_round_type == 2 && right_round_up[0]!=0)
                    {
                        if (i < right_round_up[0])
                        {
                            L_black[i] = (byte)(k1_right * i + b1_right);

                        }

                    }
                    else if (flag_right_round_type == 3)
                    {
                        if (i < right_round_up[0])
                        {
                            L_black[i] = (byte)(k1_right * i + b1_right);

                        }
                    }
                    else if (flag_right_round_type == 4 && left_turn_down[0] != 0)
                    {
                        if (i > left_turn_down[0] && i < right_round_out_up_point)
                        {
                            L_black[i] = (byte)(k4_right * i + b4_right);
                            LCenter[i] = (byte)((L_black[i] + R_black[i]) / 2);
                        }
                    }
                    else if (flag_right_round_type == 5 || flag_right_round_type == 6)
                    {
                        if (i < right_round_out_up_point && i > left_turn_down[0])
                        {
                            L_black[i] = (byte)(k2_right * i + b2_right);
                            LCenter[i] = (byte)((L_black[i] + R_black[i]) / 2);
                            centerLine[i] = LCenter[i];
                            if ((L_black[i] <= 25 || (L_black[i] > L_black[i - 1])) && i > 20)
                            {
                                round_overflow_point = (byte)(i + 1);
                                SetText("jih牛逼");
                                break;
                            }

                        }
                    }
                    else if (flag_right_round_type == 7)
                    {
                        if (i < right_turn_up[0] + 3)
                        {
                            R_black[i] = AvoidOverflow((int)(k3_right * i + b3_right));
                            LCenter[i] = (byte)((L_black[i] + R_black[i]) / 2);
                        }
                    }
                    
                }
                if ((flag_right_round_type == 2 || flag_right_round_type == 3)&& right_round_up[0] != 0)
                {
                    LeastSquareCalc_Curve(0, (byte)(right_round_up[0]), 1);
                    for (int j = 0; j < 50; j++)
                    {
                        if ((j + 1) * (j + 1) * (curve_a - 0.01f) + curve_b < 0)
                        {
                            break;
                        }
                        L_black[j] = (byte)((j + 1) * (j + 1) * (curve_a - 0.01f) + curve_b);
                        LCenter[j] = (byte)((L_black[j] + R_black[j]) / 2);

                    }
                }


                for (byte i = 2; i <= 20; i++)
                {
                    if (L_black[i] == 185 && L_black[i + 1] != 185 && left_pointDouble_flag == 0)
                    {
                        SetText("左二重补线       " + i + "      " + (L_black[i] + L_black[i + 12] - 2 * L_black[i + 6]));
                        if (abs(L_black[i] + L_black[i + 12] - 2 * L_black[i + 6]) < 4)
                        {
                            regression(1, i + 1, i + 10);
                            k_left_double = parameterB;
                            b_left_double = parameterA;
                            left_point_double = i;
                            left_pointDouble_flag = 1;
                        }
                    }
                    if (R_black[i] == 0 && R_black[i + 1] != 0 && right_pointDouble_flag == 0)
                    {
                        SetText("右二重补线       " + i + "      " + (R_black[i] + R_black[i + 12] - 2 * R_black[i + 6]));
                        if (abs(R_black[i] + R_black[i + 12] - 2 * R_black[i + 6]) < 4)
                        {
                            regression(2, i + 1, i + 10);
                            k_right_double = parameterB;
                            b_right_double = parameterA;
                            right_point_double = i;
                            right_pointDouble_flag = 1;
                        }
                    }
                }
                for (byte i = 2; i <= 20; i++)
                {
                    if (L_black[i] == 185 && L_black[i + 1] != 185 && left_pointDouble_flag == 0)
                    {
                        SetText("左二重补线       " + i + "      " + (L_black[i] + L_black[i + 12] - 2 * L_black[i + 6]));
                        if (abs(L_black[i] + L_black[i + 12] - 2 * L_black[i + 6]) < 4)
                        {
                            regression(1, i + 1, i + 10);
                            k_left_double = parameterB;
                            b_left_double = parameterA;
                            left_point_double = i;
                            left_pointDouble_flag = 1;
                        }
                    }
                    if (R_black[i] == 0 && R_black[i + 1] != 0 && right_pointDouble_flag == 0)
                    {
                        SetText("右二重补线       " + i + "      " + (R_black[i] + R_black[i + 12] - 2 * R_black[i + 6]));
                        if (abs(R_black[i] + R_black[i + 12] - 2 * R_black[i + 6]) < 4)
                        {
                            regression(2, i + 1, i + 10);
                            k_right_double = parameterB;
                            b_right_double = parameterA;
                            right_point_double = i;
                            right_pointDouble_flag = 1;
                        }
                    }
                }
                if (right_pointDouble_flag == 1 || left_pointDouble_flag == 1)
                {
                    if (right_point_double> left_point_double)
                    {
                        for (int i = 0; i <= right_point_double; i++)
                        {
                            LCenter[i] = (byte)((L_black[i] + k_right_double * i + b_right_double) / 2);
                        }
                    }
                    else if (right_point_double < left_point_double)
                    {
                        for (int i = 0; i <= left_point_double; i++)
                        {
                            LCenter[i] = (byte)((k_left_double * i + b_left_double + R_black[i]) / 2);
                        }
                    }
                }
                /***********  弯道补线*******************/
                //if (flag_right_round_type == 2 || flag_right_round_type == 3 )
                //{
                //    for (int i = 0; i <= findLine; i++)
                //    {
                //        if (flag_right_round_type == 2 || flag_right_round_type == 3)
                //        {
                //            if (L_black[i] != 185)
                //                LCenter[i] = AvoidOverflow(L_black[i] - fixValue[i] / 2);
                //        }
                //        else if (flag_left_round_type == 2 || flag_left_round_type == 3)
                //        {
                //            if (R_black[i] != 0)
                //                LCenter[i] = AvoidOverflow(R_black[i] + fixValue[i] / 2);
                //        }

                //    }
                //}
                //else if (flag_right_round_point == 1)
                //{
                //    for (int i = 0; i <= findLine; i++)
                //    {
                //        if (flag_enter_right == 1)
                //        {
                //            if (L_black[i] != 185)
                //                LCenter[i] = AvoidOverflow(L_black[i] - fixValue[i] / 2);
                //        }
                //        else if (flag_enter_left == 1)
                //        {
                //            if (R_black[i] != 0)
                //                LCenter[i] = AvoidOverflow(R_black[i] + fixValue[i] / 2);
                //        }

                //    }
                //}
                regression(0, 0, 30);
                SetText("middle_k_30" + parameterB);
                SetText("dance"+ left_turn_down[0]+"   " +flag_right_round_type6);
                /*********************************右转状态转化*********************************/

                if (jishu_type1 > 1 && flag_right_round_type == 1 && right_round_point[0] <= 31 && right_round_point[0] >= 20 && rightFindFlag[15] == 1)
                {
                    flag_right_round_type = 2;
                    jishu_type1 = 0;
                }
                else if (jishu_type2 > 1 && flag_right_round_type == 2 && ((right_round_up[0] <= 38 && right_round_up[0] > 23)))
                {
                    flag_right_round_type = 3;
                    jishu_type2 = 0;
                }
                else if (jishu_type3 > 1 && flag_right_round_type == 3 && right_round_up[0] <=21 && right_round_up[0] > 10)
                {
                    flag_right_round_type = 4;
                    jishu_type3 = 0;
                }
                else if (jishu_type4 > 1 && flag_right_round_type == 4 && left_turn_down[0] < 35 && left_turn_down[0] > 20)
                {
                    flag_right_round_type = 5;
                    jishu_type4 = 0;
                }
                else if (jishu_type5 > 1 && flag_right_round_type == 5 && left_turn_down[0] <= 16 && left_turn_down[0] != 0)
                {
                    flag_right_round_type = 6;
                    flag_right_round_type6 = 0;
                    jishu_type5 = 0;
                }
                else if (jishu_type6 > 5 && flag_right_round_type == 6 && ((leftFindFlag[4] == 1 && leftFindFlag[30] == 1) || (float_abs(parameterB) <= 2.3f && (right_round_out_up_point >= 38 || right_round_out_up_point == 0))))
                {
                    flag_right_round_type = 7;
                    jishu_type6 = 0;
                }
                else if (jishu_type7 > 7 && flag_right_round_type == 7 && ((right_turn_up[0] <= 10 && right_turn_up[0] >= 3) || (rightFindFlag[10] == 1 && right_turn_up[0]<20)))
                {
                    flag_right_round_type = 0;
                    jishu_type7 = 0;
                }

                if (flag_right_round_type != 0)
                {
                    flag_right_round = 1;
                }
                else
                {
                    flag_right_round = 0;
                }
            }
            #endregion
            #region[坡道]
            if (flag_ramp == 1 && findLine<30)
            {
                for (j = 0; j < 30; j++)
                {
                    LCenter[j] = LCenter[0];
                }
            }
            #endregion












            //else if (flag_left_round_type == 5 || flag_left_round_type == 6)
            //{
            //    LeastSquareCalc_Curve(0, (byte)(round_overflow_point), 2);
            //    for (int j = 0; j < left_round_out_up_point; j++)
            //    {
            //        R_black[j] = (byte)(j * j * curve_a + curve_b);
            //        LCenter[j] = (byte)((L_black[j] + R_black[j]) / 2);

            //    }
            //}

            //else if (flag_right_round_type == 5 || flag_right_round_type == 6)
            //{
            //    LeastSquareCalc_Curve(0, (byte)(round_overflow_point), 1);
            //    for (int j = 0; j < right_round_out_up_point; j++)
            //    {
            //        L_black[j] = (byte)(float_abs(j * j * curve_a + curve_b));
            //        LCenter[j] = (byte)((L_black[j] + R_black[j]) / 2);

            //    }
            //}



            /*****环岛计数保护措施******/
            if (flag_left_round_type == 1 || flag_right_round_type == 1)
            {
                jishu_type1 += 1;
            }
            else if (flag_left_round_type == 2 || flag_right_round_type == 2)
            {
                jishu_type2 += 1;
            }
            else if (flag_left_round_type == 3 || flag_right_round_type == 3)
            {
                jishu_type3 += 1;
            }
            else if (flag_left_round_type == 4 || flag_right_round_type == 4)
            {
                jishu_type4 += 1;
            }
            else if (flag_left_round_type == 5 || flag_right_round_type == 5)
            {
                jishu_type5 += 1;
            }
            else if (flag_left_round_type == 6 || flag_right_round_type == 6)
            {
                jishu_type6 += 1;
            }
            else if (flag_left_round_type == 7 || flag_right_round_type == 7)
            {
                jishu_type7 += 1;
            }

            //if(jishu_type1==80|| jishu_type2 == 80 || jishu_type3 == 80 || jishu_type4 == 200 || jishu_type5 == 80 || jishu_type6 == 80 || jishu_type7 == 80)
            //{
            //    flag_left_round_type = 0;
            //    flag_right_round_type = 0;
            //}
            /***********  判断结束*******************/

            //
            middle_point = LCenter[2];
            SetText("中S条件" + abs(centre_right_point[0] - centre_left_point[0]) + "    " + abs(centre_right_point[1] - centre_left_point[1]) + "    " + cumulants2);
            SetText("小S条件" + flag_shizi_left + "    " + flag_shizi_right + "    " + abs(centre_right_point[0] - centre_left_point[0]) + "    " + abs(centre_right_point[1] - centre_left_point[1]) + "    " + cumulants2 + "    " + (centre_right_point[1] + centre_left_point[1]));
            SetText("短直道条件" + abs(cumulants2) + "   " + cumulants1 + "     " + cumulants5  + "     " + cumulants3 + "    " + (L_black[30] - R_black[30]) + "    " + (L_black[25] - R_black[25]) + "   " + findLine);
            SetText("长直道条件" + abs(cumulants2) + "   " + cumulants1 + "     " + cumulants4  + "    " + (L_black[40] - R_black[40]) + "    " + (L_black[35] - R_black[35]) + "   " + findLine);
            /***************************中S处理***************************/
            if (flag_middle_S_jishu == 2)
            {
                flag_middle_S = 1;
            }
            if (flag_middle_S == 1)
            {
                if (flag_small_S == 0 &&
                    ((abs(centre_right_point[0] - centre_left_point[0]) >= 10 && abs(centre_right_point[0] - centre_left_point[0]) <= 35
                    && abs(centre_right_point[1] - centre_left_point[1]) >= 30 && abs(centre_right_point[1] - centre_left_point[1]) <= 72) 
                    ||(centre_right_point[0] <= 46 && centre_right_point[0] >=20 && centre_right_point[0] != 0) 
                    ||(centre_left_point[0] <= 46 && centre_left_point[0] >=20 && centre_left_point[0] != 0))
                    && (findLine >= 43 && cumulants2 >= 5)
                    )
                {
                    flag_middle_S_out = 0;
                }
                else
                    flag_middle_S_out += 1;
            }
            if (flag_middle_S_out == 2)
            {
                flag_middle_S = 0;
            }
            /********短直道减速处理********/
            //if (abs((LCenter[39] + LCenter[38] + LCenter[40]) / 3 - LCenter[4]) > 30 && flag_enter_shortstr == 1 && abs(LCenter[30] - LCenter[29]) < 15 && abs(LCenter[31] - LCenter[30]) < 15)
            if (((left_turn_down_wandao[0] >= 15 && left_turn_down_wandao[0] <= 27 && right_turn_down_wandao[0] == 0 && left_turn_down_wandao[0] > 0)
                || (right_turn_down_wandao[0] >= 15 && right_turn_down_wandao[0] <= 27 && left_turn_down_wandao[0] == 0 && right_turn_down_wandao[0] > 0))
                && flag_enter_shortstr == 1 && abs(LCenter[30] - LCenter[29]) < 15 && abs(LCenter[31] - LCenter[30]) < 15)
            {
                flag_jiansu_shortstr_jishu += 1;
            }
            else
                flag_jiansu_shortstr_jishu = 0;
            
            /***************************短直道处理***************************/
            if (flag_enter_shortstr_jishu == 2 && flag_enter_changzhidao != 1)
            {
                flag_enter_shortstr = 1;
            }
            if (flag_enter_shortstr == 1 &&
                ((left_turn_down_wandao[0] <= 32 && left_turn_down_wandao[0] >= 17 && right_turn_down_wandao[0] == 0 && left_turn_down_wandao[0] > 0)
                || (right_turn_down_wandao[0] <= 32 && right_turn_down_wandao[0] >= 17 && left_turn_down_wandao[0] == 0 && right_turn_down_wandao[0] > 0)
                ))
            {
                flag_huaxing_short = 1;

            }
            else
                flag_huaxing_short = 0;      //滑行判断

            if (flag_jiansu_shortstr_jishu == 2)
            {
                flag_enter_shortstr = 0;
            }
            /********长直道减速处理********/     //双十字情况
            if (((left_turn_down_wandao[0] >= 15 && left_turn_down_wandao[0] <= 27 && right_turn_down_wandao[0] == 0)
                || (right_turn_down_wandao[0] >= 15 && right_turn_down_wandao[0] <= 27 && left_turn_down_wandao[0] == 0))
                && flag_enter_changzhidao == 1 && abs(LCenter[30] - LCenter[29]) < 15 && abs(LCenter[31] - LCenter[30]) < 15)
            {
                flag_jiansu_changzhidao_jishu += 1;
            }
            else
                flag_jiansu_changzhidao_jishu = 0;

            /***************************普通长直道处理***************************/
            if (flag_enter_changzhidao_jishu == 2 || flag_doube_shizi == 2)
            {
                flag_enter_changzhidao = 1;
                Long_str_jishu += 1;
            }
            if (flag_enter_changzhidao == 1 &&
                ((left_turn_down_wandao[0] < 35 && left_turn_down_wandao[0] >= 17 && right_turn_down_wandao[0] == 0 && Compare_abs((R_black[44] + R_black[20]) / 2, R_black[32]) == 0 && flag_shizi_right==0)
                || (right_turn_down_wandao[0] < 35 && right_turn_down_wandao[0] >= 17 && left_turn_down_wandao[0] == 0 && Compare_abs((L_black[44] + L_black[20]) / 2, L_black[32]) == 0 && flag_shizi_left==0)
                ))
            {
                flag_huaxing_long = 1;
            }
            else
                flag_huaxing_long = 0;      //滑行判断

            if (flag_jiansu_changzhidao_jishu == 2)
            {
                flag_enter_changzhidao = 0;     //不用复制
            }
            /***************************坡道处理***************************/
            SetText(abs(L_black[0] - R_black[0] - fixValue[0])+"    "+ abs(L_black[25] - R_black[25] - fixValue[25]));
            if (flag_ramp_jishu == 2)
            {
                flag_ramp = 1;
                flag_ramp_jishu = 0;
            }
            if (flag_ramp == 1)
            {
                flag_ramp_out_jishu += 1;
                if ((abs(L_black[0] - R_black[0] - fixValue[0]) <= 6 && abs(L_black[25] - R_black[25] - fixValue[25]) <= 6) || flag_ramp_out_jishu ==50)
                {
                    flag_ramp = 0;
                    flag_ramp_out_jishu = 0;
                }
            }
            if (abs(LCenter[25] - LCenter[5]) > 16)
            {
                flag_huaxing_long = 0;
                flag_huaxing_short = 0;
                flag_enter_changzhidao = 0;
                flag_enter_shortstr = 0;
            }
            SetText("弯道左拐点：" + left_turn_down_wandao[0] + "弯道右拐点：" + right_turn_down_wandao[0]);
            SetText("左二次十字坐标：" + second_left_turn_down[0] + "，" + second_left_turn_down[1]);
            SetText("右二次十字坐标：" + second_right_turn_down[0] + "，" + second_right_turn_down[1]);
            SetText("中线右拐点坐标：" + centre_right_point[0] + "，" + centre_right_point[1]);
            SetText("中线左拐点坐标：" + centre_left_point[0] + "，" + centre_left_point[1]);
            refindLine = findLine;
            left_turn_down[0] = 0;
            left_turn_down[1] = 185;
            right_turn_down[0] = 0;
            right_turn_down[1] = 0;
            left_turn_up[0] = 0;
            left_turn_up[1] = 0;
            right_turn_up[0] = 0;
            right_turn_up[1] = 0;
            left_turn_down_wandao[0] = 0;
            right_turn_down_wandao[0] = 0;
            second_left_turn_down[0] = 0;//数组里面没有第0行
            second_left_turn_down[1] = 0;
            second_right_turn_down[0] = 0;//数组里面没有第0行
            second_right_turn_down[1] = 0;
            centre_right_point[1] = 0;
            centre_right_point[0] = 0;
            centre_left_point[1] = 0;
            centre_left_point[0] = 0;

            left_round_point[0] = 0;
            left_round_point[1] = 0;
            left_round_up[0] = 0;
            left_round_up[1] = 0;
            right_round_point[0] = 0;
            right_round_point[1] = 0;
            right_round_up[0] = 0;
            right_round_up[1] = 0;
            #endregion
            /*******************道路类型判断（显示语句，不用复制给下位机）*******************/
            if (flag_shizi_jinru == 1)
                SetText("斜入十字扫线");
            if (flag_stopcar == 1)
            {
                SetText("*********准备停车啦");
            }
            if (startDirect == 2)
            {
                SetText("*********准备向右停车啦");
            }
            if (flag_ramp == 1)
            {
                SetText("*********进入坡道啦");
            }
            if (flag_enter_changzhidao == 1)
            {
                SetText("*********进入长直道啦");
            }
            if (flag_small_S == 1)
            {
                SetText("**********进入小S啦");
            }
            if (flag_enter_right == 1 && flag_small_S != 1)
            {
                SetText("**********右转补线");
            }
            if (flag_enter_left == 1 && flag_small_S != 1)
            {
                SetText("********** 左转补线");
            }
            if (flag_enter_shortstr == 1)
            {
                SetText("*********进入短直道啦");
            }
            if (flag_middle_S == 1)
            {
                SetText("*********进入中S啦");
            }
            if (flag_huaxing_short == 1)
            {
                SetText("………………%短直道给我滑行");
            }
            if (flag_huaxing_long == 1)
            {
                SetText("………………%长直道给我滑行");
            }
            if (flag_left_round_type == 1)
            {
                SetText("**********进入左环岛（一状态）");
            }
            else if (flag_left_round_type == 2)
            {
                SetText("**********进入左环岛（二状态）");
            }
            else if (flag_left_round_type == 3)
            {
                SetText("**********进入左环岛（三状态）");
            }
            else if (flag_left_round_type == 4)
            {
                SetText("**********进入左环岛（四状态）");
            }
            else if (flag_left_round_type == 5)
            {
                SetText("**********进入左环岛（五状态）");
            }
            else if (flag_left_round_type == 6)
            {
                SetText("**********进入左环岛（六状态）");
            }
            else if (flag_left_round_type == 7)
            {
                SetText("**********进入左环岛（七状态）");
            }
            if (flag_right_round_type == 1)
            {
                SetText("**********进入右环岛（一状态）");
            }
            else if (flag_right_round_type == 2)
            {
                SetText("**********进入右环岛（二状态）");
            }
            else if (flag_right_round_type == 3)
            {
                SetText("**********进入右环岛（三状态）");
            }
            else if (flag_right_round_type == 4)
            {
                SetText("**********进入右环岛（四状态）");
            }
            else if (flag_right_round_type == 5)
            {
                SetText("**********进入右环岛（五状态）");
            }
            else if (flag_right_round_type == 6)
            {
                SetText("**********进入右环岛（六状态）");
            }
            else if (flag_right_round_type == 7)
            {
                SetText("**********进入右环岛（七状态）");
            }

        }



        #endregion
        /***************变量初始化***************/
        void ImageInit()
        {
            //将图像赋给BinPixels
            for (y = 0; y < 70; y++)
            {
                for (x = 0; x < 186; x++)
                {
                    BinPixels[y][x] = Pixels[y, x];
                }
                //将横向搜索数组及lcr结构体清零
                L_black[y] = 185;
                R_black[y] = 0;
                L_Start[y] = 0;
                R_Start[y] = 0;
                LCenter[y] = 0;
                Lcr[y].REnd = 0;
                Lcr[y].RStart = 0;
                Lcr[y].LEnd = 0;
                Lcr[y].LStart = 0;
                Lcr[y].LBlack = 0;
                Lcr[y].RBlack = 0;
                Lcr[y].Center = 0;
            }
        }

        /***************图像场处理***************/


        /***************完整的一场数据处理***************/
        public void SignalProcess()
        {
            mf.cleartxt();
            ImageInit();//变量初始化 
            ImageProcess();//图像场处理
            for (y = 0; y < 70; y++)
            {
                Lcr[y].LStart = L_Start[y];
                Lcr[y].RStart = R_Start[y];
                Lcr[y].LBlack = L_black[y];
                Lcr[y].RBlack = R_black[y];
                Lcr[y].Center = LCenter[y];
            }
        }


        #region 系统获取P行的函数 用户不用关心
        void get_P_from_Pixels()
        {
            P = new byte[Image_Width];
            for (int i = 0; i < Image_Width - 1; i++)
            {
                P[i] = Pixels[y, i];
            }
        }

        void SetText(object value)
        {
            mf.settext(value);

            //if (MyNrf.Form1.VoiceString == "")
            //{
            //    MyNrf.Form1.VoiceString += value.ToString();
            //}
            //else
            //{
            //    MyNrf.Form1.VoiceString += "\r\n" + value.ToString();
            //}
        }

        public void PolyCal()
        {
            //int lcrcnt = CheckLcr();
            //double[] tmpx = new double[lcrcnt];
            //double[] tmpy = new double[lcrcnt];
            //for (int i = 0; i < lcrcnt; i++) 
            //{
            //    tmpx[i] = Lcr[i].Center;
            //    tmpy[i] = i;
            //}
            //double[] pfit = MyMath.Filter.polifit(tmpy, tmpx, MyNrf.Form1.Polifit);
            //for (int i = 0; i < lcrcnt; i++)
            //{
            //    Lcr[i].CenterBack = (byte)MyMath.Filter.MyPolyVal(pfit, tmpy[i]);
            //}
        }//系统最小二乘法
        private int CheckLcr()
        {
            int tmpcnt = 0, cnt = 0;
            for (int i = 0; i < Image_Height; i++)
            {
                if (Lcr[i].Center == 255)
                {
                    cnt++;
                }
                else
                {
                    cnt = 0;
                }
                if (cnt > 5)
                {
                    tmpcnt = i - cnt;
                    cnt = 100;
                    break;
                }
            }
            if (cnt != 100)
            {
                tmpcnt = 70 - cnt;
            }
            return tmpcnt;
        }//检查中线
        #endregion
    }
}
