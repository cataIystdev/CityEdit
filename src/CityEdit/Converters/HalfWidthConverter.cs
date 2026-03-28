using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace CityEdit.Converters;

/// <summary>
/// Конвертер, который возвращает половину переданной ширины минус отступы.
/// Используется для адаптивной раскладки скинов (2 в ряд, по 50% ширины).
/// </summary>
public class HalfWidthConverter : IValueConverter
{
    /// <summary>
    /// Отступ, вычитаемый из половины ширины (margin между элементами).
    /// </summary>
    public double Margin { get; set; } = 6;

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is double width && width > 0)
        {
            var half = (width / 2) - Margin;
            return Math.Max(half, 50);
        }
        return 150.0;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}

/// <summary>
/// Конвертер ширины, учитывающий IsLastInOddRow.
/// Если IsLastInOddRow=true → полная ширина. Иначе → половина.
/// Принимает [parentWidth, isLastInOddRow].
/// </summary>
public class SkinWidthConverter : IMultiValueConverter
{
    public double Margin { get; set; } = 6;

    public object Convert(System.Collections.Generic.IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        double parentWidth = 150;
        bool isLast = false;

        if (values.Count >= 1 && values[0] is double w && w > 0)
            parentWidth = w;
        if (values.Count >= 2 && values[1] is bool b)
            isLast = b;

        if (isLast)
            return parentWidth - Margin;

        return Math.Max((parentWidth / 2) - Margin, 50);
    }
}
