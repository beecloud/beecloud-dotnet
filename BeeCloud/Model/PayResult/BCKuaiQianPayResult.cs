﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BeeCloud.Model
{
    public class BCKuaiQianPayResult : BCPayResult
    {
        /// <summary>
        /// 快钱跳转form，是一段HTML代码，自动提交
        /// </summary>
        public string html { get; set; }
    }
}
