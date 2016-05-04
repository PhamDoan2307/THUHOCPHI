using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThuHocPhi.Models;
using ThuHocPhi.Models.Tudien;
using ThuHocPhi.Shares;

namespace ThuHocPhi.Controls.Tudien
{
    class monhoc_ctrl
    {
        DataDataContext db = new DataDataContext();
        public ActionResult<List<monhoc_ett>> GetAllMH()
        {
            ActionResult<List<monhoc_ett>> op = new ActionResult<List<monhoc_ett>>();
            op.Data = new List<monhoc_ett>();
            try
            {
                var qr = db.MonHocs;

                if (qr.Count() > 0)
                {
                    //có dữ liệu
                    foreach (var i in qr)
                    {
                        monhoc_ett k = new monhoc_ett(i);
                        op.Data.Add(k);
                    }
                    op.ErrCode = CEnum.Success;
                    op.ErrDesc = String.Format(Shares.Constants.act_rs_get_Data_Success, "Môn học");
                    return op;
                }
                else
                {
                    //Không có dữ liệu;
                    op.ErrCode = CEnum.HaveNoData;
                    op.ErrDesc = String.Format(Shares.Constants.act_rs_get_Data_NULL, "Môn học");
                    op.Data = null;
                    return op;
                }
            }
            catch (Exception ex)
            {
                Utils.ShowExceptionMsg(ex);
                //Không có dữ liệu;
                op.ErrCode = CEnum.Fail;
                op.ErrDesc = String.Format(Shares.Constants.act_rs_get_Data_Fail, "Môn học");
                op.Data = null;
                return op;
            }

        }
        public ActionResult<monhoc_ett> InserMH(string mamnh, string tenmh, string sotchp)
        {
            ActionResult<monhoc_ett> op = new ActionResult<monhoc_ett>();
            try
            {
                MonHoc mh = new MonHoc();
                mh.MaMH = mamnh;
                mh.TenMH = tenmh;
                mh.SoTCHP = sotchp;
                db.MonHocs.InsertOnSubmit(mh);
                db.SubmitChanges();

                op.ErrCode = CEnum.Success;
                op.ErrDesc = String.Format(Shares.Constants.act_rs_insert_Data_Success, "Môn học");
                op.Data = new monhoc_ett(mh);
                return op;
            }
            catch (Exception ex)
            {
                Utils.ShowExceptionMsg(ex);
                op.ErrCode = CEnum.Fail;
                op.ErrDesc = String.Format(Shares.Constants.act_rs_insert_Data_Fail, "Môn học");
                op.Data = null;
                return op;
            }
        }
        public ActionResult<monhoc_ett> UpdateMH(string mamh, string tenmh, string sotchp)
        {
            ActionResult<monhoc_ett> op = new ActionResult<monhoc_ett>();
            try
            {
                var qr = db.MonHocs.Where(o => o.MaMH == mamh);
                if (qr.Count() > 0)
                {
                    MonHoc mh = qr.SingleOrDefault();
                    mh.MaMH = mamh;
                    mh.TenMH = tenmh;
                    mh.SoTCHP = sotchp;
                    db.SubmitChanges();
                    op.ErrCode = CEnum.Success;
                    op.ErrDesc = String.Format(Shares.Constants.act_rs_update_Data_Success, "Môn học");
                    op.Data = new monhoc_ett(mh);
                    return op;
                }
                else
                {
                    op.ErrCode = CEnum.HaveNoData;
                    op.ErrDesc = String.Format(Shares.Constants.act_rs_get_Data_NULL, "Môn học");
                    op.Data = null;
                    return op;
                }
            }
            catch (Exception ex)
            {
                Utils.ShowExceptionMsg(ex);
                op.ErrCode = CEnum.Fail;
                op.ErrDesc = String.Format(Shares.Constants.act_rs_update_Data_Fail, "Môn học");
                op.Data = null;
                return op;
            }
        }
        // public ActionResult<sinhvien_ett> SearchSV(string masv)
    }
}
