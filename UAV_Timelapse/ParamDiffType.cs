using System;

namespace UAV_Timelapse
{
    public enum ParamDiffType
    {
        Same,
        Different,
        OnlyInFc,
        OnlyInFile
    }

    public class ParamDiffItem
    {
        public string Name { get; set; }

        // Giá trị đang có trong FC (từ _allParams)
        public float? CurrentValue { get; set; }

        // Giá trị đọc được từ file .param
        public float? FileValue { get; set; }

        public ParamDiffType DiffType { get; set; }

        // Cho phép tick chọn để “áp dụng từ file” nếu sau này bạn muốn
        public bool ApplyFromFile { get; set; }
    }
}
