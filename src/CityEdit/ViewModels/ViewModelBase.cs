using CommunityToolkit.Mvvm.ComponentModel;

namespace CityEdit.ViewModels;

/// <summary>
/// Базовый класс для всех ViewModel проекта.
/// Наследует ObservableObject из CommunityToolkit.Mvvm для поддержки уведомлений об изменениях.
/// </summary>
public abstract class ViewModelBase : ObservableObject
{
}
