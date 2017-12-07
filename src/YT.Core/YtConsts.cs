using System.Collections.Generic;

namespace YT
{
    /// <summary>
    /// ȫ�־�̬�ֶζ���
    /// </summary>
    public class YtConsts
    {
        /// <summary>
        /// ���ػ��ο�ֵ
        /// </summary>
        public const string LocalizationSourceName = "YT";
        /// <summary>
        /// �Ƿ������⻧
        /// </summary>
        public const bool MultiTenancyEnabled = false;
        public static  List<WarnType> Types => new List<WarnType>()
        {
            new WarnType("ECB_03", "	Tomuchcurrentincircuit3	", "		��·3��������	"),
            new WarnType("ECE_2", "	SlaveI/Oataddress#2doesnotrespond	", "		IO��2����Ӧ	"),
            new WarnType("EC_2", "	Nomachineconnected	", "		û��IO������	"),
            new WarnType("EC_3", "	Systemnoready	", "		ϵͳδ����	"),
            new WarnType("EC_4", "	Nopaymentdevice	", "		��֧���豸	"),
            new WarnType("EC_6", "	NOinternet	", "		������������	"),
            new WarnType("EC_7", "	pleasecheckip?port?machinenumber	", "		�������ô�����IP,Port,������	"),
            new WarnType("EI_2", "	Communicationwithsevererror	", "		ͨѶ�豸ͨѶ��Э�����	"),
            new WarnType("EJA_01", "	Doormotornotdetect.	", "		δ��⵽���ŵ��	"),
            new WarnType("EJA_02", "	Doormotorrunerror.	", "		���ŵ������	"),
            new WarnType("ERROR:0081", "mixermotor1opencircuit	", "		������1��·	"),
            new WarnType("ERROR:0088", "mixermotor1blocked	", "		������1��ת	"),
            new WarnType("ERROR:0181", "mixermotor2opencircuit	", "		������2��·	"),
            new WarnType("ERROR:0188", "mixermotor2blocked	", "		������2��ת	"),
            new WarnType("ERROR:0281", "mixermotor3opencircuit	", "		������3��·	"),
            new WarnType("ERROR:0288", "mixermotor3blocked	", "		������3��ת	"),
            new WarnType("ERROR:0381", "ERROR:0381	", "		������4��·	"),
            new WarnType("ERROR:0388", "ERROR:0388	", "		������4��ת	"),
            new WarnType("ERROR:0681", "ERROR:0681	", "		�����ÿ�·	"),
            new WarnType("ERROR:0688", "ERROR:0688	", "		�����ö�ת	"),
            new WarnType("ERROR:0881", "canistermotor1opencircuit	", "		�Ϻе��1��·	"),
            new WarnType("ERROR:0888", "canistermotor1blocked	", "		�Ϻе��1��ת	"),
            new WarnType("ERROR:0981", "canistermotor2opencircuit	", "		�Ϻе��2��·	"),
            new WarnType("ERROR:0988", "canistermotor2blocked	", "		�Ϻе��2��ת	"),
            new WarnType("ERROR:0A81", "canistermotor3opencircuit	", "		�Ϻе��3��·	"),
            new WarnType("ERROR:0A88", "canistermotor3blocked	", "		�Ϻе��3��ת	"),
            new WarnType("ERROR:0B81", "canistermotor4opencircuit	", "		�Ϻе��4��·	"),
            new WarnType("ERROR:0B88", "canistermotor4blocked	", "		�Ϻе��4��ת	"),
            new WarnType("ERROR:0C81", "canistermotor5opencircuit	", "		�Ϻе��5��·	"),
            new WarnType("ERROR:0C88", "canistermotor5blocked	", "		�Ϻе��5��ת	"),
            new WarnType("ERROR:0D81", "canistermotor6opencircuit	", "		�Ϻе��6��·	"),
            new WarnType("ERROR:0D88", "canistermotor6blocked	", "		�Ϻе��6��ת	"),
            new WarnType("ERROR:0E81", "ERROR:0E81	", "		�Ϻе��7��·	"),
            new WarnType("ERROR:0E88", "ERROR:0E88	", "		�Ϻе��7��ת	"),
            new WarnType("ERROR:0F81", "ERROR:0F81	", "		�Ϻе��8��·	"),
            new WarnType("ERROR:0F88", "ERROR:0F88	", "		�Ϻе��8��ת	"),
            new WarnType("ERROR:1081", "ESchambermotoropencircuit	", "		ES���������·	"),
            new WarnType("ERROR:1088", "ESchambermotorblocked	", "		ES���������ת	"),
            new WarnType("ERROR:1181", "ESsealingmotoropencircuit	", "		ES�ܷ�����·	"),
            new WarnType("ERROR:1188", "ESsealingmotorblocked	", "		ES�ܷ�����ת	"),
            new WarnType("ERROR:1281", "FBwipingmotoropencircuit	", "		FB��Ƭ�����·	"),
            new WarnType("ERROR:1288", "FBwipingmotorblocked	", "		FB��Ƭ�����ת	"),
            new WarnType("ERROR:1381", "FBsealingmotoropencircuit	", "		FB�ܷ�����·	"),
            new WarnType("ERROR:1388", "FBsealingmotorblocked	", "		FB�ܷ�����ת	"),
            new WarnType("ERROR:1681", "gearpumpopencircuit	", "		���ֱÿ�·	"),
            new WarnType("ERROR:1688", "ERROR:1688	", "		���ֱö�ת	"),
            new WarnType("ERROR:1881", "cupdispensemotoropencircuit	", "		�ֱ������·	"),
            new WarnType("ERROR:1888", "cupdispensemotorblocked	", "		�ֱ������ת	"),
            new WarnType("ERROR:1981", "cuptubemotoropencircuit	", "		��Ͱ�����·	"),
            new WarnType("ERROR:1988", "cuptubemotorblocked	", "		��Ͱ�����ת	"),
            new WarnType("ERROR:1A81", "cupcatchermotoropencircuit	", "		�˱������·	"),
            new WarnType("ERROR:1A88", "cupcatchermotorblocked	", "		�˱������ת	"),
            new WarnType("ERROR:1B81", "doormotoropencircuit	", "		���ŵ����·	"),
            new WarnType("ERROR:1B88", "ERROR:1B88	", "		���ŵ����ת	"),
            new WarnType("ERROR:1C81", "smalldoormotoropencircuit	", "		С�ŵ����·	"),
            new WarnType("ERROR:1C88", "ERROR:1C88	", "		С�ŵ����ת	"),
            new WarnType("ERROR:2081", "normaltemp.watervalveopencircuit	", "		���½�ˮ����·	"),
            new WarnType("ERROR:2181", "coldwaterinletvalveopencircuit	", "		��ˮ��ˮ����·	"),
            new WarnType("ERROR:2281", "ES2p3wvalveopencircuit	", "		ES��λ��ͨ����·	"),
            new WarnType("ERROR:2981", "2p2wvalve2opencircuit	", "		��λ��ͨ��2��·	"),
            new WarnType("ERROR:2A81", "2p2wvalve3opencircuit	", "		��λ��ͨ��3��·	"),
            new WarnType("ERROR:2B81", "2p2wvalve4opencircuit	", "		��λ��ͨ��4��·	"),
            new WarnType("ERROR:2C81", "2p2wvalve5opencircuit	", "		��λ��ͨ��5��·	"),
            new WarnType("ERROR:3581", "airbreakprobesensorstayon	", "		ˮ��̽�봫����һֱ��	"),
            new WarnType("ERROR:3A81", "cupdispenserpositionsensorstayopen	", "		�ֱ����λ�ô�����һֱ��	"),
            new WarnType("ERROR:3B00", "ERROR:3B00	", "		��Ͱ��ת�������쳣	"),
            new WarnType("ERROR:3B80", "cuprorationsensorstayoff	", "		��Ͱ��ת������һֱ��	"),
            new WarnType("ERROR:3B81", "cuprotationsensorstayon	", "		��Ͱ��ת������һֱ��	"),
            new WarnType("ERROR:3C81", "cupcatcherswitchmotorsensor1stayon	", "		�˱�΢��������1һֱ��	"),
            new WarnType("ERROR:3D81", "cupcatcherswitchmotorsensor2stayon	", "		�˱�΢��������2һֱ��	"),
            new WarnType("ERROR:4381", "FBwipingsensorstayon	", "		FB����������һֱ��	"),
            new WarnType("ERROR:4480", "FBsealingsensorstayoff	", "		FB�ܷ⴫����һֱ	"),
            new WarnType("ERROR:4481", "FBsealingsensorstayopen	", "		FB�ܷ⴫����һֱ��	"),
            new WarnType("ERROR:5200", "ERROR:5200	", "��·���������	"),
            new WarnType("ERROR:5201", "ERROR:5201	", "��·���������	"),
            new WarnType("ERROR:5202", "ERROR:5202	", "��·���������	"),
            new WarnType("ERROR:5206", "ERROR:5206	", "��·���������	"),
            new WarnType("ERROR:5208", "ERROR:5208	", "��·���������	"),
            new WarnType("ERROR:520B", "ERROR:520B	", "��·���������	"),
            new WarnType("ERROR:520C", "ERROR:520C	", "��·���������	"),
            new WarnType("ERROR:5210", "ERROR:5210	", "��·���������	"),
            new WarnType("ERROR:5212", "ERROR:5212	", "��·���������	"),
            new WarnType("ERROR:5214", "ERROR:5214	", "��·���������	"),
            new WarnType("ERROR:5300", "nocups	", "		ȱ����	"),
            new WarnType("ERROR:5406", "ERROR:5406	", "		�ֱ�ʧ�ܶ��	"),
            new WarnType("ERROR:5500", "cupholdercan'tmovetocorrectposition	", "		�����ƶ�����λ	"),
            new WarnType("ERROR:5600", "extracupsbeforemakingproducts	", "		����Ʒǰ�ж��౭��	"),
            new WarnType("ERROR:5700", "nobeans	", "ȱ���ȶ�	"),
            new WarnType("ERROR:5901", "nowater1", "��ˮȱ��1	"),
            new WarnType("ERROR:5902", "nowater2", "��ˮȱ��2	"),
            new WarnType("ERROR:5A00", "wastewaterbinfull", "		��ˮͰ��	"),
            new WarnType("ERROR:5B00", "driptreynotwell-installed	", "		��ˮ���̰�װ����λ	"),
            new WarnType("ERROR:5C00", "flowmetererror	", "�����ƹ���	"),
            new WarnType("ERROR:5C07", "ERROR:5C07	", "�����ƹ���	"),
            new WarnType("ERROR:5CFF", "ERROR:5CFF	", "�����ƹ���	"),
            new WarnType("ERROR:5D00", "boilertemp.sensorerror	", "��¯�¶ȴ���������	"),
            new WarnType("ERROR:5DFF", "ERROR:5DFF	", "��¯�¶ȴ���������	"),
            new WarnType("ERROR:6100", "ERROR:6100	", "��¯�¶ȹ���	"),
            new WarnType("ERROR:6D00", "executivecomponenterror0whilemakingproduct", "����Ʒʱִ�в����쳣0	"),
            new WarnType("ERROR:7000", "", "CUP�������쳣"),
            new WarnType("ERROR:7200", "airbreakfilledovertime	", "ˮ����䳬ʱ"),
            new WarnType("ERROR:7300", "noboilerconnected", "��¯δ����"),
            new WarnType("ERROR:7600", "ESbrewerboardconnecterror", "Brewer�����Ӵ���"),
            new WarnType("ERROR:7700", "Esbrewermovementerror0", "������������������0"),
            new WarnType("ERROR:7701", "ESbrewermovementerror1", "������������������1"),
            new WarnType("ERROR:7702", "ESbrewermovementerror2", "������������������2"),
            new WarnType("ERROR:7703", "ESbrewermovementerror3", "������������������3"),
            new WarnType("ERROR:7704", "ESbrewermovementerror4", "������������������4"),
            new WarnType("ERROR:7705", "ESbrewermovementerror5", "������������������5"),
            new WarnType("ERROR:7707", "ESbrewermovementerror7", "������������������7"),
            new WarnType("ERROR:7706", "ESbrewermovementerror6", "������������������6"),
            new WarnType("ERROR:7708", "ESbrewermovementerror8", "������������������8"),
            new WarnType("ERROR:7902", "FBbrewermovementerror2", "�ݲ�����������2"),
            new WarnType("ERROR:7906", "FBbrewermovementerror6", "�ݲ�����������6	"),
            new WarnType("ERROR:7A20", "ERROR:7A20	", "ѹ���������쳣"),
            new WarnType("ERROR:7AA5", "ERROR:7AA5	", "ѹ���������쳣"),
            new WarnType("ERROR:7AA7", "ERROR:7AA7	", "ѹ���������쳣"),
            new WarnType("ERROR:CUP_01", "Cupblocksthetray.	", "��Ʒû��ȡ��"),
            new WarnType("HOPPER_2", "HOPPER_2", "�˱�������"),
            new WarnType("INT_1", "	Cannotconnecttoserver.	", "������������"),
            new WarnType("warning:01", "sugarisnotenough!	", "ȱ��Ԥ��������������"),
            new WarnType("warning:02", "ESwaterisnotenough!	", "ȱˮԤ��������������"),
            new WarnType("warning:03", "Cupisnotenough!	", "ȱ��Ԥ��������������"),
            new WarnType("WARNING:0A", "Dooropened.	", "���Ŵ�	")
        };
    }
    /// <summary>
    /// ��������
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
        /// ����
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// Ӣ��
        /// </summary>
        public string English { get; set; }
        /// <summary>
        /// ����
        /// </summary>
        public string Chinese { get; set; }
    }

}