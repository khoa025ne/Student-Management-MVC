namespace StudentManagementMVC.ViewModels;

/// <summary>
/// ViewModel cho trang Error - thuộc Presentation layer
/// </summary>
public class ErrorViewModel
{
    public string? RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}
