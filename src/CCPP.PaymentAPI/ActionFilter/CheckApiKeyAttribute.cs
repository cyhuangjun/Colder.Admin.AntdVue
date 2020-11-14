using Castle.Core.Internal;
using Coldairarrow.Api;
using Coldairarrow.Business.Base_Manage;
using Coldairarrow.Entity;
using Coldairarrow.Util;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using NetTopologySuite.Operation.Overlay;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CCPP.PaymentAPI.ActionFilter
{
    public class CheckApiKeyAttribute : BaseActionFilterAsync
    {
        public override async Task OnActionExecuting(ActionExecutingContext context)
        {
            try
            {
                string apiKey = context.HttpContext.Request.Headers[GlobalData.HTTPHEADAPIKEY];
                if (apiKey.IsNullOrEmpty())
                {
                    context.Result = Error("api key not found!", ErrorCodeDefine.ApiKeyNotFound);
                    return;
                }
                string timeStampString = context.HttpContext.Request.Headers[GlobalData.HTTPHEADTIMESTAMP]; 
                if (timeStampString.IsNullOrEmpty())
                {
                    context.Result = Error("timeStamp not found!", ErrorCodeDefine.TimeStampNotFound);
                    return;
                }
                int timestamp;
                if(!int.TryParse(timeStampString, out timestamp))
                {
                    context.Result = Error("http head timestamp error!", ErrorCodeDefine.TimeStampError);
                    return;
                }
                //var maxTimeStamp = DateTime.Now.ToUnixTimeStamp();
                var minTimeStamp = DateTime.Now.AddMinutes(-1).ToUnixTimeStamp();
                if (timestamp < minTimeStamp)
                {
                    context.Result = Error("http head timestamp error!", ErrorCodeDefine.TimeStampError);
                    return;
                }
                string signature = context.HttpContext.Request.Headers[GlobalData.HTTPHEADSIGNATURE];
                if (signature.IsNullOrEmpty())
                {
                    context.Result = Error("http head signature not found!", ErrorCodeDefine.SignatureNotFound);
                    return;
                } 
                IServiceProvider serviceProvider = context.HttpContext.RequestServices;
                var base_UserBusiness = serviceProvider.GetService<IBase_UserBusiness>();
                var user = await base_UserBusiness.GetUserByApiKeyAsync(apiKey);
                if (user == null)
                {
                    context.Result = Error("illegal operation!", ErrorCodeDefine.IllegalOperation);
                    return;
                }
                else if (user.Deleted || (user.IsFrozen ?? false))
                {
                    context.Result = Error("user frozen!", ErrorCodeDefine.UserIsFrozen);
                    return;
                }
                var securityKey = user.SecretKey;
                var signParameters = new Dictionary<string, object>();
                signParameters.Add("timestamp", timestamp);
                foreach (var argument in context.ActionArguments)
                {
                    var dict = argument.Value.ToDictionary();
                    foreach(var kvp in dict)
                    {
                        signParameters.Add(kvp.Key, kvp.Value);
                    }
                }
                var mac = signParameters.Sign(securityKey);
                if(!mac.Equals(signature))
                {
                    context.Result = Error("signatures not match!", ErrorCodeDefine.SignaturesNotMatch);
                    return;
                }
            }
            catch (Exception ex)
            {
                context.Result = Error(ex.Message, ErrorCodeDefine.ServerError);
            } 
            await Task.CompletedTask;
        } 
    }
}