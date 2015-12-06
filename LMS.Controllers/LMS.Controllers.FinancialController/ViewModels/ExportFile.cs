namespace LMS.Controllers.FinancialController.ViewModels
{
    using System;

    /// <summary>
    /// 导出文件
    /// </summary>
    [Serializable]
    public class ExportFile
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// 成功的文件路径,多个用英文逗号隔开
        /// </summary>
        public string[] FilePaths { get; set; }
    }
}