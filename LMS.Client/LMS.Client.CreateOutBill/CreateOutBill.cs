using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using LMS.Client.CreateOutBill.Controller;
using LMS.Client.CreateOutBill.Model;
using LighTake.Infrastructure.Common;
using LighTake.Infrastructure.Common.Logging;

namespace LMS.Client.CreateOutBill
{
    public class CreateOutBill
    {
        public static void CreateBill()
        {
            OutBillController.GetCreateOutBillList().ForEach(p =>
                {
                    try
                    {
                        var list = OutBillController.GetExportReceivingBill(p.ReceivingBillID);
                        if (p.Search != 100)
                        {
                            //扣费
                            if (OutBillController.CreateCustomerAmountRecords(p, list))
                            {
                                Log.Info("账单号为:{0},扣费成功！".FormatWith(p.ReceivingBillID));
                                AfterCreateCustomerAmountRecords(p, list);
                            }
                            else
                            {
                                Log.Error("账单号为:{0},扣费失败！".FormatWith(p.ReceivingBillID));
                            }
                        }
                        else
                        {
                            //现金直接或者是扣费成功后，其他操作异常
                            AfterCreateCustomerAmountRecords(p, list);
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Exception(ex);
                    }
                });
        }
        public static void AfterCreateCustomerAmountRecords(ReceivingBill bill, List<BillModel> list)
        {
            ExeclCreateController.ExeclCreate(bill, list); //生成Execl
            if (OutBillController.UpdateReceivingBillStatus(bill.ReceivingBillID))
            {
                Log.Info("账单号为:{0},修改出账单状态成功！".FormatWith(bill.ReceivingBillID));
                Log.Info(OutBillController.InsertBackUp(bill, list)
                             ? "账单号为:{0},插入备份数据库成功！".FormatWith(bill.ReceivingBillID)
                             : "账单号为:{0},插入备份数据库失败！".FormatWith(bill.ReceivingBillID));
            }
            else
            {
                Log.Error("账单号为:{0},修改出账单状态失败！".FormatWith(bill.ReceivingBillID));
            }
        }
    }
}
