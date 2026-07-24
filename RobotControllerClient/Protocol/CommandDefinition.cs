using System.Collections.Generic;

namespace RobotControllerClient.Protocol
{
    public enum CommandKind
    {
        Action,
        Request,
        Set
    }

    public class CommandParameter
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string DefaultValue { get; set; }
        public string[] Choices { get; set; }

        public CommandParameter(string name, string description, string defaultValue = "", string[] choices = null)
        {
            Name = name;
            Description = description;
            DefaultValue = defaultValue;
            Choices = choices;
        }
    }

    public class CommandDefinition
    {
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public CommandKind Kind { get; set; }
        public string Description { get; set; }
        public string Template { get; set; }
        public List<CommandParameter> Parameters { get; set; }

        public bool HasParameters
        {
            get { return Parameters != null && Parameters.Count > 0; }
        }

        public CommandDefinition(string id, string displayName, CommandKind kind, string description, string template, List<CommandParameter> parameters = null)
        {
            Id = id;
            DisplayName = displayName;
            Kind = kind;
            Description = description;
            Template = template;
            Parameters = parameters ?? new List<CommandParameter>();
        }

        public string Build(IDictionary<string, string> values)
        {
            string result = Template;
            if (Parameters != null)
            {
                foreach (CommandParameter p in Parameters)
                {
                    string token = "{" + p.Name + "}";
                    string value = p.DefaultValue;
                    if (values != null && values.ContainsKey(p.Name))
                    {
                        value = values[p.Name];
                    }
                    result = result.Replace(token, value ?? string.Empty);
                }
            }
            return result;
        }
    }

    public static class CommandCatalog
    {
        private static readonly string[] ArmChoices = { "A", "B" };
        private static readonly string[] ArmAllChoices = { "A", "B", "ALL" };
        private static readonly string[] OnOffChoices = { "ON", "OFF" };
        private static readonly string[] RadialChoices = { "EX", "RE", "EX_G", "EX_P" };
        private static readonly string[] UpDnChoices = { "UP", "DN" };

        public static List<CommandDefinition> All { get; private set; }

        static CommandCatalog()
        {
            All = new List<CommandDefinition>();

            Add("HLLO", "HLLO (통신확인)", CommandKind.Action,
                "Robot이 통신에 반응하는지 확인하는 비개입 명령. 회신: Hello", "HLLO");

            Add("CLEAR", "CLEAR (알람해제)", CommandKind.Action,
                "모든 Error 및 Alarm을 Clear 한다.", "CLEAR");

            Add("HOME", "HOME (원점복귀)", CommandKind.Action,
                "Robot을 HOME 위치로 이동한다.", "HOME ALL");

            Add("GOTO", "GOTO (스테이션 이동)", CommandKind.Action,
                "암을 지정된 스테이션으로 이동시킨다. 모든 파라미터를 순서대로 전송해야 한다.",
                "GOTO N {station} R {radial} Z {z} SLOT {slot} ARM {arm}",
                new List<CommandParameter>
                {
                    new CommandParameter("station", "스테이션 번호 (1~16)", "1"),
                    new CommandParameter("radial", "Arm radial position", "RE", RadialChoices),
                    new CommandParameter("z", "Z축 상하 위치", "DN", UpDnChoices),
                    new CommandParameter("slot", "슬롯 번호", "1"),
                    new CommandParameter("arm", "이동할 암", "A", ArmChoices)
                });

            Add("PICK", "PICK (웨이퍼 픽업)", CommandKind.Action,
                "지정된 스테이션/슬롯에서 웨이퍼를 집어 올린다.",
                "PICK {station} SLOT {slot} ARM {arm}",
                new List<CommandParameter>
                {
                    new CommandParameter("station", "스테이션 번호 (1~16)", "1"),
                    new CommandParameter("slot", "슬롯 번호", "1"),
                    new CommandParameter("arm", "픽업 암", "A", ArmChoices)
                });

            Add("PLACE", "PLACE (웨이퍼 놓기)", CommandKind.Action,
                "지정된 스테이션/슬롯에 웨이퍼를 놓는다.",
                "PLACE {station} SLOT {slot} ARM {arm}",
                new List<CommandParameter>
                {
                    new CommandParameter("station", "스테이션 번호 (1~16)", "1"),
                    new CommandParameter("slot", "슬롯 번호", "1"),
                    new CommandParameter("arm", "배치 암", "A", ArmChoices)
                });

            Add("ZAXIS", "ZAXIS (Z축 UP/DN)", CommandKind.Action,
                "지정된 스테이션에서 UP/DOWN 동작. 현재/이전 스테이션이 일치해야 한다.",
                "ZAXIS {station} SLOT {slot} {updn} ARM {arm}",
                new List<CommandParameter>
                {
                    new CommandParameter("station", "스테이션 번호 (1~16)", "1"),
                    new CommandParameter("slot", "슬롯 번호 (1~8)", "1"),
                    new CommandParameter("updn", "상하 이동", "UP", UpDnChoices),
                    new CommandParameter("arm", "암", "A", ArmChoices)
                });

            Add("SERVO", "SERVO (서보 ON/OFF)", CommandKind.Action,
                "Robot의 모든 축 서보를 ON/OFF 한다.",
                "SERVO {state}",
                new List<CommandParameter>
                {
                    new CommandParameter("state", "서보 상태", "ON", OnOffChoices)
                });

            Add("GRIP", "GRIP (그립 전/후진)", CommandKind.Action,
                "Robot Hand의 Grip Holder를 전진/후진 한다.",
                "GRIP {state} ARM {arm}",
                new List<CommandParameter>
                {
                    new CommandParameter("state", "Grip 상태", "ON", OnOffChoices),
                    new CommandParameter("arm", "동작 암", "A", ArmAllChoices)
                });

            Add("CHECKWAFER", "CHECKWAFER (웨이퍼 확인)", CommandKind.Action,
                "Endeffector의 Wafer 유무를 확인한다.",
                "CHECKWAFER {arm}",
                new List<CommandParameter>
                {
                    new CommandParameter("arm", "확인 암", "A", ArmAllChoices)
                });

            Add("ESTOP", "ESTOP (긴급정지)", CommandKind.Action,
                "진행 중인 모든 운동을 즉시 정지하고 서보를 중단한다.", "ESTOP");

            Add("MOVEMAP", "MOVEMAP (매핑위치 이동)", CommandKind.Action,
                "지정된 Stage Mapping 위치로 이동한다.",
                "MOVEMAP {station}",
                new List<CommandParameter>
                {
                    new CommandParameter("station", "이동할 Station", "1")
                });

            Add("MAPSCAN", "MAPSCAN (매핑 스캔)", CommandKind.Action,
                "지정된 Mapping 위치에서 Material Scan을 수행한다.",
                "MAPSCAN {station}",
                new List<CommandParameter>
                {
                    new CommandParameter("station", "스캔할 Station", "1")
                });

            Add("RQ_OPMODE", "RQ OPMODE (제어권)", CommandKind.Request,
                "Operation mode를 반환한다 (CDM/HOST).", "RQ OPMODE");

            Add("RQ_VERSION", "RQ VERSION (버전)", CommandKind.Request,
                "소프트웨어 버전을 반환한다.", "RQ VERSION");

            Add("RQ_WAFER", "RQ WAFER (웨이퍼 상태)", CommandKind.Request,
                "Wafer Load 상태를 반환한다.",
                "RQ WAFER ARM {arm}",
                new List<CommandParameter>
                {
                    new CommandParameter("arm", "확인 암", "ALL", ArmAllChoices)
                });

            Add("RQ_HISPD", "RQ HISPD (고속값)", CommandKind.Request,
                "Wafer Unload(고속) 속도값을 반환한다.", "RQ HISPD ALL");

            Add("RQ_LOSPD", "RQ LOSPD (저속값)", CommandKind.Request,
                "Wafer Load(저속) 속도값을 반환한다.", "RQ LOSPD ALL");

            Add("RQ_GRIPCHECK", "RQ GRIPCHECK (그립상태)", CommandKind.Request,
                "Endeffector의 Grip 상태를 반환한다.",
                "RQ GRIPCHECK ARM {arm}",
                new List<CommandParameter>
                {
                    new CommandParameter("arm", "확인 암", "ALL", ArmAllChoices)
                });

            Add("RQ_ERR", "RQ ERR (최근에러)", CommandKind.Request,
                "가장 최근의 Error Code를 반환한다.", "RQ ERR");

            Add("RQ_POS", "RQ POS (현재위치)", CommandKind.Request,
                "지정 축의 현재 위치를 반환한다.",
                "RQ POS {axis}",
                new List<CommandParameter>
                {
                    new CommandParameter("axis", "축", "ALL",
                        new[] { "T1", "T2", "Z1", "Z2", "A", "B", "R", "ALL" })
                });

            Add("RQ_ABS", "RQ ABS (좌표)", CommandKind.Request,
                "Robot의 현재 좌표 데이터를 반환한다.", "RQ ABS");

            Add("RQ_SERVO", "RQ SERVO (서보상태)", CommandKind.Request,
                "SERVO ON/OFF 상태를 반환한다.", "RQ SERVO");

            Add("RQ_FDC", "RQ FDC (헬스데이터)", CommandKind.Request,
                "마지막 모션의 Robot Health Data를 반환한다.", "RQ FDC");

            Add("SET_HISPD", "SET HISPD (고속설정)", CommandKind.Set,
                "Wafer Unload(고속) 속도를 설정한다 (1~100%).",
                "SET HISPD ALL {speed}",
                new List<CommandParameter>
                {
                    new CommandParameter("speed", "속도 % (1~100)", "50")
                });

            Add("SET_LOSPD", "SET LOSPD (저속설정)", CommandKind.Set,
                "Wafer Load(저속) 속도를 설정한다 (1~100%).",
                "SET LOSPD ALL {speed}",
                new List<CommandParameter>
                {
                    new CommandParameter("speed", "속도 % (1~100)", "50")
                });
        }

        private static void Add(string id, string display, CommandKind kind, string desc, string template, List<CommandParameter> parameters = null)
        {
            All.Add(new CommandDefinition(id, display, kind, desc, template, parameters));
        }

        public static CommandDefinition FindById(string id)
        {
            foreach (CommandDefinition c in All)
            {
                if (c.Id == id)
                {
                    return c;
                }
            }
            return null;
        }
    }
}
