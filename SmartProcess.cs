using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
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


    class SmartProcess
    {
     
        #region 系统参数
        private byte RowNum = 0;
        public byte[] P;//存储当前行像素
        public byte[,] P_Pixels;//存储当前场所有像素的二维指针数组P_Pixels[0][1]
        public int Image_Height;//图像高度;
        public int Image_Width;//图像宽度;
        public List<LCR> Lcr = new List<LCR>();
        public int Count;//记录当前场数
        #endregion

       


        #region 我的变量
        byte LcrChangeCount = 0;
        int  i = 0;
        int[] right = new int[70];   //右线
        int[] left = new int[70]; //左线
        int[] center = new int[70];     //中线
        int[] LStart = new int[70];    //左始
        int[] RStart = new int[70];     //右始
        int lastFieldCenter = 100;  //上一场第一行中线

        int cutStart = 0;
        int straightCross = 0;  //直路十字标志
        int isStraightCross = 0;
        float crossK = 0;
        int crossFristRowInto = 0;


        int[] fixValuve = new int[70]
        {
            155,155,153,151,150,148,146,145,143,142,
            140,138,137,136,134,132,131,129,127,126,
            124,122,121,119,117,115,114,110,108,107,
            105,103,102,99,97,96,94,92,90,89,
            87,85,83,81,79,78,76,75,73,71,
            70,67,66,64,62,60,57,56,54,52,
            50,48,46,45,41,39,37,35,33,31,
        };
        int cutRow = 0;
        int crossCorrectCenter = 0;
        int crossCorrectRow = 0;


        int turnCross = 0;
        int turnCorssDir = 0;//左还是右 1为左 2为右
        int turnCorssRow = 0;
        int fieldDian = 0;
        int isTurnCross = 0;  //有尖角但未出现十字

        int wrongFillRow = 0;
        int wrongFillFlag = 0;

        int element = 0;
        int turnCenter = 0;//打脚行
        int turnRow = 0;
        int flagSpeedDiff = 0;

        int leftDownstair = 0;
        int leftUpstairStrat = 0;
        int leftCircleRow = 0;
        int maybeLeftCircleRow = 0;

        int rightUpstair = 0;
        int rightDownstairStart = 0;
        int rightCircleRow = 0;
        int maybeRightCircleRow = 0;

        int smallS = 0;
        int bigS = 0;

        int flagProtect = 0;

        //int isIntoTurn = 0;
        //int flagIntoTurn = 0;
        //int intoTurnCnt = 0;
        int centerMax = 0;
        int centerMin = 0;
        int flagRamp = 0;

        int isStraightLine = 0;
        int flagStraightLine = 0;

        int straightTriangle = 0;
        int turnTriangle = 0;

        int turnTriangleRow = 0;
        int turnTriangleRowNum = 0;

        int leftEndLine = 0;
        int rightEndLine = 0;
        int leftEndLineGet = 0;
        int rightEndLineGet = 0;

        int whichTurnInCross = 6;//5右转 6左转

        int secondCar = 1;
        int startPull = 0;
        int endPullLine = 0;
        int startPullLine = 0;

        
        int secondRightUpRow = 0;
        int secondRightJumpRow = 0;
        int isSecondRightJumpFinish = 0;

        int secondLeftUpRow = 0;
        int secondLeftJumpRow = 0;
        int isSecondLeftJumpFinish = 0;

        int carRow = 0;
        int rightFindCarFlag = 0;
        int leftFindCarFlag = 0;

        int secondFindBlackWriteRow = 0;
        int secondFindWriteBlackRow = 0;
        int secondFindBlackWriteFlag = 0;
        int beginBlack = 0;

        int carFlag = 0;

        int tongCarFlag = 0;
        int tongXian = 0;

        #endregion

        #region 自定义
        int Sv_OutLine(int Value)//保护越界
        {
            if (Value > 198)
            {
                Value = 198;
            }
            else if (Value < 1)
            {
                Value = 1;
            }

            return Value;

        }
        int fabs(int a)
        {
            if (a < 0)
                a = -a;
            return a;
        }

        int LStartDianShift(int start,int row)
        {
            while (start > center[row] && P_Pixels[row, start] == 0 && start >= 6)
            {
                start = start - 5;
            }
            return start;
        }
        int RStartDianShift(int start, int row)
        {
            while (start < center[row] && P_Pixels[row, start] == 0 && start <= 193)
            {
                start = start + 5;
            }
            return start;
        }
        int MinNum(int Value1, int Value2)
        {

            if (Value1 >= Value2)
            {
                return Value2;
            }
            else
                return Value1;
        }
        byte MaxNum(int Value1, int Value2)
        {
            if (Value1 <= Value2)
            {
                return (byte)Value2;
            }
            else
                return (byte)Value1;
        }
        byte ZeroProject(int value1)
        {
            if (value1 <= 0)
            {
                return 0;
            }
            else
                return (byte)value1;
        }
        #endregion
        void Image_Init()//把每场图像处理里数据里需要初始化的变量初始化 
        {
            LStart[0] = lastFieldCenter;
            RStart[0] = lastFieldCenter;
            cutStart = 0;
            isStraightCross = 0;
            cutRow = 70;
            wrongFillRow = 0;
            wrongFillFlag = 0;

            element = 0;
            turnRow = 0;
            flagSpeedDiff = 0;

            leftDownstair = 0;
            rightUpstair = 0;
            leftCircleRow = 0;
            maybeLeftCircleRow = 0;
            rightCircleRow = 0;
            maybeRightCircleRow = 0;

            rightDownstairStart = 0;
            leftUpstairStrat = 0;

            smallS = 0;
            bigS = 0;

            centerMax = 0;
            centerMin = 255;

            isStraightLine = 0;
            flagStraightLine = 0;

            leftEndLineGet = 0;
            rightEndLineGet = 0;


            secondLeftUpRow = 0;
            secondLeftJumpRow = 0;
            isSecondLeftJumpFinish = 0;

            secondRightUpRow = 0;
            secondRightJumpRow = 0;
            isSecondRightJumpFinish = 0;
            rightFindCarFlag = 0;
            leftFindCarFlag = 0;
           

            secondFindBlackWriteRow = 0;
            secondFindWriteBlackRow = 0;
            secondFindBlackWriteFlag = 0;

            carRow = 0;
            carFlag = 0;
            beginBlack = 0;
            tongCarFlag = 0;
            
        }


        void ImageProcess()//图像行处理
        {

            ///右线大于180截断
            if (cutStart == 1)
            {
                left[RowNum] = 255;
                right[RowNum] = 255;
                LStart[RowNum] = 255;
                RStart[RowNum] = 255;
                center[RowNum] = 255;
            }
            if (cutStart == 0) //如果未截断
            {
                if (RowNum == 0 && (straightCross == 1 || turnCross == 1 || isTurnCross == 1))//如果第一行是十字特殊处理
                {
                    for (i = LStartDianShift(Sv_OutLine(left[0] - 10),RowNum); i <= 198; i++)
                    {
                        left[RowNum] = i;
                        if (P_Pixels[RowNum, i] == 0 && P_Pixels[RowNum, i + 1] == 0)
                            break;
                    }
                    for (i = RStartDianShift(Sv_OutLine(right[0] + 10),RowNum); i >= 1; i--)
                    {
                        right[RowNum] = i;
                        if (P_Pixels[RowNum, i] == 0 && P_Pixels[RowNum, i - 1] == 0)
                            break;
                    }
                }
                else
                {
                    for (i = LStart[RowNum]; i <= 198; i++)
                    {
                        left[RowNum] = i;
                        if (P_Pixels[RowNum, i] == 0 && P_Pixels[RowNum, i + 1] == 0)
                            break;
                    }
                    for (i = RStart[RowNum]; i >= 1; i--)
                    {
                        right[RowNum] = i;
                        if (P_Pixels[RowNum, i] == 0 && P_Pixels[RowNum, i - 1] == 0)
                            break;
                    }
                }
                

                if (RowNum == 0 ) 
                {
                        center[RowNum] = (left[RowNum] + right[RowNum]) / 2;
                        if (left[RowNum] >= 196 && right[RowNum] <= 3)
                            center[RowNum] = lastFieldCenter;
                        lastFieldCenter = center[RowNum];
                }

                if (RowNum >= 1)
                {
                    if (right[RowNum] == 1 && left[RowNum] != 198 && !(left[RowNum - 1] == 198 && right[RowNum - 1] == 1))
                    {
                        center[RowNum] = Sv_OutLine(center[RowNum - 1] + left[RowNum] - left[RowNum - 1]);
                    }
                    if (right[RowNum] == 1 && left[RowNum] != 198 && left[RowNum - 1] == 198 && right[RowNum - 1] == 1)
                    {
                        center[RowNum] = (left[RowNum] + right[RowNum]) / 2;
                    }
                    if (right[RowNum] != 1 && left[RowNum] == 198 && !(left[RowNum - 1] == 198 && right[RowNum - 1] == 1) )
                    {
                        center[RowNum] = Sv_OutLine(center[RowNum - 1] + right[RowNum] - right[RowNum - 1]);
                    }
                    if (right[RowNum] != 1 && left[RowNum] == 198 && left[RowNum - 1] == 198 && right[RowNum - 1] == 1)
                    {
                        center[RowNum] = (left[RowNum] + right[RowNum]) / 2;
                    }
                    if (right[RowNum] != 1 && left[RowNum] != 198)
                    {
                        center[RowNum] = (left[RowNum] + right[RowNum]) / 2;
                    }
                    if (right[RowNum] == 1 && left[RowNum] == 198)
                    {
                        center[RowNum] = (left[RowNum] + right[RowNum]) / 2;
                    }
                    center[RowNum] = Sv_OutLine(center[RowNum]);
                }
            }
            //截断的判断
            if (RowNum >= 10 && RowNum <= 65 && cutStart == 0)
            {
                if (P_Pixels[RowNum + 1, center[RowNum]] == 0
                    && P_Pixels[RowNum + 2, center[RowNum]] == 0)//当前行向上1 2行为黑
                {

                    if (P_Pixels[RowNum + 1, (right[RowNum] * 2 + center[RowNum]) / 3] == 0  //左中右线1/3处都为黑 截断
                      && P_Pixels[RowNum + 1, (left[RowNum] * 2 + center[RowNum]) / 3] == 0 //避开了车对截断的影响
                      && P_Pixels[RowNum + 2, (left[RowNum] * 2 + center[RowNum]) / 3] == 0
                      && P_Pixels[RowNum + 2, (left[RowNum] * 2 + center[RowNum]) / 3] == 0)
                        cutStart = 1;
                    
                }

                if(right[RowNum] >= 180||left[RowNum]<=10)
                {
                    cutStart = 1;
                }

                if (cutStart == 1)
                {
                    center[RowNum] = 255;
                    cutRow = RowNum;
                    setText用户自定义("截断行 " + cutRow);
                }
            }
            

            if(RowNum <= 68)
            {
                //后车
                LStart[RowNum + 1] = Sv_OutLine(left[RowNum] - 2);
                RStart[RowNum + 1] = Sv_OutLine(right[RowNum] + 2);
                while (//LStart[RowNum + 1] > center[RowNum] 
                    P_Pixels[RowNum + 1, LStart[RowNum + 1]] == 0 
                    && LStart[RowNum + 1]>=6)
                {
                    LStart[RowNum + 1] = LStart[RowNum + 1] - 1;
                }
                while (//RStart[RowNum + 1] < center[RowNum] 
                     P_Pixels[RowNum + 1, RStart[RowNum + 1]] == 0 
                    && RStart[RowNum + 1]<=193) 
                {
                    RStart[RowNum + 1] = RStart[RowNum + 1] + 1;
                }
                //
            }
          
        }

        void FieldProcess() //图像场处理函数内联
        {
            #region 前车
            /*
            #region 直入十字 加捅线
            if (straightCross == 0 && turnCross == 0)
            {
                for (i = 10; i < cutRow; i++)
                {
                    if (left[i] == 198 && right[i] == 1)
                    {
                        isStraightCross++;
                    }
                    else
                    {
                        isStraightCross = 0;
                    }
                    if (isStraightCross >= 4)
                    {
                        straightCross = 1;
                        crossCorrectRow = i;  //暂时为当前行
                        break;
                    }
                }
                //捅线 判断三角
                if (straightCross == 1)
                {
                    for (i = 0; i <= 55; i++)
                    {
                        if (P_Pixels[i, center[0]] == 0 && P_Pixels[i + 1, center[0]] == 0)
                        {
                            if(P_Pixels[i + 10, center[0]] == 0 && P_Pixels[i, Sv_OutLine(center[0] - 20)] == 0 && P_Pixels[i, Sv_OutLine(center[0] + 20)] == 0)
                            {
                                straightCross = 0;
                                break;
                            }
                        }
                    }
                }
            }
           
            if (straightCross == 1)
            {
                for (i = crossCorrectRow; i < cutRow; i++)
                {
                    for (fieldDian = center[0]; fieldDian >= 1; fieldDian--)
                    {
                        right[i] = fieldDian;
                        if (P_Pixels[i, fieldDian] == 0 && P_Pixels[i, fieldDian - 1] == 0)
                        {
                            RStart[i] = Sv_OutLine(fieldDian + 10);
                            break;
                        }
                    }
                    for (fieldDian = center[0]; fieldDian <= 198; fieldDian++)
                    {
                        left[i] = fieldDian;
                        if (P_Pixels[i, fieldDian] == 0 && P_Pixels[i, fieldDian + 1] == 0)
                        {
                            LStart[i] = Sv_OutLine(fieldDian - 10);
                            break;
                        }
                    }
                    center[i] = (left[i] + right[i]) / 2;
                }

                for (i = crossCorrectRow; i < cutRow; i++)
                {
                    if ((left[i] - right[i] - fixValuve[i] <= 10 && left[i] - right[i] + 20 >= fixValuve[i])
                        && left[i] <= 180 && right[i] >= 20) 
                    {
                        crossCorrectCenter = center[i + 2]; 
                        crossCorrectRow = i + 2;
                        break;
                    }
                }

                setText用户自定义("算斜率的终点 " +crossCorrectCenter+ " " + crossCorrectRow);
                crossK = (float)(crossCorrectCenter - center[0]) / crossCorrectRow;
                setText用户自定义("直路十字斜率 " + crossK);

                if (straightTriangle == 0)
                {
                    for (i = 0; i <= 55; i++)
                    {
                        if (P_Pixels[i, center[0]] == 0 && P_Pixels[i + 1, center[0]] == 0)
                        {
                            if (P_Pixels[i + 10, center[0]] != 0 && P_Pixels[i, Sv_OutLine(center[0] - 20)] != 0 && P_Pixels[i, Sv_OutLine(center[0] + 20)] != 0)
                            {
                                straightTriangle = 1;
                            }
                        }
                    }
                }

                if (left[0] == 198 && right[0] == 1
                   && left[1] == 198 && right[1] == 1
                   && left[2] == 198 && right[2] == 1)  //若第一行全丢
                {
                    crossFristRowInto = 1;
                    setText用户自定义("第一行进十字");
                }

                if (crossFristRowInto == 1)
                    if ((left[0] != 198 || right[0] != 1)
                        && (fabs(left[0] - right[0] - fixValuve[0]) <= 4 
                        || left[0] - right[0] <= fixValuve[0]))  //第一行恢复 出十字
                    {
                        crossFristRowInto = 0;
                        turnCross = 0;
                        straightCross = 0;
                        turnTriangle = 0;
                        turnCorssRow = 0;
                        straightTriangle = 0;
                        turnCorssDir = 0;
                    }
                for (i = 1; i <= crossCorrectRow; i++)
                {
                    center[i] = center[0] + (int)(crossK * i);
                }
                crossCorrectRow = Sv_OutLine(crossCorrectRow - 7);//下次扫描下移5行 
                setText用户自定义("直路十字");
                if (straightTriangle == 1)
                    setText用户自定义("三角");
            }
            #endregion

            #region 弯入十字
            if (turnCross == 0)
            {
                for (i = 4; i <= MinNum(55 - 4, cutRow - 4); i++)
                {
                    if (right[i] - right[i - 3] >= 4 && right[i - 1] - right[i - 4] >= 4
                        && right[i] - right[i + 3] >= 4 && right[i + 1] - right[i + 4] >= 4
                        && (turnCorssDir == 0 || turnCorssDir == 1)) 
                    {
                        isTurnCross = 1;
                        turnCorssDir = 1;
                        turnCorssRow = i;
                        setText用户自定义("右尖角 " + turnCorssRow);
                        break;
                    }

                    if (left[i + 3] - left[i] >= 4 && left[i + 4] - left[i + 1] >= 4
                        && left[i - 3] - left[i] >= 4 && left[i - 4] - left[i - 1] >= 4
                        && (turnCorssDir == 0 || turnCorssDir == 2))
                    {
                        isTurnCross = 1;
                        turnCorssDir = 2;
                        turnCorssRow = i;
                        setText用户自定义("左尖角 " + turnCorssRow);
                        break;
                    }
                }
                if(isTurnCross == 1 && turnCorssDir == 1)
                {
                    if (!(right[turnCorssRow + 2] > right[turnCorssRow + 4]
                        && right[turnCorssRow + 3] > right[turnCorssRow + 5]))
                    {
                        isTurnCross = 0;
                        turnCorssDir = 0;
                        setText用户自定义("错误尖角");
                    }
                        
                }
                if(isTurnCross == 1&&turnCorssDir == 2)
                {
                    if (!(left[turnCorssRow + 4] > left[turnCorssRow + 2]
                        && left[turnCorssRow + 5] > left[turnCorssRow + 3]))
                    {
                        isTurnCross = 0;
                        turnCorssDir = 0;
                        setText用户自定义("错误尖角");
                    }
                }
            }
            
            
            if (isTurnCross == 1)
            {
                if(turnCorssDir == 1)//右边有尖角
                {
                    for (i = turnCorssRow; i < cutRow; i++)
                    {
                        for (fieldDian = left[i]; fieldDian >= 1; fieldDian--)//从左边往右边扫
                        {
                            right[i] = fieldDian;
                            if (P_Pixels[i, fieldDian] == 0 && P_Pixels[i, fieldDian - 1] == 0)
                            {
                                RStart[i] = Sv_OutLine(fieldDian + 10);
                                break;
                            }
                                
                        }
                        center[i] = (right[i] + left[i]) / 2;

                        if (right[i - 2] - right[i - 4] >= 45 
                            && right[i - 3] - right[i - 5] >= 45 
                            && right[i] >= 100 
                            && right[i - 1] >= 100 
                            && turnCross == 0) 
                        {
                            turnCross = 1;
                            crossCorrectRow = i - 2;
                            setText用户自定义("出现赛道 " + i);
                        }
                    }
                }
                if (turnCorssDir == 2) //左边有尖角
                {
                    for (i = turnCorssRow; i < cutRow; i++)
                    {
                        for (fieldDian = right[i]; fieldDian <= 198; fieldDian++)//从右边往左边扫
                        {
                            left[i] = fieldDian;
                            if (P_Pixels[i, fieldDian] == 0 && P_Pixels[i, fieldDian + 1] == 0)
                            {
                                LStart[i] = Sv_OutLine(fieldDian - 10);
                                break;
                            }
                                
                        }
                        center[i] = (right[i] + left[i]) / 2;

                        if (left[i - 4] - left[i - 2] >= 45 
                            && left[i - 5] - left[i - 3] >= 45 
                            && left[i] <= 100 
                            && left[i - 1] <= 100 
                            && turnCross == 0) 
                        {
                            turnCross = 1;
                            crossCorrectRow = i - 2;
                            setText用户自定义("出现赛道 " + i);
                        }
                    }
                }
                if (turnCross == 0) //未发现赛道 截断
                {
                    cutRow = turnCorssRow;
                    setText用户自定义("截断 " + cutRow);
                    for (i = turnCorssRow; i <= 69; i++)
                    {
                        //left[i] = 255;
                        //right[i] = 255;
                        //LStart[i] = 10;
                        //RStart[i] = 255;
                        center[i] = 255;
                    }
                }

            }

            if(turnCross == 1)
            {
                for (i = crossCorrectRow; i < cutRow; i++)  //扫描正确的行
                {
                    if (turnCorssDir == 2) //左边有尖角
                    {
                        if (left[i] - right[i] - fixValuve[i] <= 25
                         && (left[i] != 198 || right[i] != 1)
                        && left[i - 2] - left[i] >= 45
                            && left[i - 3] - left[i - 1] >= 45)
                        {
                            crossCorrectCenter = center[i];
                            crossCorrectRow = i;
                            break;
                        }
                    }
                    if (turnCorssDir == 1)//右边有尖角
                    {
                        if (left[i] - right[i] - fixValuve[i] <= 25
                         && (left[i] != 198 || right[i] != 1)
                        && right[i] - right[i - 2] >= 45
                            && right[i - 1] - right[i - 3] >= 45)
                        {
                            crossCorrectCenter = center[i];
                            crossCorrectRow = i;
                            break;
                        }
                    }
                    
                }
               crossCorrectCenter = center[crossCorrectRow];
                setText用户自定义("算斜率的终点 " + crossCorrectCenter + " " + crossCorrectRow);
                crossK = (float)(crossCorrectCenter - center[0]) / crossCorrectRow;
                setText用户自定义("弯入斜率 " + crossK);

                for (i = 1; i <= crossCorrectRow; i++)
                {
                    center[i] = center[0] + (int)(crossK * i);
                }
                crossCorrectRow = Sv_OutLine(crossCorrectRow - 2);//下次扫描下移5行 

                if (left[0] == 198 && right[0] == 1
                    && left[1] == 198 && right[1] == 1
                    && left[2] == 198 && right[2] == 1)  //若第一行全丢
                {
                    crossFristRowInto = 1;
                    setText用户自定义("第一行进十字");
                }

                if (crossFristRowInto == 1)
                    if ((left[0] != 198 || right[0] != 1)
                        && (fabs(left[0] - right[0] - fixValuve[0]) <= 4
                        || left[0] - right[0] <= fixValuve[0]))  //第一行恢复 出十字
                    {
                        crossFristRowInto = 0;
                        turnCross = 0;
                        straightCross = 0;
                        turnTriangle = 0;
                        turnCorssRow = 0;
                        straightTriangle = 0;
                        turnCorssDir = 0;
                        isTurnCross = 0;
                    }
                //
                if (turnTriangle == 0)
                {

                    if (turnCorssDir == 1)//右边有尖角
                    {
                        for (i = 0; i < cutRow; i++)
                        {
                            if (P_Pixels[i, 198] == 0)
                            {
                                turnTriangleRow = i;
                                break;
                            }
                        }
                        for (i = 0; i < MinNum(turnTriangleRow,45); i++)
                        {
                            for (fieldDian = 198; fieldDian >= 1; fieldDian--)
                            {
                                if (P_Pixels[i, fieldDian] == 0 && P_Pixels[i, fieldDian + 1] == 0)
                                {
                                    if (fieldDian - right[i] >= 20)
                                        turnTriangleRowNum++;
                                    else
                                        turnTriangleRowNum = 0;
                                    break;
                                }
                            }
                            if (turnTriangleRowNum >= 5)
                            {
                                turnTriangle = 1;
                                break;
                            }
                        }

                    }

                    if (turnCorssDir == 2)//左边有
                    {
                        for (i = 0; i < cutRow; i++)
                        {
                            if (P_Pixels[i, 1] == 0)
                            {
                                turnTriangleRow = i;
                                break;
                            }
                        }
                        for (i = 0; i < MinNum(turnTriangleRow, 45); i++)
                        {
                            for (fieldDian = 1; fieldDian <= 198; fieldDian++)
                            {
                                if (P_Pixels[i, fieldDian] == 0 && P_Pixels[i, fieldDian - 1] == 0)
                                {
                                    if (left[i] - fieldDian >= 20)
                                        turnTriangleRowNum++;
                                    else
                                        turnTriangleRowNum = 0;
                                    break;
                                }
                            }
                            if (turnTriangleRowNum >= 5)
                            {
                                turnTriangle = 1;
                                break;
                            }
                        }
                    }
                }
                //
                if (turnTriangle == 1)
                    setText用户自定义("弯路三角");
            }
            #endregion

            #region 重新拟向
            if (turnCross == 0 && straightCross == 0 && cutRow >= 55)
            {
                for (i = 2; i < cutRow; i++)
                {
                    if (left[i] != 198 && right[i] != 1 
                        && (left[i - 2] == 198 || right[i - 2] == 1) 
                        && fabs(center[i] - center[i - 2]) >= 10)
                    {
                        setText用户自定义("补线重新扫 " + i);
                        wrongFillRow = i;
                        wrongFillFlag = 1;
                        break;
                    }
                        
                }
            }

            if(wrongFillFlag == 1)
            {
                for (i = wrongFillRow - 1; i > 0; i--)
                {
                    center[i] = (left[i] + right[i]) / 2;
                }
            }
            #endregion

            #region  扫圆角
            for (i = 5; i < cutRow; i++)
            {
                if (left[i] <= left[i - 1]
               && left[i - 1] <= left[i - 2]
               && left[i - 2] <= left[i - 3]
               && left[i - 3] <= left[i - 4]
                && left[i] < left[i - 4])
                {
                    maybeLeftCircleRow = i;
                    leftDownstair = 0;
                    leftUpstairStrat = 1;
                }
                else 
                {
                    leftDownstair = 1;
                }


                if (leftDownstair == 1 && leftUpstairStrat == 1)
                {
                    if (left[i] >= left[i - 1]
                   && left[i - 1] >= left[i - 2]
                   && left[i - 2] >= left[i - 3]
                   && left[i - 3] >= left[i - 4]
                   && left[i] > left[i - 4]
                   && left[i] - left[i - 4] <= 15) 
                    {
                        if (fabs(i - maybeLeftCircleRow) <= 15)
                        {
                            leftCircleRow = (maybeLeftCircleRow + i) / 2;
                            setText用户自定义("左圆角 " + leftCircleRow);
                            break;
                        }
                    }
                }
            }



            for (i = 5; i < cutRow; i++)
            {
                if (right[i] >= right[i - 1]
               && right[i - 1] >= right[i - 2]
               && right[i - 2] >= right[i - 3]
               && right[i - 3] >= right[i - 4]
                && right[i] > right[i - 4])
                {
                    maybeRightCircleRow = i;
                    rightUpstair = 0;
                    rightDownstairStart = 1;
                }
                else
                {
                    rightUpstair = 1;
                }


                if (rightUpstair == 1 && rightDownstairStart == 1) 
                {
                    if (right[i] <= right[i - 1]
                   && right[i - 1] <= right[i - 2]
                   && right[i - 2] <= right[i - 3]
                   && right[i - 3] <= right[i - 4]
                   && right[i] < right[i - 4]
                   && right[i - 4] - right[i] <= 15) 
                    {
                        if (fabs(i - maybeRightCircleRow) <= 15)
                        {
                            rightCircleRow = (maybeRightCircleRow + i) / 2;
                            setText用户自定义("右圆角 " + rightCircleRow);
                            break;
                        }
                    }
                }
            }
            #endregion


            #region 坡道
            if (cutRow >= 65)
            {
                for (i = 10; i < cutRow; i++)
                {
                    isStraightLine = i;
                    if (fabs(center[i] - center[i - 10]) >= 10)
                        break;
                }
                if (isStraightLine == cutRow - 1)
                    flagStraightLine = 1;
                for (i = 6; i < cutRow; i++)
                {
                    if (left[i] - right[i] - fixValuve[i] >= 20 && left[i] - right[i] - fixValuve[i] <= 35 && fabs(center[i] - 100) <= 25
                        && left[i - 3] - right[i - 3] - fixValuve[i - 3] >= 20 && left[i - 3] - right[i - 3] - fixValuve[i - 3] <= 35 && fabs(center[i - 3] - 100) <= 25
                        && left[i - 6] - right[i - 6] - fixValuve[i - 6] >= 20 && left[i - 6] - right[i - 6] - fixValuve[i - 6] <= 35 && fabs(center[i - 6] - 100) <= 25
                       // && left[i - 9] - right[i - 9] - fixValuve[i - 9] >= 20 && fabs(center[i - 9] - 100) <= 25
                        && flagStraightLine == 1 && straightCross == 0 && isTurnCross == 0 && turnCross == 0) 
                    {
                        
                        flagRamp = 15;
                        setText用户自定义(i);
                        break;
                    }
                        
                }
            }
            #endregion


            #region 起跑线
            if (straightCross == 0 && isTurnCross == 0 && turnCross == 0)
            {
                for (i = 0; i < MinNum(35,cutRow); i++)
                {
                    for (fieldDian = center[i]; fieldDian <= 198; fieldDian++)
                    {
                        left[i] = fieldDian;
                        if (P_Pixels[i, fieldDian] == 0 && P_Pixels[i, fieldDian + 1] == 0)
                        {
                            break;
                        }
                    }
                    for (fieldDian = center[i]; fieldDian >= 1; fieldDian--)
                    {
                        right[i] = fieldDian;
                        if (P_Pixels[i, fieldDian] == 0 && P_Pixels[i, fieldDian - 1] == 0)
                        {
                            break;
                        }
                    }
                }
                for (i = 5; i < MinNum(35, cutRow); i++)
                {
                    leftEndLine = i;
                    if (left[i - 4] - left[i] >= 15 && left[i - 5] - left[i - 1] >= 15
                        && left[i + 4] - left[i] >= 15 && left[i + 3] - left[i - 1] >= 15)
                    {
                        leftEndLineGet = 1;
                        break;
                    }
                }

                for (i = 5; i < MinNum(35, cutRow); i++)
                {
                    rightEndLine = i;
                    if (right[i] - right[i - 4] >= 15 && right[i - 1] - right[i - 5] >= 15
                        && right[i] - right[i + 4] >= 15 && right[i - 1] - right[i + 3] >= 15)  
                    {
                        rightEndLineGet = 1;
                        break;
                    }
                }
                if (leftEndLineGet == 1 && rightEndLineGet == 1
                    && fabs(leftEndLine - rightEndLine) <= 7)
                    setText用户自定义("起跑线");
            }
            
            #endregion


            if (rightCircleRow != 0 && leftCircleRow != 0 && isTurnCross == 0 && turnCross == 0 && straightCross == 0) 
            {
                if (left[leftCircleRow] - right[rightCircleRow] >= 40  )  
                {
                    smallS = 1;
                    setText用户自定义("小s");
                }
                if (left[leftCircleRow] - right[rightCircleRow] < 40 ) 
                {
                    bigS = 1;
                    setText用户自定义("大s");
                }

            }



            if (cutRow < 55)
            {
                element = 2;
            }
            if (cutRow >= 55)
            {
                element = 1;
                for (i = 0; i < cutRow; i++)
                {
                    if (center[i] > centerMax)
                        centerMax = center[i];
                    if (center[i] < centerMin)
                        centerMin = center[i];
                }
                if (centerMin <= 25)
                    element = 2;
                if (centerMax >= 175)
                    element = 2;

            }

            for (i = 4; i < cutRow; i++)
            {
                if (left[i] < right[i]
                  && left[i - 1] < right[i - 1]
                  && left[i - 2] < right[i - 2]
                  && left[i - 3] < right[i - 3]
                  && left[i - 4] < right[i - 4])
                    flagProtect = 1; 
            }


            //if (flagIntoTurn == 1)
            //{
            //    setText用户自定义("入弯");
            //}

            if(element == 1)
            {
                setText用户自定义("直道" );
            }
            if(element == 2)
            {
                setText用户自定义("弯道");
            }
            //if (flagProtect == 1)
            //{
            //    setText用户自定义("出赛道");
            //}
            if(flagRamp > 0)
            {
                flagRamp--;
                setText用户自定义("坡道");
            }
            */
            #endregion  

            
            if (secondCar >= 1)
            {
                #region 后车十字
                if (straightCross == 0)
                {
                    for (i = 5; i < MinNum(45,cutRow); i++)
                    {
                        if (left[i] == 198 && right[i] == 1)
                        {
                            isStraightCross++;
                        }
                        else
                        {
                            isStraightCross = 0;
                        }
                        if (isStraightCross >= 5)
                        {
                            straightCross = 1;
                            break;
                        }
                    }
                }

                if(straightCross == 1)
                {
                    setText用户自定义("十字");
                    if (left[20] == 198 && right[20] == 1
                        && startPull == 0)
                    {
                        startPull = 1;
                        setText用户自定义("开始拉线");
                    }
                    if (startPull == 1)
                    {

                       if(whichTurnInCross == 6)//左转
                       {
                           for (i = 1; i < 55; i++) 
                           {
                               startPullLine = i;
                               if (P_Pixels[i - 1, 198] != 0 && P_Pixels[i, 198] != 0)
                                   break;
                           }
                           for (i = startPullLine; i < 55; i++) 
                           {
                               endPullLine = i;
                               if (P_Pixels[i - 1, 198] == 0 && P_Pixels[i, 198] == 0)
                                   break;
                           }
                           endPullLine = (endPullLine + startPullLine) / 2;
                           cutRow = endPullLine;
                           crossK = (float)(198 - center[0]) / endPullLine;
                           setText用户自定义("拉线 " + crossK);
                       }

                       if (whichTurnInCross == 5)//右转
                       {
                           for (i = 1; i < 55; i++)
                           {
                               startPullLine = i;
                               if (P_Pixels[i - 1, 1] != 0 && P_Pixels[i, 1] != 0)
                                   break;
                           }
                           for (i = startPullLine; i < 55; i++)
                           {
                               endPullLine = i;
                               if (P_Pixels[i - 1, 1] == 0 && P_Pixels[i, 1] == 0)
                                   break;
                           }
                           endPullLine = (endPullLine + startPullLine) / 2;
                           cutRow = endPullLine;
                           crossK = (float)(1 - center[0]) / endPullLine;
                           setText用户自定义("拉线 " + crossK);
                       }

                      
                      
                       for (i = 1; i <= 69; i++)
                       {
                           if (i < endPullLine)
                               center[i] = center[0] + (int)(crossK * i);
                           else
                               center[i] = 255;
                       }
                       if (left[0] == 198 && right[0] == 1 && crossFristRowInto == 0) 
                       {
                           
                           crossFristRowInto = 1;
                       }
                        if(crossFristRowInto == 1)
                        {
                            setText用户自定义("第一行");
                            if(left[0] != 198 && right[0] != 1)
                            {
                                startPull = 0;
                                straightCross = 0;
                            }
                        }
                           
                    }
                }
             #endregion


                #region 扫跳变
                for (i = 4; i < MinNum(cutRow,60); i++)
                {
                    if (right[i] - right[i - 1] > 0
                        && right[i - 1] - right[i - 2] > 0
                        && right[i - 2] - right[i - 3] > 0
                        && right[i - 3] - right[i - 4] > 0)
                        secondRightUpRow = i;
                    if (right[i - 2] - right[i] >= 4
                        && right[i - 3] - right[i - 1] >= 4)
                    {
                        isSecondRightJumpFinish = 1;
                        secondRightJumpRow = i;
                    }
                    if (isSecondRightJumpFinish == 1
                        && secondRightJumpRow != 0
                        && secondRightUpRow != 0)  
                    {
                        isSecondRightJumpFinish = 0;
                        if (secondRightJumpRow - secondRightUpRow <= 20
                            && secondRightJumpRow >= secondRightUpRow) 
                        {
                            rightFindCarFlag = 1;
                            carRow = i;
                            break;
                        }
                            
                    }
                }

                for (i = 4; i < MinNum(cutRow, 60); i++)
                {
                    if (left[i - 1] > left[i] 
                        && left[i - 2] - left[i - 1] > 0
                        && left[i - 3] - left[i - 2] > 0
                        && left[i - 4] - left[i - 3] > 0)
                        secondLeftUpRow = i;
                    if (left[i] - left[i - 2] >= 4
                        && left[i - 1] - left[i - 3] >= 4)
                    {
                        isSecondLeftJumpFinish = 1;
                        secondLeftJumpRow = i;
                    }
                    if (isSecondLeftJumpFinish == 1
                        && secondLeftJumpRow != 0
                        && secondLeftUpRow != 0)
                    {
                        isSecondLeftJumpFinish = 0;
                        if (secondLeftJumpRow - secondLeftUpRow <= 20
                            && secondLeftJumpRow >= secondLeftUpRow)
                        {
                            leftFindCarFlag = 1;
                            carRow = i;
                            break;
                        }

                    }
                }
                #endregion

                #region 右边 判断形状 和距离
                if (rightFindCarFlag == 1)
                {
                    for (i = 3; i < MinNum(cutRow, 60); i++)
                    {
                        if (beginBlack == 0 
                            && P_Pixels[i - 2, right[carRow - 2]] != 0
                            && P_Pixels[i - 3, right[carRow - 2]] != 0)
                        {
                            beginBlack = 1;
                        }
                        if (P_Pixels[i, right[carRow - 2]] != 0
                            && P_Pixels[i - 1, right[carRow - 2]] != 0
                            && P_Pixels[i - 2, right[carRow - 2]] == 0
                            && P_Pixels[i - 3, right[carRow - 2]] == 0
                            && right[carRow - 2] < left[i]
                            && beginBlack == 1)
                        {
                            secondFindBlackWriteRow = i;
                            secondFindBlackWriteFlag = 1;
                        }
                        if (P_Pixels[i, right[carRow - 2]] == 0
                            && P_Pixels[i - 1, right[carRow - 2]] == 0
                            && P_Pixels[i - 2, right[carRow - 2]] != 0
                            && P_Pixels[i - 3, right[carRow - 2]] != 0
                            && right[carRow - 2] < left[i]
                            && secondFindBlackWriteFlag == 1)
                        {
                            secondFindWriteBlackRow = i;
                        }
                        if (secondFindBlackWriteRow != 0
                            && secondFindWriteBlackRow != 0
                            && secondFindBlackWriteRow < secondFindWriteBlackRow
                            && secondFindWriteBlackRow - secondFindBlackWriteRow <= 12)
                        {
                            carFlag = 1;
                            carRow = i;
                            setText用户自定义("右边 形状行数 -2 " + secondFindBlackWriteRow + " " + secondFindWriteBlackRow);
                            break;
                        }
                    }
                    //换一列再捅
                    if (carFlag == 0)
                    {
                        secondFindBlackWriteRow = 0;
                        secondFindWriteBlackRow = 0;
                        secondFindBlackWriteFlag = 0;
                        beginBlack = 0;
                        for (i = 3; i < MinNum(cutRow, 60); i++)
                        {
                            if (beginBlack == 0
                            && P_Pixels[i - 2, right[carRow]] != 0
                            && P_Pixels[i - 3, right[carRow]] != 0)
                            {
                                beginBlack = 1;
                            }
                            if (P_Pixels[i, right[carRow]] != 0
                                && P_Pixels[i - 1, right[carRow]] != 0
                                && P_Pixels[i - 2, right[carRow]] == 0
                                && P_Pixels[i - 3, right[carRow]] == 0
                                && right[carRow] < left[i]
                                && beginBlack == 1)
                            {
                                secondFindBlackWriteRow = i;
                                secondFindBlackWriteFlag = 1;
                            }
                            if (P_Pixels[i, right[carRow]] == 0
                                && P_Pixels[i - 1, right[carRow]] == 0
                                && P_Pixels[i - 2, right[carRow]] != 0
                                && P_Pixels[i - 3, right[carRow]] != 0
                                && right[carRow] < left[i]
                                && secondFindBlackWriteFlag == 1)
                            {
                                secondFindWriteBlackRow = i;
                            }
                            if (secondFindBlackWriteRow != 0
                                && secondFindWriteBlackRow != 0
                                && secondFindBlackWriteRow < secondFindWriteBlackRow
                                && secondFindWriteBlackRow - secondFindBlackWriteRow <= 12)
                            {
                                carFlag = 1;
                                carRow = i;
                                setText用户自定义("右边 形状行数" + secondFindBlackWriteRow + " " + secondFindWriteBlackRow);
                                break;
                            }
                        }
                    }
                }
                #endregion

                #region 左边 判断形状 和距离
                if (leftFindCarFlag == 1)
                {
                    secondFindBlackWriteRow = 0;
                    secondFindWriteBlackRow = 0;
                    secondFindBlackWriteFlag = 0;
                    beginBlack = 0;
                    for (i = 3; i < MinNum(cutRow, 60); i++)
                    {
                        if (beginBlack == 0 
                            && P_Pixels[i - 2, left[carRow - 2]] != 0
                            && P_Pixels[i - 3, left[carRow - 2]] != 0)
                        {
                            beginBlack = 1;
                        }
                        if (P_Pixels[i, left[carRow - 2]] != 0
                            && P_Pixels[i - 1, left[carRow - 2]] != 0
                            && P_Pixels[i - 2, left[carRow - 2]] == 0
                            && P_Pixels[i - 3, left[carRow - 2]] == 0
                            && left[carRow - 2] > right[i]
                            && beginBlack == 1)
                        {
                            secondFindBlackWriteRow = i;
                            secondFindBlackWriteFlag = 1;
                        }
                        if (P_Pixels[i, left[carRow - 2]] == 0
                            && P_Pixels[i - 1, left[carRow - 2]] == 0
                            && P_Pixels[i - 2, left[carRow - 2]] != 0
                            && P_Pixels[i - 3, left[carRow - 2]] != 0
                            && left[carRow - 2] < right[i]
                            && secondFindBlackWriteFlag == 1)
                        {
                            secondFindWriteBlackRow = i;
                        }
                        if (secondFindBlackWriteRow != 0
                            && secondFindWriteBlackRow != 0
                            && secondFindBlackWriteRow < secondFindWriteBlackRow
                            && secondFindWriteBlackRow - secondFindBlackWriteRow <= 12)
                        {
                            carFlag = 1;
                            carRow = i;
                            setText用户自定义("左边 形状行数 -2 " + secondFindBlackWriteRow + " " + secondFindWriteBlackRow);
                            break;
                        }
                    }
                    //换一列再捅
                    if (carFlag == 0)
                    {
                        secondFindBlackWriteRow = 0;
                        secondFindWriteBlackRow = 0;
                        secondFindBlackWriteFlag = 0;
                        for (i = 3; i < MinNum(cutRow, 60); i++)
                        {
                            if (beginBlack == 0 
                                && P_Pixels[i - 2, left[carRow]] != 0
                                && P_Pixels[i - 3, left[carRow]] != 0)
                            {
                                beginBlack = 1;
                            }
                            if (P_Pixels[i, left[carRow]] != 0
                                && P_Pixels[i - 1, left[carRow]] != 0
                                && P_Pixels[i - 2, left[carRow]] == 0
                                && P_Pixels[i - 3, left[carRow]] == 0
                                && left[carRow] > right[i]
                                && beginBlack == 1)
                            {
                                secondFindBlackWriteRow = i;
                                secondFindBlackWriteFlag = 1;
                            }
                            if (P_Pixels[i, left[carRow]] == 0
                                && P_Pixels[i - 1, left[carRow]] == 0
                                && P_Pixels[i - 2, left[carRow]] != 0
                                && P_Pixels[i - 3, left[carRow]] != 0
                                && left[carRow] > right[i]
                                && secondFindBlackWriteFlag == 1)
                            {
                                secondFindWriteBlackRow = i;
                            }
                            if (secondFindBlackWriteRow != 0
                                && secondFindWriteBlackRow != 0
                                && secondFindBlackWriteRow < secondFindWriteBlackRow
                                && secondFindWriteBlackRow - secondFindBlackWriteRow <= 12)
                            {
                                carFlag = 1;
                                carRow = i;
                                setText用户自定义("左边 形状行数" + secondFindBlackWriteRow + " " + secondFindWriteBlackRow);
                                break;
                            }
                        }
                    }
                }
                #endregion

                #region 单边补线
                if(carFlag == 1)
                {
                    if(rightFindCarFlag == 1)
                    {
                        for (i = 1; i < cutRow; i++)
                        {
                            center[i] = center[i - 1] + left[i] - left[i - 1];
                        }
                    }
                    if(leftFindCarFlag == 1)
                    {
                        for (i = 1; i < cutRow; i++)
                        {
                            center[i] = center[i - 1] + right[i] - right[i - 1];
                        }
                    }
                }
                #endregion

                #region  捅线 5次
                if (carFlag == 0)
                {
                    tongXian = center[0];
                    //tongXian[1] = (left[0] * 2 + center[0]) / 3;
                    //tongXian[2] = (right[0] * 2 + center[0]) / 3;
                    //tongXian[3] = (left[0] + center[0] * 2) / 3;
                    //tongXian[4] = (right[0] + center[0] * 2) / 3;
                    //计算捅线点
                    secondFindBlackWriteRow = 0;
                    secondFindWriteBlackRow = 0;
                    secondFindBlackWriteFlag = 0;
                    beginBlack = 0;
                    
                    for (i = 5; i < MinNum(cutRow, 65); i++)
                    {
                        if (beginBlack == 0
                        && P_Pixels[i - 2, tongXian] != 0
                        && P_Pixels[i - 3, tongXian] != 0)
                        {
                            beginBlack = 1;
                        }
                        if (P_Pixels[i, tongXian] != 0
                            && P_Pixels[i - 1, tongXian] != 0
                            && P_Pixels[i - 2, tongXian] == 0
                            && P_Pixels[i - 3, tongXian] == 0
                            && tongXian <= left[i]
                            && tongXian >= right[i]
                            && beginBlack == 1)
                        {
                            secondFindBlackWriteRow = i;
                            secondFindBlackWriteFlag = 1;
                        }
                        if (P_Pixels[i, tongXian] == 0
                            && P_Pixels[i - 1, tongXian] == 0
                            && P_Pixels[i - 2, tongXian] != 0
                            && P_Pixels[i - 3, tongXian] != 0
                            && tongXian <= left[i]
                            && tongXian >= right[i]
                            && secondFindBlackWriteFlag == 1)
                        {
                            secondFindWriteBlackRow = i;
                        }
                        if (secondFindBlackWriteRow != 0
                            && secondFindWriteBlackRow != 0
                            && secondFindBlackWriteRow < secondFindWriteBlackRow
                            && secondFindWriteBlackRow - secondFindBlackWriteRow <= 12)
                        {
                            tongCarFlag = 1;
                            carRow = i;
                            setText用户自定义("捅到线0");
                            break;
                        }
                    }
                }
                /**********换一个点再捅***********/
                if (tongCarFlag == 0)
                {
                    tongXian = (left[0] * 3 + center[0]) / 4;
                    //计算捅线点
                    secondFindBlackWriteRow = 0;
                    secondFindWriteBlackRow = 0;
                    secondFindBlackWriteFlag = 0;
                    beginBlack = 0;

                    for (i = 5; i < MinNum(cutRow, 65); i++)
                    {
                        if (beginBlack == 0
                        && P_Pixels[i - 2, tongXian] != 0
                        && P_Pixels[i - 3, tongXian] != 0)
                        {
                            beginBlack = 1;
                        }
                        if (P_Pixels[i, tongXian] != 0
                            && P_Pixels[i - 1, tongXian] != 0
                            && P_Pixels[i - 2, tongXian] == 0
                            && P_Pixels[i - 3, tongXian] == 0
                            && tongXian <= left[i]
                            && tongXian >= right[i]
                            && beginBlack == 1)
                        {
                            secondFindBlackWriteRow = i;
                            secondFindBlackWriteFlag = 1;
                        }
                        if (P_Pixels[i, tongXian] == 0
                            && P_Pixels[i - 1, tongXian] == 0
                            && P_Pixels[i - 2, tongXian] != 0
                            && P_Pixels[i - 3, tongXian] != 0
                            && tongXian <= left[i]
                            && tongXian >= right[i]
                            && secondFindBlackWriteFlag == 1)
                        {
                            secondFindWriteBlackRow = i;
                        }
                        if (secondFindBlackWriteRow != 0
                            && secondFindWriteBlackRow != 0
                            && secondFindBlackWriteRow < secondFindWriteBlackRow
                            && secondFindWriteBlackRow - secondFindBlackWriteRow <= 12)
                        {
                            tongCarFlag = 1;
                            carRow = i;
                            setText用户自定义("捅到线1");
                            break;
                        }
                    }
                }
                /**********换一个点再捅***********/
                if (tongCarFlag == 0)
                {
                    tongXian = (right[0] * 3 + center[0]) / 4;
                    //计算捅线点
                    secondFindBlackWriteRow = 0;
                    secondFindWriteBlackRow = 0;
                    secondFindBlackWriteFlag = 0;
                    beginBlack = 0;

                    for (i = 5; i < MinNum(cutRow, 65); i++)
                    {
                        if (beginBlack == 0
                        && P_Pixels[i - 2, tongXian] != 0
                        && P_Pixels[i - 3, tongXian] != 0)
                        {
                            beginBlack = 1;
                        }
                        if (P_Pixels[i, tongXian] != 0
                            && P_Pixels[i - 1, tongXian] != 0
                            && P_Pixels[i - 2, tongXian] == 0
                            && P_Pixels[i - 3, tongXian] == 0
                            && tongXian <= left[i]
                            && tongXian >= right[i]
                            && beginBlack == 1)
                        {
                            secondFindBlackWriteRow = i;
                            secondFindBlackWriteFlag = 1;
                        }
                        if (P_Pixels[i, tongXian] == 0
                            && P_Pixels[i - 1, tongXian] == 0
                            && P_Pixels[i - 2, tongXian] != 0
                            && P_Pixels[i - 3, tongXian] != 0
                            && tongXian <= left[i]
                            && tongXian >= right[i]
                            && secondFindBlackWriteFlag == 1)
                        {
                            secondFindWriteBlackRow = i;
                        }
                        if (secondFindBlackWriteRow != 0
                            && secondFindWriteBlackRow != 0
                            && secondFindBlackWriteRow < secondFindWriteBlackRow
                            && secondFindWriteBlackRow - secondFindBlackWriteRow <= 12)
                        {
                            tongCarFlag = 1;
                            carRow = i;
                            setText用户自定义("捅到线2");
                            break;
                        }
                    }
                }
                /**********换一个点再捅***********/
                if (tongCarFlag == 0)
                {
                    tongXian = (right[0]*2 + center[0]*3) / 5;
                    //计算捅线点
                    secondFindBlackWriteRow = 0;
                    secondFindWriteBlackRow = 0;
                    secondFindBlackWriteFlag = 0;
                    beginBlack = 0;

                    for (i = 5; i < MinNum(cutRow, 65); i++)
                    {
                        if (beginBlack == 0
                        && P_Pixels[i - 2, tongXian] != 0
                        && P_Pixels[i - 3, tongXian] != 0)
                        {
                            beginBlack = 1;
                        }
                        if (P_Pixels[i, tongXian] != 0
                            && P_Pixels[i - 1, tongXian] != 0
                            && P_Pixels[i - 2, tongXian] == 0
                            && P_Pixels[i - 3, tongXian] == 0
                            && tongXian <= left[i]
                            && tongXian >= right[i]
                            && beginBlack == 1)
                        {
                            secondFindBlackWriteRow = i;
                            secondFindBlackWriteFlag = 1;
                        }
                        if (P_Pixels[i, tongXian] == 0
                            && P_Pixels[i - 1, tongXian] == 0
                            && P_Pixels[i - 2, tongXian] != 0
                            && P_Pixels[i - 3, tongXian] != 0
                            && tongXian <= left[i]
                            && tongXian >= right[i]
                            && secondFindBlackWriteFlag == 1)
                        {
                            secondFindWriteBlackRow = i;
                        }
                        if (secondFindBlackWriteRow != 0
                            && secondFindWriteBlackRow != 0
                            && secondFindBlackWriteRow < secondFindWriteBlackRow
                            && secondFindWriteBlackRow - secondFindBlackWriteRow <= 12)
                        {
                            tongCarFlag = 1;
                            carRow = i;
                            setText用户自定义("捅到线3");
                            break;
                        }
                    }
                }
                /**********换一个点再捅***********/
                if (tongCarFlag == 0)
                {
                    tongXian = (left[0]*2 + center[0]*3) / 5;
                    //计算捅线点
                    secondFindBlackWriteRow = 0;
                    secondFindWriteBlackRow = 0;
                    secondFindBlackWriteFlag = 0;
                    beginBlack = 0;

                    for (i = 5; i < MinNum(cutRow, 65); i++)
                    {
                        if (beginBlack == 0
                        && P_Pixels[i - 2, tongXian] != 0
                        && P_Pixels[i - 3, tongXian] != 0)
                        {
                            beginBlack = 1;
                        }
                        if (P_Pixels[i, tongXian] != 0
                            && P_Pixels[i - 1, tongXian] != 0
                            && P_Pixels[i - 2, tongXian] == 0
                            && P_Pixels[i - 3, tongXian] == 0
                            && tongXian <= left[i]
                            && tongXian >= right[i]
                            && beginBlack == 1)
                        {
                            secondFindBlackWriteRow = i;
                            secondFindBlackWriteFlag = 1;
                        }
                        if (P_Pixels[i, tongXian] == 0
                            && P_Pixels[i - 1, tongXian] == 0
                            && P_Pixels[i - 2, tongXian] != 0
                            && P_Pixels[i - 3, tongXian] != 0
                            && tongXian <= left[i]
                            && tongXian >= right[i]
                            && secondFindBlackWriteFlag == 1)
                        {
                            secondFindWriteBlackRow = i;
                        }
                        if (secondFindBlackWriteRow != 0
                            && secondFindWriteBlackRow != 0
                            && secondFindBlackWriteRow < secondFindWriteBlackRow
                            && secondFindWriteBlackRow - secondFindBlackWriteRow <= 12)
                        {
                            tongCarFlag = 1;
                            carRow = i;
                            setText用户自定义("捅到线4");
                            break;
                        }
                    }
                }
                #endregion

                if (carFlag == 1 || tongCarFlag == 1) 
                {
                    setText用户自定义("前车在"+carRow+"行");
                }
            }

            #region 显示上位机
            for (LcrChangeCount = 0; LcrChangeCount < 70; LcrChangeCount++)
                {
                    Lcr[LcrChangeCount].LBlack = (byte)left[LcrChangeCount];
                }
            for (LcrChangeCount = 0; LcrChangeCount < 70; LcrChangeCount++)
            {
                Lcr[LcrChangeCount].RBlack = (byte)right[LcrChangeCount];
            }
            for (LcrChangeCount = 0; LcrChangeCount < 70; LcrChangeCount++)
            {
                Lcr[LcrChangeCount].Center = (byte)center[LcrChangeCount];
            }
            for (LcrChangeCount = 0; LcrChangeCount < 70; LcrChangeCount++)
            {
                Lcr[LcrChangeCount].LStart = (byte)LStart[LcrChangeCount];
            }
            for (LcrChangeCount = 0; LcrChangeCount < 70; LcrChangeCount++)
            {
                Lcr[LcrChangeCount].RStart = (byte)RStart[LcrChangeCount];
            }
            #endregion
        }
        public void SignalProcess()//信息处理
        {
            Image_Init();//变量初始化 
            
            for (RowNum = 0; RowNum < Image_Height; RowNum++) //循环叠加
            {
                get_P_from_Pixels();//获取该行像素指针P

                ImageProcess(); //图像行处理 内联函数         

            }
            FieldProcess();//图像场处理函数内联
        }

        #region 系统获取P行的函数 用户不用关心
        void get_P_from_Pixels()//获取该行像素指针P
        {
            P = new byte[Image_Width];
            for (int i = 0; i < Image_Width - 1; i++)
            {
                P[i] = P_Pixels[RowNum, i];
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
            //double[] pfit = MyMath.Filter.polyfit(tmpy, tmpx, MyNrf.Form1.Polyfit);
            //for (int i = 0; i < lcrcnt; i++)
            //{
            //    Lcr[i].CenterBack = (byte)MyMath.Filter.MyPolyVal(pfit, tmpy[i]);
            //}
        }//系统最小二乘法
        private int CheckLcr()
        {
            int tmpcnt=0,cnt=0;
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
                if(cnt>5)
                {      
                    tmpcnt=i-cnt;
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
