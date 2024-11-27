using System.ComponentModel;

namespace Communication
{
    public static class EnumExtensions
    {
        public static string GetDescription<T>(this T value)
        {
            string result = value!.ToString()!;

            System.Reflection.FieldInfo fieldInfo = value.GetType().GetField(value.ToString()!)!;

            string attributeType = fieldInfo.CustomAttributes.FirstOrDefault(r => r.AttributeType.Name.Equals(Constants.DescriptionAttribute))?.AttributeType.Name!;

            if (!string.IsNullOrEmpty(attributeType) && attributeType == Constants.DescriptionAttribute)
            {
                var attributes = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];

                if (attributes != null)
                {
                    result = attributes.Length > 0 ? attributes[0].Description : value.ToString()!;
                }
            }

            return result;
        }
    }
}
