using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace UAV_Timelapse
{
    public class ParamMeta
    {
        public string Name { get; set; }          // ACRO_RP_RATE
        public string DisplayName { get; set; }   // "Acro Roll and Pitch Rate"
        public string Description { get; set; }   // mô tả dài
        public string Units { get; set; }         // "deg/s", "cm", ...
        public string Values { get; set; }        // chuỗi options: "0:Disabled,1:Enabled,..."
        public float? Default { get; set; }       // default nếu pdef có

        public float? Min { get; set; }           // từ field Range
        public float? Max { get; set; }
        public float? Increment { get; set; }     // từ field Increment
        public string RangeText { get; set; }
    }

    public static class ParamMetaStore
    {
        private static readonly Dictionary<string, ParamMeta> _byName =
            new Dictionary<string, ParamMeta>(StringComparer.OrdinalIgnoreCase);

        public static void Clear() => _byName.Clear();

        // Load đúng 1 file pdef (ArduCopter.apm.pdef.xml, ArduPlane..., v.v.)
        public static void LoadFromXml(string xmlPath)
        {
            _byName.Clear();
            if (string.IsNullOrEmpty(xmlPath) || !File.Exists(xmlPath))
                return;

            var doc = XDocument.Load(xmlPath);

            foreach (var p in doc.Descendants("param"))
            {
                // ===== Name: cắt "ArduCopter:" đi =====
                string rawName = (string)p.Attribute("name") ?? "";
                string shortName = rawName;
                int colon = rawName.IndexOf(':');
                if (colon >= 0 && colon < rawName.Length - 1)
                    shortName = rawName.Substring(colon + 1);

                // ===== Units =====
                string unitsAttr = (string)p.Attribute("units");
                string unitsField = p.Elements("field")
                                     .FirstOrDefault(f => (string)f.Attribute("name") == "Units")
                                     ?.Value;
                string units = unitsAttr ?? unitsField ?? "";

                // ===== Options / Values =====
                string options = ExtractOptions(p);

                // ===== Default =====
                float? def = null;
                var defField = p.Elements("field")
                                .FirstOrDefault(f => (string)f.Attribute("name") == "Default");
                if (defField != null &&
                    float.TryParse(defField.Value, NumberStyles.Any,
                                   CultureInfo.InvariantCulture, out float defVal))
                {
                    def = defVal;
                }

                // ===== Range (Min / Max) =====
                float? min = null, max = null;
                var rangeField = p.Elements("field")
                                  .FirstOrDefault(f => (string)f.Attribute("name") == "Range")
                                  ?.Value;
                string rangeText = rangeField?.Trim();
                if (!string.IsNullOrWhiteSpace(rangeField))
                {
                    var tokens = rangeField.Split(new[] { ' ', '\t' },
                                                  StringSplitOptions.RemoveEmptyEntries);
                    if (tokens.Length >= 2)
                    {
                        if (float.TryParse(tokens[0], NumberStyles.Any,
                                           CultureInfo.InvariantCulture, out float minVal))
                            min = minVal;
                        if (float.TryParse(tokens[1], NumberStyles.Any,
                                           CultureInfo.InvariantCulture, out float maxVal))
                            max = maxVal;
                    }
                }

                // ===== Increment =====
                float? inc = null;
                var incField = p.Elements("field")
                                .FirstOrDefault(f => (string)f.Attribute("name") == "Increment")
                                ?.Value;
                if (!string.IsNullOrWhiteSpace(incField) &&
                    float.TryParse(incField, NumberStyles.Any,
                                   CultureInfo.InvariantCulture, out float incVal))
                {
                    inc = incVal;
                }

                var meta = new ParamMeta
                {
                    Name = shortName,
                    DisplayName = (string)p.Attribute("humanName") ?? shortName,
                    Description = (string)p.Attribute("documentation") ?? "",
                    Units = units,
                    Values = options,
                    Default = def,
                    Min = min,      // mới
                    Max = max,
                    Increment = inc,
                    RangeText = rangeText ?? ""
                };

                _byName[shortName] = meta;
            }

        }

        // Đọc mọi kiểu options trong pdef
        private static string ExtractOptions(XElement p)
        {
            string options = "";

            // 1) pdef mới: <values><value code="x">Text</value>...</values>
            var valuesNode = p.Element("values");
            if (valuesNode != null)
            {
                var pairs = valuesNode.Elements("value")
                    .Select(v =>
                    {
                        string code = ((string)v.Attribute("code") ?? "").Trim();
                        string text = (v.Value ?? "").Trim();
                        return string.IsNullOrEmpty(code) && string.IsNullOrEmpty(text)
                            ? null
                            : $"{code}:{text}";
                    })
                    .Where(s => !string.IsNullOrEmpty(s));

                options = string.Join(",", pairs);
            }

            // 2) Kiểu cũ: <field name="Values">0:Disabled,1:Enabled,...</field>
            if (string.IsNullOrWhiteSpace(options))
            {
                options = p.Elements("field")
                           .FirstOrDefault(f => (string)f.Attribute("name") == "Values")
                           ?.Value;
            }

            // 3) Bitmask: ưu tiên field "Bitmask" nếu có
            if (string.IsNullOrWhiteSpace(options))
            {
                options = p.Elements("field")
                           .FirstOrDefault(f => (string)f.Attribute("name") == "Bitmask")
                           ?.Value;
            }

            // 4) Nếu chưa có, tự build từ <bitmask><bit code="..">Text</bit>...</bitmask>
            if (string.IsNullOrWhiteSpace(options))
            {
                var bitmaskNode = p.Element("bitmask");
                if (bitmaskNode != null)
                {
                    var pairs = bitmaskNode.Elements("bit")
                        .Select(b =>
                        {
                            string code = ((string)b.Attribute("code") ?? "").Trim();
                            string text = (b.Value ?? "").Trim();
                            return string.IsNullOrEmpty(code) && string.IsNullOrEmpty(text)
                                ? null
                                : $"{code}:{text}";
                        })
                        .Where(s => !string.IsNullOrEmpty(s));

                    options = string.Join(",", pairs);
                }
            }

            // Dọn khoảng trắng, xuống dòng
            if (!string.IsNullOrEmpty(options))
            {
                options = options.Replace("\r", " ")
                                 .Replace("\n", " ")
                                 .Replace("\t", " ");
                while (options.Contains("  "))
                    options = options.Replace("  ", " ");
                options = options.Trim();
            }

            return options ?? "";
        }

        public static ParamMeta Get(string name)
        {
            if (string.IsNullOrEmpty(name)) return null;
            _byName.TryGetValue(name, out var m);
            return m;
        }
    }
}
