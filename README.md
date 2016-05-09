## BeeCloud .Net SDK (Open Source)
[![Build Status](https://travis-ci.org/beecloud/beecloud-dotnet.svg?branch=dev)](https://travis-ci.org/beecloud/beecloud-dotnet) ![license](https://img.shields.io/badge/license-MIT-brightgreen.svg) ![version](https://img.shields.io/badge/version-v2.2.1-blue.svg)


## 简介

本项目的官方GitHub地址是 [https://github.com/beecloud/beecloud-dotnet](https://github.com/beecloud/beecloud-dotnet)

SDK支持以下支付渠道： 

* 支付宝web/wap
* 微信扫码/微信内JSAPI
* 银联web/wap
* 京东web/wap
* 易宝web/wap
* 百度web/wap
* paypal
* BeeCloud网关支付

提供（国内/国际）支付、（预）退款、 查询、 打款、 代付功能

## 流程

下图为整个支付的流程:
![pic](http://7xavqo.com1.z0.glb.clouddn.com/img-beecloud%20sdk.png)


步骤①：**（从网页服务器端）发送订单信息**
步骤②：**收到BeeCloud返回的渠道支付地址（比如支付宝的收银台）**
>*特别注意：
微信扫码返回的内容不是和支付宝一样的一个有二维码的页面，而是直接给出了微信的二维码的内容，需要用户自己将微信二维码输出成二维码的图片展示出来*

步骤③：**将支付地址展示给用户进行支付**
步骤④：**用户支付完成后通过一开始发送的订单信息中的return_url来返回商户页面**
>*特别注意：
微信没有自己的页面，二维码展示在商户自己的页面上，所以没有return url的概念，需要商户自行使用一些方法去完成这个支付完成后的跳转（比如后台轮询查支付结果）*

此时商户的网页需要做相应界面展示的更新（比如告诉用户"支付成功"或"支付失败")。**不允许**使用同步回调的结果来作为最终的支付结果，因为同步回调有极大的可能性出现丢失的情况（即用户支付完没有执行return url，直接关掉了网站等等），最终支付结果应以下面的异步回调为准。

步骤⑤：**（在商户后端服务端）处理异步回调结果（[Webhook](https://beecloud.cn/doc/?index=webhook)）**
 
付款完成之后，根据客户在BeeCloud后台的设置，BeeCloud会向客户服务端发送一个Webhook请求，里面包括了数字签名，订单号，订单金额等一系列信息。客户需要在服务端依据规则要验证**数字签名是否正确，购买的产品与订单金额是否匹配，这两个验证缺一不可**。验证结束后即可开始走支付完成后的逻辑。


## 安装
**方法一**.从BeeCloud [release](https://github.com/beecloud/beecloud-dotnet/releases)中下载dll文件,然后导入自己工程。  

>下载的BeeCloud.xml文件为dll注释文件，请一起放入项目文件夹中，方便查看注释。  
>.net SDK使用了第三方Json库LitJson.dll，请一起引入项目。 

**方法二**.按需修改本工程之后编译获得dll文件(包括BeeCloud.dll, BeeCloud.xml,LitJson.dll)，导入自己工程即可。

## 注册
四个步骤，2分钟轻松搞定：
1. 注册开发者：猛击[这里](http://www.beecloud.cn/register)注册成为BeeCloud开发者。
2. 注册应用：使用注册的账号登陆[控制台](http://www.beecloud.cn/dashboard/)后，点击"+添加应用"创建新应用
3. 在新创建的APP中获取 `APP ID` `APP Secret` `Master Secret` `Test Secret`
4. 在代码中注册：

```.net
//请注意各个参数一一对应
BeeCloud.BeeCloud.registerApp(appID, appSecret, masterSecret, testSecret);
```

注册方法务必在下面其他方法使用前被注册，一般写在程序启动时。

## 使用方法
>具体使用请参考项目中的`BeeCloudSDKDemo`工程

* live模式

```.net
//设置上线模式，真实货币交易（也可不设置，默认为上线模式）
BeeCloud.BeeCloud.setTestMode(false);
```
* test模式

```.net
//设置当前为测试模式
BeeCloud.BeeCloud.setTestMode(true);
```

### 1.支付

- 境内支付

方法原型：

```.net
public static BCBill BCPayByChannel(BCBill bill)
```
调用：

```.net
BCBill bill = new BCBill(BCPay.PayChannel.ALI_WEB.ToString(), 1, BCUtil.GetUUID(), "dotNet自来水");
bill.returnUrl = "http://localhost:50003/return_ali_url.aspx";
try
{
    BCBill resultBill = BCPay.BCPayByChannel(bill);
}
catch (Exception excption)
{
    //错误处理
}
```

- 境外支付

方法原型：

```.net
public static BCInternationlBill BCInternationalPay(BCInternationlBill bill)
```

调用：

```.net
//通过登录paypal账号付款
BCInternationlBill bill = new BCInternationlBill(BCPay.InternationalPay.PAYPAL_PAYPAL.ToString(), 1, BCUtil.GetUUID(), "dotnet paypal", "USD");
bill.returnUrl = "http://localhost:50003/paypal/return_paypal_url.aspx";
try
{
    bill = BCPay.BCInternationalPay(bill);
}
catch (Exception excption)
{
    //错误处理
}

//使用信用卡付款
BCInternationlBill bill = new BCInternationlBill(BCPay.InternationalPay.PAYPAL_CREDITCARD.ToString(), 1, BCUtil.GetUUID(), "dotnet paypal", "USD");
bill.info = info;
try
{
    bill = BCPay.BCInternationalPay(bill);
}
catch (Exception excption)
{
    //错误处理
}

//使用存储的信用卡信息付款
BCInternationlBill bill = new BCInternationlBill(BCPay.InternationalPay.PAYPAL_SAVED_CREDITCARD.ToString(), 1, BCUtil.GetUUID(), "dotnet paypal", "USD");
//这里填入你在信用卡付款后获得的信用卡id。
bill.creditCardId = "CARD-1K997489XXXXXXXXXXXXXXX";
try
{
    bill = BCPay.BCInternationalPay(bill);
}
catch (Exception excption)
{
    //错误处理
}
```

### 2.(预)退款

方法原型：

```.net
public static BCRefund BCRefundByChannel(BCRefund refund)
```
调用：

```.net
BCRefund refund = new BCRefund(billNo, DateTime.Today.ToString("yyyyMMdd") + BCUtil.GetUUID().Substring(0, 8), totalFee);
refund.channel = BCPay.RefundChannel.ALI.ToString();
try
{
    refund = BCPay.BCRefundByChannel(refund);
    //Response.Redirect(refund.url);支付宝需要到退款链接输入支付密码完成退款
}
catch (Exception excption)
{
    //错误处理
}
```

### 3.退款审核

* 查询支付订单

方法原型

```.net
public static BCApproveRefundResult BCApproveRefund(string channel, List<string> ids, bool agree, string denyReason)
```

调用：

```.net
BCApproveRefundResult result = new BCApproveRefundResult();
try
{
    result = BCPay.BCApproveRefund("WX", list, true, null);
}
catch (Exception excption)
{
    //错误处理
}
```

### 4.查询 

* 查询支付订单

方法原型：

```.net
//条件查询
public static List<BCBill> BCPayQueryByCondition(BCQueryBillParameter para)
//id查询
public static BCBill BCPayQueryById(string id)
//条件查询的结果数
public static int BCPayQueryCount(BCQueryBillParameter para)
```
调用：

```.net
try
{
    BCQueryBillParameter para = new BCQueryBillParameter();
    para.channel = "ALI";
    para.limit = 50;
    bills = BCPay.BCPayQueryByCondition(para);
}
catch (Exception excption)
{
    //错误处理
}
```
* 查询退款订单

方法原型：

```.net
//条件查询
public static int BCRefundQueryCount(BCQueryRefundParameter para)
//id查询
public static BCRefund BCRefundQueryById(string id)
//条件查询结果数
public static int BCRefundQueryCount(BCQueryRefundParameter para)
```
调用：

```.net
BCQueryRefundParameter para = new BCQueryRefundParameter();
para.channel = "ALI";
para.limit = 50;
try
{
    refunds = BCPay.BCRefundQueryByCondition(para);
}
catch (Exception excption)
{
    //错误处理
}
```
* 查询退款状态（支持微信，易宝，快钱，百度）

方法原型：

```.net
public static string BCRefundStatusQuery(string channel, string refundNo)
```
调用：

```.net
try
{
    //拿常用渠道 易宝 举例
    string status = BCPay.BCRefundStatusQuery("YEE", refundNo);
}
catch (Exception excption)
{
    //错误处理
}
```
### 5.（微信/支付宝）企业打款
* （支付宝）批量打款

方法原型:

```.net
public static string BCTransfers(BCTransfersParameter para)
```
调用：

```.net
BCTransferData data = new BCTransferData();
data.transferId = BCUtil.GetUUID();
data.receiverAccount = "xx@xx.com";
data.receiverName = "某某某";
data.transferFee = 100;
data.transferNote = "note";
BCTransferData data2 = new BCTransferData();
data2.transferId = BCUtil.GetUUID();
data2.receiverAccount = "xx@xx.com";
data2.receiverName = "某某";
data2.transferFee = 100;
data2.transferNote = "note";
List<BCTransferData> list = new List<BCTransferData>();
list.Add(data);
list.Add(data2);

try
{
    BCTransfersParameter para = new BCTransfersParameter();
    para.channel = BCPay.TransferChannel.ALI.ToString();
    para.batchNo = BCUtil.GetUUID();
    para.accountName = "毛毛";
    para.transfersData = list;
    string transfersURL = BCPay.BCTransfers(para);
}
catch (Exception excption)
{
     //错误处理
}
```

* 单笔打款

方法原型:

```.net
public static string BCTransfer(BCTransferParameter para)
```

调用:

```.net
//支付宝单笔打款
try
{
    BCTransferParameter para = new BCTransferParameter();
    para.channel = BCPay.TransferChannel.ALI_TRANSFER.ToString();
    para.transferNo = BCUtil.GetUUID();
    para.totalFee = 100;
    para.desc = "C# 单笔打款";
    para.channelUserId = "XXX@163.com";
    para.channelUserName = "毛毛";
    para.accountName = "XXX有限公司";
    string aliURL = BCPay.BCTransfer(para);
}
catch (Exception excption)
{
    //错误处理
}

//微信单笔打款
try
{
    BCTransferParameter para = new BCTransferParameter();
    para.channel = BCPay.TransferChannel.WX_TRANSFER.ToString();
    para.transferNo = "1000000000";
    para.totalFee = 100;
    para.desc = "C# 单笔打款";
    para.channelUserId = "XXXXXXXXXXXXXXXXXX";
    BCPay.BCTransfer(para);
}
catch (Exception excption)
{
    //错误处理
}

//微信红包
BCRedPackInfo info = new BCRedPackInfo();
info.actName = "C# 红包";
info.sendName = "BeeCloud";
info.wishing = "啦啦啦";

try
{
    BCTransferParameter para = new BCTransferParameter();
    para.channel = BCPay.TransferChannel.WX_REDPACK.ToString();
    para.transferNo = "1000000001";
    para.totalFee = 100;
    para.desc = "C# 红包";
    para.channelUserId = "XXXXXXXXXXXXXXXX";
    para.info = info;
    BCPay.BCTransfer(para);
    Response.Write("完成");
}
catch (Exception excption)
{
    //错误处理
}
```

### 6. BeeCloud代付（打款到银行卡）

方法原型：

```C#
// Summary:
//     BC银行卡代付
//
// Parameters:
//   transfer:
//     具体参考初始化BCTransferWithBackCard
public static BCTransferWithBackCard BCBankCardTransfer(BCTransferWithBackCard transfer);
```

调用：

```C#
BCTransferWithBackCard transfer = new BCTransferWithBackCard(1, BCUtil.GetUUID(), ".net测试代付", "OUT_PC", "BOC", "xxxxxxx", "中国银行", "DE", "P", "xxxxxxxxxxxx", "xxx");
transfer.mobile = "xxxxxxxxxxxxxx";
try 
{
    transfer = BCPay.BCBankCardTransfer(transfer);
    //打款成功
}
catch (Exception excption)
{
    //错误处理
}
```

## Demo
项目中的`BeeCloudSDKDemo`工程为我们的demo  
**demo使用framework4.5**  
在demo工程中添加BeeCloud工程的dll引用，设置demo工程为启动项后F5即可运行调试
>每次修改过BeeCloud工程后请先build BeeCloud工程再运行demo调试

- 关于微信的return_url
微信没有return_url，如果用户需要支付完成做类似同步跳转的形式，需要自行完成。

- 关于支付宝的return_url
请参考demo中的`return_ali_url.aspx`

- 关于银联的return_url
请参考demo中的`return_un_url.aspx`

- 关于weekhook的接收
请参考demo中的`notify.asxp`
文档请阅读 [webhook](https://github.com/beecloud/beecloud-webhook)


## 常见问题

- `LitJson.dll`和`ThoughtWorks.QRCode.dll`哪里有下？  
可以在demo项目的bin文件夹下获得，推荐使用本项目提供的dll，有遇到过使用不同版本的dll导致出错的情况。  
  
