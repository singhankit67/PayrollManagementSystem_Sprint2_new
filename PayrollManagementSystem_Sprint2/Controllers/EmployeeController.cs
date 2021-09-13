﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PayrollManagementSystem_Sprint2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;

namespace PayrollManagementSystem_Sprint2.Controllers
{
    public class EmployeeController : Controller
    {

        static string localHostLink = "https://localhost:44314/";   //Common localhost link variable
        public IActionResult Index()
        {
            return View();
        }                 //Index Page
        public void ViewBagCheck(bool variable)
        {
            if (variable)
            {
                ViewBag.Message = "Operation Successfull";
                ViewBag.Color = "green";
            }
            else
            {
                ViewBag.Message = "Operation unsuccessful";
                ViewBag.Color = "red";
            }
        }

        #region Employee

        
        [HttpGet]
        public async Task<IActionResult> EmployeeDetailsView()     //View self details
        {
            var emp = new EmployeeMaster();
            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri($"{localHostLink}");
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var res= HttpContext.Session.GetString("EmployeeID");
            var response = await httpClient.GetAsync($"api/EmployeeMasters/{res}");
            if (response.IsSuccessStatusCode)
            {
                string apiResponse = await response.Content.ReadAsStringAsync();
                emp = JsonConvert.DeserializeObject<EmployeeMaster>(apiResponse);
                return View(emp);
            }
            else
            {
                return Unauthorized();
            }
           
        }

        public async Task<IActionResult> SelfAddressDetails()     //View self details
        {
            var emp = new EmpAddress();
            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri($"{localHostLink}");
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var res = HttpContext.Session.GetString("EmployeeID");
            
            var response = await httpClient.GetAsync($"api/EmpAddresses/{res}");
            if (response.IsSuccessStatusCode)
            {
                string apiResponse = await response.Content.ReadAsStringAsync();
                emp = JsonConvert.DeserializeObject<EmpAddress>(apiResponse);
            }            
            return View(emp);
        }
                
        public async Task<IActionResult> EditAddress()            //Edit Employee Address
        {
            EmpAddress emplist = new EmpAddress();
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri($"{localHostLink}");
                httpClient.DefaultRequestHeaders.Clear();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var res = HttpContext.Session.GetString("EmployeeID");                
                using (var response = await httpClient.GetAsync($"api/EmpAddresses/{res}"))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        emplist = JsonConvert.DeserializeObject<EmpAddress>(apiResponse);
                        return View(emplist);
                    }
                    else
                    {
                        return Unauthorized();
                    }
                }
            }
           
        }

        [HttpPost]
        public async Task<IActionResult> EditAddress(EmpAddress emp)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri($"{localHostLink}");
                httpClient.DefaultRequestHeaders.Clear();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                string stringData = JsonConvert.SerializeObject(emp);
                var contentData = new StringContent(stringData, Encoding.UTF8, "application/json");
                var res = HttpContext.Session.GetString("EmployeeID");                
                HttpResponseMessage response = httpClient.PutAsync($"api/EmpAddresses/" + res, contentData).Result;
                ViewBag.Message = response.Content.ReadAsStringAsync().Result;
                ViewBagCheck(response.IsSuccessStatusCode);
            }
            return View(emp);
        }
        #endregion

        #region Leaves

        [HttpGet]
        public async Task<IActionResult> ViewLeaveDetails()
        {
            List<LeaveDetail> leaveDetail = new List<LeaveDetail>();
            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri($"{localHostLink}");
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var res = HttpContext.Session.GetString("EmployeeID");
            ViewBag.LeaveWatch = res;
            var response = await httpClient.GetAsync($"api/LeaveDetails");
            if (response.IsSuccessStatusCode)
            {
                string apiResponse = await response.Content.ReadAsStringAsync();
                leaveDetail = JsonConvert.DeserializeObject<List<LeaveDetail>>(apiResponse);
                return View(leaveDetail);
            }
            else
            {
                return Unauthorized();
            }
        }

        public async Task<IActionResult> ViewLeaveMasterEmployee()
        {
            var leaveMaster = new LeaveMaster();
            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri($"{localHostLink}");
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var res = HttpContext.Session.GetString("EmployeeID");
            var response = await httpClient.GetAsync($"api/LeaveMasters/{res}");
            if (response.IsSuccessStatusCode)
            {
                string apiResponse = await response.Content.ReadAsStringAsync();
                leaveMaster = JsonConvert.DeserializeObject<LeaveMaster>(apiResponse);

                return View(leaveMaster);
            }

            else
            {
                return Unauthorized();
            }
        }

        public ActionResult ApplyForLeave()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> ApplyForLeave(LeaveDetail leave)
        {
            HttpClient httpClient = new HttpClient();

            var response = httpClient.PostAsJsonAsync<LeaveDetail>($"{localHostLink}api/LeaveDetails", leave);
            response.Wait();
            var result = response.Result;
            ViewBagCheck(result.IsSuccessStatusCode);
            return View();
        }

        #endregion

        #region Payroll 

        
        [HttpGet]
        public async Task<IActionResult> ViewPayrollMasterEmployee()
        {
            var payrollMaster = new PayrollMaster();
            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri($"{localHostLink}");
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var res = HttpContext.Session.GetString("EmployeeID");
            var response = await httpClient.GetAsync($"api/PayrollMasters/{res}");
            if (response.IsSuccessStatusCode)
            {
                string apiResponse = await response.Content.ReadAsStringAsync();
                payrollMaster = JsonConvert.DeserializeObject<PayrollMaster>(apiResponse);
                return View(payrollMaster);
            }
            else
            {
                return Unauthorized();
            }
            
            
        }
        #endregion

        #region Timesheet 
        public ActionResult CreateTimesheet()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> CreateTimesheet(TimeSheet employeeTimesheet)
        {
            HttpClient httpClient = new HttpClient();
            var response = httpClient.PostAsJsonAsync<TimeSheet>($"{localHostLink}api/TimeSheets", employeeTimesheet);
            response.Wait();
            var result = response.Result;
            ViewBagCheck(result.IsSuccessStatusCode);
            return View();
        }

        #endregion

    }
}



//Payroll details employee