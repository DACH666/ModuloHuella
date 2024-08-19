using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace ModuloHuella
{
    class FPutils_x86
    {



        /** @func   : FPModule_OpenDevice
         *  @brief  : 连接设备
         *  @param  : None
         *  @return : 0->连接成功 1->通信失败
         */
        // 检测设备
        [DllImport(@"FPModule_SDK.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int FPModule_OpenDevice();


        /** @func   : FPModule_CloseDevice
         *  @brief  : 断开设备
         *  @param  : None
         *  @return : 0->断开成功 1->通信失败
         */
        [DllImport(@"FPModule_SDK.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int FPModule_CloseDevice();


        /** @func   : FPModule_DetectFinger
         *  @brief  : 检测指纹输入状态
         *  @param  : pdwFpstatus[out] -> 0:无指纹输入  1:有指纹输入
         *  @return : 0->执行成功 1->通信失败
         */
        [DllImport(@"FPModule_SDK.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int FPModule_DetectFinger(ref int pdwFpstatus);


        /** @func   : FPModule_CaptureImage
         *  @brief  : 采集指纹图像
         *  @param  : pbyImageData[out] -> 指纹图像数据（数据长度为 图像宽度 x 图像高度）
                      pdwWidth[out]     -> 指纹图像宽度
                      pdwHeight[out]    -> 指纹图像高度
         *  @return : 0->执行成功 1->通信失败
         */
        [DllImport(@"FPModule_SDK.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int FPModule_CaptureImage(byte[] pbyImageData, ref int pdwWidth, ref int pdwHeight);


        /** @func   : FPModule_SetTimeout
         *  @brief  : 设置采集超时时间
         *  @param  : dwSecond[in] -> 超时时间(单位：秒) 可设置值：1秒至60秒
         *  @return : 0->执行成功 1->通信失败 
         */
        [DllImport(@"FPModule_SDK.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int FPModule_SetTimeout(int dwSecond);


        /** @func   : FPModule_GetTimeout
         *  @brief  : 获取采集超时时间
         *  @param  : pdwSecond[out] -> 超时时间 单位：秒
         *  @return : 0->执行成功 1->通信失败 
         */
        [DllImport(@"FPModule_SDK.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int FPModule_GetTimeout(ref int pdwSecond);


        /** @func   : FPModule_SetCollectTimes
*  @brief  : 设置采集次数
*  @param  : dwTimes[in] -> 0~4,0默认模式（2~4次），1~3采集次数
*  @return : 0->执行成功 1->通信失败
*/
        [DllImport(@"FPModule_SDK.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int FPModule_SetCollectTimes(int dwTimes);


        /** @func   : FPModule_GetCollectTimes
        *  @brief  : 获取采集次数
        *  @param  : pdwTimes[out] -> 采集次数
        *  @return : 0->执行成功 1->通信失败
        */
        [DllImport(@"FPModule_SDK.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int FPModule_GetCollectTimes(ref int pdwTimes);

        /** @func   : FPModule_InstallMessageHandler
         *  @brief  : 设置消息回调函数
         *  @param  : msgHandler[in] -> 消息处理函数
         *  @return : 0->执行成功
         */
        [DllImport(@"FPModule_SDK.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int FPModule_InstallMessageHandler(FPMsg.FpMessageHandler msgHandler);


        /** @func   : FPModule_FpEnroll
         *  @brief  : 录入指纹
         *  @param  : pbyFpTemplate[out] -> 指纹模板(512字节)
         *  @return : 0->执行成功 1->通信失败 2->采集超时 3->录入失败
         */
        [DllImport(@"FPModule_SDK.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int FPModule_FpEnroll(byte[] pbyFpTemplate);


        /** @func   : FPModule_GetQuality
         *  @brief  : 获取指纹模板质量分数
         *  @param  : pbyFpTemplate[in] -> 指纹模板(512字节)
         *  @return : 指纹模板分数(0~100) 分数越高，表示模板的质量越好
         */
        [DllImport(@"FPModule_SDK.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int FPModule_GetQuality(byte[] pbyFpTemplate);

        /** @func   : FPModule_MatchTemplate
         *  @brief  : 比对两枚指纹模板
         *  @param  : pbyFpTemplate1[in] -> 指纹模板1(512字节)
                      pbyFpTemplate2[in] -> 指纹模板2(512字节)
                      dwSecurityLevel[in] -> 安全等级（1~5）
         *  @return : 0->比对成功 6->比对失败 4->参数错误
         */
        [DllImport(@"FPModule_SDK.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int FPModule_MatchTemplate(byte[] pbyFpTemplate1, byte[] pbyFpTemplate2, int dwSecurityLevel);

        /** @func   : FPModule_GetDeviceInfo
         *  @brief  : 获取指纹采集仪版本信息
         *  @param  : pbyDeviceInfo[out] -> 指纹采集仪版本信息(64字节)
         *  @return : 0->执行成功 1->通信失败 
         */
        [DllImport(@"FPModule_SDK.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int FPModule_GetDeviceInfo(byte[] pbyDeviceInfo);


        /** @func   : FPModule_GetSDKVersion
         *  @brief  : 获取指纹采集仪SDK版本信息
         *  @param  : pbySDKVersion[out] -> 指纹采集仪SDK版本信息(64字节)
         *  @return : 0->执行成功
         */
        [DllImport(@"FPModule_SDK.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int FPModule_GetSDKVersion(byte[] pbySDKVersion);

        public static int ImgBufferToBmpBuffer(byte[] src, int X, int Y, byte[] des)
        {
            int num;
            int i, j;
            byte[] head = new byte[1100];

            // 定义BMP1078格式头
            byte[] bmpHead = { 
		        /***************************/
		        //file header
		        0x42,0x4d,//file type 
			        //0x36,0x6c,0x01,0x00, //file size***
			        0x0,0x0,0x0,0x00, //file size***
			        0x00,0x00, //reserved
			        0x00,0x00,//reserved
			        0x36,0x4,0x00,0x00,//head byte***
			        /***************************/
			        //infoheader
			        0x28,0x00,0x00,0x00,//struct size
        			
			        //0x00,0x01,0x00,0x00,//map width*** 
			        0x00,0x00,0x00,0x00,//map width*** 
			        //0x68,0x01,0x00,0x00,//map height***
			        0x00,0x00,0x00,0x00,//map height***
        			
			        0x01,0x00,//must be 1
			        0x08,0x00,//color count***
			        0x00,0x00,0x00,0x00, //compression
			        //0x00,0x68,0x01,0x00,//data size***
			        0x00,0x00,0x00,0x00,//data size***
			        0x00,0x00,0x00,0x00, //dpix
			        0x00,0x00,0x00,0x00, //dpiy
			        0x00,0x00,0x00,0x00,//color used
			        0x00,0x00,0x00,0x00,//color important  
      			    0x00,0x00,0x00,0x00,//color important 越界处理
	        };

            int len = bmpHead.Length;

            Buffer.BlockCopy(bmpHead, 0, head, 0, bmpHead.Length);

            //确定图象宽度数值
            num = X; head[18] = (byte)(num & 0xFF);
            num = num >> 8; head[19] = (byte)(num & 0xFF);
            num = num >> 8; head[20] = (byte)(num & 0xFF);
            num = num >> 8; head[21] = (byte)(num & 0xFF);
            //确定图象高度数值
            num = Y; head[22] = (byte)(num & 0xFF);
            num = num >> 8; head[23] = (byte)(num & 0xFF);
            num = num >> 8; head[24] = (byte)(num & 0xFF);
            num = num >> 8; head[25] = (byte)(num & 0xFF);

            //确定调色板数值
            j = 0;
            for (i = 54; i < 1078; i = i + 4)
            {
                head[i] = head[i + 1] = head[i + 2] = (byte)j;
                head[i + 3] = 0;
                j++;
            }

            // 复制head头数据
            Buffer.BlockCopy(head, 0, des, 0, 1078);

            //写入图象数据
            for (i = 0; i < Y; i++)
            {
                Buffer.BlockCopy(src, i * X, des, 1078 + (Y - 1 - i) * X, X);
            }

            return 1;
        }

    }
}
