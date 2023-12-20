using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Dynamic;
using TestManagement.DataAccess.BaseRepository.IRepository;
using TestManagement.DataAccess.Data;
using TestManagement.Models.TestMangementVMs;
using TestManagement.Models;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using System.Xml.Linq;
using Newtonsoft.Json;
using System.Text.Json.Nodes;

namespace TestManagement.Controllers
{
    public class TestingViewsController : Controller
    {
        private readonly ILogger<TestingViewsController> _logger;
        private readonly ApplicationDbContext _db;
        dynamic tsdVM = new ExpandoObject();
        private readonly IConfiguration? _configuration;
        private readonly IUnitOfWork _unitOfWork;
        bool nextIdCalled = false;

        public TestingViewsController(ILogger<TestingViewsController> logger, ApplicationDbContext db, IConfiguration configuration, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _db = db;
            _configuration = configuration;
            _unitOfWork = unitOfWork;
        }



        //Routing functions
        public IActionResult Index(long? projectId, long? applicationId)
        {
            if (projectId != null && applicationId != null)
            {

                CommonVM commonVM = getCommonVM((long)projectId, (long)applicationId).Result;
                return View(commonVM);
            }
            return View();
        }
        public IActionResult PageDefinationView(long projectId, long applicationId)
        {
            CommonVM commonVM = getCommonVM(projectId, applicationId).Result;
            return View(commonVM);
        }
        public IActionResult TestStepTemplateView(long projectId, long applicationId)
        {
            CommonVM commonVM = getCommonVM(projectId, applicationId).Result;
            return View(commonVM);
        }


        public IActionResult TestCaseView(long projectId, long applicationId)
        {
            CommonVM commonVM = getCommonVM(projectId, applicationId).Result;
            return View(commonVM);
        }
        public IActionResult ExecutionCycleView(long projectId, long applicationId)
        {
            CommonVM commonVM = getCommonVM(projectId, applicationId).Result;
            return View(commonVM);
        }
        public IActionResult ElementTypeView(long projectId, long applicationId)
        {
            CommonVM commonVM = getCommonVM(projectId, applicationId).Result;
            return View(commonVM);
        }
        public IActionResult TestPlanView(long projectId, long applicationId)
        {
            CommonVM commonVM = getCommonVM(projectId, applicationId).Result;
            return View(commonVM);
        }
        //get Page details for specific application
        [HttpGet]
        public async Task<IActionResult> GetPageDetailsForApplication(long applicationId)
        {
            try
            {
                if (applicationId != 0 && applicationId != null)
                {
                    var pageDataForApplication = _unitOfWork.Pages.GetAll(x => x.ApplicationId == applicationId && x.Status == "ACTV").ToList().Select(x => new { PageId = x.PageId, PageName = x.PageName });

                    return Json(new { statusCode = 1, data = pageDataForApplication });
                }
                else
                {
                    return Json(new { statusCode = 0, data = "Error while processing request!" });
                }
            }
            catch (Exception e)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                _logger.LogError(e, $"Path: {controllerName + "/" + actionName}\n" + e.Message);

                return StatusCode(500, e);
            }
        }

        //get page url for page in step template
        [HttpGet]
        public IActionResult GetPageUrlForPage(long pageId)
        {
            try
            {
                string pageUrl = _unitOfWork.Pages.Get(x => x.PageId == pageId).First().PageUrl;

                return Json(new { statusCode = 1, data = pageUrl });
            }
            catch (Exception e)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                _logger.LogError(e, $"Path: {controllerName + "/" + actionName}\n" + e.Message);

                return StatusCode(500, e);
            }
        }



        //get step template on search        

        //[HttpPost]
        //public IActionResult GetStepTemplate(SearchStepVM stepNameObj)
        //{
        //    if (stepNameObj != null)
        //    {



        //        //ADLArray = ADLArray.Where(a => a.Status == "ACTV").ToList();
        //        //if (ADLArray != null)
        //        //{
        //        //    Console.WriteLine("Got ADL records From  Cache:" + ADLArray.Count());
        //        //}

        //        //var searchResult = ADLArray.Where(s => s.AssetName.ToLower().Replace(" ", "").Contains(substring.ToLower().Replace(" ", "")) || (s.AssetCode != null ? s.AssetCode.ToLower().Replace(" ", "").EndsWith(substring.ToLower().Replace(" ", "")) : false)
        //        //                                        && s.Status == "ACTV");
        //        //return Json(new { options = searchResult });

        //        //string JSONresult = JsonConvert.SerializeObject(searchResult, Formatting.Indented);


        //        //Console.WriteLine(JSONresult.ToString());
        //    }
        //    return Ok();

        //}

        //Receiving data functions

        public IActionResult ProjectDetails()
        {
            try
            {
                var result = _unitOfWork.Projects.GetAll().OrderBy(x => x.CreatedDate).ToList();
                return Json(new { resultSet = result });
            }
            catch (Exception e)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                _logger.LogError(e, $"Path: {controllerName + "/" + actionName}\n" + e.Message);

                return StatusCode(500, e);

            }
        }
        [HttpPost]
        public IActionResult ApplicationDetails([FromBody] long projectId)
        {
            try
            {
                var result = _unitOfWork.Applications.GetAll(x => x.ProjectId == projectId).OrderBy(x => x.CreatedDate).ToList();
                return Json(new { resultSet = result });
            }
            catch (Exception e)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                _logger.LogError(e, $"Path: {controllerName + "/" + actionName}\n" + e.Message);

                return StatusCode(500, e);

            }
        }
        [HttpPost]
        public IActionResult GetPageDefinationsFromSearch([FromBody] PageSearchVM submitObj)
        {
            var pageList = _unitOfWork.Pages.GetAll(x => x.ApplicationId == submitObj.applicationId && x.Status == "ACTV").ToList();
            var pages = pageList.Where(
                               pa => (pa.PageName != null ? pa.PageName.ToLower().Replace(" ", "").Contains(submitObj.substring.ToLower().Replace(" ", "")) : false)
                            && pa.Status == "ACTV").ToList();
            return Json(new
            {
                options = pages
            });
        }
        public IActionResult GetElementTypes()
        {
            try
            {
                var testElementTypes = _unitOfWork.TestElementTypes.GetAll(x => x.Status.Equals("ACTV")).ToList();
                var result = (from tet in testElementTypes
                              select new
                              {
                                  testElementId = tet.TestElementTypeId,
                                  testElementType = tet.TestElementType1,
                              }).ToList();
                return Json(new { resultSet = result });
            }
            catch (Exception e)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                _logger.LogError(e, $"Path: {controllerName + "/" + actionName}\n" + e.Message);
                return StatusCode(500, e);
            }
        }

        //get element data to display dropdown
        [HttpPost]
        public IActionResult GetConfiguredElements([FromBody] long pageId)
        {
            var result = _unitOfWork.PageElements.GetAll(x => x.PageId == pageId && x.Status == "ACTV").OrderBy(x => x.ElementOrder).ToList();
            return Json(new { resultSet = result });
        }

        public IActionResult GetTestCasesForApplication(long applicationId, string testCaseName)
        {

            //return Json(new { resultSet = result });

            try
            {
                if (testCaseName != null && applicationId != 0)
                {
                    var result = _unitOfWork.TestCases.GetAll(x => x.ApplicationId == applicationId && x.Status == "ACTV").ToList();
                    var filterOptions = result.Where(s => (s.TestCaseName.ToLower().Replace(" ", "").Contains(testCaseName.ToLower().Replace(" ", "")))
                                                        ).ToList();
                    return Json(new { options = filterOptions.Take(10) });
                }
                return Json(new { options = false });
            }
            catch (Exception e)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                _logger.LogError(e, $"Path: {controllerName + "/" + actionName}\n" + e.Message);

                return Json(new { options = false });
            }

        }

        [HttpPost]
        public IActionResult GetPageDefinitions([FromBody] long applicationId)
        {
            var result = _unitOfWork.Pages.GetAll(x => x.ApplicationId == applicationId).ToList();
            return Json(new { resultSet = result });
        }
        [HttpPost]
        public IActionResult GetPlansForApplication([FromBody] long applicationId)
        {
            var result = _unitOfWork.TestPlans.GetAll(x => x.ApplicationId == applicationId).ToList();
            return Json(new { resultSet = result });
        }
        [HttpPost]
        public IActionResult GetExecutionCyclesForPlan([FromBody] long planId)
        {
            var result = _unitOfWork.TestExecutionCycles.GetAll(x => x.TestPlanId == planId).Select(x =>
            new
            {
                TestPlanId = x.TestPlanId,
                TestExecutionCycleId = x.TestExecutionCycleId,
                TestExecutionDate = x.TestExecutionDate,
                Status = x.Status,
                Comment = x.Comment
            }).ToList();
            return Json(new { resultSet = result });
        }
        [HttpPost]
        public IActionResult GetExecutedTestCaseData([FromBody] long executionCycleID)
        {
            var result = _unitOfWork.TestCaseExecutions.GetAll(x => x.TestExecutionCycleId == executionCycleID).ToList();
            return Json(new { resultSet = result });
        }

        [HttpPost]
        public IActionResult GetTestStepTemplates([FromBody] long pageId)
        {
            //var pages = _unitOfWork.Pages.GetAll(x => x.ApplicationId == applicationId).ToList();
            //var result=new List<StepTemplateInfoVM>();
            //foreach (var page in pages)
            //{
            //    result.AddRange(_unitOfWork.TestStepTemplates.GetAll(x => x.PageId == page.PageId).
            //        Select(x=> new StepTemplateInfoVM()
            //            {
            //                TestStepTemplateID=x.TestStepTemplateId,
            //                TestStepTemplateName=x.TestStepName
            //            }
            //        ).ToList());
            //}

            var result = _unitOfWork.TestStepTemplates.GetAll(x => x.PageId == pageId).ToList();
            return Json(new { resultSet = result });
        }

        [HttpPost]
        public IActionResult GetTestStepTemplateData([FromBody] long templateId)
        {
            var stepTemplate = _unitOfWork.TestStepTemplates.GetAll(x => x.TestStepTemplateId == templateId).FirstOrDefault();
            var stepValuesTemplates = _unitOfWork.TestStepValueTemplates.GetAll(x => x.TestStepTemplateId == templateId).ToList();
            var expectedResultTempaltes = _unitOfWork.TestExpectedResultTemplates.GetAll(x => x.TestStepTemplateId == templateId).ToList();
            var pageDetailsForStepTemplate = _unitOfWork.Pages.Get(x => x.PageId == stepTemplate.PageId).First();


            Dictionary<string, dynamic> result = new Dictionary<string, dynamic>();

            result["step"] = stepTemplate;
            result["stepValues"] = stepValuesTemplates;
            result["expectedResult"] = expectedResultTempaltes;
            result["pageData"] = pageDetailsForStepTemplate;

            return Json(new { resultSet = result });
        }






        //Creating Data functions
        [HttpPost]
        public async Task<IActionResult> CreateProject([FromBody] ProjectVM projectVM)
        {
            Project project = new Project();
            try
            {
                project.ProjectId = await getNextId();
                project.ProjectName = projectVM.ProjectName;
                project.ProjectDescription = projectVM.ProjectDescription;
                project.CreatedBy = projectVM.CreatedBy;
                project.CreatedDate = DateTime.Now;
                project.Status = "ACTV";
                project.StatusChangeDate = DateTime.Now;
                _unitOfWork.Projects.Add(project);
                if (SaveValuesInDB() == true)
                {
                    return Json(new { statusCode = 1, message = "Project created succesfully." });

                }
                else
                {
                    return Json(new { statusCode = 0, message = "Project is not created." });
                }
            }
            catch (Exception e)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                _logger.LogError(e, $"Path: {controllerName + "/" + actionName}\n" + e.Message);

                return StatusCode(500, e);
            }
        }

        //function to create application
        [HttpPost]
        public async Task<IActionResult> CreateApplication([FromBody] ApplicationVM applicationVM)
        {
            Application application = new Application();
            try
            {
                if (applicationVM != null)
                {


                    application.ApplicationId = await getNextId();
                    application.ProjectId = (long)applicationVM.projectId;
                    application.ApplicationName = applicationVM.applicationName;
                    application.ApplicationVersion = applicationVM.applicationVersion;
                    application.ApplicationDescription = applicationVM.applicationDescription;
                    application.CreatedBy = applicationVM.createdBy;
                    application.CreatedDate = DateTime.Now;
                    application.Status = "ACTV";
                    application.StatusChangeDate = DateTime.Now;
                    _db.Applications.Add(application);
                }
                if (SaveValuesInDB() == true)
                {
                    return Json(new { statusCode = 1, message = "Application created succesfully." });

                }
                else
                {
                    return Json(new { statusCode = 0, message = "Application is not created." });
                }

            }
            catch (Exception e)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                _logger.LogError(e, $"Path: {controllerName + "/" + actionName}\n" + e.Message);

                return StatusCode(500, e);
            }
        }



        //[HttpPost]
        //public async Task<IActionResult> CreatePageDefination([FromBody] PageDefinitionVM pageDefinitionVM)
        //{
        //    Page page = new Page();
        //    try
        //    {
        //        if (pageDefinitionVM.pageId == null || pageDefinitionVM.pageId == 0)
        //        {
        //            page.PageId = await getNextId();
        //            page.ApplicationId = (long)pageDefinitionVM.applicationId;
        //            page.PageName = pageDefinitionVM.pageName;
        //            page.PageUrl = pageDefinitionVM.pageUrl;
        //            page.DefinitionVersion = pageDefinitionVM.definitionVersion;
        //            page.Comments = pageDefinitionVM.comment;
        //            page.Status = "ACTV";
        //            page.StatusChangeDate = DateTime.Now;
        //            _unitOfWork.Pages.Add(page);
        //        }
        //        else
        //        {
        //            //page.PageId = (long)pageDefinitionVM.pageId;


        //            page = _unitOfWork.Pages.Get(x => x.PageId == pageDefinitionVM.pageId && x.Status == "ACTV").First();
        //            page.ApplicationId = (long)pageDefinitionVM.applicationId;
        //            page.PageName = pageDefinitionVM.pageName;
        //            page.PageUrl = pageDefinitionVM.pageUrl;
        //            page.DefinitionVersion = pageDefinitionVM.definitionVersion;
        //            page.Comments = pageDefinitionVM.comment;
        //            page.Status = "ACTV";
        //            page.StatusChangeDate = DateTime.Now;
        //            _db.Pages.Update(page);
        //        }
        //        if (SaveValuesInDB() == true)
        //        {
        //            return Json(new { statusCode = 1, pageId = page.PageId });
        //        }
        //        else
        //        {
        //            return Json(new { statusCode = 0, message = "Application is not created." });
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
        //        string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
        //        _logger.LogError(e, $"Path: {controllerName + "/" + actionName}\n" + e.Message);

        //        return StatusCode(500, e);
        //    }
        //}
        //[HttpPost]
        //public async Task<IActionResult> CreatePageElements([FromBody] List<PageElementsVM> pageElementsVMlist)
        //{
        //    PageElement pageElement = new PageElement();
        //    try
        //    {
        //        foreach (var pageElementVM in pageElementsVMlist)
        //        {

        //            if (pageElementVM.pageElementId == 0 || pageElementVM.pageElementId == null)
        //            {
        //                pageElement.PageElementId = await getNextId();
        //                pageElement.PageId = (long)pageElementVM.pageId;
        //                pageElement.ElementName = pageElementVM.elementName;
        //                pageElement.ElementType = pageElementVM.elementType;
        //                pageElement.ElementAddress = pageElementVM.elementXpath;
        //                pageElement.ElementOrder = pageElementVM.elementOrder;
        //                pageElement.Status = "ACTV";
        //                pageElement.StatusChangeDate = DateTime.Now;
        //                _unitOfWork.PageElements.Add(pageElement);
        //            }
        //            else
        //            {
        //                pageElement = _unitOfWork.PageElements.Get(x => x.PageElementId == pageElementVM.pageElementId && x.Status == "ACTV").First();
        //                pageElement.PageId = (long)pageElementVM.pageId;
        //                pageElement.ElementName = pageElementVM.elementName;
        //                pageElement.ElementType = pageElementVM.elementType;
        //                pageElement.ElementAddress = pageElementVM.elementXpath;
        //                pageElement.ElementOrder = pageElementVM.elementOrder;
        //                pageElement.Status = "ACTV";
        //                pageElement.StatusChangeDate = DateTime.Now;
        //                _db.PageElements.Update(pageElement);
        //            }
        //        }

        //        if (SaveValuesInDB() == true)
        //        {
        //            return Json(new { statusCode = 1, message = "Page Definition added succesfully." });
        //        }
        //        else
        //        {
        //            return Json(new { statusCode = 0, message = "Page Definition is not created." });
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
        //        string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
        //        _logger.LogError(e, $"Path: {controllerName + "/" + actionName}\n" + e.Message);

        //        return StatusCode(500, e);
        //    }
        //}


        //This function add and update page definition and their elements data
        [HttpPost]
        public async Task<IActionResult> AddUpdatePageDefinitionElements([FromBody] PageDefinitionVM pageDefObj)
        {
            try
            {
                if (pageDefObj != null)
                {
                    if (pageDefObj.operation == "Add")
                    {
                        var isDataExists = _unitOfWork.Pages.Get(x => x.PageName.ToLower() == pageDefObj.pageName.ToLower() && x.ApplicationId == pageDefObj.applicationId).FirstOrDefault();

                        if (isDataExists != null)
                        {
                            return Json(new { statusCode = 2 });
                        }

                        long pageId = await getNextId();

                        _unitOfWork.Pages.Add(new Page
                        {
                            PageId = pageId,
                            ApplicationId = pageDefObj.applicationId,
                            PageName = pageDefObj.pageName,
                            PageUrl = pageDefObj.pageUrl,
                            Status = "ACTV",
                            StatusChangeDate = DateTime.Now,
                            Comments = pageDefObj.comment,
                            DefinitionVersion = pageDefObj.definitionVersion
                        });

                        if (SaveValuesInDB() == true)
                        {
                            foreach (var element in pageDefObj.elementDetails)
                            {
                                long pageElementId = await getNextId();

                                _unitOfWork.PageElements.Add(new PageElement
                                {
                                    PageElementId = pageElementId,
                                    PageId = pageId,
                                    ElementName = element.elementName,
                                    ElementType = element.elementType,
                                    ElementAddress = element.elementXpath,
                                    ElementOrder = element.elementOrder,
                                    Status = "ACTV",
                                    StatusChangeDate = DateTime.Now,
                                });
                            }
                        }

                        else
                        {
                            return Json(new { statusCode = 0 });
                        }

                    }

                    else if (pageDefObj.operation == "Modify")
                    {
                        //update page details
                        var pageData = _unitOfWork.Pages.Get(x => x.PageId == pageDefObj.pageId && x.ApplicationId == pageDefObj.applicationId).FirstOrDefault();

                        if (pageDefObj == null)
                        {
                            return Json(new { statusCode = 0 });
                        }

                        //check definition version update
                        if (pageData.DefinitionVersion != pageDefObj.definitionVersion)
                        {
                            pageData.DefinitionVersion = pageDefObj.definitionVersion;
                        }

                        //check page url update
                        if (pageData.PageUrl != pageDefObj.pageUrl)
                        {
                            pageData.PageUrl = pageDefObj.pageUrl;
                        }

                        //check page description
                        if (pageData.Comments != "" && pageData.Comments != null)
                        {
                            if (pageData.Comments != pageDefObj.comment)
                            {
                                pageData.Comments = pageDefObj.comment;
                            }
                        }


                        //Elements Details

                        //get max element order
                        long maxElementOrder = _unitOfWork.PageElements.Get(x => x.PageId == pageDefObj.pageId && x.Status == "ACTV").OrderByDescending(x => x.ElementOrder).First().ElementOrder;

                        foreach (var element in pageDefObj.elementDetails)
                        {
                            //add elements
                            if (element.isNewElement == "true")
                            {
                                long pageElementId = await getNextId();
                                _unitOfWork.PageElements.Add(new PageElement
                                {
                                    PageElementId = pageElementId,
                                    PageId = (long)pageDefObj.pageId,
                                    ElementName = element.elementName,
                                    ElementType = element.elementType,
                                    ElementAddress = element.elementXpath,
                                    ElementOrder = element.elementOrder,
                                    Status = "ACTV",
                                    StatusChangeDate = DateTime.Now,
                                });
                            }
                            //update elements
                            else if (element.isNewElement == "false")
                            {
                                //change order for existing elements
                                if (element.existingElementOrder != element.elementOrder)
                                {
                                    var elementDetails = _unitOfWork.PageElements.Get(x => x.PageElementId == element.pageElementId && x.PageId == pageDefObj.pageId && x.Status == "ACTV").First();

                                    elementDetails.ElementOrder = element.elementOrder;
                                    elementDetails.StatusChangeDate = DateTime.Now;
                                }
                            }
                        }



                        //inactive elements
                        foreach (var id in pageDefObj.removeExistingElements)
                        {
                            var elementDetail = _unitOfWork.PageElements.Get(x => x.PageElementId == id).First();

                            elementDetail.Status = "INAC";
                            elementDetail.StatusChangeDate = DateTime.Now;
                        }
                    }

                }

                if (SaveValuesInDB() == true)
                {
                    return Json(new { statusCode = 1 });
                }
                else
                {
                    return Json(new { statusCode = 0 });
                }
            }
            catch (Exception e)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                _logger.LogError(e, $"Path: {controllerName + "/" + actionName}\n" + e.Message);

                return StatusCode(500, e);
            }
        }


        [HttpPost]

        public async Task<IActionResult> AddElementType([FromBody] string newType)
        {
            try
            {
                TestElementType testElementType = new TestElementType();

                testElementType.TestElementTypeId = await getNextId();
                testElementType.TestElementType1 = newType.Trim();
                testElementType.Status = "ACTV";
                testElementType.StatusChangeDate = DateTime.Now;
                _unitOfWork.TestElementTypes.Add(testElementType);
                if (SaveValuesInDB() == true)
                {
                    return Json(new { statusCode = 1, message = "Element type was created succesfully." });

                }
                else
                {
                    return Json(new { statusCode = 0, message = "Element type is not created." });
                }
            }
            catch (Exception e)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                _logger.LogError(e, $"Path: {controllerName + "/" + actionName}\n" + e.Message);

                return StatusCode(500, e);

            }
        }
        [HttpPost]

        public async Task<IActionResult> RemoveElementType([FromBody] long typeId)
        {
            try
            {
                var result = _unitOfWork.TestElementTypes.GetAll(x => x.TestElementTypeId == typeId).First();
                TestElementType testElementType = result;
                testElementType.Status = "INAC";
                testElementType.StatusChangeDate = DateTime.Now;
                _db.TestElementTypes.Update(testElementType);
                if (SaveValuesInDB() == true)
                {
                    return Json(new { statusCode = 1, message = "Element type was removed succesfully." });

                }
                else
                {
                    return Json(new { statusCode = 0, message = "Element type is not removed." });
                }
            }
            catch (Exception e)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                _logger.LogError(e, $"Path: {controllerName + "/" + actionName}\n" + e.Message);

                return StatusCode(500, e);

            }
        }



        //Updation Functions



        //Update status (ACTV / INAC) of application
        [HttpPost]
        public IActionResult UpdateProjectStatus([FromBody] ProjectVM projectVM)
        {
            try
            {
                var result = _unitOfWork.Projects.GetAll(x => x.ProjectId == projectVM.Id).First();
                //Project project = result[0];
                result.Status = projectVM.status;
                result.StatusChangeDate = DateTime.Now;
                _db.Projects.Update(result);
                if (SaveValuesInDB() == true)
                {
                    return Json(new { statusCode = 1, message = "Project status changed succesfully." });

                }
                else
                {
                    return Json(new { statusCode = 0, message = "Project status is not changed." });
                }
            }
            catch (Exception e)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                _logger.LogError(e, $"Path: {controllerName + "/" + actionName}\n" + e.Message);

                return StatusCode(500, e);

            }
        }

        //Update status (ACTV / INAC) of application
        [HttpPost]
        public IActionResult UpdateApplicationStatus([FromBody] ApplicationVM applicationVM)
        {
            try
            {
                var result = _unitOfWork.Applications.Get(x => x.ApplicationId == applicationVM.Id).First();
                //Application application = result;
                result.Status = applicationVM.status;
                result.StatusChangeDate = DateTime.Now;
                _db.Applications.Update(result);
                if (SaveValuesInDB() == true)
                {
                    return Json(new { statusCode = 1, message = "Application status changed succesfully." });

                }
                else
                {
                    return Json(new { statusCode = 0, message = "Application status is not changed." });
                }
            }
            catch (Exception e)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                _logger.LogError(e, $"Path: {controllerName + "/" + actionName}\n" + e.Message);

                return StatusCode(500, e);

            }
        }

        [HttpGet]

        public IActionResult UpdateElementType(long typeId, string newType)
        {
            try
            {
                var result = _unitOfWork.TestElementTypes.GetAll(x => x.TestElementTypeId == typeId).First();
                TestElementType testElementType = result;
                testElementType.TestElementType1 = newType.Trim();
                testElementType.StatusChangeDate = DateTime.Now;
                _db.TestElementTypes.Update(testElementType);
                if (SaveValuesInDB() == true)
                {
                    return Json(new { statusCode = 1, message = "Element type name was updated succesfully." });

                }
                else
                {
                    return Json(new { statusCode = 0, message = "Element type name is not changed." });
                }
            }
            catch (Exception e)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                _logger.LogError(e, $"Path: {controllerName + "/" + actionName}\n" + e.Message);

                return StatusCode(500, e);

            }
        }





        //save values of step template
        [HttpPost]
        public async Task<IActionResult> SaveStepTemplate([FromBody] TestStepTemplateVM testStepTemplate)
        {
            bool isStepTemplateInsert = false;
            bool isStepElementInsert = false;
            bool isExpectedResultInsert = false;
            try
            {
                if (testStepTemplate != null)
                {
                    var isDataExists = _unitOfWork.TestStepTemplates.Get(x => x.TestStepName == testStepTemplate.stepName && x.PageId == testStepTemplate.pageId).FirstOrDefault();

                    if (isDataExists != null)
                    {
                        return Json(new { statusCode = 2 });
                    }

                    long stepTemplateId = await getNextId();

                    _unitOfWork.TestStepTemplates.Add(new TestStepTemplate
                    {
                        TestStepTemplateId = stepTemplateId,
                        TestStepName = testStepTemplate.stepName,
                        PageId = testStepTemplate.pageId,
                        Status = "ACTV",
                        StatusChangeDate = DateTime.Now,
                        Comments = testStepTemplate.description,
                    });

                    if (SaveValuesInDB() == true)
                    {
                        isStepTemplateInsert = true;
                    }

                    if (isStepTemplateInsert == true)
                    {
                        foreach (var element in testStepTemplate.elementDetails)
                        {

                            long testStepTemplateValueId = await getNextId();

                            _unitOfWork.TestStepValueTemplates.Add(new TestStepValueTemplate
                            {
                                TestStepValueTemplateId = testStepTemplateValueId,
                                TestStepTemplateId = stepTemplateId,
                                PageElementId = element.elementId,
                                ElementValue = element.elementValue,
                                ExecutionOrder = element.elementOrder,
                                Status = "ACTV",
                                StatusChangeDate = DateTime.Now,
                                Comments = element.elementComments
                            });
                        }


                        foreach (var expectedResult in testStepTemplate.expectedResultDetails)
                        {
                           
                            long testExpectedResultTemplateId = await getNextId();


                            _unitOfWork.TestExpectedResultTemplates.Add(new TestExpectedResultTemplate
                            {
                                TestExpectedResultTemplateId = testExpectedResultTemplateId,
                                TestStepTemplateId = stepTemplateId,
                                PageId = expectedResult.pageId,
                                PageElementId = (expectedResult.elementId == 0) ? null: expectedResult.elementId,
                                ElementValue = expectedResult.elementValue,
                                ExpectedResultOrder = expectedResult.expectedResultOrder,
                                Description = expectedResult.comments,
                                Status = "ACTV",
                                StatusChangeDate = DateTime.Now,
                                Comments = expectedResult.comments,
                                CheckParameters = expectedResult.elementParameter,
                            }) ;
                        }

                    }
                    else
                    {
                        return Json(new { statusCode = 0 });
                    }
                }

                if (SaveValuesInDB() == true)
                {
                    return Json(new { statusCode = 1 });
                }
                else
                {
                    return Json(new { statusCode = 0 });

                }
            }
            catch (Exception e)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                _logger.LogError(e, $"Path: {controllerName + "/" + actionName}\n" + e.Message);

                return StatusCode(500, e);
            }
        }


        //function to save test plan
        [HttpPost]
        public async Task<IActionResult> SaveTestPlan([FromBody] TestPlanVM testPlan)
        {

            try
            {
                if (testPlan != null)
                {
                    long testPlanId = await getNextId();


                    _unitOfWork.TestPlans.Add(new TestPlan
                    {
                        TestPlanId = testPlanId,
                        ApplicationId = testPlan.applicationId,
                        TestPlanName = testPlan.planName,
                        PlanShortDescription = testPlan.shortDescription,
                        PlanObjective = testPlan.objective,
                        Status = "ACTV",
                        StatusChangeDate = DateTime.Now,
                    });

                    if (SaveValuesInDB() == true)
                    {

                        foreach (var testCase in testPlan.testCaseDetails)
                        {
                            long testPlanDetailsId = await getNextId();

                            _unitOfWork.TestPlanDetails.Add(new TestPlanDetail
                            {
                                TestPlanDetailId = testPlanDetailsId,
                                TestPlanId = testPlanId,
                                TestCaseId = testCase.testCaseId,
                                TestCaseOrder = testCase.testCaseOrder,
                                Status = "ACTV",
                                StatusChangeDate = DateTime.Now,

                            });
                        }
                    }
                    else
                    {
                        return Json(new { statusCode = 0 });
                    }
                }

                if (SaveValuesInDB() == true)
                {
                    return Json(new { statusCode = 1 });
                }
                else
                {
                    return Json(new { statusCode = 0 });
                }
            }
            catch (Exception e)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                _logger.LogError(e, $"Path: {controllerName + "/" + actionName}\n" + e.Message);

                return StatusCode(500, e);
            }
        }

        //get plan details on search
        [HttpGet]
        public async Task<IActionResult> GetPlanDetailsOnSearch(string substring, long applicationId)
        {
            try
            {

                var planList = _unitOfWork.TestPlans.GetAll(x => x.ApplicationId == applicationId && x.Status == "ACTV").ToList();
                var plans = planList.Where(
                                   pa => (pa.TestPlanName != null ? pa.TestPlanName.ToLower().Replace(" ", "").Contains(substring.ToLower().Replace(" ", "")) : false)
                                && pa.Status == "ACTV").ToList();
                return Json(new
                {
                    options = plans
                });
            }
            catch (Exception e)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                _logger.LogError(e, $"Path: {controllerName + "/" + actionName}\n" + e.Message);

                return StatusCode(500, e);
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetStepTemplateDetailsOnSearch(string substring, long applicationId)
        {
            try
            {
                var pageIdList = _unitOfWork.Pages.GetAll(x => x.ApplicationId == applicationId).ToList().Select(x => x.PageId);

                var stepTemplateList = _unitOfWork.TestStepTemplates.GetAll(x => pageIdList.Contains(x.PageId) && x.Status == "ACTV").ToList();
                var plans = stepTemplateList.Where(
                                   pa => (pa.TestStepName != null ? pa.TestStepName.ToLower().Replace(" ", "").Contains(substring.ToLower().Replace(" ", "")) : false)
                                && pa.Status == "ACTV").ToList();
                return Json(new
                {
                    options = plans
                });
            }
            catch (Exception e)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                _logger.LogError(e, $"Path: {controllerName + "/" + actionName}\n" + e.Message);

                return StatusCode(500, e);
            }
        }

        //return next sequence
        public async Task<long> getNextId()
        {
            string data = "";
            string npgCmd = "select nextval('dsqa.dsqaseq');";
            string? connectionString = _configuration.GetConnectionString("DefaultConnection");
            using (var connection = new NpgsqlConnection(connectionString))
            {
                await connection.OpenAsync();
                using (var command = new NpgsqlCommand(npgCmd, connection))
                {
                    NpgsqlDataReader dr = command.ExecuteReader();
                    // Output rows
                    while (dr.Read())
                        data = dr[0].ToString();
                }
            }
            long id = long.Parse(data);
            nextIdCalled = true;
            return id;

        }


        //get test case data
        public async Task<IActionResult> GetTestCaseData(long testCaseId)
        {
            try
            {

                var testCaseObj = _unitOfWork.TestCases.Get(x => x.TestCaseId == testCaseId && x.Status == "ACTV").First();


                return Json(new { statusCode = 1, result = testCaseObj });
            }
            catch (Exception e)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                _logger.LogError(e, $"Path: {controllerName + "/" + actionName}\n" + e.Message);

                return StatusCode(500, e);
            }
        }



        //get test plan data
        public async Task<IActionResult> GetTestPlanData(long testPlanId)
        {
            try
            {
                var testPlanObj = _unitOfWork.TestPlans.Get(x => x.TestPlanId == testPlanId && x.Status == "ACTV").ToList();   //return only one record            
                var testPlanDetails = _unitOfWork.TestPlanDetails.GetAll(x => x.TestPlanId == testPlanId).ToList();
                var testCaseList = _unitOfWork.TestCases.GetAll(x => x.ApplicationId == testPlanObj.First().ApplicationId && x.Status == "ACTV").ToList();

                var resultSet = (
                                from tp in testPlanObj
                                select new
                                {
                                    TestPlanId = tp.TestPlanId,
                                    ApplicationId = tp.ApplicationId,
                                    TestPlanName = tp.TestPlanName,
                                    PlanShortDescription = tp.PlanShortDescription,
                                    PlanObjective = tp.PlanObjective,
                                    TestCaseDetails = (from tpd in testPlanDetails
                                                       join tc in testCaseList on tpd.TestCaseId equals tc.TestCaseId
                                                       select new
                                                       {
                                                           TestCaseId = tc.TestCaseId,
                                                           ApplicationId = tc.ApplicationId,
                                                           TestCaseName = tc.TestCaseName,
                                                           TestCaseShortDescription = tc.TestCaseShortDescription,
                                                           TestCaseObjective = tc.TestCaseObjective,
                                                           TestCaseOrder = tpd.TestCaseOrder,
                                                       }).ToList().OrderBy(x => x.TestCaseOrder),
                                }).ToList();

                return Json(new { statusCode = 1, result = resultSet });
            }
            catch (Exception e)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                _logger.LogError(e, $"Path: {controllerName + "/" + actionName}\n" + e.Message);

                return StatusCode(500, e);
            }
        }

        public async Task<IActionResult> CreateTestCase([FromBody] TestCaseVM testCaseVM)
        {
            bool isTestCaseinsert = false;
            bool isStepTemplateInsert = false;
            bool isStepElementInsert = false;
            bool isExpectedResultInsert = false;
            try
            {
                if (testCaseVM != null)
                {
                    var isDataExists = _unitOfWork.TestCases.Get(x => x.TestCaseName == testCaseVM.testCaseName && x.ApplicationId == testCaseVM.applicationId).FirstOrDefault();

                    if (isDataExists != null)
                    {
                        return Json(new { statusCode = 2 });
                    }
                    long testCaseId = await getNextId();

                    _unitOfWork.TestCases.Add(new TestCase
                    {
                        TestCaseId = testCaseId,
                        ApplicationId = testCaseVM.applicationId,
                        TestCaseName = testCaseVM.testCaseName,
                        TestCaseShortDescription = testCaseVM.testCaseShortDescription,
                        TestCaseObjective = testCaseVM.testCaseObjective,
                        Status = testCaseVM.status,
                        StatusChangeDate = DateTime.Now
                    });
                    if (SaveValuesInDB() == true)
                    {
                        isTestCaseinsert = true;
                    }
                    if (isTestCaseinsert == true)
                    {
                        int order = 0;
                        foreach (var testStepPlanVM in testCaseVM.testCaseSteps)
                        {
                            long stepPlanId = await getNextId();
                            order++;
                            _unitOfWork.TestStepPlans.Add(new TestStepPlan
                            {
                                TestStepPlanId = stepPlanId,
                                TestCaseId = testCaseId,
                                TestStepName = testStepPlanVM.Value.stepName,
                                PageId = testStepPlanVM.Value.pageId,
                                Status = "ACTV",
                                StatusChangeDate = DateTime.Now,
                                TestStepOrder = order,
                                Comments = testStepPlanVM.Value.description
                            });

                            if (SaveValuesInDB() == true)
                            {
                                isStepTemplateInsert = true;
                            }

                            if (isStepTemplateInsert == true)
                            {
                                foreach (var element in testStepPlanVM.Value.elementDetails)
                                {

                                    long testStepPlanValueId = await getNextId();
                                    

                                    _unitOfWork.TestStepValuePlans.Add(new TestStepValuePlan
                                    {
                                        TestStepValuePlanId = testStepPlanValueId,
                                        TestStepPlanId = stepPlanId,
                                        PageElementId = element.pageElementId,
                                        ElementValue = element.elementValue,
                                        ExecutionOrder = element.executionOrder,
                                        Status = "ACTV",
                                        StatusChangeDate = DateTime.Now,
                                        Comments = element.comments
                                    });
                                }


                                foreach (var expectedResult in testStepPlanVM.Value.expectedResultDetails)
                                {
                                    long testExpectedResultPlanId = await getNextId();

                                    _unitOfWork.TestExpectedResultPlans.Add(new TestExpectedResultPlan
                                    {
                                        TestExpectedResultPlanId = testExpectedResultPlanId,
                                        TestStepPlanId = stepPlanId,
                                        PageId = expectedResult.pageId,
                                        PageElementId = expectedResult.elementId == 0 ? null : expectedResult.elementId,
                                        ElementValue = expectedResult.elementValue,
                                        ExpectedResultOrder = expectedResult.expectedResultOrder,
                                        Description = expectedResult.description,
                                        Status = "ACTV",
                                        StatusChangeDate = DateTime.Now,
                                    });
                                }

                            }
                            else
                            {
                                return Json(new { statusCode = 0 });
                            }
                        }
                    }
                    else
                    {
                        return Json(new { statusCode = 0 });
                    }
                }
                if (SaveValuesInDB() == true)
                {
                    return Json(new { statusCode = 1 });
                }
                else
                {
                    return Json(new { statusCode = 0 });

                }
            }
            catch (Exception e)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                _logger.LogError(e, $"Path: {controllerName + "/" + actionName}\n" + e.Message);

                return StatusCode(500, e);
            }
        }





        //public async Task<IActionResult> CreateTestCase([FromBody] TestCaseVM testCaseVM)
        //{
        //    bool isTestCaseinsert = false;
        //    bool isStepTemplateInsert = false;
        //    bool isStepElementInsert = false;
        //    bool isExpectedResultInsert = false;
        //    try
        //    {
        //        TestCase testCaseObj = new TestCase();
        //        TestStepPlan testStepPlanObj = new TestStepPlan();

        //        if (testCaseVM != null)
        //        {
        //            var isDataExists = _unitOfWork.TestCases.Get(x => x.TestCaseName == testCaseVM.testCaseName && x.ApplicationId == testCaseVM.applicationId).FirstOrDefault();

        //            if (isDataExists != null)
        //            {
        //                return Json(new { statusCode = 2 });
        //            }
        //            long testCaseId = await getNextId();

        //            //_unitOfWork.TestCases.Add(new TestCase
        //            //{
        //            //    TestCaseId = testCaseId,
        //            //    ApplicationId = testCaseVM.applicationId,
        //            //    TestCaseName = testCaseVM.testCaseName,
        //            //    TestCaseShortDescription = testCaseVM.testCaseShortDescription,
        //            //    TestCaseObjective = testCaseVM.testCaseObjective,
        //            //    Status = testCaseVM.status,
        //            //    StatusChangeDate = DateTime.Now
        //            //});

        //            testCaseObj.TestCaseId = testCaseId;
        //            testCaseObj.ApplicationId = testCaseVM.applicationId;
        //            testCaseObj.TestCaseName = testCaseVM.testCaseName;
        //            testCaseObj.TestCaseShortDescription = testCaseVM.testCaseShortDescription;
        //            testCaseObj.TestCaseObjective = testCaseVM.testCaseObjective;
        //            testCaseObj.Status = testCaseVM.status;
        //            testCaseObj.StatusChangeDate = DateTime.Now;



        //            int order = 0;
        //            foreach (var testStepPlanVM in testCaseVM.testCaseSteps)
        //            {
        //                long stepPlanId = await getNextId();
        //                order++;
        //                //_unitOfWork.TestStepPlans.Add(new TestStepPlan
        //                //{
        //                //    TestStepPlanId = stepPlanId,
        //                //    TestCaseId = testCaseId,
        //                //    TestStepName = testStepPlanVM.Value.stepName,
        //                //    PageId = testStepPlanVM.Value.pageId,
        //                //    Status = "ACTV",
        //                //    StatusChangeDate = DateTime.Now,
        //                //    TestStepOrder = order,
        //                //    Comments = testStepPlanVM.Value.description
        //                //});




        //                foreach (var element in testStepPlanVM.Value.elementDetails)
        //                {

        //                    long testStepPlanValueId = await getNextId();

        //                    _unitOfWork.TestStepValuePlans.Add(new TestStepValuePlan
        //                    {
        //                        TestStepValuePlanId = testStepPlanValueId,
        //                        TestStepPlanId = stepPlanId,
        //                        PageElementId = element.pageElementId,
        //                        ElementValue = element.elementValue,
        //                        ExecutionOrder = element.executionOrder,
        //                        Status = "ACTV",
        //                        StatusChangeDate = DateTime.Now,
        //                        Comments = element.comments
        //                    });
        //                }


        //                foreach (var expectedResult in testStepPlanVM.Value.expectedResultDetails)
        //                {
        //                    long testExpectedResultPlanId = await getNextId();

        //                    _unitOfWork.TestExpectedResultPlans.Add(new TestExpectedResultPlan
        //                    {
        //                        TestExpectedResultPlanId = testExpectedResultPlanId,
        //                        TestStepPlanId = stepPlanId,
        //                        PageId = expectedResult.pageId,
        //                        PageElementId = expectedResult.elementId,
        //                        ElementValue = expectedResult.elementValue,
        //                        ExpectedResultOrder = expectedResult.expectedResultOrder,
        //                        Description = expectedResult.description,
        //                        Status = "ACTV",
        //                        StatusChangeDate = DateTime.Now,
        //                    });
        //                }

        //            }


        //        }




        //        if (SaveValuesInDB() == true)
        //        {
        //            return Json(new { statusCode = 1 });
        //        }
        //        else
        //        {
        //            return Json(new { statusCode = 0 });

        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
        //        string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
        //        _logger.LogError(e, $"Path: {controllerName + "/" + actionName}\n" + e.Message);

        //        return StatusCode(500, e);
        //    }
        //}







        //get data for project and application of each page
        public async Task<CommonVM> getCommonVM(long projectId, long applicationId)
        {
            CommonVM commonVm = new CommonVM();
            Project project = new Project();
            Application application = new Application();
            project = _unitOfWork.Projects.GetAll(x => x.ProjectId == projectId).First();
            //project = _unitOfWork.Projects.Where(x => x.ProjectId == projectId).FirstOrDefault();
            application = _unitOfWork.Applications.GetAll(x => x.ApplicationId == applicationId).First();
            commonVm.ProjectId = projectId;
            commonVm.ProjectName = project.ProjectName;
            commonVm.ApplicationId = applicationId;
            commonVm.ApplicationName = application.ApplicationName;
            commonVm.ApplicationVersion = application.ApplicationVersion;
            return commonVm;
        }







        //save values in database
        public bool SaveValuesInDB()
        {
            try
            {
                _unitOfWork.Save();
                return true;

            }
            catch (Exception e)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                _logger.LogError(e, $"Path: {controllerName + "/" + actionName}\n" + e.Message);
                return false;
            }

        }

    }
}




