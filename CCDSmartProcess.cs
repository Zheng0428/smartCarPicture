using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO.Ports;
using System.Drawing.Imaging;
using System.IO;
using System.Configuration;
using System.Threading;
using MyNrf.Properties;

namespace MyNrf.这里写仿真程序
{
    class CCDSmartProcess
    {
        #region 系统参数
        public int CCDWeight = 128;//
        public int CCDHeight = 1;//
        private byte[] P1;
        private byte[] P2;
        private byte[] Q;
        public byte[,] CCDData;//存储当前场所有像素的二维指针数组P_Pixels[0][1]
        public List<CCDPOS> Lcr = new List<CCDPOS>();
        public int Count;//记录当前场数
        #endregion
        #region 结构体
        public struct student//结构体申请实例
        {
            public int x;
            public student(int a)
            {
                this.x = a;
            }
        }
        public struct Obstacle
        {
            public int BlackValue;//黑线的平均值

            public int BlackValue_Jianju;//黑线的平均值,两个跳变点间距

            public int WhiteValue;//黑线的平均值

            public int Length;//黑线的长度

            public int Statusl;//直角还是障碍

            public int RStay_Count;

            public int RGet_OkFlag;

            public int RBlackCCD1_temp_sum;

            public int RBlackValue;

            public int RBlackValue_Temp;

            public int RWhiteCCD1_temp_sum;

            public int RWhiteValue;

            public int LStay_Count;

            public int LGet_OkFlag;

            public int LBlackCCD1_temp_sum;

            public int LBlackValue;

            public int LBlackValue_Temp;

            public int LWhiteCCD1_temp_sum;

            public int LWhiteValue;

            public int Length_GetFlag;
        };
        public struct HistoryLine
        {
            public int LPoint;
            public int RPoint;
            public int Center;
            public HistoryLine(int a, int b, int c)
            {
                this.LPoint = a;
                this.RPoint = b;
                this.Center = c;
            }
        }
        struct Flag_All
        {
            public int count;     //用于判断黑线

            public int count1;
            
            public int Count_BlackValue;  //用于黑线取值间断

            public int Flag_Black;  //判断过黑条

            public int Flag_Temp;  //辅助判断过黑条

            public int Flag_Temp1;  //辅助判断过黑条

            public int Flag_Temp_zuo; //判断障碍位置

            public int Flag_Temp_you; //判断障碍位置

            public int Flag_ZhangAi_zuo;//判断过障碍

            public int Flag_ZhangAi_you;//判断过障碍

            public int t;  //计数值，用于避障碍场数限制

            public int p;  //计数值，用于直角弯打角限制

            public int state;

            public int LeftWan_Black_state;

            public int RightWan_Black_state;

            public int Danxian_LeftWan_Black_state;

            public int Danxian_RightWan_Black_state;

            public int time;

            public int first;

            public int ZhiJiao_Pass_Flag_Left;//直角通过标记  用来传递增大角速度抑制

            public int ZhiJiao_Pass_Flag_Right;//直角通过标记  用来传递增大角速度抑制

            public int ShiZi_Flag;

            public int ShiZi_First_Count;

            public int ShiZi_Last_Count;

            public int Flag_Danxianzuo;

            public int Flag_Danxianyou;

            //public int Flag_Danxian;

            public int Flag_Danxian_Down;

            public int Flag_Danxian_Up;

            public int Flag_Shizhitime;

            public int Flag_ZhiJiao;

            public int Flag_ZhangAi;

            public int Flag_ZhiJiao_Jiansu;

            public int Flag_StartJude;

            public int Flag_PoDao_Up;

            public int Flag_PoDao_Down;

            public int Flag_Out_PoDaoUp;

            public int Flag_WanDao;


            public int Start_PoDao;

            public int Start_Danxian;

            public int Count_Sate_Keep;

            public int Help_ShiZhi;
        };
        public struct Length
        {
            public int Left_CCD1_temp_sum;//临时和
            public int LeftAverage;//左边平均值

            public int Right_CCD1_temp_sum;
            public int RightAverage;//右边平均值

            public int LLength;
            public int RLength;

            public int Race_Length;
            public int Race_Length_Temp1;
            public int Race_Length_Temp2;
            public int Race_Length_Sum;

        };//原来的try
        public struct Line
        {

            public int LPoint;//左凹点
            public int LStart;//开始位置
            public int LEnd;//结束位置

            public int RPoint;
            public int RStart;
            public int REnd;

            public int Center;
        };

        struct Feature
        {
            public int Left_P1;
            public int Left_P2;
            public int Center;
            public int Right_P1;
            public int Right_P2;

        };
        student[] my = new student[3] { new student(1), new student(1), new student(1) };//结构体申请实例
        HistoryLine[] History = new HistoryLine[10];//记录过去十场的信息
        Obstacle obstacle = new Obstacle();//黑线信息
        Flag_All myflag = new Flag_All();//标记位
        Length length = new Length();//记录左右线  原来的try
        Line[] myline = new Line[4];
        Feature[] feature = new Feature[5];
        #endregion
        #region 相当于define
        int COMMON = 0;

        int ZHANG_AI = 2;

        int ZHI_JIAO = 1;

        int DAN_XIAN = 3;

        int CCD1_LEFT_LIMIT = 3;//左边界

        int CCD1_RIGHT_LIMIT = 126;//右边界

        int CCD2_LEFT_LIMIT = 8;//左边界

        int CCD2_RIGHT_LIMIT = 120;//右边界

        int BLACK_CHANGE_RANGE = 12;
        #endregion
        #region 单片机中外部变量
        int YangShi = 8;

        int PianCha;

        int Flag_ZhiJiao = 0;

        int Flag_ZhangAi = 0;

        int Flag_ZhiJiao_Jiansu = 0;

        int ZhiJiao_ChangShu = 15;

        int Zhijiao_Change_Center = 4;

        int Zhangai_ChangShu = 30;

        int Zhangai_Change_Center = 24;

        int Center_Help = 56;

        int Range = 5;

        int BLACK_RANGE = 15;

        int IN_EFFECTIVE_RANGE = 4;

        int Danxian_Range = 15;

        int Center_Temp = 63;

        int DaJiao_Limited = 1;

        int nSpeed = 1300;

        int Flag_Special_Control = 2;

        int PoDao_Jianshu_ChangShu = 30;

        int[] SendPara = new int[10];
        #endregion
        #region 用户自定义参数


        //通用变量
        int i = 0;

        int Buxian_Value_zuo = 0;

        int Buxian_Value_you = 0;

        int Left_Lost_Count = 0;//在直角中记录一边丢线的次数

        int Right_Lost_Count = 0;//在直角中记录一边丢线的次数


        int ShiZi_LostCount = 0;

        int ShiZi_WaitCount = 0;

        int ShiZi_FirstCenter = 63;

        int ShiZi_LastCenter = 0;

        int Danxian_Scan_Range;


        int Danxian_Center = 63;

        int Danxian_StartCenter = 63;

        int Danxian_OutCount = 0;

        int Danxian_OutCenter = 63;

        int Count_Po_Dao_Down = 0;

        int Count_Po_Dao_Up = 0;

        int Out_PoDaoUp = 0;

        int Out_PoDaoDown = 0;


        int DanxianSpecial_Out = 0; // 1表示单线丢线之前为左线，，2右线

        int[] Lower_Rember = new int[5];

        int Start_Jude = 0;

        int Start_Jude_Count = 0;


        int ZhiJiao_Out_Count = 0;


        int Temp_ZhiJiao_ChangShu = 0;

        double Temp_Zhijiao_Change_Center = 0;


        int Zhangai_ChangShu_Temp;

        int Out_Zhangai;

        int White_Count = 0;

        int Flag_White = 0;

        int Out_White_Count = 0;

        int Forward_White = 0;

        int yuzhi;

        int yuzhi_last;



        int j;

        int Temp_PianCha;

        int Zhijiao_Lost_State_Zuo;

        int Zhijiao_Lost_State_You;

        int Zhijiao_Lost_Count_Zuo;

        int Zhijiao_Lost_Count_You;

        int JianJU;

        int First_Shizhi;

        int Down_Block_Change_Point = 0;

        //CCD1变量
        int[] CCD1_HistoryCenter = new int[10];//记录十场的中点值

        int[] CCD1_HistoryLpoint = new int[5];//历史左值

        int[] CCD1_HistoryRpoint = new int[5];//历史右值

        int[] CCD1_HistorySpeed = new int[10];//历史右值

        int[] CCD1_HistoryPointHeight = new int[128];//上一场的高度值

        int[] CCD1_HistoryPointHeight2 = new int[128];//上上一场场的高度值

        int[] CCD1_HistoryPointHeight3 = new int[128];//上上上一场的高度值

        int[] CCD1_HistoryPointHeight4 = new int[128];//上上上上一场的高度值

        int CCD1_CCD1_Center_Average2 = 0;

        int CCD1_CCD1_Center_Average3 = 0;

        int CCD1_CCD1_Center_Average4 = 0;

        int CCD1_Center_Min_Left = 0;

        int CCD1_Center_Min_Right = 0;

        int CCD1_Center_Min_Left_Last = 0;

        int CCD1_Center_Min_Right_Last = 0;

        int CCD1_temp_min;

        int CCD1_temp_max;

        int CCD1_LeftBool_Last = 0;
        int CCD1_RightBool_Last = 0;
        int CCD1_Rise_Left_Last = 0;
        int CCD1_Rise_Right_Last = 0;

        //CCD2变量
        int[] CCD2_HistoryCenter = new int[10];//记录十场的中点值

        int[] CCD2_HistoryLpoint = new int[5];//历史左值

        int[] CCD2_HistoryRpoint = new int[5];//历史右值

        int[] CCD2_HistorySpeed = new int[10];//历史右值

        int[] CCD2_HistoryPointHeight = new int[128];//上一场的高度值

        int[] CCD2_HistoryPointHeight2 = new int[128];//上上一场场的高度值

        int[] CCD2_HistoryPointHeight3 = new int[128];//上上上一场的高度值

        int[] CCD2_HistoryPointHeight4 = new int[128];//上上上上一场的高度值

        //对应下位机里的局部变量

        //CCD1_变量
        int CCD1_RightBool = 0;

        int CCD1_LeftBool = 0; //判断搜到下降跳变点

        int CCD1_Rise_Left_Point = 0;

        int CCD1_Down_Left_Point = 0;

        int CCD1_Rise_Right_Point = 0;

        int CCD1_Down_Right_Point = 0;


        int CCD1_Rise_Left_Point_Temp = 0;

        int CCD1_Down_Left_Point_Temp = 0;

        int CCD1_Rise_Right_Point_Temp = 0;

        int CCD1_Down_Right_Point_Temp = 0;


        int CCD1_LeftCount = 0;//左线黑点数

        int CCD1_LeftCount_Temp = 0;

        int CCD1_RightCount = 0;//右线黑点数

        int CCD1_RightCount_Temp = 0;

        int CCD1_Left_Point = 0; //左点

        int CCD1_Right_Point = 127;  //右点



        int CCD1_Special_Down_Point = 0;

        int CCD1_Special_Up_Point = 0;

        int CCD1_Special_Point = 0;


        int CCD1_Center_Max; //中间两边的最大值

        int CCD1_Center_Average = 0;  //中线附件平均值

        int CCD1_Center_Line = 63;  //参考中心线

        int CCD1_temp_sum = 0;  //求黑线平均值


        //CCD2_变量
        int CCD2_RightBool = 0;

        int CCD2_LeftBool = 0; //判断搜到下降跳变点

        int CCD2_Rise_Left_Point = 0;

        int CCD2_Down_Left_Point = 0;

        int CCD2_Rise_Right_Point = 0;

        int CCD2_Down_Right_Point = 0;


        int CCD2_Rise_Left_Point_Temp = 0;

        int CCD2_Down_Left_Point_Temp = 0;

        int CCD2_Rise_Right_Point_Temp = 0;

        int CCD2_Down_Right_Point_Temp = 0;


        int CCD2_LeftCount = 0;//左线黑点数

        int CCD2_LeftCount_Temp = 0;

        int CCD2_RightCount = 0;//右线黑点数

        int CCD2_RightCount_Temp = 0;

        int CCD2_Left_Point = 0; //左点

        int CCD2_Right_Point = 127;  //右点




        #endregion
        #region 用户函数
        int Abs(int value)
        {
            if (value < 0)
            {
                return -value;
            }
            else return value;


        }
        int Sv_OutLine_Center(int value)
        {
            if (value < 0)
            {
                return 0;
            }
            else if (value > 127)
            {
                return 127;
            }
            else
                return value;
        }
        byte Sv_OutLine(int Value)//保护越界
        {
            if (Value > CCD1_RIGHT_LIMIT)
            {
                Value = CCD1_RIGHT_LIMIT;
            }
            else if (Value < CCD1_LEFT_LIMIT)
            {
                Value = CCD1_LEFT_LIMIT;
            }

            return (byte)Value;

        }

        int Get_Average(int a, int b)
        {

            int j = 0, sum = 0;
            for (j = a; j < b; j++)
            {
                sum += P1[j];

            }
            return (sum / (b - a));
        }

        int Average_Range(int start, int Diret)
        {
            int sum = 0;
            int a;
            if (Diret == 0)
            {
                for (i = start; i < Sv_OutLine(start + 20); i++)
                {
                    sum += P1[i];
                }
                a = sum / (Sv_OutLine(start + 20) - start);
                sum = 0;
            }
            else if (Diret == 1)
            {
                for (i = start; i > Sv_OutLine(start - 20); i--)
                {
                    sum += P1[i];
                }
                a = sum / (start - Sv_OutLine(start - 20));
                sum = 0;
            }
            else
            {
                a = 0;
            }
            return a;
        }

        int max(int a, int b)
        {
            if (a > b)
                return a;
            else
                return b;
        }
        int min(int a, int b)
        {
            if (a < b)
                return a;
            else
                return b;
        }
        /***********计算最佳阈值*************/
        int Calculate_Range(int yuzhi_center)//处理123个数
        {

            float Calculate_sumA = 0, Calculate_sumB = 0;

            float Wa = 0, Wb = 0, Ma = 0, Mb = 0;

            float Calculate_Count = 0;

            int Final_Yuzhi = 0;

            float Between_Class_Variance = 0;

            float Between_Class_Variance_Max;

            for (i = yuzhi_center - 10, Between_Class_Variance_Max = 0; i <= yuzhi_center + 10; i++)
            {
                Calculate_sumA = 0;
                Calculate_sumB = 0;
                Calculate_Count = 0;
                Between_Class_Variance = 0;
                Wa = 0;
                Wb = 0;
                Ma = 0;
                Mb = 0;
                for (j = 4; j < 126; j++)
                {
                    if (P1[j] < i)
                    {
                        Calculate_sumA += P1[j];
                        Calculate_Count++;
                    }
                    else
                    {
                        Calculate_sumB += P1[j];
                    }
                }
                Wa = Calculate_Count / (122);
                Wb = 1 - Wa;
                Ma = Calculate_sumA / Calculate_Count;
                Mb = Calculate_sumB / (122 - Calculate_Count);
                Between_Class_Variance = Wa * Wb * (Ma - Mb) * (Ma - Mb);
                if (Between_Class_Variance > Between_Class_Variance_Max)
                {
                    Final_Yuzhi = i;

                    Between_Class_Variance_Max = Between_Class_Variance;
                }

            }
            return Final_Yuzhi;

        }

        //找右边凹块，用于判断弯入黑带
        int Found_Down_Block_you(int center)
        {

            int Rise_Right_Point = 0;

            int Rise_Left_Point = 0;

            int LeftCount = 0;

            if (center < 0 || center > 127)
            {
                return 0;
            }
            else
            {
                Rise_Right_Point = center;

                for (i = center, LeftCount = 0; i <= 126; i++)
                {
                    if (P1[Sv_OutLine(i + 4)] - P1[i] > Range)
                    {
                        LeftCount++;
                        if (LeftCount > 1)
                        {
                            Rise_Right_Point = i;

                            break;
                        }

                    }
                    else
                    {
                        LeftCount = 0;
                    }
                }
                for (i = Rise_Right_Point, LeftCount = 0; i > 2; i--)
                {
                    if (P1[Sv_OutLine(i - 4)] - P1[i] > Range)
                    {
                        LeftCount++;
                        if (LeftCount > 1)
                        {
                            Rise_Left_Point = i;

                            break;
                        }
                    }
                    else
                    {
                        LeftCount = 0;
                    }
                }
                if (Rise_Left_Point != 0 && Rise_Right_Point != 0
                        && (Rise_Right_Point - Rise_Left_Point > 5)
                        && Abs(P1[Rise_Right_Point] - P1[Rise_Left_Point]) < 10
                    )
                {
                    return Rise_Left_Point;
                }
                else
                {
                    return 0;
                }
            }
        }
        //找左边凹块，用于判断弯入黑带

        int Found_Down_Block_zuo(int center)
        {
            int Rise_Right_Point = 0;

            int Rise_Left_Point = 0;

            int LeftCount = 0;

            if (center < 0 || center > 127)
            {
                return 0;
            }
            else
            {
                Rise_Left_Point = center;

                for (i = center, LeftCount = 0; i >= 2; i--)
                {
                    if (P1[Sv_OutLine(i - 4)] - P1[i] > Range)
                    {
                        LeftCount++;
                        if (LeftCount > 1)
                        {
                            Rise_Left_Point = i;

                            break;
                        }

                    }
                    else
                    {
                        LeftCount = 0;
                    }
                }
                for (i = Rise_Left_Point, LeftCount = 0; i < 126; i++)
                {
                    if (P1[Sv_OutLine(i + 4)] - P1[i] > Range)
                    {
                        LeftCount++;
                        if (LeftCount > 1)
                        {
                            Rise_Right_Point = i;

                            break;
                        }
                    }
                    else
                    {
                        LeftCount = 0;
                    }
                }
                if (Rise_Left_Point != 0 && Rise_Right_Point != 0
                        && (Rise_Right_Point - Rise_Left_Point > 5)
                        && Abs(P1[Rise_Right_Point] - P1[Rise_Left_Point]) < 10
                    )
                {
                    return Rise_Right_Point;
                }
                else
                {
                    return 0;
                }
            }
        }
        #endregion

        void Image_Init()
        {
            for (int i = 0; i < Lcr.Count; i++)
            {
                Lcr[i].LCR_Clear();
            }

            /*初始化*/


            CCD1_RightBool = 0;

            CCD1_LeftBool = 0; //判断搜到下降跳变点

            CCD1_LeftCount = 0;//左线黑点数

            CCD1_RightCount = 0;//右线黑点数

            CCD1_Left_Point = 0; //左点

            CCD1_Right_Point = 127;  //右点


            CCD1_Rise_Left_Point = 0;

            CCD1_Rise_Right_Point = 0;

            CCD1_Down_Left_Point = 0;//临时值

            CCD1_Down_Right_Point = 0;//临时值

            CCD1_Rise_Left_Point_Temp = 0;

            CCD1_Down_Left_Point_Temp = 0;

            CCD1_Rise_Right_Point_Temp = 0;

            CCD1_Down_Right_Point_Temp = 0;



            CCD1_Special_Down_Point = 0;

            CCD1_Special_Up_Point = 0;

            CCD1_Special_Point = 0;

            CCD1_Center_Min_Left = 0;

            CCD1_Center_Min_Right = 0;

            CCD1_Center_Average = 0;  //中线附件平均值

            CCD1_Center_Line = 63;  //参考中心线

            CCD1_temp_sum = 0;  //求黑线平均值

           
            
            
            length.RLength = length.LLength = 0;//使用黑线长度的清零

            obstacle.Length = 0;      //黑线值清零

            obstacle.Statusl = COMMON;

            myflag.Flag_StartJude = 0;
        }

        void ImageProcess()//ccd图像处理 
        {
            #region//CCD1_图像处理

            #region//延时图像处理时间，避免刚开机时的跳变//

            if (myflag.time < 50)
            {
                myflag.time++;
                goto END;
            }

            #endregion

            #region//初始赋值
            if (myline[0].Center == 0 && myflag.first == 0)
            {
                myline[0].Center = 63;
                myline[0].LPoint = 25;
                myline[0].RPoint = 100;
                myflag.first = 1;
                myline[1].LStart = CCD1_Center_Line;
                myline[1].LEnd = 3;
                myline[1].RStart = CCD1_Center_Line;
                myline[1].REnd = 126;
                CCD1_temp_min = P1[4];
                for (i = 4; i < 126; i++)
                {
                    if (CCD1_temp_min > P1[i])
                    {
                        CCD1_temp_min = P1[i];
                    }
                }
                yuzhi_last = (CCD1_temp_min) + 20;
            }
            if (obstacle.LBlackValue != 0)
            {
                for (i = 63; i > 5; i--)
                {
                    if ((P1[i] - obstacle.LBlackValue) < 8)
                    {
                        length.LLength++;
                    }

                }
            }
            if (obstacle.RBlackValue != 0)
            {
                for (i = 63; i < 121; i++)
                {
                    if ((P1[i] - obstacle.RBlackValue) < 8)
                    {
                        length.RLength++;
                    }

                }
            }
            #endregion

            #region//记录平均值，用于辅助判断左右障碍//

            for (i = 5, CCD1_temp_sum = 0; i < 127; i++)
            {
                CCD1_temp_sum += P1[i];
            }
            CCD1_Center_Average = (int)(CCD1_temp_sum * 1.0 / 122);

            CCD1_temp_sum = 0;

            CCD1_Center_Min_Left = P1[5];
            for (i = 5; i <= 74; i++)//20 到63
            {
                if (CCD1_Center_Min_Left > P1[i])
                {
                    CCD1_Center_Min_Left = P1[i];
                }
            }

            CCD1_Center_Min_Right = P1[64];
            for (i = 53; i <= 126; i++)
            {
                if (CCD1_Center_Min_Right > P1[i])
                {
                    CCD1_Center_Min_Right = P1[i];
                }
            }

            //防止图像硬件出问题导致不正常，仿真可能会出现
            if (CCD1_Center_Min_Left < 30)
            {
                CCD1_Center_Min_Left = CCD1_Center_Min_Left_Last;
            }
            if (CCD1_Center_Min_Right < 30)
            {
                CCD1_Center_Min_Right = CCD1_Center_Min_Right_Last;
            }

            #endregion

            #region//修正起始搜索点//
            if (myline[1].LStart < CCD1_LEFT_LIMIT + 3)
            {
                myline[1].LStart = CCD1_LEFT_LIMIT + 10;
            }
            else if (myline[1].LStart > CCD1_RIGHT_LIMIT)
            {
                myline[1].LStart = CCD1_RIGHT_LIMIT;
            }
            if (myline[1].RStart > CCD1_RIGHT_LIMIT - 3)
            {
                myline[1].RStart = CCD1_RIGHT_LIMIT - 10;
            }
            else if (myline[1].RStart < CCD1_LEFT_LIMIT)
            {
                myline[1].RStart = CCD1_LEFT_LIMIT;
            }
            #endregion

            #region//下降沿跳变点提取//

            yuzhi = Calculate_Range(yuzhi_last);

            if (yuzhi_last != 0 && ((yuzhi - yuzhi_last) >= 8 || (yuzhi - yuzhi_last) <= -8))
            {
                yuzhi = yuzhi_last;
            }
            if (obstacle.BlackValue != 0 && yuzhi - obstacle.BlackValue > 20)
            {
                yuzhi = Calculate_Range(obstacle.BlackValue + 20);
            }

            setText用户自定义("yuzhi" + yuzhi + yuzhi_last);

            if (myflag.LeftWan_Black_state >= 1 && myflag.RightWan_Black_state >= 1)
            {
                JianJU = 5;
            }
            else if (myflag.state == 3 || myflag.state == 4)
            {
                JianJU = 5;
            }
            else
            {
                JianJU = 4;
            }
            //从中间往两边搜索第一个跳变点
            //往左边
            for (i = myline[1].LStart, CCD1_LeftCount = 0; i > myline[1].LEnd; i--)
            {
                if (P1[Sv_OutLine(i)] - P1[Sv_OutLine(i - JianJU)] > Range)
                {
                    CCD1_LeftCount++;
                    {
                        if (CCD1_LeftCount > 1)
                        {
                            CCD1_Left_Point = Sv_OutLine(i - JianJU);
                            //setText用户自定义("左下降沿" + CCD1_Left_Point);
                            break;
                        }
                    }
                }
                else
                {
                    CCD1_LeftCount = 0;

                }
            }
            for (j = CCD1_Left_Point, CCD1_LeftCount = 0, CCD1_LeftCount_Temp = 0; j > myline[1].LEnd; j--)
            {
                if (P1[j] < (P1[CCD1_Left_Point]) - 1)
                {
                    CCD1_Left_Point = (int)j;
                    CCD1_LeftCount = 0;
                    //CCD1_LeftCount_Temp = 0;
                    //setText用户自定义("最小值" + CCD1_Left_Point);
                }
                else if (P1[j] == P1[CCD1_Left_Point] || P1[j] == P1[CCD1_Left_Point] - 1)
                {
                    CCD1_LeftCount_Temp++;
                    CCD1_LeftCount = 0;
                    //setText用户自定义("CCD1_LeftCount_Temp" + CCD1_LeftCount_Temp);
                    if (CCD1_LeftCount_Temp > 1 && (P1[CCD1_Left_Point] - yuzhi) < -1)//&& (P1[CCD1_Left_Point]) < CCD1_Center_Average)
                    {
                        CCD1_LeftCount_Temp = 0;

                        CCD1_LeftBool = 1;
                        //setText用户自定义("左点1" + CCD1_Left_Point);
                        break;
                    }
                }
                else if (P1[j] > P1[CCD1_Left_Point])
                {
                    CCD1_LeftCount++;
                    CCD1_LeftCount_Temp = 0;
                    //setText用户自定义("CCD1_LeftCount" + CCD1_LeftCount);
                    if (CCD1_LeftCount > 0 && (P1[CCD1_Left_Point] - yuzhi) < -1)// && (P1[CCD1_Left_Point]) < CCD1_Center_Average)
                    {
                        {
                            CCD1_LeftBool = 1;
                            //setText用户自定义("左点2" + CCD1_Left_Point);
                            CCD1_LeftCount = 0;
                            break;
                        }
                    }

                }
                if (j == myline[1].LEnd + 1)
                {
                    CCD1_Left_Point = 0;
                    CCD1_LeftBool = 0;
                    CCD1_LeftCount = 0;
                    CCD1_LeftCount_Temp = 0;
                    //setText用户自定义("左点3" + CCD1_Left_Point);
                    break;
                }
            }
            //往右边
            for (i = myline[1].RStart, CCD1_RightCount = 0; i < myline[1].REnd; i++)
            {
                if (P1[i] - P1[Sv_OutLine(i + JianJU)] > Range)
                {
                    CCD1_RightCount++;
                    {
                        if (CCD1_RightCount > 1)
                        {
                            CCD1_Right_Point = Sv_OutLine(i + JianJU);
                            //setText用户自定义("右下降沿" + CCD1_Right_Point);

                            break;
                        }
                    }
                }
                else
                {
                    CCD1_RightCount = 0;

                }
            }
            for (j = CCD1_Right_Point, CCD1_RightCount = 0, CCD1_RightCount_Temp = 0; j < myline[1].REnd; j++)
            {
                if (P1[j] < (P1[CCD1_Right_Point]) - 1)
                {
                    CCD1_Right_Point = (int)j;
                    CCD1_RightCount = 0;
                    //setText用户自定义("最小值" + CCD1_Right_Point);

                    //CCD1_RightCount_Temp = 0;
                }
                else if (P1[j] == P1[CCD1_Right_Point] || P1[j] == P1[CCD1_Right_Point] - 1)
                {
                    CCD1_RightCount_Temp++;
                    CCD1_RightCount = 0;
                    //setText用户自定义("CCD1_RightCount_Temp" + CCD1_RightCount_Temp);
                    if (CCD1_RightCount_Temp > 1 && (P1[CCD1_Right_Point] - yuzhi) < -1)//&& (P1[CCD1_Right_Point]) < CCD1_Center_Average)
                    {
                        CCD1_RightCount_Temp = 0;
                        CCD1_RightBool = 1;
                        //setText用户自定义("右点1" + CCD1_Right_Point);
                        break;
                    }
                }
                else if (P1[j] > P1[CCD1_Right_Point])
                {

                    CCD1_RightCount++;
                    CCD1_RightCount_Temp = 0;
                    if (CCD1_RightCount > 0 && (P1[CCD1_Right_Point] - yuzhi) < -1)// && (P1[CCD1_Right_Point]) < CCD1_Center_Average)
                    {
                        {
                            CCD1_RightBool = 1;
                            //setText用户自定义("右点2" + CCD1_Right_Point);
                            CCD1_RightCount = 0;
                            break;
                        }
                    }

                }
                if (j == myline[1].REnd - 1)
                {
                    CCD1_Right_Point = 127;
                    CCD1_RightBool = 0;
                    CCD1_RightCount = 0;
                    CCD1_RightCount_Temp = 0;
                    //setText用户自定义("右点3" + CCD1_Right_Point);
                    break;
                }
            }

            #endregion

            #region//继续搜索，看是否出现更有可能的黑点
            if (CCD1_LeftBool == 1 && (P1[CCD1_Left_Point] - obstacle.LBlackValue) > BLACK_RANGE
                && (P1[Sv_OutLine(CCD1_Left_Point - 5)] - P1[CCD1_Left_Point]) < BLACK_RANGE
                && obstacle.LBlackValue !=0
                )
            {
                //setText用户自定义("二度搜索左");
                for (i = CCD1_Left_Point, CCD1_LeftCount = 0; i > myline[1].LEnd; i--)
                {
                    if (P1[i] - P1[Sv_OutLine(i - JianJU)] > Range)
                    {
                        CCD1_LeftCount++;
                        {
                            if (CCD1_LeftCount > 1)
                            {
                                CCD1_Left_Point = Sv_OutLine(i - JianJU);
                                //setText用户自定义("二度搜索左下降沿" + CCD1_Left_Point);
                                break;
                            }
                        }
                    }
                    else
                    {
                        CCD1_LeftCount = 0;

                    }
                }
                for (j = CCD1_Left_Point, CCD1_LeftCount = 0, CCD1_LeftCount_Temp = 0; j > myline[1].LEnd; j--)
                {
                    if (P1[j] < (P1[CCD1_Left_Point]) - 1)
                    {
                        CCD1_Left_Point = (int)j;
                        CCD1_LeftCount = 0;
                        //CCD1_LeftCount_Temp = 0;
                        //setText用户自定义("二度搜索最小值" + CCD1_Left_Point);
                    }
                    else if (P1[j] == P1[CCD1_Left_Point] || P1[j] == P1[CCD1_Left_Point] - 1)
                    {
                        CCD1_LeftCount_Temp++;
                        CCD1_LeftCount = 0;
                        //setText用户自定义("二度搜索CCD1_LeftCount_Temp" + CCD1_LeftCount_Temp);
                        if (CCD1_LeftCount_Temp > 1 && (P1[CCD1_Left_Point] - yuzhi) < -1)//&& (P1[CCD1_Left_Point]) < CCD1_Center_Average)
                        {
                            CCD1_LeftCount_Temp = 0;

                            CCD1_LeftBool = 1;
                            //setText用户自定义("二度搜索左点1" + CCD1_Left_Point);
                            break;
                        }
                    }
                    else if (P1[j] > P1[CCD1_Left_Point])
                    {
                        CCD1_LeftCount++;
                        CCD1_LeftCount_Temp = 0;
                        //setText用户自定义("二度搜索CCD1_LeftCount" + CCD1_LeftCount);
                        if (CCD1_LeftCount > 0 && (P1[CCD1_Left_Point] - yuzhi) < -1)// && (P1[CCD1_Left_Point]) < CCD1_Center_Average)
                        {
                            {
                                CCD1_LeftBool = 1;
                                //setText用户自定义("二度搜索左点2" + CCD1_Left_Point);
                                CCD1_LeftCount = 0;
                                break;
                            }
                        }

                    }
                    if (j == myline[1].LEnd + 1)
                    {
                        CCD1_Left_Point = 0;
                        CCD1_LeftBool = 0;
                        CCD1_LeftCount = 0;
                        CCD1_LeftCount_Temp = 0;
                        //setText用户自定义("二度搜索左点3" + CCD1_Left_Point);
                        break;
                    }
                }
            }
            if (CCD1_RightBool == 1 && (P1[CCD1_Right_Point] - obstacle.RBlackValue) > BLACK_RANGE 
                && (P1[Sv_OutLine(CCD1_Right_Point + 5)] - P1[CCD1_Right_Point]) < BLACK_RANGE
                && obstacle.RBlackValue!=0
                )
            {
                //setText用户自定义("二度搜索右");
                //往右边
                for (i = CCD1_Right_Point, CCD1_RightCount = 0; i < myline[1].REnd; i++)
                {
                    if (P1[i] - P1[Sv_OutLine(i + JianJU)] > Range)
                    {
                        CCD1_RightCount++;
                        {
                            if (CCD1_RightCount > 1)
                            {
                                CCD1_Right_Point = Sv_OutLine(i + JianJU);
                                //setText用户自定义("二度搜索右下降沿" + CCD1_Right_Point);

                                break;
                            }
                        }
                    }
                    else
                    {
                        CCD1_RightCount = 0;

                    }
                }
                for (j = CCD1_Right_Point, CCD1_RightCount = 0, CCD1_RightCount_Temp = 0; j < myline[1].REnd; j++)
                {
                    if (P1[j] < (P1[CCD1_Right_Point]) - 1)
                    {
                        CCD1_Right_Point = (int)j;
                        CCD1_RightCount = 0;
                        //setText用户自定义("二度搜索最小值" + CCD1_Right_Point);

                        //CCD1_RightCount_Temp = 0;
                    }
                    else if (P1[j] == P1[CCD1_Right_Point] || P1[j] == P1[CCD1_Right_Point] - 1)
                    {
                        CCD1_RightCount_Temp++;
                        CCD1_RightCount = 0;
                        if (CCD1_RightCount_Temp > 1 && (P1[CCD1_Right_Point] - yuzhi) < -1)//&& (P1[CCD1_Right_Point]) < CCD1_Center_Average)
                        {
                            CCD1_RightCount_Temp = 0;
                            CCD1_RightBool = 1;
                            //setText用户自定义("二度搜索右点1" + CCD1_Right_Point);
                            break;
                        }
                    }
                    else if (P1[j] > P1[CCD1_Right_Point])
                    {

                        CCD1_RightCount++;
                        CCD1_RightCount_Temp = 0;
                        if (CCD1_RightCount > 0 && (P1[CCD1_Right_Point] - yuzhi) < -1)// && (P1[CCD1_Right_Point]) < CCD1_Center_Average)
                        {
                            {
                                CCD1_RightBool = 1;
                                //setText用户自定义("二度搜索右点2" + CCD1_Right_Point);
                                CCD1_RightCount = 0;
                                break;
                            }
                        }

                    }
                    if (j == myline[1].REnd - 1)
                    {
                        CCD1_Right_Point = 127;
                        CCD1_RightBool = 0;
                        CCD1_RightCount = 0;
                        CCD1_RightCount_Temp = 0;
                        //setText用户自定义("二度搜索右点3" + CCD1_Right_Point);
                        break;
                    }
                }
            }
            #endregion

            setText用户自定义("length.LLength + length.RLength" + (length.LLength + length.RLength));

            #region//判断找到的点是否越界，为了减少误判//
            if ((CCD1_Left_Point < CCD1_LEFT_LIMIT)|| ((P1[CCD1_Left_Point]) > CCD1_Center_Average && myflag.state != 3 && myflag.state != 4 && myflag.LeftWan_Black_state == 0 && myflag.RightWan_Black_state == 0))
            {
                CCD1_Left_Point = 0;
            }
            if ((CCD1_Right_Point > CCD1_RIGHT_LIMIT)|| ((P1[CCD1_Right_Point]) > CCD1_Center_Average && myflag.state != 3 && myflag.state != 4 && myflag.LeftWan_Black_state == 0 && myflag.RightWan_Black_state == 0))
            {
                CCD1_Right_Point = 127;
            }
            if ((CCD1_Center_Min_Left - CCD1_Center_Min_Left_Last) > 15 || ((CCD1_Center_Min_Left - obstacle.LBlackValue) > 20 && obstacle.LBlackValue != 0) || (CCD1_Center_Min_Left - yuzhi) > 15)
            {
                CCD1_Left_Point = 0;
            }
            else if ((CCD1_Center_Min_Right - CCD1_Center_Min_Right_Last) > 15 || ((CCD1_Center_Min_Right - obstacle.RBlackValue) > 20 && obstacle.RBlackValue != 0) || (CCD1_Center_Min_Right - yuzhi) > 15)
            {
                CCD1_Right_Point = 127;
            }

            if (P1[CCD1_Left_Point] - obstacle.LBlackValue > 20 && obstacle.LBlackValue != 0)
            {
                CCD1_Left_Point = 0;
            }
            if (P1[CCD1_Right_Point] - obstacle.RBlackValue > 20 && obstacle.RBlackValue != 0)
            {
                CCD1_Right_Point = 127;
            }
            if ((length.LLength + length.RLength) == 0 && obstacle.LBlackValue != 0 && obstacle.RBlackValue != 0 && myflag.Flag_Danxianzuo == 0 && myflag.Flag_Danxianyou == 0)
            {
                CCD1_Left_Point = 0;
                CCD1_Right_Point = 127;
            }


            if (CCD1_RightBool == 0)
            {
                CCD1_Right_Point = 127;
            }
            if (CCD1_LeftBool == 0)
            {
                CCD1_Left_Point = 0;
            }
            if (CCD1_Right_Point == 127)
            {
                CCD1_RightBool = 0;
            }
            if (CCD1_Left_Point == 0)
            {
                CCD1_LeftBool = 0;
            }


            #endregion

            setText用户自定义("CCD1_Left_Point" + CCD1_Left_Point);
            setText用户自定义("CCD1_Right_Point" + CCD1_Right_Point);

            #region//防止刚开机进入单线//
            if (CCD1_Left_Point != 0 && CCD1_Right_Point != 127 && Start_Jude == 0)
            {

                Start_Jude_Count++;

                if (Start_Jude_Count > 5)
                {
                    Start_Jude = 1;
                    setText用户自定义("开始判断元素");
                    Start_Jude_Count = 0;
                }

            }
            #endregion

            #region//开始判断元素//
            if (Start_Jude == 1)
            {
                if (Start_Jude_Count < 100)
                {
                    Start_Jude_Count++;
                }
                #region//判断障碍
                //方法一
                if (myflag.LeftWan_Black_state <= 1 && myflag.RightWan_Black_state <= 1 && (myflag.state == 0 || myflag.state == 5) && myflag.Flag_Danxianzuo == 0 && myflag.Flag_Danxianyou == 0 && DanxianSpecial_Out == 0)
                {
                    if (CCD1_HistoryLpoint[0] != 0 && CCD1_LeftBool == 1)//判断左障碍
                    {
                        if (((CCD1_Left_Point - myline[0].LPoint > 15 && myline[0].LPoint != 0) || (CCD1_Left_Point - CCD1_HistoryLpoint[3] > 15 && CCD1_HistoryLpoint[3] != 0)))//排除干扰
                        {
                            if (Abs(CCD1_HistoryCenter[8] - 63) < 15
                                    || Abs(CCD1_HistoryCenter[7] - 63) < 15
                                    || Abs(CCD1_HistoryCenter[6] - 63) < 15
                                    || Abs(CCD1_HistoryCenter[5] - 63) < 15
                                    || Abs(CCD1_HistoryCenter[4] - 63) < 15
                                    || Abs(CCD1_HistoryCenter[3] - 63) < 15
                                    || Abs(CCD1_HistoryCenter[2] - 63) < 15
                                    || Abs(CCD1_HistoryCenter[1] - 63) < 15
                                    || Abs(CCD1_HistoryCenter[0] - 63) < 15
                            )
                            {
                                if (Found_Down_Block_zuo(CCD1_Left_Point - 5) == 1)
                                {
                                    if (length.LLength > 10)
                                    {
                                        if (Abs(P1[Sv_OutLine(CCD1_Left_Point - 5)] - P1[CCD1_Left_Point]) < 10)
                                        {
                                            for (i = CCD1_Left_Point, CCD1_LeftCount = 0; i > CCD1_LEFT_LIMIT; i--)
                                            {
                                                if (P1[Sv_OutLine(i - 3)] - P1[i] > Range)
                                                {
                                                    CCD1_LeftCount++;
                                                    if (CCD1_LeftCount > 2)
                                                    {
                                                        CCD1_Rise_Left_Point_Temp = i;
                                                        break;
                                                    }
                                                }
                                                else
                                                {
                                                    CCD1_LeftCount = 0;
                                                }
                                            }
                                            if (CCD1_Left_Point - CCD1_Rise_Left_Point_Temp > 5 && CCD1_Rise_Left_Point_Temp != 0)
                                            {
                                                obstacle.Statusl = ZHANG_AI;

                                                myflag.Flag_ZhangAi_zuo = 1;

                                                myflag.Flag_ZhangAi_you = 0;
                                                setText用户自定义("方法一:左障碍");

                                                /*清除直角，单线，十字判断*/
                                                if (myflag.state != 5)
                                                {
                                                    myflag.state = 0;//
                                                }

                                                myflag.Flag_Temp_zuo = 0;

                                                myflag.t = 0;

                                                Buxian_Value_zuo = 0;

                                                Buxian_Value_you = 0;

                                                //GPIO_SetBits(PTA, GPIO_Pin_24);

                                                myflag.Flag_Temp_you = 0;

                                                myflag.Flag_Danxianzuo = 0;

                                                myflag.Flag_Danxianyou = 0;

                                                myflag.ShiZi_Flag = 1;
                                                /******************/

                                                //                                goto Over;
                                            }
                                        }
                                    }
                                }

                            }
                        }
                    }
                    if (CCD1_HistoryRpoint[0] != 0 && CCD1_RightBool == 1) //判断右障碍
                    {
                        if (((CCD1_Right_Point - myline[0].RPoint < -15 && myline[0].RPoint != 127) || (CCD1_Right_Point - CCD1_HistoryRpoint[3] < -15 && CCD1_HistoryRpoint[3] != 127)))
                        {
                            if (Abs(CCD1_HistoryCenter[8] - 63) < 15
                                || Abs(CCD1_HistoryCenter[7] - 63) < 15
                                || Abs(CCD1_HistoryCenter[6] - 63) < 15
                                || Abs(CCD1_HistoryCenter[5] - 63) < 15
                                || Abs(CCD1_HistoryCenter[4] - 63) < 15
                                || Abs(CCD1_HistoryCenter[3] - 63) < 15
                                || Abs(CCD1_HistoryCenter[2] - 63) < 15
                                || Abs(CCD1_HistoryCenter[1] - 63) < 15
                                || Abs(CCD1_HistoryCenter[0] - 63) < 15
                            )
                            {
                                if (Found_Down_Block_you(CCD1_Right_Point + 5) == 1)
                                {
                                    if (length.RLength > 10)
                                    {
                                        if (Abs(P1[Sv_OutLine(CCD1_Right_Point + 5)] - P1[CCD1_Right_Point]) < 10)//右平均值
                                        {
                                            for (i = CCD1_Right_Point, CCD1_RightCount = 0; i < CCD1_RIGHT_LIMIT; i++)
                                            {
                                                if (P1[Sv_OutLine(i + 4)] - P1[i] > Range)
                                                {
                                                    CCD1_RightCount++;
                                                    if (CCD1_RightCount > 2)
                                                    {
                                                        CCD1_Rise_Right_Point_Temp = i;
                                                        break;
                                                    }
                                                }
                                                else
                                                {
                                                    CCD1_RightCount = 0;
                                                }
                                            }
                                            if (CCD1_Rise_Right_Point_Temp - CCD1_Right_Point > 5 && CCD1_Rise_Right_Point_Temp != 0)
                                            {
                                                obstacle.Statusl = ZHANG_AI;

                                                myflag.Flag_ZhangAi_you = 1;

                                                setText用户自定义("方法一:右障碍");
                                                myflag.t = 0;

                                                myflag.Flag_ZhangAi_zuo = 0;

                                                //GPIO_SetBits(PTA, GPIO_Pin_24);

                                                /*清除直角，单线，十字判断*/
                                                if (myflag.state != 5)
                                                {
                                                    myflag.state = 0;//
                                                }
                                                Buxian_Value_zuo = 0;

                                                Buxian_Value_you = 0;

                                                myflag.Flag_Temp_zuo = 0;

                                                myflag.Flag_Temp_you = 0;

                                                myflag.Flag_Danxianzuo = 0;

                                                myflag.Flag_Danxianyou = 0;

                                                myflag.ShiZi_Flag = 1;
                                                /******************/
                                                //                                goto Over;
                                            }
                                        }
                                    }
                                }

                            }
                        }
                    }
                }
                #endregion

                #region//判断单线
                //判断左单线
                if ((myflag.Flag_Danxianzuo == 0
                      && myflag.Flag_Danxianyou == 0
                      && myflag.Flag_ZhangAi_zuo == 0
                      && myflag.Flag_ZhangAi_you == 0
                      && (myflag.state == 0 || myflag.state == 1)
                      && DanxianSpecial_Out == 0
                      && myflag.LeftWan_Black_state <= 1
                      && myflag.RightWan_Black_state <= 1
                      && (Flag_White == 0)
                      && myflag.Flag_Temp_zuo == 0
                      && myflag.Flag_Temp_you == 0
                      && myflag.Start_PoDao != 1
                      && myflag.Flag_ZhangAi == 0
                ) ||
                    (myflag.Flag_Danxianzuo == 0
                      && myflag.Flag_Danxianyou == 0
                      && myflag.Flag_ZhangAi_zuo == 0
                      && myflag.Flag_ZhangAi_you == 0
                      && (myflag.state == 5)
                      && DanxianSpecial_Out == 0
                      && myflag.LeftWan_Black_state <= 1
                      && myflag.RightWan_Black_state <= 1
                    //              && (Flag_White == 0)        
                      && myflag.Flag_Temp_zuo == 0
                      && myflag.Flag_Temp_you == 0
                      && myflag.Start_PoDao != 1
                      && myflag.Flag_ZhangAi == 0
                    )
                )
                {
                    #region//单线
                    if (
                           CCD1_Left_Point - myline[0].LPoint > 5
                        )
                    {
                        if (CCD1_Left_Point - myline[0].LPoint > Danxian_Range)
                        {
                            for (i = 9; i > 6; i--)
                            {
                                if (CCD1_HistoryCenter[i] != 63)
                                {

                                    break;
                                }
                            }
                            Danxian_StartCenter = (CCD1_HistoryCenter[i] + CCD1_HistoryCenter[i - 1]) / 2;
                            myflag.Flag_StartJude = 1;
                        }
                        else if (
                                   CCD1_HistoryRpoint[3] <= 95
                                && CCD1_HistoryRpoint[4] <= 95
                                && CCD1_HistoryLpoint[3] == 0
                                && CCD1_HistoryLpoint[4] == 0
                                && (myline[0].Center - CCD1_Left_Point) < 30
                                && (myline[0].Center - CCD1_Left_Point) > 0
                               )
                        {
                            Danxian_StartCenter = CCD1_Left_Point;
                            if (Flag_Special_Control == 1 || Flag_Special_Control == 2)
                            {
                                myflag.Flag_StartJude = 1;
                            }
                        }
                        if (myflag.Flag_StartJude == 1)
                        {                          
                            CCD1_Down_Left_Point = Sv_OutLine(Danxian_StartCenter - 30);

                            if (Danxian_StartCenter <= 12) //因为右边图像限制较靠边，所以右边不需要
                            {
                                for (i = 2, CCD1_LeftCount = 0; i < Sv_OutLine(Danxian_StartCenter + 30); i++)
                                {
                                    if (P1[i] - P1[Sv_OutLine(i + 4)] > Range)
                                    {
                                        CCD1_LeftCount++;
                                        if (CCD1_LeftCount > 1)// && (P1[Sv_OutLine(i + 4)] - yuzhi) < 0)
                                        {
                                            myflag.Flag_Danxian_Down = 1;

                                            CCD1_Down_Left_Point = Sv_OutLine(i + 4);

                                            break;
                                        }
                                        //                                else if(CCD1_LeftCount > 2 && (P1[Sv_OutLine(i + 4)] - yuzhi) >= 0)
                                        //                                {
                                        //                                    CCD1_LeftCount =0 ;
                                        //                                }
                                    }

                                    else
                                    {
                                        CCD1_LeftCount = 0;
                                        CCD1_Down_Left_Point = Sv_OutLine(Danxian_StartCenter - 30);
                                        myflag.Flag_Danxian_Down = 0;
                                    }

                                }
                            }
                            else
                            {
                                for (i = Sv_OutLine(Danxian_StartCenter - 30), CCD1_LeftCount = 0; i < Sv_OutLine(Danxian_StartCenter + 30); i++)
                                {
                                    if (P1[i] - P1[Sv_OutLine(i + 4)] > Range)
                                    {
                                        CCD1_LeftCount++;
                                        if (CCD1_LeftCount > 2)//&& (P1[Sv_OutLine(i + 4)] - yuzhi) < 0)
                                        {
                                            myflag.Flag_Danxian_Down = 1;

                                            CCD1_Down_Left_Point = Sv_OutLine(i + 4);

                                            break;
                                        }
                                        //                                else if(CCD1_LeftCount > 2 && (P1[Sv_OutLine(i + 4)] - yuzhi) >= 0)
                                        //                                {
                                        //                                    CCD1_LeftCount =0 ;
                                        //                                }
                                    }

                                    else
                                    {
                                        CCD1_LeftCount = 0;
                                        CCD1_Down_Left_Point = Sv_OutLine(Danxian_StartCenter - 30);
                                        myflag.Flag_Danxian_Down = 0;
                                    }

                                }
                            }
                            if (CCD1_Down_Left_Point >= 115)
                            {
                                for (i = CCD1_Down_Left_Point, CCD1_LeftCount = 0; i < max(Sv_OutLine(CCD1_Down_Left_Point + 10), Sv_OutLine(Danxian_StartCenter + 30)); i++)
                                {
                                    if (P1[Sv_OutLine(i + 1)] - P1[i] > 0)
                                    {
                                        CCD1_LeftCount++;
                                        if (CCD1_LeftCount > 1 && (P1[Sv_OutLine(i)] - CCD1_Center_Average) < 0)
                                        {
                                            myflag.Flag_Danxian_Up = 1;

                                            CCD1_Rise_Left_Point = i;

                                            break;
                                        }
                                        //                            else if(CCD1_LeftCount > 2 && (P1[Sv_OutLine(i + 4)] - yuzhi) >= 0)
                                        //                            {
                                        //                                CCD1_LeftCount =0 ;
                                        //                            }
                                    }

                                    else
                                    {
                                        CCD1_LeftCount = 0;
                                        CCD1_Rise_Left_Point = Sv_OutLine(Danxian_StartCenter - 30);
                                        myflag.Flag_Danxian_Up = 0;
                                    }

                                }
                            }
                            else
                            {
                                for (i = CCD1_Down_Left_Point, CCD1_LeftCount = 0; i < max(Sv_OutLine(CCD1_Down_Left_Point + 10), Sv_OutLine(Danxian_StartCenter + 30)); i++)
                                {
                                    if (P1[Sv_OutLine(i + 4)] - P1[i] > Range)
                                    {
                                        CCD1_LeftCount++;
                                        if (CCD1_LeftCount > 2)//&& (P1[Sv_OutLine(i)] - yuzhi) < 0)
                                        {
                                            myflag.Flag_Danxian_Up = 1;

                                            CCD1_Rise_Left_Point = i;

                                            break;
                                        }
                                        //                            else if(CCD1_LeftCount > 2 && (P1[Sv_OutLine(i + 4)] - yuzhi) >= 0)
                                        //                            {
                                        //                                CCD1_LeftCount =0 ;
                                        //                            }
                                    }

                                    else
                                    {
                                        CCD1_LeftCount = 0;
                                        CCD1_Rise_Left_Point = Sv_OutLine(Danxian_StartCenter - 30);
                                        myflag.Flag_Danxian_Up = 0;
                                    }

                                }
                            }

                            if (myflag.Flag_Danxian_Down == 1 && myflag.Flag_Danxian_Up == 1 && Abs(CCD1_Down_Left_Point - CCD1_Rise_Left_Point) <= 10)
                            {
                                myflag.Flag_Danxianzuo = 1;

                                setText用户自定义("变为单线，搜为左线");

                                //                //GPIO_SetBits(PTA,GPIO_Pin_24);  

                                //                    CCD1_Left_Point = Sv_OutLine(Danxian_StartCenter - 30);
                                //                    
                                //                    for (i = Sv_OutLine(Danxian_StartCenter - 30); i < Sv_OutLine(Danxian_StartCenter + 30);i++ )
                                //                    {
                                //                        if (P1[CCD1_Left_Point] > P1[i])
                                //                        {
                                //                            CCD1_Left_Point = i;
                                //                        }
                                //                    } 
                                //                CCD1_Right_Point=127;

                                myflag.Flag_Danxian_Down = 0;

                                myflag.Flag_Danxian_Up = 0;

                                Danxian_Center = CCD1_Left_Point;

                                goto Over;
                            }
                            else if (myflag.Flag_Danxian_Down == 1 && myflag.Flag_Danxian_Up == 0 && CCD1_Left_Point <= 17)
                            {
                                CCD1_Down_Left_Point_Temp = CCD1_Down_Left_Point;
                                for (i = CCD1_Down_Left_Point, CCD1_LeftCount = 0; i < max(Sv_OutLine(CCD1_Down_Left_Point + 10), Sv_OutLine(Danxian_StartCenter + 30)); i++)
                                {
                                    if (P1[i] < (P1[CCD1_Down_Left_Point]))
                                    {
                                        CCD1_Down_Left_Point = (int)i;
                                        CCD1_LeftCount = 0;
                                    }
                                    else if ((P1[Sv_OutLine(i + 4)] - P1[i] > Range))
                                    {
                                        CCD1_LeftCount++;
                                        if (CCD1_LeftCount > 2 && Abs(CCD1_Down_Left_Point - CCD1_Down_Left_Point_Temp) <= 10)
                                        {
                                            //                                CCD1_Left_Point = CCD1_Down_Left_Point;

                                            //                            //GPIO_SetBits(PTA,GPIO_Pin_24);  


                                            setText用户自定义("变为单线，搜为左线");

                                            myflag.Flag_Danxian_Up = 1;

                                            myflag.Flag_Danxianzuo = 1;

                                            Danxian_Center = CCD1_Left_Point;

                                            //   CCD1_Right_Point=127;
                                            goto Over;
                                        }
                                        else if (CCD1_LeftCount > 2 && Abs(CCD1_Down_Left_Point - CCD1_Down_Left_Point_Temp) > 10)
                                        {
                                            CCD1_LeftCount = 0;
                                        }
                                    }
                                    else
                                    {
                                        CCD1_LeftCount = 0;
                                    }
                                    if (i == max(Sv_OutLine(CCD1_Down_Left_Point + 10), Sv_OutLine(Danxian_StartCenter + 30)) - 1)
                                    {
                                        myflag.Flag_Danxian_Down = 0;

                                        myflag.Flag_Danxian_Up = 0;

                                        myflag.Flag_Danxianzuo = 0;

                                        CCD1_Rise_Right_Point = Sv_OutLine(Danxian_StartCenter - 30);

                                        CCD1_Down_Right_Point = Sv_OutLine(Danxian_StartCenter - 30);

                                        break;
                                    }
                                }
                            }
                            else if (myflag.Flag_Danxian_Down == 0 && myflag.Flag_Danxian_Up == 1 && CCD1_Left_Point <= 17)
                            {
                                CCD1_Rise_Left_Point_Temp = CCD1_Rise_Left_Point;

                                for (i = CCD1_Rise_Left_Point, CCD1_RightCount = 0; i > min(Sv_OutLine(CCD1_Rise_Left_Point - 10), Sv_OutLine(Danxian_StartCenter - 30)); i--)
                                {
                                    if (P1[i] < (P1[CCD1_Rise_Left_Point]))
                                    {
                                        CCD1_Rise_Left_Point = (int)i;

                                        CCD1_RightCount = 0;
                                    }
                                    else if (P1[Sv_OutLine(i - 4)] - P1[i] > Range)
                                    {
                                        CCD1_RightCount++;

                                        if (CCD1_RightCount > 2 && Abs(CCD1_Rise_Left_Point - CCD1_Rise_Left_Point_Temp) <= 10)
                                        {
                                            myflag.Flag_Danxian_Down = 1;
                                            //                                 CCD1_Left_Point = CCD1_Rise_Left_Point;

                                            //                            //GPIO_SetBits(PTA,GPIO_Pin_24);  


                                            setText用户自定义("变为单线，搜为左线");

                                            myflag.Flag_Danxian_Up = 1;

                                            myflag.Flag_Danxianzuo = 1;

                                            Danxian_Center = CCD1_Left_Point;

                                            //   CCD1_Right_Point=127;
                                            goto Over;

                                        }

                                        else if (CCD1_RightCount > 2 && Abs(CCD1_Rise_Left_Point - CCD1_Rise_Left_Point_Temp) > 10)
                                        {
                                            CCD1_RightCount = 0;
                                        }
                                    }
                                    else
                                    {
                                        CCD1_RightCount = 0;
                                    }
                                    if (i == min(Sv_OutLine(CCD1_Rise_Left_Point - 10), Sv_OutLine(Danxian_StartCenter - 30)) + 1)
                                    {
                                        myflag.Flag_Danxian_Up = 0;

                                        myflag.Flag_Danxian_Down = 0;

                                        break;
                                    }
                                }
                            }
                            else
                            {
                                myflag.Flag_Danxian_Down = 0;

                                myflag.Flag_Danxian_Up = 0;

                                myflag.Flag_Danxianzuo = 0;

                                CCD1_Down_Left_Point = Sv_OutLine(Danxian_StartCenter - 30);

                                CCD1_Rise_Left_Point = Sv_OutLine(Danxian_StartCenter - 30);
                            }
                        }
                    }
                    #endregion

                }
                //判断右单线

                if ((myflag.Flag_Danxianzuo == 0
                        && myflag.Flag_Danxianyou == 0
                        && myflag.Flag_ZhangAi_zuo == 0
                        && myflag.Flag_ZhangAi_you == 0
                        && (myflag.state == 0 || myflag.state == 1)
                        && DanxianSpecial_Out == 0
                        && myflag.LeftWan_Black_state <= 1
                        && myflag.RightWan_Black_state <= 1
                        && (Flag_White == 0)
                        && myflag.Flag_Temp_zuo == 0
                        && myflag.Flag_Temp_you == 0
                        && myflag.Start_PoDao != 1
                        && myflag.Flag_ZhangAi == 0

                    ) || (

                        myflag.Flag_Danxianzuo == 0
                        && myflag.Flag_Danxianyou == 0
                        && myflag.Flag_ZhangAi_zuo == 0
                        && myflag.Flag_ZhangAi_you == 0
                        && (myflag.state == 5)
                        && DanxianSpecial_Out == 0
                        && myflag.LeftWan_Black_state <= 1
                        && myflag.RightWan_Black_state <= 1
                    //                && (Flag_White == 0 )    
                        && myflag.Flag_Temp_zuo == 0
                        && myflag.Flag_Temp_you == 0
                        && myflag.Start_PoDao != 1
                        && myflag.Flag_ZhangAi == 0
                       )
                )
                {
                    #region//单线
                    if (CCD1_Right_Point - myline[0].RPoint <= -3)
                    {
                        if (CCD1_Right_Point - myline[0].RPoint < -Danxian_Range)
                        {
                            for (i = 9; i > 6; i--)
                            {
                                if (CCD1_HistoryCenter[i] != 63)
                                {

                                    break;
                                }
                            }
                            Danxian_StartCenter = (CCD1_HistoryCenter[i] + CCD1_HistoryCenter[i - 1]) / 2;
                            myflag.Flag_StartJude = 1;
                        }
                        else if (CCD1_HistoryLpoint[3] >= 35
                                && CCD1_HistoryLpoint[4] >= 35
                                && CCD1_HistoryRpoint[3] == 127
                                && CCD1_HistoryRpoint[4] == 127
                                && (CCD1_Right_Point - myline[0].Center) < 30
                                && (CCD1_Right_Point - myline[0].Center) > 0

                               )
                        {

                            Danxian_StartCenter = CCD1_Right_Point;
                            if (Flag_Special_Control == 1 || Flag_Special_Control == 2)
                            {
                                myflag.Flag_StartJude = 1;
                            }
                        }

                        if (myflag.Flag_StartJude == 1)
                        {
                            CCD1_Down_Right_Point = Sv_OutLine(Danxian_StartCenter - 30);

                            if (Danxian_StartCenter <= 12)
                            {
                                for (i = 2, CCD1_RightCount = 0; i < Sv_OutLine(Danxian_StartCenter + 30); i++)
                                {
                                    if (P1[i] - P1[Sv_OutLine(i + 4)] > Range)
                                    {
                                        CCD1_RightCount++;
                                        if (CCD1_RightCount > 1)//&& (P1[Sv_OutLine(i + 4)] - yuzhi) < 0)
                                        {

                                            myflag.Flag_Danxian_Down = 1;

                                            CCD1_Down_Right_Point = Sv_OutLine(i + 4);
                                            //setText用户自定义("sssssssssssssssssssssssssssssssssss" + CCD1_Down_Right_Point);
                                            //                                            setText用户自定义("下降沿");
                                            break;
                                        }
                                        //                            else if(CCD1_RightCount > 2 && (P1[Sv_OutLine(i + 4)] - yuzhi) >= 0)
                                        //                            {
                                        //                                CCD1_RightCount = 0;
                                        //                            }
                                    }

                                    else
                                    {
                                        CCD1_RightCount = 0;
                                        CCD1_Down_Right_Point = Sv_OutLine(Danxian_StartCenter - 30);
                                        myflag.Flag_Danxian_Down = 0;
                                    }

                                }
                            }
                            else
                            {
                                for (i = Sv_OutLine(Danxian_StartCenter - 30), CCD1_RightCount = 0; i < Sv_OutLine(Danxian_StartCenter + 30); i++)
                                {
                                    if (P1[i] - P1[Sv_OutLine(i + 4)] > Range)
                                    {
                                        CCD1_RightCount++;
                                        if (CCD1_RightCount > 2)//&& (P1[Sv_OutLine(i + 4)] - yuzhi) < 0)
                                        {
                                            myflag.Flag_Danxian_Down = 1;

                                            CCD1_Down_Right_Point = Sv_OutLine(i + 4);
                                            //                                            setText用户自定义("下降沿");
                                            break;
                                        }
                                        //                            else if(CCD1_RightCount > 2 && (P1[Sv_OutLine(i + 4)] - yuzhi) >= 0)
                                        //                            {
                                        //                                CCD1_RightCount = 0;
                                        //                            }
                                    }

                                    else
                                    {
                                        CCD1_RightCount = 0;
                                        CCD1_Down_Right_Point = Sv_OutLine(Danxian_StartCenter - 30);
                                        myflag.Flag_Danxian_Down = 0;
                                    }

                                }
                            }
                            if (CCD1_Down_Right_Point >= 115)
                            {
                                for (i = CCD1_Down_Right_Point - 10, CCD1_RightCount = 0; i < 127; i++)
                                {
                                    if (P1[Sv_OutLine(i + 1)] - P1[i] > 0)
                                    {
                                        CCD1_RightCount++;
                                        if (CCD1_RightCount > 1 && (P1[Sv_OutLine(i)] - CCD1_Center_Average) < 0)
                                        {
                                            myflag.Flag_Danxian_Up = 1;

                                            CCD1_Rise_Right_Point = i;
                                            //                                            setText用户自定义("上升沿");
                                            break;
                                        }
                                        //                            else if(CCD1_RightCount > 2 && (P1[Sv_OutLine(i)] - yuzhi) >= 0)
                                        //                            {
                                        //                                CCD1_RightCount = 0;
                                        //                            }
                                    }

                                    else
                                    {
                                        CCD1_RightCount = 0;
                                        CCD1_Rise_Right_Point = Sv_OutLine(Danxian_StartCenter - 30);
                                        myflag.Flag_Danxian_Up = 0;
                                    }

                                }
                            }
                            else
                            {
                                for (i = CCD1_Down_Right_Point, CCD1_RightCount = 0; i < max(Sv_OutLine(CCD1_Down_Right_Point + 10), Sv_OutLine(Danxian_StartCenter + 30)); i++)
                                {
                                    if (P1[Sv_OutLine(i + 4)] - P1[i] > Range)
                                    {
                                        CCD1_RightCount++;
                                        if (CCD1_RightCount > 2)//&& (P1[Sv_OutLine(i)] - yuzhi) < 0)
                                        {
                                            myflag.Flag_Danxian_Up = 1;

                                            CCD1_Rise_Right_Point = i;
                                            //                                            setText用户自定义("上升沿");
                                            break;
                                        }
                                        //                            else if(CCD1_RightCount > 2 && (P1[Sv_OutLine(i)] - yuzhi) >= 0)
                                        //                            {
                                        //                                CCD1_RightCount = 0;
                                        //                            }
                                    }

                                    else
                                    {
                                        CCD1_RightCount = 0;
                                        CCD1_Rise_Right_Point = Sv_OutLine(Danxian_StartCenter - 30);
                                        myflag.Flag_Danxian_Up = 0;
                                    }

                                }
                            }

                            if (myflag.Flag_Danxian_Down == 1 && myflag.Flag_Danxian_Up == 1 && Abs(CCD1_Rise_Right_Point - CCD1_Down_Right_Point) <= 10)
                            {
                                myflag.Flag_Danxianyou = 1;

                                //                //GPIO_SetBits(PTA,GPIO_Pin_24);  

                                setText用户自定义("变为单线，搜为右线");

                                //                    CCD1_Right_Point = Sv_OutLine(Danxian_StartCenter - 30);
                                //                   

                                //                    for (i = Sv_OutLine(Danxian_StartCenter - 30); i < Sv_OutLine(Danxian_StartCenter + 30); i++)
                                //                    {
                                //                        if (P1[CCD1_Right_Point] > P1[i])
                                //                        {
                                //                            CCD1_Right_Point = i;
                                //                        }
                                //                    } 
                                //                CCD1_Right_Point=127;
                                myflag.Flag_Danxian_Down = 0;

                                myflag.Flag_Danxian_Up = 0;

                                Danxian_Center = CCD1_Right_Point;

                                goto Over;
                            }
                            else if (myflag.Flag_Danxian_Down == 1 && myflag.Flag_Danxian_Up == 0 && CCD1_Right_Point >= 110)
                            {
                                CCD1_Down_Right_Point_Temp = CCD1_Down_Right_Point;

                                for (i = CCD1_Down_Right_Point, CCD1_RightCount = 0; i < max(Sv_OutLine(CCD1_Down_Right_Point + 10), Sv_OutLine(Danxian_StartCenter + 30)); i++)
                                {
                                    if (P1[i] < (P1[CCD1_Down_Right_Point]))
                                    {
                                        CCD1_Down_Right_Point = (int)i;
                                        CCD1_RightCount = 0;
                                    }
                                    else if (P1[Sv_OutLine(i + 4)] - P1[i] > Range)
                                    {
                                        CCD1_RightCount++;
                                        if (CCD1_RightCount > 2 && Abs(CCD1_Down_Right_Point - CCD1_Down_Right_Point_Temp) <= 10)
                                        {
                                            setText用户自定义("变为单线，搜为右线");
                                            myflag.Flag_Danxianyou = 1;

                                            //                            //GPIO_SetBits(PTA,GPIO_Pin_24);  

                                            //                                CCD1_Right_Point = CCD1_Down_Right_Point;

                                            myflag.Flag_Danxian_Up = 1;

                                            myflag.Flag_Danxianyou = 1;

                                            //   CCD1_Right_Point=127;

                                            Danxian_Center = CCD1_Right_Point;

                                            goto Over;
                                        }
                                        else if (CCD1_RightCount > 2 && Abs(CCD1_Down_Right_Point - CCD1_Down_Right_Point_Temp) > 10)
                                        {
                                            CCD1_RightCount = 0;
                                        }
                                    }
                                    else
                                    {
                                        CCD1_RightCount = 0;
                                    }
                                    if (i == max(Sv_OutLine(CCD1_Down_Right_Point + 10), Sv_OutLine(Danxian_StartCenter + 30)) - 1)
                                    {
                                        myflag.Flag_Danxian_Down = 0;

                                        myflag.Flag_Danxian_Up = 0;

                                        myflag.Flag_Danxianyou = 0;

                                        CCD1_Rise_Right_Point = Sv_OutLine(Danxian_StartCenter - 30);

                                        CCD1_Down_Right_Point = Sv_OutLine(Danxian_StartCenter - 30);
                                        break;
                                    }
                                }
                            }
                            else if (myflag.Flag_Danxian_Down == 0 && myflag.Flag_Danxian_Up == 1 && CCD1_Right_Point >= 110)
                            {
                                CCD1_Rise_Right_Point_Temp = CCD1_Rise_Right_Point;
                                for (i = CCD1_Rise_Right_Point, CCD1_RightCount = 0; i > min(Sv_OutLine(CCD1_Rise_Right_Point - 10), Sv_OutLine(Danxian_StartCenter - 30)); i--)
                                {
                                    if (P1[i] < (P1[CCD1_Rise_Right_Point]))
                                    {
                                        CCD1_Rise_Right_Point = (int)i;

                                        CCD1_RightCount = 0;
                                    }
                                    else if (P1[Sv_OutLine(i - 4)] - P1[i] > Range)
                                    {
                                        CCD1_RightCount++;

                                        if (CCD1_RightCount > 2 && Abs(CCD1_Rise_Right_Point - CCD1_Rise_Right_Point_Temp) <= 10)
                                        {
                                            setText用户自定义("变为单线，搜为右线");
                                            myflag.Flag_Danxianyou = 1;

                                            myflag.Flag_Danxian_Up = 1;


                                            Danxian_Center = CCD1_Right_Point;

                                            goto Over;
                                        }
                                        else if (CCD1_RightCount > 2 && Abs(CCD1_Rise_Right_Point - CCD1_Rise_Right_Point_Temp) > 10)
                                        {
                                            CCD1_RightCount = 0;
                                        }
                                    }
                                    else
                                    {
                                        CCD1_RightCount = 0;
                                    }
                                    if (i == min(Sv_OutLine(CCD1_Rise_Right_Point - 10), Sv_OutLine(Danxian_StartCenter - 30)) + 1)
                                    {
                                        myflag.Flag_Danxian_Up = 0;

                                        myflag.Flag_Danxian_Down = 0;

                                        break;
                                    }
                                }
                            }
                            else
                            {
                                myflag.Flag_Danxian_Down = 0;

                                myflag.Flag_Danxian_Up = 0;

                                myflag.Flag_Danxianyou = 0;
                                CCD1_Rise_Right_Point = Sv_OutLine(Danxian_StartCenter - 30);
                                CCD1_Down_Right_Point = Sv_OutLine(Danxian_StartCenter - 30);

                            }
                        }
                    }
                    #endregion


                }
                #region//特殊情况，补线补到单线处
                if ((myflag.Flag_Danxianzuo == 0
                   && myflag.Flag_Danxianyou == 0
                   && myflag.Flag_ZhangAi_zuo == 0
                   && myflag.Flag_ZhangAi_you == 0
                   && (myflag.state == 0 || myflag.state == 1)
                   && DanxianSpecial_Out == 0
                   && myflag.LeftWan_Black_state <= 1
                   && myflag.RightWan_Black_state <= 1
                   && Flag_White == 0
                   && myflag.Start_PoDao != 1
                   && myflag.Flag_Temp_zuo == 0
                   && myflag.Flag_Temp_you == 0
                   && myflag.Flag_ZhangAi == 0
                   && ((CCD1_Right_Point - myline[0].RPoint <= -1 || CCD1_Left_Point - myline[0].LPoint >= 1) || (CCD1_Left_Point == 0 && CCD1_Right_Point == 127))
                   ) ||
                   (
                       (myflag.Flag_Danxianzuo == 0
                       && myflag.Flag_Danxianyou == 0
                       && myflag.Flag_ZhangAi_zuo == 0
                       && myflag.Flag_ZhangAi_you == 0
                       && (myflag.state == 5)
                       && DanxianSpecial_Out == 0
                       && myflag.Start_PoDao != 1
                       && myflag.LeftWan_Black_state <= 1
                       && myflag.RightWan_Black_state <= 1
                    //                && Flag_White == 0
                       && myflag.Flag_ZhangAi == 0
                       && myflag.Flag_Temp_zuo == 0
                       && myflag.Flag_Temp_you == 0)
                   )
               )
                {
                    if (CCD1_RightBool == 1 && CCD1_LeftBool == 1
                        && (CCD1_Right_Point + CCD1_Left_Point) / 2 - 30 <= CCD1_Left_Point
                        && (CCD1_Right_Point + CCD1_Left_Point) / 2 - 30 >= CCD1_Right_Point)
                    {
                        Danxian_Scan_Range = 20;
                    }
                    else
                    {
                        Danxian_Scan_Range = 30;
                    }

                    for (i = Sv_OutLine(myline[0].Center - Danxian_Scan_Range), CCD1_LeftCount = 0; i < Sv_OutLine(myline[0].Center + Danxian_Scan_Range); i++)
                    {
                        if (P1[i] - P1[Sv_OutLine(i + 4)] > Range)
                        {
                            CCD1_LeftCount++;
                            {
                                if (CCD1_LeftCount > 2 && (P1[Sv_OutLine(i + 4)] - CCD1_Center_Average) < 0)
                                {
                                    CCD1_Special_Down_Point = Sv_OutLine(i + 4);
                                    for (j = CCD1_Special_Down_Point, CCD1_RightCount = 0; j < max(Sv_OutLine(CCD1_Special_Down_Point + 10), Sv_OutLine(myline[0].Center + Danxian_Scan_Range)); j++)
                                    {
                                        if (P1[Sv_OutLine(j + 3)] - P1[Sv_OutLine(j)] > Range)
                                        {
                                            CCD1_RightCount++;
                                            if (CCD1_RightCount > 2 && (P1[Sv_OutLine(j)] - CCD1_Center_Average) < 0)
                                            {
                                                CCD1_Special_Up_Point = j;
                                                break;
                                            }
                                            else if (CCD1_RightCount > 2 && (P1[Sv_OutLine(j)] - CCD1_Center_Average) >= 0)
                                            {
                                                CCD1_RightCount = 0;
                                            }
                                        }
                                        else
                                        {
                                            CCD1_RightCount = 0;

                                        }
                                    }
                                    break;
                                }
                                else if (CCD1_LeftCount > 2 && (P1[Sv_OutLine(i + 4)] - CCD1_Center_Average) >= 0)
                                {
                                    CCD1_RightCount = 0;
                                }
                            }
                        }
                        else
                        {
                            CCD1_LeftCount = 0;

                        }
                    }
                    if (CCD1_Special_Up_Point != 0 && CCD1_Special_Down_Point != 0 && (CCD1_Special_Up_Point - CCD1_Special_Down_Point) <= 10)
                    {
                        CCD1_Special_Point = CCD1_Special_Down_Point;
                        for (i = CCD1_Special_Down_Point; i < CCD1_Special_Up_Point; i++)
                        {
                            if (P1[CCD1_Special_Point] > P1[i])
                            {
                                CCD1_Special_Point = i;
                            }
                        }

                    }
                    if (CCD1_Special_Point != 0)
                    {
                        if (CCD1_Left_Point != 0 && CCD1_Right_Point == 127 && (Abs(CCD1_Special_Point - myline[0].Center) < 15 || myflag.state == 5))
                        {
                            CCD1_Right_Point = CCD1_Special_Point;

                            myflag.Flag_Danxianyou = 1;

                            Danxian_Center = CCD1_Right_Point;

                            //GPIO_SetBits(PTA, GPIO_Pin_24);

                            goto Over;
                        }
                        else if (CCD1_Left_Point == 0 && CCD1_Right_Point != 127 && (Abs(CCD1_Special_Point - myline[0].Center) < 15 || myflag.state == 5))
                        {
                            CCD1_Left_Point = CCD1_Special_Point;

                            myflag.Flag_Danxianzuo = 1;

                            Danxian_Center = CCD1_Left_Point;

                            //GPIO_SetBits(PTA, GPIO_Pin_24);

                            goto Over;
                        }
                        else if (Abs(CCD1_Special_Point - myline[0].Center) < 15 || myflag.state == 5) //默认左
                        {
                            CCD1_Left_Point = CCD1_Special_Point;

                            myflag.Flag_Danxianzuo = 1;

                            Danxian_Center = CCD1_Left_Point;

                            //GPIO_SetBits(PTA, GPIO_Pin_24);

                            goto Over;
                        }
                    }
                }
                #endregion


                #endregion

                #region//判断出单线
                if (myflag.Flag_Danxianzuo == 1 || myflag.Flag_Danxianyou == 1)
                {
                    if (Danxian_Center <= 12)
                    {
                        for (i = 2, CCD1_RightCount = 0; i < Sv_OutLine(Danxian_Center + 30); i++)
                        {
                            if (P1[i] - P1[Sv_OutLine(i + 4)] > Range)
                            {
                                CCD1_RightCount++;
                                if (CCD1_RightCount > 1 && P1[Sv_OutLine(i + 4)] < CCD1_Center_Average)
                                {
                                    myflag.Flag_Danxian_Down = 1;
                                    //setText用户自定义("haha" + myflag.Flag_Danxian_Down);&& P1[Sv_OutLine(i)]  < CCD1_Center_Average

                                    CCD1_Down_Right_Point = Sv_OutLine(i + 4);

                                    break;
                                }
                            }

                            else
                            {
                                CCD1_RightCount = 0;
                                CCD1_Down_Right_Point = Sv_OutLine(Danxian_Center - 30);

                                myflag.Flag_Danxian_Down = 0;
                            }

                        }
                    }
                    else
                    {
                       
                        for (i = Sv_OutLine(Danxian_Center - 30), CCD1_RightCount = 0; i < Sv_OutLine(Danxian_Center + 30); i++)
                        {
                            if (P1[i] - P1[Sv_OutLine(i + 4)] > Range)
                            {
                                CCD1_RightCount++;
                                if (CCD1_RightCount > 1 && P1[Sv_OutLine(i + 4)] < CCD1_Center_Average)
                                {
                                    myflag.Flag_Danxian_Down = 1;
                                    //setText用户自定义("haha" + myflag.Flag_Danxian_Down);&& P1[Sv_OutLine(i)]  < CCD1_Center_Average

                                    CCD1_Down_Right_Point = Sv_OutLine(i + 4);

                                    break;
                                }
                            }

                            else
                            {
                                CCD1_RightCount = 0;
                                CCD1_Down_Right_Point = Sv_OutLine(Danxian_Center - 30);

                                myflag.Flag_Danxian_Down = 0;
                            }

                        }
                    }
                    if (Danxian_Center >= 115)
                    {
                        for (i = CCD1_Down_Right_Point, CCD1_RightCount = 0; i < 127; i++)
                        {
                            if (P1[Sv_OutLine(i + 1)] - P1[i] > 1)
                            {
                                if (P1[Sv_OutLine(i)] < CCD1_Center_Average)
                                {
                                    myflag.Flag_Danxian_Up = 1;

                                    CCD1_Rise_Right_Point = i;
                                    break;
                                }
                            }

                            else
                            {
                                CCD1_Rise_Right_Point = Sv_OutLine(Danxian_Center - 30);
                                myflag.Flag_Danxian_Up = 0;
                            }

                        }
                    }
                    else
                    {
                       
                        for (i = CCD1_Down_Right_Point, CCD1_RightCount = 0; i < max(Sv_OutLine(CCD1_Down_Right_Point + 10), Sv_OutLine(Danxian_Center + 30)); i++)
                        {
                            if (P1[Sv_OutLine(i + 4)] - P1[i] > Range)
                            {
                                CCD1_RightCount++;
                                if (CCD1_RightCount > 1 && P1[Sv_OutLine(i)] < CCD1_Center_Average)
                                {
                                    myflag.Flag_Danxian_Up = 1;

                                    CCD1_Rise_Right_Point = i;
                                    //////setText用户自定义("hehe" + myflag.Flag_Danxian_Up);
                                    break;
                                }
                            }

                            else
                            {
                                CCD1_RightCount = 0;
                                CCD1_Rise_Right_Point = Sv_OutLine(Danxian_Center - 30);
                                myflag.Flag_Danxian_Up = 0;
                            }

                        }
                    }
                    if (myflag.Flag_Danxian_Down == 1 && myflag.Flag_Danxian_Up == 0)
                    {
                        for (i = CCD1_Down_Right_Point, CCD1_RightCount = 0; i < max(Sv_OutLine(CCD1_Down_Right_Point + 10), Sv_OutLine(Danxian_Center + 30)); i++)
                        {
                            if (P1[i] < (P1[CCD1_Down_Right_Point]))
                            {
                                CCD1_Down_Right_Point = (int)i;

                                CCD1_RightCount = 0;
                            }
                            else if (P1[i] > (P1[CCD1_Down_Right_Point]) + 1)
                            {
                                CCD1_RightCount++;

                                if (CCD1_RightCount > 1)
                                {
                                    myflag.Flag_Danxian_Up = 1;
                                    break;
                                }
                            }
                            else
                            {
                                CCD1_RightCount = 0;
                            }
                            if (i == max(Sv_OutLine(CCD1_Down_Right_Point + 10), Sv_OutLine(Danxian_Center + 30)) - 1)
                            {
                                myflag.Flag_Danxian_Up = 0;

                                myflag.Flag_Danxian_Down = 0;

                                break;
                            }
                        }
                    }
                    else if (myflag.Flag_Danxian_Down == 0 && myflag.Flag_Danxian_Up == 1)
                    {
                        for (i = CCD1_Rise_Right_Point, CCD1_RightCount = 0; i > min(Sv_OutLine(CCD1_Rise_Right_Point - 10), Sv_OutLine(Danxian_Center - 30)); i--)
                        {
                            if (P1[i] < (P1[CCD1_Rise_Right_Point]))
                            {
                                CCD1_Rise_Right_Point = (int)i;

                                CCD1_RightCount = 0;
                            }
                            else if (P1[i] > (P1[CCD1_Rise_Right_Point])+1)
                            {
                                CCD1_RightCount++;

                                if (CCD1_RightCount > 1)
                                {
                                    myflag.Flag_Danxian_Down = 1;
                                    break;
                                }
                            }
                            else
                            {
                                CCD1_RightCount = 0;
                            }
                            if (i == min(Sv_OutLine(CCD1_Down_Right_Point - 10), Sv_OutLine(Danxian_Center - 30)) + 1)
                            {
                                myflag.Flag_Danxian_Up = 0;

                                myflag.Flag_Danxian_Down = 0;

                                break;
                            }
                        }
                    }

                    if (myflag.Flag_Danxian_Down == 1 && myflag.Flag_Danxian_Up == 1)
                    {
                        if (myflag.Flag_Danxianzuo == 1)
                        {
                            CCD1_Left_Point = CCD1_Down_Right_Point;

                            for (i = CCD1_Down_Right_Point; i < CCD1_Rise_Right_Point; i++)
                            {
                                if (P1[CCD1_Left_Point] > P1[i])
                                {
                                    CCD1_Left_Point = i;
                                    CCD1_LeftBool = 1;
                                }
                            }
                            goto Over;
                        }
                        else if (myflag.Flag_Danxianyou == 1)
                        {
                            CCD1_Right_Point = CCD1_Down_Right_Point;

                            for (i = CCD1_Down_Right_Point; i < CCD1_Rise_Right_Point; i++)
                            {
                                if (P1[CCD1_Right_Point] > P1[i])
                                {
                                    CCD1_Right_Point = i;
                                    CCD1_RightBool = 1;
                                }
                            }
                            goto Over;
                        }
                    }
                    if (myflag.Flag_Danxian_Down == 0 && myflag.Flag_Danxian_Up == 0)
                    {

                        {
                            if (myflag.Flag_Danxianzuo == 1)
                            {
                                DanxianSpecial_Out = 1;
                                Danxian_OutCenter = myline[0].LPoint;
                                myflag.Flag_Danxianzuo = 0;
                                myflag.Start_Danxian = 0;
                                //setText用户自定义("出单线丢线");
                            }
                            else if (myflag.Flag_Danxianyou == 1)
                            {
                                myflag.Flag_Danxianyou = 0;
                                //                        //setText用户自定义("出单线丢线");
                                Danxian_OutCenter = myline[0].RPoint;
                                DanxianSpecial_Out = 2;
                                myflag.Start_Danxian = 0;
                            }
                        }

                        //                //GPIO_SetBits(PTA,GPIO_Pin_24);  
                        CCD1_LeftBool_Last = 0;

                        CCD1_RightBool_Last = 0;

                        myflag.Flag_Danxianzuo = 0;

                        myflag.Flag_Danxianyou = 0;
                        /*清除直角，单线，十字判断*/
                        myflag.state = 0;//

                        myflag.Flag_Temp_zuo = 0;

                        myflag.Flag_Temp_you = 0;

                        myflag.Flag_ZhangAi_zuo = 0;

                        myflag.Flag_ZhangAi_you = 0;

                        myflag.ShiZi_Flag = 1;

                        Danxian_Center = 63;
                        /******************/
                        setText用户自定义("出单线");
                        //CCD1_LeftBool = 1; 
                        //CCD1_RightBool = 1;
                        goto Over;
                    }
                    else if ((CCD1_Left_Point - myline[0].LPoint < -Danxian_Range + 5 && myflag.Flag_Danxianzuo == 1) || (CCD1_Right_Point - myline[0].RPoint > Danxian_Range - 5 && myflag.Flag_Danxianyou == 1))
                    {
                        {
                            if (myflag.Flag_Danxianzuo == 1)
                            {
                                DanxianSpecial_Out = 1;
                                myflag.Flag_Danxianzuo = 0;
                                Danxian_OutCenter = myline[0].LPoint;
                                myflag.Start_Danxian = 0;
                                //                        //setText用户自定义("出单线丢线");
                            }
                            else if (myflag.Flag_Danxianyou == 1)
                            {
                                myflag.Flag_Danxianyou = 0;
                                Danxian_OutCenter = myline[0].RPoint;
                                //                        //setText用户自定义("出单线丢线");
                                myflag.Start_Danxian = 0;
                                DanxianSpecial_Out = 2;
                            }
                        }
                        CCD1_LeftBool_Last = 0;

                        CCD1_RightBool_Last = 0;

                        myflag.Flag_Danxianyou = 0;

                        myflag.Flag_Danxianzuo = 0;

                        myflag.state = 0;//

                        myflag.Flag_Temp_zuo = 0;

                        myflag.Flag_Temp_you = 0;

                        myflag.Flag_ZhangAi_zuo = 0;

                        myflag.Flag_ZhangAi_you = 0;

                        myflag.ShiZi_Flag = 1;

                        Danxian_Center = 63;

                        setText用户自定义("出单线");
                        //                //GPIO_SetBits(PTA,GPIO_Pin_24);  

                        goto Over;
                    }
                    else if (CCD1_Left_Point == 0 && CCD1_Right_Point == 127)
                    {

                        if (myflag.Flag_Danxianzuo == 1)
                        {
                            DanxianSpecial_Out = 1;
                            myflag.Flag_Danxianzuo = 0;
                            Danxian_OutCenter = myline[0].LPoint;
                            myflag.Start_Danxian = 0;
                            //                        //setText用户自定义("出单线丢线");
                        }
                        else if (myflag.Flag_Danxianyou == 1)
                        {
                            myflag.Flag_Danxianyou = 0;
                            Danxian_OutCenter = myline[0].RPoint;
                            myflag.Start_Danxian = 0;
                            //                        //setText用户自定义("出单线丢线");

                            DanxianSpecial_Out = 2;
                        }
                        CCD1_LeftBool_Last = 0;

                        CCD1_RightBool_Last = 0;

                        myflag.Flag_Danxianyou = 0;

                        myflag.Flag_Danxianzuo = 0;

                        myflag.state = 0;//

                        myflag.Flag_Temp_zuo = 0;

                        myflag.Flag_Temp_you = 0;

                        myflag.Flag_ZhangAi_zuo = 0;

                        myflag.Flag_ZhangAi_you = 0;

                        myflag.ShiZi_Flag = 1;

                        Danxian_Center = 63;
                        setText用户自定义("出单线");
                        //                //GPIO_SetBits(PTA,GPIO_Pin_24);  

                        goto Over;
                    }
                }
                #endregion

                #region//判断直角
                #region//跳变判断
                if (feature[0].Center != 0 && myflag.Flag_Danxianzuo == 0 && myflag.Flag_Danxianyou == 0 && (myflag.state == 0 || myflag.state == 4))
                {
                    if (
                            P1[63] * 1.0 / feature[3].Center < 0.8
                         && P1[43] * 1.0 / feature[3].Left_P1 < 0.8
                         && P1[53] * 1.0 / feature[3].Left_P2 < 0.8
                         && P1[73] * 1.0 / feature[3].Right_P1 < 0.8
                         && P1[83] * 1.0 / feature[3].Right_P2 < 0.8
                         && P1[63] - feature[3].Center <= -BLACK_CHANGE_RANGE
                         && P1[43] - feature[3].Left_P1 <= -BLACK_CHANGE_RANGE
                         && P1[53] - feature[3].Left_P2 <= -BLACK_CHANGE_RANGE
                         && P1[73] - feature[3].Right_P1 <= -BLACK_CHANGE_RANGE
                         && P1[83] - feature[3].Right_P2 <= -BLACK_CHANGE_RANGE

                         && (myflag.state == 0 || myflag.state == 4)
                         && P1[63] - yuzhi < 0
                         && P1[43] - yuzhi < 0
                         && P1[53] - yuzhi < 0
                         && P1[73] - yuzhi < 0
                         && P1[83] - yuzhi < 0
                         && (length.LLength + length.RLength) > 80
                       )
                    {
                        //还要强调之前必须是直线
                        obstacle.Statusl = ZHI_JIAO;

                        myflag.Flag_ZhangAi_zuo = 0;

                        myflag.Flag_ZhangAi_you = 0;

                        myflag.Flag_Danxianzuo = 0;

                        myflag.Flag_Danxianyou = 0;

                        myflag.ShiZi_Flag = 1;

                        setText用户自定义("黑带判断-跳变");
                        //               // GPIO_SetBits(PTA,GPIO_Pin_24);  
                        goto Over;


                    }
                  
                    if (
                        ((myflag.state == 0 || myflag.state == 4) && (CCD1_Center_Average - CCD1_CCD1_Center_Average2) > 20 && (length.LLength + length.RLength) > 80)
                    ||( (length.LLength + length.RLength) > 100 && DanxianSpecial_Out == 0)
                    ||
                        (
                            feature[3].Center * 1.0 / P1[63] < 0.8
                         && feature[3].Left_P1 * 1.0 / P1[43] < 0.8
                         && feature[3].Left_P2 * 1.0 / P1[53] < 0.8
                         && feature[3].Right_P1 * 1.0 / P1[73] < 0.8
                         && feature[3].Right_P2 * 1.0 / P1[83] < 0.8
                         && feature[3].Center - P1[63] <= -BLACK_CHANGE_RANGE
                         && feature[3].Left_P1 - P1[43] <= -BLACK_CHANGE_RANGE
                         && feature[3].Left_P2 - P1[53] <= -BLACK_CHANGE_RANGE
                         && feature[3].Right_P1 - P1[73] <= -BLACK_CHANGE_RANGE
                         && feature[3].Right_P2 - P1[83] <= -BLACK_CHANGE_RANGE

                         && (myflag.state == 0 || myflag.state == 4)
                         && P1[63] - yuzhi > 0
                         && P1[43] - yuzhi > 0
                         && P1[53] - yuzhi > 0
                         && P1[73] - yuzhi > 0
                         && P1[83] - yuzhi > 0
                         && (length.LLength + length.RLength) < 60
                    )
                      )
                    {
                        obstacle.Statusl = ZHI_JIAO;

                        myflag.Flag_ZhangAi_zuo = 0;

                        myflag.Flag_ZhangAi_you = 0;

                        myflag.Flag_Danxianzuo = 0;

                        myflag.Flag_Danxianyou = 0;

                        myflag.ShiZi_Flag = 1;
                        setText用户自定义("黑带判断-反跳变");
                        //               // GPIO_SetBits(PTA,GPIO_Pin_24);  
                        goto Over;
                    }

                }
                #endregion

                #region/**********************************左弯入黑带**************************************/
                //左弯入黑带----------直接弯入。。。。
                if (myflag.Flag_Danxianzuo == 0
                  && myflag.Flag_Danxianyou == 0
                    //            && myflag.Flag_ZhangAi_zuo == 0
                    //            && myflag.Flag_ZhangAi_you == 0
                  && (myflag.state == 0)
                    //  && DanxianSpecial_Out == 0
                    //            && Flag_White == 0
                  && myflag.Flag_Temp_zuo == 0
                  && myflag.Flag_Temp_you == 0
                  && myflag.RightWan_Black_state <= 1)
                {
                    if (myflag.LeftWan_Black_state == 6)
                    {
                        if (CCD1_Right_Point != 127 && CCD1_Left_Point != 0 && CCD1_Right_Point - CCD1_Left_Point > 80)
                        {
                            obstacle.Statusl = ZHI_JIAO;

                            myflag.Flag_ZhangAi_zuo = 0;

                            myflag.Flag_ZhangAi_you = 0;

                            myflag.Flag_Danxianzuo = 0;

                            myflag.Flag_Danxianyou = 0;

                            myflag.ShiZi_Flag = 1;

                            //GPIO_SetBits(PTA,GPIO_Pin_24);  
                            setText用户自定义("黑带判断-左弯入");
                            myflag.LeftWan_Black_state = 0;

                            goto Over;
                        }

                    }
                    else if (myflag.LeftWan_Black_state == 5)
                    {
                        if (CCD1_Left_Point > myline[0].LPoint && (P1[Sv_OutLine(CCD1_Left_Point - 3)] - P1[CCD1_Left_Point]) < (BLACK_CHANGE_RANGE-2) && (myline[0].RPoint - CCD1_Right_Point) >= -3)
                        {
                            //                    setText用户自定义("有可能是单线或弯入黑带");
                            //CCD1_Left_Point = 0;
                            for (i = CCD1_Left_Point, CCD1_LeftCount = 0; i > CCD1_LEFT_LIMIT; i--)
                            {
                                if (P1[Sv_OutLine_Center(i - JianJU)] - P1[i] > Range)
                                {
                                    CCD1_LeftCount++;
                                    if (CCD1_LeftCount > 1)
                                    {
                                        myflag.LeftWan_Black_state = 6;
                                        break;
                                    }
                                }
                                else
                                {
                                    CCD1_LeftCount = 0;
                                }
                            }
                            if (myflag.LeftWan_Black_state == 5)
                            {
                                myflag.LeftWan_Black_state = 0;
                            }
                        }
                        else if (CCD1_Left_Point == 0)//&& CCD1_Right_Point < 100)
                        {
                            if (myline[0].LPoint > 25)
                            {
                                //                        setText用户自定义("有可能是单线或弯入黑带");
                                //CCD1_Left_Point = 0;
                                myflag.LeftWan_Black_state = 6;
                            }
                        }
                        else
                        {
                            myflag.LeftWan_Black_state = 0;
                        }
                    }
                    else if (myflag.LeftWan_Black_state == 4)
                    {
                        if (CCD1_Left_Point > myline[0].LPoint && (P1[Sv_OutLine(CCD1_Left_Point - 3)] - P1[CCD1_Left_Point]) < (BLACK_CHANGE_RANGE-2) && (myline[0].RPoint - CCD1_Right_Point) >= -3)
                        {
                            //                    setText用户自定义("有可能是单线或弯入黑带");
                            //CCD1_Left_Point = 0;
                            for (i = CCD1_Left_Point, CCD1_LeftCount = 0; i > CCD1_LEFT_LIMIT; i--)
                            {
                                if (P1[Sv_OutLine_Center(i - JianJU)] - P1[i] > Range)
                                {
                                    CCD1_LeftCount++;
                                    if (CCD1_LeftCount > 1)
                                    {
                                        myflag.LeftWan_Black_state = 5;
                                        break;
                                    }
                                }
                                else
                                {
                                    CCD1_LeftCount = 0;
                                }
                            }
                            if (myflag.LeftWan_Black_state == 4)
                            {
                                myflag.LeftWan_Black_state = 0;
                            }
                        }
                        else if (CCD1_Left_Point == 0)//&& CCD1_Right_Point < 100)
                        {
                            if (myline[0].LPoint > 25)
                            {
                                //                        setText用户自定义("有可能是单线或弯入黑带");
                                //CCD1_Left_Point = 0;
                                myflag.LeftWan_Black_state = 6;
                            }
                        }
                        else
                        {
                            myflag.LeftWan_Black_state = 0;
                        }
                    }

                    else if (myflag.LeftWan_Black_state == 3)
                    {
                        if (CCD1_Left_Point > myline[0].LPoint && (P1[Sv_OutLine(CCD1_Left_Point - 3)] - P1[CCD1_Left_Point]) < (BLACK_CHANGE_RANGE-2) && (myline[0].RPoint - CCD1_Right_Point) >= -3)
                        {
                            setText用户自定义("有可能是单线或弯入黑带");
                            //CCD1_Left_Point = 0;
                            for (i = CCD1_Left_Point, CCD1_LeftCount = 0; i > CCD1_LEFT_LIMIT; i--)
                            {
                                if (P1[Sv_OutLine_Center(i - JianJU)] - P1[i] > Range)
                                {
                                    CCD1_LeftCount++;
                                    if (CCD1_LeftCount > 1)
                                    {
                                        myflag.LeftWan_Black_state = 4;
                                        break;
                                    }
                                }
                                else
                                {
                                    CCD1_LeftCount = 0;
                                }
                            }
                            if (myflag.LeftWan_Black_state == 3)
                            {
                                myflag.LeftWan_Black_state = 0;
                            }
                        }
                        else if (CCD1_Left_Point == 0)//&& CCD1_Right_Point < 100)
                        {
                            if (myline[0].LPoint > 25)
                            {
                                //                        setText用户自定义("有可能是单线或弯入黑带");
                                //CCD1_Left_Point = 0;
                                myflag.LeftWan_Black_state = 6;
                            }
                        }
                        else
                        {
                            myflag.LeftWan_Black_state = 0;
                        }
                    }
                    else if (myflag.LeftWan_Black_state == 2)
                    {
                        if (CCD1_Left_Point > myline[0].LPoint && (P1[Sv_OutLine(CCD1_Left_Point - 3)] - P1[CCD1_Left_Point]) < (BLACK_CHANGE_RANGE-2) && (myline[0].RPoint - CCD1_Right_Point) >= -3)
                        {
                            //setText用户自定义("有可能是单线或弯入黑带");
                            //CCD1_Left_Point = 0;
                            for (i = CCD1_Left_Point, CCD1_LeftCount = 0; i > CCD1_LEFT_LIMIT; i--)
                            {
                                if (P1[Sv_OutLine_Center(i - JianJU)] - P1[i] > Range)
                                {
                                    CCD1_LeftCount++;
                                    if (CCD1_LeftCount > 1)
                                    {
                                        myflag.LeftWan_Black_state = 3;
                                        break;
                                    }
                                }
                                else
                                {
                                    CCD1_LeftCount = 0;
                                }
                            }
                            if (myflag.LeftWan_Black_state == 2)
                            {
                                myflag.LeftWan_Black_state = 0;
                            }
                        }
                        else if (CCD1_Left_Point == 0)//&& CCD1_Right_Point < 100)
                        {
                            if (myline[0].LPoint > 25)
                            {
                                //                        setText用户自定义("有可能是单线或弯入黑带");
                                //CCD1_Left_Point = 0;
                                myflag.LeftWan_Black_state = 6;
                            }
                        }
                        else
                        {
                            myflag.LeftWan_Black_state = 0;
                        }
                    }
                    else if (myflag.LeftWan_Black_state == 1)
                    {
                        if (CCD1_Left_Point > myline[0].LPoint && (myline[0].RPoint - CCD1_Right_Point) >= -3 && (P1[Sv_OutLine(CCD1_Left_Point - 3)] - P1[CCD1_Left_Point]) < (BLACK_CHANGE_RANGE-2))
                        {
                            for (i = CCD1_Left_Point, CCD1_LeftCount = 0; i > CCD1_LEFT_LIMIT; i--)
                            {
                                if (P1[Sv_OutLine_Center(i - JianJU - 1)] - P1[i] > Range)
                                {
                                    CCD1_LeftCount++;
                                    if (CCD1_LeftCount > 1)
                                    {
                                        myflag.LeftWan_Black_state = 2;
                                        break;
                                    }
                                }
                                else if (i == CCD1_LEFT_LIMIT + 1 && Found_Down_Block_zuo(Sv_OutLine_Center(CCD1_Left_Point + 10)) != 0)
                                {
                                    myflag.RightWan_Black_state = 2;
                                    break;
                                }
                                else
                                {
                                    CCD1_LeftCount = 0;
                                }
                            }
                            setText用户自定义("有可能是单线或弯入黑带");
                            //CCD1_Left_Point = 0;

                        }
                        else
                        {
                            myflag.LeftWan_Black_state = 0;
                        }
                    }
                    else if (myflag.LeftWan_Black_state == 0)
                    {
                        if ((CCD1_Left_Point - myline[0].LPoint) > 2 && myline[0].LPoint == 0 && CCD1_Right_Point <= 115)
                        {
                            //                    setText用户自定义("有可能是单线或弯入黑带");
                            //CCD1_Left_Point = 0;
                            myflag.LeftWan_Black_state = 1;
                        }
                        else if ((CCD1_Left_Point - myline[0].LPoint) > 10 && CCD1_Right_Point <= 115)//可能是小弯入黑带
                        {
                            //                    setText用户自定义("有可能是单线或弯入黑带");
                            //CCD1_Left_Point = 0;
                            myflag.LeftWan_Black_state = 1;
                        }
                    }
                    setText用户自定义(" myflag.LeftWan_Black_state " + myflag.LeftWan_Black_state);
                }
                #endregion

                #region/**********************************右弯入黑带**************************************/
                //右弯入黑带----------直接弯入。。。。
                if (myflag.Flag_Danxianzuo == 0
                   && myflag.Flag_Danxianyou == 0
                    //            && myflag.Flag_ZhangAi_zuo == 0
                    //            && myflag.Flag_ZhangAi_you == 0
                   && (myflag.state == 0)
                    //  && DanxianSpecial_Out == 0
                    //            && Flag_White == 0
                   && myflag.Flag_Temp_zuo == 0
                   && myflag.Flag_Temp_you == 0
                   && myflag.LeftWan_Black_state <= 1)
                {
                    if (myflag.RightWan_Black_state == 6)
                    {
                        if (CCD1_Right_Point != 127 && CCD1_Left_Point != 0 && CCD1_Right_Point - CCD1_Left_Point > 80)
                        {
                            obstacle.Statusl = ZHI_JIAO;

                            myflag.Flag_ZhangAi_zuo = 0;

                            myflag.Flag_ZhangAi_you = 0;

                            myflag.Flag_Danxianzuo = 0;

                            myflag.Flag_Danxianyou = 0;

                            myflag.ShiZi_Flag = 1;

                            //                   // GPIO_SetBits(PTA,GPIO_Pin_24);  
                            setText用户自定义("黑带判断-右弯入");

                            myflag.RightWan_Black_state = 0;

                            goto Over;
                        }

                    }
                    else if (myflag.RightWan_Black_state == 5)
                    {
                        if (CCD1_Left_Point - myline[0].LPoint >= -3 && CCD1_Right_Point < myline[0].RPoint && (P1[Sv_OutLine(CCD1_Right_Point + 3)] - P1[CCD1_Right_Point]) < (BLACK_CHANGE_RANGE-2))
                        {
                            //                    setText用户自定义("有可能是单线或弯入黑带");
                            //CCD1_Left_Point = 0;
                            for (i = CCD1_Right_Point, CCD1_RightCount = 0; i < CCD1_RIGHT_LIMIT; i++)
                            {
                                if (P1[Sv_OutLine_Center(i + JianJU)] - P1[i] > Range)
                                {
                                    CCD1_RightCount++;
                                    if (CCD1_RightCount > 1)
                                    {
                                        myflag.RightWan_Black_state = 6;
                                        break;
                                    }
                                }
                                else
                                {
                                    CCD1_RightCount = 0;
                                }
                            }
                            if (myflag.RightWan_Black_state == 5)
                            {
                                myflag.RightWan_Black_state = 0;
                            }
                        }
                        else if (CCD1_Right_Point == 127)//&& CCD1_Left_Point > 25)
                        {
                            if (myline[0].RPoint < 100)
                            {
                                //                        setText用户自定义("有可能是单线或弯入黑带");
                                //CCD1_Left_Point = 0;
                                myflag.RightWan_Black_state = 6;
                            }
                        }
                        else
                        {
                            myflag.RightWan_Black_state = 0;
                        }
                    }
                    else if (myflag.RightWan_Black_state == 4)
                    {
                        if (CCD1_Left_Point - myline[0].LPoint >= -3 && CCD1_Right_Point < myline[0].RPoint && (P1[Sv_OutLine(CCD1_Right_Point + 3)] - P1[CCD1_Right_Point]) < (BLACK_CHANGE_RANGE-2))
                        {
                            //                    setText用户自定义("有可能是单线或弯入黑带");
                            //CCD1_Left_Point = 0;
                            for (i = CCD1_Right_Point, CCD1_RightCount = 0; i < CCD1_RIGHT_LIMIT; i++)
                            {
                                if (P1[Sv_OutLine_Center(i + JianJU)] - P1[i] > Range)
                                {
                                    CCD1_RightCount++;
                                    if (CCD1_RightCount > 1)
                                    {
                                        myflag.RightWan_Black_state = 5;
                                        break;
                                    }
                                }
                                else
                                {
                                    CCD1_RightCount = 0;
                                }
                            }
                            if (myflag.RightWan_Black_state == 4)
                            {
                                myflag.RightWan_Black_state = 0;
                            }
                        }
                        else if (CCD1_Right_Point == 127)//&& CCD1_Left_Point > 25)
                        {
                            if (myline[0].RPoint < 100)
                            {
                                //                        setText用户自定义("有可能是单线或弯入黑带");
                                //CCD1_Left_Point = 0;
                                myflag.RightWan_Black_state = 6;
                            }
                        }
                        else
                        {
                            myflag.RightWan_Black_state = 0;
                        }
                    }

                    else if (myflag.RightWan_Black_state == 3)
                    {
                        if (CCD1_Left_Point - myline[0].LPoint >= -3 && CCD1_Right_Point < myline[0].RPoint && (P1[Sv_OutLine(CCD1_Right_Point + 3)] - P1[CCD1_Right_Point]) < (BLACK_CHANGE_RANGE-2))
                        {
                            //                    setText用户自定义("有可能是单线或弯入黑带");
                            //CCD1_Left_Point = 0;
                            for (i = CCD1_Right_Point, CCD1_RightCount = 0; i < CCD1_RIGHT_LIMIT; i++)
                            {
                                if (P1[Sv_OutLine_Center(i + JianJU)] - P1[i] > Range)
                                {
                                    CCD1_RightCount++;
                                    if (CCD1_RightCount > 1)
                                    {
                                        myflag.RightWan_Black_state = 4;
                                        break;
                                    }
                                }
                                else
                                {
                                    CCD1_RightCount = 0;
                                }
                            }
                            if (myflag.RightWan_Black_state == 3)
                            {
                                myflag.RightWan_Black_state = 0;
                            }
                        }
                        else if (CCD1_Right_Point == 127)//&& CCD1_Left_Point > 25)
                        {
                            if (myline[0].RPoint < 100)
                            {
                                //                        setText用户自定义("有可能是单线或弯入黑带");
                                //CCD1_Left_Point = 0;
                                myflag.RightWan_Black_state = 6;
                            }
                        }
                        else
                        {
                            myflag.RightWan_Black_state = 0;
                        }
                    }
                    else if (myflag.RightWan_Black_state == 2)
                    {
                        if (CCD1_Left_Point - myline[0].LPoint >= -3 && CCD1_Right_Point < myline[0].RPoint && (P1[Sv_OutLine(CCD1_Right_Point + 3)] - P1[CCD1_Right_Point]) < (BLACK_CHANGE_RANGE-2))
                        {
                            //                    setText用户自定义("有可能是单线或弯入黑带");
                            //CCD1_Left_Point = 0;
                            for (i = CCD1_Right_Point, CCD1_RightCount = 0; i < CCD1_RIGHT_LIMIT; i++)
                            {
                                if (P1[Sv_OutLine_Center(i + JianJU)] - P1[i] > Range)
                                {
                                    CCD1_RightCount++;
                                    if (CCD1_RightCount > 1)
                                    {
                                        myflag.RightWan_Black_state = 3;
                                        break;
                                    }
                                }
                                else
                                {
                                    CCD1_RightCount = 0;
                                }
                            }
                            if (myflag.RightWan_Black_state == 2)
                            {
                                myflag.RightWan_Black_state = 0;
                            }
                        }
                        else if (CCD1_Right_Point == 127)//&& CCD1_Left_Point > 25)
                        {
                            if (myline[0].RPoint < 100)
                            {
                                //                        setText用户自定义("有可能是单线或弯入黑带");
                                //CCD1_Left_Point = 0;
                                myflag.RightWan_Black_state = 6;
                            }
                        }
                        else
                        {
                            myflag.RightWan_Black_state = 0;
                        }
                    }
                    else if (myflag.RightWan_Black_state == 1)
                    {
                        if (CCD1_Left_Point - myline[0].LPoint >= -3 && CCD1_Right_Point < myline[0].RPoint && (P1[Sv_OutLine(CCD1_Right_Point + 3)] - P1[CCD1_Right_Point]) < (BLACK_CHANGE_RANGE-2))
                        {
                            //                    setText用户自定义("有可能是单线或弯入黑带");
                            //CCD1_Left_Point = 0;
                            for (i = CCD1_Right_Point, CCD1_RightCount = 0; i < CCD1_RIGHT_LIMIT; i++)
                            {
                                if (P1[Sv_OutLine_Center(i + JianJU + 1)] - P1[i] > Range)
                                {
                                    CCD1_RightCount++;
                                    if (CCD1_RightCount > 1)
                                    {
                                        myflag.RightWan_Black_state = 2;
                                        break;
                                    }
                                }
                                else if (i == CCD1_RIGHT_LIMIT - 1 && Found_Down_Block_you(Sv_OutLine_Center(CCD1_Right_Point + 10)) != 0)
                                {
                                    myflag.RightWan_Black_state = 2;
                                    break;
                                }
                                else
                                {
                                    CCD1_RightCount = 0;
                                }
                            }
                        }
                        else
                        {
                            myflag.RightWan_Black_state = 0;
                        }
                    }
                    else if (myflag.RightWan_Black_state == 0)
                    {
                        if ((myline[0].RPoint - CCD1_Right_Point) > 2 && myline[0].RPoint == 127 && CCD1_Left_Point >= 15)
                        {
                            //                    setText用户自定义("有可能是单线或弯入黑带");
                            //CCD1_Left_Point = 0;
                            myflag.RightWan_Black_state = 1;
                        }
                        else if ((myline[0].RPoint - CCD1_Right_Point) > 10 && CCD1_Left_Point >= 15)//小弯入黑带
                        {
                            //                    setText用户自定义("有可能是单线或弯入黑带");
                            //CCD1_Left_Point = 0;
                            myflag.RightWan_Black_state = 1;
                        }
                    }
                    setText用户自定义(" myflag.RightWan_Black_state " + myflag.RightWan_Black_state);
                }
                #endregion
                #endregion

                #region//直接判断直角//
                if (myflag.Flag_Danxianzuo == 0
                    && myflag.Flag_Danxianyou == 0
                    && myflag.Flag_ZhangAi_zuo == 0
                    && myflag.Flag_ZhangAi_you == 0
                    && (myflag.state < 3)
                    && DanxianSpecial_Out == 0
                    && Flag_White == 0
                    && myflag.Start_PoDao == 0
                    && myflag.Flag_Temp_zuo == 0
                    && myflag.Flag_Temp_you == 0
                    && myflag.LeftWan_Black_state <= 1
                    && myflag.RightWan_Black_state <= 1
                    && Zhijiao_Lost_State_Zuo == 0
                    && Zhijiao_Lost_State_You == 0
                    && (Flag_Special_Control == 0 || Flag_Special_Control == 2)
                    )
                {
                   
                    if (
                           Abs(CCD1_HistoryCenter[8] - 63) < 15 && Abs(CCD1_HistoryCenter[7] - 63) < 15 && Abs(CCD1_HistoryCenter[6] - 63) < 15
                        && Abs(CCD1_Left_Point - myline[0].LPoint) < 10 && CCD1_Left_Point != 0 && myline[0].LPoint != 0
                        && Abs(CCD1_Left_Point - CCD1_HistoryLpoint[3]) < 10 && CCD1_HistoryLpoint[3] != 0
                        && Abs(CCD1_Left_Point - CCD1_HistoryLpoint[2]) < 10 && CCD1_HistoryLpoint[2] != 0
                        && Abs(CCD1_Left_Point - CCD1_HistoryLpoint[1]) < 10 && CCD1_HistoryLpoint[1] != 0
                        && CCD1_Right_Point == 127 && myline[0].RPoint != 127
                        && CCD1_Right_Point - myline[0].RPoint >= 8
                        && CCD1_Left_Point >= 5 && myline[0].LPoint >= 5
                        && CCD1_Left_Point < 20 && myline[0].LPoint < 20
                        && myline[0].RPoint >= 105
                        && myline[0].RPoint < 120
                        && Abs(CCD1_Center_Average - CCD1_CCD1_Center_Average2) < 15
                       )
                    {
                      
                        Zhijiao_Lost_State_Zuo = 1;

                        for (i = CCD1_Left_Point, CCD1_RightCount = 0; i <= CCD1_RIGHT_LIMIT; i++)
                        {
                            if (P1[i] - P1[Sv_OutLine(i + 3)] > Range)
                            {
                                CCD1_RightCount++;
                                {
                                    if (CCD1_RightCount > 1)
                                    {
                                        Zhijiao_Lost_State_Zuo = 0;
                                        //setText用户自定义("右下降沿" + CCD1_Right_Point);

                                        CCD1_RightCount = 0;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                CCD1_RightCount = 0;

                            }
                        }
                        
                        if (Zhijiao_Lost_State_Zuo == 1)
                        {
                            goto Over;
                        }


                    }
                    else if (
                           Abs(CCD1_HistoryCenter[8] - 63) < 15 && Abs(CCD1_HistoryCenter[7] - 63) < 15 && Abs(CCD1_HistoryCenter[6] - 63) < 15 && Abs(CCD1_Right_Point - myline[0].RPoint) < 10 && CCD1_Right_Point != 0 && myline[0].RPoint != 0
                        && Abs(CCD1_Right_Point - CCD1_HistoryRpoint[3]) < 10 && CCD1_HistoryRpoint[3] != 127
                        && Abs(CCD1_Right_Point - CCD1_HistoryRpoint[2]) < 10 && CCD1_HistoryRpoint[2] != 127
                        && Abs(CCD1_Right_Point - CCD1_HistoryRpoint[1]) < 10 && CCD1_HistoryRpoint[1] != 127
                        && CCD1_Left_Point == 0 && myline[0].LPoint != 0
                        && myline[0].LPoint - CCD1_Left_Point >= 8
                        && myline[0].LPoint >= 5
                        && myline[0].LPoint < 20
                        && CCD1_Right_Point >= 105 && myline[0].RPoint >= 105
                        && CCD1_Right_Point < 120 && myline[0].RPoint < 120
                        && Abs(CCD1_Center_Average - CCD1_CCD1_Center_Average2) < 15)
                    {
                        Zhijiao_Lost_State_You = 1;

                        for (i = CCD1_Right_Point, CCD1_LeftCount = 0; i > CCD1_LEFT_LIMIT; i--)
                        {
                            if (P1[Sv_OutLine(i)] - P1[Sv_OutLine(i - 3)] > Range)
                            {
                                CCD1_LeftCount++;
                                {
                                    if (CCD1_LeftCount > 1)
                                    {
                                        Zhijiao_Lost_State_You = 0;
                                        CCD1_LeftCount = 0;
                                        //setText用户自定义("左下降沿" + CCD1_Left_Point);
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                CCD1_LeftCount = 0;

                            }
                        }
                        if (Zhijiao_Lost_State_You == 1)
                        {
                            goto Over;
                        }


                    }
                    else
                    {
                        Zhijiao_Lost_State_Zuo = 0;
                        Zhijiao_Lost_State_You = 0;
                    }
                }
                #endregion

                #region//判断坡道//
                //上坡
                if (
                       myflag.Flag_Danxianzuo == 0
                    && myflag.Flag_Danxianyou == 0
                    && myflag.Flag_ZhangAi_zuo == 0
                    && myflag.Flag_ZhangAi_you == 0
                    && myflag.state == 0
                    && DanxianSpecial_Out == 0
                    && obstacle.Statusl == COMMON
                    && myflag.Flag_PoDao_Up == 0
                    && myflag.Flag_PoDao_Down == 0
                    && myflag.Flag_Out_PoDaoUp == 0
                    && myflag.LeftWan_Black_state <= 1
                    && myflag.RightWan_Black_state <= 1
                    && myflag.Start_PoDao == 0
                    //&& Abs(nLeftSpeed-nRightSpeed) < 200 //区分弯道
                    //&& nSpeed < (SetSpeed-500)
                    && Start_Jude_Count >= 100
                   )
                {
                    if ((CCD1_Right_Point - CCD1_Left_Point) >= 105
                        && Abs(CCD1_HistoryCenter[9] - 63) < 10
                        && Abs(CCD1_HistoryCenter[8] - 63) < 10
                        && Abs(CCD1_HistoryCenter[7] - 63) < 10
                        && (CCD1_Right_Point != 127 || CCD1_Left_Point != 0)
                        )
                    {
                        if ((CCD1_Right_Point - CCD1_Left_Point) > (CCD1_HistoryRpoint[2] - CCD1_HistoryLpoint[2])
                        && (CCD1_HistoryRpoint[2] - CCD1_HistoryLpoint[2]) > (CCD1_HistoryRpoint[1] - CCD1_HistoryLpoint[1])
                        && CCD1_Right_Point >= CCD1_HistoryRpoint[2]
                        && CCD1_Left_Point <= CCD1_HistoryLpoint[2]
                        && CCD1_HistoryRpoint[2] >= CCD1_HistoryRpoint[1]
                        && CCD1_HistoryLpoint[2] <= CCD1_HistoryLpoint[1]
                        && Abs(CCD1_Left_Point - myline[0].LPoint) < 10
                        && Abs(CCD1_Right_Point - myline[0].RPoint) < 10
                        )
                        {
                            Count_Po_Dao_Up++;
                            if (Count_Po_Dao_Up > 1)
                            {
                                myflag.Flag_PoDao_Up = 1;

                                myflag.Flag_PoDao_Down = 0;

                                myflag.Start_PoDao = 1;

                                setText用户自定义("上坡道");
                                // GPIO_SetBits(PTA,GPIO_Pin_24);
                                goto Over;
                                //                        obstacle.Statusl = Po_Dao_Up;

                            }
                        }
                        else
                        {
                            Count_Po_Dao_Up = 0;
                        }
                        // setText用户自定义("Count_Po_Dao_Up" + Count_Po_Dao_Up);
                    }
                    else
                    {
                        Count_Po_Dao_Up = 0;
                    }

                }
                //下坡
                if (CCD1_Right_Point != 127 && CCD1_Left_Point != 0
                    && myflag.Flag_Danxianzuo == 0
                    && myflag.Flag_Danxianyou == 0
                    && myflag.Flag_ZhangAi_zuo == 0
                    && myflag.Flag_ZhangAi_you == 0
                    && myflag.state == 0
                    && DanxianSpecial_Out == 0
                    && myflag.LeftWan_Black_state <= 1
                    && myflag.RightWan_Black_state <= 1
                    && obstacle.Statusl == COMMON
                    && myflag.Flag_PoDao_Down == 0
                    && myflag.Start_PoDao == 1
                    //            &&  myflag.Flag_Out_PoDaoUp == 1
                    //&& Abs(nLeftSpeed-nRightSpeed) < 200 //区分弯道

                    //&& Abs(Center_Temp-myline[0].Center)<8
                    && Abs(Center_Temp - (CCD1_Right_Point + CCD1_Left_Point) / 2) < 15
                    && Start_Jude_Count >= 100
               )
                {
                    if ((CCD1_Right_Point - CCD1_Left_Point) <= 85 && CCD1_HistoryLpoint[2] != 0 && CCD1_HistoryRpoint[2] != 127 && CCD1_Right_Point != 127 && CCD1_Left_Point != 0)
                    {
                        if ((CCD1_Right_Point - CCD1_Left_Point) < (CCD1_HistoryRpoint[2] - CCD1_HistoryLpoint[2])
                        && CCD1_Right_Point <= CCD1_HistoryRpoint[2]
                        && CCD1_Left_Point >= CCD1_HistoryLpoint[2]
                        && CCD1_HistoryRpoint[2] <= CCD1_HistoryRpoint[1]
                        && CCD1_HistoryLpoint[2] >= CCD1_HistoryLpoint[1]
                        )
                        {
                            Count_Po_Dao_Down++;

                            if (Count_Po_Dao_Down > 1 && myflag.Flag_PoDao_Down == 0)
                            {
                                myflag.Flag_PoDao_Down = 1;

                                myflag.Flag_PoDao_Up = 0;

                                myflag.Flag_Out_PoDaoUp = 0;

                                myflag.Start_PoDao = 2;

                                //                    obstacle.Statusl = Po_Dao_Up;

                                //GPIO_SetBits(PTA, GPIO_Pin_24);
                                setText用户自定义("下坡道");
                                goto Over;
                                ;
                            }

                        }
                        else
                        {
                            Count_Po_Dao_Down = 0;
                        }
                        //setText用户自定义("Count_Po_Dao_Down" + Count_Po_Dao_Down);
                    }
                    else
                    {
                        Count_Po_Dao_Down = 0;
                    }



                }
                #endregion

            }
            #endregion

        Over: ;
            if (yuzhi < 20)
            {
                yuzhi = yuzhi_last;
            }
            if (myflag.state != 0)
            {
                myflag.LeftWan_Black_state = 0;
                myflag.RightWan_Black_state = 0;
                Buxian_Value_zuo = 0;
                Buxian_Value_you = 0;

            }


            //    if(myflag.LeftWan_Black_state == 6)
            //    {
            //        if(myflag.state > 1 || (CCD1_Right_Point - CCD1_Left_Point) > 80 )
            //        {
            //            myflag.LeftWan_Black_state = 0;
            //        }
            //    }
            //    if(myflag.RightWan_Black_state == 6)
            //    {
            //        if(myflag.state > 1 || (CCD1_Right_Point - CCD1_Left_Point) > 80 )
            //        {
            //            myflag.RightWan_Black_state = 0;
            //        }
            //    }

            #region//直接判断直角处理//
            if (Zhijiao_Lost_State_Zuo == 1)
            {
                if (Abs(CCD1_Left_Point - myline[0].LPoint) < 5 && CCD1_Left_Point != 0 && myline[0].LPoint != 0
                    && CCD1_Right_Point == 127
                    && CCD1_Left_Point >= 5 && myline[0].LPoint >= 5
                    && CCD1_Left_Point < 20 && myline[0].LPoint < 20)
                {
                    Zhijiao_Lost_Count_Zuo++;
                    if (Zhijiao_Lost_Count_Zuo > 1)
                    {
                        myflag.state = 3;
                        Zhijiao_Lost_State_Zuo = 0;
                        // GPIO_SetBits(PTA,GPIO_Pin_24);
                        myflag.Flag_Temp_you = 1;
                        Zhijiao_Lost_Count_Zuo = 0;
                        setText用户自定义("*****************************直角折角you*****************************");

                    }
                }
                else
                {
                    Zhijiao_Lost_Count_Zuo = 0;
                    Zhijiao_Lost_State_Zuo = 0;
                }
            }

            if (Zhijiao_Lost_State_You == 1)
            {
                if (Abs(CCD1_Right_Point - myline[0].RPoint) < 5 && CCD1_Right_Point != 0 && myline[0].RPoint != 0
                    && CCD1_Left_Point == 0
                    && CCD1_Right_Point < 120 && myline[0].RPoint < 120
                    && CCD1_Right_Point >= 105 && myline[0].RPoint >= 105)
                {
                    Zhijiao_Lost_Count_You++;
                    if (Zhijiao_Lost_Count_You > 1)
                    {
                        myflag.state = 3;
                        // GPIO_SetBits(PTA,GPIO_Pin_24);
                        Zhijiao_Lost_State_You = 0;
                        myflag.Flag_Temp_zuo = 1;
                        Zhijiao_Lost_Count_You = 0;
                        setText用户自定义("*****************************直角折角zuo*****************************");

                    }
                }
                else
                {
                    Zhijiao_Lost_Count_You = 0;
                    Zhijiao_Lost_State_You = 0;
                }
            }
            #endregion

            #region//单线状态赋值//
            if (myflag.Flag_Danxianyou == 1)
            {
                obstacle.Statusl = DAN_XIAN;

                //        CCD1_LeftBool = 0;

                //        CCD1_Left_Point = 0;
                /*清除直角，单线，十字判断*/
                if (myflag.state != 5)
                {
                    myflag.state = 0;//
                }
                Buxian_Value_zuo = 0;

                Buxian_Value_you = 0;

                myflag.Flag_Temp_zuo = 0;

                myflag.Flag_Temp_you = 0;

                myflag.Flag_ZhangAi_zuo = 0;

                myflag.Flag_ZhangAi_you = 0;

                myflag.ShiZi_Flag = 1;

                Danxian_Center = CCD1_Right_Point;
                /******************/

            }
            if (myflag.Flag_Danxianzuo == 1)
            {
                obstacle.Statusl = DAN_XIAN;

                //        CCD1_RightBool = 0;

                //        CCD1_Right_Point = 127;
                /*清除直角，单线，十字判断*/
                if (myflag.state != 5)
                {
                    myflag.state = 0;//
                }
                Buxian_Value_zuo = 0;

                Buxian_Value_you = 0;

                myflag.Flag_Temp_zuo = 0;

                myflag.Flag_Temp_you = 0;

                myflag.Flag_ZhangAi_zuo = 0;

                myflag.Flag_ZhangAi_you = 0;

                myflag.ShiZi_Flag = 1;

                Danxian_Center = CCD1_Left_Point;
                /******************/

            }
            #endregion

            setText用户自定义("Statusl" + obstacle.Statusl);
            setText用户自定义("state" + myflag.state);
            setText用户自定义("Start_PoDao" + myflag.Start_PoDao);

            #region//判断要进入直角后状态机处理//
            //丢线情况的记录
            if (obstacle.Statusl == ZHI_JIAO && myflag.state == 0)//
            {

                myflag.state = 1;//进入直角

                myflag.Flag_ZhangAi_you = 0;

                myflag.Flag_ZhangAi_zuo = 0;

                myflag.Flag_Black = 1;

                myflag.Flag_Temp = 0;

                myflag.Flag_Temp1 = 0;
            }
            else if (myflag.state == 1)//延时
            {

                if (CCD1_Left_Point != 0 && CCD1_Right_Point != 127)
                {
                    myflag.Flag_Temp1++;
                }
                else
                {
                    myflag.Flag_Temp = 0;
                }
                if (myflag.Flag_Temp1 > 1)
                {
                    myflag.Flag_Temp++;
                }
                if (myflag.Flag_Temp > YangShi)
                {
                    {
                        myflag.Flag_Temp = 0;

                        myflag.Flag_Temp1 = 0;

                        myflag.state = 2;
                    }
                }

                myflag.count1++;

                if (myflag.count1 >= 100)   //一段时间内没检测到转弯，自动清零
                {
                    myflag.state = 5;

                    myflag.count1 = 0;
                }

            }
            else if (myflag.state == 2)//如果一边丢先就打角
            {
                myflag.Flag_Temp = 0;

                myflag.Flag_Temp1 = 0;

                //右边丢线
                if (CCD1_RightBool == 0 && CCD1_LeftBool == 1)
                {
                    Right_Lost_Count++;

                }
                else
                {
                    Right_Lost_Count = 0;
                }

                if (CCD1_RightBool == 1 && CCD1_LeftBool == 0)
                {
                    Left_Lost_Count++;
                }
                else
                {
                    Left_Lost_Count = 0;
                }

                if (Right_Lost_Count > DaJiao_Limited)
                {
                    myflag.Flag_Temp_you = 1;  //要向右转弯
                    myflag.Flag_Black = 0;
                    myflag.count = 0;   //大于0的数，防止过直角后判断错误
                    myflag.Flag_ZhiJiao = 1;//进入直角弯判断，给出标志位，让陀螺仪微分项变化
                    //					// GPIO_SetBits(PTA,GPIO_Pin_24);
                    myflag.state = 3;

                    Left_Lost_Count = 0;
                    Right_Lost_Count = 0;
                }
                else if (Left_Lost_Count > DaJiao_Limited)
                {
                    myflag.Flag_Temp_zuo = 1;  //要向左转弯
                    myflag.Flag_Black = 0;
                    myflag.count = 0;  //大于0的数，防止过直角后判断错误
                    myflag.Flag_ZhiJiao = 1;  //进入直角弯判断，给出标志位，让陀螺仪微分项变化
                    //					// GPIO_SetBits(PTA,GPIO_Pin_24);
                    myflag.state = 3;

                    Left_Lost_Count = 0;
                    Right_Lost_Count = 0;


                }
                myflag.count++;

                if (myflag.count >= 200)   //一段时间内没检测到转弯，自动清零
                {
                    myflag.state = 5;

                    myflag.Flag_ZhiJiao_Jiansu = 0;

                    myflag.Flag_Black = 0;

                    myflag.count = 0;

                    myflag.ZhiJiao_Pass_Flag_Left = 0;//标记增大水平抑制

                    myflag.ZhiJiao_Pass_Flag_Right = 0;//标记增大水平抑制
                }
            }
            else if (myflag.state == 3)
            {
                myflag.Flag_Temp = 0;

                myflag.Flag_Temp1 = 0;

                if (myflag.Flag_Temp_zuo == 0 && myflag.Flag_Temp_you == 0)
                {
                    myflag.state = 4;

                }
            }
            else if (myflag.state == 4)//等待第二条黑线
            {
                myflag.Flag_Temp = 0;

                myflag.Flag_Temp1 = 0;

                myflag.count++;

                if (myflag.ZhiJiao_Pass_Flag_Left == 1 || myflag.ZhiJiao_Pass_Flag_Right == 1)
                {
                    ZhiJiao_Out_Count++;
                }
                if (ZhiJiao_Out_Count > 15)
                {
                    myflag.ZhiJiao_Pass_Flag_Left = 0;//标记增大水平抑制

                    myflag.ZhiJiao_Pass_Flag_Right = 0;//标记增大水平抑制

                    ZhiJiao_Out_Count = 0;
                }
                if (myflag.count >= 5)
                {
                    if (obstacle.Statusl == ZHI_JIAO)
                    {
                        myflag.state = 5;

                        //          myflag.Flag_ZhiJiao_Jiansu = 0;

                        myflag.Flag_Black = 0;

                        myflag.count = 0;

                        myflag.ZhiJiao_Pass_Flag_Left = 0;//标记增大水平抑制

                        myflag.ZhiJiao_Pass_Flag_Right = 0;//标记增大水平抑制
                    }
                }

                if (myflag.count >= 100)   //一段时间内没检测到黑条，自动清零
                {
                    myflag.state = 5;

                    //          myflag.Flag_ZhiJiao_Jiansu = 0;

                    myflag.Flag_Black = 0;

                    myflag.count = 0;

                    myflag.ZhiJiao_Pass_Flag_Left = 0;//标记增大水平抑制

                    myflag.ZhiJiao_Pass_Flag_Right = 0;//标记增大水平抑制
                }

            }
            else if (myflag.state == 5)
            {
                myflag.Flag_Temp++;
                if (myflag.Flag_Temp > YangShi + 5)
                {
                    myflag.state = 0;

                    myflag.Flag_Temp = 0;

                }

            }

            #endregion

            #region//对左右点进行赋值//

            //setText用户自定义("CCD1_Left_Point" + CCD1_Left_Point);

            //setText用户自定义("CCD1_Right_Point" + CCD1_Right_Point);

            myline[1].LPoint = CCD1_Left_Point;

            myline[1].RPoint = CCD1_Right_Point;

            #endregion

            #region//障碍处理//

            if (myflag.Flag_ZhangAi_zuo == 1 || myflag.Flag_ZhangAi_you == 1 && myflag.t == 0)
            {
                if (nSpeed >= 1300)
                {
                    Zhangai_ChangShu_Temp = Zhangai_ChangShu;
                }
                else if (nSpeed >= 1200 && nSpeed < 1300)
                {
                    Zhangai_ChangShu_Temp = Zhangai_ChangShu + 4;
                }
                else if (nSpeed >= 1000 && nSpeed < 1200)
                {
                    Zhangai_ChangShu_Temp = Zhangai_ChangShu + 10;
                }
                else if (nSpeed < 1000)
                {
                    Zhangai_ChangShu_Temp = Zhangai_ChangShu + 15;
                }
            }
            if (myflag.Flag_ZhangAi_zuo == 1) //左障碍处理
            {
                CCD1_Center_Line = 52; //直接改基准线
                myflag.Flag_ZhangAi = 0;
                myflag.t++;
                if (myflag.t > Zhangai_ChangShu_Temp)
                {
                    myflag.t = 0;
                    myflag.Flag_ZhangAi_zuo = 0;
                    CCD1_Center_Line = Center_Temp;
                    myflag.Flag_ZhangAi = 1;

                    CCD1_LeftBool_Last = 0;

                    CCD1_RightBool_Last = 0;
                }
                setText用户自定义("左障碍处理");
            }
            else if (myflag.Flag_ZhangAi_you == 1)//右障碍处理
            {

                CCD1_Center_Line = 78; //直接改基准线
                myflag.Flag_ZhangAi = 0;
                myflag.t++;
                if (myflag.t > Zhangai_ChangShu_Temp)
                {
                    myflag.t = 0;
                    myflag.Flag_ZhangAi_you = 0;
                    CCD1_Center_Line = Center_Temp;
                    myflag.Flag_ZhangAi = 1;
                    CCD1_LeftBool_Last = 0;

                    CCD1_RightBool_Last = 0;
                }
                setText用户自定义("右障碍处理");
            }
            else
            {
                CCD1_Center_Line = Center_Temp;
            }
            #endregion

            #region//计算中线//
            //排除跳变,十字出现较多
            if (Abs(myline[1].LPoint - myline[0].LPoint) > 30
                && myline[1].LPoint != 0
                && myline[0].LPoint == 0
                && myflag.Flag_Danxianzuo == 0
                && myflag.Flag_ZhangAi_zuo == 0
                && DanxianSpecial_Out == 0
                      && Flag_White == 0
                && myflag.LeftWan_Black_state <= 1
                && myflag.RightWan_Black_state <= 1
                && myflag.state != 3
                )
            {
                myline[1].LPoint = 0;
                CCD1_LeftBool = 0;
                //                setText用户自定义("发生非元素跳变-左");
            }
            if (Abs(myline[1].RPoint - myline[0].RPoint) < -30
                 && myline[1].RPoint != 127
                && myline[0].RPoint == 127
                && myflag.Flag_Danxianyou == 0
                && myflag.Flag_ZhangAi_you == 0
                && DanxianSpecial_Out == 0
                        && Flag_White == 0
                && myflag.LeftWan_Black_state <= 1
                && myflag.RightWan_Black_state <= 1
                && myflag.state != 3
                )
            {
                myline[1].RPoint = 127;
                CCD1_RightBool = 0;
                //                setText用户自定义("发生非元素跳变-右");
            }

            if (  myflag.Flag_ZhangAi_zuo == 0
                && myflag.Flag_ZhangAi_you == 0
                && myflag.Flag_Danxianzuo == 0
                && myflag.Flag_Danxianyou == 0
                && myflag.Start_PoDao == 0
                && myflag.Flag_ZhangAi == 0
                && DanxianSpecial_Out == 0
                && myflag.state == 0
                && myflag.LeftWan_Black_state == 0
                && myflag.RightWan_Black_state == 0
                )
            {
                if (CCD1_RightBool == 1 && CCD1_LeftBool == 1)
                {
                    Forward_White = 0;
                    First_Shizhi = 0;
                    myflag.ShiZi_First_Count = 0;
                    if (CCD1_RightBool_Last == 1 && CCD1_LeftBool_Last == 0)
                    {
                        myline[1].Center = Sv_OutLine_Center((int)(myline[0].Center + (myline[1].RPoint - myline[0].RPoint) * 1.5));
                    }
                    else if (CCD1_RightBool_Last == 0 && CCD1_LeftBool_Last == 1)
                    {
                        myline[1].Center = Sv_OutLine_Center((int)(myline[0].Center + (myline[1].LPoint - myline[0].LPoint) * 1.5));
                    }
                    else if (CCD1_RightBool_Last == 0 && CCD1_LeftBool_Last == 0)
                    {
                        myline[1].Center = Sv_OutLine_Center(myline[1].RPoint + myline[1].LPoint) / 2;// - ((myline[1].RPoint + myline[1].LPoint) / 2 - myline[0].Center) / 2;
                    }
                    else
                    {
                        myline[1].Center = Sv_OutLine_Center(myline[1].RPoint + myline[1].LPoint) / 2;
                    }
                }
                else if (CCD1_RightBool == 1 && CCD1_LeftBool == 0)
                {
                    Forward_White = 0;
                    First_Shizhi = 0;
                    myflag.ShiZi_First_Count = 0;
                    if (myline[1].RPoint - Center_Help >= 0)
                    {
                        if (CCD1_RightBool_Last == 1 && CCD1_LeftBool_Last == 0)
                        {
                            myline[1].Center = Sv_OutLine_Center((int)(myline[0].Center + (myline[1].RPoint - myline[0].RPoint) * 1.5));
                        }
                        else if (CCD1_RightBool_Last == 1 && CCD1_LeftBool_Last == 1)
                        {
                            myline[1].Center = Sv_OutLine_Center((int)(myline[0].Center + (myline[1].RPoint - myline[0].RPoint) * 1.5));
                        }
                        else if (CCD1_RightBool_Last == 0 && CCD1_LeftBool_Last == 0)
                        {
                            myline[1].Center = Sv_OutLine_Center(myline[1].RPoint - Center_Help);// -(myline[1].RPoint - Center_Help - myline[0].Center) / 2;
                        }
                        else
                        {
                            myline[1].Center = Sv_OutLine_Center(myline[1].RPoint - Center_Help);
                        }
                        Buxian_Value_zuo = 0;
                    }
                    else
                    {
                        if (CCD1_RightBool_Last == 1 && CCD1_LeftBool_Last == 0)
                        {
                            Buxian_Value_zuo += myline[0].RPoint - myline[1].RPoint;
                            myline[1].Center = Sv_OutLine_Center((int)(myline[0].Center + (myline[1].RPoint - myline[0].RPoint) * 1.5));
                        }
                        else if (CCD1_RightBool_Last == 1 && CCD1_LeftBool_Last == 1)
                        {
                            myline[1].Center = Sv_OutLine_Center((int)(myline[0].Center + (myline[1].RPoint - myline[0].RPoint) * 1.5));
                            Buxian_Value_zuo = 0;
                        }
                        else if (CCD1_RightBool_Last == 0 && CCD1_LeftBool_Last == 0)
                        {
                            myline[1].Center = Sv_OutLine_Center(myline[1].RPoint - Center_Help);// -(myline[1].RPoint - Center_Help - myline[0].Center) / 2;
                            Buxian_Value_zuo = 0;
                        }
                        else
                        {
                            myline[1].Center = Sv_OutLine_Center(myline[1].RPoint - Center_Help);
                            Buxian_Value_zuo = 0;
                        }
                    }

                }

                else if (CCD1_RightBool == 0 && CCD1_LeftBool == 1)
                {
                    Forward_White = 0;
                    First_Shizhi = 0;
                    myflag.ShiZi_First_Count = 0;
                    if (myline[1].LPoint + Center_Help <= 127)
                    {
                        if (CCD1_RightBool_Last == 0 && CCD1_LeftBool_Last == 1)
                        {
                            myline[1].Center = Sv_OutLine_Center((int)(myline[0].Center + (myline[1].LPoint - myline[0].LPoint) * 1.2));
                        }
                        else if (CCD1_RightBool_Last == 1 && CCD1_LeftBool_Last == 1)
                        {
                            myline[1].Center = Sv_OutLine_Center((int)(myline[0].Center + (myline[1].LPoint - myline[0].LPoint) * 1.2));
                        }
                        else if (CCD1_RightBool_Last == 0 && CCD1_LeftBool_Last == 0)
                        {
                            myline[1].Center = Sv_OutLine_Center(myline[1].LPoint + Center_Help);// -(myline[1].LPoint + Center_Help - myline[0].Center) / 2;
                        }
                        else
                        {
                            myline[1].Center = Sv_OutLine_Center(myline[1].LPoint + Center_Help);
                        }
                        Buxian_Value_zuo = 0;
                    }
                    else
                    {
                        if (CCD1_RightBool_Last == 0 && CCD1_LeftBool_Last == 1)
                        {
                            Buxian_Value_you += myline[1].LPoint - myline[0].LPoint;
                            myline[1].Center = Sv_OutLine_Center((int)(myline[0].Center + (myline[1].LPoint - myline[0].LPoint) * 1.2));
                        }
                        else if (CCD1_RightBool_Last == 1 && CCD1_LeftBool_Last == 1)
                        {
                            myline[1].Center = Sv_OutLine_Center((int)(myline[0].Center + (myline[1].LPoint - myline[0].LPoint) * 1.2));
                            Buxian_Value_zuo = 0;
                        }
                        else if (CCD1_RightBool_Last == 0 && CCD1_LeftBool_Last == 0)
                        {
                            myline[1].Center = Sv_OutLine_Center(myline[1].LPoint + Center_Help);// -(myline[1].LPoint + Center_Help - myline[0].Center) / 2;
                            Buxian_Value_zuo = 0;
                        }
                        else
                        {
                            myline[1].Center = Sv_OutLine_Center(myline[1].LPoint + Center_Help);
                            Buxian_Value_zuo = 0;
                        }
                    }
                }
                else if (CCD1_RightBool == 0 && CCD1_LeftBool == 0)
                {
                    Forward_White = 1;

                    myline[1].Center = 63;
                    //            if (First_Shizhi == 0)
                    //            {
                    //                First_Shizhi = myline[0].Center;
                    //               
                    //            }
                    ////                    setText用户自定义("First_Shizhi" + First_Shizhi);

                    //            if (Abs(63 - First_Shizhi) >= 10 && Abs(63 - First_Shizhi) <= 20)
                    //            { 
                    //                if (myflag.ShiZi_First_Count == 0)
                    //                {
                    //                    ShiZi_FirstCenter = First_Shizhi;

                    //                    if (ShiZi_FirstCenter > 63)
                    //                    {
                    //                        ShiZi_FirstCenter =(int) (63 + (ShiZi_FirstCenter - 63) * 1.5);
                    //                    }
                    //                    else if (ShiZi_FirstCenter < 63)
                    //                    {
                    //                        ShiZi_FirstCenter = (int)(63 - (-ShiZi_FirstCenter + 63) * 1.5);

                    //                    }
                    //                }
                    //                else
                    //                {
                    //                    if (ShiZi_FirstCenter > 63)
                    //                    {
                    //                        ShiZi_FirstCenter =(int)( 63 + (ShiZi_FirstCenter - 63) * 0.8);
                    //                    }
                    //                    else if (ShiZi_FirstCenter < 63)
                    //                    {
                    //                        ShiZi_FirstCenter = (int)(63 - (-ShiZi_FirstCenter + 63) * 0.8);

                    //                    }
                    //                }
                    //                myflag.ShiZi_First_Count = 1;

                    //                myline[1].Center = ShiZi_FirstCenter;

                    ////                        setText用户自定义("十字处理1"+myline[1].Center);
                    //                GPIO_SetBits(PTA, GPIO_Pin_24);

                    //            }
                    //            else if (Abs(63 - First_Shizhi) > 20)
                    //            {
                    //                if (myflag.ShiZi_First_Count == 0)
                    //                {
                    //                    ShiZi_FirstCenter = First_Shizhi;

                    //                    if (ShiZi_FirstCenter > 63)
                    //                    {
                    //                        ShiZi_FirstCenter = (int)(63 + (ShiZi_FirstCenter - 63) * 0.8);
                    //                    }
                    //                    else if (ShiZi_FirstCenter < 63)
                    //                    {
                    //                        ShiZi_FirstCenter = (int)(63 - (-ShiZi_FirstCenter + 63) * 0.8);

                    //                    }
                    //                }
                    //                else
                    //                {
                    //                    if (ShiZi_FirstCenter > 63)
                    //                    {
                    //                        ShiZi_FirstCenter = (int)(63 + (ShiZi_FirstCenter - 63) * 0.6);
                    //                    }
                    //                    else if (ShiZi_FirstCenter < 63)
                    //                    {
                    //                        ShiZi_FirstCenter = (int)(63 - (-ShiZi_FirstCenter + 63) * 0.6);

                    //                    }
                    //                }
                    ////                        setText用户自定义("十字处理2" + myline[1].Center);
                    //                myflag.ShiZi_First_Count = 1;
                    //                
                    //                GPIO_SetBits(PTA, GPIO_Pin_24);

                    //                myline[1].Center = ShiZi_FirstCenter;
                    //            }
                    //            else
                    //            {
                    //                myline[1].Center = 63 - (63 - myline[0].Center) / 2;
                    //            }


                }

                else
                {
                    myline[1].Center = myline[0].Center;
                }
            }
            else
            {
                if (CCD1_RightBool == 1 && CCD1_LeftBool == 1 && myflag.Flag_Danxianzuo == 0 && myflag.Flag_Danxianyou == 0 && myflag.Flag_Temp_zuo == 0 && myflag.Flag_Temp_you == 0)
                {
                    myline[1].Center = Sv_OutLine_Center((myline[1].RPoint + myline[1].LPoint) >> 1);
                    Forward_White = 0;
                }
                else if (CCD1_RightBool == 1 && CCD1_LeftBool == 0 && myflag.Flag_Danxianzuo == 0 && myflag.Flag_Danxianyou == 0 && myflag.Flag_Temp_zuo == 0 && myflag.Flag_Temp_you == 0)
                {
                    Forward_White = 0;
                    if (myline[1].RPoint - Center_Help >= 0)
                    {
                        myline[1].Center = Sv_OutLine_Center(myline[1].RPoint - Center_Help);
                        Buxian_Value_zuo = 0;
                    }
                    else
                    {
                        if (CCD1_RightBool_Last == 1 && CCD1_LeftBool_Last == 0)
                        {
                            Buxian_Value_zuo += myline[0].RPoint - myline[1].RPoint;
                            myline[1].Center = Sv_OutLine_Center(myline[1].RPoint - Center_Help);
                        }
                        else if (CCD1_RightBool_Last == 0 && CCD1_LeftBool_Last == 1)
                        {
                            myline[1].Center = 63;
                            Buxian_Value_zuo = 0;
                        }
                        else
                        {
                            myline[1].Center = Sv_OutLine_Center(myline[1].RPoint - Center_Help);
                            Buxian_Value_zuo = 0;
                        }
                    }

                }

                else if (CCD1_RightBool == 0 && CCD1_LeftBool == 1 && myflag.Flag_Danxianzuo == 0 && myflag.Flag_Danxianyou == 0 && myflag.Flag_Temp_zuo == 0 && myflag.Flag_Temp_you == 0)
                {
                    Forward_White = 0;
                    if (myline[1].LPoint + Center_Help <= 127)
                    {
                        myline[1].Center = myline[1].LPoint + Center_Help;
                        Buxian_Value_you = 0;
                    }
                    else
                    {
                        if (CCD1_RightBool_Last == 0 && CCD1_LeftBool_Last == 1)
                        {
                            Buxian_Value_you += myline[1].LPoint - myline[0].LPoint;
                            myline[1].Center = Sv_OutLine_Center(myline[1].LPoint + Center_Help);
                        }
                        else if (CCD1_RightBool_Last == 0 && CCD1_LeftBool_Last == 1)
                        {
                            myline[1].Center = 63;
                            Buxian_Value_you = 0;
                        }
                        else
                        {
                            myline[1].Center = Sv_OutLine_Center(myline[1].LPoint + Center_Help);
                            Buxian_Value_you = 0;
                        }
                    }
                }
                else if (CCD1_RightBool == 0 && CCD1_LeftBool == 0 && myflag.Flag_Danxianzuo == 0 && myflag.Flag_Danxianyou == 0 && myflag.Flag_Temp_zuo == 0 && myflag.Flag_Temp_you == 0)
                {
                    Forward_White = 1;
                    myline[1].Center = 63;
                }
                else
                {
                    myline[1].Center = myline[0].Center;
                }

            }
            if (myflag.Flag_Danxianyou == 1)
            {
                myline[1].Center = myline[1].RPoint;
            }

            else if (myflag.Flag_Danxianzuo == 1)
            {
                myline[1].Center = myline[1].LPoint;
            }

            if (CCD1_RightBool == 0 && CCD1_LeftBool == 0 && myflag.state != 5 && myflag.Start_PoDao == 0 && DanxianSpecial_Out == 0 && myflag.Flag_Danxianzuo == 0 && myflag.Flag_Danxianyou == 0)
            {
                White_Count++;
                if (White_Count > 2)
                {
                    Flag_White = 1;
                }
            }
            //        if (   myflag.Flag_Danxianzuo == 0 && myflag.Flag_Danxianyou == 0
            //            && myflag.Flag_Temp_zuo == 0 && myflag.Flag_Temp_you == 0
            //            && myflag.Flag_ZhangAi_zuo == 0 && myflag.Flag_ZhangAi_you == 0)
            //        {
            //            if(Abs(myline[1].Center - myline[0].Center)>=15)
            //            {
            //              myline[1].Center = myline[0].Center;      
            //            }
            //             
            //        }           

            //出单线圆滑补线
            //        if (DanxianSpecial_Out != 0)
            //        {
            //            if (myline[0].Center >63)
            //            {
            //                myline[1].Center = myline[0].Center +( (CCD1_HistoryCenter[9] - CCD1_HistoryCenter[5])/4);
            //            }
            //            else 
            //            {
            //                myline[1].Center = myline[0].Center - ((CCD1_HistoryCenter[9] - CCD1_HistoryCenter[5])/4);
            //            }
            //        }
            //中线防跳变
            //     if(    myflag.Flag_Temp_zuo == 0 
            //         && myflag.Flag_Temp_you == 0
            //         && myflag.Flag_ZhangAi_zuo == 0
            //         && myflag.Flag_ZhangAi_you == 0
            //         && myflag.Flag_Danxianyou == 0
            //         && myflag.Flag_Danxianzuo == 0
            //         && myflag.Start_PoDao == 0
            //         && DanxianSpecial_Out == 0
            //         && myflag.LeftWan_Black_state <= 1
            //         && myflag.LeftWan_Black_state <= 1    
            //      )
            //      {
            //        if(myline[1].Center - myline[0].Center>8)
            //        {
            //            myline[1].Center = myline[0].Center+8;
            //        }
            //        else if(myline[0].Center - myline[1].Center>8)
            //        {
            //            myline[1].Center = myline[0].Center-8;
            //        }
            //      }
            #endregion

            setText用户自定义("Center" + myline[1].Center);

            #region//障碍排错
            if (myflag.Flag_ZhangAi_zuo == 1 || myflag.Flag_ZhangAi_you == 1)
            {
                if (Abs(myline[1].Center - myline[0].Center) > 25)
                {
                    myflag.Flag_ZhangAi_zuo = 0;
                    myflag.Flag_ZhangAi_you = 0;
                }
            }
            #endregion

            #region//十字处理//
            if (myflag.ShiZi_Flag == 0)
            {
                if (CCD1_Left_Point != 0 || CCD1_Right_Point != 127)//全有
                {
                    myflag.ShiZi_Flag = 1;
                }
            }
            else if (myflag.ShiZi_Flag == 1)//状态0  判断进入十字  判断无线
            {
                if (
                       CCD1_Left_Point == 0 && CCD1_Right_Point == 127
                    && myflag.Flag_Danxianzuo == 0
                    && myflag.Flag_Danxianyou == 0
                    && myflag.Flag_ZhangAi_zuo == 0
                    && myflag.Flag_ZhangAi_you == 0
                    && myflag.Start_PoDao == 0
                    && myflag.state == 0
                    && DanxianSpecial_Out == 0
                    && obstacle.Statusl == COMMON
                   )//全丢线
                {
                    ShiZi_LostCount++;
                    if (ShiZi_LostCount > 1)//2场丢线
                    {
                        myflag.ShiZi_Flag = 2;

                        ShiZi_LostCount = 0;

                    }
                }
                else
                {
                    ShiZi_LostCount = 0;
                }
                if (CCD1_Left_Point != 0 || CCD1_Right_Point != 127)
                {
                    ShiZi_FirstCenter = myline[1].Center;
                }
            }
            else if (myflag.ShiZi_Flag == 2)//十字中的有线
            {
                if (CCD1_Left_Point != 0 || CCD1_Right_Point != 127)
                {
                    ShiZi_WaitCount++;
                    if (ShiZi_WaitCount > 1)//2场等待
                    {
                        myflag.ShiZi_Flag = 3;

                        ShiZi_WaitCount = 0;

                    }
                }

            }
            else if (myflag.ShiZi_Flag == 3)//等待出十字
            {
                if (CCD1_Left_Point == 0 && CCD1_Right_Point == 127)//全丢线
                {
                    ShiZi_LostCount++;
                    if (ShiZi_LostCount > 1)//二场丢线
                    {
                        myflag.ShiZi_Flag = 4;
                        ShiZi_LostCount = 0;

                    }
                }
                else
                {
                    ShiZi_LostCount = 0;
                }
                if (CCD1_Left_Point != 0 || CCD1_Right_Point != 127)
                {
                    ShiZi_LastCenter = myline[1].Center;//记录中线
                }
            }
            else if (myflag.ShiZi_Flag == 4)
            {
                if (CCD1_Left_Point != 0 && CCD1_Right_Point != 127)//已经出了十字
                {
                    myflag.ShiZi_Flag = 1;
                    ShiZi_LostCount = 0;
                    ShiZi_LastCenter = 0;
                }

            }
            if (myflag.ShiZi_Flag == 2)//十字
            {
                if (CCD1_Left_Point == 0 && CCD1_Right_Point == 127
                    && myflag.Flag_Danxianzuo == 0
                    && myflag.Flag_Danxianyou == 0
                    && myflag.Flag_ZhangAi_zuo == 0
                    && myflag.Flag_ZhangAi_you == 0
                    && myflag.state == 0
                    && DanxianSpecial_Out == 0
                    && myflag.Start_PoDao == 0
                    && obstacle.Statusl == COMMON)
                {
                    if (myflag.ShiZi_First_Count == 0)
                    {
                        if (ShiZi_FirstCenter > 63)
                        {
                            ShiZi_FirstCenter = 63 + (ShiZi_FirstCenter - 63) * 2;
                        }
                        else if (ShiZi_FirstCenter < 63)
                        {
                            ShiZi_FirstCenter = 63 - (-ShiZi_FirstCenter + 63) * 2;

                        }
                    }
                    else
                    {
                        if (ShiZi_FirstCenter > 63)
                        {
                            ShiZi_FirstCenter = 63 + (ShiZi_FirstCenter - 63) / 2;
                        }
                        else if (ShiZi_FirstCenter < 63)
                        {
                            ShiZi_FirstCenter = 63 - (-ShiZi_FirstCenter + 63) / 2;

                        }
                    }
                    myflag.ShiZi_First_Count = 1;
                    myline[1].Center = ShiZi_FirstCenter;
                    // GPIO_SetBits(PTA,GPIO_Pin_24);
                    //            SendPara[28] = myline[1].Center;

                }

            }
            if (myflag.ShiZi_Flag == 4)//十字
            {
                if (CCD1_Left_Point == 0 && CCD1_Right_Point == 127
                    && myflag.Flag_Danxianzuo == 0
                    && myflag.Flag_Danxianyou == 0
                    && myflag.Flag_ZhangAi_zuo == 0
                    && myflag.Flag_ZhangAi_you == 0
                    && myflag.state == 0
                    && DanxianSpecial_Out == 0
                    && obstacle.Statusl == COMMON)
                {
                    if (myflag.ShiZi_Last_Count == 0)
                    {
                        if (ShiZi_LastCenter > 63)
                        {
                            ShiZi_LastCenter = (int)(63 + (ShiZi_LastCenter - 63) * 1.5);
                        }
                        else if (ShiZi_LastCenter < 63)
                        {
                            ShiZi_LastCenter = (int)(63 - (-ShiZi_LastCenter + 63) * 1.5);

                        }
                    }
                    else
                    {
                        if (ShiZi_LastCenter > 63)
                        {
                            ShiZi_LastCenter = 63 + (ShiZi_LastCenter - 63) / 2;
                        }
                        else if (ShiZi_LastCenter < 63)
                        {
                            ShiZi_LastCenter = 63 - (-ShiZi_LastCenter + 63) / 2;

                        }
                    }
                    myflag.ShiZi_Last_Count = 1;
                    // GPIO_SetBits(PTA,GPIO_Pin_24);
                    myline[1].Center = ShiZi_LastCenter;

                    //            SendPara[29] = ShiZi_LastCenter;

                }
            }

            if (myflag.ShiZi_Flag > 1)
            {
                myflag.Flag_Shizhitime++;
            }
            if (myflag.Flag_Shizhitime > 400)
            {
                myflag.Flag_Shizhitime = 0;
                myflag.ShiZi_Flag = 1;
                ShiZi_LostCount = 0;
                ShiZi_LastCenter = 0;
                ShiZi_FirstCenter = 63;
                myflag.ShiZi_First_Count = 0;
                myflag.ShiZi_Last_Count = 0;
            }
            #endregion

            #region//直角转弯处理//

            #region//直角转弯转角幅度和场数处理
            if (myflag.Flag_Temp_zuo == 1)  //左转弯
            {
                if (nSpeed > 1750)
                {
                    Temp_ZhiJiao_ChangShu = ZhiJiao_ChangShu - 1;

                    Temp_Zhijiao_Change_Center = Zhijiao_Change_Center * 1.0 + 3.7;
                }
                else if (nSpeed > 1700 && nSpeed <= 1750)
                {
                    Temp_ZhiJiao_ChangShu = ZhiJiao_ChangShu - 1;

                    Temp_Zhijiao_Change_Center = Zhijiao_Change_Center * 1.0 + 3.6;
                }
                else if (nSpeed > 1650 && nSpeed <= 1700)
                {
                    Temp_ZhiJiao_ChangShu = ZhiJiao_ChangShu - 1;

                    Temp_Zhijiao_Change_Center = Zhijiao_Change_Center * 1.0 + 3.5;
                }
                else if (nSpeed > 1600 && nSpeed <= 1650)
                {
                    Temp_ZhiJiao_ChangShu = ZhiJiao_ChangShu - 1;

                    Temp_Zhijiao_Change_Center = Zhijiao_Change_Center * 1.0 + 3.4;
                }
                else if (nSpeed > 1550 && nSpeed <= 1600)
                {
                    Temp_ZhiJiao_ChangShu = ZhiJiao_ChangShu - 1;

                    Temp_Zhijiao_Change_Center = Zhijiao_Change_Center * 1.0 + 3.3;
                }
                else if (nSpeed > 1500 && nSpeed <= 1550)
                {
                    Temp_ZhiJiao_ChangShu = ZhiJiao_ChangShu - 1;

                    Temp_Zhijiao_Change_Center = Zhijiao_Change_Center * 1.0 + 3.2;
                }
                else if (nSpeed > 1450 && nSpeed <= 1500)
                {
                    Temp_ZhiJiao_ChangShu = ZhiJiao_ChangShu - 1;

                    Temp_Zhijiao_Change_Center = Zhijiao_Change_Center * 1.0 + 3.1;
                }
                else if (nSpeed > 1400 && nSpeed <= 1450)
                {
                    Temp_ZhiJiao_ChangShu = ZhiJiao_ChangShu - 1;

                    Temp_Zhijiao_Change_Center = Zhijiao_Change_Center * 1.0 + 3.0;
                }
                else if (nSpeed > 1350 && nSpeed <= 1400)
                {
                    Temp_ZhiJiao_ChangShu = ZhiJiao_ChangShu - 1;

                    Temp_Zhijiao_Change_Center = Zhijiao_Change_Center * 1.0 + 2.9;
                }
                else if (nSpeed > 1330 && nSpeed <= 1350)
                {
                    Temp_ZhiJiao_ChangShu = ZhiJiao_ChangShu;

                    Temp_Zhijiao_Change_Center = Zhijiao_Change_Center * 1.0 + 2.7;
                }
                else if (nSpeed > 1310 && nSpeed <= 1330)
                {
                    Temp_ZhiJiao_ChangShu = ZhiJiao_ChangShu;

                    Temp_Zhijiao_Change_Center = Zhijiao_Change_Center * 1.0 + 2.5;
                }
                else if (nSpeed > 1290 && nSpeed <= 1310)
                {
                    Temp_ZhiJiao_ChangShu = ZhiJiao_ChangShu;

                    Temp_Zhijiao_Change_Center = Zhijiao_Change_Center * 1.0 + 2.3;
                }
                else if (nSpeed > 1270 && nSpeed <= 1290)
                {
                    Temp_ZhiJiao_ChangShu = ZhiJiao_ChangShu;

                    Temp_Zhijiao_Change_Center = Zhijiao_Change_Center * 1.0 + 2.1;
                }
                else if (nSpeed > 1250 && nSpeed <= 1270)
                {
                    Temp_ZhiJiao_ChangShu = ZhiJiao_ChangShu;

                    Temp_Zhijiao_Change_Center = Zhijiao_Change_Center * 1.0 + 2.05;
                }
                else if (nSpeed > 1230 && nSpeed <= 1250)
                {
                    Temp_ZhiJiao_ChangShu = ZhiJiao_ChangShu;

                    Temp_Zhijiao_Change_Center = Zhijiao_Change_Center * 1.0 + 1.95;
                }
                else if (nSpeed > 1210 && nSpeed <= 1230)
                {
                    Temp_ZhiJiao_ChangShu = ZhiJiao_ChangShu;

                    Temp_Zhijiao_Change_Center = Zhijiao_Change_Center * 1.0 + 1.85;
                }
                else if (nSpeed > 1190 && nSpeed <= 1210)  //ok       
                {
                    Temp_ZhiJiao_ChangShu = ZhiJiao_ChangShu;

                    Temp_Zhijiao_Change_Center = Zhijiao_Change_Center * 1.0 + 1.75;
                }
                else if (nSpeed > 1170 && nSpeed <= 1190)  //暂时不错
                {
                    Temp_ZhiJiao_ChangShu = ZhiJiao_ChangShu;

                    Temp_Zhijiao_Change_Center = Zhijiao_Change_Center * 1.0 + 1.6;
                }
                else if (nSpeed > 1150 && nSpeed <= 1170)  //ok
                {
                    Temp_ZhiJiao_ChangShu = ZhiJiao_ChangShu;

                    Temp_Zhijiao_Change_Center = Zhijiao_Change_Center * 1.0 + 1.4;
                }
                else if (nSpeed > 1130 && nSpeed <= 1150)
                {
                    Temp_ZhiJiao_ChangShu = ZhiJiao_ChangShu;

                    Temp_Zhijiao_Change_Center = Zhijiao_Change_Center * 1.0 + 1.2;
                }
                else if (nSpeed > 1110 && nSpeed <= 1130)
                {
                    Temp_ZhiJiao_ChangShu = ZhiJiao_ChangShu;

                    Temp_Zhijiao_Change_Center = Zhijiao_Change_Center * 1.0 + 1.0;
                }
                else if (nSpeed > 1090 && nSpeed <= 1110)
                {
                    Temp_ZhiJiao_ChangShu = ZhiJiao_ChangShu;

                    Temp_Zhijiao_Change_Center = Zhijiao_Change_Center * 1.0 + 0.8;
                }
                else if (nSpeed > 1070 && nSpeed <= 1090)
                {
                    Temp_ZhiJiao_ChangShu = ZhiJiao_ChangShu;

                    Temp_Zhijiao_Change_Center = Zhijiao_Change_Center * 1.0 + 0.6;
                }
                else if (nSpeed > 1050 && nSpeed <= 1070)
                {
                    Temp_ZhiJiao_ChangShu = ZhiJiao_ChangShu;

                    Temp_Zhijiao_Change_Center = Zhijiao_Change_Center * 1.0 + 0.4;
                }
                else if (nSpeed > 1030 && nSpeed <= 1050)
                {
                    Temp_ZhiJiao_ChangShu = ZhiJiao_ChangShu + 1;

                    Temp_Zhijiao_Change_Center = Zhijiao_Change_Center * 1.0 - 0.3;
                }
                else if (nSpeed > 1010 && nSpeed <= 1030)
                {
                    Temp_ZhiJiao_ChangShu = ZhiJiao_ChangShu + 1;

                    Temp_Zhijiao_Change_Center = Zhijiao_Change_Center * 1.0 - 0.1;
                }
                else if (nSpeed > 990 && nSpeed <= 1010)
                {
                    Temp_ZhiJiao_ChangShu = ZhiJiao_ChangShu + 1;

                    Temp_Zhijiao_Change_Center = Zhijiao_Change_Center * 1.0 - 0.2;
                }
                else if (nSpeed > 970 && nSpeed <= 990)
                {
                    Temp_ZhiJiao_ChangShu = ZhiJiao_ChangShu + 2;

                    Temp_Zhijiao_Change_Center = Zhijiao_Change_Center * 1.0 - 0.3;
                }
                else if (nSpeed > 950 && nSpeed <= 970)
                {
                    Temp_ZhiJiao_ChangShu = ZhiJiao_ChangShu + 2;

                    Temp_Zhijiao_Change_Center = Zhijiao_Change_Center * 1.0 - 0.4;
                }
                else if (nSpeed > 930 && nSpeed <= 950)
                {
                    Temp_ZhiJiao_ChangShu = ZhiJiao_ChangShu + 3;

                    Temp_Zhijiao_Change_Center = Zhijiao_Change_Center * 1.0 - 0.5;
                }
                else if (nSpeed > 880 && nSpeed <= 930)
                {
                    Temp_ZhiJiao_ChangShu = ZhiJiao_ChangShu + 3;

                    Temp_Zhijiao_Change_Center = Zhijiao_Change_Center * 1.0 - 0.6;
                }
                else if (nSpeed > 830 && nSpeed <= 880)
                {
                    Temp_ZhiJiao_ChangShu = ZhiJiao_ChangShu + 3;

                    Temp_Zhijiao_Change_Center = Zhijiao_Change_Center * 1.0 - 0.7;
                }
                else if (nSpeed <= 830)
                {
                    Temp_ZhiJiao_ChangShu = ZhiJiao_ChangShu + 3;

                    Temp_Zhijiao_Change_Center = Zhijiao_Change_Center * 1.0 - 0.8;

                }
                else
                {
                    Temp_ZhiJiao_ChangShu = ZhiJiao_ChangShu;

                    Temp_Zhijiao_Change_Center = Zhijiao_Change_Center * 1.0;
                }

            }
            else if (myflag.Flag_Temp_you == 1) //右转弯
            {
                if (nSpeed > 1750)
                {
                    Temp_ZhiJiao_ChangShu = ZhiJiao_ChangShu - 1;

                    Temp_Zhijiao_Change_Center = Zhijiao_Change_Center * 1.0 + 3.75;
                }
                else if (nSpeed > 1700 && nSpeed <= 1750)
                {
                    Temp_ZhiJiao_ChangShu = ZhiJiao_ChangShu - 1;

                    Temp_Zhijiao_Change_Center = Zhijiao_Change_Center * 1.0 + 3.65;
                }
                else if (nSpeed > 1650 && nSpeed <= 1700)
                {
                    Temp_ZhiJiao_ChangShu = ZhiJiao_ChangShu - 1;

                    Temp_Zhijiao_Change_Center = Zhijiao_Change_Center * 1.0 + 3.55;
                }
                else if (nSpeed > 1600 && nSpeed <= 1650)
                {
                    Temp_ZhiJiao_ChangShu = ZhiJiao_ChangShu - 1;

                    Temp_Zhijiao_Change_Center = Zhijiao_Change_Center * 1.0 + 3.45;
                }
                else if (nSpeed > 1550 && nSpeed <= 1600)
                {
                    Temp_ZhiJiao_ChangShu = ZhiJiao_ChangShu - 1;

                    Temp_Zhijiao_Change_Center = Zhijiao_Change_Center * 1.0 + 3.35;
                }
                else if (nSpeed > 1500 && nSpeed <= 1550)
                {
                    Temp_ZhiJiao_ChangShu = ZhiJiao_ChangShu - 1;

                    Temp_Zhijiao_Change_Center = Zhijiao_Change_Center * 1.0 + 3.25;
                }
                else if (nSpeed > 1450 && nSpeed <= 1500)
                {
                    Temp_ZhiJiao_ChangShu = ZhiJiao_ChangShu - 1;

                    Temp_Zhijiao_Change_Center = Zhijiao_Change_Center * 1.0 + 3.15;
                }
                else if (nSpeed > 1400 && nSpeed <= 1450)
                {
                    Temp_ZhiJiao_ChangShu = ZhiJiao_ChangShu - 1;

                    Temp_Zhijiao_Change_Center = Zhijiao_Change_Center * 1.0 + 3.05;
                }
                else if (nSpeed > 1350 && nSpeed <= 1400)
                {
                    Temp_ZhiJiao_ChangShu = ZhiJiao_ChangShu - 1;

                    Temp_Zhijiao_Change_Center = Zhijiao_Change_Center * 1.0 + 2.95;
                }
                else if (nSpeed > 1330 && nSpeed <= 1350)
                {
                    Temp_ZhiJiao_ChangShu = ZhiJiao_ChangShu;

                    Temp_Zhijiao_Change_Center = Zhijiao_Change_Center * 1.0 + 2.7;
                }
                else if (nSpeed > 1310 && nSpeed <= 1330)
                {
                    Temp_ZhiJiao_ChangShu = ZhiJiao_ChangShu;

                    Temp_Zhijiao_Change_Center = Zhijiao_Change_Center * 1.0 + 2.6;
                }
                else if (nSpeed > 1290 && nSpeed <= 1310)
                {
                    Temp_ZhiJiao_ChangShu = ZhiJiao_ChangShu;

                    Temp_Zhijiao_Change_Center = Zhijiao_Change_Center * 1.0 + 2.4;
                }
                else if (nSpeed > 1270 && nSpeed <= 1290)
                {
                    Temp_ZhiJiao_ChangShu = ZhiJiao_ChangShu;

                    Temp_Zhijiao_Change_Center = Zhijiao_Change_Center * 1.0 + 2.2;
                }
                else if (nSpeed > 1250 && nSpeed <= 1270)
                {
                    Temp_ZhiJiao_ChangShu = ZhiJiao_ChangShu;

                    Temp_Zhijiao_Change_Center = Zhijiao_Change_Center * 1.0 + 2.1;
                }
                else if (nSpeed > 1230 && nSpeed <= 1250)
                {
                    Temp_ZhiJiao_ChangShu = ZhiJiao_ChangShu;

                    Temp_Zhijiao_Change_Center = Zhijiao_Change_Center * 1.0 + 2.0;
                }
                else if (nSpeed > 1210 && nSpeed <= 1230)
                {
                    Temp_ZhiJiao_ChangShu = ZhiJiao_ChangShu;

                    Temp_Zhijiao_Change_Center = Zhijiao_Change_Center * 1.0 + 1.7;
                }
                else if (nSpeed > 1190 && nSpeed <= 1210)  //ok       
                {
                    Temp_ZhiJiao_ChangShu = ZhiJiao_ChangShu;

                    Temp_Zhijiao_Change_Center = Zhijiao_Change_Center * 1.0 + 1.6;
                }
                else if (nSpeed > 1170 && nSpeed <= 1190)  //暂时不错
                {
                    Temp_ZhiJiao_ChangShu = ZhiJiao_ChangShu;

                    Temp_Zhijiao_Change_Center = Zhijiao_Change_Center * 1.0 + 1.4;
                }
                else if (nSpeed > 1150 && nSpeed <= 1170)  //ok
                {
                    Temp_ZhiJiao_ChangShu = ZhiJiao_ChangShu;

                    Temp_Zhijiao_Change_Center = Zhijiao_Change_Center * 1.0 + 1.2;
                }
                else if (nSpeed > 1130 && nSpeed <= 1150)
                {
                    Temp_ZhiJiao_ChangShu = ZhiJiao_ChangShu;

                    Temp_Zhijiao_Change_Center = Zhijiao_Change_Center * 1.0 + 1.1;
                }
                else if (nSpeed > 1110 && nSpeed <= 1130)
                {
                    Temp_ZhiJiao_ChangShu = ZhiJiao_ChangShu;

                    Temp_Zhijiao_Change_Center = Zhijiao_Change_Center * 1.0 + 1.0;
                }
                else if (nSpeed > 1090 && nSpeed <= 1110)
                {
                    Temp_ZhiJiao_ChangShu = ZhiJiao_ChangShu;

                    Temp_Zhijiao_Change_Center = Zhijiao_Change_Center * 1.0 + 0.8;
                }
                else if (nSpeed > 1070 && nSpeed <= 1090)
                {
                    Temp_ZhiJiao_ChangShu = ZhiJiao_ChangShu;

                    Temp_Zhijiao_Change_Center = Zhijiao_Change_Center * 1.0 + 0.6;
                }
                else if (nSpeed > 1050 && nSpeed <= 1070)
                {
                    Temp_ZhiJiao_ChangShu = ZhiJiao_ChangShu;

                    Temp_Zhijiao_Change_Center = Zhijiao_Change_Center * 1.0 + 0.4;
                }
                else if (nSpeed > 1030 && nSpeed <= 1050)
                {
                    Temp_ZhiJiao_ChangShu = ZhiJiao_ChangShu + 1;

                    Temp_Zhijiao_Change_Center = Zhijiao_Change_Center * 1.0 - 0.3;
                }
                else if (nSpeed > 1010 && nSpeed <= 1030)
                {
                    Temp_ZhiJiao_ChangShu = ZhiJiao_ChangShu + 1;

                    Temp_Zhijiao_Change_Center = Zhijiao_Change_Center * 1.0 - 0.1;
                }
                else if (nSpeed > 990 && nSpeed <= 1010)
                {
                    Temp_ZhiJiao_ChangShu = ZhiJiao_ChangShu + 1;

                    Temp_Zhijiao_Change_Center = Zhijiao_Change_Center * 1.0 - 0.2;
                }
                else if (nSpeed > 970 && nSpeed <= 990)
                {
                    Temp_ZhiJiao_ChangShu = ZhiJiao_ChangShu + 2;

                    Temp_Zhijiao_Change_Center = Zhijiao_Change_Center * 1.0 - 0.3;
                }
                else if (nSpeed > 950 && nSpeed <= 970)
                {
                    Temp_ZhiJiao_ChangShu = ZhiJiao_ChangShu + 2;

                    Temp_Zhijiao_Change_Center = Zhijiao_Change_Center * 1.0 - 0.4;
                }
                else if (nSpeed > 930 && nSpeed <= 950)
                {
                    Temp_ZhiJiao_ChangShu = ZhiJiao_ChangShu + 3;

                    Temp_Zhijiao_Change_Center = Zhijiao_Change_Center * 1.0 - 0.5;
                }
                else if (nSpeed > 880 && nSpeed <= 930)
                {
                    Temp_ZhiJiao_ChangShu = ZhiJiao_ChangShu + 3;

                    Temp_Zhijiao_Change_Center = Zhijiao_Change_Center * 1.0 - 0.6;
                }
                else if (nSpeed > 830 && nSpeed <= 880)
                {
                    Temp_ZhiJiao_ChangShu = ZhiJiao_ChangShu + 3;

                    Temp_Zhijiao_Change_Center = Zhijiao_Change_Center * 1.0 - 0.7;
                }
                else if (nSpeed <= 830)
                {
                    Temp_ZhiJiao_ChangShu = ZhiJiao_ChangShu + 3;

                    Temp_Zhijiao_Change_Center = Zhijiao_Change_Center * 1.0 - 0.8;

                }
                else
                {
                    Temp_ZhiJiao_ChangShu = ZhiJiao_ChangShu;

                    Temp_Zhijiao_Change_Center = Zhijiao_Change_Center * 1.0;
                }
            }
            #endregion

            //直角转弯
            if (myflag.Flag_Temp_zuo == 1)  //左转弯
            {
                myflag.p++;
                myline[1].Center = Sv_OutLine((int)(CCD1_Center_Line - (Temp_Zhijiao_Change_Center * myflag.p)));
                if (myflag.p > (Temp_ZhiJiao_ChangShu))// || ( CCD1_Right_Point!=127 && CCD1_Left_Point!=0 ) )
                {
                    myflag.Flag_Temp_zuo = 0;
                    myflag.Flag_ZhiJiao = 0;
                    myflag.p = 0;
                    Temp_ZhiJiao_ChangShu = 0;
                    Temp_Zhijiao_Change_Center = 0;
                    myflag.ZhiJiao_Pass_Flag_Left = 1;
                    //        myflag.state =3;
                    CCD1_LeftBool_Last = 0;

                    CCD1_RightBool_Last = 0;
                }
                setText用户自定义("直角左转弯处理");
                //     // GPIO_SetBits(PTA,GPIO_Pin_24);
            }
            else if (myflag.Flag_Temp_you == 1) //右转弯
            {
                myflag.p++;
                myline[1].Center = Sv_OutLine((int)(CCD1_Center_Line + (Temp_Zhijiao_Change_Center * myflag.p)));
                if (myflag.p > Temp_ZhiJiao_ChangShu)//|| (  CCD1_Right_Point!=127 && CCD1_Left_Point!=0 ) )
                {
                    myflag.Flag_Temp_you = 0;
                    myflag.Flag_ZhiJiao = 0;
                    myflag.p = 0;
                    Temp_ZhiJiao_ChangShu = 0;
                    Temp_Zhijiao_Change_Center = 0;
                    myflag.ZhiJiao_Pass_Flag_Right = 1;
                    //        myflag.state =3;
                    CCD1_LeftBool_Last = 0;

                    CCD1_RightBool_Last = 0;
                }
                setText用户自定义("直角右转弯处理");
                //     // GPIO_SetBits(PTA,GPIO_Pin_24);
            }
            #endregion

            #region//出单8场处理//
            if (DanxianSpecial_Out != 0)
            {
                Danxian_OutCount++;

                if (Danxian_OutCount > 8)
                {
                    DanxianSpecial_Out = 0;
                    Danxian_OutCount = 0;
                }
                else
                {
                    if (myline[1].LPoint == 0 && myline[1].RPoint == 127)
                    {
                        if (Danxian_OutCenter > 100)
                        {
                            myline[1].Center = (int)(Danxian_OutCenter * 0.8);
                        }
                        else
                        {
                            myline[1].Center = Danxian_OutCenter;
                        }

                    }
                    setText用户自定义("Danxian_OutCenter" + Danxian_OutCenter);
                    //判断出弯单线进黑带
                    if(Danxian_OutCenter>63 &&  myflag.state == 0)
                    {                       

                        if (myflag.Danxian_RightWan_Black_state == 2)
                        {
                            Down_Block_Change_Point = Found_Down_Block_you(Sv_OutLine_Center(Down_Block_Change_Point - 10));

                            if (Down_Block_Change_Point != 0 && (length.LLength + length.RLength) > 50)
                            {
                                myflag.Danxian_RightWan_Black_state = 0;

                                myflag.state = 1;

                                myflag.Flag_ZhangAi_zuo = 0;

                                myflag.Flag_ZhangAi_you = 0;

                                myflag.Flag_Danxianzuo = 0;

                                myflag.Flag_Danxianyou = 0;

                                setText用户自定义("单线后弯入黑带");
                            }
                        }
                        else if (myflag.Danxian_RightWan_Black_state == 1)
                        {
                            Down_Block_Change_Point = Found_Down_Block_you(Sv_OutLine_Center(Down_Block_Change_Point - 10));

                            if (Down_Block_Change_Point != 0)
                            {
                                myflag.Danxian_RightWan_Black_state = 2;
                            }
                        }
                        else if (myflag.Danxian_RightWan_Black_state == 0)
                        {
                            Down_Block_Change_Point = Found_Down_Block_you(Sv_OutLine_Center(Danxian_OutCenter + 10));

                            if (Down_Block_Change_Point!=0)
                            {
                                myflag.Danxian_RightWan_Black_state = 1;
                            }
                        }
                       
                    }
                    else if (Danxian_OutCenter < 63 && myflag.state == 0)
                    {

                        if (myflag.Danxian_LeftWan_Black_state == 2)
                        {
                            Down_Block_Change_Point = Found_Down_Block_zuo(Sv_OutLine_Center(Down_Block_Change_Point + 10));

                            if (Down_Block_Change_Point != 0 && (length.LLength + length.RLength) > 50)
                            {
                                myflag.Danxian_LeftWan_Black_state = 0;

                                myflag.state = 1;

                                myflag.Flag_ZhangAi_zuo = 0;

                                myflag.Flag_ZhangAi_you = 0;

                                myflag.Flag_Danxianzuo = 0;

                                myflag.Flag_Danxianyou = 0;

                                setText用户自定义("单线后弯入黑带");
                            }
                        }
                        else if (myflag.Danxian_LeftWan_Black_state == 1)
                        {
                            Down_Block_Change_Point = Found_Down_Block_zuo(Sv_OutLine_Center(Down_Block_Change_Point + 10));

                            if (Down_Block_Change_Point != 0)
                            {
                                myflag.Danxian_LeftWan_Black_state = 2;
                            }
                        }
                        else if (myflag.Danxian_LeftWan_Black_state == 0)
                        {
                            Down_Block_Change_Point = Found_Down_Block_zuo(Sv_OutLine_Center(Danxian_OutCenter - 10));

                            if (Down_Block_Change_Point != 0)
                            {

                                myflag.Danxian_LeftWan_Black_state = 1;
                            }
                        }
                        
                    }

                    setText用户自定义("Down_Block_Change_Point" + Down_Block_Change_Point);

                    setText用户自定义("Danxian_RightWan_Black_state" + myflag.Danxian_RightWan_Black_state);

                    setText用户自定义("Danxian_LeftWan_Black_state" + myflag.Danxian_LeftWan_Black_state);

                }
                setText用户自定义("出单线处理");
            }
            #endregion

            #region//出障碍10场内不进行弯道判断
            if (myflag.Flag_ZhangAi == 1)
            {
                Out_Zhangai++;
                if (Out_Zhangai > 10)
                {
                    myflag.Flag_ZhangAi = 0;
                    Out_Zhangai = 0;
                }
                setText用户自定义("出障碍处理");
            }

            #endregion

            #region//上坡20场减速处理//

            if (myflag.Flag_PoDao_Up == 1)
            {
                Out_PoDaoUp++;
                if (Out_PoDaoUp > PoDao_Jianshu_ChangShu)
                {
                    myflag.Flag_PoDao_Up = 0;

                    Out_PoDaoUp = 0;

                    myflag.Start_PoDao = 1;

                    myflag.Flag_Out_PoDaoUp = 1;
                }
                setText用户自定义("上坡处理");
                // GPIO_SetBits(PTA,GPIO_Pin_24);

            }

            #endregion

            #region//下坡50场减速处理//

            if (myflag.Flag_PoDao_Down == 1)
            {
                Out_PoDaoDown++;
                if (Out_PoDaoDown > 40)
                {
                    myflag.Flag_PoDao_Down = 0;

                    Out_PoDaoDown = 0;

                    myflag.Start_PoDao = 0;

                    myflag.Flag_Out_PoDaoUp = 0;

                }
                setText用户自定义("下坡处理");

                // GPIO_SetBits(PTA,GPIO_Pin_24);         

            }

            #endregion

            #region//出十字10场不处理单线
            if (Flag_White == 1)
            {
                Out_White_Count++;
                if (Out_White_Count > 20)
                {

                    Flag_White = 0;
                    Out_White_Count = 0;
                }
                setText用户自定义("在进入十字20场内不处理单线");

            }
            #endregion

            #region//确定下一场搜线范围，保留当场值//

            //确定下一次搜索范围

            if (myflag.Flag_Danxianzuo == 1)
            {
                myline[1].LStart = (myline[1].LPoint + 127) >> 1;
                myline[1].LEnd = CCD1_LEFT_LIMIT;
                myline[1].RStart = (myline[1].LPoint + 127) >> 1;
                myline[1].REnd = CCD1_RIGHT_LIMIT;
            }
            else if (myflag.Flag_Danxianyou == 1)
            {
                myline[1].LStart = (0 + myline[1].RPoint) >> 1;
                myline[1].LEnd = CCD1_LEFT_LIMIT;
                myline[1].RStart = (0 + myline[1].RPoint) >> 1;
                myline[1].REnd = CCD1_RIGHT_LIMIT;

            }
            else if (Abs(myline[1].Center - 63) < 8)
            {
                if (myflag.LeftWan_Black_state == 6 && ((CCD1_Left_Point == 0 && CCD1_Right_Point == 127)))
                {
                    myline[1].LStart = 25;
                    myline[1].LEnd = CCD1_LEFT_LIMIT;
                    myline[1].RStart = 25;
                    myline[1].REnd = CCD1_RIGHT_LIMIT;

                }
                else if (myflag.RightWan_Black_state == 6 && ((CCD1_Left_Point == 0 && CCD1_Right_Point == 127)))
                {
                    myline[1].LStart = 100;
                    myline[1].LEnd = CCD1_LEFT_LIMIT;
                    myline[1].RStart = 100;
                    myline[1].REnd = CCD1_RIGHT_LIMIT;
                }
                else if (CCD1_Left_Point == 0 || CCD1_Right_Point == 127)
                {
                    myline[1].LStart = (myline[1].Center + 5);
                    myline[1].LEnd = CCD1_LEFT_LIMIT;
                    myline[1].RStart = (myline[1].Center - 5);
                    myline[1].REnd = CCD1_RIGHT_LIMIT;
                }
                else
                {
                    myline[1].LStart = (myline[1].Center);
                    myline[1].LEnd = CCD1_LEFT_LIMIT;
                    myline[1].RStart = (myline[1].Center);
                    myline[1].REnd = CCD1_RIGHT_LIMIT;

                }
            }
            else
            {
                myline[1].LStart = myline[1].Center;
                myline[1].LEnd = CCD1_LEFT_LIMIT;
                myline[1].RStart = myline[1].Center;
                myline[1].REnd = CCD1_RIGHT_LIMIT;
            }
            //保留本场中点值
            myline[0].Center = myline[1].Center;
            myline[0].LPoint = myline[1].LPoint;
            myline[0].RPoint = myline[1].RPoint;


            CCD1_CCD1_Center_Average4 = CCD1_CCD1_Center_Average3;

            CCD1_CCD1_Center_Average3 = CCD1_CCD1_Center_Average2;

            CCD1_CCD1_Center_Average2 = CCD1_Center_Average;
            //保留本场标志位
            //    if(   
            //        myflag.Flag_ZhangAi_zuo == 0
            //        && myflag.Flag_ZhangAi_you == 0
            //        && myflag.Flag_Danxianzuo == 0
            //        && myflag.Flag_Danxianyou == 0
            //        && myflag.Start_PoDao == 0
            //        && myflag.Flag_ZhangAi == 0
            //        && DanxianSpecial_Out == 0
            //        && myflag.state == 0
            //      )
            //    {
            CCD1_LeftBool_Last = CCD1_LeftBool;

            CCD1_RightBool_Last = CCD1_RightBool;
            //    }
            //    else
            //    {
            //        CCD1_LeftBool_Last = 0;
            //    
            //        CCD1_RightBool_Last = 0;

            //    }
            #endregion

            #region//上传赋值//
            Lcr[0].Center = (byte)myline[1].Center;
            Lcr[0].Left = (byte)myline[1].LPoint;
            Lcr[0].Right = (byte)myline[1].RPoint;

            Lcr[1].Center = (byte)myline[1].Center;
            Lcr[1].Left = (byte)myline[1].LPoint;
            Lcr[1].Right = (byte)myline[1].RPoint;

            #endregion

            #region//单线及十字处理//

            if (myflag.Flag_Danxianzuo == 1)
            {
                if (myflag.Start_Danxian == 0 && myline[1].RPoint >= 110 && Flag_Special_Control == 1)
                {
                    PianCha = Sv_OutLine(myline[1].RPoint - Center_Help) - CCD1_Center_Line;
                    myflag.Start_Danxian = 1;
                }
                else
                {
                    PianCha = myline[1].LPoint - CCD1_Center_Line;
                }
                setText用户自定义("单线左循迹");

            }
            else if (myflag.Flag_Danxianyou == 1)
            {
                if (myflag.Start_Danxian == 0 && myline[1].LPoint > 0 && myline[1].LPoint <= 17 && Flag_Special_Control == 1)
                {
                    PianCha = Sv_OutLine(myline[1].LPoint + Center_Help) - CCD1_Center_Line;
                    myflag.Start_Danxian = 1;

                }
                else
                {
                    PianCha = myline[1].RPoint - CCD1_Center_Line;
                }
                setText用户自定义("单线右循迹");
            }
            else
            {
                if (Buxian_Value_zuo != 0 && myflag.state == 0 && myflag.Flag_Temp_zuo == 0 && myflag.Flag_Temp_you == 0)
                {
                    PianCha = myline[1].Center - CCD1_Center_Line - Buxian_Value_zuo;
                }
                else if (Buxian_Value_you != 0 && myflag.state == 0 && myflag.Flag_Temp_zuo == 0 && myflag.Flag_Temp_you == 0)
                {
                    PianCha = myline[1].Center - CCD1_Center_Line + Buxian_Value_you;
                }
                else
                {
                    PianCha = myline[1].Center - CCD1_Center_Line;
                }
            }

            //防止弯入黑带后左右晃导致图像丢失
            if (myflag.LeftWan_Black_state > 1)
            {
                setText用户自定义("左弯入直角");
                if (myline[1].RPoint == 127 && myflag.LeftWan_Black_state == 6)
                {
                    PianCha = 63 - Center_Help - CCD1_Center_Line;
                }
                else
                {
                    PianCha = Sv_OutLine(myline[1].RPoint - Center_Help) - CCD1_Center_Line;
                }
            }
            else if (myflag.RightWan_Black_state > 1)
            {
                setText用户自定义("右弯入直角");
                if (myline[1].LPoint == 0 && myflag.RightWan_Black_state == 6)
                {
                    PianCha = 63 + Center_Help - CCD1_Center_Line;
                }
                else
                {
                    PianCha = Sv_OutLine(myline[1].LPoint + Center_Help) - CCD1_Center_Line;
                }
            }

            #endregion

            #region//偏差特殊处理及限幅//
            if ((myflag.Flag_ZhangAi_you == 1 && PianCha > 0)
                || (myflag.Flag_ZhangAi_zuo == 1 && PianCha < 0)
                || (myflag.ZhiJiao_Pass_Flag_Left == 1 && PianCha > 0)
                || (myflag.ZhiJiao_Pass_Flag_Right == 1 && PianCha < 0)

            )
            {
                PianCha = (int)(PianCha * 0.5);
            }
            else if (myflag.state == 4)
            {

                if (myflag.Count_Sate_Keep < 30)
                {
                    myflag.Count_Sate_Keep++;
                    PianCha = (int)(PianCha * 0.5);
                }
                else
                {
                    myflag.Count_Sate_Keep = 30;
                }
            }
            if (
               ((CCD1_LeftBool == 1 && CCD1_RightBool == 0 && PianCha > 10) || (CCD1_LeftBool == 0 && CCD1_RightBool == 1 && PianCha < -10))
             && myflag.Flag_Temp_zuo == 0
             && myflag.Flag_Temp_you == 0
             && myflag.Flag_ZhangAi_zuo == 0
             && myflag.Flag_ZhangAi_you == 0
                //         && myflag.Flag_Danxianyou == 0
                //         && myflag.Flag_Danxianzuo == 0
             && myflag.Start_PoDao == 0
             && myflag.LeftWan_Black_state <= 1
             && myflag.Flag_ZhangAi == 0
             && myflag.LeftWan_Black_state <= 1
             )
            {
                myflag.Flag_WanDao = 1;
            }
            else
            {
                myflag.Flag_WanDao = 0;
            }

            Temp_PianCha = PianCha;

            //        if(nSpeed > 1200)  //速度小于1200不进行贴内额外处理
            {
                if (myflag.Flag_WanDao == 1)
                {
                    if (Temp_PianCha > 10)
                    {
                        PianCha = Temp_PianCha + IN_EFFECTIVE_RANGE;
                    }
                    else if (Temp_PianCha < -10)
                    {
                        PianCha = Temp_PianCha - IN_EFFECTIVE_RANGE;
                    }
                }
            }

            setText用户自定义("PianCha" + PianCha);

            //防坡道误判 
            if (myflag.Start_PoDao > 0)
            {
                Out_PoDaoUp++;
                if (Out_PoDaoUp > 100)
                {
                    myflag.Start_PoDao = 0;

                    myflag.Flag_PoDao_Up = 0;

                    myflag.Flag_PoDao_Down = 0;

                    myflag.Flag_Out_PoDaoUp = 0;

                    Out_PoDaoUp = 0;

                    Out_PoDaoUp = 0;

                    Out_PoDaoDown = 0;

                }
                else if (Abs(PianCha) > 20)
                {

                    myflag.Start_PoDao = 0;

                    myflag.Flag_PoDao_Up = 0;

                    myflag.Flag_PoDao_Down = 0;

                    myflag.Flag_Out_PoDaoUp = 0;

                    Out_PoDaoUp = 0;

                    Out_PoDaoDown = 0;

                    Out_PoDaoUp = 0;
                }
                //           // GPIO_SetBits(PTA,GPIO_Pin_24);
            }
            #endregion

            #region//保留历史中线值//
            for (i = 0; i <= 8; i++)
            {
                CCD1_HistoryCenter[i] = CCD1_HistoryCenter[i + 1];
            }
            CCD1_HistoryCenter[9] = myline[1].Center;

            for (i = 0; i <= 3; i++)
            {

                CCD1_HistoryLpoint[i] = CCD1_HistoryLpoint[i + 1];
            }
            CCD1_HistoryLpoint[4] = myline[1].LPoint;

            for (i = 0; i <= 3; i++)
            {

                CCD1_HistoryRpoint[i] = CCD1_HistoryRpoint[i + 1];
            }
            CCD1_HistoryRpoint[4] = myline[1].RPoint;
            for (i = 0; i <= 8; i++)
            {

                CCD1_HistorySpeed[i] = CCD1_HistorySpeed[i + 1];
            }
            CCD1_HistorySpeed[9] = nSpeed;

            /*记录每场5个特征点的高度*/
            for (i = 0; i <= 3; i++)
            {
                feature[i].Left_P1 = feature[i + 1].Left_P1;
                feature[i].Left_P2 = feature[i + 1].Left_P2;
                feature[i].Right_P1 = feature[i + 1].Right_P1;
                feature[i].Right_P2 = feature[i + 1].Right_P2;
                feature[i].Center = feature[i + 1].Center;
            }
            feature[4].Left_P1 = P1[43];
            feature[4].Left_P2 = P1[53];
            feature[4].Right_P1 = P1[73];
            feature[4].Right_P2 = P1[83];
            feature[4].Center = P1[63];
            //历史高度值
            for (i = 0; i < 128; i++)
            {
                CCD1_HistoryPointHeight4[i] = CCD1_HistoryPointHeight3[i];
            }
            for (i = 0; i < 128; i++)
            {
                CCD1_HistoryPointHeight3[i] = CCD1_HistoryPointHeight2[i];
            }
            for (i = 0; i < 128; i++)
            {
                CCD1_HistoryPointHeight2[i] = CCD1_HistoryPointHeight[i];
            }
            for (i = 0; i < 128; i++)
            {
                CCD1_HistoryPointHeight[i] = P1[i];
            }
            CCD1_Center_Min_Left_Last = CCD1_Center_Min_Left;

            CCD1_Center_Min_Right_Last = CCD1_Center_Min_Left;

            yuzhi_last = yuzhi;
            #endregion

            #region//保持稳定，求黑线平均值，只判断一次//
            if ((obstacle.LStay_Count < 10) && (obstacle.LGet_OkFlag == 0) && CCD1_Left_Point != 0 && CCD1_Right_Point != 127)//保持稳定
            {
                obstacle.LStay_Count++;

                obstacle.LBlackValue_Temp = P1[63];

                for (i = 63; i > 5; i--)
                {
                    if (P1[i] < obstacle.LBlackValue_Temp)
                    {
                        obstacle.LBlackValue_Temp = P1[i];
                    }
                }
                length.Race_Length_Sum += (int)(CCD1_Right_Point - CCD1_Left_Point);

                obstacle.LBlackCCD1_temp_sum += obstacle.LBlackValue_Temp;
            }
            else if (obstacle.LGet_OkFlag == 0 && obstacle.LStay_Count >= 10)//保持稳定后求平均值
            {

                obstacle.LBlackValue = obstacle.LBlackCCD1_temp_sum / 10;

                length.Race_Length_Temp1 = length.Race_Length_Sum / 10;

                length.Race_Length_Sum = 0;

                obstacle.LGet_OkFlag = 1;


            }
            else
            {
                obstacle.LBlackCCD1_temp_sum = 0;

                obstacle.LStay_Count = 0;//要求连续8场都在限定范围内  否则清零

            }

            if (obstacle.RStay_Count < 10 && obstacle.RGet_OkFlag == 0 && CCD1_Left_Point != 0 && CCD1_Right_Point != 127)//保持稳定
            {

                obstacle.RStay_Count++;


                obstacle.RBlackValue_Temp = P1[63];

                for (i = 63; i < 125; i++)
                {
                    if (P1[i] < obstacle.RBlackValue_Temp)
                    {
                        obstacle.RBlackValue_Temp = P1[i];
                    }
                }
                length.Race_Length_Sum += (int)(CCD1_Right_Point - CCD1_Left_Point);

                obstacle.RBlackCCD1_temp_sum += obstacle.RBlackValue_Temp;

            }
            else if (obstacle.RGet_OkFlag == 0 && obstacle.RStay_Count >= 10)//保持稳定后求平均值
            {

                obstacle.RBlackValue = obstacle.RBlackCCD1_temp_sum / 10;

                length.Race_Length_Temp2 = length.Race_Length_Sum / 10;

                length.Race_Length_Sum = 0;

                obstacle.RGet_OkFlag = 1;

            }
            else
            {
                obstacle.RBlackCCD1_temp_sum = 0;
                obstacle.RStay_Count = 0;//要求连续8场都在限定范围内  否则清零

            }

            if (obstacle.LGet_OkFlag == 1 && obstacle.RGet_OkFlag == 1)   //计算出稳定后黑线和白线的值，只判断一次
            {
                obstacle.BlackValue = (obstacle.RBlackValue + obstacle.LBlackValue) / 2;

                length.Race_Length = (length.Race_Length_Temp1 + length.Race_Length_Temp2) / 2;

                ////setText用户自定义("OK");

                obstacle.RGet_OkFlag = 2;

                obstacle.LGet_OkFlag = 2;

                //       // GPIO_SetBits(PTA,GPIO_Pin_24);

            }
            if (obstacle.LGet_OkFlag == 2 && obstacle.RGet_OkFlag == 2)
            {
                myflag.Count_BlackValue++;
            }
            if (myflag.Count_BlackValue > 50)
            {

                length.Race_Length_Sum = 0;

                length.Race_Length_Temp1 = 0;

                length.Race_Length_Temp2 = 0;

                myflag.Count_BlackValue = 0;

                obstacle.LGet_OkFlag = 0;

                obstacle.RGet_OkFlag = 0;

                obstacle.LStay_Count = 0;

                obstacle.RStay_Count = 0;
            }
            #endregion


            #endregion

            #region//CCD2_图像处理

            #region//初始赋值
            if (myline[2].Center == 0 && myflag.first == 1)
            {
                myline[2].Center = 63;
                myline[2].LPoint = 25;
                myline[2].RPoint = 100;
                myflag.first = 2;
                myline[3].LStart = CCD1_Center_Line;
                myline[3].LEnd = 3;
                myline[3].RStart = CCD1_Center_Line;
                myline[3].REnd = 126;

            }

            #endregion

            #region//修正起始搜索点//
            if (myline[3].LStart < CCD2_LEFT_LIMIT + 3)
            {
                myline[3].LStart = CCD2_LEFT_LIMIT + 3;
            }
            else if (myline[3].LStart > CCD2_RIGHT_LIMIT - 3)
            {
                myline[3].LStart = CCD2_RIGHT_LIMIT - 3;
            }
            if (myline[3].RStart > CCD2_RIGHT_LIMIT - 3)
            {
                myline[3].RStart = CCD2_RIGHT_LIMIT - 3;
            }
            else if (myline[3].RStart < CCD2_LEFT_LIMIT + 3)
            {
                myline[3].RStart = CCD2_LEFT_LIMIT + 3;
            }
            #endregion

            #region//下降沿跳变点提取//
            {
                JianJU = 4;
            }
            //从中间往两边搜索第一个跳变点
            //往左边
            for (i = myline[3].LStart, CCD2_LeftCount = 0; i > myline[3].LEnd; i--)
            {
                if (P2[Sv_OutLine(i)] - P2[Sv_OutLine(i - JianJU)] > Range)
                {
                    CCD2_LeftCount++;
                    {
                        if (CCD2_LeftCount > 1)
                        {
                            CCD2_Left_Point = Sv_OutLine(i - JianJU);
                            //setText用户自定义("左下降沿" + CCD2_Left_Point);
                            break;
                        }
                    }
                }
                else
                {
                    CCD2_LeftCount = 0;

                }
            }
            for (j = CCD2_Left_Point, CCD2_LeftCount = 0, CCD2_LeftCount_Temp = 0; j > myline[3].LEnd; j--)
            {
                if (P2[j] < (P2[CCD2_Left_Point]) - 1)
                {
                    CCD2_Left_Point = (int)j;
                    CCD2_LeftCount = 0;
                    //CCD2_LeftCount_Temp = 0;
                    //setText用户自定义("最小值" + CCD2_Left_Point);
                }
                else if (P2[j] == P2[CCD2_Left_Point] || P2[j] == P2[CCD2_Left_Point] - 1)
                {
                    CCD2_LeftCount_Temp++;
                    CCD2_LeftCount = 0;
                    //setText用户自定义("CCD2_LeftCount_Temp" + CCD2_LeftCount_Temp);
                    if (CCD2_LeftCount_Temp > 1 && (P2[CCD2_Left_Point] - yuzhi) < -1)//&& (P2[CCD2_Left_Point]) < CCD1_Center_Average)
                    {
                        CCD2_LeftCount_Temp = 0;

                        CCD2_LeftBool = 1;
                        //setText用户自定义("左点1" + CCD2_Left_Point);
                        break;
                    }
                }
                else if (P2[j] > P2[CCD2_Left_Point])
                {
                    CCD2_LeftCount++;
                    CCD2_LeftCount_Temp = 0;
                    //setText用户自定义("CCD2_LeftCount" + CCD2_LeftCount);
                    if (CCD2_LeftCount > 0 && (P2[CCD2_Left_Point] - yuzhi) < -1)// && (P2[CCD2_Left_Point]) < CCD1_Center_Average)
                    {
                        {
                            CCD2_LeftBool = 1;
                            //setText用户自定义("左点2" + CCD2_Left_Point);
                            CCD2_LeftCount = 0;
                            break;
                        }
                    }

                }
                if (j == myline[3].LEnd + 1)
                {
                    CCD2_Left_Point = 0;
                    CCD2_LeftBool = 0;
                    CCD2_LeftCount = 0;
                    CCD2_LeftCount_Temp = 0;
                    //setText用户自定义("左点3" + CCD2_Left_Point);
                    break;
                }
            }
            //往右边
            for (i = myline[3].RStart, CCD2_RightCount = 0; i < myline[3].REnd; i++)
            {
                if (P2[i] - P2[Sv_OutLine(i + JianJU)] > Range)
                {
                    CCD2_RightCount++;
                    {
                        if (CCD2_RightCount > 1)
                        {
                            CCD2_Right_Point = Sv_OutLine(i + JianJU);
                            //setText用户自定义("右下降沿" + CCD2_Right_Point);

                            break;
                        }
                    }
                }
                else
                {
                    CCD2_RightCount = 0;

                }
            }
            for (j = CCD2_Right_Point, CCD2_RightCount = 0, CCD2_RightCount_Temp = 0; j < myline[3].REnd; j++)
            {
                if (P2[j] < (P2[CCD2_Right_Point]) - 1)
                {
                    CCD2_Right_Point = (int)j;
                    CCD2_RightCount = 0;
                    //setText用户自定义("最小值" + CCD2_Right_Point);

                    //CCD2_RightCount_Temp = 0;
                }
                else if (P2[j] == P2[CCD2_Right_Point] || P2[j] == P2[CCD2_Right_Point] - 1)
                {
                    CCD2_RightCount_Temp++;
                    CCD2_RightCount = 0;
                    //setText用户自定义("CCD2_RightCount_Temp" + CCD2_RightCount_Temp);
                    if (CCD2_RightCount_Temp > 1 && (P2[CCD2_Right_Point] - yuzhi) < -1)//&& (P2[CCD2_Right_Point]) < CCD1_Center_Average)
                    {
                        CCD2_RightCount_Temp = 0;
                        CCD2_RightBool = 1;
                        //setText用户自定义("右点1" + CCD2_Right_Point);
                        break;
                    }
                }
                else if (P2[j] > P2[CCD2_Right_Point])
                {

                    CCD2_RightCount++;
                    CCD2_RightCount_Temp = 0;
                    if (CCD2_RightCount > 0 && (P2[CCD2_Right_Point] - yuzhi) < -1)// && (P2[CCD2_Right_Point]) < CCD1_Center_Average)
                    {
                        {
                            CCD2_RightBool = 1;
                            //setText用户自定义("右点2" + CCD2_Right_Point);
                            CCD2_RightCount = 0;
                            break;
                        }
                    }

                }
                if (j == myline[3].REnd - 1)
                {
                    CCD2_Right_Point = 127;
                    CCD2_RightBool = 0;
                    CCD2_RightCount = 0;
                    CCD2_RightCount_Temp = 0;
                    //setText用户自定义("右点3" + CCD2_Right_Point);
                    break;
                }
            }

            #endregion

            #region//继续搜索，看是否出现更有可能的黑点
            if (CCD2_LeftBool == 1 && (P2[CCD2_Left_Point] - obstacle.LBlackValue) > BLACK_RANGE && (P2[Sv_OutLine(CCD2_Left_Point - 5)] - P2[CCD2_Left_Point]) < BLACK_RANGE)
            {
                //setText用户自定义("二度搜索左");
                for (i = CCD2_Left_Point, CCD2_LeftCount = 0; i > myline[3].LEnd; i--)
                {
                    if (P2[i] - P2[Sv_OutLine(i - JianJU)] > Range)
                    {
                        CCD2_LeftCount++;
                        {
                            if (CCD2_LeftCount > 1)
                            {
                                CCD2_Left_Point = Sv_OutLine(i - JianJU);
                                //setText用户自定义("二度搜索左下降沿" + CCD2_Left_Point);
                                break;
                            }
                        }
                    }
                    else
                    {
                        CCD2_LeftCount = 0;

                    }
                }
                for (j = CCD2_Left_Point, CCD2_LeftCount = 0, CCD2_LeftCount_Temp = 0; j > myline[3].LEnd; j--)
                {
                    if (P2[j] < (P2[CCD2_Left_Point]) - 1)
                    {
                        CCD2_Left_Point = (int)j;
                        CCD2_LeftCount = 0;
                        //CCD2_LeftCount_Temp = 0;
                        //setText用户自定义("二度搜索最小值" + CCD2_Left_Point);
                    }
                    else if (P2[j] == P2[CCD2_Left_Point] || P2[j] == P2[CCD2_Left_Point] - 1)
                    {
                        CCD2_LeftCount_Temp++;
                        CCD2_LeftCount = 0;
                        //setText用户自定义("二度搜索CCD2_LeftCount_Temp" + CCD2_LeftCount_Temp);
                        if (CCD2_LeftCount_Temp > 1 && (P2[CCD2_Left_Point] - yuzhi) < -1)//&& (P2[CCD2_Left_Point]) < CCD1_Center_Average)
                        {
                            CCD2_LeftCount_Temp = 0;

                            CCD2_LeftBool = 1;
                            //setText用户自定义("二度搜索左点1" + CCD2_Left_Point);
                            break;
                        }
                    }
                    else if (P2[j] > P2[CCD2_Left_Point])
                    {
                        CCD2_LeftCount++;
                        CCD2_LeftCount_Temp = 0;
                        //setText用户自定义("二度搜索CCD2_LeftCount" + CCD2_LeftCount);
                        if (CCD2_LeftCount > 0 && (P2[CCD2_Left_Point] - yuzhi) < -1)// && (P2[CCD2_Left_Point]) < CCD1_Center_Average)
                        {
                            {
                                CCD2_LeftBool = 1;
                                //setText用户自定义("二度搜索左点2" + CCD2_Left_Point);
                                CCD2_LeftCount = 0;
                                break;
                            }
                        }

                    }
                    if (j == myline[3].LEnd + 1)
                    {
                        CCD2_Left_Point = 0;
                        CCD2_LeftBool = 0;
                        CCD2_LeftCount = 0;
                        CCD2_LeftCount_Temp = 0;
                        //setText用户自定义("二度搜索左点3" + CCD2_Left_Point);
                        break;
                    }
                }
            }
            if (CCD2_RightBool == 1 && (P2[CCD2_Right_Point] - obstacle.RBlackValue) > BLACK_RANGE && (P2[Sv_OutLine(CCD2_Right_Point + 5)] - P2[CCD2_Right_Point]) < BLACK_RANGE)
            {
                //setText用户自定义("二度搜索右");
                //往右边
                for (i = CCD2_Right_Point, CCD2_RightCount = 0; i < myline[3].REnd; i++)
                {
                    if (P2[i] - P2[Sv_OutLine(i + JianJU)] > Range)
                    {
                        CCD2_RightCount++;
                        {
                            if (CCD2_RightCount > 1)
                            {
                                CCD2_Right_Point = Sv_OutLine(i + JianJU);
                                //setText用户自定义("二度搜索右下降沿" + CCD2_Right_Point);

                                break;
                            }
                        }
                    }
                    else
                    {
                        CCD2_RightCount = 0;

                    }
                }
                for (j = CCD2_Right_Point, CCD2_RightCount = 0, CCD2_RightCount_Temp = 0; j < myline[3].REnd; j++)
                {
                    if (P2[j] < (P2[CCD2_Right_Point]) - 1)
                    {
                        CCD2_Right_Point = (int)j;
                        CCD2_RightCount = 0;
                        //setText用户自定义("二度搜索最小值" + CCD2_Right_Point);

                        //CCD2_RightCount_Temp = 0;
                    }
                    else if (P2[j] == P2[CCD2_Right_Point] || P2[j] == P2[CCD2_Right_Point] - 1)
                    {
                        CCD2_RightCount_Temp++;
                        CCD2_RightCount = 0;
                        if (CCD2_RightCount_Temp > 1 && (P2[CCD2_Right_Point] - yuzhi) < -1)//&& (P2[CCD2_Right_Point]) < CCD1_Center_Average)
                        {
                            CCD2_RightCount_Temp = 0;
                            CCD2_RightBool = 1;
                            //setText用户自定义("二度搜索右点1" + CCD2_Right_Point);
                            break;
                        }
                    }
                    else if (P2[j] > P2[CCD2_Right_Point])
                    {

                        CCD2_RightCount++;
                        CCD2_RightCount_Temp = 0;
                        if (CCD2_RightCount > 0 && (P2[CCD2_Right_Point] - yuzhi) < -1)// && (P2[CCD2_Right_Point]) < CCD1_Center_Average)
                        {
                            {
                                CCD2_RightBool = 1;
                                //setText用户自定义("二度搜索右点2" + CCD2_Right_Point);
                                CCD2_RightCount = 0;
                                break;
                            }
                        }

                    }
                    if (j == myline[3].REnd - 1)
                    {
                        CCD2_Right_Point = 127;
                        CCD2_RightBool = 0;
                        CCD2_RightCount = 0;
                        CCD2_RightCount_Temp = 0;
                        //setText用户自定义("二度搜索右点3" + CCD2_Right_Point);
                        break;
                    }
                }
            }
            #endregion

            #region//判断找到的点是否越界，为了减少误判//
            if ((CCD2_Left_Point < CCD2_LEFT_LIMIT))
            {
                CCD2_Left_Point = 0;
            }
            if ((CCD2_Right_Point > CCD2_RIGHT_LIMIT))
            {
                CCD2_Right_Point = 127;

            }
            if (CCD2_RightBool == 0)
            {
                CCD2_Right_Point = 127;
            }
            if (CCD2_LeftBool == 0)
            {
                CCD2_Left_Point = 0;
            }
            if (CCD2_Right_Point == 127)
            {
                CCD2_RightBool = 0;
            }
            if (CCD2_Left_Point == 0)
            {
                CCD2_LeftBool = 0;
            }


            #endregion

            setText用户自定义("CCD2_Left_Point" + CCD2_Left_Point);
            setText用户自定义("CCD2_Right_Point" + CCD2_Right_Point);
            #region//判断坡道//
            if (Start_Jude==1)
            {	
		     //上坡
			    if (
						     myflag.Flag_Danxianzuo == 0
					    && myflag.Flag_Danxianyou == 0
					    && myflag.Flag_ZhangAi_zuo == 0
					    && myflag.Flag_ZhangAi_you == 0
					    && myflag.state == 0
					    && DanxianSpecial_Out == 0
					    && obstacle.Statusl == COMMON
					    && myflag.Flag_PoDao_Up == 0
					    && myflag.Flag_PoDao_Down == 0
					    && myflag.Flag_Out_PoDaoUp == 0
					    && myflag.LeftWan_Black_state <= 1
					    && myflag.RightWan_Black_state <= 1
					    && myflag.Start_PoDao == 0
                        //&& Abs(nLeftSpeed-nRightSpeed) < 200 //区分弯道
					    //            && nSpeed < (SetSpeed-500)
					    && Start_Jude_Count >= 100
				     )
			    {                    
					    if ((CCD2_Right_Point - CCD2_Left_Point) >= 70
							    && Abs(CCD2_HistoryCenter[9] - 63) < 10
							    && Abs(CCD2_HistoryCenter[8] - 63) < 10
							    && Abs(CCD2_HistoryCenter[7] - 63) < 10
							    && (CCD2_Right_Point != 127 || CCD2_Left_Point != 0)
							    )
					    {

							    if ((CCD2_Right_Point - CCD2_Left_Point) > (CCD2_HistoryRpoint[2] - CCD2_HistoryLpoint[2])
							    && (CCD2_HistoryRpoint[2]-CCD2_HistoryLpoint[2]) > (CCD2_HistoryRpoint[1]-CCD2_HistoryLpoint[1])
							    &&  CCD2_Right_Point >=  CCD2_HistoryRpoint[2]     
							    &&  CCD2_Left_Point <= CCD2_HistoryLpoint[2] 
							    && CCD2_HistoryRpoint[2] >= CCD2_HistoryRpoint[1]
							    && CCD2_HistoryLpoint[2] <= CCD2_HistoryLpoint[1]
							    && Abs(CCD2_Left_Point - myline[2].LPoint)<10
							    && Abs(CCD2_Right_Point - myline[2].RPoint)<10
							    )
							    {
									    Count_Po_Dao_Up++;
									    if (Count_Po_Dao_Up > 1)
									    {
											    myflag.Flag_PoDao_Up = 1;
											
											    myflag.Flag_PoDao_Down = 0;                      
													 
											    myflag.Start_PoDao = 1;

                                            //GPIO_SetBits(PTA,GPIO_Pin_24);
											     setText用户自定义("上坡道");
										      goto Over1;
										 //                        obstacle.Statusl = Po_Dao_Up;
                                         
									    }
							    }
							    else
							    {
									    Count_Po_Dao_Up = 0;
							    }
							    // setText用户自定义("Count_Po_Dao_Up" + Count_Po_Dao_Up);
					    }
					    else
					    {
                            Count_Po_Dao_Up = 0;
					    }

			    }
			    //下坡
			    if (CCD2_Right_Point != 127 && CCD2_Left_Point != 0
					    && myflag.Flag_Danxianzuo == 0
					    && myflag.Flag_Danxianyou == 0
					    && myflag.Flag_ZhangAi_zuo == 0
					    && myflag.Flag_ZhangAi_you == 0
					    && myflag.state == 0
					    && DanxianSpecial_Out == 0
					    && myflag.LeftWan_Black_state <= 1
					    && myflag.RightWan_Black_state <= 1
					    && obstacle.Statusl == COMMON
					    && myflag.Flag_PoDao_Down == 0
					    &&  myflag.Start_PoDao == 1
                        //&&  myflag.Flag_Out_PoDaoUp == 1
                        //&& Abs(nLeftSpeed-nRightSpeed) < 200 //区分弯道

					    //&& Abs(Center_Temp-myline[2].Center)<8
					    && Abs(Center_Temp-(CCD2_Right_Point+CCD2_Left_Point)/2)<15
					    && Start_Jude_Count >= 100

		     )
			    {
					    if ((CCD2_Right_Point - CCD2_Left_Point) <= 50 && CCD2_HistoryLpoint[2] != 0 && CCD2_HistoryRpoint[2] != 127 && CCD2_Right_Point != 127 && CCD2_Left_Point != 0)
					    {
							    if ((CCD2_Right_Point - CCD2_Left_Point) < (CCD2_HistoryRpoint[2] - CCD2_HistoryLpoint[2])
							    &&  CCD2_Right_Point <=  CCD2_HistoryRpoint[2]     
							    &&  CCD2_Left_Point >= CCD2_HistoryLpoint[2] 
							    && CCD2_HistoryRpoint[2] <= CCD2_HistoryRpoint[1]
							    && CCD2_HistoryLpoint[2] >= CCD2_HistoryLpoint[1]    
							    )
							    {
									    Count_Po_Dao_Down++;
									
									    if (Count_Po_Dao_Down > 1 && myflag.Flag_PoDao_Down == 0)
									    {
											    myflag.Flag_PoDao_Down = 1;

											    myflag.Flag_PoDao_Up = 0;

											    myflag.Flag_Out_PoDaoUp = 0;

											    myflag.Start_PoDao = 2;

											    //                    obstacle.Statusl = Po_Dao_Up;

                                                //GPIO_SetBits(PTA, GPIO_Pin_24);
											    setText用户自定义("下坡道");
											     goto Over1;
                                             
									    }

							    }
							    else
							    {
									    Count_Po_Dao_Down = 0;
							    }
							    //setText用户自定义("Count_Po_Dao_Down" + Count_Po_Dao_Down);
					    }
					    else
					    {
                            Count_Po_Dao_Down = 0;
					    }



			    }
		    }
			#endregion
			Over1:;

            myline[3].LPoint = CCD2_Left_Point;

            myline[3].RPoint = CCD2_Right_Point;

            #region//计算中线//
            if (CCD2_LeftBool == 1 && CCD2_RightBool == 1)
            {
                myline[3].Center = Sv_OutLine_Center((myline[3].LPoint + myline[3].RPoint) / 2);
            }
            else if (CCD2_LeftBool == 1 && CCD2_RightBool == 0)
            {
                myline[3].Center = Sv_OutLine_Center(myline[3].LPoint + 50);
            }
            else if (CCD2_LeftBool == 0 && CCD2_RightBool == 1)
            {
                myline[3].Center = Sv_OutLine_Center(myline[3].RPoint - 50);
            }
            else if (CCD2_LeftBool == 0 && CCD2_RightBool == 0)
            {
                myline[3].Center = Center_Temp;
            }
            #endregion

            setText用户自定义("Center"+ myline[3].Center);

            #region//确定下一次搜索范围
            if (myflag.Help_ShiZhi == 1)
            {
                myline[3].LStart = myline[3].Center;
                myline[3].LEnd = CCD2_LEFT_LIMIT;
                myline[3].RStart = myline[3].Center;
                myline[3].REnd = CCD2_RIGHT_LIMIT;

            }
            else
            {
                myline[3].LStart = myline[1].Center + 10;
                myline[3].LEnd = CCD2_LEFT_LIMIT;
                myline[3].RStart = myline[1].Center - 10;
                myline[3].REnd = CCD2_RIGHT_LIMIT;
            }
            //保留本场中点值
            myline[2].Center = myline[3].Center;
            myline[2].LPoint = myline[3].LPoint;
            myline[2].RPoint = myline[3].RPoint;

            for (i = 0; i <= 8; i++)
            {
                CCD2_HistoryCenter[i] = CCD2_HistoryCenter[i + 1];
            }
            CCD2_HistoryCenter[9] = myline[3].Center;

            for (i = 0; i <= 3; i++)
            {

                CCD2_HistoryLpoint[i] = CCD2_HistoryLpoint[i + 1];
            }
            CCD2_HistoryLpoint[4] = myline[3].LPoint;

            for (i = 0; i <= 3; i++)
            {

                CCD2_HistoryRpoint[i] = CCD2_HistoryRpoint[i + 1];
            }
            CCD2_HistoryRpoint[4] = myline[3].RPoint;
            for (i = 0; i <= 8; i++)
            {

                CCD2_HistorySpeed[i] = CCD2_HistorySpeed[i + 1];
            }
            CCD2_HistorySpeed[9] = nSpeed;


            #endregion

          if (CCD1_LeftBool == 0
             && CCD1_RightBool == 0
             && Flag_White == 1
             && (myflag.state == 0 || myflag.state == 5)
             && myflag.Flag_ZhangAi_zuo == 0
             && myflag.Flag_ZhangAi_you == 0
             && myflag.Flag_Danxianyou == 0
             && myflag.Flag_Danxianzuo == 0
                     && myflag.Flag_ZhangAi == 0
             && myflag.LeftWan_Black_state <= 1
             && myflag.RightWan_Black_state <= 1
                )
            {
                myflag.Help_ShiZhi = 1;
                if (myline[3].Center > 73)
                {
                    PianCha = myline[1].Center - (CCD1_Center_Line - 10);
                }
                else if (myline[3].Center < 53)
                {
                    PianCha = myline[1].Center - (CCD1_Center_Line + 10);
                }
            }
            else
            {
                myflag.Help_ShiZhi = 0;
            }
            #region//上传赋值//
            Lcr[2].Center = (byte)myline[3].Center;
            Lcr[2].Left = (byte)myline[3].LPoint;
            Lcr[2].Right = (byte)myline[3].RPoint;

            #endregion

            if (CCD1_LeftBool == 0 && CCD1_RightBool == 0 && Flag_White == 1)
            {
                PianCha = myline[3].Center - CCD1_Center_Line;
            }
            setText用户自定义("PianCha" + PianCha);
        END: ;
            #endregion
        }


        public void SignalProcess()//信息处理
        {
            Image_Init();//变量初始化 
            get_P_from_Pixels();
            ImageProcess();
        }

        #region 系统获取P行的函数 用户不用关心
        void get_P_from_Pixels()//获取该行像素指针P
        {
            //Q = new byte[CCDWeight * CCDHeight];
            P1 = new byte[128];
            P2 = new byte[128];
            //for (int i = 0; i < CCDWeight; i++)
            //{
            //    for (int j = 0; j < CCDHeight; j++)
            //    {
            //        P1[i] = CCDData[j, i];
            //        //if (i > 0 && i<127)
            //        //{
            //        //    P1[i] = (byte)(Q[i] - Q[i - 1]);
            //        //}
            //    }

            //}
            int m = 0;
            for (int i = 0; i < CCDWeight; i++)
            {
                P1[m++] = CCDData[CCDHeight - 1, i];
            }
            m = 0;
            for (int i = 0; i < CCDWeight; i++)
            {
                P2[m++] = CCDData[CCDHeight - 1 - 1, i];
            }


        }

        void setText用户自定义(object value)//显示自定义信息
        {
            if (MyNrf.Form1.VoiceString == "")
            {
                MyNrf.Form1.VoiceString += value.ToString();
            }
            else
            {
                MyNrf.Form1.VoiceString += "\r\n" + value.ToString();
            }
        }
        #endregion

    }
}
