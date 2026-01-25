using System.ComponentModel.DataAnnotations;

namespace DataAccess.Enums
{
    /// <summary>
    /// Enum cho các ngành học trong hệ thống
    /// </summary>
    public enum MajorType
    {
        [Display(Name = "Chưa xác định")]
        Undefined = 0,

        [Display(Name = "Kỹ thuật phần mềm")]
        SoftwareEngineering = 1,

        [Display(Name = "Trí tuệ nhân tạo")]
        ArtificialIntelligence = 2,

        [Display(Name = "Khoa học máy tính")]
        ComputerScience = 3,

        [Display(Name = "Hệ thống thông tin")]
        InformationSystems = 4,

        [Display(Name = "An toàn thông tin")]
        InformationSecurity = 5,

        [Display(Name = "Thiết kế đồ họa")]
        GraphicDesign = 6,

        [Display(Name = "Thiết kế nội thất")]
        InteriorDesign = 7,

        [Display(Name = "Kinh doanh quốc tế")]
        InternationalBusiness = 8,

        [Display(Name = "Quản trị kinh doanh")]
        BusinessAdministration = 9,

        [Display(Name = "Kế toán")]
        Accounting = 10,

        [Display(Name = "Tài chính ngân hàng")]
        Finance = 11,

        [Display(Name = "Marketing")]
        Marketing = 12,

        [Display(Name = "Tiếng Anh")]
        English = 13,

        [Display(Name = "Tiếng Nhật")]
        Japanese = 14,

        [Display(Name = "Tiếng Hàn")]
        Korean = 15
    }
}
