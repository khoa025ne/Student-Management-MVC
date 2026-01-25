namespace DataAccess.Enums
{
    /// <summary>
    /// Trạng thái đăng ký môn học
    /// </summary>
    public enum EnrollmentStatus
    {
        /// <summary>
        /// Đang học
        /// </summary>
        Enrolled = 1,

        /// <summary>
        /// Đã hoàn thành
        /// </summary>
        Completed = 2,

        /// <summary>
        /// Đã hủy đăng ký
        /// </summary>
        Dropped = 3,

        /// <summary>
        /// Bị đình chỉ
        /// </summary>
        Suspended = 4,

        /// <summary>
        /// Không đạt
        /// </summary>
        Failed = 5
    }
}
