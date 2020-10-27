namespace YeGods.Common.Extensions
{
  using System;
  using System.Text;
  using System.Globalization;

  public static class StringManipulation
  {
    public static string NormalizeStringForUrl(this string name)
    {
      string lowerCaseName = name.Trim().ToLowerInvariant();
      String normalizedString = lowerCaseName.Normalize(NormalizationForm.FormD);
      StringBuilder stringBuilder = new StringBuilder();

      foreach (char c in normalizedString)
      {
        switch (CharUnicodeInfo.GetUnicodeCategory(c))
        {
          case UnicodeCategory.LowercaseLetter:
          case UnicodeCategory.UppercaseLetter:
          case UnicodeCategory.DecimalDigitNumber:
            stringBuilder.Append(c);
            break;
          case UnicodeCategory.SpaceSeparator:
          case UnicodeCategory.ConnectorPunctuation:
          case UnicodeCategory.DashPunctuation:
            stringBuilder.Append('_');
            break;
        }
      }
      string result = stringBuilder.ToString();
      return String.Join("-", result.Split(new char[] { '_' }
        , StringSplitOptions.RemoveEmptyEntries));
    }

    public static string Truncate(this string value, int maxLength)
    {
      if (string.IsNullOrEmpty(value)) return value;
      return value.Length <= maxLength ? value : value.Substring(0, maxLength) + " …";
    }
  }
}
