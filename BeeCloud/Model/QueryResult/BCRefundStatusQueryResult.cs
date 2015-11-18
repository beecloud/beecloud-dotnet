﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BeeCloud.Model
{
    public class BCRefundStatusQueryResult
    {
        /// <summary>
        /// 返回码，0为正常
        /// </summary>
        public int resultCode { get; set; }
        /// <summary>
        /// 返回信息， OK为正常
        /// </summary>
        public string resultMsg { get; set; }
        /// <summary>
        /// 具体错误信息
        /// </summary>
        public string errDetail { get; set; }
        /// <summary>
        /// 退款状态
        /// </summary>
        public string refundStatus { get; set; }
    }
}
