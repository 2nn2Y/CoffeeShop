using System.Collections.Generic;

namespace YT
{
    /// <summary>
    /// 全局静态字段定义
    /// </summary>
    public class YtConsts
    {
        /// <summary>
        /// 本地化参考值
        /// </summary>
        public const string LocalizationSourceName = "YT";
        /// <summary>
        /// 是否开启多租户
        /// </summary>
        public const bool MultiTenancyEnabled = false;
        public static  List<WarnType> Types => new List<WarnType>()
        {
            new WarnType("ECB_03", "	Tomuchcurrentincircuit3	", "		电路3电流过大	"),
            new WarnType("ECE_2", "	SlaveI/Oataddress#2doesnotrespond	", "		IO板2不响应	"),
            new WarnType("EC_2", "	Nomachineconnected	", "		没有IO板连接	"),
            new WarnType("EC_3", "	Systemnoready	", "		系统未就绪	"),
            new WarnType("EC_4", "	Nopaymentdevice	", "		无支付设备	"),
            new WarnType("EC_6", "	NOinternet	", "		请检查网络连接	"),
            new WarnType("EC_7", "	pleasecheckip?port?machinenumber	", "		网络设置错误检查IP,Port,机器号	"),
            new WarnType("EI_2", "	Communicationwithsevererror	", "		通讯设备通讯或协议错误	"),
            new WarnType("EJA_01", "	Doormotornotdetect.	", "		未检测到大门电机	"),
            new WarnType("EJA_02", "	Doormotorrunerror.	", "		大门电机故障	"),
            new WarnType("ERROR:0081", "mixermotor1opencircuit	", "		搅拌电机1开路	"),
            new WarnType("ERROR:0088", "mixermotor1blocked	", "		搅拌电机1堵转	"),
            new WarnType("ERROR:0181", "mixermotor2opencircuit	", "		搅拌电机2开路	"),
            new WarnType("ERROR:0188", "mixermotor2blocked	", "		搅拌电机2堵转	"),
            new WarnType("ERROR:0281", "mixermotor3opencircuit	", "		搅拌电机3开路	"),
            new WarnType("ERROR:0288", "mixermotor3blocked	", "		搅拌电机3堵转	"),
            new WarnType("ERROR:0381", "ERROR:0381	", "		搅拌电机4开路	"),
            new WarnType("ERROR:0388", "ERROR:0388	", "		搅拌电机4堵转	"),
            new WarnType("ERROR:0681", "ERROR:0681	", "		空气泵开路	"),
            new WarnType("ERROR:0688", "ERROR:0688	", "		空气泵堵转	"),
            new WarnType("ERROR:0881", "canistermotor1opencircuit	", "		料盒电机1开路	"),
            new WarnType("ERROR:0888", "canistermotor1blocked	", "		料盒电机1堵转	"),
            new WarnType("ERROR:0981", "canistermotor2opencircuit	", "		料盒电机2开路	"),
            new WarnType("ERROR:0988", "canistermotor2blocked	", "		料盒电机2堵转	"),
            new WarnType("ERROR:0A81", "canistermotor3opencircuit	", "		料盒电机3开路	"),
            new WarnType("ERROR:0A88", "canistermotor3blocked	", "		料盒电机3堵转	"),
            new WarnType("ERROR:0B81", "canistermotor4opencircuit	", "		料盒电机4开路	"),
            new WarnType("ERROR:0B88", "canistermotor4blocked	", "		料盒电机4堵转	"),
            new WarnType("ERROR:0C81", "canistermotor5opencircuit	", "		料盒电机5开路	"),
            new WarnType("ERROR:0C88", "canistermotor5blocked	", "		料盒电机5堵转	"),
            new WarnType("ERROR:0D81", "canistermotor6opencircuit	", "		料盒电机6开路	"),
            new WarnType("ERROR:0D88", "canistermotor6blocked	", "		料盒电机6堵转	"),
            new WarnType("ERROR:0E81", "ERROR:0E81	", "		料盒电机7开路	"),
            new WarnType("ERROR:0E88", "ERROR:0E88	", "		料盒电机7堵转	"),
            new WarnType("ERROR:0F81", "ERROR:0F81	", "		料盒电机8开路	"),
            new WarnType("ERROR:0F88", "ERROR:0F88	", "		料盒电机8堵转	"),
            new WarnType("ERROR:1081", "ESchambermotoropencircuit	", "		ES挤饼电机开路	"),
            new WarnType("ERROR:1088", "ESchambermotorblocked	", "		ES挤饼电机堵转	"),
            new WarnType("ERROR:1181", "ESsealingmotoropencircuit	", "		ES密封电机开路	"),
            new WarnType("ERROR:1188", "ESsealingmotorblocked	", "		ES密封电机堵转	"),
            new WarnType("ERROR:1281", "FBwipingmotoropencircuit	", "		FB刮片电机开路	"),
            new WarnType("ERROR:1288", "FBwipingmotorblocked	", "		FB刮片电机堵转	"),
            new WarnType("ERROR:1381", "FBsealingmotoropencircuit	", "		FB密封电机开路	"),
            new WarnType("ERROR:1388", "FBsealingmotorblocked	", "		FB密封电机堵转	"),
            new WarnType("ERROR:1681", "gearpumpopencircuit	", "		齿轮泵开路	"),
            new WarnType("ERROR:1688", "ERROR:1688	", "		齿轮泵堵转	"),
            new WarnType("ERROR:1881", "cupdispensemotoropencircuit	", "		分杯电机开路	"),
            new WarnType("ERROR:1888", "cupdispensemotorblocked	", "		分杯电机堵转	"),
            new WarnType("ERROR:1981", "cuptubemotoropencircuit	", "		杯桶电机开路	"),
            new WarnType("ERROR:1988", "cuptubemotorblocked	", "		杯桶电机堵转	"),
            new WarnType("ERROR:1A81", "cupcatchermotoropencircuit	", "		运杯电机开路	"),
            new WarnType("ERROR:1A88", "cupcatchermotorblocked	", "		运杯电机堵转	"),
            new WarnType("ERROR:1B81", "doormotoropencircuit	", "		大门电机开路	"),
            new WarnType("ERROR:1B88", "ERROR:1B88	", "		大门电机堵转	"),
            new WarnType("ERROR:1C81", "smalldoormotoropencircuit	", "		小门电机开路	"),
            new WarnType("ERROR:1C88", "ERROR:1C88	", "		小门电机堵转	"),
            new WarnType("ERROR:2081", "normaltemp.watervalveopencircuit	", "		常温进水阀开路	"),
            new WarnType("ERROR:2181", "coldwaterinletvalveopencircuit	", "		冷水进水阀开路	"),
            new WarnType("ERROR:2281", "ES2p3wvalveopencircuit	", "		ES二位三通阀开路	"),
            new WarnType("ERROR:2981", "2p2wvalve2opencircuit	", "		二位二通阀2开路	"),
            new WarnType("ERROR:2A81", "2p2wvalve3opencircuit	", "		二位二通阀3开路	"),
            new WarnType("ERROR:2B81", "2p2wvalve4opencircuit	", "		二位二通阀4开路	"),
            new WarnType("ERROR:2C81", "2p2wvalve5opencircuit	", "		二位二通阀5开路	"),
            new WarnType("ERROR:3581", "airbreakprobesensorstayon	", "		水盒探针传感器一直开	"),
            new WarnType("ERROR:3A81", "cupdispenserpositionsensorstayopen	", "		分杯马达位置传感器一直开	"),
            new WarnType("ERROR:3B00", "ERROR:3B00	", "		杯桶旋转传感器异常	"),
            new WarnType("ERROR:3B80", "cuprorationsensorstayoff	", "		杯桶旋转传感器一直关	"),
            new WarnType("ERROR:3B81", "cuprotationsensorstayon	", "		杯桶旋转传感器一直开	"),
            new WarnType("ERROR:3C81", "cupcatcherswitchmotorsensor1stayon	", "		运杯微动传感器1一直开	"),
            new WarnType("ERROR:3D81", "cupcatcherswitchmotorsensor2stayon	", "		运杯微动传感器2一直开	"),
            new WarnType("ERROR:4381", "FBwipingsensorstayon	", "		FB刮渣传感器一直开	"),
            new WarnType("ERROR:4480", "FBsealingsensorstayoff	", "		FB密封传感器一直	"),
            new WarnType("ERROR:4481", "FBsealingsensorstayopen	", "		FB密封传感器一直开	"),
            new WarnType("ERROR:5200", "ERROR:5200	", "电路板电流过大	"),
            new WarnType("ERROR:5201", "ERROR:5201	", "电路板电流过大	"),
            new WarnType("ERROR:5202", "ERROR:5202	", "电路板电流过大	"),
            new WarnType("ERROR:5206", "ERROR:5206	", "电路板电流过大	"),
            new WarnType("ERROR:5208", "ERROR:5208	", "电路板电流过大	"),
            new WarnType("ERROR:520B", "ERROR:520B	", "电路板电流过大	"),
            new WarnType("ERROR:520C", "ERROR:520C	", "电路板电流过大	"),
            new WarnType("ERROR:5210", "ERROR:5210	", "电路板电流过大	"),
            new WarnType("ERROR:5212", "ERROR:5212	", "电路板电流过大	"),
            new WarnType("ERROR:5214", "ERROR:5214	", "电路板电流过大	"),
            new WarnType("ERROR:5300", "nocups	", "		缺杯子	"),
            new WarnType("ERROR:5406", "ERROR:5406	", "		分杯失败多次	"),
            new WarnType("ERROR:5500", "cupholdercan'tmovetocorrectposition	", "		杯托移动不到位	"),
            new WarnType("ERROR:5600", "extracupsbeforemakingproducts	", "		做产品前有多余杯子	"),
            new WarnType("ERROR:5700", "nobeans	", "缺咖啡豆	"),
            new WarnType("ERROR:5901", "nowater1", "净水缺乏1	"),
            new WarnType("ERROR:5902", "nowater2", "净水缺乏2	"),
            new WarnType("ERROR:5A00", "wastewaterbinfull", "		废水桶满	"),
            new WarnType("ERROR:5B00", "driptreynotwell-installed	", "		接水托盘安装不到位	"),
            new WarnType("ERROR:5C00", "flowmetererror	", "流量计故障	"),
            new WarnType("ERROR:5C07", "ERROR:5C07	", "流量计故障	"),
            new WarnType("ERROR:5CFF", "ERROR:5CFF	", "流量计故障	"),
            new WarnType("ERROR:5D00", "boilertemp.sensorerror	", "锅炉温度传感器故障	"),
            new WarnType("ERROR:5DFF", "ERROR:5DFF	", "锅炉温度传感器故障	"),
            new WarnType("ERROR:6100", "ERROR:6100	", "锅炉温度过低	"),
            new WarnType("ERROR:6D00", "executivecomponenterror0whilemakingproduct", "做产品时执行部件异常0	"),
            new WarnType("ERROR:7000", "", "CUP板连接异常"),
            new WarnType("ERROR:7200", "airbreakfilledovertime	", "水箱填充超时"),
            new WarnType("ERROR:7300", "noboilerconnected", "锅炉未连接"),
            new WarnType("ERROR:7600", "ESbrewerboardconnecterror", "Brewer板连接错误"),
            new WarnType("ERROR:7700", "Esbrewermovementerror0", "咖啡酿造器动作错误0"),
            new WarnType("ERROR:7701", "ESbrewermovementerror1", "咖啡酿造器动作错误1"),
            new WarnType("ERROR:7702", "ESbrewermovementerror2", "咖啡酿造器动作错误2"),
            new WarnType("ERROR:7703", "ESbrewermovementerror3", "咖啡酿造器动作错误3"),
            new WarnType("ERROR:7704", "ESbrewermovementerror4", "咖啡酿造器动作错误4"),
            new WarnType("ERROR:7705", "ESbrewermovementerror5", "咖啡酿造器动作错误5"),
            new WarnType("ERROR:7707", "ESbrewermovementerror7", "咖啡酿造器动作错误7"),
            new WarnType("ERROR:7706", "ESbrewermovementerror6", "咖啡酿造器动作错误6"),
            new WarnType("ERROR:7708", "ESbrewermovementerror8", "咖啡酿造器动作错误8"),
            new WarnType("ERROR:7902", "FBbrewermovementerror2", "泡茶器动作错误2"),
            new WarnType("ERROR:7906", "FBbrewermovementerror6", "泡茶器动作错误6	"),
            new WarnType("ERROR:7A20", "ERROR:7A20	", "压力传感器异常"),
            new WarnType("ERROR:7AA5", "ERROR:7AA5	", "压力传感器异常"),
            new WarnType("ERROR:7AA7", "ERROR:7AA7	", "压力传感器异常"),
            new WarnType("ERROR:CUP_01", "Cupblocksthetray.	", "产品没被取走"),
            new WarnType("HOPPER_2", "HOPPER_2", "退币器故障"),
            new WarnType("INT_1", "	Cannotconnecttoserver.	", "请检查网络连接"),
            new WarnType("warning:01", "sugarisnotenough!	", "缺糖预警（尚余少量）"),
            new WarnType("warning:02", "ESwaterisnotenough!	", "缺水预警（尚余少量）"),
            new WarnType("warning:03", "Cupisnotenough!	", "缺杯预警（尚余少量）"),
            new WarnType("WARNING:0A", "Dooropened.	", "大门打开	")
        };
    }
    /// <summary>
    /// 警告类型
    /// </summary>
    public class WarnType
    {
        /// <summary>
        /// 
        /// </summary>
        public WarnType() { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <param name="e"></param>
        /// <param name="c"></param>
        public WarnType(string t, string e, string c)
        {
            Type = t;
            English = e;
            Chinese = c;
        }
        /// <summary>
        /// 类型
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 英文
        /// </summary>
        public string English { get; set; }
        /// <summary>
        /// 中文
        /// </summary>
        public string Chinese { get; set; }
    }

}